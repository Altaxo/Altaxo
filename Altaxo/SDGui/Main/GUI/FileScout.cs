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
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.Gui.Pads;


namespace Altaxo.Main.GUI
{
	public class FileScout : UserControl, ICSharpCode.SharpDevelop.Gui.IPadContent
	{
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
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
				return resourceService.GetString("MainWindow.Windows.FileScoutLabel");
			}
		}
		
		public string Icon 
		{
			get 
			{
				return "Icons.16x16.OpenFolderBitmap";
			}
		}
		
		public void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		Splitter      splitter1     = new Splitter();
		
		FileList   filelister = new FileList();
		ShellTree  filetree   = new ShellTree();
		
		public FileScout()
		{
			Dock      = DockStyle.Fill;
			
			filetree.Dock = DockStyle.Top;
			filetree.BorderStyle = BorderStyle.Fixed3D;
			filetree.Location = new System.Drawing.Point(0, 22);
			filetree.Size = new System.Drawing.Size(184, 157);
			filetree.TabIndex = 1;
			filetree.AfterSelect += new TreeViewEventHandler(DirectorySelected);
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.FLOPPY"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.DRIVE"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.CDROM"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.NETWORK"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Desktop"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PersonalFiles"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.MyComputer"));
			
			filetree.ImageList = imglist;
			
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
			this.Controls.Add(filetree);
		}
		
		void DirectorySelected(object sender, TreeViewEventArgs e)
		{
			filelister.ShowFilesInPath(filetree.NodePath + Path.DirectorySeparatorChar);
		}
		
		void FileSelected(object sender, EventArgs e)
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IFileService    fileService    = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			foreach (FileList.FileListItem item in filelister.SelectedItems) 
			{
				
				switch (Path.GetExtension(item.FullName)) 
				{
					case ".axoprj":
					case ".axoprz":
						Current.ProjectService.OpenProject(item.FullName);
						break;
					case ".spc":
						if(Current.Workbench.ActiveViewContent is Altaxo.Worksheet.GUI.WorksheetController)
						{
							Altaxo.Worksheet.GUI.WorksheetController ctrl = (Altaxo.Worksheet.GUI.WorksheetController)Current.Workbench.ActiveViewContent;
							string [] files = new string[] { item.FullName };
							Altaxo.Serialization.Galactic.Import.ImportSpcFiles(files,ctrl.DataTable);
						}
						break;
					default:
						fileService.OpenFile(item.FullName);
						break;
				}
			}
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
