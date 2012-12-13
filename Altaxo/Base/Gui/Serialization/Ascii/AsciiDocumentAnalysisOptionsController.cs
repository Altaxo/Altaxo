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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Globalization;

using Altaxo.Collections;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
	public interface IAsciiDocumentAnalysisOptionsView
	{
		int NumberOfLinesToAnalyze { get; set; }
		void SetNumberFormatsToAnalyze(SelectableListNodeList availableFormats, ObservableCollection<Boxed<SelectableListNode>> currentlySelectedItems);
		void SetDateTimeFormatsToAnalyze(SelectableListNodeList availableFormats, ObservableCollection<Boxed<SelectableListNode>> currentlySelectedItems);
	}


	[ExpectedTypeOfView(typeof(IAsciiDocumentAnalysisOptionsView))]
	[UserControllerForObject(typeof(AsciiDocumentAnalysisOptions))]
	public class AsciiDocumentAnalysisOptionsController : MVCANControllerBase<AsciiDocumentAnalysisOptions, IAsciiDocumentAnalysisOptionsView>
	{
		SelectableListNodeList _availableCultureList;

		System.Collections.ObjectModel.ObservableCollection<Boxed<SelectableListNode>> _numberFormatsToAnalyze;
		System.Collections.ObjectModel.ObservableCollection<Boxed<SelectableListNode>> _dateTimeFormatsToAnalyze;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				GetAvailableCultures(ref _availableCultureList);

				_numberFormatsToAnalyze = new System.Collections.ObjectModel.ObservableCollection<Boxed<SelectableListNode>>();
				foreach (var item in _availableCultureList)
					if (_doc.NumberFormatsToTest.Contains((CultureInfo)item.Tag))
						_numberFormatsToAnalyze.Add(item);

				_dateTimeFormatsToAnalyze = new System.Collections.ObjectModel.ObservableCollection<Boxed<SelectableListNode>>();
			
				foreach (var item in _availableCultureList)
					if (_doc.DateTimeFormatsToTest.Contains((CultureInfo)item.Tag))
						_dateTimeFormatsToAnalyze.Add(item);
			}

			if (null != _view)
			{
				_view.NumberOfLinesToAnalyze = AsciiDocumentAnalysis.DefaultNumberOfLinesToAnalyze;
				_view.SetNumberFormatsToAnalyze(_availableCultureList, _numberFormatsToAnalyze);
				_view.SetDateTimeFormatsToAnalyze(_availableCultureList, _dateTimeFormatsToAnalyze);
			}
		}

		public override bool Apply()
		{
			_doc.NumberOfLinesToAnalyze = _view.NumberOfLinesToAnalyze;

			_doc.NumberFormatsToTest.Clear();
			_doc.DateTimeFormatsToTest.Clear();
			foreach (var item in _numberFormatsToAnalyze)
				_doc.NumberFormatsToTest.Add((CultureInfo)item.Value.Tag);
			foreach (var item in _dateTimeFormatsToAnalyze)
				_doc.DateTimeFormatsToTest.Add((CultureInfo)item.Value.Tag);

			if (_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}

		private int CompareCultures(CultureInfo x, CultureInfo y)
		{
			return string.Compare(x.DisplayName, y.DisplayName);
		}

		void GetAvailableCultures(ref SelectableListNodeList list)
		{
			list = new SelectableListNodeList();
			var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			Array.Sort(cultures, CompareCultures);

			var invCult = System.Globalization.CultureInfo.InvariantCulture;
			AddCulture(list, invCult, false);

			foreach (var cult in cultures)
				AddCulture(list, cult, false);

			if (null == list.FirstSelectedNode)
				list[0].IsSelected = true;
		}

		void AddCulture(SelectableListNodeList cultureList, CultureInfo cult, bool isSelected)
		{
			cultureList.Add(new SelectableListNode(cult.DisplayName, cult, isSelected));
		}


	}
}
