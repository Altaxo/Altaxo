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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Gui.Scripting
{
  /// <summary>
  /// Summary description for ScriptExecutionDialog.
  /// </summary>
  public class ScriptExecutionDialog : System.Windows.Forms.Form
  {
    private System.Windows.Forms.Button _btOk;
    private System.Windows.Forms.Button _btCompile;
    private System.Windows.Forms.Button _btUpdate;
    private System.Windows.Forms.Button _btCancel;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public ScriptExecutionDialog()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

   }

    IScriptController _controller;
    public ScriptExecutionDialog(IScriptController controller)
    {
      _controller = controller;
      InitializeComponent();

      if(_controller!=null && _controller.ViewObject!=null)
      {
        Control mycontrol = (Control)_controller.ViewObject;
        mycontrol.Location = new Point(0,0);
        mycontrol.Size = new Size(this._btOk.Location.X - System.Windows.Forms.SystemInformation.MenuHeight/2,this.ClientSize.Height);
        mycontrol.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.Controls.Add(mycontrol);
      }
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

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this._btOk = new System.Windows.Forms.Button();
      this._btCompile = new System.Windows.Forms.Button();
      this._btUpdate = new System.Windows.Forms.Button();
      this._btCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // _btOk
      // 
      this._btOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btOk.Location = new System.Drawing.Point(728, 16);
      this._btOk.Name = "_btOk";
      this._btOk.Size = new System.Drawing.Size(56, 23);
      this._btOk.TabIndex = 0;
      this._btOk.Text = "Ok";
      this._btOk.Click += new System.EventHandler(this._btOk_Click);
      // 
      // _btCompile
      // 
      this._btCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btCompile.Location = new System.Drawing.Point(728, 64);
      this._btCompile.Name = "_btCompile";
      this._btCompile.Size = new System.Drawing.Size(56, 23);
      this._btCompile.TabIndex = 1;
      this._btCompile.Text = "Compile";
      this._btCompile.Click += new System.EventHandler(this._btCompile_Click);
      // 
      // _btUpdate
      // 
      this._btUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btUpdate.Location = new System.Drawing.Point(728, 120);
      this._btUpdate.Name = "_btUpdate";
      this._btUpdate.Size = new System.Drawing.Size(56, 23);
      this._btUpdate.TabIndex = 2;
      this._btUpdate.Text = "Update";
      this._btUpdate.Click += new System.EventHandler(this._btUpdate_Click);
      // 
      // _btCancel
      // 
      this._btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btCancel.Location = new System.Drawing.Point(728, 528);
      this._btCancel.Name = "_btCancel";
      this._btCancel.Size = new System.Drawing.Size(56, 23);
      this._btCancel.TabIndex = 3;
      this._btCancel.Text = "Cancel";
      this._btCancel.Click += new System.EventHandler(this._btCancel_Click);
      // 
      // ScriptExecutionDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(792, 566);
      this.Controls.Add(this._btCancel);
      this.Controls.Add(this._btUpdate);
      this.Controls.Add(this._btCompile);
      this.Controls.Add(this._btOk);
      this.Name = "ScriptExecutionDialog";
      this.Text = "ScriptExecutionDialog";
      this.ResumeLayout(false);

    }
    #endregion


    private void _btCompile_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.Compile();
    }

    private void _btUpdate_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.Update();

      DialogResult = DialogResult.OK;
      Close();
    }

    private void _btCancel_Click(object sender, System.EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void _btOk_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
      {
        if(_controller.Apply())
        {
          DialogResult = DialogResult.OK;
          Close();
        }
      }
    }
  }
}
