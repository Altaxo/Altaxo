﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractSecondaryViewContent : AbstractBaseViewContent, ISecondaryViewContent
	{
		public virtual void NotifyBeforeSave()
		{
		}
		
		public virtual void NotifyAfterSave(bool successful)
		{
		}
		
		public virtual void NotifyFileNameChanged()
		{
		}
	}
}
