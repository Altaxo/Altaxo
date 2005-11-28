#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

using Altaxo.Main.Services;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Modal dialog to cancel a background thread.
  /// </summary>
  public class BackgroundCancelDialog : System.Windows.Forms.Form
  {
    private System.Windows.Forms.Label lblText;
    private System.Windows.Forms.Button btCancel;
    private System.Timers.Timer _timer;
    System.Threading.Thread _thread;
    IExternalDrivenBackgroundMonitor _monitor;
    private System.Windows.Forms.Button _btInterrupt;
    private System.Windows.Forms.Button _btAbort;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public BackgroundCancelDialog(  System.Threading.Thread thread, IExternalDrivenBackgroundMonitor monitor)
    {
      _thread = thread;
      _monitor = monitor;
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
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
      this.lblText = new System.Windows.Forms.Label();
      this.btCancel = new System.Windows.Forms.Button();
      this._timer = new System.Timers.Timer();
      this._btInterrupt = new System.Windows.Forms.Button();
      this._btAbort = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this._timer)).BeginInit();
      this.SuspendLayout();
      // 
      // lblText
      // 
      this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.lblText.Location = new System.Drawing.Point(8, 8);
      this.lblText.Name = "lblText";
      this.lblText.Size = new System.Drawing.Size(368, 56);
      this.lblText.TabIndex = 0;
      // 
      // btCancel
      // 
      this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btCancel.Location = new System.Drawing.Point(152, 72);
      this.btCancel.Name = "btCancel";
      this.btCancel.TabIndex = 1;
      this.btCancel.Text = "Cancel";
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // _timer
      // 
      this._timer.Enabled = true;
      this._timer.SynchronizingObject = this;
      this._timer.Elapsed += new System.Timers.ElapsedEventHandler(this._timer_Elapsed);
      // 
      // _btInterrupt
      // 
      this._btInterrupt.Location = new System.Drawing.Point(72, 72);
      this._btInterrupt.Name = "_btInterrupt";
      this._btInterrupt.TabIndex = 2;
      this._btInterrupt.Text = "Interrupt";
      this._btInterrupt.Visible = false;
      this._btInterrupt.Click += new System.EventHandler(this._btInterrupt_Click);
      // 
      // _btAbort
      // 
      this._btAbort.Location = new System.Drawing.Point(232, 72);
      this._btAbort.Name = "_btAbort";
      this._btAbort.TabIndex = 3;
      this._btAbort.Text = "Abort";
      this._btAbort.Visible = false;
      this._btAbort.Click += new System.EventHandler(this._btAbort_Click);
      // 
      // BackgroundCancelDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(384, 98);
      this.Controls.Add(this._btAbort);
      this.Controls.Add(this._btInterrupt);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.lblText);
      this.Name = "BackgroundCancelDialog";
      this.Text = "Working...";
      this.Closing += new System.ComponentModel.CancelEventHandler(this.BackgroundCancelDialog_Closing);
      ((System.ComponentModel.ISupportInitialize)(this._timer)).EndInit();
      this.ResumeLayout(false);

    }
    #endregion

   
  
    private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      if(!_thread.IsAlive)
      {
        this.Close();
      }

      this.lblText.Text = _monitor.ReportText;

      _monitor.ShouldReport = true;
    }


  
    private void btCancel_Click(object sender, System.EventArgs e)
    {
      _monitor.CancelledByUser = true;
      _btInterrupt.Visible = true;
    }

    private void BackgroundCancelDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_thread.IsAlive)
        e.Cancel = true;
    }

    private void _btInterrupt_Click(object sender, System.EventArgs e)
    {
      if(_thread.IsAlive)
        _thread.Interrupt();

      _btAbort.Visible = true;
    
    }

    private void _btAbort_Click(object sender, System.EventArgs e)
    {
      if(_thread.IsAlive)
        _thread.Abort();
    }
  }
}
