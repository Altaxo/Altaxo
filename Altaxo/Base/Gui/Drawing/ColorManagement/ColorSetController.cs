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
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Main;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  public interface IColorListView : IStyleListView
  {
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

      CmdAddCustomColorToList = new RelayCommand(EhUserRequest_AddCustomColorToList);
      CmdForAllSelectedItemsSetOpacity = new RelayCommand(EhUserRequest_ForAllSelectedItemSetOpacity);
      CmdForAllSelectedItemsShiftHue = new RelayCommand(EhUserRequest_ForAllSelectedItemShiftHue);
      CmdForAllSelectedItemsSetSaturation = new RelayCommand(EhUserRequest_ForAllSelectedItemSetSaturation);
      CmdForAllSelectedItemsSetBrightness = new RelayCommand(EhUserRequest_ForAllSelectedItemSetBrightness);
      CmdForAllSelectedItemsSetColorName = new RelayCommand(EhUserRequest_ForAllSelectedItemSetColorName);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      IEnumerable<ControllerAndSetNullMethod> GetMySubControllers()
      {
        yield return new ControllerAndSetNullMethod(_customColorController, () => _customColorController = null);
      }

      return base.GetSubControllers().Concat(GetMySubControllers());
    }

    #region Bindings

    public ICommand CmdAddCustomColorToList { get; }

    public ICommand CmdForAllSelectedItemsSetOpacity { get; }
    public ICommand CmdForAllSelectedItemsShiftHue { get; }
    public ICommand CmdForAllSelectedItemsSetSaturation { get; }
    public ICommand CmdForAllSelectedItemsSetBrightness { get; }
    public ICommand CmdForAllSelectedItemsSetColorName { get; }

    public NamedColorController CustomColorController => _customColorController;

    private double _opacity = 100;

    public double Opacity
    {
      get => _opacity;
      set
      {
        if (!(_opacity == value))
        {
          _opacity = value;
          OnPropertyChanged(nameof(Opacity));
        }
      }
    }

    private double _shiftHue = 100;

    public double ShiftHue
    {
      get => _shiftHue;
      set
      {
        if (!(_shiftHue == value))
        {
          _shiftHue = value;
          OnPropertyChanged(nameof(ShiftHue));
        }
      }
    }

    private double _saturation = 100;

    public double Saturation
    {
      get => _saturation;
      set
      {
        if (!(_saturation == value))
        {
          _saturation = value;
          OnPropertyChanged(nameof(Saturation));
        }
      }
    }

    private double _brighness = 100;

    public double Brighness
    {
      get => _brighness;
      set
      {
        if (!(_brighness == value))
        {
          _brighness = value;
          OnPropertyChanged(nameof(Brighness));
        }
      }
    }

    private string _colorName;

    public string ColorName
    {
      get => _colorName;
      set
      {
        if (!(_colorName == value))
        {
          _colorName = value;
          OnPropertyChanged(nameof(ColorName));
        }
      }
    }





    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _customColorController = new NamedColorController();
        _customColorController.InitializeDocument(NamedColors.White);
      
        if (_customColorController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_customColorController);
      }
    }

    private void EhUserRequest_AddCustomColorToList()
    {
      if (_customColorController.Apply(false))
      {
        var namedColor = (NamedColor)_customColorController.ModelObject;
        CurrentItems.Add(new SelectableListNode(ToDisplayName(namedColor), namedColor, false));
        SetListDirty();
      }
    }

    private void EhUserRequest_ForAllSelectedItemSetOpacity()
    {
      var alphaValue = AxoColor.NormFloatToByte((float)Opacity);

      bool anyChange = false;
      foreach (var item in CurrentItems.Where(node => node.IsSelected))
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

    private void EhUserRequest_ForAllSelectedItemShiftHue()
    {
      var hueShift = ShiftHue;
      if (0 == hueShift || -1 == hueShift || 1 == hueShift)
        return;

      bool anyChange = false;
      foreach (var item in CurrentItems.Where(node => node.IsSelected))
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

    private void EhUserRequest_ForAllSelectedItemSetSaturation()
    {
      var saturation = Saturation;
      bool anyChange = false;
      foreach (var item in CurrentItems.Where(node => node.IsSelected))
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

    private void EhUserRequest_ForAllSelectedItemSetBrightness()
    {
      var brightness = Brighness;
      bool anyChange = false;
      foreach (var item in CurrentItems.Where(node => node.IsSelected))
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

    private void EhUserRequest_ForAllSelectedItemSetColorName()
    {
      var baseName = ColorName;
      bool anyChange = false;

      var items = CurrentItems.Where(node => node.IsSelected).ToArray();

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
      if (_availableItemsRootNode is null)
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
      if (CurrentItems is null)
        CurrentItems = new SelectableListNodeList();
      else
        CurrentItems.Clear();

      foreach (var color in _doc)
      {
        CurrentItems.Add(new SelectableListNode(ToDisplayName(color), color, false));
      }
    }

    protected override void EhAvailableItem_AddToCurrent()
    {
      var avNodes = _availableItemsRootNode.TakeFromHereToFirstLeaves(false).Where(node => node.IsSelected && node.Tag is NamedColor).Select(node => (NamedColor)node.Tag).ToArray();
      if (avNodes.Length == 0)
        return;

      foreach (var namedColor in avNodes)
        CurrentItems.Add(new SelectableListNode(ToDisplayName(namedColor), namedColor, false));
      SetListDirty();
    }

    protected override bool IsItemEditable(Altaxo.Main.IImmutable item)
    {
      if (item is null)
        return false;

      return true;
    }
  }
}
