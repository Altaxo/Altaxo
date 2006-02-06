using System;
using System.Xml;

namespace MathML
{
	/// <summary>
	/// The fn element makes explicit the fact that a more general MathML 
	/// object is intended to be used in the same manner as if it were a 
	/// pre-defined function such as sin or plus.
	/// </summary>
	public class MathMLFnElement : MathMLContentContainer
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLFnElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

		/// <summary>
		 /// accept a visitor.
		 /// return the return value of the visitor's visit method
		 /// </summary>
		public override object Accept(MathMLVisitor v, object args)
		{
			return v.Visit(this, args);
		}
	}
}
