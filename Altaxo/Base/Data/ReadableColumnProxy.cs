using System;
using Altaxo.Main;

namespace Altaxo.Data
{


 
	/// <summary>
	/// Summary description for DataColumnPlaceHolder.
	/// </summary>
	public class ReadableColumnProxy : DocNodeProxy
	{
    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ReadableColumnProxy),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj,typeof(DocNodeProxy)); // serialize the base class
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ReadableColumnProxy s = o!=null ? (ReadableColumnProxy)o : new ReadableColumnProxy();
        info.GetBaseValueEmbedded(s,typeof(DocNodeProxy),parent);         // deserialize the base class

        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);

        return s;
      }
    }
    #endregion

    public ReadableColumnProxy(IReadableColumn column)
      : base(column)
    {
    }

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    protected ReadableColumnProxy()
    {
    }

    /// <summary>
    /// Cloning constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    public ReadableColumnProxy(ReadableColumnProxy from)
      : base(from)
    {
    }

    private void EhXmlDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
      if(this.Document!=null)
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhXmlDeserializationFinished);
    }

    protected override bool IsValidDocument(object obj)
    {
      return (obj is IReadableColumn) || obj==null;
    }
   
    public IReadableColumn Document
    {
      get
      {
        return (IReadableColumn)base.DocumentObject;        
      }
    }

    public override object Clone()
    {
      return new ReadableColumnProxy(this);
    }
  }
}
