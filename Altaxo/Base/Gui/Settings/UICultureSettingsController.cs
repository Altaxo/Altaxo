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

	public interface IUICultureSettingsView
	{
		/// <summary>Gets or sets a value indicating whether to use the operating system settings for the UI culture or use another one.</summary>
		/// <value>If <see langword="true"/>, own settings are used; otherwise the operating system settings are used for the current UI culture.</value>
		bool OverrideOperatingSystemSettings { get; set; }

		/// <summary>Initializes the culture format list.</summary>
		/// <param name="list">List containing all selectable cultures.</param>
		void InitializeCultureFormatList(SelectableListNodeList list);

		/// <summary>Occurs when the culture name changed.</summary>
		event Action CultureChanged;

		/// <summary>Occurs when the user chooses to change the state of the override system culture UI element (probably a checkbox or two radio buttons).</summary>
		event Action OverrideSystemCultureChanged;

		string NumberDecimalSeparator { get; set; }
		string NumberGroupSeparator { get; set; }

	}

	/// <summary>Manages</summary>
	[ExpectedTypeOfView(typeof(IUICultureSettingsView))]
	[UserControllerForObject(typeof(UICultureSettings))]
	public class UICultureSettingsController : IMVCANController
	{
		IUICultureSettingsView _view;
		UICultureSettings _originalDoc;

		/// <summary>Holds temporary the settings.</summary>
		UICultureSettings _doc;

		/// <summary>Represents the document with the operation system settings.</summary>
		UICultureSettings _sysSettingsDoc;

		/// <summary>If true, indicates that the document was created by this controller and should be saved to Altaxo settings when <see cref="Apply"/> is called.</summary>
		bool _isHoldingOwnDocument;

		/// <summary>List of available cultures.</summary>
		SelectableListNodeList _availableCulturesList;

		/// <summary>List with only a single entry: the operating system UI culture;</summary>
		SelectableListNodeList _sysSettingsCultureList;


		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || (null!=args[0] && !(args[0] is AutoUpdateSettings)))
				return false;

			_originalDoc = args[0] as UICultureSettings;

			if (null == _originalDoc)
			{
				_isHoldingOwnDocument = true;
				_originalDoc = Current.PropertyService.Get(UICultureSettings.SettingsStoragePath, UICultureSettings.FromDefault());
			}

			_doc = (UICultureSettings)_originalDoc.Clone();
			_sysSettingsDoc = UICultureSettings.FromDefault();

			Initialize(true);
			
			return true;
		}

		void Initialize(bool initData)
		{
			if (initData)
			{
				_availableCulturesList = new SelectableListNodeList();
				var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
				Array.Sort(cultures, CompareCultures);
				foreach (var cult in cultures)
					AddToCultureList(cult, cult.Name == _doc.CultureName);
				if (null == _availableCulturesList.FirstSelectedNode)
					_availableCulturesList[0].IsSelected = true;

				var defCult = _sysSettingsDoc.ToCulture();
				_sysSettingsCultureList = new SelectableListNodeList();
				_sysSettingsCultureList.Add(new SelectableListNode(defCult.DisplayName,defCult,true));

			}

			if (null != _view)
			{
				_view.OverrideOperatingSystemSettings = _doc.OverrideParentCulture;
				_view.InitializeCultureFormatList(_doc.OverrideParentCulture ? _availableCulturesList : _sysSettingsCultureList);

				_view.NumberDecimalSeparator = _doc.NumberDecimalSeparator;
				_view.NumberGroupSeparator = _doc.NumberGroupSeparator;
			}
		}

		void AddToCultureList(CultureInfo cult, bool isSelected)
		{
			_availableCulturesList.Add(new SelectableListNode(cult.DisplayName, cult, cult.Name == _doc.CultureName));
		}

		private int CompareCultures(CultureInfo x, CultureInfo y)
		{
			return string.Compare(x.DisplayName, y.DisplayName);
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
				if (null != _view)
				{
					_view.CultureChanged -= EhCultureChanged;
					_view.OverrideSystemCultureChanged -= EhOverrideSystemCultureChanged;
				}

				_view = value as IUICultureSettingsView;

				if (null != _view)
				{
					Initialize(false);
					_view.CultureChanged += EhCultureChanged;
					_view.OverrideSystemCultureChanged += EhOverrideSystemCultureChanged;
				}
			}
		}

		void EhOverrideSystemCultureChanged()
		{
			bool overrideSysSettings = _view.OverrideOperatingSystemSettings;
			_view.InitializeCultureFormatList(overrideSysSettings ? _availableCulturesList : _sysSettingsCultureList);
			SetElementsAfterCultureChanged(overrideSysSettings ? _doc : _sysSettingsDoc);
		}

		void EhCultureChanged()
		{
			var node = _availableCulturesList.FirstSelectedNode;
			if (node != null)
			{
				CultureInfo c = (CultureInfo)node.Tag;
				_doc.SetMembersFromCulture(c);
				SetElementsAfterCultureChanged(_doc);
			}
		}

		void SetElementsAfterCultureChanged(UICultureSettings s)
		{
			_view.NumberDecimalSeparator = s.NumberDecimalSeparator;
			_view.NumberGroupSeparator = s.NumberGroupSeparator;
		}

		public object ModelObject
		{
			get { return _originalDoc; }
		}

		public bool Apply()
		{
			if (_view.OverrideOperatingSystemSettings)
			{
				_doc.NumberDecimalSeparator = _view.NumberDecimalSeparator;
				_doc.NumberGroupSeparator = _view.NumberGroupSeparator;
				_originalDoc.CopyFrom(_doc);
			}
			else
			{
				_originalDoc.CopyFrom(_sysSettingsDoc);
			}

			if (_isHoldingOwnDocument)
			{
				


				// first we set the properties that Sharpdevelop awaits to change its language,
				Current.PropertyService.Set("CoreProperties.UILanguage", _originalDoc.NeutralCultureName);
				// then we set our own culture settings
				Current.PropertyService.Set(UICultureSettings.SettingsStoragePath, _originalDoc);
			}

			return true;
		}
	}
}
