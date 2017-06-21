using System;
using Newtonsoft.Json;

namespace PrEmergency
{
	public class GitHubPullRequest
	{
		[JsonProperty("url")]
		public string Url { get; set; }
		[JsonProperty("html_url")]
		public string HtmlUrl { get; set; }
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("number")]
		public int Number { get; set; }
		[JsonProperty("state")]
		public string State { get; set; }
		[JsonProperty("title")]
		public string Title { get; set; }
		[JsonProperty("user")]
		public GitHubUser User { get; set; }
		[JsonProperty("body")]
		public string Body { get; set; }
		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }
		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
		[JsonProperty("closed_at")]
		public string ClosedAt { get; set; }
		[JsonProperty("merged_at")]
		public string MergedAt { get; set; }
		[JsonProperty("assignees")]
		public GitHubUser[] Assignees { get; set; }
	}

	public class GitHubUser
	{
		[JsonProperty("login")]
		public string Login { get; set; }
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("url")]
		public string Url { get; set; }
		[JsonProperty("html_url")]
		public string HtmlUrl { get; set; }
	}
}