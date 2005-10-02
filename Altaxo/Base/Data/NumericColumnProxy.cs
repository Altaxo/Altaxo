using System;
using Altaxo.Main;

namespace Altaxo.Data
{


 
  /// <summary>
  /// Holds a "weak" reference to a numeric column, altogether with a document path to that column.
  /// </summary>
  public class NumericColumnProxy : DocNodeProxy
  {
    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericColumnProxy),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj,typeof(DocNodeProxy)); // serialize the base class
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        NumericColumnProxy s = o!=null ? (NumericColumnProxy)o : new NumericColumnProxy();
        info.GetBaseValueEmbedded(s,typeof(DocNodeProxy),parent);         // deserialize the base class

        return s;
      }
    }
    #endregion

    /// <summary>
    /// Constructor by giving a numeric column.
    /// </summary>
    /// <param name="column">The numeric column to hold.</param>
    public NumericColumnProxy(INumericColumn column)
      : base(column)
    {
    }

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    protected NumericColumnProxy()
    {
    }

    /// <summary>
    /// Cloning constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    public NumericColumnProxy(NumericColumnProxy from)
      : base(from)
    {
    }

    /// <summary>
    /// Tests whether or not the holded object is valid. Here the test returns true if the column
    /// is either of type <see>INumericColumn</see> or the holded object is <c>null</c>.
    /// </summary>
    /// <param name="obj">Object to test.</param>
    /// <returns>True if this is a valid document object.</returns>
    protected override bool IsValidDocument(object obj)
    {
      return (obj is INumericColumn) || obj==null;
    }
   
    /// <summary>
    /// Returns the holded object. Null can be returned if the object is no longer available (e.g. disposed).
    /// </summary>
    public INumericColumn Document
    {
      get
      {
        return (INumericColumn)base.DocumentObject;        
      }
    }

    /// <summary>
    /// Clones this holder. For holded objects, which are part of the document hierarchy,
    /// the holded object is <b>not</b> cloned (only the reference is copied). For all other objects, the object
    /// is cloned, too, if the object supports the <see>ICloneable</see> interface.
    /// </summary>
    /// <returns>The cloned object holder.</returns>
    public override object Clone()
    {
      return new NumericColumnProxy(this);
    }
  }
}
