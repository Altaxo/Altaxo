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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Altaxo.Drawing;
using Altaxo.Graph;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// ComboBox to present all registered textures as <see cref="ImageProxy"/> instances.
  /// </summary>
  public partial class TextureImageComboBox : ImageComboBox
  {
    private class CC : IValueConverter
    {
      private TextureImageComboBox _cb;

      public CC(TextureImageComboBox c)
      {
        _cb = c;
      }

      /// <summary>Converts an image proxy to the combobox item.</summary>
      /// <param name="value">The value produced by the binding source.</param>
      /// <param name="targetType">The type of the binding target property.</param>
      /// <param name="parameter">The converter parameter to use.</param>
      /// <param name="culture">The culture to use in the converter.</param>
      /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        if (value != null)
        {
          var val = (ImageProxy)value;
          if (!_cb._cachedItems.ContainsKey(val.ContentHash))
            _cb._cachedItems.Add(val.ContentHash, new ImageComboBoxItem(_cb, new KeyValuePair<string, ImageProxy>(val.Name, val)));
          return _cb._cachedItems[val.ContentHash];
        }
        else
        {
          return null;
        }
      }

      /// <summary>Converts the combobox item to the image proxy.</summary>
      /// <param name="value">The value that is produced by the binding target.</param>
      /// <param name="targetType">The type to convert to.</param>
      /// <param name="parameter">The converter parameter to use.</param>
      /// <param name="culture">The culture to use in the converter.</param>
      /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        if (null == value)
          return null;

        var it = (ImageComboBoxItem)value;
        var pair = (KeyValuePair<string, ImageProxy>)(it.Value);
        return pair.Value;
      }
    }

    /// <summary>Key is the content hash of the image proxy, value is the cached image.</summary>
    private static Dictionary<string, ImageSource> _cachedImages = new Dictionary<string, ImageSource>();

    /// <summary>Cached items. Key is the content hash of the image proxy, value is the combobox item.</summary>
    private Dictionary<string, ImageComboBoxItem> _cachedItems = new Dictionary<string, ImageComboBoxItem>();

    private ObservableCollection<ImageComboBoxItem> _textureItems = new ObservableCollection<ImageComboBoxItem>();
    private ObservableCollection<ImageComboBoxItem> _hatchItems = new ObservableCollection<ImageComboBoxItem>();
    private ObservableCollection<ImageComboBoxItem> _syntheticItems = new ObservableCollection<ImageComboBoxItem>();
    private ObservableCollection<ImageComboBoxItem> _currentItemsSource;

    public event DependencyPropertyChangedEventHandler TextureImageChanged;

    public event DependencyPropertyChangedEventHandler TextureImageTypeChanged;

    static TextureImageComboBox()
    {
    }

    public TextureImageComboBox()
    {
      InitializeComponent();

      InitializeItemLists();

      SetItemsSourceInDependenceOnTextureType();

      var _valueBinding = new Binding
      {
        Source = this,
        Path = new PropertyPath(_nameOfValueProp),
        Converter = new CC(this)
      };
      SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
    }

    private void InitializeItemLists()
    {
      ImageComboBoxItem it;

      // Texture items
      {
        foreach (KeyValuePair<string, ImageProxy> pair in TextureManager.BuiltinTextures)
        {
          _cachedItems.Add(pair.Value.ContentHash, it = new ImageComboBoxItem(this, pair));
          _textureItems.Add(it);
        }

        foreach (KeyValuePair<string, ImageProxy> pair in TextureManager.UserTextures)
        {
          _cachedItems.Add(pair.Value.ContentHash, it = new ImageComboBoxItem(this, pair));
          _textureItems.Add(it);
        }

        if (!_cachedItems.ContainsKey(BrushX.DefaultTextureBrush.ContentHash))
        {
          _cachedItems.Add(BrushX.DefaultTextureBrush.ContentHash, it = new ImageComboBoxItem(this, new KeyValuePair<string, ImageProxy>(BrushX.DefaultTextureBrush.Name, BrushX.DefaultTextureBrush)));
          _textureItems.Add(it);
        }
      }

      // Hatch items
      {
        var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Graph.Gdi.HatchBrushes.HatchBrushBase));
        foreach (var t in types)
        {
          Altaxo.Graph.Gdi.HatchBrushes.HatchBrushBase brush = null;
          try
          {
            brush = Activator.CreateInstance(t) as Altaxo.Graph.Gdi.HatchBrushes.HatchBrushBase;
          }
          catch (Exception) { }
          if (null != brush)
          {
            var pair = new KeyValuePair<string, ImageProxy>(t.Name, brush);
            if (!_cachedItems.TryGetValue(brush.ContentHash, out it))
              _cachedItems.Add(brush.ContentHash, it = new ImageComboBoxItem(this, pair));
            _hatchItems.Add(it);
          }
        }
        if (!_cachedItems.ContainsKey(BrushX.DefaultHatchBrush.ContentHash))
        {
          _cachedItems.Add(BrushX.DefaultHatchBrush.ContentHash, it = new ImageComboBoxItem(this, new KeyValuePair<string, ImageProxy>(BrushX.DefaultHatchBrush.Name, BrushX.DefaultHatchBrush)));
          _hatchItems.Add(it);
        }
      }

      // Synthetic items
      {
        var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Graph.Gdi.SyntheticBrushes.SyntheticBrushBase));
        foreach (var t in types)
        {
          Altaxo.Graph.Gdi.SyntheticBrushes.SyntheticBrushBase brush = null;
          try
          {
            brush = Activator.CreateInstance(t) as Altaxo.Graph.Gdi.SyntheticBrushes.SyntheticBrushBase;
          }
          catch (Exception) { }
          if (null != brush)
          {
            var pair = new KeyValuePair<string, ImageProxy>(t.Name, brush);
            if (!_cachedItems.TryGetValue(brush.ContentHash, out it))
              _cachedItems.Add(brush.ContentHash, it = new ImageComboBoxItem(this, pair));
            _syntheticItems.Add(it);
          }
        }
        if (!_cachedItems.ContainsKey(BrushX.DefaultSyntheticBrush.ContentHash))
        {
          _cachedItems.Add(BrushX.DefaultSyntheticBrush.ContentHash, it = new ImageComboBoxItem(this, new KeyValuePair<string, ImageProxy>(BrushX.DefaultSyntheticBrush.Name, BrushX.DefaultSyntheticBrush)));
          _syntheticItems.Add(it);
        }
      }
    }

    private void AddImage(ImageProxy img)
    {
      if (null == img)
        return;
      if (_cachedItems.ContainsKey(img.ContentHash))
        return;
      ImageComboBoxItem it;
      _cachedItems.Add(img.ContentHash, it = new ImageComboBoxItem(this, new KeyValuePair<string, ImageProxy>(img.Name, img)));
      _currentItemsSource.Add(it);
    }

    #region Dependency property

    private const string _nameOfValueProp = "TextureImage";

    public ImageProxy TextureImage
    {
      get { return (ImageProxy)GetValue(TextureImageProperty); }
      set { SetValue(TextureImageProperty, value); }
    }

    public static readonly DependencyProperty TextureImageProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(ImageProxy), typeof(TextureImageComboBox),
        new FrameworkPropertyMetadata(EhTextureImageChanged));

    private static void EhTextureImageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((TextureImageComboBox)obj).OnTextureImageChanged(obj, args);
    }

    #endregion Dependency property

    #region Dependency property TextureImageType

    public BrushType TextureImageType
    {
      get { return (BrushType)GetValue(TextureImageTypeProperty); }
      set { SetValue(TextureImageTypeProperty, value); }
    }

    public static readonly DependencyProperty TextureImageTypeProperty =
        DependencyProperty.Register("TextureImageType", typeof(BrushType), typeof(TextureImageComboBox),
        new FrameworkPropertyMetadata(BrushType.HatchBrush, EhTextureImageTypeChanged));

    private static void EhTextureImageTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((TextureImageComboBox)obj).OnTextureImageTypeChanged(obj, args);
    }

    #endregion Dependency property TextureImageType

    protected virtual void OnTextureImageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (null != TextureImageChanged)
        TextureImageChanged(this, args);
    }

    protected virtual void OnTextureImageTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      SetItemsSourceInDependenceOnTextureType();

      if (null != TextureImageTypeChanged)
        TextureImageTypeChanged(this, args);
    }

    private void SetItemsSourceInDependenceOnTextureType()
    {
      SelectedIndex = -1;
      switch (TextureImageType)
      {
        case BrushType.TextureBrush:
          _currentItemsSource = _textureItems;
          break;

        case BrushType.HatchBrush:
          _currentItemsSource = _hatchItems;
          break;

        case BrushType.SyntheticTextureBrush:
          _currentItemsSource = _syntheticItems;
          break;

        default:
          _currentItemsSource = new ObservableCollection<ImageComboBoxItem>();
          break;
      }

      ItemsSource = _currentItemsSource;

      _menuLoadTextureFromFile.IsEnabled = TextureImageType == BrushType.TextureBrush;
    }

    private void EhLoadFromFile(object sender, EventArgs e)
    {
      var options = new OpenFileOptions();
      options.AddFilter("*.*", "All files (*.*");
      options.FilterIndex = 0;
      if (Current.Gui.ShowOpenFileDialog(options))
      {
        var img = ImageProxy.FromFile(options.FileName);
        if (img.IsValid)
        {
          TextureManager.UserTextures.Add(img);
          AddImage(img);
          TextureImage = img;
        }
      }
    }

    public override string GetItemText(object item)
    {
      var val = (KeyValuePair<string, ImageProxy>)item;
      return val.Key;
    }

    public override ImageSource GetItemImage(object item)
    {
      var val = (KeyValuePair<string, ImageProxy>)item;
      if (!_cachedImages.TryGetValue(val.Key, out var result))
        _cachedImages.Add(val.Key, result = GetImage(val.Value));
      return result;
    }

    public static ImageSource GetImage(ImageProxy val)
    {
      return GuiHelper.ToWpf(val);
    }
  }
}
