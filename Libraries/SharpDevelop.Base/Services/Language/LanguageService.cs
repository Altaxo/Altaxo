// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	public class LanguageService : AbstractService
	{
		string languagePath;
		
		ImageList languageImageList = null;
		ArrayList languages         = null;
		
		public ImageList LanguageImageList {
			get {
				if (languageImageList == null) {
					LoadLanguageDefinitions();
				}
				return languageImageList;
			}
		}
		
		public ArrayList Languages {
			get {
				if (languages == null) {
					LoadLanguageDefinitions();
				}
				return languages;
			}
		}
		void LoadLanguageDefinitions()
		{
			languageImageList = new ImageList();
			languageImageList.ColorDepth = ColorDepth.Depth32Bit;
			languages         = new ArrayList();
			LanguageImageList.ImageSize = new Size(46, 38);
			
			XmlDocument doc = new XmlDocument();
			doc.Load(languagePath + "LanguageDefinition.xml");
			
			XmlNodeList nodes = doc.DocumentElement.ChildNodes;
			
			foreach (XmlNode node in nodes) {
				XmlElement el = node as XmlElement;
				if (el != null) {
					languages.Add(new Language(el.Attributes["name"].InnerText,
					                           el.Attributes["code"].InnerText,
					                           LanguageImageList.Images.Count));
					LanguageImageList.Images.Add(new Bitmap(languagePath + el.Attributes["icon"].InnerText));
				}
			}
		}
		
		public LanguageService()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			languagePath =  propertyService.DataDirectory +
			                Path.DirectorySeparatorChar + "resources" +
		                    Path.DirectorySeparatorChar + "languages" +
		                    Path.DirectorySeparatorChar;
		}
	}
}
