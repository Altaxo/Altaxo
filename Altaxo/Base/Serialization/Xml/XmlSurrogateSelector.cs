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
using System.Xml;
using System.Reflection;
using System.Runtime;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Responsible for storage and retrieving of the xml surrogate classes.
  /// </summary>
  public class XmlSurrogateSelector
  {

    /// <summary>
    /// Use to store the surrogates for a given class.
    /// </summary>
    /// <remarks>There are two kind of keys here: 1) System.Type objects and 2) strings containing the fully qualified name of a type.
    /// The key strings are used to retrieve the serialization surrogate onto deserialization, and if the class to deserialize no longer exists in the assembly.
    /// The values are instances (!) of type IXmlSerializationSurrogate.</remarks>
    System.Collections.Hashtable m_Surrogates = new System.Collections.Hashtable();


    /// <summary>
    /// Used to store the actual serialization versions of the classes. Classes which are not marked with the <see cref="SerializationVersionAttribute" /> attribute
    /// are considered to have a version of 0. 
    /// </summary>
    /// <remarks>The keys for the hashtable are System.Type objects, the values are integers storing the serialization version.</remarks>
    System.Collections.Hashtable m_Versions = new System.Collections.Hashtable();

    /// <summary>
    /// Constructs an empty surrogate selector.
    /// </summary>
    public XmlSurrogateSelector()
    {
    }

    /// <summary>
    /// Get the fully qualified name of a type. This includes the short assembly name; the full type name, and the version, separated by a comma.
    /// </summary>
    /// <param name="type">The type for which the name should be returned.</param>
    /// <returns>The fully qualified name of the type.</returns>
    public string GetFullyQualifiedTypeName(System.Type type)
    {
      object version = m_Versions[type];
      return GetFullyQualifiedTypeName(type, (null==version ? 0 : (int)version));
    }

    /// <summary>
    /// Get the serialization version  of a type.
    /// </summary>
    /// <param name="type">The type for which the version should be returned.</param>
    /// <returns>The serialization version of the type.</returns>
    public int GetVersion(System.Type type)
    {
      object version = m_Versions[type];
      return null == version ? 0 : (int)version;
    }

    /// <summary>
    /// Get the fully qualified name of a type. This includes the short assembly name; the full type name, and the version, separated by a comma.
    /// </summary>
    /// <param name="type">The type for which the name should be returned.</param>
    /// <param name="version">The version of this type.</param>
    /// <returns>The fully qualified name of the type.</returns>
    public string GetFullyQualifiedTypeName(System.Type type, int version)
    {
      string[] assembly = type.Assembly.FullName.Split(new char[] { ',' }, 2);
      return string.Format("{0},{1},{2}", assembly[0], type.ToString(), version.ToString());
    }
    /// <summary>
    /// Adds a surrogate for the type <code>type</code>.
    /// </summary>
    /// <param name="type">The type for which the surrogate is added.</param>
    /// <param name="version">The version of the surrogate (higher version numbers mean more recent versions).</param>
    /// <param name="surrogate">The surrogate used to serialize/deserialize the type.</param>
    public void AddSurrogate(System.Type type, int version, IXmlSerializationSurrogate surrogate)
    {
      // if this attribute cares about a currently existing type,
      // consider the highest value of version among all attributes
      // which care for the same type as the current version of that type
      AddTypeAndVersionIfHigher(type,version,surrogate);
  
      m_Surrogates[GetFullyQualifiedTypeName(type,version)] = surrogate;
    }

    /// <summary>
    /// Adds a surrogate for the type specified by assembly name, full type name, and version.
    /// </summary>
    /// <param name="assemblyname">The short name of the assembly.</param>
    /// <param name="typename">The fully qualified type name.</param>
    /// <param name="version">The version.</param>
    /// <param name="surrogate">The surrogate which is responsible to deserialize the type.</param>
    public void AddSurrogate(string assemblyname, string typename, int version, IXmlSerializationSurrogate surrogate)
    {
      m_Surrogates[assemblyname+","+typename+","+version] = surrogate;
    }


    /// <summary>
    /// Adds a surrogate for the type specified in the XmlSerializationForAttribute.
    /// </summary>
    /// <param name="attr">The attribute used to describe the type this surrogate is intended for.</param>
    /// <param name="surrogate">The surrogate used to serialize/deserialize the type.</param>
    public void AddSurrogate(XmlSerializationSurrogateForAttribute attr, IXmlSerializationSurrogate surrogate)
    {
      if(null!=attr.SerializationType)
        AddSurrogate(attr.SerializationType, attr.Version, surrogate);
      else
        AddSurrogate(attr.AssemblyName,attr.TypeName,attr.Version,surrogate);
    }

    protected void AddTypeAndVersionIfHigher(System.Type type, int version, IXmlSerializationSurrogate surrogate)
    {
      int storedversion = m_Versions.ContainsKey(type) ? (int)m_Versions[type] : int.MinValue;

      if(version>storedversion)
      {
        m_Versions[type] = version;
        m_Surrogates[type] = surrogate;
      }
    }
    /// <summary>
    /// Get a serialization surrogate for the spezified type.
    /// </summary>
    /// <param name="type">The full qualified type name (<see cref="GetFullyQualifiedTypeName(System.Type)" />) for which a serialization surrogate should be found.</param>
    /// <returns>The serialization surrogate for the specified type, or null if no surrogate is found.</returns>
    public IXmlSerializationSurrogate GetSurrogate(string type)
    {
      return (IXmlSerializationSurrogate)m_Surrogates[type];
    }

    /// <summary>
    /// Get a serialization surrogate for the spezified type.
    /// </summary>
    /// <param name="type">The type for which a serialization surrogate should be found.</param>
    /// <returns>The serialization surrogate for the specified type, or null if no surrogate is found.</returns>
    public IXmlSerializationSurrogate GetSurrogate(System.Type type)
    {
      return (IXmlSerializationSurrogate)m_Surrogates[type];
    }


    /// <summary>
    /// Scans all momentarily loaded assemblies for xml serialization surrogates. 
    /// Only assemblies that are marked with the SupportsSerializationVersioningAttribute are scanned.
    /// </summary>
    public void TraceLoadedAssembliesForSurrogates()
    {
      System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach(Assembly assembly in assemblies)
      {
        // test if the assembly supports Serialization
        Attribute suppVersioning = Attribute.GetCustomAttribute(assembly,typeof(SupportsSerializationVersioningAttribute));
        if(null==suppVersioning)
          continue; // this assembly don't support this, so skip it
        
        Type[] definedtypes = assembly.GetTypes();
        foreach(Type definedtype in definedtypes)
        {
          Attribute[] surrogateattributes = Attribute.GetCustomAttributes(definedtype,typeof(XmlSerializationSurrogateForAttribute));
            
          foreach(XmlSerializationSurrogateForAttribute att in surrogateattributes)
          {
            object obj = Activator.CreateInstance(definedtype);
            System.Diagnostics.Debug.Assert(obj is IXmlSerializationSurrogate, 
              string.Format("Classes that have the XmlSerializationSurrogateForAttribute applied have to implement IXmlSerializationSurrogate. This is not the case for the type " + definedtype.ToString()));
            if(obj is IXmlSerializationSurrogate)
            {
              this.AddSurrogate(att,(IXmlSerializationSurrogate)obj);
                
            
            }
          }
        } // end foreach type
      } // end foreach assembly 
          
    }
  }
}
