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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Altaxo.Collections;
using Altaxo.Geometry;

namespace Altaxo.Gui
{
  public static partial class GuiHelper
  {
    #region Combobox

    public static void Initialize(ComboBox view, SelectableListNodeList data)
    {
      int idx = data.FirstSelectedNodeIndex; // Note: the selected index must be determined _before_ the data are bound to the box (otherwise when a binding is in place, it can happen that the selection is resetted)

      if (view.ItemsSource != data)
      {
        //view.ItemsSource = null;
        view.ItemsSource = data;
      }

      if (idx >= 0)
        view.SelectedItem = data[idx];
    }

    /// <summary>
    /// Initializes a combobox. If none of the provided nodes is selected, the selection
    /// of this combobox will cleared.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="data">The data.</param>
    public static void InitializeDeselectable(ComboBox view, SelectableListNodeList data)
    {
      int idx = data.FirstSelectedNodeIndex; // Note: the selected index must be determined _before_ the data are bound to the box (otherwise when a binding is in place, it can happen that the selection is resetted)

      if (view.ItemsSource != data)
      {
        //view.ItemsSource = null;
        view.ItemsSource = data;
      }

      if (idx >= 0)
        view.SelectedItem = data[idx];
      else
        view.SelectedItem = null;
    }

    public static void SynchronizeSelectionFromGui(ComboBox view)
    {
      foreach (ISelectableItem it in view.ItemsSource)
        it.IsSelected = false;

      if (view.SelectedItem is not null)
        ((ISelectableItem)view.SelectedItem).IsSelected = true;
    }

    #endregion Combobox

    #region ListBox

    public static void Initialize(ListBox view, SelectableListNodeList data)
    {
      view.ItemsSource = null;
      view.ItemsSource = data;
      if (view.SelectionMode == SelectionMode.Single)
      {
        view.SelectedIndex = data.FirstSelectedNodeIndex;
      }
      else
      {
        foreach (var n in data)
          if (n.IsSelected)
            view.SelectedItems.Add(n);
      }
    }

    /// <summary>
    /// Sets the items of a list box with <see cref="CheckableSelectableListNode"/> items. We presume here that the ListBox has an appropriate DataTemplate, thus only the ItemsSource
    /// property of the ListBox is set with the data.
    /// </summary>
    /// <param name="view">ListBox to set.</param>
    /// <param name="data">The data to set for the ListBox.</param>
    public static void Initialize(ListBox view, CheckableSelectableListNodeList data)
    {
      view.ItemsSource = data;
    }

    public static void SynchronizeSelectionFromGui(ListBox view)
    {
      if (view.ItemsSource is not null)
      {
        foreach (ISelectableItem it in view.ItemsSource)
          it.IsSelected = false;
      }

      foreach (ISelectableItem it in view.SelectedItems)
        it.IsSelected = true;
    }

    #endregion ListBox

    #region ListView

    public static void Initialize(ListView view, SelectableListNodeList data)
    {
      view.ItemsSource = null;
      view.ItemsSource = data;

      if (view.SelectionMode == SelectionMode.Single)
      {
        view.SelectedIndex = data.FirstSelectedNodeIndex;
      }
      else
      {
        if (data is not null)
        {
          foreach (var n in data)
            if (n.IsSelected)
              view.SelectedItems.Add(n);
        }
      }
    }

    public static void Refresh(ListView view)
    {
      var h = view.ItemsSource;
      view.ItemsSource = null;
      view.ItemsSource = h;
    }

    public static void SynchronizeSelectionFromGui(ListView listView)
    {
      if (listView.ItemsSource is not null)
      {
        foreach (ISelectableItem it in listView.ItemsSource)
          it.IsSelected = false;
      }

      foreach (ISelectableItem it in listView.SelectedItems)
        it.IsSelected = true;
    }

