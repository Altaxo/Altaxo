﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void NodeInitializer(CustomNode node);
	
	public class CustomNode : AbstractProjectBrowserTreeNode
	{
		NodeInitializer nodeInitializer = null;
		
		public NodeInitializer NodeInitializer {
			get {
				return nodeInitializer;
			}
			set {
				nodeInitializer = value;
			}
		}
		
		public CustomNode()
		{
		}
		
		protected override void Initialize()
		{
			if (nodeInitializer != null) {
				nodeInitializer(this);
			}
			base.Initialize();
		}
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
	}
}
