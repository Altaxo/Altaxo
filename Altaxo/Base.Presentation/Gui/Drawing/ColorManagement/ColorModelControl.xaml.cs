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

#nullable disable warnings
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

namespace Altaxo.Gui.Drawing.ColorManagement
{
  using Altaxo.Collections;
  using Altaxo.Drawing;
  using Altaxo.Geometry;
  using Altaxo.Gui.Common;

  /// <summary>
  /// Interaction logic for ColorModelControl.xaml
  /// </summary>
  public partial class ColorModelControl : UserControl, IColorModelView
  {
    private DecimalUpDown[] _guiComponents = new DecimalUpDown[4];
    private TextBox[] _guiAltComponents = new TextBox[4];
    private Label[] _guiLabelForComponents = new Label[4];
    private Label[] _guiLabelForAltComponents = new Label[4];

    private IColorModel _colorModel;
    private ITextOnlyColorModel _altColorModel;
    private AxoColor _currentColor;

    public AxoColor CurrentColor { get { return _currentColor; } }

    public event Action? ColorModelSelectionChanged;

    public event Action? TextOnlyColorModelSelectionChanged;

    public event Action<AxoColor>? CurrentColorChanged;

    public bool _isLoaded;

    public ColorModelControl()
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

      _colorModel = new ColorModelRGB();
      _altColorModel = new TextOnlyColorModelRGB();

      Loaded += EhLoaded;
    }

    private void EhLoaded(object sender, RoutedEventArgs e)
    {
      _isLoaded = true;
    }

