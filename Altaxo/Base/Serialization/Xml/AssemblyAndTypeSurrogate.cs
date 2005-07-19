using System;

namespace Altaxo.Serialization.Xml
{
	/// <summary>
	/// Summary description for AssemblyAndTypeSurrogate.
	/// </summary>
	/// 
	public class AssemblyAndTypeSurrogate
	{
    string _assemblyName;
    string _typeName;


    #region Serialization
 

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AssemblyAndTypeSurrogate),0)]
      public class XmlSerializationSurrogate : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AssemblyAndTypeSurrogate s = (AssemblyAndTypeSurrogate)obj;
       
        info.AddValue("AssemblyName",s._assemblyName);
        info.AddValue("TypeName",s._typeName);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AssemblyAndTypeSurrogate s = o==null ? new AssemblyAndTypeSurrogate() : (AssemblyAndTypeSurrogate)o;

        s._assemblyName = info.GetString("AssemblyName");
        s._typeName = info.GetString("TypeName");

        return s;
      }
        
    }

   
    #endregion

		protected AssemblyAndTypeSurrogate()
		{
		}

    public AssemblyAndTypeSurrogate(object o)
    {
      if(o==null)
        throw new ArgumentNullException("To determine the type, the argument must not be null");
      this._assemblyName = o.GetType().Assembly.FullName;
      this._typeName = o.GetType().FullName;
    }

    public object CreateInstance()
    {
      try
      {
        System.Runtime.Remoting.ObjectHandle oh =  System.Activator.CreateInstance(_assemblyName,_typeName);
        return oh.Unwrap();
      }
      catch(Exception)
      {
      }
      return null;
    }
	}
}
