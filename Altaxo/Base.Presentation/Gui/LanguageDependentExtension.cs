using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Altaxo.Gui
{
	/// <summary>
	/// Base class of language dependent extensions like <see cref="LocalizeExtension"/> or <see cref="StringParseExtension"/>.
	/// </summary>
	/// <seealso cref="System.Windows.Markup.MarkupExtension" />
	/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
	/// <seealso cref="System.Windows.IWeakEventListener" />
	public abstract class LanguageDependentExtension : MarkupExtension, INotifyPropertyChanged, IWeakEventListener
	{
		protected LanguageDependentExtension()
		{
			this.UpdateOnLanguageChange = true;
		}

		public abstract string Value { get; }

		/// <summary>
		/// Set whether the LocalizeExtension should use a binding to automatically
		/// change the text on language changes.
		/// The default value is true.
		/// </summary>
		public bool UpdateOnLanguageChange { get; set; }

		private bool isRegisteredForLanguageChange;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (UpdateOnLanguageChange)
			{
				Binding binding = new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
				return binding.ProvideValue(serviceProvider);
			}
			else
			{
				return this.Value;
			}
		}

		private event System.ComponentModel.PropertyChangedEventHandler ChangedEvent;

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (!isRegisteredForLanguageChange)
				{
					isRegisteredForLanguageChange = true;
					LanguageChangeWeakEventManager.AddListener(this);
				}
				ChangedEvent += value;
			}
			remove { ChangedEvent -= value; }
		}

		private static readonly System.ComponentModel.PropertyChangedEventArgs
				valueChangedEventArgs = new System.ComponentModel.PropertyChangedEventArgs("Value");

		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			var handler = ChangedEvent;
			if (handler != null)
				handler(this, valueChangedEventArgs);
			return true;
		}
	}
}