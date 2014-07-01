using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.DataConnection
{
	public class LoginCredentials : ICloneable
	{
		public string UserName { get; private set; }

		public string Password { get; private set; }

		public LoginCredentials(string username, string password)
		{
			UserName = username;
			Password = password;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public bool AreEmpty
		{
			get
			{
				return string.IsNullOrEmpty(UserName);
			}
		}
	}
}