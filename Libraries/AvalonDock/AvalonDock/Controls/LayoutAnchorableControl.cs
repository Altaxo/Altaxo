﻿/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
	public class LayoutAnchorableControl : Control
	{
		#region Constructors

		static LayoutAnchorableControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorableControl)));
			FocusableProperty.OverrideMetadata(typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(false));
		}

		public LayoutAnchorableControl()
		{
			//SetBinding(FlowDirectionProperty, new Binding("Model.Root.Manager.FlowDirection") { Source = this });
		}

		#endregion

		#region Properties

		#region Model

		/// <summary>
		/// Model Dependency Property
		/// </summary>
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(LayoutAnchorable), typeof(LayoutAnchorableControl),
				new FrameworkPropertyMetadata((LayoutAnchorable)null, new PropertyChangedCallback(OnModelChanged)));

		/// <summary>
		/// Gets or sets the Model property.  This dependency property 
		/// indicates the model attached to this view.
		/// </summary>
		public LayoutAnchorable Model
		{
			get
			{
				return (LayoutAnchorable)GetValue(ModelProperty);
			}
			set
			{
				SetValue(ModelProperty, value);
			}
		}

		/// <summary>
		/// Handles changes to the Model property.
		/// </summary>
		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableControl)d).OnModelChanged(e);
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the Model property.
		/// </summary>
		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
			{
				((LayoutContent)e.OldValue).PropertyChanged -= Model_PropertyChanged;
			}

			if (Model != null)
			{
				Model.PropertyChanged += Model_PropertyChanged;
				SetLayoutItem(Model.Root.Manager.GetLayoutItemFromModel(Model));
			}
			else
				SetLayoutItem(null);
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsEnabled")
			{
				if (Model != null)
				{
					IsEnabled = Model.IsEnabled;
					if (!IsEnabled && Model.IsActive)
					{
						if ((Model.Parent != null) && (Model.Parent is LayoutAnchorablePane))
						{
							((LayoutAnchorablePane)Model.Parent).SetNextSelectedIndex();
						}
					}
				}
			}
		}

		#endregion

		#region LayoutItem

		/// <summary>
		/// LayoutItem Read-Only Dependency Property
		/// </summary>
		private static readonly DependencyPropertyKey LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(LayoutAnchorableControl),
				new FrameworkPropertyMetadata((LayoutItem)null));

		public static readonly DependencyProperty LayoutItemProperty = LayoutItemPropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the LayoutItem property.  This dependency property 
		/// indicates the LayoutItem attached to this tag item.
		/// </summary>
		public LayoutItem LayoutItem
		{
			get
			{
				return (LayoutItem)GetValue(LayoutItemProperty);
			}
		}

		/// <summary>
		/// Provides a secure method for setting the LayoutItem property.  
		/// This dependency property indicates the LayoutItem attached to this tag item.
		/// </summary>
		/// <param name="value">The new value for the property.</param>
		protected void SetLayoutItem(LayoutItem value)
		{
			SetValue(LayoutItemPropertyKey, value);
		}

		#endregion

		#endregion

		#region Overrides

		protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			if (Model != null)
				Model.IsActive = true;

			base.OnGotKeyboardFocus(e);
		}


		#endregion
	}
}
