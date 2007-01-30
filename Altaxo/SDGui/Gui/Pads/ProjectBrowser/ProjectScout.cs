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
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Xml;
using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;

namespace Altaxo.Gui.Pads
{

  public class ProjectScout : UserControl, ICSharpCode.SharpDevelop.Gui.IPadContent
  {
   
    public Control Control 
    {
      get 
      {
        return this;
      }
    }
    
    public string Title 
    {
      get 
      {
        return ResourceService.GetString("MainWindow.Windows.AltaxoProjectScoutLabel");
      }
    }
    
    public string Icon 
    {
      get 
      {
        return "Icons.16x16.OpenFolderBitmap";
      }
    }

    string category;
    public string Category 
    {
      get 
      {
        return category;
      }
      set
      {
        category = value;
      }
    }
    string[] shortcut; // TODO: Inherit from AbstractPadContent
    public string[] Shortcut 
    {
      get 
      {
        return shortcut;
      }
      set 
      {
        shortcut = value;
      }
    }
    
    public void RedrawContent()
    {
      OnTitleChanged(null);
      OnIconChanged(null);
    }
    
    Splitter      splitter1     = new Splitter();
    
    //FileList   filelister = new FileList();
    ProjectTree  filetree   = new ProjectTree();
    
    public ProjectScout()
    {
      Dock      = DockStyle.Fill;


      filetree.Dock = DockStyle.Fill;
      filetree.BorderStyle = BorderStyle.Fixed3D;
      filetree.Location = new System.Drawing.Point(0, 22);
      filetree.Size = new System.Drawing.Size(184, 157);
      filetree.TabIndex = 1;
      filetree.AfterSelect += new TreeViewEventHandler(DirectorySelected);
      ImageList imglist = new ImageList();
      imglist.ColorDepth = ColorDepth.Depth32Bit;
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.FLOPPY"));
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.DRIVE"));
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.CDROM"));
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.NETWORK"));
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Desktop"));
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.PersonalFiles"));
      imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.MyComputer"));
      
      filetree.ImageList = imglist;
      
      /*
      filelister.Dock = DockStyle.Fill;
      filelister.BorderStyle = BorderStyle.Fixed3D;
      filelister.Location = new System.Drawing.Point(0, 184);
      
      filelister.Sorting = SortOrder.Ascending;
      filelister.Size = new System.Drawing.Size(184, 450);
      filelister.TabIndex = 3;
      filelister.ItemActivate += new EventHandler(FileSelected);
      
      splitter1.Dock = DockStyle.Top;
      splitter1.Location = new System.Drawing.Point(0, 179);
      splitter1.Size = new System.Drawing.Size(184, 5);
      splitter1.TabIndex = 2;
      splitter1.TabStop = false;
      splitter1.MinSize = 50;
      splitter1.MinExtra = 50;
      
      this.Controls.Add(filelister);
      this.Controls.Add(splitter1);
      */
      this.Controls.Add(filetree);
    }
    
    void DirectorySelected(object sender, TreeViewEventArgs e)
    {
      //filelister.ShowFilesInPath(filetree.NodePath + Path.DirectorySeparatorChar);
    }
    
    void FileSelected(object sender, EventArgs e)
    {
      
    }
    protected virtual void OnTitleChanged(EventArgs e)
    {
      if (TitleChanged != null) 
      {
        TitleChanged(this, e);
      }
    }
    protected virtual void OnIconChanged(EventArgs e)
    {
      if (IconChanged != null) 
      {
        IconChanged(this, e);
      }
    }
    public event EventHandler TitleChanged;
    public event EventHandler IconChanged;
  }
  



  
}
