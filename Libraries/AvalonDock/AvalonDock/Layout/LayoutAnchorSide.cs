﻿/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

using System;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutAnchorSide : LayoutGroup<LayoutAnchorGroup>
	{
		#region Constructors

		public LayoutAnchorSide()
		{
		}

		#endregion

		#region Properties

		#region Side

		private AnchorSide _side;
		public AnchorSide Side
		{
			get
			{
				return _side;
			}
			private set
			{
				if (_side != value)
				{
					RaisePropertyChanging("Side");
					_side = value;
					RaisePropertyChanged("Side");
				}
			}
		}

		#endregion

		#endregion

		#region Overrides

		protected override bool GetVisibility()
		{
			return Children.Count > 0;
		}


		protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
			base.OnParentChanged(oldValue, newValue);

			UpdateSide();
		}

		#endregion

		#region Private Methods

		private void UpdateSide()
		{
			if (Root.LeftSide == this)
				Side = AnchorSide.Left;
			else if (Root.TopSide == this)
				Side = AnchorSide.Top;
			else if (Root.RightSide == this)
				Side = AnchorSide.Right;
			else if (Root.BottomSide == this)
				Side = AnchorSide.Bottom;
		}

		#endregion
	}
}
