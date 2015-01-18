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
using Altaxo.Graph.Gdi.LabelFormatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.LabelFormatting
{
	public interface IMultiLineLabelFormattingBaseView
	{
		double LineSpacing { get; set; }

		SelectableListNodeList TextBlockAlignement { set; }
	}

	[UserControllerForObject(typeof(MultiLineLabelFormattingBase))]
	[ExpectedTypeOfView(typeof(IMultiLineLabelFormattingBaseView))]
	public class MultiLineLabelFormattingBaseController : MVCANControllerBase<MultiLineLabelFormattingBase, IMultiLineLabelFormattingBaseView>
	{
		private SelectableListNodeList _textBlockAlignmentChoices;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_textBlockAlignmentChoices = new SelectableListNodeList(_doc.TextBlockAlignment);
			}
			if (null != _view)
			{
				_view.LineSpacing = _doc.LineSpacing;
				_view.TextBlockAlignement = _textBlockAlignmentChoices;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.LineSpacing = _view.LineSpacing;
			_doc.TextBlockAlignment = (System.Drawing.StringAlignment)_textBlockAlignmentChoices.FirstSelectedNode.Tag;

			if (_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);
			return true;
		}
	}
}