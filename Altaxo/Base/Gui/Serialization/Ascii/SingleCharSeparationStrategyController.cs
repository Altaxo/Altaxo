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
using Altaxo.Serialization.Ascii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Serialization.Ascii
{
	public interface ISingleCharSeparationStrategyView
	{
		char SeparatorChar { get; set; }
	}

	[ExpectedTypeOfView(typeof(ISingleCharSeparationStrategyView))]
	[UserControllerForObject(typeof(SingleCharSeparationStrategy))]
	public class SingleCharSeparationStrategyController : MVCANControllerEditCopyOfDocBase<SingleCharSeparationStrategy, ISingleCharSeparationStrategyView>
	{
		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
			}

			if (null != _view)
			{
				_view.SeparatorChar = _doc.SeparatorChar;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.SeparatorChar = _view.SeparatorChar;

			if (_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}