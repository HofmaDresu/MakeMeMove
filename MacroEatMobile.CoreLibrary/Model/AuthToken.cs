using System;

namespace MacroEatMobile.Core
{
	public class AuthToken
	{
		public AuthToken ()
		{
		}

		public string AccessToken {
			get;
			set;
		}
		public DateTime? ExpirationDate {
			get;
			set;
		}
		public string RefreshToken {
			get;
			set;
		}
	}
}

