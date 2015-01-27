#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using Altaxo.Main.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common.Tools
{
	public interface ITestAllProjectsInFolderView
	{
		string FolderPaths { get; set; }

		bool TestSavingAndReopening { get; set; }
	}

	[UserControllerForObject(typeof(TestAllProjectsInFolderOptions))]
	[ExpectedTypeOfView(typeof(ITestAllProjectsInFolderView))]
	public class TestAllProjectsInFolderController : MVCANControllerEditOriginalDocBase<TestAllProjectsInFolderOptions, ITestAllProjectsInFolderView>
	{
		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (null != _view)
			{
				_view.FolderPaths = _doc.FolderPaths;
				_view.TestSavingAndReopening = _doc.TestSavingAndReopening;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.TestSavingAndReopening = _view.TestSavingAndReopening;
			_doc.FolderPaths = _view.FolderPaths;

			var applyResult = !string.IsNullOrEmpty(_doc.FolderPaths);

			return ApplyEnd(applyResult, disposeController);
		}
	}
}