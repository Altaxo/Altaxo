﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Markup extension that works like StringParser.Parse
	/// </summary>
	[MarkupExtensionReturnType(typeof(string))]
	public sealed class StringParseExtension : LanguageDependentExtension
	{
		string text;
		
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
		
		public override string Value {
			get {
				string result = StringParser.Parse(text);
				if (UsesAccessors)
					result = MenuService.ConvertLabel(result);
				return result;
			}
		}
	}
}
