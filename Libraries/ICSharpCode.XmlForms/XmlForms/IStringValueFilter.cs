// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.XmlForms {
	
	/// <summary>
	/// This interface is used to filter the values defined in the xml files.
	/// It could be used for the localization of control texts.
	/// </summary>
	public interface IStringValueFilter
	{
		/// <summary>
		/// Is called for each value string in the definition xml file.
		/// </summary>
		/// <returns>
		/// The filtered text value
		/// </returns>
		string GetFilteredValue(string originalValue);
	}
}
