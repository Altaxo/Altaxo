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
	/// <summary>
	/// Base class implementing <see cref="Altaxo.Main.Services.ITextOutputService"/>. You only need to overwrite <see cref="InternalWrite(string)"/>
	/// </summary>
	/// <seealso cref="Altaxo.Main.Services.ITextOutputService" />
	public abstract class TextOutputServiceBase : ITextOutputService
	{
		private object _locker = new object();

		#region IOutputService Members

		protected abstract void InternalWrite(string text);

		public void Write(string text)
		{
			InternalWrite(text);
		}

		public void WriteLine()
		{
			InternalWrite(System.Environment.NewLine);
		}

		public void WriteLine(string text)
		{
			InternalWrite(text + System.Environment.NewLine);
		}

		public void WriteLine(string format, params object[] args)
		{
			InternalWrite(string.Format(format, args) + System.Environment.NewLine);
		}

		public void WriteLine(System.IFormatProvider provider, string format, params object[] args)
		{
			InternalWrite(string.Format(provider, format, args) + System.Environment.NewLine);
		}

		public void Write(string format, params object[] args)
		{
			InternalWrite(string.Format(format, args));
		}

		public void Write(System.IFormatProvider provider, string format, params object[] args)
		{
			InternalWrite(string.Format(provider, format, args));
		}

		#endregion IOutputService Members
	}
}