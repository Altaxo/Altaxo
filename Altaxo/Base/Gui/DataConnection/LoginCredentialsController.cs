#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using Altaxo.DataConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public interface ILoginCredentialsView
	{
		string Username { get; set; }

		string Password { get; set; }
	}

	[ExpectedTypeOfView(typeof(ILoginCredentialsView))]
	[UserControllerForObject(typeof(LoginCredentials))]
	public class LoginCredentialsController : MVCANControllerEditCopyOfDocBase<LoginCredentials, ILoginCredentialsView>
	{
		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.Username = _doc.UserName;
				_view.Password = _doc.Password;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc = new LoginCredentials(_view.Username, _view.Password);

			if (_doc.AreEmpty)
			{
				Current.Gui.ErrorMessageBox("You must provide at least a user name.");
				return false;
			}

			if (!object.ReferenceEquals(_originalDoc, _doc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}