    public static double[] GetColumnWidths(ListView listView)
    {
      var gv = (GridView)listView.View;

      var ret = new double[gv.Columns.Count];
      for (int i = 0; i < ret.Length; i++)
        ret[i] = gv.Columns[i].ActualWidth;
      return ret;
    }

    public static void SetColumnWidths(ListView listView, double[] widths)
    {
      var gv = (GridView)listView.View;

      int len = Math.Min(widths.Length, gv.Columns.Count);
      for (int i = 0; i < len; i++)
        gv.Columns[i].Width = widths[i];
    }

    /// <summary>
    /// Sets the column names of a list view and set up the binding to items that derive from <see cref="Altaxo.Collections.ListNode"/>
    /// </summary>
    /// <param name="listView">The ListView where the columns to set.</param>
    /// <param name="columnHeaders">The text of the column headers.</param>
    /// <remarks>The first column is bind to the property "Text" of the items, the next columns to the property "Text1", "Text2", and so on.</remarks>
    public static void InitializeListViewColumnsAndBindToListNode(ListView listView, string[] columnHeaders)
    {
      if ((listView.View as GridView) is null)
        listView.View = new GridView();

      var grid = listView.View as GridView;

      grid.Columns.Clear();

      int colNo = -1;
      foreach (var colName in columnHeaders)
      {
        ++colNo;

        var gvCol = new GridViewColumn() { Header = colName };
        var binding = new Binding(colNo == 0 ? "Text " : "Text" + colNo.ToString());
        gvCol.DisplayMemberBinding = binding;
        grid.Columns.Add(gvCol);
      }
    }

    #endregion ListView

    #region TabControl

    public static void Initialize(TabControl view, SelectableListNodeList data)
    {
      int idx = data.FirstSelectedNodeIndex; // Note: the selected index must be determined _before_ the data are bound to the box (otherwise when a binding is in place, it can happen that the selection is resetted)

      if (view.ItemsSource != data)
      {
        //view.ItemsSource = null;
        view.ItemsSource = data;
      }

      if (idx >= 0)
        view.SelectedItem = data[idx];
    }

    public static void SynchronizeSelectionFromGui(TabControl view)
    {
      foreach (ISelectableItem it in view.ItemsSource)
        it.IsSelected = false;

      if (view.SelectedItem is not null)
        ((ISelectableItem)view.SelectedItem).IsSelected = true;
    }

    #endregion TabControl

    #region Logical tree

    /// <summary>
    /// Gets the object itself or the first recursive parent of the object that has the type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to find.</typeparam>
    /// <param name="child">The object where the search starts.</param>
    /// <returns>The object itself or the first recursive parent of the object which is of type T. If no such parent exist, <c>null</c> is returned.</returns>
    public static T GetLogicalParentOfType<T>(DependencyObject child) where T : DependencyObject
    {
      if (child is null)
        return null;

      if (child is T)
        return (T)child;

      return GetLogicalParentOfType<T>(LogicalTreeHelper.GetParent(child));
    }

    #endregion Logical tree

    #region Conversion to / from Xaml string

    /// <summary>
    /// Registers a converter for a provided type.
    /// </summary>
    /// <typeparam name="T">The type for which the converter is to be registered.</typeparam>
    /// <typeparam name="TC">The type of the converter.</typeparam>
    public static void RegisterConverter<T, TC>()
    {
      var attr = new Attribute[1];
      var vConv = new TypeConverterAttribute(typeof(TC));
      attr[0] = vConv;
      TypeDescriptor.AddAttributes(typeof(T), attr);
    }

    private static bool _converterForBindingExpressionRegistered;

