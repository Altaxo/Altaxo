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
	/// A custom dictionary, for storing controls
	/// </summary>
	public class ControlDictionary
	{
		Hashtable baseHashtable = new Hashtable();
		
		public Control this[object key] {
			get {
				return (Control)baseHashtable[key];
			}
			set {
				baseHashtable[key] = value;
			}
		}
	}
}
