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
	/// <summary>
	/// </summary>
	public abstract class AbstractSecondaryViewContent : AbstractBaseViewContent, ISecondaryViewContent
	{
		public virtual void Selected()
		{
		}
		
		public virtual void Deselected()
		{
		}
		
		public virtual void NotifyBeforeSave()
		{
		}
	}
}
