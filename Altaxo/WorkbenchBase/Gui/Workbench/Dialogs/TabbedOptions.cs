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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Altaxo.Gui.Settings;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// TabControl for option panels.
  /// </summary>
  public class TabbedOptions : TabControl, ICanBeDirty
  {
    public void AddOptionPanels(IEnumerable<IOptionPanelDescriptor> dialogPanelDescriptors)
    {
      if (dialogPanelDescriptors is null)
        throw new ArgumentNullException("dialogPanelDescriptors");
      foreach (IOptionPanelDescriptor descriptor in dialogPanelDescriptors)
      {
        if (descriptor is not null)
        {
          if (descriptor.HasOptionPanel)
          {
            Items.Add(new OptionTabPage(this, descriptor));
          }
          AddOptionPanels(descriptor.ChildOptionPanelDescriptors);
        }
      }
      OnIsDirtyChanged(null, null);
    }

    private bool oldIsDirty;

    public bool IsDirty
    {
      get
      {
        return oldIsDirty;
      }
    }

    public event EventHandler IsDirtyChanged;

    private void OnIsDirtyChanged(object sender, EventArgs e)
    {
      bool isDirty = false;
      foreach (IOptionPanel op in OptionPanels)
      {
        var dirty = op as ICanBeDirty;
        if (dirty is not null && dirty.IsDirty)
        {
          isDirty = true;
          break;
        }
      }
      if (isDirty != oldIsDirty)
      {
        oldIsDirty = isDirty;
        if (IsDirtyChanged is not null)
          IsDirtyChanged(this, EventArgs.Empty);
      }
    }

    public IEnumerable<IOptionPanel> OptionPanels
    {
      get
      {
        return
          from tp in Items.OfType<OptionTabPage>()
          where tp.optionPanel is not null
          select tp.optionPanel;
      }
    }

    private sealed class OptionTabPage : TabItem
    {
      private IOptionPanelDescriptor descriptor;
      private TextBlock placeholder;
      internal IOptionPanel optionPanel;
      private TabbedOptions options;

      public OptionTabPage(TabbedOptions options, IOptionPanelDescriptor descriptor)
      {
        this.options = options;
        this.descriptor = descriptor;
        string title = StringParser.Parse(descriptor.Label);
        Header = title;
        placeholder = new TextBlock { Text = title };
        placeholder.IsVisibleChanged += placeholder_IsVisibleChanged;
        Content = placeholder;
      }

      private void placeholder_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(LoadPadContentIfRequired));
      }

      private void LoadPadContentIfRequired()
      {
        if (placeholder is not null && placeholder.IsVisible)
        {
          placeholder = null;
          optionPanel = descriptor.OptionPanel;
          if (optionPanel is not null)
          {
            // some panels initialize themselves on the first LoadOptions call,
            // so we need to load the options before attaching event handlers
            //optionPanel.LoadOptions(); // TODO LelliD 2017-11-20 reimplement this

            var dirty = optionPanel as ICanBeDirty;
            if (dirty is not null)
              dirty.IsDirtyChanged += options.OnIsDirtyChanged;
            Altaxo.Current.GetRequiredService<IWinFormsService>().SetContent(this, optionPanel.ViewObject);
          }
        }
      }
    }
  }
}
