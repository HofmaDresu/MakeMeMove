using System;

namespace MacroEatMobile.Core
{
	public class NetworkException : Exception
	{

		public NetworkException (Exception innerException) : base ("", innerException)
		{
			
		}
	}

}