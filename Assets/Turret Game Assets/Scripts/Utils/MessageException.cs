using System;

namespace AssemblyCSharp
{
	public class MessageException : Exception
	{
		string errorMessage;
		
		public MessageException (string message)
		{
			errorMessage = message;
		}
		
		public string ErrorMessage
		{
			get { return errorMessage; }
		}
	}
}

