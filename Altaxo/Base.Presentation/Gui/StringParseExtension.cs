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

using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;
using System;
using System.Windows.Markup;

namespace Altaxo.Gui
{
	/// <summary>
	/// Markup extension that works like StringParser.Parse. This extension uses a given string directly (the string is <b>not</b> a resource key), then
	/// passes it to <see cref="StringParser.Parse(string)"/>. If <see cref="UsesAccessors"/> is set to true, the string is additionally
	/// converted to Wpf accessor syntax.
	/// </summary>
	[MarkupExtensionReturnType(typeof(string))]
	public sealed class StringParseExtension : LanguageDependentExtension
	{
		private string text;

		public StringParseExtension(string text)
		{
			this.text = text;
			this.UsesAccessors = true;
		}

		/// <summary>
		/// Set whether the text uses accessors.
		/// If set to true (default), accessors will be converted to WPF syntax.
		/// </summary>
		public bool UsesAccessors { get; set; }

		public override string Value
		{
			get
			{
				string result = StringParser.Parse(text);
				if (UsesAccessors)
					result = MenuService.ConvertLabel(result);
				return result;
			}
		}
	}
}