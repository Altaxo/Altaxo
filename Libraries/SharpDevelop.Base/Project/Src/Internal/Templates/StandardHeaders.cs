// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class StandardHeader
	{
		static string version = "1.0";
		static string TemplateFileName = "StandardHeader.xml";
		static ArrayList standardHeaders = new ArrayList();
		
		
		public static ArrayList StandardHeaders {
			get {
				return standardHeaders;
			}
		}
		
		static bool LoadHeaders(string fileName)
		{
			if (!File.Exists(fileName)) {
				return false;
			}
			
			XmlDocument doc = new XmlDocument();
			try {
				doc.Load(fileName);
				
				if (doc.DocumentElement.GetAttribute("version") != version) {
					return false;
				}
				
				foreach (XmlElement el in doc.DocumentElement.ChildNodes) {
					standardHeaders.Add(new StandardHeader(el));
				}
			} catch (Exception) {
				return false;
			}
			return true;
		}
		
		public static void StoreHeaders()
		{
			XmlDocument doc    = new XmlDocument();
			doc.LoadXml("<StandardProperties version = \"" + version + "\" />");
			
			foreach (StandardHeader standardHeader in standardHeaders) {
				XmlElement newElement = doc.CreateElement("Property");
				newElement.SetAttribute("name", standardHeader.Name);
				newElement.InnerText = standardHeader.Header;
				doc.DocumentElement.AppendChild(newElement);
			}
			doc.Save(Path.Combine(PropertyService.ConfigDirectory, TemplateFileName));
			SetHeaders();
		}
		
		public static void SetHeaders()
		{
			
			foreach (StandardHeader standardHeader in standardHeaders) {
				StringParser.Properties[standardHeader.Name] = standardHeader.Header;
			}
		}
		
		static StandardHeader()
		{
			
			if (!LoadHeaders(Path.Combine(PropertyService.ConfigDirectory, TemplateFileName))) {
				if (!LoadHeaders(FileUtility.Combine(PropertyService.DataDirectory,  "options", TemplateFileName))) {
					MessageService.ShowWarning("Can not load standard headers");
				}
			}
		}
		
		#region StandardHeader 
		string name;
		string header;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string Header {
			get {
				return header;
			}
			set {
				header = value;
			}
		}
		public StandardHeader(XmlElement el)
		{
			this.name   = el.GetAttribute("name");
			this.header = el.InnerText;
		}
		public override string ToString()
		{
			return Name.Substring("StandardHeader.".Length);
		}
		
		#endregion
	}
}
