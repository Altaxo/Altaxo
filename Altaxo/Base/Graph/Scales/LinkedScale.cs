#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;

using Altaxo.Serialization;
using Altaxo.Data;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Scales
{
  [Serializable]
  public class LinkedScale : Scale
  {
    /// <summary>
    /// The (real) scale which is behind this fassade. Can not be another linked scale.
    /// </summary>
    private Scale _scaleWrapped; 

		private Scale _scaleLinkedTo;

    /// <summary>Index of linked scale in the linked layer.</summary>
    int _linkedScaleIndex;

		LinkedScaleParameters _linkParameters;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedScale), 2)]
    class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LinkedScale s = (LinkedScale)obj;

        info.AddValue("ScaleWrapped", s._scaleWrapped);
				info.AddValue("LinkParameters", s._linkParameters);
				info.AddValue("LinkedScaleIndex", s._linkedScaleIndex);
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
				s._linkedScaleIndex = info.GetInt32("LinkedScaleIndex");
       
        return s;
      }
    }
    #endregion

		private LinkedScale()
		{
		}


		public LinkedScale(Scale scaleToWrap, Scale scaleLinkedTo, int scaleNumber)
		{
			_linkedScaleIndex = scaleNumber;
			_linkParameters = new LinkedScaleParameters();
			_linkParameters.Changed += EhLinkParametersChanged;

			WrappedScale = scaleToWrap;
			ScaleLinkedTo = scaleLinkedTo;
		}
    
    void CopyFrom(LinkedScale from)
    {
			this._linkedScaleIndex = from._linkedScaleIndex;
			_linkParameters = (LinkedScaleParameters)from._linkParameters.Clone();
			_linkParameters.Changed += EhLinkParametersChanged;
			
			this.WrappedScale = from._scaleWrapped == null ? null : (Scale)from._scaleWrapped.Clone();
			this.ScaleLinkedTo = from._scaleLinkedTo; // not cloning, the cloned scale is linked to the same scale 
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
        return _scaleLinkedTo;
      }

      set
      {
        if(object.ReferenceEquals(_scaleLinkedTo,value))
          return;

        if (null != _scaleLinkedTo)
        {
          _scaleLinkedTo.Changed -= EhLinkedScaleChanged;
        }

        _scaleLinkedTo = value;

        if (null != _scaleLinkedTo)
        {
          _scaleLinkedTo.Changed += EhLinkedScaleChanged;
        }

        OnLinkPropertiesChanged(); // calculated the bounds of the wrapped scale 
      }
    }

    public int LinkedScaleIndex
    {
      get { return _linkedScaleIndex; }
      set { _linkedScaleIndex = value; }
    }

		public void EhLinkedLayerScaleInstanceChanged(int idx, Scale oldScale, Scale newScale)
		{
			if (_linkedScaleIndex == idx)
				ScaleLinkedTo = newScale;
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

		public void SetLinkParameter(LinkedScaleParameters parameters)
		{
			_linkParameters.SetTo(parameters.OrgA, parameters.OrgB, parameters.EndA, parameters.EndB);
		}

		public LinkedScaleParameters LinkParameters
		{
			get
			{
				return _linkParameters;
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
        if (object.ReferenceEquals(_scaleWrapped, value))
          return;

          if (null != _scaleWrapped)
          {
            _scaleWrapped.Changed -= new EventHandler(this.EhWrappedScaleChanged);
          }
        _scaleWrapped = value;
          if (null != _scaleWrapped)
          {
            _scaleWrapped.Changed += new EventHandler(this.EhWrappedScaleChanged);
          }

          OnLinkPropertiesChanged();
          OnChanged();
        }
      }
   

    void EhWrappedScaleChanged(object sender, EventArgs e)
    {
			OnChanged();
    }

    void EhLinkedScaleChanged(object sender, EventArgs e)
    {
      OnLinkPropertiesChanged();
    }

		void EhLinkParametersChanged(object sender, EventArgs e)
		{
			OnLinkPropertiesChanged();
		}
  

    protected virtual void OnLinkPropertiesChanged()
    {
      // calculate the new bounds
      if (null != _scaleLinkedTo)
      {
        AltaxoVariant org = _scaleLinkedTo.OrgAsVariant;
        AltaxoVariant end = _scaleLinkedTo.EndAsVariant;

        if (!IsStraightLink)
        {
          org = org * LinkOrgB + LinkOrgA;
          end = end * LinkEndB + LinkEndA;
        }

        _scaleWrapped.SetScaleOrgEnd(org, end);
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
        // it is not possible for a this scale to act back to the scale which is linked
        // but to make the plot items influence the range of the linked scale we can give back
        // the data bounds object of the linked scale

        return _scaleLinkedTo == null ? _scaleWrapped.DataBoundsObject : _scaleLinkedTo.DataBoundsObject;
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
			get { return null==_scaleLinkedTo ? false : _scaleLinkedTo.IsOrgExtendable; }
		}

		/// <summary>Returns true if it is allowed to extend the scale end (to higher values).</summary>
		public override bool IsEndExtendable
		{
			get { return null == _scaleLinkedTo ? false : _scaleLinkedTo.IsEndExtendable; }
		}

		public override void Rescale()
		{
      if (null != _scaleLinkedTo)
        _scaleLinkedTo.Rescale();
		}

		public override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
		{
      if (null != _scaleLinkedTo)
      {
        if (!IsStraightLink)
        {
          org = (org - LinkOrgA) / LinkOrgB;
          end = (end - LinkEndA) / LinkEndB;
        }
        return _scaleLinkedTo.SetScaleOrgEnd(org, end);
      }
			return null;
		}
	}
}
