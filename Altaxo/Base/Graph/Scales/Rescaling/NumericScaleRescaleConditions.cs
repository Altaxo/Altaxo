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

namespace Altaxo.Graph.Scales.Rescaling
{
	using Altaxo.Graph.Scales.Boundaries;

	/// <summary>
	/// Determines the behaviour of the axis when some of the data has changed.
	/// </summary>
	[Serializable]
	public abstract class NumericScaleRescaleConditions
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IUnboundNumericScaleRescaleConditions
	{
		#region InnerClasses

		/// <summary>
		/// Old boundary rescaling until 2015-02
		/// </summary>
		public enum BoundaryRescalingV1
		{
			/// <summary>
			/// Scale this boundary so that the data fits.
			/// </summary>
			Auto = 0,

			/// <summary>
			/// This axis boundary is set to a fixed value.
			/// </summary>
			Fixed = 1,

			/// <summary>
			/// This axis boundary is set to a fixed value.
			/// </summary>
			Equal = 1,

			/// <summary>
			/// The axis boundary is set to fit the data, but is set not greater than a certain value.
			/// </summary>
			NotGreater = 2,

			/// <summary>
			/// The axis boundary is set to fit the data, but is set not greater than a certain value.
			/// </summary>
			LessOrEqual = 2,

			/// <summary>
			/// The axis boundary is set to fit the data, but is set not lesser than a certain value.
			/// </summary>
			GreaterOrEqual = 3,

			/// <summary>
			/// The axis boundary is set to fit the data, but is set not lesser than a certain value.
			/// </summary>
			NotLess = 3,

			/// <summary>
			/// The axis boundary is set to use the span from the other axis boundary.
			/// </summary>
			UseSpan = 4,
		}

		#endregion InnerClasses

		// User provided parameters

		protected BoundaryRescaling _orgRescaling;
		protected BoundaryRescaling _endRescaling;

		protected BoundariesRelativeTo _userProvidedOrgRelativeTo;
		protected BoundariesRelativeTo _userProvidedEndRelativeTo;

		protected double _userProvidedOrgValue;
		protected double _userProvidedEndValue;

		protected double _dataBoundsOrg = 1;
		protected double _dataBoundsEnd = 2;

		// Results

		protected double _resultingOrg = 1;
		protected double _resultingEnd = 2;
		protected double _resultingMinOrg = UnboundMinOrg;
		protected double _resultingMaxEnd = UnboundMaxEnd;

		private const double UnboundMinOrg = double.NegativeInfinity;
		private const double UnboundMaxEnd = double.PositiveInfinity;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.Scaling.NumericAxisRescaleConditions", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.Rescaling.NumericAxisRescaleConditions", 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");

				/*
				NumericAxisRescaleConditions s = (NumericAxisRescaleConditions)obj;
				info.AddEnum("OrgRescaling", s._orgRescaling);
				info.AddValue("Org", s._org);
				info.AddEnum("EndRescaling", s._endRescaling);
				info.AddValue("End", s._end);
				info.AddEnum("SpanRescaling", s._spanRescaling);
				info.AddValue("Span", s._span);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				NumericScaleRescaleConditions s = null != o ? (NumericScaleRescaleConditions)o : new LinearScaleRescaleConditions();

				s._userProvidedOrgRelativeTo = BoundariesRelativeTo.Absolute;
				s._userProvidedEndRelativeTo = BoundariesRelativeTo.Absolute;

				var orgRescaling = (BoundaryRescaling)(int)info.GetEnum("OrgRescaling", typeof(BoundaryRescalingV1));
				var org = (double)info.GetDouble("Org");
				var endRescaling = (BoundaryRescaling)(int)info.GetEnum("EndRescaling", typeof(BoundaryRescalingV1));
				var end = (double)info.GetDouble("End");
				var spanRescaling = (BoundaryRescaling)(int)info.GetEnum("SpanRescaling", typeof(BoundaryRescalingV1));
				var span = (double)info.GetDouble("Span");

				if (4 == (int)orgRescaling)
					orgRescaling = BoundaryRescaling.Auto;
				if (4 == (int)endRescaling)
					endRescaling = BoundaryRescaling.Auto;

				s._orgRescaling = orgRescaling;
				s._endRescaling = endRescaling;
				s._userProvidedOrgValue = org;
				s._userProvidedEndValue = end;

				s._resultingOrg = org;
				s._resultingEnd = end;

				return s;
			}
		}

		/// <summary>
		/// 2015-10-02 added RelativeTo parameters.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericScaleRescaleConditions), 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (NumericScaleRescaleConditions)obj;

				// Cached values
				info.AddValue("DataBoundsOrg", s._dataBoundsOrg);
				info.AddValue("DataBoundsEnd", s._dataBoundsEnd);

				// User provided values
				info.AddEnum("OrgRescaling", s._orgRescaling);
				info.AddEnum("EndRescaling", s._endRescaling);
				info.AddEnum("OrgRelativeTo", s._userProvidedOrgRelativeTo);
				info.AddEnum("EndRelativeTo", s._userProvidedEndRelativeTo);
				info.AddValue("UserProvidedOrg", s._userProvidedOrgValue);
				info.AddValue("UserProvidedEnd", s._userProvidedEndValue);

				// Final result
				info.AddValue("ResultingOrg", s._resultingOrg);
				info.AddValue("ResultingMinOrg", s._resultingMinOrg);
				info.AddValue("ResultingEnd", s._resultingEnd);
				info.AddValue("ResultingMaxEnd", s._resultingMaxEnd);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (NumericScaleRescaleConditions)o;

				// Cached values
				s._dataBoundsOrg = (double)info.GetDouble("DataBoundsOrg");
				s._dataBoundsEnd = (double)info.GetDouble("DataBoundsEnd");

				// User provided values
				s._orgRescaling = (BoundaryRescaling)info.GetEnum("OrgRescaling", typeof(BoundaryRescaling));
				s._endRescaling = (BoundaryRescaling)info.GetEnum("EndRescaling", typeof(BoundaryRescaling));
				s._userProvidedOrgRelativeTo = (BoundariesRelativeTo)info.GetEnum("OrgRelativeTo", typeof(BoundariesRelativeTo));
				s._userProvidedEndRelativeTo = (BoundariesRelativeTo)info.GetEnum("EndRelativeTo", typeof(BoundariesRelativeTo));
				s._userProvidedOrgValue = (double)info.GetDouble("UserProvidedOrg");
				s._userProvidedEndValue = (double)info.GetDouble("UserProvidedEnd");

				// Final result
				s._resultingOrg = info.GetDouble("ResultingOrg");
				s._resultingMinOrg = info.GetDouble("ResultingMinOrg");
				s._resultingEnd = info.GetDouble("ResultingEnd");
				s._resultingMaxEnd = info.GetDouble("ResultingMaxEnd");

				return s;
			}
		}

		#endregion Serialization

		public NumericScaleRescaleConditions()
		{
		}

		public NumericScaleRescaleConditions(NumericScaleRescaleConditions from)
		{
			CopyFrom(from);
		}

		/// <summary>Copies from another instance.</summary>
		/// <param name="obj">The other instance to copy from.</param>
		/// <returns>True if at least some data could be copied.</returns>
		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as NumericScaleRescaleConditions;
			if (null == from)
				return false;

			this._orgRescaling = from._orgRescaling;
			this._endRescaling = from._endRescaling;

			this._userProvidedOrgRelativeTo = from._userProvidedOrgRelativeTo;
			this._userProvidedEndRelativeTo = from._userProvidedEndRelativeTo;

			this._userProvidedOrgValue = from._userProvidedOrgValue;
			this._userProvidedEndValue = from._userProvidedEndValue;

			this._dataBoundsOrg = from._dataBoundsOrg;
			this._dataBoundsEnd = from._dataBoundsEnd;

			this._resultingOrg = from._resultingOrg;
			this._resultingEnd = from._resultingEnd;

			this._resultingMinOrg = from._resultingMinOrg;
			this._resultingMaxEnd = from._resultingMaxEnd;

			EhSelfChanged(EventArgs.Empty);

			return true;
		}

		public abstract object Clone();

		#region public properties

		public virtual double ResultingOrg { get { return _resultingOrg; } }

		public virtual double ResultingEnd { get { return _resultingEnd; } }

		public bool IsResultingOrgFixed { get { return _resultingOrg == _resultingMinOrg; } }

		public bool IsResultingEndFixed { get { return _resultingEnd == _resultingMaxEnd; } }

		#endregion public properties

		public virtual void SetUserParameters(BoundaryRescaling orgRescaling, double orgValue, BoundaryRescaling endRescaling, double endValue)
		{
			SetUserParameters(orgRescaling, BoundariesRelativeTo.Absolute, orgValue, endRescaling, BoundariesRelativeTo.Absolute, endValue);
		}

		public virtual void SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, double orgValue, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, double endValue)
		{
			bool isChange =

			_orgRescaling != orgRescaling ||
			_userProvidedOrgRelativeTo != orgRelativeTo ||
			_userProvidedOrgValue != orgValue ||
			_endRescaling != endRescaling ||
			_userProvidedEndRelativeTo != endRelativeTo ||
			_userProvidedEndValue != endValue;

			_orgRescaling = orgRescaling;
			_userProvidedOrgRelativeTo = orgRelativeTo;
			_userProvidedOrgValue = orgValue;

			_endRescaling = endRescaling;
			_userProvidedEndRelativeTo = endRelativeTo;
			_userProvidedEndValue = endValue;

			if (isChange)
			{
				ProcessOrg_UserParametersChanged();
				ProcessEnd_UserParametersChanged();
				EhSelfChanged();
			}
		}

		#region Accessors

		public BoundaryRescaling OrgRescaling
		{
			get
			{
				return _orgRescaling;
			}
		}

		public BoundaryRescaling EndRescaling
		{
			get
			{
				return _endRescaling;
			}
		}

		public BoundariesRelativeTo OrgRelativeTo { get { return _userProvidedOrgRelativeTo; } }

		public BoundariesRelativeTo EndRelativeTo { get { return _userProvidedEndRelativeTo; } }

		public virtual double UserProvidedOrgValue { get { return _userProvidedOrgValue; } }

		public virtual double UserProvidedEndValue { get { return _userProvidedEndValue; } }

		#endregion Accessors

		#region Event handling

		public void OnUserZoomed(double newZoomOrg, double newZoomEnd)
		{
			if (!(newZoomOrg < newZoomEnd))
				throw new ArgumentOutOfRangeException("zoomOrg should be less than zoomEnd");

			var oldResultingOrg = _resultingOrg;
			var oldResultingEnd = _resultingEnd;
			var oldResultingMinOrg = _resultingMinOrg;
			var oldResultingMaxEnd = _resultingMaxEnd;

			ProcessOrg_UserZoomed(newZoomOrg);
			ProcessEnd_UserZoomed(newZoomEnd);

			if (
				oldResultingOrg != _resultingOrg ||
				oldResultingEnd != _resultingEnd ||
				oldResultingMinOrg != _resultingMinOrg ||
				oldResultingMaxEnd != _resultingMaxEnd
				)
			{
				EhSelfChanged();
			}
		}

		public void OnUserRescaled()
		{
			var oldResultingOrg = _resultingOrg;
			var oldResultingEnd = _resultingEnd;
			var oldResultingMinOrg = _resultingMinOrg;
			var oldResultingMaxEnd = _resultingMaxEnd;

			ProcessOrg_UserRescaled();
			ProcessEnd_UserRescaled();

			if (
				oldResultingOrg != _resultingOrg ||
				oldResultingEnd != _resultingEnd ||
				oldResultingMinOrg != _resultingMinOrg ||
				oldResultingMaxEnd != _resultingMaxEnd
				)
			{
				EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets the mean value of the data bounds. We use the 'scale' mean, i.e. the physical value of the scale where its logical value is 0.5.
		/// </summary>
		/// <returns></returns>
		protected abstract double GetDataBoundsScaleMean();

		/// <summary>
		/// Fixes the data bounds org and end. Here you have to handle special cases, like org and end are equal. At return, org should be strictly less than end.
		/// </summary>
		/// <param name="dataBoundsOrg">The data bounds org.</param>
		/// <param name="dataBoundsEnd">The data bounds end.</param>
		protected abstract void FixValuesForDataBoundsOrgAndEnd(ref double dataBoundsOrg, ref double dataBoundsEnd);

		/// <summary>
		/// Fixes the values when the user zoomed. For instance, if org is greater then end, both values are interchanged.
		/// </summary>
		/// <param name="zoomOrg">The zoom org.</param>
		/// <param name="zoomEnd">The zoom end.</param>
		protected abstract void FixValuesForUserZoomed(ref double zoomOrg, ref double zoomEnd);

		/// <summary>
		/// Announces a change of the data bounds of the set of data belonging to a scale.
		/// </summary>
		/// <param name="dataBoundsOrg">The one side of the data bounds.</param>
		/// <param name="dataBoundsEnd">The other side of the data bounds.</param>
		/// <returns>True if the provided data bounds resulted in changed orgs and ends of this object; otherwise, false.</returns>
		/// <exception cref="ArgumentOutOfRangeException">dataBoundsOrg should be less than dataBoundsEnd</exception>
		public bool OnDataBoundsChanged(double dataBoundsOrg, double dataBoundsEnd)
		{
			if (!(dataBoundsOrg <= dataBoundsEnd))
				throw new ArgumentOutOfRangeException("dataBoundsOrg should be less than dataBoundsEnd");

			var oldResultingOrg = _resultingOrg;
			var oldResultingEnd = _resultingEnd;
			var oldResultingMinOrg = _resultingMinOrg;
			var oldResultingMaxEnd = _resultingMaxEnd;

			FixValuesForDataBoundsOrgAndEnd(ref dataBoundsOrg, ref dataBoundsEnd);

			_dataBoundsOrg = dataBoundsOrg;
			_dataBoundsEnd = dataBoundsEnd;

			ProcessOrg_DataBoundsChanged();
			ProcessEnd_DataBoundsChanged();

			var hasChanged =
				_resultingOrg != oldResultingOrg ||
				_resultingEnd != oldResultingEnd ||
				_resultingMinOrg != oldResultingMinOrg ||
				_resultingMaxEnd != oldResultingMaxEnd;

			if (hasChanged)
				EhSelfChanged();

			return hasChanged;
		}

		#endregion Event handling

		#region Resulting Org/End to/fron User Org/End

		protected abstract double GetResultingOrgFromUserProvidedOrg();

		protected abstract double GetUserProvidedOrgFromResultingOrg(double resultingOrg);

		//		protected abstract double GetResultingOrgFromDataBoundsOrg();

		protected abstract double GetResultingEndFromUserProvidedEnd();

		protected abstract double GetUserProvidedEndFromResultingEnd(double resultingEnd);

		//		protected abstract double GetResultingEndFromDataBoundsEnd();

		#endregion Resulting Org/End to/fron User Org/End

		#region Process Org End

		protected void ProcessOrg_DataBoundsChanged()
		{
			var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

			switch (_orgRescaling)
			{
				case BoundaryRescaling.Fixed:
				case BoundaryRescaling.FixedManually:
				case BoundaryRescaling.FixedZoomable:
					_resultingOrg = resultingUserProvidedOrgValue;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_resultingMinOrg = _resultingOrg; // Strictly fixed
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_resultingMinOrg = resultingUserProvidedOrgValue; // not fixed because resultingOrg can go further down to resultingUserProvidedOrgValue
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_resultingMinOrg = UnboundMinOrg; // Not fixed
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_resultingMinOrg = UnboundMinOrg; // Not fixed
					}
					break;

				case BoundaryRescaling.Auto:
					_resultingOrg = _dataBoundsOrg;
					_resultingMinOrg = UnboundMinOrg; // Not fixed
					break;

				case BoundaryRescaling.AutoTempFixed:
					_orgRescaling = BoundaryRescaling.Auto;
					_resultingOrg = _dataBoundsOrg;
					_resultingMinOrg = UnboundMinOrg; // Not fixed
					break;
			}
		}

		protected void ProcessOrg_UserRescaled()
		{
			var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

			switch (_orgRescaling)
			{
				case BoundaryRescaling.Auto:
					_resultingOrg = _dataBoundsOrg;
					_resultingMinOrg = UnboundMinOrg; // Not fixed
					break;

				case BoundaryRescaling.AutoTempFixed:
					_orgRescaling = BoundaryRescaling.Auto; // Fall back to Auto rescaling
					_resultingOrg = _dataBoundsOrg;
					_resultingMinOrg = UnboundMinOrg; // Not fixed
					break;

				case BoundaryRescaling.Fixed:
					_resultingOrg = resultingUserProvidedOrgValue;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					break;

				case BoundaryRescaling.FixedManually:
					// TODO ask for switching to AutoTemp or AutoTempFixed
					// HERE as long as fixed manually, we treat this as fixed
					_resultingOrg = resultingUserProvidedOrgValue;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					break;

				case BoundaryRescaling.FixedZoomable: // treat as fixed
					_resultingOrg = resultingUserProvidedOrgValue;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_resultingMinOrg = UnboundMinOrg; // Not fixed
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_resultingMinOrg = UnboundMinOrg; // Not fixed
					}
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_resultingMinOrg = _resultingOrg; // fixed
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_resultingMinOrg = resultingUserProvidedOrgValue; // not fixed till resultingUserProvidedOrg
					}
					break;
			}
		}

		protected void ProcessOrg_UserZoomed(double zoomValueOrg)
		{
			var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

			switch (_orgRescaling)
			{
				case BoundaryRescaling.Auto:
					_orgRescaling = BoundaryRescaling.AutoTempFixed;
					_resultingOrg = zoomValueOrg;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					_userProvidedOrgValue = GetUserProvidedOrgFromResultingOrg(_resultingOrg);
					break;

				case BoundaryRescaling.AutoTempFixed:
					_resultingOrg = zoomValueOrg;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					_userProvidedOrgValue = GetUserProvidedOrgFromResultingOrg(_resultingOrg);
					break;

				case BoundaryRescaling.Fixed:
					// Ignore zoom
					break;

				case BoundaryRescaling.FixedManually:
					// use the new values as user provided values
					_resultingOrg = zoomValueOrg;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					_userProvidedOrgValue = GetUserProvidedOrgFromResultingOrg(zoomValueOrg);
					break;

				case BoundaryRescaling.FixedZoomable:
					// use the new values, but keep user provided values
					_resultingOrg = zoomValueOrg;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					break;

				case BoundaryRescaling.LessOrEqual:
					_resultingOrg = zoomValueOrg;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					break;

				case BoundaryRescaling.GreaterOrEqual:
					_resultingOrg = zoomValueOrg;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					break;
			}
		}

		protected void ProcessOrg_UserParametersChanged()
		{
			var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

			switch (_orgRescaling)
			{
				case BoundaryRescaling.Fixed:
				case BoundaryRescaling.FixedManually:
				case BoundaryRescaling.FixedZoomable:
					_resultingOrg = resultingUserProvidedOrgValue;
					_resultingMinOrg = _resultingOrg; // Strictly fixed
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_resultingMinOrg = _resultingOrg; // fixed
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_resultingMinOrg = resultingUserProvidedOrgValue; // fixed till user value
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_resultingMinOrg = UnboundMinOrg; // not fixed
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_resultingMinOrg = UnboundMinOrg; // not fixed
					}
					break;

				case BoundaryRescaling.Auto:

					_resultingOrg = _dataBoundsOrg;
					_resultingMinOrg = UnboundMinOrg; // not fixed
					break;

				case BoundaryRescaling.AutoTempFixed:
					_resultingOrg = resultingUserProvidedOrgValue;
					_resultingMinOrg = _resultingOrg; // fixed
					break;
			}
		}

		protected void ProcessEnd_DataBoundsChanged()
		{
			var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

			switch (_endRescaling)
			{
				case BoundaryRescaling.Fixed:
				case BoundaryRescaling.FixedManually:
				case BoundaryRescaling.FixedZoomable:
					_resultingEnd = resultingUserProvidedEndValue;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedEndValue >= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_resultingMaxEnd = UnboundMaxEnd; // not fixed
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_resultingMaxEnd = UnboundMaxEnd; // not fixed
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedEndValue <= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_resultingMaxEnd = _resultingEnd; // fixed
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_resultingMaxEnd = resultingUserProvidedEndValue; // because resultingEnd can go further up to resultingUserProvidedValue
					}
					break;

				case BoundaryRescaling.Auto:
					_resultingEnd = _dataBoundsEnd;
					_resultingMaxEnd = UnboundMaxEnd; // not fixed
					break;

				case BoundaryRescaling.AutoTempFixed:
					_endRescaling = BoundaryRescaling.Auto; // Fall back to auto rescaling
					_resultingEnd = _dataBoundsEnd;
					_resultingMaxEnd = UnboundMaxEnd; // not fixed
					break;
			}
		}

		protected void ProcessEnd_UserRescaled()
		{
			var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

			switch (_endRescaling)
			{
				case BoundaryRescaling.Auto:
					_resultingEnd = _dataBoundsEnd;
					_resultingMaxEnd = UnboundMaxEnd; // not fixed
					break;

				case BoundaryRescaling.AutoTempFixed:
					_endRescaling = BoundaryRescaling.Auto; // Fall back to auto
					_resultingEnd = _dataBoundsEnd;
					_resultingMaxEnd = UnboundMaxEnd; // not fixed
					break;

				case BoundaryRescaling.Fixed:
					_resultingEnd = resultingUserProvidedEndValue;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;

				case BoundaryRescaling.FixedManually:
					// TODO ask for switching to AutoTemp or AutoTempFixed
					// HERE as long as fixed manually, we treat this as fixed
					_resultingEnd = resultingUserProvidedEndValue;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;

				case BoundaryRescaling.FixedZoomable: // treat as fixed
					_resultingEnd = resultingUserProvidedEndValue;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedEndValue >= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_resultingMaxEnd = UnboundMaxEnd; // not fixed
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_resultingMaxEnd = UnboundMaxEnd; // not fixed
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedEndValue <= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_resultingMaxEnd = _resultingEnd; // fixed
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_resultingMaxEnd = _userProvidedEndValue; // fixed until user provided end value
					}
					break;
			}
		}

		protected void ProcessEnd_UserZoomed(double zoomValueEnd)
		{
			var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

			switch (_endRescaling)
			{
				case BoundaryRescaling.Auto:
					_endRescaling = BoundaryRescaling.AutoTempFixed;
					_resultingEnd = zoomValueEnd;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					_userProvidedEndValue = GetUserProvidedEndFromResultingEnd(_resultingEnd);
					break;

				case BoundaryRescaling.AutoTempFixed:
					_resultingEnd = zoomValueEnd;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					_userProvidedEndValue = GetUserProvidedEndFromResultingEnd(_resultingEnd);
					break;

				case BoundaryRescaling.Fixed:
					// Ignore zoom
					break;

				case BoundaryRescaling.FixedManually:
					// use the new values as user provided values
					_resultingEnd = zoomValueEnd;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					_userProvidedEndValue = GetUserProvidedEndFromResultingEnd(zoomValueEnd);
					break;

				case BoundaryRescaling.FixedZoomable:
					// use the new values, but keep user provided values
					_resultingEnd = zoomValueEnd;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;

				case BoundaryRescaling.LessOrEqual:
					_resultingEnd = zoomValueEnd;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;

				case BoundaryRescaling.GreaterOrEqual:
					_resultingEnd = zoomValueEnd;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;
			}
		}

		protected void ProcessEnd_UserParametersChanged()
		{
			var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

			switch (_endRescaling)
			{
				case BoundaryRescaling.Fixed:
				case BoundaryRescaling.FixedManually:
				case BoundaryRescaling.FixedZoomable:
					_resultingEnd = resultingUserProvidedEndValue;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedEndValue >= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_resultingMaxEnd = UnboundMaxEnd; // not fixed
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_resultingMaxEnd = UnboundMaxEnd; // not fixed
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedEndValue <= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_resultingMaxEnd = _resultingEnd; // fixed
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_resultingMaxEnd = _userProvidedEndValue; // fixed till user value
					}
					break;

				case BoundaryRescaling.Auto:
					_resultingEnd = _dataBoundsEnd;
					_resultingMaxEnd = UnboundMaxEnd; // not fixed
					break;

				case BoundaryRescaling.AutoTempFixed:
					_resultingEnd = resultingUserProvidedEndValue;
					_resultingMaxEnd = _resultingEnd; // strictly fixed
					break;
			}
		}

		#endregion Process Org End

		#region IScaleRescaleConditions implementation

		void IUnboundNumericScaleRescaleConditions.SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, Data.AltaxoVariant orgValue, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, Data.AltaxoVariant endValue)
		{
			double orgV, endV;
			if (orgValue.CanConvertedToDouble)
			{
				orgV = orgValue.ToDouble();
			}
			else
			{
				throw new InvalidOperationException("Can not convert orgValue to a double value.");
			}

			if (orgValue.CanConvertedToDouble)
			{
				endV = endValue.ToDouble();
			}
			else
			{
				throw new InvalidOperationException("Can not convert endValue to a double value.");
			}

			this.SetUserParameters(orgRescaling, orgRelativeTo, orgV, endRescaling, endRelativeTo, endV);
		}

		#endregion IScaleRescaleConditions implementation
	}
}
