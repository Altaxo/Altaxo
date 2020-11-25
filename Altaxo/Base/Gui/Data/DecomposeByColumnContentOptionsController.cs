#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Data
{
  using Altaxo.Collections;
  using Altaxo.Data;

  public interface IDecomposeByColumnContentOptionsView
  {
    void InitializeDestinationOutputFormat(SelectableListNodeList list);

    void InitializeDestinationColumnSorting(SelectableListNodeList list);
  }

  [UserControllerForObject(typeof(DecomposeByColumnContentOptions))]
  [ExpectedTypeOfView(typeof(IDecomposeByColumnContentOptionsView))]
  public class DecomposeByColumnContentOptionsController : MVCANControllerEditOriginalDocBase<DecomposeByColumnContentOptions, IDecomposeByColumnContentOptionsView>
  {
    private SelectableListNodeList _choicesDestinationOutputFormat;
    private SelectableListNodeList _choicesDestinationColSort;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _choicesDestinationOutputFormat = null;
      _choicesDestinationColSort = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _choicesDestinationOutputFormat = new SelectableListNodeList();
        _choicesDestinationOutputFormat.FillWithEnumeration(_doc.DestinationOutput);

        _choicesDestinationColSort = new SelectableListNodeList();
        _choicesDestinationColSort.FillWithEnumeration(_doc.DestinationColumnSorting);
      }
      if (_view is not null)
      {
        _view.InitializeDestinationOutputFormat(_choicesDestinationOutputFormat);
        _view.InitializeDestinationColumnSorting(_choicesDestinationColSort);
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.DestinationOutput = (DecomposeByColumnContentOptions.OutputFormat)_choicesDestinationOutputFormat.FirstSelectedNode.Tag;
      _doc.DestinationColumnSorting = (DecomposeByColumnContentOptions.OutputSorting)_choicesDestinationColSort.FirstSelectedNode.Tag;

      return ApplyEnd(true, disposeController);
    }
  }
}
