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


using Altaxo.Calc.Regression.Nonlinear;


namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Summary description for FitEnsembleControl.
  /// </summary>
  [UserControlForController(typeof(IFitEnsembleViewEventSink))]
  public class FitEnsembleControl : System.Windows.Forms.UserControl, IFitEnsembleView
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public FitEnsembleControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();


    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
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

    #region IFitEnsembleView Members

    IFitEnsembleViewEventSink _controller;
    public IFitEnsembleViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller=value;
      }
    }

    public void Initialize(FitEnsemble ensemble, object[] fitEleControls)
    {
      // remove all child controls first.
      this.Controls.Clear();

      // foreach element in the ensemble, create a new control, and 
      // position the elements

      int currentYPosition = 0;
      for(int i=0;i<fitEleControls.Length;i++)
      {
        Control fectrl = (Control)fitEleControls[i];
        fectrl.Location = new Point(0,currentYPosition);
        fectrl.Size = new Size(this.ClientSize.Width, fectrl.Size.Height);
        fectrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.Controls.Add(fectrl);

        currentYPosition += fectrl.Size.Height;
        currentYPosition += System.Windows.Forms.SystemInformation.MenuHeight;
      }

      this.AutoScroll=true;
      this.AutoScrollMinSize = new Size(this.ClientSize.Width,currentYPosition);
    }


    #endregion
  }
}
