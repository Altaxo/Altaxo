using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Diagnostics;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class StandardHeader
	{
		static string version = "1.0";
		static string TemplateFileName = "StandardHeader.xml";
		static ArrayList standardHeaders = new ArrayList();
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public static ArrayList StandardHeaders {
			get {
				return standardHeaders;
			}
		}
		
		static bool LoadHeaders(string fileName)
		{
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
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			foreach (StandardHeader standardHeader in standardHeaders) {
				XmlElement newElement = doc.CreateElement("Property");
				newElement.SetAttribute("name", standardHeader.Name);
				newElement.InnerText = standardHeader.Header;
				doc.DocumentElement.AppendChild(newElement);
			}
			doc.Save(Path.Combine(propertyService.ConfigDirectory, TemplateFileName));
			SetHeaders();
		}
		
		public static void SetHeaders()
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			foreach (StandardHeader standardHeader in standardHeaders) {
				stringParserService.Properties[standardHeader.Name] = standardHeader.Header;
			}
		}
		
		static StandardHeader()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (!LoadHeaders(Path.Combine(propertyService.ConfigDirectory, TemplateFileName))) {
				if (!LoadHeaders(Path.Combine(propertyService.DataDirectory + Path.DirectorySeparatorChar + "options", TemplateFileName))) {
					IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowWarning("Can not load standard headers");
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
