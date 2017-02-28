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
using System.Text;
using System.Windows;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Interaction logic for StringListEditorDialog.xaml
	/// </summary>
	public partial class StringListEditorDialog : Window
	{
		public StringListEditorDialog()
		{
			InitializeComponent();
		}

		public bool ShowBrowse
		{
			get { return stringListEditor.ShowBrowse; }
			set { stringListEditor.ShowBrowse = value; }
		}

		public string ListCaption
		{
			get { return stringListEditor.ListCaption; }
			set { stringListEditor.ListCaption = value; }
		}

		public string TitleText
		{
			get { return stringListEditor.TitleText; }
			set { stringListEditor.TitleText = value; }
		}

		public string AddButtonText
		{
			get { return stringListEditor.AddButtonText; }
			set { stringListEditor.AddButtonText = value; }
		}

		public string[] GetList()
		{
			return stringListEditor.GetList();
		}

		public void LoadList(IEnumerable<string> list)
		{
			stringListEditor.LoadList(list);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}