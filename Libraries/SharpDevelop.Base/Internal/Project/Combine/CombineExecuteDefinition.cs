// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public enum EntryExecuteType {
		None,
		Execute
	}
	
	public class CombineExecuteDefinition
	{
		public CombineEntry     Entry = null;
		public EntryExecuteType Type  = EntryExecuteType.None;
		
		public CombineExecuteDefinition()
		{
		}
		
		public CombineExecuteDefinition(CombineEntry entry, EntryExecuteType type)
		{
			this.Entry = entry;
			this.Type  = type;
		}
	}
}
