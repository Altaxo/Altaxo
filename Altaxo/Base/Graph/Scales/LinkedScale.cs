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
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using System;

namespace Altaxo.Graph.Scales
{
	[Serializable]
	public class LinkedScale : Scale
	{
		/// <summary>
		/// The (real) scale which is behind this fassade. Can not be another linked scale.
		/// It is recommended that the type of this scale matches the type of the scale linked to.
		/// In some rather rare cases the type of this scale may be different than the type of the scale linked to.
		/// </summary>
		private Scale _scaleWrapped;

		/// <summary>
		/// The _scale linked to. This scale normally is located in a sibling layer.
		/// </summary>
		private RelDocNodeProxy _scaleLinkedToProxy;

		/// <summary>
		/// Store the reference of the resolved scale linked to in order to detect when a new scale is resolved. If this is the case, the new scale must be tested for circular dependencies.
		/// </summary>
		private WeakReference _cachedResolvedScaleLinkedToWeak = new WeakReference(new object());

		/// <summary>
		/// Flag that indicated whether the latest resolved scaleLinkedTo was circular dependent on this scale. In this case the case is treated as if no Scale could be resolved.
		/// </summary>
		private bool _isScaleCircularDependend;

		private LinkedScaleParameters _linkParameters;

		#region Serialization

		[Obsolete]
		private int? _linkedScaleIndex;

