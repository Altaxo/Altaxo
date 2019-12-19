﻿/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

namespace AvalonDock.Layout
{
	public interface ILayoutUpdateStrategy
	{
		bool BeforeInsertAnchorable(
			LayoutRoot layout,
			LayoutAnchorable anchorableToShow,
			ILayoutContainer destinationContainer);

		void AfterInsertAnchorable(
			LayoutRoot layout,
			LayoutAnchorable anchorableShown);


		bool BeforeInsertDocument(
			LayoutRoot layout,
			LayoutDocument anchorableToShow,
			ILayoutContainer destinationContainer);

		void AfterInsertDocument(
			LayoutRoot layout,
			LayoutDocument anchorableShown);
	}
}
