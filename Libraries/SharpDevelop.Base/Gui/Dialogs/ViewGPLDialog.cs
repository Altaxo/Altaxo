// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

using ICSharpCode.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ViewGPLDialog : BaseSharpDevelopForm 
	{
		public ViewGPLDialog()
		{
			base.SetupFromXml(Path.Combine(PropertyService.DataDirectory, @"resources\dialogs\ViewGPLDialog.xfrm"));
			LoadGPL();
		}
		
		void LoadGPL()
		{
			string filename = FileUtilityService.SharpDevelopRootPath + 
			                  Path.DirectorySeparatorChar + "doc" +
			                  Path.DirectorySeparatorChar + "license.txt";
			if (FileUtilityService.TestFileExists(filename)) {
				RichTextBox licenseRichTextBox = (RichTextBox)ControlDictionary["licenseRichTextBox"];
				licenseRichTextBox.LoadFile(filename, RichTextBoxStreamType.PlainText);
			}
		}
	}
}
