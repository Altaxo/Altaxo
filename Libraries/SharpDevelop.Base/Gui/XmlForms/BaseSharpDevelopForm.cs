// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.XmlForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	public abstract class BaseSharpDevelopForm : XmlForm
	{
		static PropertyService     propertyService = null;
		static IMessageService     messageService  = null;
		static StringParserService stringParserService = null;
		static FileUtilityService  fileUtilityService = null;
		static IconService         iconService = null;
		static MenuService         menuService = null;
		static ResourceService     resourceService = null;
		
		protected static ResourceService ResourceService {
			get {
				if (resourceService == null) {
					resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				}
				return resourceService;
			}
		}

		protected static MenuService MenuService {
			get {
				if (menuService == null) {
					menuService = (MenuService)ServiceManager.Services.GetService(typeof(MenuService));
				}
				return menuService;
			}
		}
		protected static IconService IconService {
			get {
				if (iconService == null) {
					iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
				}
				return iconService;
			}
		}
		protected static PropertyService PropertyService {
			get {
				if (propertyService == null) {
					propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				}
				return propertyService;
			}
		}
		
		protected static IMessageService MessageService {
			get {
				if (messageService == null) {
					messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				}
				return messageService;
			}
		}
		
		protected static StringParserService StringParserService {
			get {
				if (stringParserService == null) {
					stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				}
				return stringParserService;
			}
		}
		
		protected static FileUtilityService FileUtilityService {
			get {
				if (fileUtilityService == null) {
					fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				}
				return fileUtilityService;
			}
		}
		
		public BaseSharpDevelopForm(string fileName) : base(fileName)
		{
		}
		
		public BaseSharpDevelopForm()
		{
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
			xmlLoader.ObjectCreator        = new SharpDevelopObjectCreator();
		}
		
		public void SetEnabledStatus(bool enabled, params string[] controlNames)
		{
			foreach (string controlName in controlNames) {
				Control control = ControlDictionary[controlName];
				if (control == null) {
					MessageService.ShowError(controlName + " not found!");
				} else {
					control.Enabled = enabled;
				}
			}
		}
		
		
	}
}
