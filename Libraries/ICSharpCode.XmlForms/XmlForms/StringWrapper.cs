// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.GUI.Xml {
	
	/// <summary>
	/// This class wrapps a string to an object, the string is accessible
	/// through the 'Text' property. This class was written for setting the
	/// items in a combobox inside a xml definition.
	/// </summary>
	public class StringWrapper 
	{
		string text;
		
		/// <summary>
		/// Get/Set the string.
		/// </summary>
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		/// <summary>
		/// returns <code>Text</code>
		/// </summary>
		public override string ToString()
		{
			return text;
		}
	}
}
