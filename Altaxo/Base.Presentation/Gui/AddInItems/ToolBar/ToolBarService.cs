// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Altaxo.AddInItems;
using Altaxo.Main.Services;

namespace Altaxo.Gui.AddInItems
{
  /// <summary>
  /// Creates WPF toolbars from the AddIn Tree.
  /// </summary>
  public static class ToolBarService
  {
    /// <summary>
    /// Style key used for toolbar images.
    /// </summary>
    public static readonly ResourceKey ImageStyleKey = new ComponentResourceKey(typeof(ToolBarService), "ImageStyle");

    /// <summary>
    /// Updates the status of the tool bar items
    /// </summary>
    /// <param name="toolBarItems">The tool bar items.</param>
    /// <remarks>The workbench calls <see cref="IStatusUpdate.UpdateStatus"/> for all tool bars when
    /// <see cref="System.Windows.Input.CommandManager.RequerySuggested"/> fires.</remarks>
    public static void UpdateStatus(IEnumerable toolBarItems)
    {
      MenuService.UpdateStatus(toolBarItems);
    }

    public static IList CreateToolBarItems(UIElement inputBindingOwner, object owner, string addInTreePath)
    {
      return CreateToolBarItems(inputBindingOwner, AddInTree.BuildItems<ToolbarItemDescriptor>(addInTreePath, owner, false));
    }

    private static IList CreateToolBarItems(UIElement inputBindingOwner, IEnumerable descriptors)
    {
      var result = new List<object>();
      foreach (ToolbarItemDescriptor descriptor in descriptors)
      {
        object item = CreateToolBarItemFromDescriptor(inputBindingOwner, descriptor);
        var submenuBuilder = item as IMenuItemBuilder;
        if (submenuBuilder is not null)
        {
          result.AddRange(submenuBuilder.BuildItems(descriptor.Codon, descriptor.Parameter));
        }
        else
        {
          result.Add(item);
        }
      }
      return result;
    }

    private static object CreateToolBarItemFromDescriptor(UIElement inputBindingOwner, ToolbarItemDescriptor descriptor)
    {
      Codon codon = descriptor.Codon;
      object caller = descriptor.Parameter;
      string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Item";

      switch (type)
      {
        case "Separator":
          return new ConditionalSeparator(codon, caller, true, descriptor.Conditions);

        case "CheckBox":
          return new ToolBarCheckBox(codon, caller, descriptor.Conditions);

        case "Item":
          return new ToolBarButton(inputBindingOwner, codon, caller, descriptor.Conditions);

        case "DropDownButton":
          return new ToolBarDropDownButton(
            codon, caller, MenuService.CreateUnexpandedMenuItems(
              new MenuService.MenuCreateContext { ActivationMethod = "ToolbarDropDownMenu" },
              descriptor.SubItems), descriptor.Conditions);

        case "SplitButton":
          return new ToolBarSplitButton(
            codon, caller, MenuService.CreateUnexpandedMenuItems(
              new MenuService.MenuCreateContext { ActivationMethod = "ToolbarDropDownMenu" },
              descriptor.SubItems), descriptor.Conditions);

        case "Builder":
          return codon.AddIn.CreateObject(codon.Properties["class"]);

        case "Custom":
          var resultType = codon.AddIn.FindType(codon.Properties["class"]);
          if (resultType is not null)
          {
            object result = null;
            var c1 = resultType.GetConstructor(new Type[] { typeof(Codon), typeof(object), typeof(IReadOnlyCollection<ICondition>) });
            result = c1?.Invoke(new object[] { codon, caller, descriptor.Conditions });
            if (result is null)
            {
              result = Activator.CreateInstance(resultType);
            }
            if (result is ComboBox cb)
              cb.SetResourceReference(FrameworkElement.StyleProperty, ToolBar.ComboBoxStyleKey);
            if (result is ICustomToolBarItem ctbi)
              ctbi.Initialize(inputBindingOwner, codon, caller);
            return result;
          }
          else
            throw new System.NotSupportedException("Unsupported: Custom item must contain a class name");

        default:
          throw new System.NotSupportedException("unsupported menu item type : " + type);
      }
    }

    private static ToolBar CreateToolBar(UIElement inputBindingOwner, object owner, AddInTreeNode treeNode)
    {
      ToolBar tb = new CoreToolBar();
      ToolBarTray.SetIsLocked(tb, true);
      tb.ItemsSource = CreateToolBarItems(inputBindingOwner, treeNode.BuildChildItems<ToolbarItemDescriptor>(owner));
      UpdateStatus(tb.ItemsSource); // setting Visible is only possible after the items have been added
      return tb;
    }

    private sealed class CoreToolBar : ToolBar
    {
      public CoreToolBar()
      {
        LanguageChangeWeakEventManager.LanguageChanged += EhLanguageChanged;
      }

      private void EhLanguageChanged()
      {
        MenuService.UpdateText(ItemsSource);
      }
    }

    public static ToolBar CreateToolBar(UIElement inputBindingOwner, object owner, string addInTreePath)
    {
      return CreateToolBar(inputBindingOwner, owner, AddInTree.GetTreeNode(addInTreePath));
    }

    public static ToolBar[] CreateToolBars(UIElement inputBindingOwner, object owner, string addInTreePath)
    {
      AddInTreeNode treeNode;
      try
      {
        treeNode = AddInTree.GetTreeNode(addInTreePath);
      }
      catch (TreePathNotFoundException)
      {
        return null;
      }
      var toolBars = new List<ToolBar>();
      foreach (AddInTreeNode childNode in treeNode.ChildNodes.Values)
      {
        toolBars.Add(CreateToolBar(inputBindingOwner, owner, childNode));
      }
      return toolBars.ToArray();
    }

    public static ToolBar[] CreateToolBars(object uiElementInputBindingOwner, object owner, string addInTreePath)
    {
      if (!(uiElementInputBindingOwner is UIElement))
        throw new ArgumentException("Parameter must be an UIElement!", nameof(uiElementInputBindingOwner));

      return CreateToolBars((UIElement)uiElementInputBindingOwner, owner, addInTreePath);
    }

    internal static object CreateToolBarItemContent(Codon codon)
    {
      object result = null;
      Image image = null;
      Label label = null;
      bool isImage = false;
      bool isLabel = false;
      if (codon.Properties.Contains("icon"))
      {
        image = new Image
        {
          Source = PresentationResourceService.GetBitmapSource(StringParser.Parse(codon.Properties["icon"])),
          Height = 16
        };
        image.SetResourceReference(FrameworkElement.StyleProperty, ToolBarService.ImageStyleKey);
        isImage = true;
      }
      if (codon.Properties.Contains("label"))
      {
        label = new Label
        {
          Content = StringParser.Parse(codon.Properties["label"]),
          Padding = new Thickness(0),
          VerticalContentAlignment = VerticalAlignment.Center
        };
        isLabel = true;
      }

      if (isImage && isLabel)
      {
        var panel = new StackPanel
        {
          Orientation = Orientation.Horizontal
        };
        image.Margin = new Thickness(0, 0, 5, 0);
        panel.Children.Add(image);
        panel.Children.Add(label);
        result = panel;
      }
      else
        if (isImage)
      {
        result = image;
      }
      else
        if (isLabel)
      {
        result = label;
      }
      else
      {
        result = codon.Id;
      }

      return result;
    }
  }

  public interface ICustomToolBarItem
  {
    void Initialize(UIElement inputBindingOwner, Codon codon, object owner);
  }
}
