/*
// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.IO;

using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	public class RingBuffer : IXmlConvertable
	{
		int maxLength     = 10;
		ArrayList entries = new ArrayList();
		
		public ArrayList Entries {
			get {
				return entries;
			}
		}
		
		public RingBuffer(int maxLength)
		{
			this.maxLength = maxLength;
		}
		
		protected RingBuffer(XmlElement element)
		{
			maxLength = Int32.Parse(element.Attributes["maxlength"].InnerText);
			
			foreach (XmlNode node in element.ChildNodes) {
				DictionaryEntry newEntry = new DictionaryEntry(node.Attributes["key"].InnerText, node.Attributes["value"].InnerText);
				if (!FilterEntry(newEntry)) {
					entries.Add(newEntry);
				}
			}
		}
		
		protected virtual bool FilterEntry(DictionaryEntry entry)
		{
			return false;
		}
		
		public void AddEntry(string key, string val)
		{
			for (int i = 0; i < entries.Count; ++i) {
				DictionaryEntry entry = (DictionaryEntry)entries[i];
				if (entry.Key.ToString() == key) {
					entries.RemoveAt(i);
					--i;
				}
			}
			
			while (entries.Count >= maxLength) {
				entries.RemoveAt(entries.Count - 1);
			}
			
			if (entries.Count > 0) {
				entries.Insert(0, name);
			} else {
				entries.Add(name);
			}
			
			OnChanged(EventArgs.Empty);
		}
		
		public object FromXmlElement(XmlElement element)
		{
			return new RingBuffer(element);
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement entries = doc.CreateElement("Entries");
			
			XmlAttribute lengthAttribute = doc.CreateAttribute("maxlength");
			lengthAttribute.InnerText = maxLength.ToString();
			entries.Attributes.Append(lengthAttribute);
			
			foreach (DictionaryEntry entry in entries) {
				XmlElement entryElement = doc.CreateElement("Entry");
				
				XmlAttribute keyAttribute = doc.CreateAttribute("key");
				keyAttribute.InnerText = entry.Key.ToString();
				entryElement.Attributes.Append(keyAttribute);
				
				XmlAttribute valueAttribute = doc.CreateAttribute("value");
				valueAttribute.InnerText = entry.Value.ToString();
				entryElement.Attributes.Append(valueAttribute);
				
				entries.AppendChild(entryElement);
			}
			
			return entries;
		}
		
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		public event EventHandler Changed;
	}
}
*/
