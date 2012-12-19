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

using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Settings
{
	/// <summary>
	/// Interface that the Gui component has to implement in order to be a view for <see cref="DocumentCultureSettingsController"/>.
	/// </summary>
	public interface IAsciiAnalysisSettingsView
	{
		/// <summary>Gets or sets a value indicating whether to use the operating system settings for the UI culture or use another one.</summary>
		/// <value>If <see langword="true"/>, own settings are used; otherwise the operating system settings are used for the current UI culture.</value>
		bool OverrideSystemSettings { get; set; }
	
		/// <summary>Occurs when the user chooses to change the state of the override system culture UI element (probably a checkbox or two radio buttons).</summary>
		event Action OverrideSystemSettingsChanged;


		object OptionsView { set; }
	}

	/// <summary>Manages the user interaction to set the members of <see cref="DocumentCultureSettings"/>.</summary>
	[ExpectedTypeOfView(typeof(IAsciiAnalysisSettingsView))]
	public class AsciiAnalysisSettingsController : MVCANControllerBase<AsciiDocumentAnalysisOptions, IAsciiAnalysisSettingsView>
	{
		/// <summary>If true, indicates that the document was created by this controller and should be saved to Altaxo settings when <see cref="Apply"/> is called.</summary>
		bool _isHoldingOwnDocument;

		bool _overrideSystemSettings;

		IMVCANController _analysisOptionsController;

		AsciiDocumentAnalysisOptions _sysSettingsDoc = AsciiDocumentAnalysisOptions.SystemDefault;

		/// <summary>Initialize the controller with the document. If successfull, the function has to return true.</summary>
		/// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
		/// <returns>Returns <see langword="true"/> if successfull; otherwise <see langword="false"/>.</returns>
		public override bool InitializeDocument(params object[] args)
		{
			_overrideSystemSettings = AsciiDocumentAnalysisOptions.UseCustomUserSettings;
			_originalDoc = AsciiDocumentAnalysisOptions.UserDefault;
			_doc = (AsciiDocumentAnalysisOptions)_originalDoc.Clone();
			_sysSettingsDoc = AsciiDocumentAnalysisOptions.SystemDefault;

			Initialize(true);
			
			return true;
		}

		
		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_analysisOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _overrideSystemSettings ? _doc : _sysSettingsDoc }, typeof(IMVCANController), UseDocument.Directly);
			}

			if (null != _view)
			{
				_view.OverrideSystemSettings = _overrideSystemSettings;
				_view.OptionsView = _analysisOptionsController.ViewObject;
			}
		}

		protected override void AttachView()
		{
			_view.OverrideSystemSettingsChanged += new Action(EhOverrideSystemSettingsChanged);
		}

		protected override void DetachView()
		{
			_view.OverrideSystemSettingsChanged -= new Action(EhOverrideSystemSettingsChanged);
		}

		void EhOverrideSystemSettingsChanged()
		{
			_overrideSystemSettings = _view.OverrideSystemSettings;

			if (_overrideSystemSettings)
				_analysisOptionsController.InitializeDocument(_doc);
			else
				_analysisOptionsController.InitializeDocument(_sysSettingsDoc);
		}


		public override bool Apply()
		{
			_overrideSystemSettings = _view.OverrideSystemSettings;

			if (_overrideSystemSettings)
			{
				if (!_analysisOptionsController.Apply())
					return false;

			}

			AsciiDocumentAnalysisOptions.UserDefault = _overrideSystemSettings ? _doc : null;

			return true;
		}
	}
}
