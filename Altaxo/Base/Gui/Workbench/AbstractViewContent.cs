// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Provides a default implementation for the IViewContent interface.
	/// It provides a files collection that, by default, automatically registers the view with the
	/// files added to it.
	/// Several properties have default implementation that depend on the contents of the files collection,
	/// e.g. IsDirty is true when at least one of the files in the collection is dirty.
	/// To support the changed event, this class registers event handlers with the members of the files collection.
	///
	/// When used with an empty Files collection, IsViewOnly will return true and this class can be used as a base class
	/// for view contents not using files.
	/// </summary>
	public abstract class AbstractViewContent : IViewContent
	{
		protected bool _isActive;

		protected bool _isSelected;

		protected bool _isVisible;

		protected string _title;

		protected LanguageDependentString _titleToBeLocalized;

		private string _infoTip;

		private LanguageDependentString _infoTipToBeLocalized;

		private ICommand _closeCommand;

		private bool _isDirty;

		private IServiceContainer _services;

		private bool _isDisposed;

		public event EventHandler IsDirtyChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		public virtual bool CloseWithSolution
		{
			get
			{
				return true;
			}
		}

		public abstract object ViewObject
		{
			get; set;
		}

		public virtual object InitiallyFocusedControl
		{
			get { return null; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view of this instance is active in the UI.
		/// </summary>
		/// <value>
		///   <c>true</c> if the view of this instance is active in the UI; otherwise, <c>false</c>.
		/// </value>
		public bool IsActive
		{
			get
			{
				return _isActive;
			}
			set
			{
				if (!(_isActive == value))
				{
					_isActive = value;
					OnPropertyChanged(nameof(IsActive));
				}
			}
		}

		/// <summary>
		/// Gets if the view content is read-only (can be saved only when choosing another file name).
		/// </summary>
		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view of this instance is selected (e.g. inside its container).
		/// </summary>
		/// <value>
		///   <c>true</c> if the view of this instance is selected; otherwise, <c>false</c>.
		/// </value>
		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				if (!(_isSelected == value))
				{
					_isSelected = value;
					OnPropertyChanged(nameof(IsSelected));
				}
			}
		}

		/// <summary>
		/// Gets if the view content is view-only (cannot be saved at all).
		/// </summary>
		public virtual bool IsViewOnly
		{
			get { return false; }
		}

		public bool IsVisible
		{
			get
			{
				return _isVisible;
			}
			set
			{
				if (!(_isVisible == value))
				{
					_isVisible = value;
					OnPropertyChanged(nameof(IsVisible));
				}
			}
		}

		#region Title

		/// <summary>
		/// Gets/Sets the title of the current tab page.
		/// This value will be passed through the string parser before being displayed.
		/// </summary>
		public virtual string Title
		{
			get
			{
				return _title ?? string.Empty;
			}
			set
			{
				if (null != _titleToBeLocalized)
				{
					_titleToBeLocalized.ValueChanged -= EhTitleLocalizationChanged;
					_titleToBeLocalized = null;
				}

				if (!(_title == value))
				{
					_title = value;
					OnPropertyChanged(nameof(Title));
				}
			}
		}

		public virtual string IconSource
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Sets a localized title that will update automatically when the language changes.
		/// </summary>
		/// <param name="text">The input to the string parser which will localize title.</param>
		protected virtual void SetLocalizedTitle(string text)
		{
			_titleToBeLocalized = new LanguageDependentString(text);
			_titleToBeLocalized.ValueChanged += EhTitleLocalizationChanged;
			EhTitleLocalizationChanged(_titleToBeLocalized, EventArgs.Empty);
		}

		protected virtual void EhTitleLocalizationChanged(object sender, EventArgs e)
		{
			var value = _titleToBeLocalized.Value;
			if (!(_title == value))
			{
				_title = value;
				OnPropertyChanged(nameof(Title));
			}
		}

		#endregion Title

		#region ContentId

		/// <summary>
		/// Gets or sets the content identifier. Here, the content identifier is calculated of the reference hash of the document.
		/// Setting is not implemented here.
		/// </summary>
		/// <value>
		/// The content identifier.
		/// </value>
		public virtual string ContentId
		{
			get
			{
				var model = this.ModelObject;
				if (model != null)
					return "ContentHash:" + RuntimeHelpers.GetHashCode(model).ToString(System.Globalization.CultureInfo.InvariantCulture);
				else
					return "ContentHash:Empty";
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion ContentId

		public virtual INavigationPoint BuildNavPoint()
		{
			return null;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#region CloseCommand

		public System.Windows.Input.ICommand CloseCommand
		{
			get
			{
				if (_closeCommand == null)
				{
					_closeCommand = Current.Gui.NewRelayCommand(OnClose, CanClose);
				}

				return _closeCommand;
			}
		}

		protected virtual bool CanClose()
		{
			return true;
		}

		protected virtual void OnClose()
		{
			Current.Workbench.CloseContent(this);
		}

		#endregion CloseCommand

		#region InfoTip

		public string InfoTip
		{
			get { return _infoTip; }
			set
			{
				if (null != _infoTipToBeLocalized)
				{
					_infoTipToBeLocalized.ValueChanged -= EhInfoTipLocalizationChanged;
					_infoTipToBeLocalized = null;
				}

				if (!(_infoTip == value))
				{
					_infoTip = value;
					OnPropertyChanged(nameof(InfoTip));
				}
			}
		}

		/// <summary>
		/// Sets a localized info tip that will update automatically when the language changes.
		/// </summary>
		/// <param name="text">The input to the string parser which will localize info tip.</param>
		protected void SetLocalizedInfoTip(string text)
		{
			_infoTipToBeLocalized = new LanguageDependentString(text);
			_infoTipToBeLocalized.ValueChanged += EhInfoTipLocalizationChanged;
			EhInfoTipLocalizationChanged(_infoTipToBeLocalized, EventArgs.Empty);
		}

		private void EhInfoTipLocalizationChanged(object sender, EventArgs e)
		{
			string value = _infoTipToBeLocalized.Value;
			if (!(_infoTip == value))
			{
				_infoTip = value;
				OnPropertyChanged(nameof(InfoTip));
			}
		}

		#endregion InfoTip

		#region IDisposable

		public event EventHandler Disposed;

		public bool IsDisposed
		{
			get { return _isDisposed; }
		}

		public virtual void Dispose()
		{
			_isDisposed = true;

			Disposed?.Invoke(this, EventArgs.Empty);
			OnPropertyChanged(nameof(IsDisposed));
		}

		#endregion IDisposable

		#region IsDirty

		public virtual bool IsDirty
		{
			get { return _isDirty; }
		}

		public virtual void OnIsDirtyChanged()
		{
			IsDirtyChanged?.Invoke(this, EventArgs.Empty);
			OnPropertyChanged(nameof(IsDirty));
		}

		public void ClearIsDirty()
		{
			if (!(_isDirty == false))
			{
				_isDirty = false;
				OnIsDirtyChanged();
			}
		}

		public void SetDirty()
		{
			if (!(_isDirty == true))
			{
				_isDirty = true;
				OnIsDirtyChanged();
			}
		}

		#endregion IsDirty

		#region IServiceProvider

		public IServiceContainer Services
		{
			get
			{
				return _services ?? (_services = new ServiceContainer());
			}
			protected set
			{
				_services = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		public abstract object ModelObject { get; }

		public T GetService<T>()
		{
			return (T)GetService(typeof(T));
		}

		public object GetService(Type serviceType)
		{
			object obj = _services?.GetService(serviceType);

			if (obj != null)
				return obj;

			if (serviceType.IsInstanceOfType(this))
				return this;

			return null;
		}

		#endregion IServiceProvider
	}
}