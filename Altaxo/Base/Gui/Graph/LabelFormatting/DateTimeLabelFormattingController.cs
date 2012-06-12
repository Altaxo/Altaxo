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

using Altaxo.Graph.Gdi.LabelFormatting;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph.LabelFormatting
{
	public interface IDateTimeLabelFormattingView
	{
		IMultiLineLabelFormattingBaseView MultiLineLabelFormattingBaseView { get; }

		void InitializeTimeConversion(SelectableListNodeList items);
		string FormattingString { get; set; }
		string FormattingStringAlternate { get; set; }
		bool ShowAlternateFormattingOnMidnight { get; set; }
		bool ShowAlternateFormattingOnNoon { get; set; }



	}

	[ExpectedTypeOfView(typeof(IDateTimeLabelFormattingView))]
	[UserControllerForObject(typeof(DateTimeLabelFormatting),110)]
	public class DateTimeLabelFormattingController : MVCANControllerBase<DateTimeLabelFormatting, IDateTimeLabelFormattingView>
	{
		SelectableListNodeList _timeConversionChoices;
		MultiLineLabelFormattingBaseController _baseController;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_baseController = new MultiLineLabelFormattingBaseController() { UseDocumentCopy = UseDocument.Directly };
				_baseController.InitializeDocument(_doc);
				_timeConversionChoices = new SelectableListNodeList(_doc.LabelTimeConversion);
			}

			if (null != _view)
			{
				_baseController.ViewObject = _view.MultiLineLabelFormattingBaseView;
				_view.InitializeTimeConversion(_timeConversionChoices);
				_view.FormattingString = _doc.FormattingString;
				_view.ShowAlternateFormattingOnMidnight = _doc.ShowAlternateFormattingAtMidnight;
				_view.ShowAlternateFormattingOnNoon = _doc.ShowAlternateFormattingAtNoon;
				_view.FormattingStringAlternate = _doc.FormattingStringAlternate;
			}
		}

		public override bool Apply()
		{
			if (!_baseController.Apply())
				return false;


			_doc.LabelTimeConversion = (DateTimeLabelFormatting.TimeConversion)_timeConversionChoices.FirstSelectedNode.Tag;

			_doc.FormattingString = _view.FormattingString;
			_doc.ShowAlternateFormattingAtMidnight = _view.ShowAlternateFormattingOnMidnight;
			_doc.ShowAlternateFormattingAtNoon = _view.ShowAlternateFormattingOnNoon;
			_doc.FormattingStringAlternate = _view.FormattingStringAlternate;

			if(_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}
