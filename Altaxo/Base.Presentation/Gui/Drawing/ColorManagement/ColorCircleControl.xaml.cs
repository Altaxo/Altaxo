#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// Interaction logic for ColorMixingControl.xaml
  /// </summary>
  public partial class ColorCircleControl : UserControl, IColorCircleView
  {
    public event Action ColorModelSelectionChanged;

    public event Action TextOnlyColorModelSelectionChanged;

    public event Action<AxoColor> CurrentColorChanged;

    public event Action ColorCircleModelChanged;

    public event Action ColorVariationModelChanged;

    /// <summary>
    /// Occurs when at least one of the hue values of the color circle has changed. Argument is the list of hue values of the circle, with the first item
    /// always representing the main hue value.
    /// </summary>
    public event Action<IReadOnlyList<double>> HueValuesChanged;

    public event Action<int> NumberOfColorShadesChanged;

    private DecimalUpDown[] _guiComponents = new DecimalUpDown[4];
    private TextBox[] _guiAltComponents = new TextBox[4];
    private Label[] _guiLabelForComponents = new Label[4];
    private Label[] _guiLabelForAltComponents = new Label[4];

    private StackPanel[] _guiShadesStackPanels;

    private IColorModel _colorModel;
    private ITextOnlyColorModel _altColorModel;
    private AxoColor _currentColor;

    public AxoColor CurrentColor { get { return _currentColor; } }

    public ColorCircleControl()
    {
      InitializeComponent();

      _guiLabelForComponents[0] = _guiLabelForComponent0;
      _guiLabelForComponents[1] = _guiLabelForComponent1;
      _guiLabelForComponents[2] = _guiLabelForComponent2;
      _guiLabelForComponents[3] = _guiLabelForComponent3;

      _guiComponents[0] = _guiComponent0;
      _guiComponents[1] = _guiComponent1;
      _guiComponents[2] = _guiComponent2;
      _guiComponents[3] = _guiComponent3;

      _guiLabelForAltComponents[0] = _guiLabelForAltComponent0;
      _guiLabelForAltComponents[1] = _guiLabelForAltComponent1;
      _guiLabelForAltComponents[2] = _guiLabelForAltComponent2;
      _guiLabelForAltComponents[3] = _guiLabelForAltComponent3;

      _guiAltComponents[0] = _guiAltComponent0;
      _guiAltComponents[1] = _guiAltComponent1;
      _guiAltComponents[2] = _guiAltComponent2;
      _guiAltComponents[3] = _guiAltComponent3;

      _guiShadesStackPanels = new StackPanel[5];
      _guiShadesStackPanels[0] = _guiColorShades0;
      _guiShadesStackPanels[1] = _guiColorShades1;
      _guiShadesStackPanels[2] = _guiColorShades2;
      _guiShadesStackPanels[3] = _guiColorShades3;
      _guiShadesStackPanels[4] = _guiColorShades4;

      _colorModel = new ColorModelRGB();
      _altColorModel = new TextOnlyColorModelRGB();

      _guiColorCircleSurface.HueValuesChanged += EhColorCircleSurface_HueValuesChanged;
    }

    private void EhColorCircleSurface_HueValuesChanged(IReadOnlyList<double> hueValues)
    {
      HueValuesChanged?.Invoke(hueValues);
    }

    private void EhColorModelSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiColorModel);
      ColorModelSelectionChanged?.Invoke();
    }

    private void EhTextOnlyColorModelSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAltComponentsType);
      TextOnlyColorModelSelectionChanged?.Invoke();
    }

    private void EhColorCircleModelSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiColorCircleModel);
      ColorCircleModelChanged?.Invoke();
    }

    private void EhColorVariationSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiColorVariation);
      ColorVariationModelChanged?.Invoke();
    }

    public void AmendAlphaComponent(ref AxoColor color)
    {
      if (_colorModel.IsUsingByteComponents)
        color.A = (byte)_guiAlphaValue.Value;
      else
        color.ScA = (float)_guiAlphaValue.Value;
    }

    private void EhAlphaValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      if (!IsLoaded)
        return;

      var newColor = _currentColor;
      AmendAlphaComponent(ref newColor);

      ChangeCurrentColor(newColor, false, false, false, false);
    }

    private void EhComponentChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      if (!IsLoaded)
        return;

      var newColor = _colorModel.GetColorFromComponents(_guiComponents.Select(c => (double)c.Value).ToArray());

      AmendAlphaComponent(ref newColor);

      ChangeCurrentColor(newColor, true, true, false, true);
    }

    private void EhNumberOfShadesChangedByUpDown(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
      if (IsLoaded)
      {
        int n = e.NewValue;
        _guiNumberOfShadesSlider.Value = n;
        NumberOfColorShadesChanged?.Invoke(n);
      }
    }

    private void EhNumberOfShadesChangedBySlider(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (IsLoaded)
      {
        int n = (int)Math.Round(e.NewValue);
        _guiNumberOfShadesUpDown.Value = n;
      }
    }

    public void ChangeCurrentColor(AxoColor newColor, bool updateCircle, bool updateVariations, bool updateComponents, bool updateAltComponents)
    {
      _currentColor = newColor;

      if (updateCircle)
      {
        _guiColorCircleSurface.SetHueBaseValue(_currentColor.GetHue(), false);
      }

      if (updateComponents)
      {
        // now calculate components
        var components = _colorModel.GetComponentsForColor(_currentColor);

        UpdateComponentValues(() =>
        {
          for (int i = 0; i < components.Length; ++i)
            _guiComponents[i].Value = (decimal)components[i];

          if (_colorModel.IsUsingByteComponents)
            _guiAlphaValue.Value = _currentColor.A;
          else
            _guiAlphaValue.Value = (decimal)_currentColor.ScA;
        }
        );
      }

      if (updateAltComponents)
      {
        // update alternative components
        var altComponents = _altColorModel.GetComponentsForColor(_currentColor, Altaxo.Settings.GuiCulture.Instance);
        for (int i = 0; i < altComponents.Length; ++i)
          _guiAltComponents[i].Text = altComponents[i];
      }

      CurrentColorChanged?.Invoke(_currentColor);
    }

    private void UpdateComponentValues(Action updateAction)
    {
      _guiComponent0.ValueChanged -= EhComponentChanged;
      _guiComponent1.ValueChanged -= EhComponentChanged;
      _guiComponent2.ValueChanged -= EhComponentChanged;
      _guiComponent3.ValueChanged -= EhComponentChanged;
      _guiAlphaValue.ValueChanged -= EhAlphaValueChanged;

      updateAction();

      _guiComponent0.ValueChanged += EhComponentChanged;
      _guiComponent1.ValueChanged += EhComponentChanged;
      _guiComponent2.ValueChanged += EhComponentChanged;
      _guiComponent3.ValueChanged += EhComponentChanged;
      _guiAlphaValue.ValueChanged += EhAlphaValueChanged;
    }

    public void InitializeAvailableColorModels(SelectableListNodeList listOfColorModels)
    {
      GuiHelper.Initialize(_guiColorModel, listOfColorModels);
    }

    public void InitializeAvailableTextOnlyColorModels(SelectableListNodeList listOfTextOnlyColorModels)
    {
      GuiHelper.Initialize(_guiAltComponentsType, listOfTextOnlyColorModels);
    }

    public void InitializeColorModel(IColorModel colorModel, bool silentSet)
    {
      _colorModel = colorModel;

      if (!silentSet)
      {
        _guiLabelComponentsType.Content = Current.Gui.GetUserFriendlyClassName(colorModel.GetType());

        // now update components
        var components = _colorModel.GetComponentsForColor(_currentColor);
        UpdateComponentValues(() =>
        {
          for (int i = 0; i < components.Length; ++i)
          {
            if (_colorModel.IsUsingByteComponents)
            {
              _guiComponents[i].Maximum = 255;
              _guiComponents[i].Change = 1;
              _guiComponents[i].DecimalPlaces = 0;
            }
            else
            {
              _guiComponents[i].Maximum = 1;
              _guiComponents[i].Change = 0.01M;
              _guiComponents[i].DecimalPlaces = 3;
            }
            _guiComponents[i].Value = (decimal)components[i];
          }

          if (_colorModel.IsUsingByteComponents)
          {
            _guiAlphaValue.Maximum = 255;
            _guiAlphaValue.Change = 1;
            _guiAlphaValue.DecimalPlaces = 0;
            _guiAlphaValue.Value = _currentColor.A;
          }
          else
          {
            _guiAlphaValue.Maximum = 1;
            _guiAlphaValue.Change = 0.01M;
            _guiAlphaValue.DecimalPlaces = 3;
            _guiAlphaValue.Value = (decimal)_currentColor.ScA;
          }
        }
        );

        // update labels
        var labels = _colorModel.GetNamesOfComponents();
        for (int i = 0; i < labels.Length; ++i)
        {
          _guiLabelForComponents[i].Content = labels[i];
        }

        // update visibility
        for (int i = 0; i < 4; ++i)
        {
          _guiLabelForComponents[i].Visibility = _guiComponents[i].Visibility = i < labels.Length ? Visibility.Visible : Visibility.Hidden;
        }

        ChangeCurrentColor(_currentColor, false, false, true, true);
      }
    }

    public void InitializeTextOnlyColorModel(ITextOnlyColorModel colorModel, bool silentSet)
    {
      _altColorModel = colorModel;

      if (!silentSet)
      {
        // now update components
        var components = _altColorModel.GetComponentsForColor(_currentColor, Altaxo.Settings.GuiCulture.Instance);

        for (int i = 0; i < components.Length; ++i)
          _guiAltComponents[i].Text = components[i];

        // update labels
        var labels = _altColorModel.GetNamesOfComponents();
        for (int i = 0; i < labels.Length; ++i)
        {
          _guiLabelForAltComponents[i].Content = labels[i];
        }

        // update visibility
        for (int i = 0; i < 4; ++i)
        {
          _guiLabelForAltComponents[i].Visibility = _guiAltComponents[i].Visibility = i < components.Length ? Visibility.Visible : Visibility.Hidden;
        }
      }
    }

    public void InitializeAvailableColorCircleModels(SelectableListNodeList availableColorCircleModels)
    {
      GuiHelper.Initialize(_guiColorCircleModel, availableColorCircleModels);
    }

    public IReadOnlyList<double> InitializeCurrentColorCircleModel(IColorCircleModel currentColorCircleModel, bool silentSet)
    {
      _guiColorCircleSurface.ColorCircleModel = currentColorCircleModel;

      if (!silentSet)
      {
      }

      return _guiColorCircleSurface.HueValues;
    }

    public void InitializeAvailableColorVariationModels(SelectableListNodeList availableColorVariationModels)
    {
      GuiHelper.Initialize(_guiColorVariation, availableColorVariationModels);
    }

    public void InitializeCurrentColorVariationModel(IColorVariationModel currentColorVariationModel, bool silentSet)
    {
    }

    public void InitializeCurrentColor(AxoColor color)
    {
      _currentColor = color;
      UpdateAllAccordingToCurrentModelAndCurrentColor();
    }

    private void UpdateAllAccordingToCurrentModelAndCurrentColor()
    {
      // update color circle
      _guiColorCircleSurface.SetHueBaseValue(_currentColor.GetHue(), true);

      // now update components
      var components = _colorModel.GetComponentsForColor(_currentColor);
      UpdateComponentValues(() =>
      {
        for (int i = 0; i < components.Length; ++i)
        {
          if (_colorModel.IsUsingByteComponents)
          {
            _guiComponents[i].Maximum = 255;
            _guiComponents[i].Change = 1;
          }
          else
          {
            _guiComponents[i].Maximum = 1;
            _guiComponents[i].Change = 0.001M;
          }
          _guiComponents[i].Value = (decimal)components[i];
        }

        if (_colorModel.IsUsingByteComponents)
        {
          _guiAlphaValue.Maximum = 255;
          _guiAlphaValue.Change = 1;
          _guiAlphaValue.Value = _currentColor.A;
        }
        else
        {
          _guiAlphaValue.Maximum = 1;
          _guiAlphaValue.Change = 0.001M;
          _guiAlphaValue.Value = (decimal)_currentColor.ScA;
        }
      }
      );
    }

    public void InitializeNumberOfColorShades(int numberOfColorShades)
    {
      _guiNumberOfShadesSlider.Value = numberOfColorShades;
      _guiNumberOfShadesUpDown.Value = numberOfColorShades;
    }

    public void InitializeColorShades(AxoColor[][] colorShades)
    {
      const int maxJ = 20;
      int i, j;

      for (i = 0; i < colorShades.Length; ++i)
      {
        var sp = _guiShadesStackPanels[i];
        sp.Visibility = Visibility.Visible;
        int numPresentChilds = sp.Children.Count;

        for (j = 0; j < colorShades[i].Length; ++j)
        {
          if (j >= numPresentChilds)
          {
            var newRect = new Rectangle() { Width = 16, Height = 16, Stroke = Brushes.Black, StrokeThickness = 0.5 };
            newRect.MouseEnter += EhColorShade_MouseEnter;
            newRect.MouseLeave += EhColorShade_MouseLeave;
            newRect.MouseDown += EhColorShadeRectangle_MouseDown;
            sp.Children.Add(newRect);
          }

          var rect = (Rectangle)sp.Children[j];
          rect.Visibility = Visibility.Visible;
          rect.Tag = colorShades[i][j];
          rect.Fill = new SolidColorBrush(GuiHelper.ToWpf(colorShades[i][j]));
        }

        for (; j < maxJ && j < numPresentChilds; ++j)
          sp.Children[j].Visibility = Visibility.Hidden;
      }

      for (; i < _guiShadesStackPanels.Length; ++i)
      {
        _guiShadesStackPanels[i].Visibility = Visibility.Hidden;
      }
    }

    private void EhColorShade_MouseEnter(object sender, MouseEventArgs e)
    {
      var rect = (Rectangle)sender;
      var color = (AxoColor)rect.Tag;

      var newTag = new Tuple<AxoColor, AxoColor>(color, _currentColor);
      rect.Tag = newTag;
      rect.Stroke = Brushes.Orange;

      ChangeCurrentColor(color, false, false, true, true);
    }

    private void EhColorShade_MouseLeave(object sender, MouseEventArgs e)
    {
      var rect = (Rectangle)sender;
      var colors = (Tuple<AxoColor, AxoColor>)rect.Tag;

      var myColor = colors.Item1;
      var savedCurrentColor = colors.Item2;

      rect.Tag = myColor;
      rect.Stroke = Brushes.Black;

      ChangeCurrentColor(savedCurrentColor, false, false, true, true);
    }

    private void EhColorShadeRectangle_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var rect = (Rectangle)sender;
      AxoColor color;

      if (rect.Tag is Tuple<AxoColor, AxoColor> t)
      {
        color = t.Item1;
        rect.Tag = new Tuple<AxoColor, AxoColor>(color, color); // make the color permanent
      }
      else
      {
        throw new InvalidProgramException();
      }

      color.A = (byte)_guiAlphaValue.Value;

      ChangeCurrentColor(color, false, false, true, true);
    }
  }
}
