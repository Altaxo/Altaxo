﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 5475 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	public static class LanguageService
	{
		static string languagePath = Path.Combine(PropertyService.DataDirectory, "resources", "languages");
		
		static List<Language> languages = null;
		
		public static List<Language> Languages {
			get {
				return languages;
			}
		}
		
		static LanguageService()
		{
			languages = new List<Language>();
			
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.Combine(languagePath, "LanguageDefinition.xml"));
			
			XmlNodeList nodes = doc.DocumentElement.ChildNodes;
			
			foreach (XmlNode node in nodes) {
				XmlElement el = node as XmlElement;
				if (el != null) {
					languages.Add(new Language(
						el.Attributes["name"].InnerText,
						el.Attributes["code"].InnerText,
						Path.Combine(languagePath, el.Attributes["icon"].InnerText)
					));
				}
			}
		}
	}
}
