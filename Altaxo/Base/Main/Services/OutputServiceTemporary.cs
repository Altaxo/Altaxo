#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Services
{
	public class OutputServiceTemporary : IOutputService
	{
		private StringBuilder _stb = new StringBuilder();
		private object _locker = new object();

		public string Text
		{
			get
			{
				return _stb.ToString();
			}
		}

		#region IOutputService Members

		public void Write(string text)
		{
			lock (_locker)
			{
				_stb.Append(text);
			}
		}

		public void WriteLine()
		{
			Write(System.Environment.NewLine);
		}

		public void WriteLine(string text)
		{
			Write(text + System.Environment.NewLine);
		}

		public void WriteLine(string format, params object[] args)
		{
			Write(string.Format(format, args) + System.Environment.NewLine);
		}

		public void WriteLine(System.IFormatProvider provider, string format, params object[] args)
		{
			Write(string.Format(provider, format, args) + System.Environment.NewLine);
		}

		public void Write(string format, params object[] args)
		{
			Write(string.Format(format, args));
		}

		public void Write(System.IFormatProvider provider, string format, params object[] args)
		{
			Write(string.Format(provider, format, args));
		}

		#endregion IOutputService Members
	}
}