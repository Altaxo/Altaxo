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
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.XmlForms;

using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	public class SharpDevelopObjectCreator : DefaultObjectCreator
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public override object CreateObject(string name)
		{
			object o = base.CreateObject(name);
			if (o != null) {
				try {
					PropertyInfo propertyInfo = o.GetType().GetProperty("FlatStyle");
					if (propertyInfo != null) {
						if (o is Label) {
							propertyInfo.SetValue(o, FlatStyle.Standard, null);
						} else {
							propertyInfo.SetValue(o, FlatStyle.System, null);
						}
					}
				} catch (Exception) {}
			}
			return o;
		}
	}
}
