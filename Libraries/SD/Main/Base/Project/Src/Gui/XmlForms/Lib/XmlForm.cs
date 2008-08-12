﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	/// <summary>
	/// The basic xml generated form.
	/// </summary>
	public abstract class XmlForm : Form
	{
		protected XmlLoader xmlLoader;
		
		/// <summary>
		/// Gets the ControlDictionary for this Form.
		/// </summary>
		public Dictionary<string, Control> ControlDictionary {
			get {
				return xmlLoader.ControlDictionary;
			}
		}
		
		public XmlForm()
		{
		}
		
//		/// <summary>
//		/// Creates a new instance
//		/// </summary>
//		/// <param name="fileName">
//		/// Name of the xml file which defines this form.
//		/// </param>
//		public XmlForm(string fileName)
//		{
//			SetupFromXml(fileName);
//		}
		
		
		public T Get<T>(string name) where T: System.Windows.Forms.Control
		{
			return xmlLoader.Get<T>(name);
		}
//
//		protected void SetupFromXml(string fileName)
//		{
//			if (fileName == null) {
//				throw new System.ArgumentNullException("fileName");
//			}
//
//			using (Stream stream = File.OpenRead(fileName)) {
//				SetupFromXmlStream(stream);
//			}
//		}
		
		protected void SetupFromXmlResource(string resourceName)
		{
			Assembly caller = Assembly.GetCallingAssembly();
			resourceName = "Resources." + resourceName;
			SetupFromXmlStream(caller.GetManifestResourceStream(resourceName));
		}
		
		protected void SetupFromXmlStream(Stream stream)
		{
			if (stream == null) {
				throw new System.ArgumentNullException("stream");
			}
			SuspendLayout();
			xmlLoader = new XmlLoader();
			SetupXmlLoader();
			if (stream != null) {
				xmlLoader.LoadObjectFromStream(this, stream);
			}
			RightToLeftConverter.ConvertRecursive(this);
			ResumeLayout(false);
		}
		
		protected virtual void SetupXmlLoader()
		{
		}
		
	}
}
