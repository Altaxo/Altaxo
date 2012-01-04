using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Settings;

namespace Altaxo.Gui.Settings
{
	public interface IAutoUpdateSettingsView
	{
		bool EnableAutoUpdates { get; set; }
		bool DownloadUnstableVersion { get; set; }
		int DownloadInterval { get; set; }

		/// <summary>When to install Altaxo: Bit0: at startup; Bit1: at shutdown</summary>
		int InstallAt { get; set; }
	}


	

	[ExpectedTypeOfView(typeof(IAutoUpdateSettingsView))]
	[UserControllerForObject(typeof(AutoUpdateSettings))]
	public class AutoUpdateSettingsController : IMVCANController
	{
		IAutoUpdateSettingsView _view;
		AutoUpdateSettings _doc;


		public AutoUpdateSettingsController()
		{
			_doc = Current.PropertyService.Get(AutoUpdateSettings.SettingsStoragePath, new AutoUpdateSettings());
			Initialize(true);
		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || !(args[0] is AutoUpdateSettings))
				return false;
			_doc = (AutoUpdateSettings)args[0];
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
			_doc.InstallAtStartup = 0 != (_view.InstallAt & 1);
			_doc.InstallAtShutdown =0 != (_view.InstallAt & 2);

			Current.PropertyService.Set(AutoUpdateSettings.SettingsStoragePath, _doc);

			return true;
		}
	}
}
