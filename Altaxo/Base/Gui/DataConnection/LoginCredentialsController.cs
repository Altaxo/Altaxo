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
	public class LoginCredentialsController : MVCANControllerBase<LoginCredentials, ILoginCredentialsView>
	{
		protected override void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.Username = _doc.UserName;
				_view.Password = _doc.Password;
			}
		}

		public override bool Apply()
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