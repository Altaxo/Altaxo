using System;

namespace Altaxo.Calc.Regression.Nonlinear
{

  public class ParameterSetElement : ICloneable
  {
    public string Name;
    public double Parameter;
    public double Variance;
    public bool   Vary;
    

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSetElement),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ParameterSetElement s = (ParameterSetElement)obj;

        info.AddValue("Name",s.Name);
        info.AddValue("Value",s.Parameter);
        info.AddValue("Variance",s.Variance);
        info.AddValue("Vary",s.Vary);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ParameterSetElement s = o!=null ? (ParameterSetElement)o : new ParameterSetElement();

        s.Name = info.GetString("Name");
        s.Parameter = info.GetDouble("Value");
        s.Variance = info.GetDouble("Variance");
        s.Vary = info.GetBoolean("Vary");

        return s;
      }
    }

    #endregion


    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    protected ParameterSetElement()
    {
    }
      
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
      this.Variance = from.Variance;
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
	public class ParameterSet : System.Collections.CollectionBase, ICloneable
	{
    /// <summary>
    /// Event is fired if the main initialization is finished. This event can be fired
    /// multiple times (every time the set has changed basically.
    /// </summary>
    public event EventHandler InitializationFinished;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSet),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ParameterSet s = (ParameterSet)obj;

        info.CreateArray("Parameters",s.Count);
        for(int i=0;i<s.Count;++i)
          info.AddValue("e",s[i]);
        info.CommitArray();
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ParameterSet s = o!=null ? (ParameterSet)o : new ParameterSet();

        int arraycount = info.OpenArray();
        for(int i=0;i<arraycount;++i)
          s.Add( (ParameterSetElement)info.GetValue(s) );
        info.CloseArray(arraycount);

       

        return s;
      }
    }

    #endregion
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

    #region ICloneable Members

    public object Clone()
    {
      ParameterSet result = new ParameterSet();
      for (int i = 0; i < Count; ++i)
        result.Add((ParameterSetElement)this[i].Clone());

      return result;
    }

    #endregion
  }
}
