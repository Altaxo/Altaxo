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

using Altaxo.Data;
using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Gdi.LabelFormatting
{
	/// <summary>
	/// Formatting of DateTime values.
	/// </summary>
	public class DateTimeLabelFormatting : MultiLineLabelFormattingBase
	{
		public enum TimeConversion { Original, ToUtc, ToLocal };

		private string _formatString = "{0:T}";
		private string _formatStringAlternate = "{0:T}\r\n{0:d}";

		private bool _showAlternateFormattingAtMidnight = true;
		private bool _showAlternateFormattingAtNoon;

		private TimeConversion _timeConversion = TimeConversion.Original;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimeLabelFormatting), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DateTimeLabelFormatting s = (DateTimeLabelFormatting)obj;
				info.AddBaseValueEmbedded(s, typeof(MultiLineLabelFormattingBase));

				info.AddEnum("TimeConversion", s._timeConversion);
				info.AddValue("FormatString", s._formatString);
				info.AddValue("ShowAlternateFormattingAtMidnight", s._showAlternateFormattingAtMidnight);
				info.AddValue("ShowAlternateFormattingAtNoon", s._showAlternateFormattingAtNoon);
				info.AddValue("FormatStringAlternate", s._formatStringAlternate);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DateTimeLabelFormatting s = (DateTimeLabelFormatting)o ?? new DateTimeLabelFormatting();
				info.GetBaseValueEmbedded(s, typeof(MultiLineLabelFormattingBase), parent);

				s._timeConversion = (DateTimeLabelFormatting.TimeConversion)info.GetEnum("TimeConversion", typeof(DateTimeLabelFormatting.TimeConversion));
				s._formatString = info.GetString("FormatString");
				s._showAlternateFormattingAtMidnight = info.GetBoolean("ShowAlternateFormattingAtMidnight");
				s._showAlternateFormattingAtNoon = info.GetBoolean("ShowAlternateFormattingAtNoon");
				s._formatStringAlternate = info.GetString("FormatStringAlternate");

				return s;
			}
		}

		#endregion Serialization

		public DateTimeLabelFormatting()
		{
		}

		public DateTimeLabelFormatting(DateTimeLabelFormatting from)
			: base(from) // everything is done here, since CopyFrom is virtual
		{
		}

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as DateTimeLabelFormatting;
				if (null != from)
				{
					_formatString = from._formatString;
					_formatStringAlternate = from._formatStringAlternate;
					_showAlternateFormattingAtMidnight = from._showAlternateFormattingAtMidnight;
					_showAlternateFormattingAtNoon = from._showAlternateFormattingAtNoon;
					_timeConversion = from._timeConversion;
				}
			}
			return isCopied;
		}

		public override object Clone()
		{
			return new DateTimeLabelFormatting(this);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		protected override string FormatItem(AltaxoVariant item)
		{
			if (item.IsType(AltaxoVariant.Content.VDateTime) && !string.IsNullOrEmpty(_formatString))
			{
				var dt = item.ToDateTime();

				switch (_timeConversion)
				{
					case TimeConversion.ToLocal:
						dt = dt.ToLocalTime();
						break;

					case TimeConversion.ToUtc:
						dt = dt.ToUniversalTime();
						break;
				}

				bool showAlternate = false;
				showAlternate |= (_showAlternateFormattingAtMidnight && Math.Abs(dt.TimeOfDay.TotalSeconds) < 1);
				showAlternate |= (_showAlternateFormattingAtNoon && Math.Abs((dt.TimeOfDay - TimeSpan.FromHours(12)).TotalSeconds) < 1);

				try
				{
					return string.Format(showAlternate ? _formatStringAlternate : _formatString, dt);
				}
				catch (Exception)
				{
				}
			}
			return item.ToString();
		}

		#region Properties

		public string FormattingString
		{
			get
			{
				return _formatString;
			}
			set
			{
				_formatString = value ?? string.Empty;
			}
		}

		public string FormattingStringAlternate
		{
			get
			{
				return _formatStringAlternate;
			}
			set
			{
				_formatStringAlternate = value ?? string.Empty;
			}
		}

		public bool ShowAlternateFormattingAtMidnight
		{
			get
			{
				return _showAlternateFormattingAtMidnight;
			}
			set
			{
				_showAlternateFormattingAtMidnight = value;
			}
		}

		public bool ShowAlternateFormattingAtNoon
		{
			get
			{
				return _showAlternateFormattingAtNoon;
			}
			set
			{
				_showAlternateFormattingAtNoon = value;
			}
		}

		public TimeConversion LabelTimeConversion
		{
			get
			{
				return _timeConversion;
			}
			set
			{
				_timeConversion = value;
			}
		}

		#endregion Properties
	}
}