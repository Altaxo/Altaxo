// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core.Services;
using ICSharpCode.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	public class SharpDevelopPropertyValueCreator : IPropertyValueCreator
	{
		public bool CanCreateValueForType(Type propertyType)
		{
			return propertyType == typeof(Icon) || propertyType == typeof(Image);
		}
		
		public object CreateValue(Type propertyType, string valueString)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			if (propertyType == typeof(Icon)) {
				return resourceService.GetIcon(valueString);
			}
			
			if (propertyType == typeof(Image)) {
				return resourceService.GetBitmap(valueString);
			}
			
			return null;
		}
	}
}
