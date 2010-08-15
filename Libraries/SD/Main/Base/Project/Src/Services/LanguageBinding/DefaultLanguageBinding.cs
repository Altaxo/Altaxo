// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	public class DefaultLanguageBinding : ILanguageBinding
	{
		public virtual IFormattingStrategy FormattingStrategy {
			get {
				return null;
			}
		}
		
		public virtual LanguageProperties Properties {
			get {
				return null;
			}
		}
		
		public virtual void Attach(ITextEditor editor)
		{
		}
		
		public virtual void Detach()
		{
		}
		
		public virtual IBracketSearcher BracketSearcher {
			get {
				return null;
			}
		}
	}
}
