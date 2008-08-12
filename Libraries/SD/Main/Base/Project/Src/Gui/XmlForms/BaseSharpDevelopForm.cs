﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	public abstract class BaseSharpDevelopForm : XmlForm
	{
//		public BaseSharpDevelopForm(string fileName) : base(fileName)
//		{
//		}
		
		public BaseSharpDevelopForm()
		{
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
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
