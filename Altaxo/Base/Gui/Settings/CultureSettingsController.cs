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
using System.Globalization;

using Altaxo.Collections;
using Altaxo.Settings;

namespace Altaxo.Gui.Settings
{

	public interface ICultureSettingsView
	{
		/// <summary>Gets or sets a value indicating whether to use the operating system settings for the UI culture or use another one.</summary>
		/// <value>If <see langword="true"/>, own settings are used; otherwise the operating system settings are used for the current UI culture.</value>
		bool OverrideOperatingSystemSettings { get; set; }

		/// <summary>Initializes the culture format list.</summary>
		/// <param name="list">List containing all selectable cultures.</param>
		void InitializeCultureFormatList(SelectableListNodeList list);

	}

	/// <summary>Manages</summary>
	[ExpectedTypeOfView(typeof(ICultureSettingsView))]
	[UserControllerForObject(typeof(CultureSettings))]
	public class CultureSettingsController : IMVCANController
	{
		public static string SettingsStoragePath = "Altaxo.Options.UICulture";

		ICultureSettingsView _view;
		CultureSettings _doc;

		/// <summary>If true, indicates that the document was created by this controller and should be saved to Altaxo settings when <see cref="Apply"/> is called.</summary>
		bool _isHoldingOwnDocument;


		SelectableListNodeList _cultureList;

		public CultureSettingsController()
		{
			//_doc = Current.PropertyService.Get(AutoUpdateSettings.SettingsStoragePath, new AutoUpdateSettings());

			_doc = new CultureSettings();

			Initialize(true);
		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || (null!=args[0] && !(args[0] is AutoUpdateSettings)))
				return false;

			_doc = args[0] as CultureSettings;

			if (null == _doc)
			{
				_isHoldingOwnDocument = true;
				_doc = Current.PropertyService.Get(CultureSettingsController.SettingsStoragePath, new CultureSettings());
			}

				Initialize(true);
			
			return true;
		}

		void Initialize(bool initData)
		{
			if (initData)
			{
				_cultureList = new SelectableListNodeList();
				var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
				foreach (var cult in cultures)
					_cultureList.Add(new SelectableListNode(cult.DisplayName, cult, cult.Equals(_doc.CultureInfo)));
			}

			if (null != _view)
			{
				_view.OverrideOperatingSystemSettings = _doc.OverrideParentCulture;
				_view.InitializeCultureFormatList(_cultureList);
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
				_view = value as ICultureSettingsView;

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
			_doc.OverrideParentCulture = _view.OverrideOperatingSystemSettings;
			var sel = _cultureList.FirstSelectedNode;
			if (null != sel)
				_doc.CultureInfo = (CultureInfo)sel.Tag;

			return true;
		}
	}
}
