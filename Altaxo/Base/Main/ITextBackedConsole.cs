using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Represents a storage for text that can be used like a text console. 
	/// </summary>
	public interface ITextBackedConsole : System.ComponentModel.INotifyPropertyChanged, ICloneable
	{
		/// <summary>Writes the specified string value to the text backed console.</summary>
		/// <param name="value">The string to write. </param>
		void Write(string value);

		/// <summary>Writes the text representation of the specified array of objects to the text backed console using the specified format information.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An array of objects to write using <paramref name="format" />. </param>
		void Write(string format, params object[] args);

		/// <summary>Writes the current line terminator to the text backed console.</summary>
		void WriteLine();

		/// <summary>Writes the specified string value, followed by the current line terminator, to the text backed console.</summary>
		/// <param name="value">The value to write. </param>
		void WriteLine(string value);


		/// <summary>Writes the text representation of the specified array of objects, followed by the current line terminator, to the text backed console using the specified format information.</summary>
		/// <param name="format">A composite format string. </param>
		/// <param name="args">An array of objects to write using <paramref name="format" />. </param>
		void WriteLine(string format, params object[] args);

		/// <summary>Removes all characters from the current text backed console.</summary>
		void Clear();


		/// <summary>Gets or sets the entire text of the text backed console. If setting the text, and if the text is different from the text that is currently stored in the instance, a property changed event (see <see cref="System.ComponentModel.PropertyChangedEventHandler"/>) is fired with 'Text' as parameter.</summary>
		/// <value>The text of this console.</value>
		string Text { get; set; }
	}


	/// <summary>
	/// Implementation of <see cref="ITextBackedConsole"/>, where the text is stored in a <see cref="System.Text.StringBuilder"/> instance.
	/// </summary>
	public class TextBackedConsole : ITextBackedConsole
	{
		static readonly System.ComponentModel.PropertyChangedEventArgs _textChangedPropertyEventArgs = new System.ComponentModel.PropertyChangedEventArgs("Text");

		object _synchronizingObject;
		StringBuilder _stb;

		/// <summary>
		/// Occurs when a property value changes. Here, it is fired when the <see cref="Text"/> property changed.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="TextBackedConsole"/> class with empty text.
		/// </summary>
		public TextBackedConsole()
		{
			Console.Write("aa");
			_synchronizingObject = new object();
			_stb = new StringBuilder();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextBackedConsole"/> class. The text is copied from the provided text backed console.
		/// </summary>
		/// <param name="from">The instance to copy the text from.</param>
		public TextBackedConsole(TextBackedConsole from)
		{
			_synchronizingObject = new object();
			_stb = new StringBuilder(from._stb.ToString());
		}

		object ICloneable.Clone()
		{
			return new TextBackedConsole(this);
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>The cloned instance.</returns>
		public TextBackedConsole Clone()
		{
			return new TextBackedConsole(this);
		}

		public void Write(string value)
		{
			_stb.Append(value);
			OnTextChanged();
		}


		/// <summary>
		/// Writes the text representation of the specified array of objects to the text backed console using the specified format information.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
		public void Write(string format, params object[] args)
		{
			_stb.AppendFormat(format, args);
			OnTextChanged();
		}


		/// <summary>
		/// Writes the current line terminator to the text backed console.
		/// </summary>
		public void WriteLine()
		{
			_stb.AppendLine();
			OnTextChanged();
		}

		/// <summary>
		/// Writes the specified string value, followed by the current line terminator, to the text backed console.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public void WriteLine(string value)
		{
			_stb.AppendLine(value);
			OnTextChanged();
		}


		/// <summary>
		/// Writes the text representation of the specified array of objects, followed by the current line terminator, to the text backed console using the specified format information.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
		public void WriteLine(string format, params object[] args)
		{
			_stb.AppendFormat(format, args);
			_stb.AppendLine();
			OnTextChanged();
		}


		/// <summary>
		/// Removes all characters from the current text backed console.
		/// </summary>
		public void Clear()
		{
			var count = _stb.Length;
			_stb.Clear();

			if (count != 0)
				OnTextChanged();
		}

		/// <summary>
		/// Gets or sets the entire text of the text backed console. If setting the text, and if the text is different from the text that is currently stored in the instance, a property changed event (see <see cref="System.ComponentModel.PropertyChangedEventHandler"/>) is fired with 'Text' as parameter.
		/// </summary>
		/// <value>
		/// The text of this console.
		/// </value>
		public string Text
		{
			get
			{
				return _stb.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					Clear();
					return;
				}


				var isDifferent = _stb.Length != value.Length || 0 != string.CompareOrdinal(value, _stb.ToString());
				if (isDifferent)
				{
					lock (_synchronizingObject)
					{
						_stb.Clear();
						_stb.Append(value);
					}
					OnTextChanged();
				}
			}
		}

		/// <summary>
		/// Should be called internally when the text has changed.
		/// </summary>
		protected virtual void OnTextChanged()
		{
			var pc = PropertyChanged;
			if (null != pc)
				pc(this, _textChangedPropertyEventArgs);
		}






	}
}
