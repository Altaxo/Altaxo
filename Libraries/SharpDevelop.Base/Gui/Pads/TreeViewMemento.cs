// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Xml;
using System.Resources;


using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class TreeViewMemento : IXmlConvertable
	{
		TreeView treeView = null;
		XmlElement parent = null;
		
		public TreeViewMemento()
		{
		}
		
		public TreeViewMemento(TreeView treeView)
		{
			this.treeView = treeView;
		}
		
		void SaveTree(TreeNodeCollection nodes, XmlDocument doc, XmlElement el)
		{
			foreach (TreeNode node in nodes) {
				if (node.IsExpanded) {
					XmlElement child = doc.CreateElement("Node");
					
					XmlAttribute attr = doc.CreateAttribute("name");
					attr.InnerText = node.Text;
					child.Attributes.Append(attr);

					el.AppendChild(child);
					SaveTree(node.Nodes, doc, child);
				}
			}
		}
		
		void RestoreTree(TreeNodeCollection nodes, XmlElement parent)
		{
			XmlNodeList nodelist = parent.ChildNodes;
			foreach (XmlElement el in nodelist) {
				foreach (TreeNode node in nodes) {
					if (node.Text == el.Attributes["name"].InnerText) {
						node.Expand();
						RestoreTree(node.Nodes, el);
						break;
					}
				}
			}
		}
		
		public void Restore(TreeView view)
		{
			view.BeginUpdate();
			RestoreTree(view.Nodes, (XmlElement)parent);
			view.EndUpdate();
		}
		
		public object FromXmlElement(XmlElement element)
		{
			this.parent = element;
			return this;
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			Debug.Assert(treeView != null);
			
			XmlElement treenode  = doc.CreateElement("TreeView");
			SaveTree(treeView.Nodes, doc, treenode);
			return treenode;
		}
	}
}
