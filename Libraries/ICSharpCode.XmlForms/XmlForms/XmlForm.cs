// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.XmlForms {
	
	/// <summary>
	/// The basic xml generated form.
	/// </summary>
	public abstract class XmlForm : Form
	{
		protected XmlLoader xmlLoader;
		
		/// <summary>
		/// Gets the ControlDictionary for this Form.
		/// </summary>
		public ControlDictionary ControlDictionary {
			get {
				return xmlLoader.ControlDictionary;
			}
		}
		
		public XmlForm()
		{
		}
		
		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="fileName">
		/// Name of the xml file which defines this form.
		/// </param>
		public XmlForm(string fileName)
		{
			SetupFromXml(fileName);
		}
		
		protected void SetupFromXml(string fileName)
		{
			SuspendLayout();
			xmlLoader = new XmlLoader();
			SetupXmlLoader();
			if (fileName != null && fileName.Length > 0) {
				xmlLoader.LoadObjectFromFileDefinition(this, fileName);
			}
			ResumeLayout(false);
		}
		
		protected void SetupFromXmlStream(Stream stream)
		{
			SuspendLayout();
			xmlLoader = new XmlLoader();
			SetupXmlLoader();
			if (stream != null) {
				xmlLoader.LoadObjectFromStream(this, stream);
			}
			ResumeLayout(false);
		}
		
		protected virtual void SetupXmlLoader()
		{
		}
		
	}
}
