#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Summary description for ColorTypeThicknessPenControl.
	/// </summary>
	public class ColorTypeThicknessPenControl : System.Windows.Forms.UserControl, IColorTypeThicknessPenView
	{
    private System.Windows.Forms.ComboBox _cbColor;
    private System.Windows.Forms.ComboBox _cbLineType;
    private System.Windows.Forms.ComboBox _cbThickness;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ColorTypeThicknessPenControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
      this._cbColor = new System.Windows.Forms.ComboBox();
      this._cbLineType = new System.Windows.Forms.ComboBox();
      this._cbThickness = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // _cbColor
      // 
      this._cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbColor.Location = new System.Drawing.Point(64, 8);
      this._cbColor.Name = "_cbColor";
      this._cbColor.Size = new System.Drawing.Size(121, 21);
      this._cbColor.TabIndex = 0;
      this._cbColor.SelectionChangeCommitted += new System.EventHandler(this._cbColor_SelectionChangeCommitted);
      // 
      // _cbLineType
      // 
      this._cbLineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbLineType.Location = new System.Drawing.Point(64, 40);
      this._cbLineType.Name = "_cbLineType";
      this._cbLineType.Size = new System.Drawing.Size(121, 21);
      this._cbLineType.TabIndex = 1;
      this._cbLineType.SelectionChangeCommitted += new System.EventHandler(this._cbLineType_SelectionChangeCommitted);
      // 
      // _cbThickness
      // 
      this._cbThickness.Location = new System.Drawing.Point(64, 72);
      this._cbThickness.Name = "_cbThickness";
      this._cbThickness.Size = new System.Drawing.Size(121, 21);
      this._cbThickness.TabIndex = 2;
      this._cbThickness.Text = "1";
      this._cbThickness.Validating += new System.ComponentModel.CancelEventHandler(this._cbThickness_Validating);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(0, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(56, 23);
      this.label1.TabIndex = 3;
      this.label1.Text = "Color:";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(0, 40);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(56, 23);
      this.label2.TabIndex = 4;
      this.label2.Text = "Type:";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(0, 72);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(56, 23);
      this.label3.TabIndex = 5;
      this.label3.Text = "Thickness:";
      // 
      // ColorTypeThicknessPenControl
      // 
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this._cbThickness);
      this.Controls.Add(this._cbLineType);
      this.Controls.Add(this._cbColor);
      this.Name = "ColorTypeThicknessPenControl";
      this.Size = new System.Drawing.Size(184, 96);
      this.ResumeLayout(false);

    }
		#endregion

    #region IColorTypeThicknessPenView Members


    IColorTypeThicknessPenViewEventSink _controller;
    public IColorTypeThicknessPenViewEventSink Controller
    {
      get { return _controller; }
      set { _controller = value; }
    }

    public void InitializeColors(string[] names, int selection)
    {
      this._cbColor.Items.Clear();
      this._cbColor.Items.AddRange(names);
      this._cbColor.SelectedIndex = selection;
    }

    public void InitializeLineType(string[] names, int selection)
    {
      this._cbLineType.Items.Clear();
      this._cbLineType.Items.AddRange(names);
      this._cbLineType.SelectedIndex = selection;
    }

    public void InitializeLineWidth(string[] names, string selection)
    {
      this._cbThickness.Items.Clear();
      this._cbThickness.Items.AddRange(names);
      this._cbThickness.Text = selection;
    }

    #endregion

    private void _cbColor_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
    if(null!=_controller)
      _controller.EhView_ColorChanged(this._cbColor.SelectedIndex);
    }

    private void _cbLineType_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(null!=_controller)
        _controller.EhView_LineTypeChanged(this._cbLineType.SelectedIndex);
    }

    private void _cbThickness_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
    if(null!=_controller)
      _controller.EhView_LineWidthChanged(this._cbThickness.Text,e);
    }
  }
}