    public void ChangeCurrentColor(AxoColor newColor, bool update1D, bool update2D, bool updateComponents, bool updateAltComponents)
    {
      _currentColor = newColor;

      if (update1D || update2D)
      {
        var (pos1D, pos2D) = _colorModel.GetRelativePositionsFor1Dand2DColorSurfaceFromColor(_currentColor);

        _gui1DColorControl.SelectionRectangleRelativePositionChanged -= Eh1DColorControl_ValueChanged;
        _gui2DColorControl.SelectionRectangleRelativePositionChanged -= Eh2DColorControl_ValueChanged;

        _gui1DColorControl.SelectionRectangleRelativePosition = pos1D;
        _gui2DColorControl.SelectionRectangleRelativePosition = pos2D;

        _gui1DColorControl.SelectionRectangleRelativePositionChanged += Eh1DColorControl_ValueChanged;
        _gui2DColorControl.SelectionRectangleRelativePositionChanged += Eh2DColorControl_ValueChanged;
      }

      if (updateComponents)
      {
        // now calculate components
        var components = _colorModel.GetComponentsForColor(_currentColor);

        UpdateComponentValues(() =>
        {
          for (int i = 0; i < components.Length; ++i)
            _guiComponents[i].Value = (decimal)components[i];
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

    public void AmendAlphaComponent(ref AxoColor color)
    {
      if (_colorModel.IsUsingByteComponents)
        color.A = (byte)_guiAlphaValue.Value;
      else
        color.ScA = (float)_guiAlphaValue.Value;
    }

    /// <summary>
    /// Occurs if a color component was changed by the user.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The instance containing the event data.</param>
    private void EhComponentChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      if (!_isLoaded)
        return;

      var newColor = _colorModel.GetColorFromComponents(_guiComponents.Select(c => (double)c.Value).ToArray());

      AmendAlphaComponent(ref newColor);

      ChangeCurrentColor(newColor, true, true, false, true);
    }

    private void EhAlphaValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      if (!_isLoaded)
        return;

      var newColor = _currentColor;

      AmendAlphaComponent(ref newColor);

      ChangeCurrentColor(newColor, false, false, false, false);
    }

    private void Eh1DColorControl_ValueChanged(double relValue)
    {
      var baseColor = _colorModel.GetColorFor1DColorSurfaceFromRelativePosition(relValue);
      var imgSource = ColorBitmapCreator.GetBitmap(relPos => _colorModel.GetColorFor2DColorSurfaceFromRelativePosition(relPos, baseColor));
      _gui2DColorControl.Set2DColorImage(imgSource);

      var newColor = _colorModel.GetColorFor2DColorSurfaceFromRelativePosition(_gui2DColorControl.SelectionRectangleRelativePosition, baseColor);

      AmendAlphaComponent(ref newColor);

      ChangeCurrentColor(newColor, false, false, true, true);
    }

    private void Eh2DColorControl_ValueChanged(PointD2D relPos)
    {
      double pos1D = _gui1DColorControl.SelectionRectangleRelativePosition;
      var baseColor = _colorModel.GetColorFor1DColorSurfaceFromRelativePosition(pos1D);
      var newColor = _colorModel.GetColorFor2DColorSurfaceFromRelativePosition(relPos, baseColor);

      AmendAlphaComponent(ref newColor);

      ChangeCurrentColor(newColor, false, false, true, true);
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

    private void UpdateColorSurfacePositions(double position1D, PointD2D position2D)
    {
      _gui1DColorControl.SelectionRectangleRelativePositionChanged -= Eh1DColorControl_ValueChanged;
      _gui2DColorControl.SelectionRectangleRelativePositionChanged -= Eh2DColorControl_ValueChanged;

      _gui2DColorControl.SelectionRectangleRelativePosition = position2D;
      _gui1DColorControl.SelectionRectangleRelativePosition = position1D;

      _gui1DColorControl.SelectionRectangleRelativePositionChanged += Eh1DColorControl_ValueChanged;
      _gui2DColorControl.SelectionRectangleRelativePositionChanged += Eh2DColorControl_ValueChanged;
    }

    private void UpdateAllAccordingToCurrentModelAndCurrentColor()
    {
      var (pos1D, pos2D) = _colorModel.GetRelativePositionsFor1Dand2DColorSurfaceFromColor(_currentColor);

      // update color surfaces
      UpdateColorSurfacePositions(pos1D, pos2D);
      var baseColor = _colorModel.GetColorFor1DColorSurfaceFromRelativePosition(pos1D);
      var imgSource1D = ColorBitmapCreator.GetBitmap(p => _colorModel.GetColorFor1DColorSurfaceFromRelativePosition(p.Y));
      _gui1DColorControl.Set1DColorImage(imgSource1D);
      var imgSource2D = ColorBitmapCreator.GetBitmap(relPos => _colorModel.GetColorFor2DColorSurfaceFromRelativePosition(relPos, baseColor));
      _gui2DColorControl.Set2DColorImage(imgSource2D);

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
      _guiLabelComponentsType.Content = Current.Gui.GetUserFriendlyClassName(_colorModel.GetType());
      var labels = _colorModel.GetNamesOfComponents();
      for (int i = 0; i < labels.Length; ++i)
      {
        _guiLabelForComponents[i].Content = labels[i];
      }

      // update visibility
      for (int i = 0; i < 4; ++i)
      {
        _guiLabelForComponents[i].Visibility = _guiComponents[i].Visibility = i < components.Length ? Visibility.Visible : Visibility.Hidden;
      }

      // update alternative components
      var altComponents = _altColorModel.GetComponentsForColor(_currentColor, Altaxo.Settings.GuiCulture.Instance);
      for (int i = 0; i < altComponents.Length; ++i)
        _guiAltComponents[i].Text = altComponents[i];
    }

    public void InitializeAvailableColorModels(SelectableListNodeList listOfColorModels)
    {
      GuiHelper.Initialize(_guiColorModel, listOfColorModels);
    }

    public void InitializeColorModel(IColorModel colorModel, bool silentSet)
    {
      _colorModel = colorModel;

      if (!silentSet)
        UpdateAllAccordingToCurrentModelAndCurrentColor();
    }

    public void InitializeAvailableTextOnlyColorModels(SelectableListNodeList listOfTextOnlyColorModels)
    {
      GuiHelper.Initialize(_guiAltComponentsType, listOfTextOnlyColorModels);
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

    public void InitializeCurrentColor(AxoColor color)
    {
      _currentColor = color;
      UpdateAllAccordingToCurrentModelAndCurrentColor();
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
  }
}
