// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Xml;


using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace Altaxo.Worksheet.Commands
{
	[ConditionAttribute()]
	public class SelectedDataCondition : AbstractCondition
	{
		[ICSharpCode.Core.AddIns.XmlMemberAttribute("selecteddatacolumns", IsRequired = true)]
		string selectedData;
		
		public string SelectedData 
		{
			get 
			{
				return selectedData;
			}
			set 
			{
				selectedData = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			if(Current.Workbench.ActiveViewContent==null)
				return false;
			if(!(Current.Workbench.ActiveViewContent is Altaxo.Worksheet.GUI.WorksheetController))
				return false;

			Altaxo.Worksheet.GUI.WorksheetController ctrl 
				= Current.Workbench.ActiveViewContent as Altaxo.Worksheet.GUI.WorksheetController; 

			return ctrl.SelectedColumns.Count>=1;
		}
	}
}
