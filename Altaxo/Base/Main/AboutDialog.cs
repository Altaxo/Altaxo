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

namespace Altaxo.Main
{
  /// <summary>
  /// Summary description for AboutDialog.
  /// </summary>
  public class AboutDialog : System.Windows.Forms.Form
  {
    private System.Windows.Forms.Button m_btOK;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.LinkLabel m_LinkLabel;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public AboutDialog()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      // Create a new link using the Add method of the LinkCollection class.
      int len = m_LinkLabel.Text.Length;
      int pos = m_LinkLabel.Text.IndexOf("http://");
      m_LinkLabel.Links.Add(pos,len-pos,"http://sourceforge.net/projects/altaxo");


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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
      this.m_btOK = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.m_LinkLabel = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // m_btOK
      // 
      this.m_btOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_btOK.Location = new System.Drawing.Point(216, 432);
      this.m_btOK.Name = "m_btOK";
      this.m_btOK.Size = new System.Drawing.Size(75, 23);
      this.m_btOK.TabIndex = 0;
      this.m_btOK.Text = "OK";
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(192, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(104, 32);
      this.label1.TabIndex = 1;
      this.label1.Text = "Altaxo";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(64, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(432, 32);
      this.label2.TabIndex = 2;
      this.label2.Text = "data processing / data plotting program";
      // 
      // textBox1
      // 
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.Location = new System.Drawing.Point(8, 112);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBox1.Size = new System.Drawing.Size(496, 312);
      this.textBox1.TabIndex = 3;
      this.textBox1.Text = resources.GetString("textBox1.Text");
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.Location = new System.Drawing.Point(312, 16);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(192, 16);
      this.label3.TabIndex = 4;
      this.label3.Text = "(C) 2002-2007 Dr. Dirk Lellinger";
      // 
      // m_LinkLabel
      // 
      this.m_LinkLabel.Location = new System.Drawing.Point(16, 80);
      this.m_LinkLabel.Name = "m_LinkLabel";
      this.m_LinkLabel.Size = new System.Drawing.Size(488, 16);
      this.m_LinkLabel.TabIndex = 5;
      this.m_LinkLabel.TabStop = true;
      this.m_LinkLabel.Text = "You can obtain the latest version of Altaxo from http://sourceforge.net/projects/" +
          "altaxo";
      this.m_LinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_LinkLabel_LinkClicked);
      // 
      // AboutDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(512, 458);
      this.Controls.Add(this.m_LinkLabel);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.m_btOK);
      this.Name = "AboutDialog";
      this.Text = "About Altaxo";
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private void m_LinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
    {
      // Determine which link was clicked within the LinkLabel.
      m_LinkLabel.Links[m_LinkLabel.Links.IndexOf(e.Link)].Visited = true;
      // Display the appropriate link based on the value of the LinkData property of the Link object.
      System.Diagnostics.Process.Start(e.Link.LinkData.ToString());

    }
  }
}
