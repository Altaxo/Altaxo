// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public class CombineConfiguration : AbstractConfiguration, IXmlConvertable
	{
		ArrayList configurations = new ArrayList();
		Combine combine;
		
		public class Config 
		{
			public CombineEntry Entry;
			public string       ConfigurationName;
			public bool         Build;
		}
		
		public Config GetConfiguration(int number)
		{
			if (number < configurations.Count) {
				return (Config)configurations[number];
			} 
			Debug.Assert(false, "Configuration number " + number + " not found.\n" + configurations.Count + " configurations avaiable.");
			return null;
		}
		
		public CombineConfiguration(string name, Combine combine)
		{
			this.combine = combine;
			this.Name    = name;
		}
		
		public CombineConfiguration(XmlElement element, Combine combine)
		{
			this.combine = combine;
			Name        = element.Attributes["name"].InnerText;
			
			XmlNodeList nodes = element.ChildNodes;
			foreach (XmlElement confignode in nodes) {
				Config config = new Config();
				
				config.Entry             = combine.GetEntry(confignode.Attributes["name"].InnerText);
				config.ConfigurationName = confignode.Attributes["configurationname"].InnerText;
				config.Build             = Boolean.Parse(confignode.Attributes["build"].InnerText);
					
				configurations.Add(config);
			}
		}
		
		public void AddEntry(IProject project)
		{
			Config conf = new Config();
			conf.Entry             = combine.GetEntry(project.Name);
			conf.ConfigurationName = project.ActiveConfiguration.Name;
			conf.Build             = false;
			configurations.Add(conf);
		}
		public void AddEntry(Combine  combine)
		{
			Config conf = new Config();
			conf.Entry             = this.combine.GetEntry(combine.Name);
			conf.ConfigurationName = combine.ActiveConfiguration != null ? combine.ActiveConfiguration.Name : String.Empty;
			conf.Build             = false;
			configurations.Add(conf);
		}
		
		public object FromXmlElement(XmlElement element)
		{
			return null;
		}
		
		public void RemoveEntry(CombineEntry entry)
		{
			Config removeConfig = null;
			
			foreach (Config config in configurations) {
				if (config.Entry == entry) {
					removeConfig = config;
					break;
				}
			}
			
			Debug.Assert(removeConfig != null);
			configurations.Remove(removeConfig);
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			if (combine == null) 
				throw new ApplicationException("combine can't be null");
			
			XmlElement cel = doc.CreateElement("Configuration");
			
			XmlAttribute nameattr = doc.CreateAttribute("name");
			nameattr.InnerText = Name;
			cel.Attributes.Append(nameattr);
			
			foreach (Config config in configurations) {
				if (config == null || config.Entry == null) {
					continue;
				}
				XmlElement el = doc.CreateElement("Entry");
				
				XmlAttribute attr1 = doc.CreateAttribute("name");
				attr1.InnerText = config.Entry.Name;
				el.Attributes.Append(attr1);
				
				XmlAttribute attr2 = doc.CreateAttribute("configurationname");
				attr2.InnerText = config.ConfigurationName;
				el.Attributes.Append(attr2);
				
				XmlAttribute attr3 = doc.CreateAttribute("build");
				attr3.InnerText = config.Build.ToString();
				el.Attributes.Append(attr3);
				
				cel.AppendChild(el);
			}
			return cel;
		}
	}
}
