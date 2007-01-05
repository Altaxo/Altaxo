#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Altaxo.Data;

using Altaxo.Calc.Regression.Nonlinear;


namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Summary description for FitElementControl.
  /// </summary>
  [UserControlForController(typeof(IFitElementViewEventSink))]
  public class FitElementControl : System.Windows.Forms.UserControl, IFitElementView
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public FitElementControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();


    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
    }
    #endregion

    #region IFitElementView
    IFitElementViewEventSink _controller;
    public IFitElementViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }
    #endregion


    FitElement _fitElement;
    int _numberOfX;
    int _numberOfY;
    int _numberOfParameter;
    int _totalSlots;
    int _slotHeight;
    Pen _pen;


    // Variables for paint and click
    /// <summary>Upper left vertex of the fit function box.</summary>
    Point _fitBoxLocation;
    /// <summary>Size of the fit function box.</summary>
    Size _fitBoxSize;

    /// <summary>Width of the IVarianceScaling box.</summary>
    int _errorFunctionWidth;
    /// <summary>X position of the IVarianceScaling box.</summary>
    int _errorFunctionX;

    /// <summary>X coordinate of the left edge of the external parameters boxes.</summary>
    int _externalParametersX;
    /// <summary>With of the external parameters boxes.</summary>
    int _externalParametersWidth;

    int _VariablesX = 0;
    int _DependentVariablesWidth;
    int _DependentVariablesY;
    int _IndependentVariablesWidth;
    bool _fitFunctionSelected;

    public int SlotHeight { get { return _slotHeight; } }


    public bool FitFunctionSelected 
    {
      set
      {
        _fitFunctionSelected = value;
        this.Invalidate();
      }
    }

    public void Initialize(FitElement fitElement)
    {
      _fitElement = fitElement;

      _numberOfX = _fitElement.NumberOfIndependentVariables;
      _numberOfY = _fitElement.NumberOfDependentVariables;
      _numberOfParameter = _fitElement.NumberOfParameters;

      _totalSlots = Math.Max(_numberOfParameter, _numberOfX + _numberOfY + 1);
      _slotHeight = System.Windows.Forms.SystemInformation.MenuButtonSize.Height;
      _pen = System.Drawing.Pens.Blue;

      this.ClientSize = new Size(this.ClientSize.Width, _totalSlots * _slotHeight);
    }

    void IFitElementView.Refresh()
    {
      this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      StringFormat leftJustified = new StringFormat(StringFormat.GenericTypographic);
      //      stringFormat.Alignment = StringAlignment.Center;
      leftJustified.LineAlignment = StringAlignment.Center;

      StringFormat rightJustified = new StringFormat(StringFormat.GenericTypographic);
      rightJustified.Alignment = StringAlignment.Far;
      rightJustified.LineAlignment = StringAlignment.Center;

      StringFormat bothCentered = new StringFormat(StringFormat.GenericTypographic);
      bothCentered.Alignment = StringAlignment.Center;
      bothCentered.LineAlignment = StringAlignment.Center;

      _fitBoxLocation = new Point(this.ClientSize.Width / 3, 0);
      _fitBoxSize = new Size(this.ClientSize.Width / 3, _totalSlots * _slotHeight);


      e.Graphics.DrawRectangle(_pen, _fitBoxLocation.X, _fitBoxLocation.Y, _fitBoxSize.Width, _fitBoxSize.Height - 1);


      int currentY = 0;

      // Draw the independent variables

      for (int i = 0; i < _numberOfX; i++)
      {
        Point triangleStart = new Point(_fitBoxLocation.X, currentY + (1 * _slotHeight) / 4);
        Point triangleMidst = new Point(_fitBoxLocation.X + _slotHeight / 2, currentY + _slotHeight / 2);
        Point triangleEnd = new Point(_fitBoxLocation.X, currentY + (3 * _slotHeight) / 4);

        e.Graphics.DrawLine(_pen, triangleStart, triangleMidst);
        e.Graphics.DrawLine(_pen, triangleMidst, triangleEnd);
        if (_fitElement.FitFunction != null)
        {
          e.Graphics.DrawString(
            _fitElement.FitFunction.IndependentVariableName(i),
            System.Windows.Forms.SystemInformation.MenuFont,
            System.Drawing.Brushes.Black,
            new Rectangle(_fitBoxLocation.X + _slotHeight / 2 + 2, currentY, _fitBoxSize.Width - 1 - _slotHeight / 2, _slotHeight),
            leftJustified
            );
        }
        currentY += _slotHeight;
      }



      // now draw the dependent variables
      _DependentVariablesY = (_totalSlots - _numberOfY) * _slotHeight;
      currentY = _DependentVariablesY;

      _errorFunctionWidth = (6 * _slotHeight) / 4;
      _errorFunctionX = _fitBoxLocation.X - _errorFunctionWidth / 2;

      for (int i = 0; i < _numberOfY; i++)
      {
        Rectangle errorFuncRect = new Rectangle(
          _errorFunctionX,
          currentY,
          _errorFunctionWidth,
          _slotHeight - 1);


        e.Graphics.FillRectangle(Brushes.WhiteSmoke, errorFuncRect);
        e.Graphics.DrawRectangle(_pen, errorFuncRect);
        if (_fitElement.ErrorEvaluation(i) != null)
        {
          e.Graphics.DrawString(_fitElement.ErrorEvaluation(i).ShortName,
            System.Windows.Forms.SystemInformation.MenuFont,
            System.Drawing.Brushes.Black,
            errorFuncRect,
            bothCentered);
        }



        if (_fitElement.FitFunction != null)
          e.Graphics.DrawString(
            _fitElement.FitFunction.DependentVariableName(i),
            System.Windows.Forms.SystemInformation.MenuFont,
            System.Drawing.Brushes.Black,
            new Rectangle(_errorFunctionX + _errorFunctionWidth + (1 * _slotHeight) / 4, currentY, _fitBoxSize.Width - 1 - _slotHeight / 2, _slotHeight),
            leftJustified
            );

        currentY += _slotHeight;

      }


      // now draw the internal parameters
      currentY = 0;

      for (int i = 0; i < _numberOfParameter; i++)
      {

        e.Graphics.DrawEllipse(_pen, _fitBoxLocation.X + _fitBoxSize.Width - _slotHeight / 4, currentY + _slotHeight / 4, _slotHeight / 2, _slotHeight / 2);
        if (_fitElement.FitFunction != null)
          e.Graphics.DrawString(
            _fitElement.FitFunction.ParameterName(i),
            System.Windows.Forms.SystemInformation.MenuFont,
            System.Drawing.Brushes.Black,
            new Rectangle(_fitBoxLocation.X, currentY, _fitBoxSize.Width - 1 - _slotHeight / 4, _slotHeight),
            rightJustified
            );

        currentY += _slotHeight;

      }


      // now draw the external parameters
      currentY = 0;

      _externalParametersX = _fitBoxLocation.X + _fitBoxSize.Width + _slotHeight / 2;
      _externalParametersWidth = this.ClientSize.Width - _externalParametersX;
      for (int i = 0; i < _numberOfParameter; i++)
      {
        Rectangle rect = new Rectangle(_externalParametersX, currentY, _externalParametersWidth, _slotHeight);

        System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics,
          rect.X - 2, rect.Y, rect.Width + 2, rect.Height,
          System.Windows.Forms.Border3DStyle.Etched,
          System.Windows.Forms.Border3DSide.All);

        e.Graphics.DrawString(
          _fitElement.ParameterName(i),
          System.Windows.Forms.SystemInformation.MenuFont,
          System.Drawing.Brushes.Black,
          new Rectangle(_externalParametersX, currentY, _externalParametersWidth, _slotHeight),
          leftJustified
          );

        currentY += _slotHeight;

      }

      // Draw the names of the independent columns
      currentY = 0;
      _VariablesX = 0;
      _IndependentVariablesWidth = _fitBoxLocation.X - _slotHeight / 4;
      for (int i = 0; i < _numberOfX; i++)
      {
        Rectangle rect = new Rectangle(0, currentY, _IndependentVariablesWidth, _slotHeight);
        System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics,
          rect.X, rect.Y, rect.Width + 2, rect.Height,
          System.Windows.Forms.Border3DStyle.Etched,
          System.Windows.Forms.Border3DSide.All);

        INumericColumn col = _fitElement.IndependentVariables(i);
        if (col != null)
        {

          string name = col.FullName;

          e.Graphics.DrawString(
            name,
            System.Windows.Forms.SystemInformation.MenuFont,
            System.Drawing.Brushes.Black,
            rect,
            rightJustified
            );
        }
        currentY += _slotHeight;
      }

      {
        // draw the plot range
        Calc.PositiveIntegerRange plotRange = _fitElement.GetRowRange();
        string rangestring = "Range: " + plotRange.First.ToString() + " to " + (plotRange.IsInfinite ? "infinity" : plotRange.Last.ToString());
        Rectangle rect = new Rectangle(0, currentY, _IndependentVariablesWidth, _slotHeight);
        System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics,
          rect.X, rect.Y, rect.Width + 2, rect.Height,
          System.Windows.Forms.Border3DStyle.Etched,
          System.Windows.Forms.Border3DSide.All);
        e.Graphics.DrawString(
             rangestring,
             System.Windows.Forms.SystemInformation.MenuFont,
             System.Drawing.Brushes.Black,
             rect,
             rightJustified
             );
      }
      // Draw the names of the dependent columns
      currentY = _DependentVariablesY;
      _DependentVariablesWidth = _errorFunctionX - (2 * _slotHeight) / 8;
      for (int i = 0; i < _numberOfY; i++)
      {
        Rectangle rect = new Rectangle(0, currentY, _DependentVariablesWidth, _slotHeight);

        System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics,
          rect.X, rect.Y, rect.Width + 2, rect.Height,
          System.Windows.Forms.Border3DStyle.Etched,
          System.Windows.Forms.Border3DSide.All);

        INumericColumn col = _fitElement.DependentVariables(i);
        if (col != null)
        {
          string name = col.FullName;

          e.Graphics.DrawString(
            name,
            System.Windows.Forms.SystemInformation.MenuFont,
            System.Drawing.Brushes.Black,
            rect,
            rightJustified
            );
        }
        currentY += _slotHeight;
      }

      string fitFuncName = null;
      if (_fitElement.FitFunction == null)
        fitFuncName = "?";
      else if (_fitElement.FitFunction is Altaxo.Scripting.IFitFunctionScriptText)
        fitFuncName = (_fitElement.FitFunction as Altaxo.Scripting.IFitFunctionScriptText).FitFunctionName;
      else
        fitFuncName = _fitElement.FitFunction.ToString();

      Rectangle fitFuncBox = new Rectangle(
        _fitBoxLocation.X + _fitBoxSize.Width / 4,
        _fitBoxLocation.Y + _fitBoxSize.Height / 4,
        _fitBoxSize.Width / 2,
        _fitBoxSize.Height / 2);

      e.Graphics.DrawString(fitFuncName,
        System.Windows.Forms.SystemInformation.MenuFont,
        System.Drawing.Brushes.Black,
        fitFuncBox
        );

      if (this._fitFunctionSelected)
        e.Graphics.DrawRectangle(Pens.Red, fitFuncBox);




      base.OnPaint(e);
    }

    Point _lastMouseDown;
    MouseButtons _lastMouseButton;
    protected override void OnMouseDown(MouseEventArgs e)
    {
      _lastMouseDown = new Point(e.X,e.Y);
      _lastMouseButton = e.Button;
      
      base.OnMouseDown (e);
    }

    protected bool IsClickOnExternalParameter(Point point, ref int idx)
    {

      if(point.X>this._externalParametersX  &&
        point.X<(this._externalParametersX+this._externalParametersWidth) &&
        point.Y>=this._fitBoxLocation.Y &&
        point.Y<(_fitElement.NumberOfParameters*_slotHeight))
      {
        idx = (point.Y - _fitBoxLocation.Y)/_slotHeight;
        return true;
      }
      else
      {
        return false;
      }
    }


    protected bool IsClickOnIndependentVariable(Point point, ref int idx)
    {

      if(point.X>_VariablesX  &&
        point.X<this._IndependentVariablesWidth &&
        point.Y>=this._fitBoxLocation.Y &&
        point.Y<(_fitElement.NumberOfIndependentVariables*_slotHeight))
      {
        idx = (point.Y - _fitBoxLocation.Y)/_slotHeight;
        return true;
      }
      else
      {
        return false;
      }
    }

    protected bool IsClickOnFitRange(Point point)
    {

      if (point.X > _VariablesX &&
        point.X < this._IndependentVariablesWidth &&
        point.Y >= (_fitBoxLocation.Y + _fitElement.NumberOfIndependentVariables * _slotHeight) &&
        point.Y < (_fitBoxLocation.Y + (1+_fitElement.NumberOfIndependentVariables) * _slotHeight))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    protected bool IsClickOnDependentVariable(Point point, ref int idx)
    {

      if(point.X>_VariablesX  &&
        point.X<this._DependentVariablesWidth &&
        point.Y>=_DependentVariablesY &&
        point.Y<(_fitBoxLocation.Y + _fitBoxSize.Height))
      {
        idx = (point.Y - _DependentVariablesY)/_slotHeight;
        return true;
      }
      else
      {
        return false;
      }
    }

    protected bool IsClickOnErrorFunction(Point point, ref int idx)
    {

      if(point.X>this._errorFunctionX  &&
        point.X<(this._errorFunctionX+this._errorFunctionWidth) &&
        point.Y>=_DependentVariablesY &&
        point.Y<(_fitBoxLocation.Y + _fitBoxSize.Height))
      {
        idx = (point.Y - _DependentVariablesY)/_slotHeight;
        return true;
      }
      else
      {
        return false;
      }
    }

    protected bool IsClickOnFitFunction(Point point)
    {

      if(point.X>this._fitBoxLocation.X  &&
        point.X<(this._fitBoxLocation.X + this._fitBoxSize.Width) &&
        point.Y>=this._fitBoxLocation.Y &&
        point.Y<(_fitBoxLocation.Y + this._fitBoxSize.Height))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    private class IdxMenuItem : MenuItem
    {
      public int TagIndex;
      public IdxMenuItem(string txt, System.EventHandler evt, int idx) : base(txt,evt) { TagIndex=idx; }
    }
   
    protected override void OnClick(EventArgs e)
    {
      int idx=0;
      if(_controller!=null && _lastMouseButton==MouseButtons.Right)
      {
        // test if clicked on a dependent variable column
        if(IsClickOnDependentVariable(_lastMouseDown, ref idx))
        {
          ContextMenu menu = new ContextMenu();
          MenuItem item = new IdxMenuItem("Delete",new EventHandler(this.EhDeleteDependentVariable),idx);
          menu.MenuItems.Add(item);
          menu.Show(this,this._lastMouseDown);
          return;
        }
      }
      base.OnClick (e);
    }


    protected void EhDeleteDependentVariable(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_DeleteDependentVariable(((IdxMenuItem)sender).TagIndex);
    }

    protected override void OnDoubleClick(EventArgs e)
    {
      if(_controller==null)
      {
        base.OnDoubleClick(e);
        return;
      }

      Point point = _lastMouseDown;

      int idx=0;

      // test if clicked on a independent variable column
      if(IsClickOnIndependentVariable(point, ref idx))
      {
        _controller.EhView_ChooseIndependentColumn(idx);
        return;
      }

      if (IsClickOnFitRange(point))
      {
        _controller.EhView_ChooseFitRange();
        return;
      }

      // test if clicked on a dependent variable column
      if(IsClickOnDependentVariable(point, ref idx))
      {
        _controller.EhView_ChooseDependentColumn(idx);
        return;
      }

      // test if clicked on a error norm box
      if(IsClickOnErrorFunction(point, ref idx))
      {
        _controller.EhView_ChooseErrorFunction(idx);
        return;
      }

      if(IsClickOnExternalParameter(point, ref idx))
      {
        _controller.EhView_ChooseExternalParameter(idx);
        return;
      }

      if(IsClickOnFitFunction(point))
      {
        _controller.EhView_ChooseFitFunction();
        return;
      }

      base.OnDoubleClick (e);
    }
  }
}
