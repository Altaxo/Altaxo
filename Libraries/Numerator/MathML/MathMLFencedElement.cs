using System;
using System.Xml;

namespace MathML
{
	/// <summary>
	/// This interface extends the MathMLPresentationContainer interface for the 
	/// MathML fenced content element mfenced.
	/// </summary>
	public class MathMLFencedElement : MathMLPresentationContainer
	{
		/// <summary>
		/// creates a new MathMLOperatorElement. 
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any).</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any).</param>
		/// <param name="doc">The owner document</param>
		public MathMLFencedElement(string prefix, string localName, string namespaceURI, MathMLDocument doc)
			: base(prefix, localName, namespaceURI, doc)
		{
		}

        /// <summary>
        /// A string representing the opening-fence for the mfenced element, if specified; 
        /// this is the element’s open attribute.
        /// </summary>
		public string Open
		{
			get { return HasAttribute("open") ? GetAttribute("open") : "("; }
			set { SetAttribute("open", value); }
		}

		/// <summary>
		/// A string representing the opening-fence for the mfenced element, if specified; 
		/// this is the element’s close attribute.
		/// </summary>
		public string Close
		{
			get { return HasAttribute("close") ? GetAttribute("close") : ")"; }
			set { SetAttribute("close", value); }
		}

		/// <summary>
		/// A string representing any separating characters inside the mfenced element, if specified; 
		/// this is the element’s separators attribute. The value of separators is a sequence of zero 
		/// or more separator characters (or entity references), optionally separated by whitespace. 
		/// Each sep#i consists of exactly one character or entity reference. Thus, separators=",;" 
		/// is equivalent to separators=" , ; ".
		/// </summary>
		public string Separators
		{
			get 
			{ 
				string s = GetAttribute("separators"); 
				return s.Length > 0 ? s : ",";
			}
			set { SetAttribute("separators", value); }
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
