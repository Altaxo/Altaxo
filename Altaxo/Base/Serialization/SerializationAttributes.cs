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
using System.Runtime.Serialization;


namespace Altaxo.Serialization
{

  /// <summary>
  /// Interface IDeserializationSubstitute is intended to provide a 
  /// deserializable class instead of a class which cannot be deserialized
  /// /i.e a Windows Form)
  /// it has to hold all parameters which are neccessary to restore the
  /// original class 
  /// </summary>
  public interface IDeserializationSubstitute
  {
    /// <summary>
    /// restores the original object from this substitute
    /// </summary>
    /// <param name="parentobject">a parent object can be provided in order to restore hierarchies</param>
    /// <returns>the original object</returns>
    object GetRealObject(object parentobject);
  }


  /// <summary>
  /// DeserializationFinisher serves only as a "indicator" object for calling IDeserializationCallback::OnDeserialization
  /// this call is made twice: one time from the deserialization infrastructure (the runtime classes) and one time
  /// by myself after all deserialization has finished to fixup special objects
  /// to indicate this second call to OnDeserialization(object obj), the object obj have to be of type DeserializationFinisher
  /// </summary>
  public class DeserializationFinisher
  {
    protected object m_Parent;

    public DeserializationFinisher() { m_Parent=null; }
    public DeserializationFinisher(object o) { m_Parent=o; }
    public object Parent
    {
      get { return m_Parent; }
    }
  }


  /// <summary>
  /// SerializationSurrogateAttribute provides the mapping between the version of a serialized class
  /// and the ISerializationSurrogate, which is responsible for the serialization and
  /// deserialization of that class
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
  public class SerializationSurrogateAttribute : Attribute
  {
    protected int m_Version;
    protected System.Type m_SurrogateType;

    public SerializationSurrogateAttribute(int version, Type surrogatetype)
    {
      m_Version = version;
      m_SurrogateType = surrogatetype; 
    }
    /// <summary>
    /// returns the version of the class, for which the surrogate is intended
    /// </summary>
    public int Version
    {
      get { return m_Version; }
    }
    /// <summary>
    /// returns a instance of the ISerializationSurrogate which is used to serialize
    /// and deserialize this version of the class
    /// </summary>
    public System.Runtime.Serialization.ISerializationSurrogate Surrogate
    {
      get { return (ISerializationSurrogate)Activator.CreateInstance(m_SurrogateType); }
    }
  } // end class SerializationSurrogateAttribute

  /// <summary>
  /// SerializationVersionAttribute provides the actual version of a class concerning serialization,
  /// i.e. every time the serialization behavior changed, you have to give the
  /// class a higher version number
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
  public class SerializationVersionAttribute : Attribute
  {
    protected int m_Version;
    protected string m_Comment;

    public SerializationVersionAttribute(int version)
    {
      m_Version = version;
      m_Comment = null; 
    }

    public SerializationVersionAttribute(int version, string comment)
    {
      m_Version = version;
      m_Comment = comment; 
    }

    public int Version
    {
      get { return m_Version; }
    }
    
    public string Comment
    {
      get { return m_Comment; }
    }
  } // end class SerializationVersionAttribute


  [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
  public class SupportsSerializationVersioningAttribute : Attribute
  {
    public SupportsSerializationVersioningAttribute()
    {
    }
  }


  
  public class AltaxoStreamingContext
  {
    public System.Runtime.Serialization.SurrogateSelector m_SurrogateSelector;
    public System.Type m_FormatterType;

    public static System.Runtime.Serialization.ISurrogateSelector GetSurrogateSelector(System.Runtime.Serialization.StreamingContext context)
    {
      AltaxoStreamingContext sc = context.Context as AltaxoStreamingContext;
      return null!=sc ? sc.m_SurrogateSelector : null;
    }
  }


}
