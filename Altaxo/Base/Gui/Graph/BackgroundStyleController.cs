#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Calc;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IBackgroundStyleViewEventSink
	{
		/// <summary>
		/// Called if the background color is changed.
		/// </summary>
		/// <param name="newValue">The new selected item of the combo box.</param>
		void EhView_BackgroundBrushChanged(BrushX newValue);

		/// <summary>
		/// Called if the background style changed.
		/// </summary>
		/// <param name="newValue">The new index of the style.</param>
		void EhView_BackgroundStyleChanged(int newValue);
	}

	public interface IBackgroundStyleView
	{
		/// <summary>
		/// Get/sets the controller of this view.
		/// </summary>
		IBackgroundStyleViewEventSink Controller { get; set; }

		/// <summary>
		/// Initializes the content of the background color combo box.
		/// </summary>
		void BackgroundBrush_Initialize(BrushX brush);

		/// <summary>
		/// Initializes the enable state of the background color combo box.
		/// </summary>
		void BackgroundBrushEnable_Initialize(bool enable);

		/// <summary>
		/// Initializes the background styles.
		/// </summary>
		/// <param name="names"></param>
		/// <param name="selection"></param>
		void BackgroundStyle_Initialize(string[] names, int selection);
	}

	#endregion Interfaces

	/// <summary>
	/// Controls a IBackgroundStyle instance.
	/// </summary>
	[UserControllerForObject(typeof(IBackgroundStyle))]
	[ExpectedTypeOfView(typeof(IBackgroundStyleView))]
	public class BackgroundStyleController : MVCANDControllerEditOriginalDocBase<IBackgroundStyle, IBackgroundStyleView>, IBackgroundStyleViewEventSink
	{
		protected System.Type[] _backgroundStyles;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public BackgroundStyleController(IBackgroundStyle doc)
		{
			InitializeDocument(doc);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_backgroundStyles = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IBackgroundStyle));
			}

			if (null != _view)
			{
				InitializeBackgroundStyle();
			}
		}

		public override bool Apply(bool disposeController)
		{
			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.Controller = this;
		}

		protected override void DetachView()
		{
			_view.Controller = null;

			base.DetachView();
		}

		private void InitializeBackgroundStyle()
		{
			int sel = Array.IndexOf(this._backgroundStyles, this._doc == null ? null : this._doc.GetType());
			_view.BackgroundStyle_Initialize(Current.Gui.GetUserFriendlyClassName(this._backgroundStyles, true), sel + 1);

			if (this._doc != null && this._doc.SupportsBrush)
				_view.BackgroundBrush_Initialize(this._doc.Brush);
			else
				_view.BackgroundBrush_Initialize(new BrushX(NamedColors.Transparent));

			_view.BackgroundBrushEnable_Initialize(this._doc != null && this._doc.SupportsBrush);
		}

		public object TemporaryModelObject
		{
			get
			{
				return _doc;
			}
		}

		#region IPlotRangeViewEventSink Members

		public void EhView_BackgroundBrushChanged(BrushX brush)
		{
			if (this._doc != null && this._doc.SupportsBrush)
			{
				this._doc.Brush = brush;
				OnMadeDirty();
			}
		}

		/// <summary>
		/// Called if the background style changed.
		/// </summary>
		/// <param name="newValue">The new index of the style.</param>
		public void EhView_BackgroundStyleChanged(int newValue)
		{
			BrushX backgroundColor = new BrushX(NamedColors.Transparent);

			if (newValue != 0)
			{
				_doc = (IBackgroundStyle)Activator.CreateInstance(this._backgroundStyles[newValue - 1]);
				backgroundColor = _doc.Brush;
			}
			else // is null
			{
				_doc = null;
			}

			if (_doc != null && _doc.SupportsBrush)
			{
				_view.BackgroundBrush_Initialize(backgroundColor);
				_view.BackgroundBrushEnable_Initialize(true);
			}
			else
			{
				_view.BackgroundBrushEnable_Initialize(false);
			}

			OnMadeDirty();
		}

		#endregion IPlotRangeViewEventSink Members
	}
}