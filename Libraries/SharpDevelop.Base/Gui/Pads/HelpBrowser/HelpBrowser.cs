// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Reflection;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace ICSharpCode.SharpDevelop.Gui.Pads 
{
	public class HelpBrowserWindow : BrowserPane
	{
		public HelpBrowserWindow() : base(true)
		{
			ContentName = "Help";
		}
	}
	
	class HelpLinkInformation
	{
		string link;
		bool isMSDN = false;
		
		public string Link {
			get {
				return link;
			}
			set {
				link = value;
			}
		}
		public bool IsMSDN {
			get {
				return isMSDN;
			}
			set {
				isMSDN = value;
			}
		}
		
		public HelpLinkInformation(string link, bool isMSDN)
		{
			this.link = link;
			this.isMSDN = isMSDN;
		}
		
	}
	
	public class HelpBrowser : AbstractPadContent
	{
		static readonly string helpPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) +
		                                  Path.DirectorySeparatorChar + ".." +
		                                  Path.DirectorySeparatorChar + "doc" +
		                                  Path.DirectorySeparatorChar + "help" +
		                                  Path.DirectorySeparatorChar;
		
		static readonly string helpFileName = helpPath + "SharpDevelopHelp.zip";
		static readonly string mainTOCFile  = "HelpConv.xml";
		
		Panel     browserPanel = new Panel();
		TreeView  treeView     = new TreeView();
		
		HelpBrowserWindow helpBrowserWindow = null;
		
		public override Control Control {
			get {
				return browserPanel;
			}
		}
		
		public HelpBrowser() : base("${res:MainWindow.Windows.HelpScoutLabel}", "Icons.16x16.HelpIcon")
		{
			treeView.Dock = DockStyle.Fill;
			treeView.ImageList = new ImageList();
			treeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			treeView.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.HelpClosedFolder"));
			treeView.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.HelpOpenFolder"));
			
			treeView.ImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.HelpTopic"));
			treeView.BeforeExpand   += new TreeViewCancelEventHandler(BeforeExpand);			
			treeView.BeforeCollapse += new TreeViewCancelEventHandler(BeforeCollapse);
			treeView.AfterSelect    += new TreeViewEventHandler(SelectNode);
			browserPanel.Controls.Add(treeView);
			
			LoadHelpfile();
		}
		
		/// <remarks>
		/// Parses the xml tree and generates a TreeNode tree out of it.
		/// </remarks>
		void ParseTree(TreeNodeCollection nodeCollection, XmlNode parentNode)
		{
			foreach (XmlNode node in parentNode.ChildNodes) {
				switch (node.Name) {
					case "HelpFolder":
						TreeNode newFolderNode = new TreeNode(node.Attributes["name"].InnerText);
						newFolderNode.ImageIndex = newFolderNode.SelectedImageIndex = 0;
						ParseTree(newFolderNode.Nodes, node);
						
						bool isMSDNLink = node.Attributes["ismsdn"] != null && node.Attributes["ismsdn"].Value.ToLower() == "true";
						newFolderNode.Tag = node.Attributes["link"] != null ? new HelpLinkInformation(node.Attributes["link"].InnerText , isMSDNLink) : null;
						
						nodeCollection.Add(newFolderNode);
						break;
					case "HelpTopic":
						TreeNode newNode = new TreeNode(node.Attributes["name"].InnerText);
						newNode.ImageIndex = newNode.SelectedImageIndex = 2;
						
						isMSDNLink = node.Attributes["ismsdn"] != null && node.Attributes["ismsdn"].Value.ToLower() == "true";
						newNode.Tag = new HelpLinkInformation(node.Attributes["link"].InnerText, isMSDNLink);
						nodeCollection.Add(newNode);
						break;
					case "HelpReference":
						TreeNode newReferenceNode = new TreeNode("Reference");
						newReferenceNode.Tag = node.Attributes["reference"].InnerText;
						nodeCollection.Add(newReferenceNode);
						break;
				}
			}
		}
		
		XmlDocument LoadCompressedXmlDocument(string requestedFile)
		{
			ZipInputStream s = new ZipInputStream(File.OpenRead(helpFileName));
		
			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null) {
				if (theEntry.Name == requestedFile) {
					
					StringBuilder sb = new StringBuilder();
					int size = 2048;
					byte[] data = new byte[2048];
					while (true) {
						size = s.Read(data, 0, data.Length);
						if (size > 0) {
							sb.Append(Encoding.UTF8.GetString(data, 0, size));
						} else {
							break;
						}
					}
					s.Close();
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(sb.ToString());
					return doc;
				}
			}
			s.Close();
			Debug.Assert(false);
			return null;
		}
		
		void LoadHelpfile()
		{
			XmlDocument doc = LoadCompressedXmlDocument(mainTOCFile);new XmlDocument();
			ParseTree(treeView.Nodes, doc.DocumentElement);
		}
		
		void HelpBrowserClose(object sender, EventArgs e)
		{
			helpBrowserWindow = null;
		}
		
		public void ShowHelpBrowser(string url)
		{
			if (helpBrowserWindow == null) {
				helpBrowserWindow = new HelpBrowserWindow();
				WorkbenchSingleton.Workbench.ShowView(helpBrowserWindow);
				helpBrowserWindow.WorkbenchWindow.CloseEvent += new EventHandler(HelpBrowserClose);
			}
			helpBrowserWindow.Load(url);
			helpBrowserWindow.WorkbenchWindow.SelectWindow();
		}
		
		void ShowHelp(TreeNode node)
		{
			if (node == null || node.Tag == null) {
				return;
			}
			string navigationName;
			
			if(((HelpLinkInformation)node.Tag).IsMSDN == true) {
				navigationName = ((HelpLinkInformation)node.Tag).Link;
			} else {
				navigationName = "mk:@MSITStore:" + helpPath + ((HelpLinkInformation)node.Tag).Link;
			}
			
			ShowHelpBrowser(navigationName);
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			ShowHelp(e.Node);
		}
		
		void BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.ImageIndex < 2) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
			}
			
			TreeNode[] nodes = new TreeNode[e.Node.Nodes.Count];
			e.Node.Nodes.CopyTo(nodes, 0);
			e.Node.Nodes.Clear();
			
			foreach (TreeNode node in nodes) {
				if (node.Tag is string) {
					
					XmlDocument doc = LoadCompressedXmlDocument(node.Tag.ToString());
					ParseTree(e.Node.Nodes, doc.DocumentElement);
				} else {
					e.Node.Nodes.Add(node);
				}
			}
			ShowHelp(e.Node);
		}
		
		void BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{ 
			if (e.Node.ImageIndex < 2) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
			}
		}
	}
}
