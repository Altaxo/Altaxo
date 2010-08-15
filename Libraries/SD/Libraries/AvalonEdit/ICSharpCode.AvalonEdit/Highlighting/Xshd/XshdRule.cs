// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 4626 $</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd
{
	/// <summary>
	/// &lt;Rule&gt; element.
	/// </summary>
	[Serializable]
	public class XshdRule : XshdElement
	{
		/// <summary>
		/// Gets/sets the rule regex.
		/// </summary>
		public string Regex { get; set; }
		
		/// <summary>
		/// Gets/sets the rule regex type.
		/// </summary>
		public XshdRegexType RegexType { get; set; }
		
		/// <summary>
		/// Gets/sets the color reference.
		/// </summary>
		public XshdReference<XshdColor> ColorReference { get; set; }
		
		/// <inheritdoc/>
		public override object AcceptVisitor(IXshdVisitor visitor)
		{
			return visitor.VisitRule(this);
		}
	}
}