    /// <summary>
    /// Clones a framework element and all elements under it, including Bindings.
    /// </summary>
    /// <param name="elementToClone">The element to clone.</param>
    /// <returns>The cloned element. Please keep in mind that the cloned element (and the elements under it) have the same name than the original.</returns>
    /// <remarks>For credit and details see <see href="http://stackoverflow.com/questions/32541/how-can-you-clone-a-wpf-object"/>.</remarks>
    public static FrameworkElement CloneFrameworkElement(FrameworkElement elementToClone)
    {
      if (!_converterForBindingExpressionRegistered)
      {
        GuiHelper.RegisterConverter<BindingExpression, Common.Converters.BindingConverter>();
        _converterForBindingExpressionRegistered = true;
      }

      var sb = new StringBuilder();
      var writer = XmlWriter.Create(sb, new XmlWriterSettings
      {
        Indent = true,
        ConformanceLevel = ConformanceLevel.Fragment,
        OmitXmlDeclaration = true,
        NamespaceHandling = NamespaceHandling.OmitDuplicates,
      });
      var mgr = new XamlDesignerSerializationManager(writer)
      {

        // HERE BE MAGIC!!!
        XamlWriterMode = XamlWriterMode.Expression
      };
      // THERE WERE MAGIC!!!

      System.Windows.Markup.XamlWriter.Save(elementToClone, mgr);

      string frameworkElementAsXamlString = sb.ToString();

      // now deserialize it again

      var stringReader = new System.IO.StringReader(frameworkElementAsXamlString);
      var xmlReader = XmlReader.Create(stringReader);
      var clonedFrameworkElement = (FrameworkElement)XamlReader.Load(xmlReader);
      return clonedFrameworkElement;
    }

    #endregion Conversion to / from Xaml string

    #region Mouse

    public static Altaxo.Gui.AltaxoMouseButtons GetMouseState(MouseDevice mouse)
    {
      var result = Altaxo.Gui.AltaxoMouseButtons.None;
      if (MouseButtonState.Pressed == mouse.LeftButton)
        result |= AltaxoMouseButtons.Left;
      if (MouseButtonState.Pressed == mouse.MiddleButton)
        result |= AltaxoMouseButtons.Middle;
      if (MouseButtonState.Pressed == mouse.RightButton)
        result |= AltaxoMouseButtons.Right;
      if (MouseButtonState.Pressed == mouse.XButton1)
        result |= AltaxoMouseButtons.XButton1;
      if (MouseButtonState.Pressed == mouse.XButton2)
        result |= AltaxoMouseButtons.XButton2;

      return result;
    }

    public static Altaxo.Gui.AltaxoMouseButtons GetMouseState(MouseEventArgs mouse)
    {
      var result = Altaxo.Gui.AltaxoMouseButtons.None;
      if (MouseButtonState.Pressed == mouse.LeftButton)
        result |= AltaxoMouseButtons.Left;
      if (MouseButtonState.Pressed == mouse.MiddleButton)
        result |= AltaxoMouseButtons.Middle;
      if (MouseButtonState.Pressed == mouse.RightButton)
        result |= AltaxoMouseButtons.Right;
      if (MouseButtonState.Pressed == mouse.XButton1)
        result |= AltaxoMouseButtons.XButton1;
      if (MouseButtonState.Pressed == mouse.XButton2)
        result |= AltaxoMouseButtons.XButton2;

      return result;
    }

    public static Altaxo.Gui.AltaxoMouseButtons ToAltaxo(MouseButton mouseButton)
    {
      switch (mouseButton)
      {
        case MouseButton.Left:
          return AltaxoMouseButtons.Left;

        case MouseButton.Middle:
          return AltaxoMouseButtons.Middle;

        case MouseButton.Right:
          return AltaxoMouseButtons.Right;

        case MouseButton.XButton1:
          return AltaxoMouseButtons.XButton1;

        case MouseButton.XButton2:
          return AltaxoMouseButtons.XButton2;

        default:
          return AltaxoMouseButtons.None;
      }
    }

    public static Altaxo.Gui.AltaxoMouseEventArgs ToAltaxo(MouseButtonEventArgs e, IInputElement positionReference)
    {
      var pos = e.GetPosition(positionReference);

      var result = new Altaxo.Gui.AltaxoMouseEventArgs
      {
        Button = ToAltaxo(e.ChangedButton),
        Clicks = e.ClickCount,
        Delta = 0,
        X = pos.X,
        Y = pos.Y
      };

      return result;
    }

