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
using System.Collections.Specialized;
using System.Collections.Utility;

using ICSharpCode.Core.Properties;
using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// This class reperesents the base class for all nodes in the
	/// class browser.
	/// </summary>
	public class AbstractClassScoutNode : TreeNode
	{
		protected string contextmenuAddinTreePath = String.Empty;
		static AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		static ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		
		/// <summary>
		/// Gets the add-in tree path for the context menu.
		/// </summary>
		/// <remarks>
		/// I choosed to give back the add-in tree path instead of a popup menu
		/// or a menuitem collection, because I don't want to add a magic library
		/// or Windows.Forms dependency.
		/// </remarks>
		public virtual string ContextmenuAddinTreePath {
			get {
				return contextmenuAddinTreePath;
			}
			set {
				contextmenuAddinTreePath = value;
			}
		}
		
		public AbstractClassScoutNode(string name) : base(name)
		{
		}
		
		public void BeforeExpand()
		{
			BuildClassNode(this);
		}
		
		void BuildClassNode(TreeNode node)
		{
			
			IAmbience languageConversion= ambienceService.CurrentAmbience;
			languageConversion.ConversionFlags = ConversionFlags.None;
			
			ClassScoutTag classScoutTag = node.Tag as ClassScoutTag;
			if (classScoutTag == null) {
				return;
			}
			IClass c = classScoutTag.Tag as IClass;
			if (c == null) {
				return;
			}
			node.Nodes.Clear();
			
			
			AbstractClassScoutNode classNode = (AbstractClassScoutNode)node;
			
			// don't insert delegate 'members'
			if (c.ClassType == ClassType.Delegate) {
				return;
			}
			
			foreach (IClass innerClass in c.InnerClasses) {
				AbstractClassScoutNode newClassNode = new AbstractClassScoutNode(innerClass.Name);
				newClassNode.SelectedImageIndex = newClassNode.ImageIndex = classBrowserIconService.GetIcon(innerClass);
				newClassNode.ContextmenuAddinTreePath = "/SharpDevelop/Views/ClassScout/ContextMenu/ClassNode";
				newClassNode.Tag = new ClassScoutTag(innerClass.Region.BeginLine, innerClass.CompilationUnit.FileName, innerClass);
				classNode.Nodes.Add(newClassNode);
				BuildClassNode(newClassNode);
			}
			
			foreach (IMethod method in c.Methods) {
				TreeNode methodNode = new AbstractClassScoutNode(languageConversion.Convert(method));
				if (method.Region != null)
					methodNode.Tag = new ClassScoutTag(method.Region.BeginLine, classScoutTag.FileName);
				methodNode.SelectedImageIndex = methodNode.ImageIndex = classBrowserIconService.GetIcon(method);
				classNode.Nodes.Add(methodNode);
			}
			
			foreach (IProperty property in c.Properties) {
				TreeNode propertyNode = new AbstractClassScoutNode(languageConversion.Convert(property));
				if (property.Region != null)
					propertyNode.Tag = new ClassScoutTag(property.Region.BeginLine, classScoutTag.FileName);
				propertyNode.SelectedImageIndex = propertyNode.ImageIndex = classBrowserIconService.GetIcon(property);
				classNode.Nodes.Add(propertyNode);
			}
			
			foreach (IIndexer indexer in c.Indexer) {
				TreeNode indexerNode = new AbstractClassScoutNode(languageConversion.Convert(indexer));
				if (indexer.Region != null)
					indexerNode.Tag = new ClassScoutTag(indexer.Region.BeginLine, classScoutTag.FileName);
				indexerNode.SelectedImageIndex = indexerNode.ImageIndex = classBrowserIconService.GetIcon(indexer);
				classNode.Nodes.Add(indexerNode);
			}
			
			foreach (IField field in c.Fields) {
				TreeNode fieldNode = new AbstractClassScoutNode(languageConversion.Convert(field));
				if (field.Region != null)
					fieldNode.Tag = new ClassScoutTag(field.Region.BeginLine, classScoutTag.FileName);
				fieldNode.SelectedImageIndex = fieldNode.ImageIndex = classBrowserIconService.GetIcon(field);
				classNode.Nodes.Add(fieldNode);
			}
			
			foreach (IEvent e in c.Events) {
				TreeNode eventNode = new AbstractClassScoutNode(languageConversion.Convert(e));
				if (e.Region != null)
					eventNode.Tag = new ClassScoutTag(e.Region.BeginLine, classScoutTag.FileName);
				eventNode.SelectedImageIndex = eventNode.ImageIndex = classBrowserIconService.GetIcon(e);
				classNode.Nodes.Add(eventNode);
			}
			
			SortUtility.QuickSort(classNode.Nodes, TreeNodeComparer.Default);
		}
		
	}
}
