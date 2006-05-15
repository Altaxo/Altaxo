﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1380 $</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class LayoutConfiguration
	{
		readonly static string configFile = "LayoutConfig.xml";
		public static ArrayList Layouts = new ArrayList();
		public static string[]  DefaultLayouts = new string[] {
			"Default",
			"Debug",
			"Plain"
		};
		string name;
		string fileName;
		string displayName = null;
		
		bool   readOnly;
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string DisplayName {
			get {
				return displayName == null ? Name : displayName;
			}
			set {
				displayName = value;
			}
		}
		
		public bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				readOnly = value;
			}
		}
		
		public LayoutConfiguration()
		{
			
		}
		
		LayoutConfiguration(XmlElement el)
		{
			name       = el.GetAttribute("name");
			fileName   = el.GetAttribute("file");
			readOnly   = Boolean.Parse(el.GetAttribute("readonly"));
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
		
		public static string CurrentLayoutName {
			get {
				
				return PropertyService.Get("Workbench.CurrentLayout", "Default");
			}
			set {
				if (WorkbenchSingleton.InvokeRequired)
					throw new InvalidOperationException("Invoke required");
				if (value != CurrentLayoutName) {
					PropertyService.Set("Workbench.CurrentLayout", value);
					WorkbenchSingleton.Workbench.WorkbenchLayout.LoadConfiguration();
					OnLayoutChanged(EventArgs.Empty);
					#if DEBUG
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
					#endif
				}
			}
		}
		
		public static string CurrentLayoutFileName {
			get {
				string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
				LayoutConfiguration current = CurrentLayout;
				if (current != null) {
					return Path.Combine(configPath, current.FileName);
				}
				return null;
			}
		}
		
		public static string CurrentLayoutTemplateFileName {
			get {
				string dataPath = Path.Combine(PropertyService.DataDirectory, "resources" + Path.DirectorySeparatorChar + "layouts");
				LayoutConfiguration current = CurrentLayout;
				if (current != null) {
					return Path.Combine(dataPath, current.FileName);
				}
				return null;
			}
		}
		
		public static LayoutConfiguration CurrentLayout {
			get {
				foreach (LayoutConfiguration config in Layouts) {
					if (config.name == CurrentLayoutName) {
						return config;
					}
				}
				return null;
			}
		}
		
		public static LayoutConfiguration GetLayout(string name)
		{
			foreach (LayoutConfiguration config in Layouts) {
				if (config.Name == name) {
					return config;
				}
			}
			return null;
		}
		
		static LayoutConfiguration()
		{
			LoadLayoutConfiguration();
		}
		
		static void LoadLayoutConfiguration()
		{
			
			string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
			if (File.Exists(Path.Combine(configPath, configFile))) {
				LoadLayoutConfiguration(Path.Combine(configPath, configFile));
			}
			string dataPath   = Path.Combine(PropertyService.DataDirectory, "resources" + Path.DirectorySeparatorChar + "layouts");
			LoadLayoutConfiguration(Path.Combine(dataPath, configFile));
		}
		
		static void LoadLayoutConfiguration(string layoutConfig)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(layoutConfig);
			
			foreach (XmlElement el in doc.DocumentElement.ChildNodes) {
				Layouts.Add(new LayoutConfiguration(el));
			}
		}
		
		protected static void OnLayoutChanged(EventArgs e)
		{
			if (LayoutChanged != null) {
				LayoutChanged(null, e);
			}
		}
		public static event EventHandler LayoutChanged;
	}
}