    public static Altaxo.Gui.AltaxoMouseEventArgs ToAltaxo(MouseWheelEventArgs e, IInputElement positionReference)
    {
      var pos = e.GetPosition(positionReference);

      var result = new Altaxo.Gui.AltaxoMouseEventArgs
      {
        Button = AltaxoMouseButtons.None,
        Clicks = 0,
        Delta = e.Delta,
        X = pos.X,
        Y = pos.Y
      };

      return result;
    }

    #region Miscellaneous

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint GetDoubleClickTime();

    /// <summary>
    /// Gets the double click time in ms.
    /// </summary>
    /// <value>
    /// The double click time.
    /// </value>
    public static int DoubleClickTime
    {
      get
      {
        return (int)GetDoubleClickTime();
      }
    }

    #endregion Miscellaneous

    #endregion Mouse

    #region Keyboard

    public static AltaxoKeyboardKey ToAltaxo(System.Windows.Input.Key key)
    {
      switch (key)
      {
        case Key.None:
          return AltaxoKeyboardKey.None;
        case Key.Cancel:
          return AltaxoKeyboardKey.Cancel;
        case Key.Back:
          return AltaxoKeyboardKey.Back;
        case Key.Tab:
          return AltaxoKeyboardKey.Tab;
        case Key.LineFeed:
          return AltaxoKeyboardKey.LineFeed;
        case Key.Clear:
          return AltaxoKeyboardKey.Clear;
        case Key.Return:
          return AltaxoKeyboardKey.Return;
        case Key.Pause:
          return AltaxoKeyboardKey.Pause;
        case Key.Capital:
          return AltaxoKeyboardKey.Capital;
        case Key.KanaMode:
          return AltaxoKeyboardKey.KanaMode;
        case Key.JunjaMode:
          return AltaxoKeyboardKey.JunjaMode;
        case Key.FinalMode:
          return AltaxoKeyboardKey.FinalMode;
        case Key.HanjaMode:
          return AltaxoKeyboardKey.HanjaMode;
        case Key.Escape:
          return AltaxoKeyboardKey.Escape;
        case Key.ImeConvert:
          return AltaxoKeyboardKey.ImeConvert;
        case Key.ImeNonConvert:
          return AltaxoKeyboardKey.ImeNonConvert;
        case Key.ImeAccept:
          return AltaxoKeyboardKey.ImeAccept;
        case Key.ImeModeChange:
          return AltaxoKeyboardKey.ImeModeChange;
        case Key.Space:
          return AltaxoKeyboardKey.Space;
        case Key.PageUp:
          return AltaxoKeyboardKey.PageUp;
        case Key.Next:
          return AltaxoKeyboardKey.Next;
        case Key.End:
          return AltaxoKeyboardKey.End;
        case Key.Home:
          return AltaxoKeyboardKey.Home;
        case Key.Left:
          return AltaxoKeyboardKey.Left;
        case Key.Up:
          return AltaxoKeyboardKey.Up;
        case Key.Right:
          return AltaxoKeyboardKey.Right;
        case Key.Down:
          return AltaxoKeyboardKey.Down;
        case Key.Select:
          return AltaxoKeyboardKey.Select;
        case Key.Print:
          return AltaxoKeyboardKey.Print;
        case Key.Execute:
          return AltaxoKeyboardKey.Execute;
        case Key.Snapshot:
          return AltaxoKeyboardKey.Snapshot;
        case Key.Insert:
          return AltaxoKeyboardKey.Insert;
        case Key.Delete:
          return AltaxoKeyboardKey.Delete;
        case Key.Help:
          return AltaxoKeyboardKey.Help;
        case Key.D0:
          return AltaxoKeyboardKey.D0;
        case Key.D1:
          return AltaxoKeyboardKey.D1;
        case Key.D2:
          return AltaxoKeyboardKey.D2;
        case Key.D3:
          return AltaxoKeyboardKey.D3;
        case Key.D4:
          return AltaxoKeyboardKey.D4;
        case Key.D5:
          return AltaxoKeyboardKey.D5;
        case Key.D6:
          return AltaxoKeyboardKey.D6;
        case Key.D7:
          return AltaxoKeyboardKey.D7;
        case Key.D8:
          return AltaxoKeyboardKey.D8;
        case Key.D9:
          return AltaxoKeyboardKey.D9;
        case Key.A:
          return AltaxoKeyboardKey.A;
        case Key.B:
          return AltaxoKeyboardKey.B;
        case Key.C:
          return AltaxoKeyboardKey.C;
        case Key.D:
          return AltaxoKeyboardKey.D;
        case Key.E:
          return AltaxoKeyboardKey.E;
        case Key.F:
          return AltaxoKeyboardKey.F;
        case Key.G:
          return AltaxoKeyboardKey.G;
        case Key.H:
          return AltaxoKeyboardKey.H;
        case Key.I:
          return AltaxoKeyboardKey.I;
        case Key.J:
          return AltaxoKeyboardKey.J;
        case Key.K:
          return AltaxoKeyboardKey.K;
        case Key.L:
          return AltaxoKeyboardKey.L;
        case Key.M:
          return AltaxoKeyboardKey.M;
        case Key.N:
          return AltaxoKeyboardKey.N;
        case Key.O:
          return AltaxoKeyboardKey.O;
        case Key.P:
          return AltaxoKeyboardKey.P;
        case Key.Q:
          return AltaxoKeyboardKey.Q;
        case Key.R:
          return AltaxoKeyboardKey.R;
        case Key.S:
          return AltaxoKeyboardKey.S;
        case Key.T:
          return AltaxoKeyboardKey.T;
        case Key.U:
          return AltaxoKeyboardKey.U;
        case Key.V:
          return AltaxoKeyboardKey.V;
        case Key.W:
          return AltaxoKeyboardKey.W;
        case Key.X:
          return AltaxoKeyboardKey.X;
        case Key.Y:
          return AltaxoKeyboardKey.Y;
        case Key.Z:
          return AltaxoKeyboardKey.Z;
        case Key.LWin:
          return AltaxoKeyboardKey.LWin;
        case Key.RWin:
          return AltaxoKeyboardKey.RWin;
        case Key.Apps:
          return AltaxoKeyboardKey.Apps;
        case Key.Sleep:
          return AltaxoKeyboardKey.Sleep;
        case Key.NumPad0:
          return AltaxoKeyboardKey.NumPad0;
        case Key.NumPad1:
          return AltaxoKeyboardKey.NumPad1;
        case Key.NumPad2:
          return AltaxoKeyboardKey.NumPad2;
        case Key.NumPad3:
          return AltaxoKeyboardKey.NumPad3;
        case Key.NumPad4:
          return AltaxoKeyboardKey.NumPad4;
        case Key.NumPad5:
          return AltaxoKeyboardKey.NumPad5;
        case Key.NumPad6:
          return AltaxoKeyboardKey.NumPad6;
        case Key.NumPad7:
          return AltaxoKeyboardKey.NumPad7;
        case Key.NumPad8:
          return AltaxoKeyboardKey.NumPad8;
        case Key.NumPad9:
          return AltaxoKeyboardKey.NumPad9;
        case Key.Multiply:
          return AltaxoKeyboardKey.Multiply;
        case Key.Add:
          return AltaxoKeyboardKey.Add;
        case Key.Separator:
          return AltaxoKeyboardKey.Separator;
        case Key.Subtract:
          return AltaxoKeyboardKey.Subtract;
        case Key.Decimal:
          return AltaxoKeyboardKey.Decimal;
        case Key.Divide:
          return AltaxoKeyboardKey.Divide;
        case Key.F1:
          return AltaxoKeyboardKey.F1;
        case Key.F2:
          return AltaxoKeyboardKey.F2;
        case Key.F3:
          return AltaxoKeyboardKey.F3;
        case Key.F4:
          return AltaxoKeyboardKey.F4;
        case Key.F5:
          return AltaxoKeyboardKey.F5;
        case Key.F6:
          return AltaxoKeyboardKey.F6;
        case Key.F7:
          return AltaxoKeyboardKey.F7;
        case Key.F8:
          return AltaxoKeyboardKey.F8;
        case Key.F9:
          return AltaxoKeyboardKey.F9;
        case Key.F10:
          return AltaxoKeyboardKey.F10;
        case Key.F11:
          return AltaxoKeyboardKey.F11;
        case Key.F12:
          return AltaxoKeyboardKey.F12;
        case Key.F13:
          return AltaxoKeyboardKey.F13;
        case Key.F14:
          return AltaxoKeyboardKey.F14;
        case Key.F15:
          return AltaxoKeyboardKey.F15;
        case Key.F16:
          return AltaxoKeyboardKey.F16;
        case Key.F17:
          return AltaxoKeyboardKey.F17;
        case Key.F18:
          return AltaxoKeyboardKey.F18;
        case Key.F19:
          return AltaxoKeyboardKey.F19;
        case Key.F20:
          return AltaxoKeyboardKey.F20;
        case Key.F21:
          return AltaxoKeyboardKey.F21;
        case Key.F22:
          return AltaxoKeyboardKey.F22;
        case Key.F23:
          return AltaxoKeyboardKey.F23;
        case Key.F24:
          return AltaxoKeyboardKey.F24;
        case Key.NumLock:
          return AltaxoKeyboardKey.NumLock;
        case Key.Scroll:
          return AltaxoKeyboardKey.Scroll;
        case Key.LeftShift:
          return AltaxoKeyboardKey.LeftShift;
        case Key.RightShift:
          return AltaxoKeyboardKey.RightShift;
        case Key.LeftCtrl:
          return AltaxoKeyboardKey.LeftCtrl;
        case Key.RightCtrl:
          return AltaxoKeyboardKey.RightCtrl;
        case Key.LeftAlt:
          return AltaxoKeyboardKey.LeftAlt;
        case Key.RightAlt:
          return AltaxoKeyboardKey.RightAlt;
        case Key.BrowserBack:
          return AltaxoKeyboardKey.BrowserBack;
        case Key.BrowserForward:
          return AltaxoKeyboardKey.BrowserForward;
        case Key.BrowserRefresh:
          return AltaxoKeyboardKey.BrowserRefresh;
        case Key.BrowserStop:
          return AltaxoKeyboardKey.BrowserStop;
        case Key.BrowserSearch:
          return AltaxoKeyboardKey.BrowserSearch;
        case Key.BrowserFavorites:
          return AltaxoKeyboardKey.BrowserFavorites;
        case Key.BrowserHome:
          return AltaxoKeyboardKey.BrowserHome;
        case Key.VolumeMute:
          return AltaxoKeyboardKey.VolumeMute;
        case Key.VolumeDown:
          return AltaxoKeyboardKey.VolumeDown;
        case Key.VolumeUp:
          return AltaxoKeyboardKey.VolumeUp;
        case Key.MediaNextTrack:
          return AltaxoKeyboardKey.MediaNextTrack;
        case Key.MediaPreviousTrack:
          return AltaxoKeyboardKey.MediaPreviousTrack;
        case Key.MediaStop:
          return AltaxoKeyboardKey.MediaStop;
        case Key.MediaPlayPause:
          return AltaxoKeyboardKey.MediaPlayPause;
        case Key.LaunchMail:
          return AltaxoKeyboardKey.LaunchMail;
        case Key.SelectMedia:
          return AltaxoKeyboardKey.SelectMedia;
        case Key.LaunchApplication1:
          return AltaxoKeyboardKey.LaunchApplication1;
        case Key.LaunchApplication2:
          return AltaxoKeyboardKey.LaunchApplication2;
        case Key.Oem1:
          return AltaxoKeyboardKey.Oem1;
        case Key.OemPlus:
          return AltaxoKeyboardKey.OemPlus;
        case Key.OemComma:
          return AltaxoKeyboardKey.OemComma;
        case Key.OemMinus:
          return AltaxoKeyboardKey.OemMinus;
        case Key.OemPeriod:
          return AltaxoKeyboardKey.OemPeriod;
        case Key.OemQuestion:
          return AltaxoKeyboardKey.OemQuestion;
        case Key.Oem3:
          return AltaxoKeyboardKey.Oem3;
        case Key.AbntC1:
          return AltaxoKeyboardKey.AbntC1;
        case Key.AbntC2:
          return AltaxoKeyboardKey.AbntC2;
        case Key.OemOpenBrackets:
          return AltaxoKeyboardKey.OemOpenBrackets;
        case Key.Oem5:
          return AltaxoKeyboardKey.Oem5;
        case Key.Oem6:
          return AltaxoKeyboardKey.Oem6;
        case Key.OemQuotes:
          return AltaxoKeyboardKey.OemQuotes;
        case Key.Oem8:
          return AltaxoKeyboardKey.Oem8;
        case Key.OemBackslash:
          return AltaxoKeyboardKey.OemBackslash;
        case Key.ImeProcessed:
          return AltaxoKeyboardKey.ImeProcessed;
        case Key.System:
          return AltaxoKeyboardKey.System;
        case Key.OemAttn:
          return AltaxoKeyboardKey.OemAttn;
        case Key.OemFinish:
          return AltaxoKeyboardKey.OemFinish;
        case Key.OemCopy:
          return AltaxoKeyboardKey.OemCopy;
        case Key.DbeSbcsChar:
          return AltaxoKeyboardKey.DbeSbcsChar;
        case Key.OemEnlw:
          return AltaxoKeyboardKey.OemEnlw;
        case Key.OemBackTab:
          return AltaxoKeyboardKey.OemBackTab;
        case Key.DbeNoRoman:
          return AltaxoKeyboardKey.DbeNoRoman;
        case Key.DbeEnterWordRegisterMode:
          return AltaxoKeyboardKey.DbeEnterWordRegisterMode;
        case Key.DbeEnterImeConfigureMode:
          return AltaxoKeyboardKey.DbeEnterImeConfigureMode;
        case Key.EraseEof:
          return AltaxoKeyboardKey.EraseEof;
        case Key.Play:
          return AltaxoKeyboardKey.Play;
        case Key.DbeNoCodeInput:
          return AltaxoKeyboardKey.DbeNoCodeInput;
        case Key.NoName:
          return AltaxoKeyboardKey.NoName;
        case Key.Pa1:
          return AltaxoKeyboardKey.Pa1;
        case Key.OemClear:
          return AltaxoKeyboardKey.OemClear;
        case Key.DeadCharProcessed:
          return AltaxoKeyboardKey.DeadCharProcessed;
        default:
          throw new NotImplementedException("Unknown key");
      };
    }

    public static Altaxo.Gui.AltaxoKeyboardModifierKeys ToAltaxo(ModifierKeys mk)
    {
      var result = Altaxo.Gui.AltaxoKeyboardModifierKeys.None;

      if (mk.HasFlag(ModifierKeys.Alt))
        result |= AltaxoKeyboardModifierKeys.Alt;
      if (mk.HasFlag(ModifierKeys.Control))
        result |= AltaxoKeyboardModifierKeys.Control;
      if (mk.HasFlag(ModifierKeys.Shift))
        result |= AltaxoKeyboardModifierKeys.Shift;
      if (mk.HasFlag(ModifierKeys.Windows))
        result |= AltaxoKeyboardModifierKeys.Windows;

      return result;
    }

    #endregion Keyboard
  }
}
