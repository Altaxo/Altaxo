// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;

using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Internal.Project;
using SharpDevelop.Internal.Parser;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class DefaultDotNetClassScoutNodeBuilder : IClassScoutNodeBuilder
	{
		int imageIndexOffset;
		IAmbience languageConversion;

		public DefaultDotNetClassScoutNodeBuilder()
		{
		}

		public bool CanBuildClassTree(IProject project)
		{
			return true;
		}

		void GetCurrentAmbience()
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));

			languageConversion = ambienceService.CurrentAmbience;
			languageConversion.ConversionFlags = ConversionFlags.None;
		}

		public void AddToClassTree(TreeNode parentNode, ParseInformationEventArgs e)
		{
			AddToClassTree(parentNode, e.FileName, (ICompilationUnit)e.ParseInformation.MostRecentCompilationUnit);
		}
		
		public void RemoveFromClassTree(TreeNode parentNode, ParseInformationEventArgs e) {
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			
			TreeNode classNode = new TreeNode();
			
			ICompilationUnit unit = (ICompilationUnit)e.ParseInformation.MostRecentCompilationUnit;
			foreach (IClass c in unit.Classes) {
				classNode.Text = c.Name;
				classNode.SelectedImageIndex = classNode.ImageIndex = classBrowserIconService.GetIcon(c);
				TreeNode node = GetNodeByPath(c.Namespace, parentNode.Nodes, false);
				if (node != null) {
					int oldIndex = SortUtility.BinarySearch(classNode, node.Nodes, TreeNodeComparer.Default);
					if(oldIndex >= 0) {
						node.Nodes[oldIndex].Remove();
					}
				}
			}
		}
		
		public void UpdateClassTree(TreeNode projectNode)
		{
			TreeNode newNode = BuildClassTreeNode((IProject)projectNode.Tag, imageIndexOffset);
			projectNode.Nodes.Clear();
			foreach (TreeNode node in newNode.Nodes) {
				projectNode.Nodes.Add(node);
			}
			SortUtility.QuickSort(projectNode.Nodes, TreeNodeComparer.Default);
		}

		public TreeNode BuildClassTreeNode(IProject p, int imageIndexOffset)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			this.imageIndexOffset = imageIndexOffset;
			GetCurrentAmbience();

			TreeNode prjNode = new AbstractClassScoutNode(p.Name);
			prjNode.SelectedImageIndex = prjNode.ImageIndex = imageIndexOffset + iconService.GetImageIndexForProjectType(p.ProjectType);

 			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.BuildAction == BuildAction.Compile) {
					int i = 0;
					IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
					if (parserService.GetParser(finfo.Name) == null) {
						continue;
					}

					for(i=0; i < 5 && parserService.GetParseInformation(finfo.Name) == null; i++) {
						Thread.Sleep(100);
					}

					if (parserService.GetParseInformation(finfo.Name) == null) {
						continue;
					}

					IParseInformation parseInformation = parserService.GetParseInformation(finfo.Name);
					if (parseInformation != null) {
						ICompilationUnit unit = parseInformation.BestCompilationUnit as ICompilationUnit;
						if (unit != null) {
						   AddToClassTree(prjNode, finfo.Name, unit);
						}
					}
				}
			}
			return prjNode;
		}
		
		public void AddToClassTree(TreeNode parentNode, string filename, ICompilationUnit unit)
		{
			foreach (IClass c in unit.Classes) {
				TreeNode node = GetNodeByPath(c.Namespace, parentNode.Nodes, true);
				if (node == null) {
					node = parentNode;
				}
				
				TreeNode oldClassNode = GetNodeByName(node.Nodes, c.Name);
				bool wasExpanded = false;
				if (oldClassNode != null) {
					wasExpanded = oldClassNode.IsExpanded;
					oldClassNode.Remove();
				}
				
				TreeNode classNode = BuildClassNode(filename, c);
				if(classNode != null) {
					SortUtility.SortedInsert(classNode, node.Nodes, TreeNodeComparer.Default);
					if (wasExpanded) {
						classNode.Expand();
					}
				}
			}
		}
		
		TreeNode BuildClassNode(string filename, IClass c)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));

			AbstractClassScoutNode classNode = new AbstractClassScoutNode(c.Name);
			classNode.SelectedImageIndex = classNode.ImageIndex = classBrowserIconService.GetIcon(c);
			classNode.ContextmenuAddinTreePath = "/SharpDevelop/Views/ClassScout/ContextMenu/ClassNode";
			classNode.Tag = new ClassScoutTag(c.Region.BeginLine, filename);

			// don't insert delegate 'members'
			if (c.ClassType == ClassType.Delegate) {
				return null;
			}

			foreach (IClass innerClass in c.InnerClasses) {
				if (innerClass.ClassType == ClassType.Delegate) {
					TreeNode innerClassNode = new AbstractClassScoutNode(languageConversion.Convert(innerClass));
					innerClassNode.Tag = new ClassScoutTag(innerClass.Region.BeginLine, filename);
					innerClassNode.SelectedImageIndex = innerClassNode.ImageIndex = classBrowserIconService.GetIcon(innerClass);
					classNode.Nodes.Add(innerClassNode);
				} else {
					TreeNode n = BuildClassNode(filename, innerClass);
					if(classNode != null) {
						classNode.Nodes.Add(n);
					}
				}
			}

			foreach (IMethod method in c.Methods) {
				TreeNode methodNode = new AbstractClassScoutNode(languageConversion.Convert(method));
				methodNode.Tag = new ClassScoutTag(method.Region.BeginLine, filename);
				methodNode.SelectedImageIndex = methodNode.ImageIndex = classBrowserIconService.GetIcon(method);
				classNode.Nodes.Add(methodNode);
			}
			
			foreach (IProperty property in c.Properties) {
				TreeNode propertyNode = new AbstractClassScoutNode(languageConversion.Convert(property));
				propertyNode.Tag = new ClassScoutTag(property.Region.BeginLine, filename);
				propertyNode.SelectedImageIndex = propertyNode.ImageIndex = classBrowserIconService.GetIcon(property);
				classNode.Nodes.Add(propertyNode);
			}
			
			foreach (IField field in c.Fields) {
				TreeNode fieldNode = new AbstractClassScoutNode(languageConversion.Convert(field));
				fieldNode.Tag = new ClassScoutTag(field.Region.BeginLine, filename);
				fieldNode.SelectedImageIndex = fieldNode.ImageIndex = classBrowserIconService.GetIcon(field);
				classNode.Nodes.Add(fieldNode);
			}
			
			foreach (IEvent e in c.Events) {
				TreeNode eventNode = new AbstractClassScoutNode(languageConversion.Convert(e));
				eventNode.Tag = new ClassScoutTag(e.Region.BeginLine, filename);
				eventNode.SelectedImageIndex = eventNode.ImageIndex = classBrowserIconService.GetIcon(e);
				classNode.Nodes.Add(eventNode);
			}
			
			SortUtility.QuickSort(classNode.Nodes, TreeNodeComparer.Default);
			return classNode;
		}
		
		static public TreeNode GetNodeByPath(string directory, TreeNodeCollection root, bool create)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));

			string[] treepath   = directory.Split(new char[] { '.' });
			TreeNodeCollection curcollection = root;
			TreeNode           curnode       = null;
			foreach (string path in treepath) {
				if (path.Length == 0 || path[0] == '.') {
					continue;
				}

				TreeNode node = GetNodeByName(curcollection, path);
				if (node == null) {
					if (create) {
						TreeNode newnode = new AbstractClassScoutNode(path);
						newnode.ImageIndex = newnode.SelectedImageIndex = classBrowserIconService.NamespaceIndex;

						SortUtility.SortedInsert(newnode, curcollection, TreeNodeComparer.Default);
						curnode = newnode;
						curcollection = curnode.Nodes;
						continue;
					} else {
						return null;
					}
				}
				curnode = node;
				curcollection = curnode.Nodes;
			}
			return curnode;
		}

		static TreeNode GetNodeByName(TreeNodeCollection collection, string name)
		{
			foreach (TreeNode node in collection) {
				if (node.Text == name) {
					return node;
				}
			}
			return null;
		}
	}
}
