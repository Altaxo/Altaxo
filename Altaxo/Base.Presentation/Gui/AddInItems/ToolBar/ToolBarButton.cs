﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Altaxo.AddInItems;
using Altaxo.Main.Services;

namespace Altaxo.Gui.AddInItems
{
	/// <summary>
	/// A tool bar button based on the AddIn-tree.
	/// </summary>
	internal sealed class ToolBarButton : Button, IStatusUpdate
	{
		private readonly Codon codon;
		private readonly object caller;
		private readonly string inputGestureText;
		private readonly IEnumerable<ICondition> conditions;

		public ToolBarButton(UIElement inputBindingOwner, Codon codon, object caller, IReadOnlyCollection<ICondition> conditions)
		{
			ToolTipService.SetShowOnDisabled(this, true);

			this.codon = codon;
			this.caller = caller;
			this.Command = CommandWrapper.CreateLazyCommand(codon, conditions);
			this.CommandParameter = caller;
			this.Content = ToolBarService.CreateToolBarItemContent(codon);
			this.conditions = conditions;

			if (codon.Properties.Contains("name"))
			{
				this.Name = codon.Properties["name"];
			}

			if (!string.IsNullOrEmpty(codon.Properties["shortcut"]))
			{
				KeyGesture kg = MenuService.ParseShortcut(codon.Properties["shortcut"]);
				MenuCommand.AddGestureToInputBindingOwner(inputBindingOwner, kg, this.Command, GetFeatureName());
				this.inputGestureText = MenuService.GetDisplayStringForShortcut(kg);
			}
			UpdateText();

			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.ButtonStyleKey);
		}

		private string GetFeatureName()
		{
			string commandName = codon.Properties["command"];
			if (string.IsNullOrEmpty(commandName))
			{
				return codon.Properties["class"];
			}
			else
			{
				return commandName;
			}
		}

		public void UpdateText()
		{
			if (codon.Properties.Contains("tooltip"))
			{
				string toolTip = StringParser.Parse(codon.Properties["tooltip"]);
				if (!string.IsNullOrEmpty(inputGestureText))
					toolTip = toolTip + " (" + inputGestureText + ")";
				this.ToolTip = toolTip;
			}
		}

		/// <summary>
		/// Updates the status of the toolbar button.
		/// </summary>
		/// <remarks>The workbench calls <see cref="IStatusUpdate.UpdateStatus"/> for all tool bars, usually when
		/// <see cref="CommandManager.RequerySuggested"/> fires.</remarks>
		public void UpdateStatus()
		{
			if (Altaxo.AddInItems.Condition.GetFailedAction(conditions, caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
		}
	}
}