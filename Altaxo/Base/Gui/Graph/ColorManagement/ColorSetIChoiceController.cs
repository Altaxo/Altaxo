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

#nullable disable
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Drawing.ColorManagement;

namespace Altaxo.Gui.Graph.ColorManagement
{
  public interface IColorSetsView
  {
    NGTreeNode ColorSetTree { set; }
  }

  [ExpectedTypeOfView(typeof(IColorSetsView))]
  public class ColorSetChoiceController : MVCANControllerEditImmutableDocBase<ColorSetIdentifier, IColorSetsView>
  {
    private NGTreeNode _treeRootNode = new NGTreeNode();

    public bool ShowPlotColorsOnly { get; set; }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {

        ColorSetManager.Instance.TryGetList(_doc.Name, out var value);
        ColorControllerHelper.UpdateColorTreeViewTreeNodes(_treeRootNode, ShowPlotColorsOnly, value.List);
      }
      if (null != _view)
      {
        _view.ColorSetTree = _treeRootNode;
      }
    }

    public override bool Apply(bool disposeController)
    {
      var selNode = _treeRootNode.AnyBetweenHereAndLeaves(node => node.IsSelected);

      if (null == selNode)
        return false;

      var tag = selNode.Tag as IColorSet;

      if (null == tag)
      {
        string insteadOf = selNode.Tag != null ? selNode.Tag.GetType().Name : "nothing";
        Current.Gui.ErrorMessageBox("Please select a color set instead of " + insteadOf, "Wrong selection");
        return false;
      }

      _doc = new ColorSetIdentifier(Altaxo.Main.ItemDefinitionLevel.Project, tag.Name);

      return ApplyEnd(true, disposeController);
    }
  }
}
