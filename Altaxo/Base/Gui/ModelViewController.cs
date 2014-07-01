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

using System;

namespace Altaxo.Gui
{
	/// <summary>
	/// The interface that a controller of the MVC (Model-View-Controller) model must implement.
	/// </summary>
	public interface IMVCController : IDisposable
	{
		/// <summary>
		/// Returns the Gui element that shows the model to the user.
		/// </summary>
		object ViewObject { get; set; }

		/// <summary>
		/// Returns the model (document) that this controller manages.
		/// </summary>
		object ModelObject { get; }
	}

	/// <summary>
	/// Concatenation of a <see cref="IMVCController" /> and a <see cref="IApplyController" />.
	/// </summary>
	public interface IMVCAController : IMVCController, IApplyController
	{
	}

	/// <summary>
	/// Enumerates wheter or not a document controlled by a controller is directly changed or not.
	/// </summary>
	public enum UseDocument
	{
		/// <summary>
		/// The document is not used directly. All changes by the user are temporarily stored and committed only when the Apply function of the controller is called.
		/// </summary>
		Copy,

		/// <summary>
		/// The document is used directly. All changes by the user are directly reflected in the controlled document.
		/// </summary>
		Directly
	}

	public interface IMVCANController : IMVCAController
	{
		/// <summary>
		/// Initialize the controller with the document. If successfull, the function has to return true.
		/// </summary>
		/// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
		/// <returns>Returns <see langword="true"/> if successfull; otherwise <see langword="false"/>.</returns>
		bool InitializeDocument(params object[] args);

		/// <summary>
		/// Sets whether or not a copy of the document is used. If set to true, a copy of the document is used, so if the controller is not applied,
		/// all changes can be reverted. If set to false, no copy must be made. The document is directly changed by the controller, and changes can not be reverted.
		/// Use the last option if a controller up in the hierarchie has already made a copy of the document.
		/// </summary>
		UseDocument UseDocumentCopy { set; }
	}

	/// <summary>
	/// Extends the <see cref="IMVCANController"/> by an event to signal that the user changed some data. This can be used for instance to update a preview panel etc.
	/// </summary>
	public interface IMVCANDController : IMVCANController
	{
		/// <summary>Event fired when the user changed some data that will change the model.</summary>
		event Action<IMVCANDController> MadeDirty;

		/// <summary>Gets the provisional model object. This is the model object that is based on the current user input.</summary>
		object ProvisionalModelObject { get; }
	}

	public abstract class MVCANControllerBase<TModel, TView> : IMVCANController where TView : class
	{
		protected TModel _doc;
		protected TModel _originalDoc;
		protected TView _view;
		protected bool _useDocumentCopy;

		protected abstract void Initialize(bool initData);

		protected virtual void AttachView()
		{
		}

		protected virtual void DetachView()
		{
		}

		public abstract bool Apply();

		public virtual bool InitializeDocument(params object[] args)
		{
			if (null == args || 0 == args.Length || !(args[0] is TModel))
				return false;

			_doc = _originalDoc = (TModel)args[0];
			if (_useDocumentCopy && _originalDoc is ICloneable)
				_doc = (TModel)((ICloneable)_originalDoc).Clone();

			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { _useDocumentCopy = value == UseDocument.Copy; }
		}

		public virtual object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					DetachView();
				}

				_view = value as TView;

				if (null != _view)
				{
					Initialize(false);
					AttachView();
				}
			}
		}

		public virtual object ModelObject
		{
			get { return _originalDoc; }
		}

		public virtual void Dispose()
		{
			ViewObject = null;
		}
	}

	public abstract class MVCANDControllerBase<TModel, TView> : MVCANControllerBase<TModel, TView>, IMVCANDController where TView : class
	{
		protected Altaxo.Main.EventSuppressor _suppressDirtyEvent = new Altaxo.Main.EventSuppressor(null);

		public event Action<IMVCANDController> MadeDirty;

		public override bool InitializeDocument(params object[] args)
		{
			if (null == args || 0 == args.Length || !(args[0] is TModel))
				return false;

			_doc = _originalDoc = (TModel)args[0];
			if (_useDocumentCopy && _originalDoc is ICloneable)
				_doc = (TModel)((ICloneable)_originalDoc).Clone();

			using (var suppressor = _suppressDirtyEvent.Suspend())
			{
				Initialize(true);
			}
			return true;
		}

		public override object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					DetachView();
				}

				_view = value as TView;

				if (null != _view)
				{
					using (var suppressor = _suppressDirtyEvent.Suspend())
					{
						Initialize(false);
						AttachView();
					}
				}
			}
		}

		protected virtual void OnMadeDirty()
		{
			if (_suppressDirtyEvent.PeekEnabled && null != MadeDirty)
				MadeDirty(this);
		}

		public object ProvisionalModelObject
		{
			get { return _doc; }
		}
	}

	/// <summary>
	/// Wraps an <see cref="Altaxo.Gui.IMVCANController"/> instance in a wrapper class
	/// </summary>
	public interface IMVCControllerWrapper
	{
		Altaxo.Gui.IMVCANController MVCController { get; }
	}
}