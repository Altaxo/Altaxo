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
	public abstract class NumericAxisRescaleConditions
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		Main.ICopyFrom
	{
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
		protected bool _isResultingOrgFixed;
		protected bool _isResultingEndFixed;

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
				NumericAxisRescaleConditions s = null != o ? (NumericAxisRescaleConditions)o : new LinearScaleRescaleConditions();

				s._userProvidedOrgRelativeTo = BoundariesRelativeTo.Absolute;
				s._userProvidedEndRelativeTo = BoundariesRelativeTo.Absolute;

				var orgRescaling = (BoundaryRescaling)info.GetEnum("OrgRescaling", typeof(BoundaryRescaling));
				var org = (double)info.GetDouble("Org");
				var endRescaling = (BoundaryRescaling)info.GetEnum("EndRescaling", typeof(BoundaryRescaling));
				var end = (double)info.GetDouble("End");
				var spanRescaling = (BoundaryRescaling)info.GetEnum("SpanRescaling", typeof(BoundaryRescaling));
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
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericAxisRescaleConditions), 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (NumericAxisRescaleConditions)obj;
				info.AddEnum("OrgRescaling", s._orgRescaling);
				info.AddEnum("EndRescaling", s._endRescaling);
				info.AddEnum("OrgRelativeTo", s._userProvidedOrgRelativeTo);
				info.AddEnum("EndRelativeTo", s._userProvidedEndRelativeTo);
				info.AddValue("UserProvidedOrg", s._userProvidedOrgValue);
				info.AddValue("UserProvidedEnd", s._userProvidedOrgValue);

				// Cached values
				info.AddValue("DataBoundsOrg", s._dataBoundsOrg);
				info.AddValue("DataBoundsEnd", s._dataBoundsEnd);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (NumericAxisRescaleConditions)o;

				s._orgRescaling = (BoundaryRescaling)info.GetEnum("OrgRescaling", typeof(BoundaryRescaling));
				s._endRescaling = (BoundaryRescaling)info.GetEnum("EndRescaling", typeof(BoundaryRescaling));
				s._userProvidedOrgRelativeTo = (BoundariesRelativeTo)info.GetEnum("OrgRelativeTo", typeof(BoundariesRelativeTo));
				s._userProvidedEndRelativeTo = (BoundariesRelativeTo)info.GetEnum("EndRelativeTo", typeof(BoundariesRelativeTo));
				s._userProvidedOrgValue = (double)info.GetDouble("UserProvidedOrg");
				s._userProvidedEndValue = (double)info.GetDouble("UserProvidedEnd");

				// Cached values
				s._dataBoundsOrg = (double)info.GetDouble("DataBoundsOrg");
				s._dataBoundsEnd = (double)info.GetDouble("DataBoundsEnd");
				return s;
			}
		}

		#endregion Serialization

		public NumericAxisRescaleConditions()
		{
		}

		public NumericAxisRescaleConditions(NumericAxisRescaleConditions from)
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
			var from = obj as NumericAxisRescaleConditions;
			if (null == from)
				return false;

			this._orgRescaling = from._orgRescaling;
			this._endRescaling = from._endRescaling;

			this._userProvidedOrgRelativeTo = from._userProvidedOrgRelativeTo;
			this._userProvidedEndRelativeTo = from._userProvidedEndRelativeTo;

			this._userProvidedOrgValue = from._userProvidedOrgValue;
			this._userProvidedEndValue = from._userProvidedEndValue;

			this._resultingOrg = from._resultingOrg;
			this._resultingEnd = from._resultingEnd;

			this._isResultingOrgFixed = from._isResultingOrgFixed;
			this._isResultingEndFixed = from._isResultingEndFixed;

			EhSelfChanged(EventArgs.Empty);

			return true;
		}

		public abstract object Clone();

		#region public properties

		public double ResultingOrg { get { return _resultingOrg; } }

		public double ResultingEnd { get { return _resultingEnd; } }

		public bool IsResultingOrgFixed { get { return _isResultingOrgFixed; } }

		public bool IsResultingEndFixed { get { return _isResultingEndFixed; } }

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

		public double UserProvidedOrgValue { get { return _userProvidedOrgValue; } }

		public double UserProvidedEndValue { get { return _userProvidedEndValue; } }

		#endregion Accessors

		#region Event handling

		public void OnUserZoomed(double newZoomOrg, double newZoomEnd)
		{
			if (!(newZoomOrg < newZoomEnd))
				throw new ArgumentOutOfRangeException("zoomOrg should be less than zoomEnd");

			ProcessOrg_UserZoomed(newZoomOrg);
			ProcessEnd_UserZoomed(newZoomEnd);
		}

		public void OnUserRescaled()
		{
			ProcessOrg_UserRescaled();
			ProcessEnd_UserRescaled();
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
		protected abstract void FixDataBoundsOrgAndEnd(ref double dataBoundsOrg, ref double dataBoundsEnd);

		public void OnDataBoundsChanged(double dataBoundsOrg, double dataBoundsEnd)
		{
			if (!(dataBoundsOrg <= dataBoundsEnd))
				throw new ArgumentOutOfRangeException("dataBoundsOrg should be less than dataBoundsEnd");

			var oldResultingOrg = _resultingOrg;
			var oldResultingEnd = _resultingEnd;
			var oldResultingOrgFixed = _isResultingOrgFixed;
			var oldResultringEndFixed = _isResultingEndFixed;

			FixDataBoundsOrgAndEnd(ref dataBoundsOrg, ref dataBoundsEnd);

			_dataBoundsOrg = dataBoundsOrg;
			_dataBoundsEnd = dataBoundsEnd;

			ProcessOrg_DataBoundsChanged();
			ProcessEnd_DataBoundsChanged();

			var changed =
				_resultingOrg != oldResultingOrg ||
				_resultingEnd != oldResultingEnd ||
				_isResultingOrgFixed != oldResultingOrgFixed ||
				_isResultingEndFixed != oldResultringEndFixed;

			if (changed)
				EhSelfChanged();
		}

		#endregion Event handling

		#region Resulting Org/End to/fron User Org/End

		protected double GetResultingOrgFromUserProvidedOrg()
		{
			switch (_userProvidedOrgRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return _userProvidedOrgValue;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return _userProvidedOrgValue + _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return _userProvidedOrgValue + _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return _userProvidedOrgValue + GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		protected double GetUserProvidedOrgFromResultingOrg(double resultingOrg)
		{
			switch (_userProvidedOrgRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return resultingOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return resultingOrg - _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return resultingOrg - _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return resultingOrg - GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		protected double GetResultingOrgFromDataBoundsOrg()
		{
			switch (_userProvidedOrgRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return _userProvidedOrgValue + _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return _userProvidedOrgValue + _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return _userProvidedOrgValue + GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		protected double GetResultingEndFromUserProvidedEnd()
		{
			switch (_userProvidedEndRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return _userProvidedEndValue;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return _userProvidedEndValue + _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return _userProvidedEndValue + _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return _userProvidedEndValue + GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		protected double GetUserProvidedEndFromResultingEnd(double resultingEnd)
		{
			switch (_userProvidedEndRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return resultingEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return resultingEnd - _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return resultingEnd - _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return resultingEnd - GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		protected double GetResultingEndFromDataBoundsEnd()
		{
			switch (_userProvidedEndRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return _userProvidedEndValue + _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return _userProvidedEndValue + _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return _userProvidedEndValue + GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

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
					_isResultingOrgFixed = true;
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_isResultingOrgFixed = true;
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_isResultingOrgFixed = false;
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_isResultingOrgFixed = true;
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_isResultingOrgFixed = false;
					}
					break;

				case BoundaryRescaling.Auto:
				case BoundaryRescaling.AutoTempFixed:
					_resultingOrg = _dataBoundsOrg;
					_isResultingOrgFixed = false;
					break;
			}
		}

		protected void ProcessOrg_UserRescaled()
		{
			var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

			switch (_orgRescaling)
			{
				case BoundaryRescaling.Auto:
				case BoundaryRescaling.AutoTempFixed:
					_resultingOrg = GetResultingOrgFromDataBoundsOrg();
					_isResultingOrgFixed = false;
					break;

				case BoundaryRescaling.Fixed:
					_resultingOrg = resultingUserProvidedOrgValue;
					_isResultingOrgFixed = true;
					break;

				case BoundaryRescaling.FixedManually:
					// TODO ask for switching to AutoTemp or AutoTempFixed
					// HERE as long as fixed manually, we treat this as fixed
					_resultingOrg = resultingUserProvidedOrgValue;
					_isResultingOrgFixed = true;
					break;

				case BoundaryRescaling.FixedZoomable: // treat as fixed
					_resultingOrg = resultingUserProvidedOrgValue;
					_isResultingOrgFixed = true;
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_isResultingOrgFixed = true;
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_isResultingOrgFixed = false;
					}
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_isResultingOrgFixed = true;
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_isResultingOrgFixed = false;
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
					_isResultingOrgFixed = true;
					break;

				case BoundaryRescaling.AutoTempFixed:
					_resultingOrg = zoomValueOrg;
					_isResultingOrgFixed = true;
					break;

				case BoundaryRescaling.Fixed:
					// Ignore zoom
					break;

				case BoundaryRescaling.FixedManually:
					// use the new values as user provided values
					_resultingOrg = zoomValueOrg;
					_isResultingOrgFixed = true;
					_userProvidedOrgValue = GetUserProvidedOrgFromResultingOrg(zoomValueOrg);
					break;

				case BoundaryRescaling.FixedZoomable:
					// use the new values, but keep user provided values
					_resultingOrg = zoomValueOrg;
					_isResultingOrgFixed = true;
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedOrgValue <= zoomValueOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_isResultingOrgFixed = true;
					}
					else
					{
						_resultingOrg = zoomValueOrg;
						_isResultingOrgFixed = true;
					}
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_isResultingOrgFixed = true;
					}
					else
					{
						_resultingOrg = zoomValueOrg;
						_isResultingOrgFixed = true;
					}
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
					_isResultingOrgFixed = true;
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_isResultingOrgFixed = true;
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_isResultingOrgFixed = false;
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
					{
						_resultingOrg = resultingUserProvidedOrgValue;
						_isResultingOrgFixed = true;
					}
					else
					{
						_resultingOrg = _dataBoundsOrg;
						_isResultingOrgFixed = false;
					}
					break;

				case BoundaryRescaling.Auto:

					_resultingOrg = _dataBoundsOrg;
					_isResultingOrgFixed = false;
					break;

				case BoundaryRescaling.AutoTempFixed:
					_resultingOrg = resultingUserProvidedOrgValue;
					_isResultingOrgFixed = true;
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
					_isResultingEndFixed = true;
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedEndValue >= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_isResultingEndFixed = false;
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_isResultingEndFixed = true;
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedEndValue <= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_isResultingEndFixed = false;
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_isResultingEndFixed = true;
					}
					break;

				case BoundaryRescaling.Auto:
				case BoundaryRescaling.AutoTempFixed:
					_resultingEnd = _dataBoundsEnd;
					_isResultingEndFixed = false;
					break;
			}
		}

		protected void ProcessEnd_UserRescaled()
		{
			var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

			switch (_endRescaling)
			{
				case BoundaryRescaling.Auto:
				case BoundaryRescaling.AutoTempFixed:
					_resultingEnd = GetResultingEndFromDataBoundsEnd();
					_isResultingEndFixed = false;
					break;

				case BoundaryRescaling.Fixed:
					_resultingEnd = resultingUserProvidedEndValue;
					_isResultingEndFixed = true;
					break;

				case BoundaryRescaling.FixedManually:
					// TODO ask for switching to AutoTemp or AutoTempFixed
					// HERE as long as fixed manually, we treat this as fixed
					_resultingEnd = resultingUserProvidedEndValue;
					_isResultingEndFixed = true;
					break;

				case BoundaryRescaling.FixedZoomable: // treat as fixed
					_resultingEnd = resultingUserProvidedEndValue;
					_isResultingEndFixed = true;
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedEndValue >= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_isResultingEndFixed = false;
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_isResultingEndFixed = true;
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedEndValue <= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_isResultingEndFixed = false;
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_isResultingEndFixed = true;
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
					_isResultingEndFixed = true;
					break;

				case BoundaryRescaling.AutoTempFixed:
					_resultingEnd = zoomValueEnd;
					_isResultingEndFixed = true;
					break;

				case BoundaryRescaling.Fixed:
					// Ignore zoom
					break;

				case BoundaryRescaling.FixedManually:
					// use the new values as user provided values
					_resultingEnd = zoomValueEnd;
					_isResultingEndFixed = true;
					_userProvidedEndValue = GetUserProvidedEndFromResultingEnd(zoomValueEnd);
					break;

				case BoundaryRescaling.FixedZoomable:
					// use the new values, but keep user provided values
					_resultingEnd = zoomValueEnd;
					_isResultingEndFixed = true;
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedEndValue <= zoomValueEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_isResultingEndFixed = true;
					}
					else
					{
						_resultingEnd = zoomValueEnd;
						_isResultingEndFixed = true;
					}
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedEndValue >= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_isResultingEndFixed = true;
					}
					else
					{
						_resultingEnd = zoomValueEnd;
						_isResultingEndFixed = true;
					}
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
					_isResultingEndFixed = true;
					break;

				case BoundaryRescaling.GreaterOrEqual:
					if (resultingUserProvidedEndValue >= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_isResultingEndFixed = false;
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_isResultingEndFixed = true;
					}
					break;

				case BoundaryRescaling.LessOrEqual:
					if (resultingUserProvidedEndValue <= _dataBoundsEnd)
					{
						_resultingEnd = resultingUserProvidedEndValue;
						_isResultingEndFixed = false;
					}
					else
					{
						_resultingEnd = _dataBoundsEnd;
						_isResultingEndFixed = true;
					}
					break;

				case BoundaryRescaling.Auto:
					_resultingEnd = _dataBoundsEnd;
					_isResultingEndFixed = false;
					break;

				case BoundaryRescaling.AutoTempFixed:
					_resultingEnd = resultingUserProvidedEndValue;
					_isResultingEndFixed = true;
					break;
			}
		}

		#endregion Process Org End
	}
}