using System;

namespace Altaxo.Graph.Axes.Scaling
{
	/// <summary>
	/// Summary description for LogarithmicAxisRescaleConditions.
	/// </summary>
	public class LogarithmicAxisRescaleConditions : NumericAxisRescaleConditions
	{
    /// <summary>
    /// This will process the temporary values for the axis origin and axis end. Depending on the rescaling conditions,
    /// the values of org and end are changed.
    /// </summary>
    /// <param name="org">The temporary axis origin (usually the lower boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoOrg">On return, this value is true if the org value was not modified.</param>
    /// <param name="end">The temporary axis end (usually the upper boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoEnd">On return, this value is true if the end value was not modified.</param>
    public override void Process(ref double org, out bool isAutoOrg, ref double end, out bool isAutoEnd)
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
            org = Math.Exp((Math.Log(oorg)+Math.Log(oend)-Math.Log(_span))*0.5);
            end = Math.Exp((Math.Log(oorg)+Math.Log(oend)+Math.Log(_span))*0.5);
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