		[Obsolete]
		private int? _linkedLayerIndex;

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.LinkedScale", 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old versions not supported.");
				/*
				LinkedScale s = (LinkedScale)obj;

				info.AddValue("ScaleWrapped", s._scaleWrapped);
				info.AddValue("LinkParameters", s._linkParameters);
				info.AddValue("LinkedScaleIndex", s._linkedScaleIndex);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinkedScale s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual LinkedScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinkedScale s = null != o ? (LinkedScale)o : new LinkedScale();

				s.WrappedScale = (Scale)info.GetValue("ScaleWrapped", s);

				s._linkParameters = (LinkedScaleParameters)info.GetValue("LinkParameters", s);
				if (null != s._linkParameters) s._linkParameters.ParentObject = s;

				s._linkedScaleIndex = info.GetInt32("LinkedScaleIndex");
				return s;
			}
		}

		[Obsolete]
		public void SetLinkedLayerIndex(int linkedLayerIndex, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			_linkedLayerIndex = linkedLayerIndex;
			info.DeserializationFinished += EhXmlDeserializationFinished_UseLinkedScaleAndLayerIndex;
		}

		[Obsolete]
		private void EhXmlDeserializationFinished_UseLinkedScaleAndLayerIndex(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
		{
			if (null != _linkedScaleIndex && null != _linkedLayerIndex)
			{
				int linkedScaleIndex = _linkedScaleIndex.Value;
				int linkedLayerIndex = _linkedLayerIndex.Value;

				// Retrieve the document
				Scale scale = null;
				var layer = AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.HostLayer>(this);
				if (null != layer)
				{
					var parentLayer = layer.ParentLayer;
					if (null != parentLayer)
					{
						var sibling = parentLayer.Layers[linkedLayerIndex] as Altaxo.Graph.Gdi.XYPlotLayer;
						if (null != sibling)
						{
							scale = sibling.Scales[linkedScaleIndex].Scale;
						}
					}
				}

				if (null != scale)
					this.ScaleLinkedTo = scale;
			}

			if (this.ScaleLinkedTo != null || isFinallyCall)
				info.DeserializationFinished -= EhXmlDeserializationFinished_UseLinkedScaleAndLayerIndex;
		}

		/// <summary>
		/// 2013-09-26 LinkedLayerIndex added, thus scale is now directly linked
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.LinkedScale", 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			/// <summary>Index of linked scale in the layer where the linked scale is located.</summary>
			private int _linkedScaleIndex;

			/// <summary>
			/// Number of the layer where the linked scale can be located. Note that the linked layer must have the same parent layer that the layer this scale belongs to.
			/// </summary>
			private int _linkedLayerIndex;

			private LinkedScale _instance;

			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old versions not supported.");

				/*

				LinkedScale s = (LinkedScale)obj;

				info.AddValue("ScaleWrapped", s._scaleWrapped);
				info.AddValue("LinkParameters", s._linkParameters);
				info.AddValue("LinkedScaleIndex", s._linkedScaleIndex);
				info.AddValue("LinkedLayerIndex", s._linkedLayerIndex);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinkedScale s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual LinkedScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinkedScale s = null != o ? (LinkedScale)o : new LinkedScale();

				s.WrappedScale = (Scale)info.GetValue("ScaleWrapped", s);
				s.LinkParameters = (LinkedScaleParameters)info.GetValue("LinkParameters", s);

				var surr = new XmlSerializationSurrogate3();
				surr._linkedScaleIndex = info.GetInt32("LinkedScaleIndex");
				surr._linkedLayerIndex = info.GetInt32("LinkedLayerIndex");
				surr._instance = s;

				// create a callback to resolve the instance as early as possible
				info.DeserializationFinished += surr.EhXmlDeserializationFinished;

				return s;
			}

			private void EhXmlDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
			{
				// Retrieve the document
				Scale scale = null;
				var layer = AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.HostLayer>(_instance);
				if (null != layer)
				{
					var parentLayer = layer.ParentLayer;
					if (null != parentLayer)
					{
						var sibling = parentLayer.Layers[_linkedLayerIndex] as Altaxo.Graph.Gdi.XYPlotLayer;
						if (null != sibling)
						{
							scale = sibling.Scales[_linkedScaleIndex].Scale;
						}
					}
				}

				if (null != scale || isFinallyCall)
				{
					_instance.ScaleLinkedTo = scale;
					info.DeserializationFinished -= EhXmlDeserializationFinished;
				}
			}
		}

		/// <summary>
		/// 2014-01-01 LinkedScale is now saved as a RelDocNodeProxy
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedScale), 4)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LinkedScale s = (LinkedScale)obj;

				info.AddValue("ScaleWrapped", s._scaleWrapped);
				info.AddValue("ScaleLinkedTo", s._scaleLinkedToProxy);
				info.AddValue("LinkParameters", s._linkParameters);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinkedScale s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual LinkedScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinkedScale s = null != o ? (LinkedScale)o : new LinkedScale();

				s.WrappedScale = (Scale)info.GetValue("ScaleWrapped", s);
				s.ChildSetMember(ref s._scaleLinkedToProxy, (Main.RelDocNodeProxy)info.GetValue("ScaleLinkedTo", s));
				s.LinkParameters = (LinkedScaleParameters)info.GetValue("LinkParameters", s);

				return s;
			}
		}

		#endregion Serialization

		private LinkedScale()
		{
		}

		public LinkedScale(Scale scaleToWrap)
		{
			_linkParameters = new LinkedScaleParameters() { ParentObject = this };
			WrappedScale = scaleToWrap;
		}

		[Obsolete("Only for old deserialization purposes")]
		public LinkedScale(Scale scaleToWrap, int scaleNumberLinkedTo)
		{
			_linkParameters = new LinkedScaleParameters() { ParentObject = this };
			WrappedScale = scaleToWrap;
			_linkedScaleIndex = scaleNumberLinkedTo;
		}

		private void CopyFrom(LinkedScale from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			ChildCopyToMember(ref _linkParameters, from._linkParameters);
			ChildCopyToMember(ref _scaleWrapped, from._scaleWrapped);
			ChildSetMember(ref _scaleLinkedToProxy, new RelDocNodeProxy(from._scaleLinkedToProxy, true, this));
			_cachedResolvedScaleLinkedToWeak.Target = from._cachedResolvedScaleLinkedToWeak.Target;
		}

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _scaleWrapped)
				yield return new Main.DocumentNodeAndName(_scaleWrapped, () => _scaleWrapped = null, "ScaleWrapped");

			if (null != _linkParameters)
				yield return new Main.DocumentNodeAndName(_linkParameters, () => _linkParameters = null, "LinkParameters");

			if (null != _scaleLinkedToProxy)
				yield return new Main.DocumentNodeAndName(_scaleLinkedToProxy, () => _scaleLinkedToProxy = null, "ScaleLinkedTo");
		}

		public override object Clone()
		{
			LinkedScale result = new LinkedScale();
			result.CopyFrom(this);
			return result;
		}

		public Scale ScaleLinkedTo
		{
			get
			{
				var scaleLinkedTo = _scaleLinkedToProxy != null ? (_scaleLinkedToProxy.Document as Scale) : null;
				if (null != scaleLinkedTo && !object.ReferenceEquals(scaleLinkedTo, _cachedResolvedScaleLinkedToWeak.Target)) // seems to be a newly resolved instance
				{
					// then we have to test whether this instance is circular dependent
					_cachedResolvedScaleLinkedToWeak = new WeakReference(scaleLinkedTo);
					// Test for circular references
					_isScaleCircularDependend = IsScaleDependentOnMe(scaleLinkedTo);
				}
				return _isScaleCircularDependend ? null : scaleLinkedTo; // if it is circular dependent, return null
			}
			set
			{
				if (object.ReferenceEquals(value, _scaleLinkedToProxy != null ? (_scaleLinkedToProxy.Document as Scale) : null))
					return;

				if (ChildSetMember(ref _scaleLinkedToProxy, new RelDocNodeProxy(value, this)))
				{
					OnLinkPropertiesChanged(); // calculated the bounds of the wrapped scale
				}
			}
		}

		/// <summary>
		/// Checks if the scale in the argument is dependend on this Scale. This would mean a circular dependency, which should be avoided.
		/// </summary>
		/// <param name="scaleToTest">The scale to check.</param>
		/// <returns>True if the provided scale or one of its linked scales is dependend on this scale.</returns>
		public bool IsScaleDependentOnMe(Scale scaleToTest)
		{
			return WouldScaleBeDependentOnMe(this, scaleToTest);
		}

		/// <summary>
		/// Checks if the scale in the argument is dependend on this Scale. This would mean a circular dependency, which should be avoided.
		/// </summary>
		/// <param name="scaleThatWouldBecomeALinkedScale">Scale that is a linked scale or that would become a linked scale.</param>
		/// <param name="scaleToTest">The scale to check.</param>
		/// <returns>True if the provided scale or one of its linked scales is dependend on this scale.</returns>
		public static bool WouldScaleBeDependentOnMe(Scale scaleThatWouldBecomeALinkedScale, Scale scaleToTest)
		{
			if (object.ReferenceEquals(scaleThatWouldBecomeALinkedScale, scaleToTest))
				return true; // Scale are identical, thus they are really dependent on each other

			var linkedScale = scaleToTest as LinkedScale;
			while (null != linkedScale)
			{
				if (object.ReferenceEquals(scaleThatWouldBecomeALinkedScale, linkedScale))
					return true;	// this means a direct circular dependency (we are at the original scale), so return true

				if (object.ReferenceEquals(scaleThatWouldBecomeALinkedScale, linkedScale.ScaleLinkedTo))
					return true; // That would mean a circular dependency also, thus also return true;

				linkedScale = linkedScale.ScaleLinkedTo as LinkedScale;
			}
			return false; // no dependency detected
		}

		public bool IsStraightLink
		{
			get
			{
				return _linkParameters.IsStraightLink;
			}
		}

		/// <summary>
		/// Set all parameters of the axis link by once.
		/// </summary>
		/// <param name="orgA">The value a of x-axis link for link of axis origin: org' = a + b*org.</param>
		/// <param name="orgB">The value b of x-axis link for link of axis origin: org' = a + b*org.</param>
		/// <param name="endA">The value a of x-axis link for link of axis end: end' = a + b*end.</param>
		/// <param name="endB">The value b of x-axis link for link of axis end: end' = a + b*end.</param>
		public void SetLinkParameter(double orgA, double orgB, double endA, double endB)
		{
			_linkParameters.SetTo(orgA, orgB, endA, endB);
		}

		public void SetLinkParameterToStraightLink()
		{
			_linkParameters.SetToStraightLink();
		}

		public LinkedScaleParameters LinkParameters
		{
			get
			{
				return _linkParameters;
			}
			set
			{
				if (ChildSetMember(ref _linkParameters, value ?? new LinkedScaleParameters()))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public double LinkOrgA
		{
			get { return _linkParameters.OrgA; }
			set
			{
				_linkParameters.OrgA = value;
			}
		}

		public double LinkOrgB
		{
			get { return _linkParameters.OrgB; }
			set
			{
				_linkParameters.OrgB = value;
			}
		}

		public double LinkEndA
		{
			get { return _linkParameters.EndA; }
			set
			{
				_linkParameters.EndA = value;
			}
		}

		public double LinkEndB
		{
			get { return _linkParameters.EndB; }
			set
			{
				_linkParameters.EndB = value;
				OnLinkPropertiesChanged();
			}
		}

		public Scale WrappedScale
		{
			get
			{
				return _scaleWrapped;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				if (ChildSetMember(ref _scaleWrapped, value))
				{
					OnLinkPropertiesChanged();
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
		{
			return _scaleWrapped.PhysicalVariantToNormal(x);
		}

		public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
		{
			return _scaleWrapped.NormalToPhysicalVariant(x);
		}

		public override object RescalingObject
		{
			get { return _scaleWrapped.RescalingObject; }
		}

		public override IPhysicalBoundaries DataBoundsObject
		{
			get
			{
				var scaleLinkedTo = ScaleLinkedTo;

				if (object.ReferenceEquals(this, scaleLinkedTo))
					throw new InvalidProgramException("_scaleLinkedTo is reference equal to this scale. Please report this error to the forum");

				// it is not possible for a this scale to act back to the scale which is linked
				// but to make the plot items influence the range of the linked scale we can give back
				// the data bounds object of the linked scale

				return scaleLinkedTo == null ? _scaleWrapped.DataBoundsObject : scaleLinkedTo.DataBoundsObject;
			}
		}

		public override Altaxo.Data.AltaxoVariant OrgAsVariant
		{
			get
			{
				return _scaleWrapped.OrgAsVariant;
			}
		}

		public override Altaxo.Data.AltaxoVariant EndAsVariant
		{
			get
			{
				return _scaleWrapped.EndAsVariant;
			}
		}

		/// <summary>Returns true if it is allowed to extend the origin (to lower values).</summary>
		public override bool IsOrgExtendable
		{
			get
			{
				var scaleLinkedTo = ScaleLinkedTo;
				return null == scaleLinkedTo ? false : scaleLinkedTo.IsOrgExtendable;
			}
		}

		/// <summary>Returns true if it is allowed to extend the scale end (to higher values).</summary>
		public override bool IsEndExtendable
		{
			get
			{
				var scaleLinkedTo = ScaleLinkedTo;
				return null == scaleLinkedTo ? false : scaleLinkedTo.IsEndExtendable;
			}
		}

		public override void OnUserRescaled()
		{
			var scaleLinkedTo = ScaleLinkedTo;

			if (null != scaleLinkedTo)
				scaleLinkedTo.OnUserRescaled();
		}

		public override void OnUserZoomed(AltaxoVariant newZoomOrg, AltaxoVariant newZoomEnd)
		{
			// ignore this - we are linked
		}

		public override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
		{
			var scaleLinkedTo = ScaleLinkedTo;

			if (null != scaleLinkedTo)
			{
				if (!IsStraightLink)
				{
					org = (org - LinkOrgA) / LinkOrgB;
					end = (end - LinkEndA) / LinkEndB;
				}
				return scaleLinkedTo.SetScaleOrgEnd(org, end);
			}
			return null;
		}

		#region Changed event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(sender, _linkParameters))
			{
				OnLinkPropertiesChanged();
			}
			else if (object.ReferenceEquals(sender, _scaleLinkedToProxy))
			{
				OnLinkPropertiesChanged();
			}

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		private void EhLinkedScaleChanged(object sender, EventArgs e)
		{
			OnLinkPropertiesChanged();
		}

		protected virtual void OnLinkPropertiesChanged()
		{
			// calculate the new bounds
			var scaleLinkedTo = this.ScaleLinkedTo;
			if (null != scaleLinkedTo)
			{
				AltaxoVariant org = scaleLinkedTo.OrgAsVariant;
				AltaxoVariant end = scaleLinkedTo.EndAsVariant;

				if (!IsStraightLink)
				{
					org = org * LinkOrgB + LinkOrgA;
					end = end * LinkEndB + LinkEndA;
				}

				_scaleWrapped.SetScaleOrgEnd(org, end);
			}
		}

		#endregion Changed event handling
	}
}