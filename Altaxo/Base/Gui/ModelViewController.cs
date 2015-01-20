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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	/// <summary>
	/// Interface that can be optionally implemented by controllers to support some actions when the controller's Apply function is successfully executed.
	/// The controller has to call the event <see cref="SuccessfullyApplied"/> after each successfully apply.
	/// </summary>
	public interface IMVCSupportsApplyCallback
	{
		/// <summary>
		/// Occurs when the controller has sucessfully executed the apply function.
		/// </summary>
		event Action SuccessfullyApplied;
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

		/// <summary>
		/// Called when the user input has to be applied to the document being controlled. Returns true if Apply is successfull.
		/// </summary>
		/// <param name="disposeController">If the Apply operation was successfull, and this argument is <c>true</c>, the controller should release all temporary resources, because they are not needed any more.
		/// If this argument is <c>false</c>, the controller should be reinitialized with the current model (the model that results from the Apply operation).</param>
		/// <returns>
		/// True if the apply operation was successfull, otherwise false. If false is returned, the <paramref name="disposeController" /> argument is ignored: thus the controller is not disposed.
		/// </returns>
		/// <remarks>
		/// This function is called in two cases: Either the user pressed OK or the user pressed Apply.
		/// </remarks>
		public abstract bool Apply(bool disposeController);

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public virtual bool Revert(bool disposeController)
		{
			return false;
		}

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
		protected Altaxo.Main.SuspendableObject _suppressDirtyEvent = new Altaxo.Main.SuspendableObject();

		public event Action<IMVCANDController> MadeDirty;

		public override bool InitializeDocument(params object[] args)
		{
			if (null == args || 0 == args.Length || !(args[0] is TModel))
				return false;

			_doc = _originalDoc = (TModel)args[0];
			if (_useDocumentCopy && _originalDoc is ICloneable)
				_doc = (TModel)((ICloneable)_originalDoc).Clone();

			using (var suppressor = _suppressDirtyEvent.SuspendGetToken())
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
					using (var suppressor = _suppressDirtyEvent.SuspendGetToken())
					{
						Initialize(false);
						AttachView();
					}
				}
			}
		}

		protected virtual void OnMadeDirty()
		{
			if (!_suppressDirtyEvent.IsSuspended && null != MadeDirty)
				MadeDirty(this);
		}

		public object ProvisionalModelObject
		{
			get { return _doc; }
		}
	}

	public struct ControllerAndSetNullMethod
	{
		private IMVCAController _doc;
		private Action _setMemberToNullAction;

		public ControllerAndSetNullMethod(IMVCAController doc, Action setMemberToNullAction)
		{
			_doc = doc;
			_setMemberToNullAction = setMemberToNullAction;
		}

		public IMVCAController Controller { get { return _doc; } }

		public Action SetMemberToNullAction { get { return _setMemberToNullAction; } }

		public bool IsEmpty { get { return null == _doc; } }
	}

	public abstract class MVCANControllerEditOriginalDocBase<TModel, TView> : IMVCANController
		where TView : class
		where TModel : ICloneable
	{
		protected TModel _doc;
		protected TModel _clonedCopyOfDoc;
		protected TView _view;
		protected bool _useDocumentCopy;

		public bool IsDisposed { get; private set; }

		/// <summary>
		/// The suspend token of the document being edited. If <see cref="_useDocumentCopy"/> is false, we assume that a controller higher in hierarchy has made a copy
		/// of the document, thus we do not use a suspendToken for the document.
		/// </summary>
		protected Altaxo.Main.ISuspendToken _suspendToken;

		protected virtual void Initialize(bool initData)
		{
			if (null == _doc)
				throw new InvalidOperationException("This controller was not initialized with a document.");
			if (IsDisposed)
				throw new ObjectDisposedException(this.GetType().FullName);

			if (initData)
			{
				if (_useDocumentCopy && null == _suspendToken && (_doc is Altaxo.Main.ISuspendableByToken))
					_suspendToken = ((Altaxo.Main.ISuspendableByToken)_doc).SuspendGetToken();
			}
		}

		protected virtual void AttachView()
		{
		}

		protected virtual void DetachView()
		{
		}

		/// <summary>
		/// Called when the user input has to be applied to the document being controlled. Returns true if Apply is successfull.
		/// </summary>
		/// <param name="disposeController">If the Apply operation was successfull, and this argument is <c>true</c>, the controller should release all temporary resources, because they are not needed any more.
		/// If this argument is <c>false</c>, the controller should be reinitialized with the current model (the model that results from the Apply operation).</param>
		/// <returns>
		/// True if the apply operation was successfull, otherwise false. If false is returned, the <paramref name="disposeController" /> argument is ignored: thus the controller is not disposed.
		/// </returns>
		/// <remarks>
		/// This function is called in two cases: Either the user pressed OK or the user pressed Apply.
		/// </remarks>
		public abstract bool Apply(bool disposeController);

		/// <summary>
		/// Standard procedure at the end of the Apply phase. If the <paramref name="applyResult"/> is <c>true</c>, the controller is either disposed (if <c>disposeController</c> is <c>true</c>) or
		/// the document is shortly resumed (if <c>disposeController</c> is <c>false</c>. Nothing is done if <c>applyResult</c> is <c>false</c>.
		/// </summary>
		/// <param name="applyResult">If set to <c>true</c>, the apply operation was successful.</param>
		/// <param name="disposeController">If set to <c>true</c>, the controller is no longer needed and should be disposed.</param>
		/// <returns>The same value as the parameter <c>applyResult</c>. (for the convenience that you can use this function in the return statement of Apply).</returns>
		protected bool ApplyEnd(bool applyResult, bool disposeController)
		{
			if (applyResult == true)
			{
				if (disposeController)
				{
					Dispose();
				}
				else
				{
					if (null != _suspendToken)
					{
						_suspendToken.ResumeCompleteTemporarily();
					}
				}
			}
			return applyResult;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public virtual bool Revert(bool disposeController)
		{
			foreach (var subControllerItem in GetSubControllers())
			{
				if (null != subControllerItem.Controller)
					subControllerItem.Controller.Revert(disposeController);
			}

			bool reverted = false;
			if (null != _clonedCopyOfDoc && !object.ReferenceEquals(_doc, _clonedCopyOfDoc))
			{
				CopyHelper.Copy(ref _doc, _clonedCopyOfDoc);
				reverted = true;
			}

			if (disposeController)
			{
				Dispose();
			}
			else // not disposing means we have to show the reverted data
			{
				var viewTmp = ViewObject; // store view
				ViewObject = null; // detach view temporarily
				Initialize(true); // initialize data
				ViewObject = viewTmp; // attach view again
			}

			return reverted;
		}

		public virtual bool InitializeDocument(params object[] args)
		{
			if (IsDisposed)
				throw new ObjectDisposedException(this.GetType().FullName);

			if (null == args || 0 == args.Length || !(args[0] is TModel))
				return false;

			_doc = (TModel)args[0];

			if (_useDocumentCopy && _doc is ICloneable)
				_clonedCopyOfDoc = (TModel)((ICloneable)_doc).Clone();

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
			get { return _doc; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public abstract IEnumerable<ControllerAndSetNullMethod> GetSubControllers();

		public virtual void Dispose(bool isDisposing)
		{
			if (!IsDisposed)
			{
				foreach (var subControllerItem in this.GetSubControllers())
				{
					if (null != subControllerItem.Controller)
						subControllerItem.Controller.Dispose();
					if (null != subControllerItem.SetMemberToNullAction)
						subControllerItem.SetMemberToNullAction();
				}

				ViewObject = null;

				if (null != _suspendToken)
				{
					_suspendToken.Dispose();
					_suspendToken = null;
				}

				if ((_clonedCopyOfDoc is IDisposable) && !object.ReferenceEquals(_doc, _clonedCopyOfDoc))
					((IDisposable)_clonedCopyOfDoc).Dispose();

				IsDisposed = true;
			}
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