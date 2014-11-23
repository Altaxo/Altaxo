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

namespace Altaxo.Graph.Scales
{
	using Altaxo.Data;
	using Ticks;

	/// <summary>
	/// Compound of a <see cref="Scale"/> and a <see cref="TickSpacing"/>.
	/// </summary>
	public class ScaleWithTicks : ICloneable, Main.IChangedEventSource, Main.IDocumentNode
	{
		private Scale _scale;
		private TickSpacing _tickSpacing;

		[field: NonSerialized]
		public event Action<Scale, Scale> ScaleInstanceChanged;

		[field: NonSerialized]
		public event EventHandler Changed;

		[NonSerialized]
		protected object _parentObject;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScaleWithTicks), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ScaleWithTicks s = (ScaleWithTicks)obj;

				info.AddValue("Scale", s._scale);
				info.AddValue("TickSpacing", s._tickSpacing);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScaleWithTicks s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual ScaleWithTicks SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScaleWithTicks s = null != o ? (ScaleWithTicks)o : new ScaleWithTicks();

				// Note: both functions should not trigger actions on scale or tickspacing,
				// so we use the settings function without triggering events
				s.SetNewScale((Scale)info.GetValue("Scale", s));
				s.SetNewTickSpacing((TickSpacing)info.GetValue("TickSpacing", s));

				s._tickSpacing.FinalProcessScaleBoundaries(s._scale.OrgAsVariant, s._scale.EndAsVariant, s._scale);

