// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Scott Ferrett" email="surf@softvelocity.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	// Summary:
	// Represents a reader that reads MSBuild files converting the MSBuild specific escape %25 to %
	public class MSBuildFileReader : XmlTextReader
	{
		private string Convert(string instr)
		{
			if (instr != null && instr.Contains("%25")) {
				return instr.Replace("%25", "%");
			}
			return instr;
		}
		public MSBuildFileReader(string fileName) : base(fileName) { }
		public MSBuildFileReader(TextReader r) : base(r) { }
		//
		// Summary:
		// Gets the value of the attribute with the specified index.
		// 
		// Parameters:
		// i: 
		//    The index of the attribute. The index is zero-based. (The first attribute
		//    has index 0.) 
		// 
		// Returns:
		// The value of the specified attribute.
		// 
		// Exceptions:
		//    System.ArgumentOutOfRangeException
		// 
		public override string GetAttribute(int i)
		{
			return Convert(base.GetAttribute(i));
		}
		//
		// Summary:
		// Gets the value of the attribute with the specified name.
		// 
		// Parameters:
		// name: 
		//    The qualified name of the attribute. 
		// 
		// Returns:
		// The value of the specified attribute. If the attribute is not found, null is
		// returned.
		public override string GetAttribute(string name)
		{
			return Convert(base.GetAttribute(name));
		}
		//
		// Summary:
		// Gets the value of the attribute with the specified local name and namespace
		// URI.
		// 
		// Parameters:
		// localName: 
		//    The local name of the attribute. 
		// namespaceURI: 
		//    The namespace URI of the attribute. 
		// 
		// Returns:
		// The value of the specified attribute. If the attribute is not found, null is
		// returned. This method does not move the reader.
		public override string GetAttribute(string localName, string namespaceURI)
		{
			return Convert(base.GetAttribute(localName, namespaceURI));
		}
	}
	// Summary:
	// Represents a writer that writes MSBuild files converting the MSBuild specific escape % to %25
	public class MSBuildFileWriter : XmlTextWriter
	{
		private string Convert(string instr)
		{
			if (instr != null && instr.Contains("%")) {
				return instr.Replace("%", "%25");
			}
			return instr;
		}
		public MSBuildFileWriter(string fileName, Encoding encoding) : base(fileName, encoding) { }
		
		public override void WriteString(string text)
		{
			base.WriteString(Convert(text));
		}
	}
}
