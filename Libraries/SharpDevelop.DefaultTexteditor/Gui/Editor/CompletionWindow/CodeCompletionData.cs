// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃƒÂ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using SharpDevelop.Internal.Parser;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	class CodeCompletionData : ICompletionData
	{
		static ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		static IParserService           parserService           = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
		static AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		
		int      imageIndex;
		int      overloads;
		string   text;
		string   description;
		string   documentation;
		string   completionString;
		IClass   c;
		bool     convertedDocumentation = false;
		
		public int Overloads {
			get {
				return overloads;
			}
			set {
				overloads = value;
			}
		}
		
		public int ImageIndex {
			get {
				return imageIndex;
			}
			set {
				imageIndex = value;
			}
		}
		
		public string[] Text {
			get {
				return new string[] { text };
			}
			set {
				text = value[0];
			}
		}
		public string Description {
			get {
				// get correct delegate description (when description is requested)
				// in the classproxies aren't methods saved, therefore delegate methods
				// must be get through the real class instead out of the proxy
				//
				// Mike
				if (c is ClassProxy && c.ClassType == ClassType.Delegate) {
					description = ambienceService.CurrentAmbience.Convert(parserService.GetClass(c.FullyQualifiedName));
					c = null;
				}
				
				// don't give a description string, if no documentation or description is provided
				if (description.Length + documentation.Length == 0) {
					return null;
				}
				if (!convertedDocumentation) {
					convertedDocumentation = true;
					try {
						documentation = GetDocumentation(documentation);
						// new (by G.B.)
						// XmlDocument doc = new XmlDocument();
						// doc.LoadXml("<doc>" + documentation + "</doc>");
						// XmlNode root      = doc.DocumentElement;
						// XmlNode paramDocu = root.SelectSingleNode("summary");
						// documentation = paramDocu.InnerXml;
					} catch (Exception e) {
						Console.WriteLine(e.ToString());
					}
				}
				return description + (overloads > 0 ? " (+" + overloads + " overloads)" : String.Empty) + "\n" + documentation;
			}
			set {
				description = value;
			}
		}
		
		public CodeCompletionData(string s, int imageIndex)
		{
			description = documentation = String.Empty;
			text = s;
			completionString = s;
			this.imageIndex = imageIndex;
		}
		
		public CodeCompletionData(IClass c)
		{
			// save class (for the delegate description shortcut
			this.c = c;
			imageIndex = classBrowserIconService.GetIcon(c);
			text = c.Name;
			completionString = c.Name;
			description = ambienceService.CurrentAmbience.Convert(c);
			documentation = c.Documentation;
		}
		
		public CodeCompletionData(IMethod method)
		{
			imageIndex  = classBrowserIconService.GetIcon(method);
			text        = method.Name;
			description = ambienceService.CurrentAmbience.Convert(method);
			completionString = method.Name;
			documentation = method.Documentation;
		}
		
		public CodeCompletionData(IField field)
		{
			imageIndex  = classBrowserIconService.GetIcon(field);
			text        = field.Name;
			description = ambienceService.CurrentAmbience.Convert(field);
			completionString = field.Name;
			documentation = field.Documentation;
		}
		
		public CodeCompletionData(IProperty property)
		{
			imageIndex  = classBrowserIconService.GetIcon(property);
			text        = property.Name;
			description = ambienceService.CurrentAmbience.Convert(property);
			completionString = property.Name;
			documentation = property.Documentation;
		}
		
		public CodeCompletionData(IEvent e)
		{
			imageIndex  = classBrowserIconService.GetIcon(e);
			text        = e.Name;
			description = ambienceService.CurrentAmbience.Convert(e);
			completionString = e.Name;
			documentation = e.Documentation;
		}
		
		public void InsertAction(TextEditorControl control)
		{
			((SharpDevelopTextAreaControl)control).ActiveTextAreaControl.TextArea.InsertString(completionString);
		}

		public static string GetDocumentation(string doc)
		{
			System.IO.StringReader reader = new System.IO.StringReader("<docroot>" + doc + "</docroot>");
			XmlTextReader xml   = new XmlTextReader(reader);
			StringBuilder ret   = new StringBuilder();
			Regex whitespace    = new Regex(@"\s+");
			
			try {
				xml.Read();
				do {
					if (xml.NodeType == XmlNodeType.Element) {
						string elname = xml.Name.ToLower();
						if (elname == "remarks") {
							ret.Append("Remarks:\n");
						} else if (elname == "example") {
							ret.Append("Example:\n");
						} else if (elname == "exception") {
							ret.Append("Exception: " + GetCref(xml["cref"]) + ":\n");
						} else if (elname == "returns") {
							ret.Append("Returns: ");
						} else if (elname == "see") {
							ret.Append(GetCref(xml["cref"]) + xml["langword"]);
						} else if (elname == "seealso") {
							ret.Append("See also: " + GetCref(xml["cref"]) + xml["langword"]);
						} else if (elname == "paramref") {
							ret.Append(xml["name"]);
						} else if (elname == "param") {
							ret.Append(xml["name"].Trim() + ": ");
						} else if (elname == "value") {
							ret.Append("Value: ");
						}
					} else if (xml.NodeType == XmlNodeType.EndElement) {
						string elname = xml.Name.ToLower();
						if (elname == "para" || elname == "param") {
							ret.Append("\n");
						}
					} else if (xml.NodeType == XmlNodeType.Text) {
						ret.Append(whitespace.Replace(xml.Value, " "));
					}
				} while(xml.Read());
			} catch {
				return doc;
			}
			return ret.ToString();
		}
		
		static string GetCref(string cref)
		{
			if (cref == null) return "";
			if (cref.Length < 2) return cref;
			if (cref.Substring(1, 1) == ":") return cref.Substring(2, cref.Length - 2);
			return cref;
		}
	
	}
}