				return s;
			}
		}

		#endregion Serialization

		private ScaleWithTicks()
		{
		}

		public ScaleWithTicks(Scale scale)
		{
			SetNewScale(scale);
			SetNewTickSpacing(CreateDefaultTicks(scale.GetType()));
			UpdateIfTicksChanged();
		}

		public ScaleWithTicks(Scale scale, TickSpacing ticks)
		{
			SetNewScale(scale);
			SetNewTickSpacing(ticks);
			UpdateIfTicksChanged();
		}

		public ScaleWithTicks(ScaleWithTicks from)
		{
			SetNewScale(null == from._scale ? null : (Scale)from._scale.Clone());
			SetNewTickSpacing(null == from._tickSpacing ? null : (TickSpacing)from._tickSpacing.Clone());
		}

		public object Clone()
		{
			return new ScaleWithTicks(this);
		}

		public Scale Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				if (object.ReferenceEquals(_scale, value))
					return;

				Scale oldValue = SetNewScale(value);

				OnScaleInstanceChanged(oldValue, value);
				UpdateIfScaleChanged();
			}
		}

		private Scale SetNewScale(Scale value)
		{
			if (null != _scale)
			{
				_scale.Changed -= EhScaleChanged;
			}

			Scale oldValue = _scale;
			_scale = value;

			if (null != _scale)
			{
				_scale.Changed += EhScaleChanged;
				_scale.ParentObject = this;
			}

			return oldValue;
		}

		public TickSpacing TickSpacing
		{
			get
			{
				return _tickSpacing;
			}
			set
			{
				if (object.ReferenceEquals(_tickSpacing, value))
					return;

				SetNewTickSpacing(value);
				UpdateIfTicksChanged();
			}
		}

		private void SetNewTickSpacing(TickSpacing value)
		{
			if (null != _tickSpacing)
			{
				_tickSpacing.Changed -= EhTickSpacingChanged;
			}

			_tickSpacing = value;

			if (null != _tickSpacing)
			{
				_tickSpacing.Changed += EhTickSpacingChanged;
				_tickSpacing.ParentObject = this;
			}
		}

		public void SetTo(Scale scale, TickSpacing tickSpacing)
		{
			Scale oldScale = null;
			bool wasNewTickSpacing = false;

			if (!object.ReferenceEquals(_scale, scale))
				oldScale = SetNewScale(scale);

			if (!object.ReferenceEquals(_tickSpacing, tickSpacing))
			{
				wasNewTickSpacing = null == _tickSpacing || !_tickSpacing.Equals(tickSpacing);
				SetNewTickSpacing(tickSpacing);
			}

			if (null != oldScale) // then a new scale was used
			{
				OnScaleInstanceChanged(oldScale, _scale);
				UpdateIfScaleChanged();
			}
			else if (wasNewTickSpacing) // then new ticks were used
			{
				UpdateIfTicksChanged();
			}
		}

		protected virtual void OnScaleInstanceChanged(Scale oldValue, Scale newValue)
		{
			if (ScaleInstanceChanged != null)
				ScaleInstanceChanged(oldValue, newValue);
		}

		private void EhScaleChanged(object sender, EventArgs e)
		{
			UpdateIfScaleChanged();
		}

		private void EhTickSpacingChanged(object sender, EventArgs e)
		{
			UpdateIfTicksChanged();
		}

		private void UpdateIfTicksChanged()
		{
			if (null != _scale)
			{
				_scale.Rescale();
				UpdateIfScaleChanged();
			}
		}

		private void UpdateIfScaleChanged()
		{
			if (null != _tickSpacing && null != _scale)
			{
				AltaxoVariant org = _scale.OrgAsVariant;
				AltaxoVariant end = _scale.EndAsVariant;
				if (_tickSpacing.PreProcessScaleBoundaries(ref org, ref end, _scale.IsOrgExtendable, _scale.IsEndExtendable))
				{
					_scale.Changed -= EhScaleChanged; // suppress the changed event from scale

					// note: depending on the value of org and end, it might be that those values won't be accepted
					try
					{
						_scale.SetScaleOrgEnd(org, end);
					}
					catch (Exception)
					{
					}

					_scale.Changed += EhScaleChanged; // switch event from scale on again
				}
				_tickSpacing.FinalProcessScaleBoundaries(_scale.OrgAsVariant, _scale.EndAsVariant, _scale);
				OnChanged();
			}
		}

		protected void OnChanged()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		#region Static functions

		private static Dictionary<System.Type, SortedDictionary<int, System.Type>> _scaleToTickSpacingTypes = new Dictionary<Type, SortedDictionary<int, Type>>();

		public static void RegisterDefaultTicking(System.Type scaleType, System.Type tickSpacingType, int priority)
		{
			if (!_scaleToTickSpacingTypes.ContainsKey(scaleType))
				_scaleToTickSpacingTypes.Add(scaleType, new SortedDictionary<int, Type>());
			_scaleToTickSpacingTypes[scaleType].Add(priority, tickSpacingType);
		}

		public static TickSpacing CreateDefaultTicks(System.Type type)
		{
			if (_scaleToTickSpacingTypes.ContainsKey(type))
			{
				SortedDictionary<int, Type> dict = _scaleToTickSpacingTypes[type];

				foreach (KeyValuePair<int, System.Type> entry in dict)
					return (TickSpacing)System.Activator.CreateInstance(entry.Value);
			}

			return new NoTickSpacing();
		}

		static ScaleWithTicks()
		{
			RegisterDefaultTicking(typeof(DateTimeScale), typeof(DateTimeTickSpacing), 100);

			RegisterDefaultTicking(typeof(AngularDegreeScale), typeof(AngularDegreeTickSpacing), 100);

			RegisterDefaultTicking(typeof(AngularRadianScale), typeof(AngularRadianTickSpacing), 100);

			RegisterDefaultTicking(typeof(TextScale), typeof(TextTickSpacing), 100);

			RegisterDefaultTicking(typeof(Log10Scale), typeof(Log10TickSpacing), 100);

			RegisterDefaultTicking(typeof(LinearScale), typeof(LinearTickSpacing), 100);

			RegisterDefaultTicking(typeof(InverseScale), typeof(InverseTickSpacing), 100);
		}

		#endregion Static functions

		public object ParentObject
		{
			get
			{
				return _parentObject;
			}
			set
			{
				_parentObject = value;
			}
		}

		string Main.INamedObject.Name
		{
			get { throw new NotImplementedException(); }
		}
	}
}