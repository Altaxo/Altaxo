﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2149 $</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	public class CodeCompletionPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.CodeCompletionOptionPanel.xfrm"));
			
			EnableCodeCompletionSettingsGroupBox();
			Get<CheckBox>("codeCompletionEnabled").CheckedChanged += delegate(object sender, EventArgs e) {
				EnableCodeCompletionSettingsGroupBox();
			};
			Get<CheckBox>("codeCompletionEnabled").Checked = CodeCompletionOptions.EnableCodeCompletion;
			
			Get<CheckBox>("useDataUsageCache").CheckedChanged += delegate(object sender, EventArgs e) {
				ControlDictionary["dataUsageCacheLabel1"].Enabled = Get<CheckBox>("useDataUsageCache").Checked;
				ControlDictionary["dataUsageCacheLabel2"].Enabled = Get<CheckBox>("useDataUsageCache").Checked;
				ControlDictionary["dataUsageCacheItemCountNumericUpDown"].Enabled = Get<CheckBox>("useDataUsageCache").Checked;
			};
			Get<CheckBox>("useDataUsageCache").Checked = CodeCompletionOptions.DataUsageCacheEnabled;
			
			Get<NumericUpDown>("dataUsageCacheItemCount").Value = CodeCompletionOptions.DataUsageCacheItemCount;
			
			ControlDictionary["clearDataUseCacheButton"].Click += delegate(object sender, EventArgs e) {
				ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionDataUsageCache.ResetCache();
			};
			
			Get<CheckBox>("useTooltips").CheckedChanged += delegate(object sender, EventArgs e) {
				ControlDictionary["useDebugTooltipsOnlyCheckBox"].Enabled = Get<CheckBox>("useTooltips").Checked;
			};
			Get<CheckBox>("useTooltips").Checked = CodeCompletionOptions.TooltipsEnabled;
			
			Get<CheckBox>("useDebugTooltipsOnly").Checked = CodeCompletionOptions.TooltipsOnlyWhenDebugging;
			
			Get<CheckBox>("useKeywordCompletion").Checked = CodeCompletionOptions.KeywordCompletionEnabled;
			
			Get<CheckBox>("useInsight").CheckedChanged += delegate(object sender, EventArgs e) {
				ControlDictionary["refreshInsightOnCommaCheckBox"].Enabled = Get<CheckBox>("useInsight").Checked;
			};
			Get<CheckBox>("useInsight").Checked = CodeCompletionOptions.InsightEnabled;
			
			Get<CheckBox>("refreshInsightOnComma").Checked = CodeCompletionOptions.InsightRefreshOnComma;
		}
		
		public override bool StorePanelContents()
		{
			CodeCompletionOptions.EnableCodeCompletion = Get<CheckBox>("codeCompletionEnabled").Checked;
			CodeCompletionOptions.DataUsageCacheEnabled = Get<CheckBox>("useDataUsageCache").Checked;
			CodeCompletionOptions.DataUsageCacheItemCount = (int)Get<NumericUpDown>("dataUsageCacheItemCount").Value;
			CodeCompletionOptions.TooltipsEnabled = Get<CheckBox>("useTooltips").Checked;
			CodeCompletionOptions.TooltipsOnlyWhenDebugging = Get<CheckBox>("useDebugTooltipsOnly").Checked;
			CodeCompletionOptions.KeywordCompletionEnabled = Get<CheckBox>("useKeywordCompletion").Checked;
			CodeCompletionOptions.InsightEnabled = Get<CheckBox>("useInsight").Checked;
			CodeCompletionOptions.InsightRefreshOnComma = Get<CheckBox>("refreshInsightOnComma").Checked;
			return base.StorePanelContents();
		}
		
		void EnableCodeCompletionSettingsGroupBox()
		{
			ControlDictionary["groupBox"].Enabled = Get<CheckBox>("codeCompletionEnabled").Checked;
		}
	}
}
