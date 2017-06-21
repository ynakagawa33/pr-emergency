using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using JsonSerializer = Amazon.Lambda.Serialization.Json.JsonSerializer;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace PrEmergency
{
	public class Function
	{
		private static string Region => Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");
		private static string GitHubRootEndpoint => "https://api.github.com";
		private static string GitHubListPullRequestsEndpoint => "/repos/:owner/:repo/pulls";
		private static string GitHubPaginationQueryStringTemplate => "?per_page=100&page={0}";

		public async Task<dynamic> FunctionHandler(dynamic input, ILambdaContext context)
		{
			context.Logger.LogLine("PrEmergency started!");
			context.Logger.LogLine(Region);
			var rawJson = JsonConvert.SerializeObject(input);
			context.Logger.LogLine(rawJson);
			var eventData = (EventData) JsonConvert.DeserializeObject<EventData>(rawJson);

			if (string.IsNullOrEmpty(eventData.GitHubOAuthToken))
			{
				context.Logger.LogLine(
					$"Input json detected not contain {nameof(eventData.GitHubOAuthToken)} key. return immediately.");
				throw new ArgumentException(nameof(eventData.GitHubOAuthToken));
			}
			if (string.IsNullOrEmpty(eventData.GitHubOrganizationName))
			{
				context.Logger.LogLine(
					$"Input json detected not contain {nameof(eventData.GitHubOrganizationName)} key. return immediately.");
				throw new ArgumentException(nameof(eventData.GitHubOrganizationName));
			}
			if (string.IsNullOrEmpty(eventData.GitHubRepositoryName))
			{
				context.Logger.LogLine(
					$"Input json detected not contain {nameof(eventData.GitHubRepositoryName)} key. return immediately.");
				throw new ArgumentException(nameof(eventData.GitHubRepositoryName));
			}
			if (string.IsNullOrEmpty(eventData.TargetGitHubUserNameCsv))
			{
				context.Logger.LogLine(
					$"Input json detected not contain {nameof(eventData.TargetGitHubUserNameCsv)} key. return immediately.");
				throw new ArgumentException(nameof(eventData.TargetGitHubUserNameCsv));
			}

			if (string.IsNullOrEmpty(eventData.SlackWebhookUrl))
			{
				context.Logger.LogLine(
					$"Input json detected not contain {nameof(eventData.SlackWebhookUrl)} key. return immediately.");
				throw new ArgumentException(nameof(eventData.SlackWebhookUrl));
			}


			var gitHubPullRequests = new List<GitHubPullRequest>();
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization =
					AuthenticationHeaderValue.Parse($"token {eventData.GitHubOAuthToken}");
				httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("pr-emergency");

				// Note that page numbering is 1-based and that omitting the ?page parameter will return the first page.
				var page = 1;
				while (true)
				{
					var url =
						GitHubRootEndpoint
						+ GitHubListPullRequestsEndpoint
							.Replace(":owner", eventData.GitHubOrganizationName)
							.Replace(":repo", eventData.GitHubRepositoryName)
						+ string.Format(GitHubPaginationQueryStringTemplate, page);

					var response = await httpClient.GetAsync(url);
					var gitHubPullRequestsByPageJson = await response.Content.ReadAsStringAsync();
					context.Logger.LogLine(gitHubPullRequestsByPageJson);
					var gitHubPullRequestsByPage = JsonConvert.DeserializeObject<List<GitHubPullRequest>>(gitHubPullRequestsByPageJson);

					if (gitHubPullRequestsByPage.Count == 0) break;
					gitHubPullRequests.AddRange(gitHubPullRequestsByPage);
					page++;
				}
			}

			var targetUsers = eventData.TargetGitHubUserNameCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			var targetPullRequests = gitHubPullRequests
				.Where(pr => pr.State == "open")
				.Where(pr => targetUsers.Contains(pr.User.Login))
				.Where(pr => DateTime.ParseExact(pr.CreatedAt, "yyyy-MM-ddTHH:mm:ssZ", null) < DateTime.UtcNow.AddDays(-7))
				.Where(pr => !pr.Title.Contains("[停]")) // レビューを停止している PR は対象に含めない
				.Where(pr => !pr.Title.Contains("[WIP]")) // 作業中の PR は対象に含めない
				.ToList();
			var postMessage =
				$@"{(eventData.DryRun ? "`@nux-dev`" : "<!subteam^S2WPQQU2F|nux-dev>")} PR が作成されてから、 7 日以上経過しています。可及的速やかに Close してください。"
				+ Environment.NewLine
				+ string.Join(Environment.NewLine, targetPullRequests.Select(pr => $@"*{pr.Title}* created by {pr.User.Login}"));

			if (targetPullRequests.Count == 0)
				return postMessage;

			using (var httpClient = new HttpClient())
			{
				var postJson = JsonConvert.SerializeObject(new
				{
					text = postMessage
				});

				using (var content = new StringContent(postJson, Encoding.UTF8, "application/json"))
				{
					var requestContentText = await content.ReadAsStringAsync();
					context.Logger.LogLine(requestContentText);
					var response = await httpClient.PostAsync(eventData.SlackWebhookUrl, content);
					var responseContentText = await response.Content.ReadAsStringAsync();
					context.Logger.LogLine($"{response.StatusCode}:{responseContentText}");
				}
			}

			return postMessage;
		}
	}
}