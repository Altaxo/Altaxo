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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D.Plot.Groups;
using Altaxo.Main;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  public interface IColorListView : IStyleListView
  {
    void SetCustomColorView(object guiCustomColorViewObject);

    event Action UserRequest_AddCustomColorToList;

    event Action<double> UserRequest_ForAllSelectedItemsSetOpacity;

    event Action<double> UserRequest_ForAllSelectedItemsShiftHue;

    event Action<double> UserRequest_ForAllSelectedItemsSetSaturation;

    event Action<double> UserRequest_ForAllSelectedItemsSetBrightness;

    event Action<string> UserRequest_ForAllSelectedItemsSetColorName;
  }

  [ExpectedTypeOfView(typeof(IColorListView))]
  [UserControllerForObject(typeof(IColorSet))]
  public class ColorSetController : StyleListController<ColorSetManager, IColorSet, NamedColor>
  {
    private NamedColorController _customColorController;

    public ColorSetController()
      : base(ColorSetManager.Instance)
    {
      _customColorController = new NamedColorController();
      Current.Gui.FindAndAttachControlTo(_customColorController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      IEnumerable<ControllerAndSetNullMethod> GetMySubControllers()
      {
        yield return new ControllerAndSetNullMethod(_customColorController, () => _customColorController = null);
      }

      return base.GetSubControllers().Concat(GetMySubControllers());
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _customColorController = new NamedColorController();
        _customColorController.InitializeDocument(NamedColors.White);
      }

      if (null != _view)
      {
        if (null == _customColorController.ViewObject)
          Current.Gui.FindAndAttachControlTo(_customColorController);
        ((IColorListView)_view).SetCustomColorView(_customColorController.ViewObject);
      }
    }

    protected override void AttachView()
    {
      base.AttachView();
      ((IColorListView)_view).UserRequest_AddCustomColorToList += EhUserRequest_AddCustomColorToList;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsSetOpacity += EhUserRequest_ForAllSelectedItemSetOpacity;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsShiftHue += EhUserRequest_ForAllSelectedItemShiftHue;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsSetSaturation += EhUserRequest_ForAllSelectedItemSetSaturation;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsSetBrightness += EhUserRequest_ForAllSelectedItemSetBrightness;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsSetColorName += EhUserRequest_ForAllSelectedItemSetColorName;
    }

    protected override void DetachView()
    {
      ((IColorListView)_view).UserRequest_AddCustomColorToList -= EhUserRequest_AddCustomColorToList;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsSetOpacity -= EhUserRequest_ForAllSelectedItemSetOpacity;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsShiftHue -= EhUserRequest_ForAllSelectedItemShiftHue;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsSetSaturation -= EhUserRequest_ForAllSelectedItemSetSaturation;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsSetBrightness -= EhUserRequest_ForAllSelectedItemSetBrightness;
      ((IColorListView)_view).UserRequest_ForAllSelectedItemsSetColorName -= EhUserRequest_ForAllSelectedItemSetColorName;

      base.DetachView();
    }

    private void EhUserRequest_AddCustomColorToList()
    {
      if (_customColorController.Apply(false))
      {
        var namedColor = (NamedColor)_customColorController.ModelObject;
        _currentItems.Add(new SelectableListNode(ToDisplayName(namedColor), namedColor, false));
        SetListDirty();
      }
    }

    private void EhUserRequest_ForAllSelectedItemSetOpacity(double opacity)
    {
      var alphaValue = AxoColor.NormFloatToByte((float)opacity);

      bool anyChange = false;
      foreach (var item in _currentItems.Where(node => node.IsSelected))
      {
        var color = ((NamedColor)item.Tag).Color;
        if (color.A != alphaValue)
        {
          color.A = alphaValue;
          var ncolor = new NamedColor(color);
          item.Tag = ncolor;
          item.Text = ToDisplayName(ncolor);

          anyChange = true;
        }
      }

      if (anyChange)
        SetListDirty();
    }

    private void EhUserRequest_ForAllSelectedItemShiftHue(double hueShift)
    {
      if (0 == hueShift || -1 == hueShift || 1 == hueShift)
        return;

      bool anyChange = false;
      foreach (var item in _currentItems.Where(node => node.IsSelected))
      {
        var color = ((NamedColor)item.Tag).Color;
        var (a, h, s, b) = color.ToAhsb();
        h += (float)hueShift;
        h -= (float)Math.Floor(h); // Normalize hue to 0..1

        color = AxoColor.FromAhsb(a, h, s, b);

        var ncolor = new NamedColor(color);
        item.Tag = ncolor;
        item.Text = ToDisplayName(ncolor);

        anyChange = true;
      }

      if (anyChange)
        SetListDirty();
    }

    private void EhUserRequest_ForAllSelectedItemSetSaturation(double saturation)
    {
      bool anyChange = false;
      foreach (var item in _currentItems.Where(node => node.IsSelected))
      {
        var color = ((NamedColor)item.Tag).Color;
        var (a, h, s, b) = color.ToAhsb();

        if (s != saturation)
        {
          s = (float)saturation;
          color = AxoColor.FromAhsb(a, h, s, b);

          var ncolor = new NamedColor(color);
          item.Tag = ncolor;
          item.Text = ToDisplayName(ncolor);

          anyChange = true;
        }
      }

      if (anyChange)
        SetListDirty();
    }

    private void EhUserRequest_ForAllSelectedItemSetBrightness(double brightness)
    {
      bool anyChange = false;
      foreach (var item in _currentItems.Where(node => node.IsSelected))
      {
        var color = ((NamedColor)item.Tag).Color;
        var (a, h, s, b) = color.ToAhsb();

        if (b != brightness)
        {
          b = (float)brightness;
          color = AxoColor.FromAhsb(a, h, s, b);

          var ncolor = new NamedColor(color);
          item.Tag = ncolor;
          item.Text = ToDisplayName(ncolor);

          anyChange = true;
        }
      }

      if (anyChange)
        SetListDirty();
    }

    private void EhUserRequest_ForAllSelectedItemSetColorName(string baseName)
    {
      bool anyChange = false;

      var items = _currentItems.Where(node => node.IsSelected).ToArray();

      if (items.Length == 0)
      {
        return;
      }
      else if (items.Length == 1)
      {
        var ncolor = (NamedColor)items[0].Tag;
        var newName = baseName;
        if (ncolor.Name != newName)
        {
          items[0].Tag = new NamedColor(ncolor.Color, newName);
          items[0].Text = newName;
          anyChange = true;
        }
      }
      else
      {
        string formatString = string.Format(System.Globalization.CultureInfo.InvariantCulture, "D0{0}", 1 + (int)Math.Floor(Math.Log10(items.Length)));

        for (int i = 0; i < items.Length; ++i)
        {
          var ncolor = (NamedColor)items[i].Tag;
          var newName = baseName + i.ToString(formatString, System.Globalization.CultureInfo.InvariantCulture);
          if (ncolor.Name != newName)
          {
            items[i].Tag = new NamedColor(ncolor.Color, newName);
            items[i].Text = newName;
            anyChange = true;
          }
        }
      }

      if (anyChange)
        SetListDirty();
    }

    protected override string ToDisplayName(NamedColor item)
    {
      return item.ToString();
    }

    protected override void Controller_AvailableItems_Initialize()
    {
      if (null == _availableItemsRootNode)
        _availableItemsRootNode = new NGTreeNode();
      else
        _availableItemsRootNode.Nodes.Clear();

      var levelDict = new Dictionary<ItemDefinitionLevel, NGTreeNode>();
      var allLists = ColorSetManager.Instance.GetEntryValues().ToArray();
      ;
      Array.Sort(allLists, (x, y) =>
      {
        int result = Comparer<ItemDefinitionLevel>.Default.Compare(x.Level, y.Level);
        return 0 != result ? result : string.Compare(x.List.Name, y.List.Name);
      }
      );

      foreach (var list in allLists)
      {
        if (!levelDict.TryGetValue(list.Level, out var levelNode))
        {
          levelNode = new NGTreeNode(Enum.GetName(typeof(ItemDefinitionLevel), list.Level));
          levelDict.Add(list.Level, levelNode);
          _availableItemsRootNode.Nodes.Add(levelNode);
        }

        var listNode = new NGTreeNode(list.List.Name);
        foreach (var color in list.List)
          listNode.Nodes.Add(new NGTreeNode(ToDisplayName(color)) { Tag = color });
        levelNode.Nodes.Add(listNode);
      }
    }

    protected override void Controller_CurrentItems_Initialize()
    {
      if (null == _currentItems)
        _currentItems = new SelectableListNodeList();
      else
        _currentItems.Clear();

      foreach (var color in _doc)
      {
        _currentItems.Add(new SelectableListNode(ToDisplayName(color), color, false));
      }
    }

    protected override void EhAvailableItem_AddToCurrent()
    {
      var avNodes = _availableItemsRootNode.TakeFromHereToFirstLeaves(false).Where(node => node.IsSelected && node.Tag is NamedColor).Select(node => (NamedColor)node.Tag).ToArray();
      if (avNodes.Length == 0)
        return;

      foreach (var namedColor in avNodes)
        _currentItems.Add(new SelectableListNode(ToDisplayName(namedColor), namedColor, false));
      SetListDirty();
    }

    protected override bool IsItemEditable(Altaxo.Main.IImmutable item)
    {
      if (null == item)
        return false;

      return true;
    }
  }
}
