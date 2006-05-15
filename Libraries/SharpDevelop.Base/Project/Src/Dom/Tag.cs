// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version>$Revision: 966 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class Tag //Comment
	{
		string key;
		
		public string Key {
			get {
				return key;
			}
		}
		
		string commentString;
		DomRegion region;
		
		public string CommentString {
			get {
				return commentString;
			}
			set {
				commentString = value;
			}
		}
		
		public DomRegion Region {
			get {
				return region;
			}
			set {
				region = value;
			}
		}
		
		public Tag(string key, DomRegion region)
		{
			this.key = key;
			this.region = region;
		}
	}
}
