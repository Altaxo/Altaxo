// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractViewContent : AbstractBaseViewContent, IViewContent
	{
		string untitledName = String.Empty;
		string titleName    = null;
		string fileName     = null;
		
		bool   isDirty  = false;
		bool   isViewOnly = false;
		
		public virtual string UntitledName {
			get {
				return untitledName;
			}
			set {
				untitledName = value;
			}
		}
		
		public virtual string TitleName {
			get {
				return IsUntitled ? untitledName : titleName;
			}
			set {
				titleName = value;
				OnTitleNameChanged(EventArgs.Empty);
			}
		}
		
		public virtual string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
				OnFileNameChanged(EventArgs.Empty);
			}
		}
		
		public AbstractViewContent()
		{
		}
		
		public AbstractViewContent(string titleName)
		{
			this.titleName = titleName;
		}
		
		public AbstractViewContent(string titleName, string fileName)
		{
			this.titleName = titleName;
			this.fileName  = fileName;
		}
		
		public bool IsUntitled {
			get {
				return titleName == null;
			}
		}
		
		public virtual bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
				OnDirtyChanged(EventArgs.Empty);
			}
		}
		
		public virtual bool IsReadOnly {
			get {
				return false;
			}
		}		
		
		public virtual bool IsViewOnly {
			get {
				return isViewOnly;
			}
			set {
				isViewOnly = value;
			}
		}
		
		public virtual void Save()
		{
			if (IsDirty) {
				Save(fileName);
			}
		}
		
		public virtual void Save(string fileName)
		{
			throw new System.NotImplementedException();
		}
		
		public abstract void Load(string fileName);
		
		protected virtual void OnDirtyChanged(EventArgs e)
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, e);
			}
		}
		
		protected virtual void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
			}
		}
		
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			if (FileNameChanged != null) {
				FileNameChanged(this, e);
			}
		}
		
		
		protected virtual void OnBeforeSave(EventArgs e)
		{
			if (BeforeSave != null) {
				BeforeSave(this, e);
			}
		}
		
		public event EventHandler TitleNameChanged;
		public event EventHandler FileNameChanged;
		public event EventHandler DirtyChanged;
		public event EventHandler BeforeSave;
	}
}
