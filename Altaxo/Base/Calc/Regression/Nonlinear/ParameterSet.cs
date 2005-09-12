using System;

namespace Altaxo.Calc.Regression.Nonlinear
{

  public class ParameterSetElement : ICloneable
  {
    public string Name;
    public double Parameter;
    public double Variance;
    public bool   Vary;
    
      
      public ParameterSetElement(string name)
    {
      this.Name = name;
      this.Vary = true;
    }

    public ParameterSetElement(string name, double value)
    {
      this.Name = name;
      this.Parameter = value;
      this.Vary = true;
    }

    public ParameterSetElement(ParameterSetElement from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(ParameterSetElement from)
    {
      this.Name = from.Name;
      this.Parameter = from.Parameter;
      this.Vary = from.Vary;
    }

    
    #region ICloneable Members

    public object Clone()
    {
      return new ParameterSetElement(this);
    }

    #endregion
  }
	/// <summary>
	/// Summary description for ParameterSet.
	/// </summary>
	public class ParameterSet : System.Collections.CollectionBase
	{
    /// <summary>
    /// Event is fired if the main initialization is finished. This event can be fired
    /// multiple times (every time the set has changed basically.
    /// </summary>
    public event EventHandler InitializationFinished;


		public ParameterSet()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    public void OnInitializationFinished()
    {
      if(null!=InitializationFinished)
        InitializationFinished(this,EventArgs.Empty);
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
