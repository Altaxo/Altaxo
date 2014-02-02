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

#endregion Copyright

using Altaxo.Collections;
using Altaxo.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Settings
{
	/// <summary>
	/// Interface that the Gui component has to implement in order to be a view for <see cref="DocumentCultureSettingsController"/>.
	/// </summary>
	public interface ICultureSettingsView
	{
		/// <summary>Initializes the culture format list.</summary>
		/// <param name="list">List containing all selectable cultures.</param>
		void InitializeCultureFormatList(SelectableListNodeList list);

		/// <summary>Occurs when the culture name changed.</summary>
		event Action CultureChanged;

		/// <summary>Gets or sets the number decimal separator.</summary>
		/// <value>The number decimal separator.</value>
		string NumberDecimalSeparator { get; set; }

		/// <summary>Gets or sets the number group separator.</summary>
		/// <value>The number group separator.</value>
		string NumberGroupSeparator { get; set; }
	}

	/// <summary>Manages the user interaction to set the members of <see cref="DocumentCultureSettings"/>.</summary>
	[ExpectedTypeOfView(typeof(ICultureSettingsView))]
	[UserControllerForObject(typeof(CultureSettings))]
	public class CultureSettingsController : MVCANControllerBase<CultureSettings, ICultureSettingsView>
	{
		/// <summary>List of available cultures.</summary>
		private SelectableListNodeList _availableCulturesList;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_availableCulturesList = new SelectableListNodeList();
				var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
				Array.Sort(cultures, CompareCultures);
				AddToCultureList(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture.LCID == _doc.CultureID);
				foreach (var cult in cultures)
					AddToCultureList(cult, cult.LCID == _doc.CultureID);
				if (null == _availableCulturesList.FirstSelectedNode)
					_availableCulturesList[0].IsSelected = true;
			}

			if (null != _view)
			{
				_view.InitializeCultureFormatList(_availableCulturesList);

				_view.NumberDecimalSeparator = _doc.NumberDecimalSeparator;
				_view.NumberGroupSeparator = _doc.NumberGroupSeparator;
			}
		}

		private void AddToCultureList(CultureInfo cult, bool isSelected)
		{
			_availableCulturesList.Add(new SelectableListNode(cult.DisplayName, cult, cult.LCID == _doc.CultureID));
		}

		private int CompareCultures(CultureInfo x, CultureInfo y)
		{
			return string.Compare(x.DisplayName, y.DisplayName);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.CultureChanged += EhCultureChanged;
		}

		protected override void DetachView()
		{
			_view.CultureChanged -= EhCultureChanged;
			base.DetachView();
		}

		private void EhCultureChanged()
		{
			var node = _availableCulturesList.FirstSelectedNode;
			if (node != null)
			{
				CultureInfo c = (CultureInfo)node.Tag;
				_doc = new CultureSettings(c);
				SetElementsAfterCultureChanged(_doc);
			}
		}

		private void SetElementsAfterCultureChanged(CultureSettings s)
		{
			_view.NumberDecimalSeparator = s.NumberDecimalSeparator;
			_view.NumberGroupSeparator = s.NumberGroupSeparator;
		}

		public override bool Apply()
		{
			var doc = (CultureInfo)_doc.Culture.Clone();
			doc.NumberFormat.NumberDecimalSeparator = _view.NumberDecimalSeparator;
			doc.NumberFormat.NumberGroupSeparator = _view.NumberGroupSeparator;
			_doc = new CultureSettings(doc);
			_originalDoc = (CultureSettings)_doc.Clone();

			return true;
		}
	}
}