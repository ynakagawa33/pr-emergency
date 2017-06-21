namespace PrEmergency
{
	internal class Program
	{
		/// <summary>
		///     Entry point for local debug
		/// </summary>
		private static void Main(string[] args)
		{
			// use custom LambdaContext to access local context logger
			new Function().FunctionHandler(new EventData
				{
					DryRun = true,
					GitHubOrganizationName = "",
					GitHubRepositoryName = "",
					GitHubOAuthToken = "",
					TargetGitHubUserNameCsv = "",
					SlackWebhookUrl = ""
				}, new DebugLambdaContext())
				.Wait();
		}
	}
}