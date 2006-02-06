using System;

namespace MathML
{
	/// <summary>
	/// A way to describe a 'placeholder' when editing math ml. This class is NOT part of
	/// the W3C published mathml specification, and this class will not be persisted to or
	/// from a mathml document. This class is only to be programatically created by editing 
	/// applications.
	/// </summary>
	public class MathMLPlaceholderElement : MathMLPresentationToken
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLPlaceholderElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		/// Accept a visitor. Calls the Visitor's MathMLPlaceHolder Vist method
		/// </summary>
		public override object Accept(MathMLVisitor v, object args)
		{
			return v.Visit(this, args);
		}
	}
}
