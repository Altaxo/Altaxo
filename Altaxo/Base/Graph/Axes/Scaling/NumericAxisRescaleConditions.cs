using System;

namespace Altaxo.Graph.Axes.Scaling
{
	/// <summary>
	/// Summary description for AxisRescaleConditions.
	/// </summary>
	public class NumericAxisRescaleConditions
	{
    protected BoundaryRescaling _orgRescaling;
    protected BoundaryRescaling _endRescaling;
    protected BoundaryRescaling _spanRescaling;
    protected double _org;
    protected double _end;
    protected double _span;

    /// <summary>
    /// Sets the scaling behaviour of the axis by providing org and end values.
    /// </summary>
    /// <param name="orgRescaling">Type of scaling behaviour for the axis origin.</param>
    /// <param name="org">The axis org value. If orgRescaling is Auto, this value is ignored.</param>
    /// <param name="endRescaling">Type of scaling behaviour for the axis end.</param>
    /// <param name="end">The axis end value. If endRescaling is Auto, this value is ignored.</param>
    public virtual void SetOrgAndEnd(BoundaryRescaling orgRescaling, double org, BoundaryRescaling endRescaling, double end)
    {
      _orgRescaling = orgRescaling;
      _org = org;
      _endRescaling = endRescaling;
      _end = end;
      _spanRescaling = BoundaryRescaling.Auto;
    }

    /// <summary>
    /// Sets the scaling behavior so, that a certain size (end-org) of the axis is maintained.
    /// </summary>
    /// <param name="spanRescaling">The type of scaling for the axis span.</param>
    /// <param name="span">The axis span value. If spanRescaling is Auto, this value is ignored.</param>
    public virtual void SetSpan(BoundaryRescaling spanRescaling, double span)
    {
      _spanRescaling = spanRescaling;
      _span = span;
    }

    /// <summary>
    /// Sets the scaling behaviour to auto for both ends of the axis.
    /// </summary>
    public virtual void SetAuto()
    {
      _orgRescaling = BoundaryRescaling.Auto;
      _endRescaling = BoundaryRescaling.Auto;
      _spanRescaling = BoundaryRescaling.Auto;
    }

    /// <summary>
    /// This will process the temporary values for the axis origin and axis end. Depending on the rescaling conditions,
    /// the values of org and end are changed.
    /// </summary>
    /// <param name="org">The temporary axis origin (usually the lower boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoOrg">On return, this value is true if the org value was not modified.</param>
    /// <param name="end">The temporary axis end (usually the upper boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoEnd">On return, this value is true if the end value was not modified.</param>
    public virtual void Process(ref double org, out bool isAutoOrg, ref double end, out bool isAutoEnd)
    {
      double oorg = org;
      double oend = end;
      isAutoOrg = true;
      isAutoEnd = true;

      if(_spanRescaling!=BoundaryRescaling.Auto)
      {
        switch(_spanRescaling)
        {
          case BoundaryRescaling.Fixed:
            org = ((oorg+oend)-_span)*0.5;
            end = ((oorg+oend)+_span)*0.5;
            isAutoOrg = false;
            isAutoEnd = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if(Math.Abs(oorg-oend)<_span)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if(Math.Abs(oorg-oend)>_span)
              goto case BoundaryRescaling.Fixed;
            break;
        } // switch
      }
      else // spanRescaling is Auto
      {
        switch(_orgRescaling)
        {
          case BoundaryRescaling.Fixed:
            org = _org;
            isAutoOrg = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if(oorg<_org)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if(oorg>_org)
              goto case BoundaryRescaling.Fixed;
            break;
        }
        switch(_endRescaling)
        {
          case BoundaryRescaling.Fixed:
            end = _end;
            isAutoEnd = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if(oend<_end)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if(oend>_end)
              goto case BoundaryRescaling.Fixed;
            break;
        }
      }
    }
	}
}
