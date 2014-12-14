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
	public class ScaleWithTicks
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		ICloneable
	{
		private Scale _scale;
		private TickSpacing _tickSpacing;

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
				s.InternalSetNewScaleSilently((Scale)info.GetValue("Scale", s));
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
			InternalSetNewScaleSilently(scale);
			SetNewTickSpacing(CreateDefaultTicks(scale.GetType()));
			UpdateIfTicksChanged();
		}

		public ScaleWithTicks(Scale scale, TickSpacing ticks)
		{
			InternalSetNewScaleSilently(scale);
			SetNewTickSpacing(ticks);
			UpdateIfTicksChanged();
		}

		public ScaleWithTicks(ScaleWithTicks from)
		{
			InternalSetNewScaleSilently(null == from._scale ? null : (Scale)from._scale.Clone());
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

				Scale oldValue = InternalSetNewScaleSilently(value);

				EhSelfChanged(new ScaleInstanceChangedEventArgs(oldValue, value));
				UpdateIfScaleChanged();
			}
		}

		private Scale InternalSetNewScaleSilently(Scale value)
		{
			if (null != _scale)
			{
				_scale.ParentObject = null;
			}

			Scale oldValue = _scale;
			_scale = value;

			if (null != _scale)
			{
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
				_tickSpacing.ParentObject = null;
			}

			_tickSpacing = value;

			if (null != _tickSpacing)
			{
				_tickSpacing.ParentObject = this;
			}
		}

		public void SetTo(Scale scale, TickSpacing tickSpacing)
		{
			Scale oldScale = null;
			bool wasNewTickSpacing = false;

			if (!object.ReferenceEquals(_scale, scale))
				oldScale = InternalSetNewScaleSilently(scale);

			if (!object.ReferenceEquals(_tickSpacing, tickSpacing))
			{
				wasNewTickSpacing = null == _tickSpacing || !_tickSpacing.Equals(tickSpacing);
				SetNewTickSpacing(tickSpacing);
			}

			if (null != oldScale) // then a new scale was used
			{
				EhSelfChanged(new ScaleInstanceChangedEventArgs(oldScale, _scale));
				UpdateIfScaleChanged();
			}
			else if (wasNewTickSpacing) // then new ticks were used
			{
				UpdateIfTicksChanged();
			}
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
					using (var suspendToken = _scale.SuspendGetToken())
					{
						try
						{
							// note: depending on the value of org and end, it might be that those values won't be accepted
							_scale.SetScaleOrgEnd(org, end);
						}
						catch (Exception)
						{
						}

						suspendToken.ResumeSilently();
					}
				}
				_tickSpacing.FinalProcessScaleBoundaries(_scale.OrgAsVariant, _scale.EndAsVariant, _scale);
				EhSelfChanged(EventArgs.Empty);
			}
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

		public override string Name
		{
			get { return "ScaleWithTicks"; }
			set
			{
				throw new NotImplementedException("Name cannot be set");
			}
		}

		#region Changed event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(_scale, sender))
				e = new ScaleWithTicksEventArgs() { ScaleChanged = true };
			else if (object.ReferenceEquals(_tickSpacing, sender))
				e = new ScaleWithTicksEventArgs() { TicksChanged = true };

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		protected override void OnChanged(EventArgs e)
		{
			var es = e as ScaleWithTicksEventArgs;
			if (null != es)
			{
				if (es.ScaleChanged)
					UpdateIfScaleChanged();
				if (es.TicksChanged)
					UpdateIfTicksChanged();
			}

			base.OnChanged(e);
		}

		internal class ScaleWithTicksEventArgs : Main.SelfAccumulateableEventArgs
		{
			internal bool ScaleChanged { get; set; }

			internal bool TicksChanged { get; set; }

			public override void Add(Main.SelfAccumulateableEventArgs e)
			{
				var other = e as ScaleWithTicksEventArgs;

				if (null == other)
					throw new ArgumentException("Expected event args of type: " + typeof(ScaleWithTicksEventArgs).ToString());

				ScaleChanged |= other.ScaleChanged;
				TicksChanged |= other.TicksChanged;
			}
		}

		#endregion Changed event handling
	}
}