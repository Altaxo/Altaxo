// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 4142 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.AvalonEdit.Rendering
{
	struct HeightTreeLineNode
	{
		internal HeightTreeLineNode(double height)
		{
			this.collapsedSections = null;
			this.height = height;
		}
		
		internal double height;
		internal List<CollapsedLineSection> collapsedSections;
		
		internal bool IsDirectlyCollapsed {
			get { return collapsedSections != null; }
		}
		
		internal void AddDirectlyCollapsed(CollapsedLineSection section)
		{
			if (collapsedSections == null)
				collapsedSections = new List<CollapsedLineSection>();
			collapsedSections.Add(section);
		}
		
		internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
		{
			Debug.Assert(collapsedSections.Contains(section));
			collapsedSections.Remove(section);
			if (collapsedSections.Count == 0)
				collapsedSections = null;
		}
		
		/// <summary>
		/// Returns 0 if the line is directly collapsed, otherwise, returns <see cref="height"/>.
		/// </summary>
		internal double TotalHeight {
			get {
				return IsDirectlyCollapsed ? 0 : height;
			}
		}
	}
}
