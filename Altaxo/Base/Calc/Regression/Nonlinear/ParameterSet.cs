using System;

namespace Altaxo.Calc.Regression.Nonlinear
{

  public class ParameterSetElement
  {
    public string Name;
    public double Parameter;
    public bool   Vary;

    public void CopyFrom(ParameterSetElement from)
    {
      this.Name = from.Name;
      this.Parameter = from.Parameter;
      this.Vary = from.Vary;
    }

  }
	/// <summary>
	/// Summary description for ParameterSet.
	/// </summary>
	public class ParameterSet : System.Collections.CollectionBase
	{
		public ParameterSet()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    public ParameterSetElement this[int i]
    {
      get
      {
        return (ParameterSetElement)this.InnerList[i];
      }
    }

    public void Add(ParameterSetElement ele)
    {
      this.InnerList.Add(ele);
    }
	}
}
