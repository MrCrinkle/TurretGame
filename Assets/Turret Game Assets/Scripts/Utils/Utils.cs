#define DEBUG

using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace AssemblyCSharp
{
	public class Utils
	{
		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
			if (!condition)
			{
				MessageException exception = new MessageException("Assert error");
				UnityEngine.Debug.Log(exception.Message);
				throw exception;
				
			}
		}
		
		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				MessageException exception = new MessageException(message);
				UnityEngine.Debug.Log(exception.Message);
				throw exception;
				
			};
		}
	}
}

