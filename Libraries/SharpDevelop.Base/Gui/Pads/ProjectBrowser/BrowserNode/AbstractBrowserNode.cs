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

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class reperesents the base class for all nodes in the
	/// project browser.
	/// </summary>
	public abstract class AbstractBrowserNode : TreeNode, IDisposable
	{
		Image  iconImage = null;
		public static bool ShowExtensions = false;
		
		protected bool   canLabelEdited = true;
		protected object userData  = null;
		protected string contextmenuAddinTreePath = String.Empty;
		
//		public new Font NodeFont {
//			get {
//				return base.NodeFont;
//			} 
//			set {
//				base.NodeFont = value;
//			}
//		}
		
		/// <returns>
		/// True, if this node can be label edited, false otherwise.
		/// </returns>
		public bool CanLabelEdited {
			get {
				return canLabelEdited;
			}
		}
		
		/// <summary>
		/// Returns the combine in which this node belongs to. This assumes that
		/// any node is child of a combine.
		/// </summary>
		public virtual Combine Combine {
			get {
				if (Parent == null || !(Parent is AbstractBrowserNode)) {
					return null;
				}
				return ((AbstractBrowserNode)Parent).Combine;
			}
		}
		
		/// <summary>
		/// Returns the project in which this node belongs to. This assumes that
		/// any node is child of a project. THIS DON'T WORK ON COMBINE NODES!
		/// (a combine node returns null)
		/// </summary>
		public virtual IProject Project {
			get {
				if (Parent == null || !(Parent is AbstractBrowserNode)) {
					return null;
				}
				return ((AbstractBrowserNode)Parent).Project;
			}
		}		
		
		/// <summary>
		/// Holds generic user data for this node.
		/// </summary>
		public object UserData {
			get {
				return userData;
			}
			set {
				userData = value;
			}
		}
		

#region System.IDisposable interface implementation
		public virtual void Dispose()
		{
			
		}
#endregion

		/// <summary>
		/// This property gets/sets the current image of this tree node.
		/// </summary>
		public Image IconImage {
			get {
				return iconImage;
			}
			set {
				iconImage = value;
				ImageIndex = SelectedImageIndex = ProjectBrowserView.GetImageIndexForImage(iconImage);
			}
		}
				
		/// <summary>
		/// Generates a Drag & Drop data object. If this property returns null
		/// the node indicates that it can't be dragged.
		/// </summary>
		public virtual DataObject DragDropDataObject {
			get {
				return null;
			}
		}
		
		/// <summary>
		/// Gets the drag & drop effect, when a DataObject is dragged over this node.
		/// </summary>
		/// <param name="proposedEffect">
		/// The default effect DragDropEffects.Copy and DragDropEffects.Move, depending on the 
		/// key the user presses while performing d&d.
		/// </param>
		/// <returns>
		/// DragDropEffects.None when no drag&drop can occur.
		/// </returns>
		public virtual DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			return DragDropEffects.None;
		}
		
		/// <summary>
		/// If GetDragDropEffect returns something != DragDropEffects.None this method should
		/// handle the DoDragDrop(obj, GetDragDropEffect(obj, proposedEffect)).
		/// </summary>
		public virtual void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			throw new System.NotImplementedException();
		}
		
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
		
		public AbstractBrowserNode()
		{
			NodeFont = ProjectBrowserView.PlainFont;
	}
		
		/// <summary>
		/// This method is called on the 'open' event.
		/// </summary>
		public virtual void ActivateItem()
		{
		}
		
		/// <summary>
		/// This method is before a label edit starts.
		/// </summary>
		public virtual void BeforeLabelEdit()
		{
		}
		
		/// <summary>
		/// This method is called when a remove is selected on this
		/// node. The node should remove the object the node represents
		/// (e.g. a file, folder, project from a combine etc.) the node
		/// will be removed from the project tree, when it returns true
		/// on this function.
		/// </summary>
		/// <returns>
		/// true, if the remove of the object was successfull, false
		/// otherwise
		/// </returns>
		public virtual bool RemoveNode()
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// This method is called when a label edit has finished.
		/// The New Name is the 'new name' the user has given the node. The
		/// node must handle the name change itself.
		/// </summary>
		public virtual void AfterLabelEdit(string newName)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// This method is called before the node expandes. (e.g. for folders like nodes)
		/// </summary>
		public virtual void BeforeExpand()
		{
		}
		
		/// <summary>
		/// This method is called before the node collapses. (e.g. for folders like nodes)
		/// </summary>
		public virtual void BeforeCollapse()
		{
		}
		
		/// <summary>
		/// This method is called when a redraw event from the projectbrowserview is called
		/// (maybe the language has changed).
		/// </summary>
		public virtual void UpdateNaming()
		{
			foreach (AbstractBrowserNode childNode in Nodes) {
				childNode.UpdateNaming();
			}
		}
		
	}
}
