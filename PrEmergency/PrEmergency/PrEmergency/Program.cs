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
					GitHubOrganizationName = "eightcard",
					GitHubRepositoryName = "lk-linkknowledge",
					GitHubOAuthToken = "",
					TargetGitHubUserNameCsv = "kumaie,mtaniuchi,TakumaHarada,takashi-okumura,ynakagawa33,kenta-sugihara,tfukudan",
					SlackWebhookUrl = ""
				}, new DebugLambdaContext())
				.Wait();
		}
	}
}