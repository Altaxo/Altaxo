using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
namespace Altaxo.Settings
{
	/// <summary>
	/// Manages the settings for a culture, i.e. number and DateTime formats etc.
	/// </summary>
	public abstract class CultureSettingsBase : Main.ICopyFrom
	{
		/// <summary>Gets or sets a value indicating whether to override the parent culture or use it.</summary>
		protected bool _overrideParentCulture;
		
		/// <summary>Gets or sets the name of the culture (with region identifier).</summary>
		protected string _cultureName;
		
		/// <summary>Gets or sets the number decimal separator.</summary>
		protected string _numberDecimalSeparator;
		
		/// <summary>Gets or sets the number group separator.</summary>
		protected string _numberGroupSeparator;

		/// <summary>Initializes a new instance of the <see cref="UICultureSettings"/> class with nothing initialized.</summary>
		protected CultureSettingsBase()
		{
		}

		/// <summary>Copies from another instance.</summary>
		/// <param name="ffrom">Other instance to copy from.</param>
		/// <returns>True if data was copied from the other instance; otherwise false.</returns>
		public virtual bool CopyFrom(object ffrom)
		{
			var from = ffrom as CultureSettingsBase;
			if (null != from)
			{
				this._overrideParentCulture = from._overrideParentCulture;
				this._cultureName = from._cultureName;
				this._numberDecimalSeparator = from._numberDecimalSeparator;
				this._numberGroupSeparator = from._numberGroupSeparator;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>Set the members of this instance from a given culture.</summary>
		/// <param name="c">The culture to use as template.</param>
		public virtual void SetMembersFromCulture(CultureInfo c)
		{
			CultureName = c.Name;
			NumberDecimalSeparator = c.NumberFormat.NumberDecimalSeparator;
			NumberGroupSeparator = c.NumberFormat.NumberGroupSeparator;
		}

		/// <summary>Gets or sets a value indicating whether to override the parent culture or use it.</summary>
		/// <value> If <see langword="true"/>, the culture that is stored in this object is applied. Otherwise, the culture of the superior object (for instance the operating system) is used.</value>
		public virtual bool OverrideParentCulture
		{
			get { return _overrideParentCulture; }
			set
			{
				_overrideParentCulture = value;
				if (value == false)
					SetMembersFromCulture(OriginalCulture);
			}
		}

		/// <summary>Gets or sets the name of the culture.</summary>
		/// <value>The name of the culture in the format &lt;languagecode2&gt;-&lt;country/regioncode2&gt; (same format than in <see cref="P:System.Globalization.CultureInfo.Name"/>).</value>
		public string CultureName
		{
			get { return _cultureName; }
			set { _cultureName = value; }
		}

		/// <summary>Gets the neutral name of the culture, i.e. without region identifier..</summary>
		/// <value>The name of the neutral culture.</value>
		public string NeutralCultureName
		{
			get
			{
				var result = CultureName.Split(new char[] { '-' });
				return result[0];
			}
		}


		/// <summary>Gets or sets the number decimal separator.</summary>
		/// <value>The number decimal separator.</value>
		public string NumberDecimalSeparator
		{
			get { return _numberDecimalSeparator; }
			set { _numberDecimalSeparator = value; }
		}

		/// <summary>Gets or sets the number group separator.</summary>
		/// <value>The number group separator.</value>
		public string NumberGroupSeparator
		{
			get { return _numberGroupSeparator; }
			set { _numberGroupSeparator = value; }
		}

		/// <summary>Gets the parent culture, i.e. the culture set in the operating system settings.</summary>
		public abstract CultureInfo OriginalCulture	{	get; }

	
	}
}
