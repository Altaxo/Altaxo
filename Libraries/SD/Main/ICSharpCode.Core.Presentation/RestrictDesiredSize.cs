﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Cancels the desired size of the child elements. Use this control around scrolling containers (e.g. ListBox) used
	/// inside auto-scroll contexts.
	/// </summary>
	public class RestrictDesiredSize : Decorator
	{
		Size lastArrangeSize = new Size(double.NaN, double.NaN);
		
		protected override Size MeasureOverride(Size constraint)
		{
			return new Size(0, 0);
		}
		
		protected override Size ArrangeOverride(Size arrangeSize)
		{
			if (lastArrangeSize != arrangeSize) {
				lastArrangeSize = arrangeSize;
				base.MeasureOverride(arrangeSize);
			}
			return base.ArrangeOverride(arrangeSize);
		}
	}
}
