using System;
using Amazon.Lambda.Core;

namespace PrEmergency
{
	/// <summary>
	///     Implementation for Local Debug
	/// </summary>
	public class DebugLambdaLogger : ILambdaLogger
	{
		public void Log(string message)
		{
			Console.Write(message);
		}

		public void LogLine(string message)
		{
			Console.WriteLine(message);
		}
	}
}