﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5574 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

using ICSharpCode.Core;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Holds a customized highlighting color.
	/// </summary>
	public class CustomizedHighlightingColor
	{
		/// <summary>
		/// The language to which this customization applies. null==all languages.
		/// </summary>
		public string Language;
		
		/// <summary>
		/// The name of the highlighting color being modified.
		/// </summary>
		public string Name;
		
		public bool Bold, Italic;
		public Color? Foreground, Background;
		
		public static List<CustomizedHighlightingColor> LoadColors()
		{
			return PropertyService.Get("CustomizedHighlightingRules", new List<CustomizedHighlightingColor>());
		}
		
		/// <summary>
		/// Saves the set of colors.
		/// </summary>
		public static void SaveColors(IEnumerable<CustomizedHighlightingColor> colors)
		{
			lock (staticLockObj) {
				activeColors = null;
				PropertyService.Set("CustomizedHighlightingRules", colors.ToList());
			}
			EventHandler e = ActiveColorsChanged;
			if (e != null)
				e(null, EventArgs.Empty);
		}
		
		static ReadOnlyCollection<CustomizedHighlightingColor> activeColors;
		static readonly object staticLockObj = new object();
		
		public static ReadOnlyCollection<CustomizedHighlightingColor> ActiveColors {
			get {
				lock (staticLockObj) {
					if (activeColors == null)
						activeColors = LoadColors().AsReadOnly();
					return activeColors;
				}
			}
		}
		
		/// <summary>
		/// Occurs when the set of customized highlighting colors was changed.
		/// </summary>
		public static EventHandler ActiveColorsChanged;
	}
}
