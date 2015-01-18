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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Controller that shows to documents: one document in the enabled state, which can be changed. And another document in the disabled state, which can not be changed.
	/// </summary>
	/// <typeparam name="TModel">The type of the model.</typeparam>
	[ExpectedTypeOfView(typeof(IConditionalDocumentView))]
	public class ConditionalDocumentControllerWithDisabledView<TModel> : IMVCANController
	{
		private Func<TModel> _documentCreationActionForEnabledState;
		private Func<TModel> _documentCreationActionForDisabledState;
		private Func<TModel, UseDocument, IMVCANController> _controllerCreationAction;

		private IConditionalDocumentView _view;
		private IMVCANController _controllerForEnabledState;
		private IMVCANController _controllerForDisabledState;
		private UseDocument _useDocumentCopy;
		protected bool _isInEnabledState;
		protected string _enablingText = null;

		public ConditionalDocumentControllerWithDisabledView(
			Func<TModel> DocumentCreationActionForEnabledState,
			Func<TModel> DocumentCreationActionForDisabledState
			)
			: this(DocumentCreationActionForEnabledState, DocumentCreationActionForEnabledState, InternalCreateController)
		{
		}

		public ConditionalDocumentControllerWithDisabledView(
			Func<TModel> DocumentCreationActionForEnabledState,
			Func<TModel> DocumentCreationActionForDisabledState,
			Func<TModel, UseDocument, IMVCANController> ControllerCreationAction)
		{
			if (null == DocumentCreationActionForEnabledState)
				throw new ArgumentNullException("CreationAction");
			if (null == DocumentCreationActionForDisabledState)
				throw new ArgumentNullException("CreationAction2");
			if (null == ControllerCreationAction)
				throw new ArgumentNullException("ControllerCreationAction");

			_documentCreationActionForEnabledState = DocumentCreationActionForEnabledState;
			_controllerCreationAction = ControllerCreationAction;
		}

		public IMVCANController UnderlyingControllerForEnabledState
		{
			get
			{
				return _controllerForEnabledState;
			}
		}

		public string EnablingText
		{
			set
			{
				_enablingText = value;
				if (null != _view && null != _enablingText)
					_view.EnablingText = value;
			}
		}

		private static IMVCANController InternalCreateController(TModel doc, UseDocument useDocumentCopy)
		{
			return (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { doc }, typeof(IMVCANController), useDocumentCopy);
		}

		/// <summary>
		/// Initialize the controller with the document. If successfull, the function has to return true.
		/// Here, you can give two arguments. The first is the document for the enabled state, the second is the document to show in the disabled state.
		/// </summary>
		/// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
		/// <returns>
		/// Returns <see langword="true" /> if successfull; otherwise <see langword="false" />.
		/// </returns>
		public bool InitializeDocument(params object[] args)
		{
			if (null != args && args.Length >= 1 && null != args[0] && (args[0] is TModel))
			{
				_controllerForEnabledState = _controllerCreationAction((TModel)args[0], _useDocumentCopy);
				_isInEnabledState = true;
			}
			else
			{
				_isInEnabledState = false;
			}

			if (null != args && args.Length >= 2 && null != args[1] && (args[1] is TModel))
			{
				_controllerForDisabledState = _controllerCreationAction((TModel)args[1], _useDocumentCopy);
			}

			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set
			{
				_useDocumentCopy = value;
				if (null != _controllerForEnabledState)
					_controllerForEnabledState.UseDocumentCopy = value;
				if (null != _controllerForDisabledState)
					_controllerForDisabledState.UseDocumentCopy = value;
			}
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (_view != null)
				{
					_view.ConditionalViewEnabledChanged -= EhViewEnabledChanged;
				}
				_view = value as IConditionalDocumentView;

				if (_view != null)
				{
					Initialize(false);
					_view.ConditionalViewEnabledChanged += EhViewEnabledChanged;
				}
			}
		}

		public object ModelObject
		{
			get { return _isInEnabledState ? _controllerForEnabledState.ModelObject : null; }
		}

		public void Dispose()
		{
		}

		public bool Apply(bool disposeController)
		{
			if (null != _controllerForEnabledState)
				return _controllerForEnabledState.Apply(disposeController);
			else
				return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		private void Initialize(bool initData)
		{
			if (null != _view)
			{
				if (null != _enablingText)
					_view.EnablingText = _enablingText;

				if (_isInEnabledState)
				{
					_view.IsConditionalViewEnabled = true;
					_view.ConditionalView = _controllerForEnabledState.ViewObject;
				}
				else
				{
					_view.IsConditionalViewEnabled = false;
					_view.ConditionalView = _controllerForDisabledState.ViewObject;
				}
			}
		}

		private void EhViewEnabledChanged()
		{
			AnnounceEnabledChanged(_view.IsConditionalViewEnabled);
		}

		public void AnnounceEnabledChanged(bool enableState)
		{
			_isInEnabledState = enableState;

			if (enableState)
			{
				if (null != _controllerForEnabledState)
				{
					_view.ConditionalView = _controllerForEnabledState.ViewObject;
				}
				else // document and controller have to be constructed
				{
					TModel document = _documentCreationActionForEnabledState();
					_controllerForEnabledState = _controllerCreationAction(document, _useDocumentCopy);
					if (null != _view)
						_view.ConditionalView = _controllerForEnabledState.ViewObject;
				}
			}
			else // disabled
			{
				if (null != _controllerForDisabledState)
				{
					_view.ConditionalView = _controllerForDisabledState.ViewObject;
				}
				else // document and controller have to be constructed
				{
					TModel document = _documentCreationActionForDisabledState();
					_controllerForDisabledState = _controllerCreationAction(document, _useDocumentCopy);
					if (null != _view)
						_view.ConditionalView = _controllerForEnabledState.ViewObject;
				}
			}

			if (null != _view)
			{
				_view.IsConditionalViewEnabled = enableState;
			}
		}
	}
}