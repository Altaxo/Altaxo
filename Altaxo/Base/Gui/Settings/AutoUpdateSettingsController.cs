#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Settings;

namespace Altaxo.Gui.Settings
{
	/// <summary>Interface that a Gui window must implement in order to be able to be controlled by <see cref="AutoUpdateSettingsController"/>.</summary>
	public interface IAutoUpdateSettingsView
	{
		/// <summary>Gets or sets the state of the Gui element(s) that enables/disables auto updates (<see cref="P:AutoUpdateSettings.EnableAutoUpdates"/>).</summary>
		bool EnableAutoUpdates { get; set; }
		
		/// <summary>Gets or sets the state of the Gui element(s) that indicate whether to download the stable or the unstable versions (<see cref="P:AutoUpdateSettings.DownloadUnstableVersion"/>).</summary>
		bool DownloadUnstableVersion { get; set; }

		/// <summary>Gets or sets the state of the Gui element(s) that indicate whether to show the download window (<see cref="P:AutoUpdateSettings.ShowDownloadWindow"/>).</summary>
		bool ShowDownloadWindow { get; set; }

		/// <summary>Gets or sets the state of the Gui element that is used to show the download interval (<see cref="P:AutoUpdateSettings.DownloadIntervalInDays"/>).</summary>
		/// <value>The download interval in days.</value>
		int DownloadInterval { get; set; }

		/// <summary>Gets or sets the Gui element(s) that indicate when to install auto updates. Here: Bit0=1: at startup; Bit1=1: at shutdown</summary>
		int InstallAt { get; set; }
	}




	/// <summary>Manages the <see cref="IAutoUpdateSettingsView">Gui interface</see> for the <see cref="AutoUpdateSettings">auto update settings</see>.</summary>
	[ExpectedTypeOfView(typeof(IAutoUpdateSettingsView))]
	[UserControllerForObject(typeof(AutoUpdateSettings))]
	public class AutoUpdateSettingsController : IMVCANController
	{
		IAutoUpdateSettingsView _view;
		AutoUpdateSettings _doc;
		
		/// <summary>If true, indicates that the document was created by this controller and should be saved to Altaxo settings when <see cref="Apply"/> is called.</summary>
		bool _isHoldingOwnDocument;

		public AutoUpdateSettingsController()
		{
		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || (null!=args[0] && !(args[0] is AutoUpdateSettings)))
				return false;
			_doc = args[0] as AutoUpdateSettings;

			if (null == _doc)
			{
				_isHoldingOwnDocument = true;
				_doc = Current.PropertyService.Get(AutoUpdateSettings.SettingsStoragePath, new AutoUpdateSettings());
			}


			Initialize(true);
			return true;
		}

		void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.EnableAutoUpdates = _doc.EnableAutoUpdates;
				_view.DownloadUnstableVersion = _doc.DownloadUnstableVersion;
				_view.DownloadInterval = _doc.DownloadIntervalInDays;
				_view.ShowDownloadWindow = _doc.ShowDownloadWindow;
				_view.InstallAt = (_doc.InstallAtStartup ? 1 : 0) + ( _doc.InstallAtShutdown ? 2 : 0);
			}
		}


		public UseDocument UseDocumentCopy
		{
			set {  }
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{


				_view = value as IAutoUpdateSettingsView;

				if (null != _view)
				{
					Initialize(false);
				}
			
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public bool Apply()
		{
			_doc.EnableAutoUpdates = _view.EnableAutoUpdates;
			_doc.DownloadUnstableVersion = _view.DownloadUnstableVersion;
			_doc.DownloadIntervalInDays = _view.DownloadInterval;
			_doc.ShowDownloadWindow = _view.ShowDownloadWindow;
			_doc.InstallAtStartup = 0 != (_view.InstallAt & 1);
			_doc.InstallAtShutdown =0 != (_view.InstallAt & 2);

			if(_isHoldingOwnDocument)
				Current.PropertyService.Set(AutoUpdateSettings.SettingsStoragePath, _doc);

			return true;
		}
	}
}
