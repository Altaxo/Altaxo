// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

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

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	class FileIcon
	{
		[DllImport("shell32.dll")]
		static extern int SHGetFileInfo(string pszPath,
												  uint dwFileAttributes,
												  out SHFILEINFO psfi,
												  uint cbfileInfo,
												  SHGFI uFlags);
		
		[StructLayout(LayoutKind.Sequential)]
		struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			
			[MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
			public string szDisplayName;
			
			[MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
			public string szTypeName;
		}
		
		enum SHGFI
		{
			SmallIcon			= 0x00000001,
			LargeIcon			= 0x00000000,
			Icon					= 0x00000100,
			DisplayName			= 0x00000200,
			Typename				= 0x00000400,
			SysIconIndex		= 0x00004000,
			UseFileAttributes	= 0x00000010
		}
		
		public static Bitmap GetBitmap(string strPath, bool bSmall)
		{
			SHFILEINFO info = new SHFILEINFO();
			int cbFileInfo = Marshal.SizeOf(info);
			SHGFI flags;
			
			if(bSmall) {
				flags = SHGFI.Icon|SHGFI.SmallIcon|SHGFI.UseFileAttributes;
			} else {
				flags = SHGFI.Icon|SHGFI.LargeIcon|SHGFI.UseFileAttributes;
			}
			
			SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);
			return Bitmap.FromHicon(info.hIcon);
		}
	}
	public enum DriveType {
		Unknown     = 0,
		NoRoot      = 1,
		Removeable  = 2,
		Fixed       = 3,
		Remote      = 4,
		Cdrom       = 5,
		Ramdisk     = 6
	}
	
	public class DriveObject 
	{
		string text  = null;
		string drive = null;
		
		public string Drive {
			get {
				return drive;
			}
		}
		class NativeMethods {
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern int GetVolumeInformation(string volumePath,
			                                              StringBuilder volumeNameBuffer,
			                                              int volNameBuffSize,
			                                              ref int volumeSerNr,
			                                              ref int maxComponentLength,
			                                              ref int fileSystemFlags,
			                                              StringBuilder fileSystemNameBuffer,
			                                              int fileSysBuffSize);
			
			[DllImport("kernel32.dll")]
			public static extern DriveType GetDriveType(string driveName);
		}
		
		public static string VolumeLabel(string volumePath)
		{
			try {
				StringBuilder volumeName  = new StringBuilder(128);
				int dummyInt = 0;
				NativeMethods.GetVolumeInformation(volumePath,
				                                   volumeName,
				                                   128,
				                                   ref dummyInt,
				                                   ref dummyInt,
				                                   ref dummyInt,
				                                   null,
				                                   0);
				return volumeName.ToString();
			} catch (Exception) {
				return String.Empty;
			}
		}
		
		public static DriveType GetDriveType(string driveName)
		{
			return NativeMethods.GetDriveType(driveName);
		}
		
		public static Image GetImageForFile(string fileName)
		{
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			int index = iconService.GetImageIndexForFile(fileName);
			
			// integrated file (cmbx for example) may not be 
			// registered so look first in the imagelist
			if (index > 0) {
				return iconService.ImageList.Images[index];
			} else {
				return FileIcon.GetBitmap(fileName, true);
			}
		}
		
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public DriveObject(string drive) 
		{
			this.drive = drive;
			
			text = drive.Substring(0, 2);
			
			switch(GetDriveType(drive)) {
				case DriveType.Removeable:
					text += " (Removeable)";
				break;
				case DriveType.Fixed:
					text += " (Fixed)";
				break;
				case DriveType.Cdrom:
					text += " (CD)";
				break;
				case DriveType.Remote:
					text += " (Remote)";
				break;
			}
		}
		
		public override string ToString()
		{
			return text;
		}
	}
	
	class IconManager
	{
		private static ImageList icons = new ImageList();
		private static Hashtable iconIndecies = new Hashtable();
		private static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		static IconManager()
		{
			icons.ColorDepth = ColorDepth.Depth32Bit;
		}
		
		
		public static ImageList List
		{
			get {
				return icons;
			}
		}
		public static int GetIndexForFile(string file)
		{
			string key;
			
			// icon files and exe files can have their custom icons
			if(Path.GetExtension(file).ToLower() == ".ico" || 
			   Path.GetExtension(file).ToLower() == ".exe") {
				key = file;
			} else {
				key = Path.GetExtension(file).ToLower();
			}
			
			// clear the icon cache
			if(icons.Images.Count > 100) {
				icons.Images.Clear();
				iconIndecies.Clear();
			}
			
			if(iconIndecies.Contains(key)) {
				return (int)iconIndecies[key];
			} else {
				icons.Images.Add(DriveObject.GetImageForFile(file));
				int index = icons.Images.Count - 1;
				iconIndecies.Add(key, index);
				return index;
			}
		}
	}
	
	public class FileList : ListView
	{
		private FileSystemWatcher watcher;
		
//		private MagicMenus.PopupMenu menu = null;
		
		public FileList()
		{
			ResourceManager resources = new ResourceManager("ProjectComponentResources", this.GetType().Module.Assembly);
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			Columns.Add("File", 100, HorizontalAlignment.Left);
			Columns.Add("Size", -2, HorizontalAlignment.Right);
			Columns.Add("Last modified", -2, HorizontalAlignment.Left);
			
//			menu = new MagicMenus.PopupMenu();
//			menu.MenuCommands.Add(new MagicMenus.MenuCommand("Delete file", new EventHandler(deleteFiles)));
//			menu.MenuCommands.Add(new MagicMenus.MenuCommand("Rename", new EventHandler(renameFile)));
			
			try {
				watcher = new FileSystemWatcher();
			} catch {}
			
			if(watcher != null) {
				watcher.NotifyFilter = NotifyFilters.FileName;
				watcher.EnableRaisingEvents = false;
				
				watcher.Renamed += new RenamedEventHandler(fileRenamed);
				watcher.Deleted += new FileSystemEventHandler(fileDeleted);
				watcher.Created += new FileSystemEventHandler(fileCreated);
				watcher.Changed += new FileSystemEventHandler(fileChanged);
			}
			
			HideSelection 	= false;
			GridLines		= true;
			LabelEdit		= true;
			SmallImageList = IconManager.List;
			HeaderStyle 	= ColumnHeaderStyle.Nonclickable;
			View 				= View.Details;
			Alignment		= ListViewAlignment.Left;
		}
		
		void fileDeleted(object sender, FileSystemEventArgs e)
		{
			foreach(FileListItem fileItem in Items)
			{
				if(fileItem.FullName.ToLower() == e.FullPath.ToLower()) {
					Items.Remove(fileItem);
					break;
				}
			}
		}
		
		void fileChanged(object sender, FileSystemEventArgs e)
		{
			foreach(FileListItem fileItem in Items)
			{
				if(fileItem.FullName.ToLower() == e.FullPath.ToLower()) {
					
					FileInfo info = new FileInfo(e.FullPath);
					
					fileItem.SubItems[1].Text = Math.Round((double)info.Length / 1024).ToString() + " KB";
					fileItem.SubItems[2].Text = info.LastWriteTime.ToString();
					break;
				}
			}
		}
		
		void fileCreated(object sender, FileSystemEventArgs e)
		{
			FileInfo info = new FileInfo(e.FullPath);
			
			ListViewItem fileItem = Items.Add(new FileListItem(e.FullPath));
			fileItem.SubItems.Add(Math.Round((double)info.Length / 1024).ToString() + " KB");
			fileItem.SubItems.Add(info.LastWriteTime.ToString());
			
			Items.Add(fileItem);
		}
		
		void fileRenamed(object sender, RenamedEventArgs e)
		{
			foreach(FileListItem fileItem in Items)
			{
				if(fileItem.FullName.ToLower() == e.OldFullPath.ToLower()) {
					fileItem.FullName = e.FullPath;
					fileItem.Text = e.Name;
					break;
				}
			}
		}
		
		void renameFile(object sender, EventArgs e)
		{
			if(SelectedItems.Count == 1) {
				SelectedItems[0].BeginEdit();
			}
		}
		
		void deleteFiles(object sender, EventArgs e)
		{
			IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			
			if (messageService.AskQuestion("Are you sure ?", "Delete files")) {
				foreach(FileListItem fileItem in SelectedItems)
				{
					try {
						File.Delete(fileItem.FullName);
					} catch(Exception ex) {
						messageService.ShowError(ex, "Couldn't delete file '" + Path.GetFileName(fileItem.FullName) + "'");
						break;
					}
				}
			}
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			
			ListViewItem itemUnderMouse = GetItemAt(PointToScreen(new Point(e.X, e.Y)).X, PointToScreen(new Point(e.X, e.Y)).Y);
			
			if(e.Button == MouseButtons.Right && this.SelectedItems.Count > 0) {
//				menu.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
			}
		}
		
		protected override void OnAfterLabelEdit(LabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);
			
			if(e.Label == null) {
				e.CancelEdit = true;
				return;
			}
			
			string filename = ((FileListItem)Items[e.Item]).FullName;
			string newname = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + e.Label;
			
			try {
				File.Move(filename, newname);
				((FileListItem)Items[e.Item]).FullName = newname;
			} catch(Exception ex) {
				e.CancelEdit = true;
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(ex, "Rename failed");
			}
		}
		
		public void ShowFilesInPath(string path)
		{
			string[] files;
			Items.Clear();
		
			try {
				files = Directory.GetFiles(path);
			} catch (Exception) {
				return;
			}
			
			watcher.Path = path;
			watcher.EnableRaisingEvents = true;
			
			foreach (string file in files) {
				FileInfo info = new FileInfo(file);
				ListViewItem fileItem = Items.Add(new FileListItem(file));
				fileItem.SubItems.Add(Math.Round((double)info.Length / 1024).ToString() + " KB");
				fileItem.SubItems.Add(info.LastWriteTime.ToString());
			}
			
			EndUpdate();
		}
		
		public class FileListItem : ListViewItem
		{
			string fullname;
			public string FullName {
				get {
					return fullname;
				} set {
					fullname = value;
				}
			}
			
			public FileListItem(string fullname) : base(Path.GetFileName(fullname))
			{
				this.fullname = fullname;
				ImageIndex = IconManager.GetIndexForFile(fullname);
			}
		}
	}
	
	public class FileScout : UserControl, IPadContent
	{
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		public Control Control {
			get {
				return this;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.FileScoutLabel");
			}
		}
		
		public string Icon {
			get {
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
			
			foreach (FileList.FileListItem item in filelister.SelectedItems) {
				
				switch (Path.GetExtension(item.FullName)) {
					case ".cmbx":
					case ".prjx":
						projectService.OpenCombine(item.FullName);
						break;
					default:
						fileService.OpenFile(item.FullName);
						break;
				}
			}
		}
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;
	}
	
	public class ShellTree : TreeView
	{
		public string NodePath {
			get {
				return (string)SelectedNode.Tag;
			}
			set {
				PopulateShellTree(value);
			}
		}
		
		public ShellTree()
		{
			Sorted = true;
			TreeNode rootNode = Nodes.Add("Desktop");
			rootNode.ImageIndex = 6;
			rootNode.SelectedImageIndex = 6;
			rootNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			
			TreeNode myFilesNode = rootNode.Nodes.Add("My Documents");
			myFilesNode.ImageIndex = 7;
			myFilesNode.SelectedImageIndex = 7;
			myFilesNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			myFilesNode.Nodes.Add("");
			
			TreeNode computerNode = rootNode.Nodes.Add("My Computer");
			computerNode.ImageIndex = 8;
			computerNode.SelectedImageIndex = 8;
			computerNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			
			foreach (string driveName in Environment.GetLogicalDrives()) {
				DriveObject drive = new DriveObject(driveName);
				
				TreeNode node = new TreeNode(drive.ToString());
				node.Nodes.Add(new TreeNode(""));
				node.Tag = driveName.Substring(0, driveName.Length - 1);
				computerNode.Nodes.Add(node);
				
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				
				switch(DriveObject.GetDriveType(driveName)) {
					case DriveType.Removeable:
						node.ImageIndex = node.SelectedImageIndex = 2;
						break;
					case DriveType.Fixed:
						node.ImageIndex = node.SelectedImageIndex = 3;
						break;
					case DriveType.Cdrom:
						node.ImageIndex = node.SelectedImageIndex = 4;
						break;
					case DriveType.Remote:
						node.ImageIndex = node.SelectedImageIndex = 5;
						break;
					default:
						node.ImageIndex = node.SelectedImageIndex = 3;
						break;
				}
			}
			
			foreach (string directory in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))) {
				TreeNode node = rootNode.Nodes.Add(Path.GetFileName(directory));
				node.Tag = directory;
				node.ImageIndex = node.SelectedImageIndex = 0;
				node.Nodes.Add(new TreeNode(""));
			}
			
			rootNode.Expand();
			computerNode.Expand();
			
			InitializeComponent();
		}
		
		int getNodeLevel(TreeNode node)
		{
			TreeNode parent = node;
			int depth = 0;
			
			while(true)
			{
				parent = parent.Parent;
				if(parent == null) {
					return depth;
				}
				depth++;
			}
		}
		
		void InitializeComponent ()
		{
			BeforeSelect   += new TreeViewCancelEventHandler(SetClosedIcon);
			AfterSelect    += new TreeViewEventHandler(SetOpenedIcon);
		}
		
		void SetClosedIcon(object sender, TreeViewCancelEventArgs e) // Set icon as closed
		{
			if (SelectedNode != null) {
				if(getNodeLevel(SelectedNode) > 2) {
					SelectedNode.ImageIndex = SelectedNode.SelectedImageIndex = 0;
				}
			}
		}
		
		void SetOpenedIcon(object sender, TreeViewEventArgs e) // Set icon as opened
		{
			if(getNodeLevel(e.Node) > 2) {
				if (e.Node.Parent != null && e.Node.Parent.Parent != null) {
					e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
				}
			}
		}
		
		void PopulateShellTree(string path)
		{
			string[]  pathlist = path.Split(new char[] { Path.DirectorySeparatorChar });
			TreeNodeCollection  curnode = Nodes;
			
			foreach(string dir in pathlist) {
				
				foreach(TreeNode childnode in curnode) {
					if (((string)childnode.Tag).ToUpper().Equals(dir.ToUpper())) {
						SelectedNode = childnode;
						
						PopulateSubDirectory(childnode, 2);
						childnode.Expand();
						
						curnode = childnode.Nodes;
						break;
					}
				}
			}
		}
		
		void PopulateSubDirectory(TreeNode curNode, int depth)
		{
			if (--depth < 0) {
				return;
			}
			
			if (curNode.Nodes.Count == 1 && curNode.Nodes[0].Text.Equals("")) {
				
				string[] directories = null;
				try {
					directories  = Directory.GetDirectories(curNode.Tag.ToString() + Path.DirectorySeparatorChar);
				} catch (Exception) {
					return;
				}
				
				curNode.Nodes.Clear();
				
				foreach (string fulldir in directories) {
					try {
						string dir = System.IO.Path.GetFileName(fulldir);
						
						FileAttributes attr = File.GetAttributes(fulldir);
						if ((attr & FileAttributes.Hidden) == 0) {
							TreeNode node   = curNode.Nodes.Add(dir);
							node.Tag = curNode.Tag.ToString() + Path.DirectorySeparatorChar + dir;
							node.ImageIndex = node.SelectedImageIndex = 0;
							
							node.Nodes.Add(""); // Add dummy child node to make node expandable
							
							PopulateSubDirectory(node, depth);
						}
					} catch (Exception) {
					}
				}
			} else {
				foreach (TreeNode node in curNode.Nodes) {
					PopulateSubDirectory(node, depth); // Populate sub directory
				}
			}
		}
		
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			
			try {
				// do not populate if the "My Cpmputer" node is expaned
				if(e.Node.Parent != null && e.Node.Parent.Parent != null) {
					PopulateSubDirectory(e.Node, 2);
					Cursor.Current = Cursors.Default;
				} else {
					PopulateSubDirectory(e.Node, 1);
					Cursor.Current = Cursors.Default;
				}
			} catch (Exception excpt) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(excpt, "Device error");
				e.Cancel = true;
			}
			
			Cursor.Current = Cursors.Default;
		}
	}
}
