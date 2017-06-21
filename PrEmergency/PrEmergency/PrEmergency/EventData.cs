namespace PrEmergency
{
	public class EventData
	{
		public bool DryRun { get; set; }

		public string GitHubOAuthToken { get; set; }
		public string GitHubOrganizationName { get; set; }
		public string GitHubRepositoryName { get; set; }

		public string TargetGitHubUserNameCsv { get; set; }

		public string SlackWebhookUrl { get; set; }
	}
}