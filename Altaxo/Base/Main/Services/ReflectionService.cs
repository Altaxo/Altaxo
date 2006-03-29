#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Altaxo.Main.GUI;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Static functions for searching by attributes.
  /// </summary>
  public class ReflectionService
  {
    static List<Assembly> _loadedAssemblies;
    static ReflectionService()
    {
      AppDomain currentDomain = AppDomain.CurrentDomain;
      _loadedAssemblies = new List<Assembly>();
      _loadedAssemblies.AddRange(currentDomain.GetAssemblies());
      currentDomain.AssemblyLoad += new AssemblyLoadEventHandler(EhAssemblyLoaded);
    }

    static void EhAssemblyLoaded(object sender, AssemblyLoadEventArgs e)
    {
      _loadedAssemblies.Add(e.LoadedAssembly);
    }

    private class DictEntryKeyComparer : IComparer
    {
      #region IComparer Members

      public int Compare(object x, object y)
      {
        IComparable xx = (IComparable) ((DictionaryEntry)x).Key;
        IComparable yy = (IComparable) ((DictionaryEntry)y).Key;

        return xx.CompareTo(yy);
      }

      #endregion
    }

    /// <summary>
    /// Determines whether or not a given subtype is derived from a basetype or implements the interface basetype.
    /// </summary>
    /// <param name="subtype">The subtype.</param>
    /// <param name="basetype">The basetype.</param>
    /// <returns>If basetype is a class type, the return value is true if subtype derives from basetype. If basetype is an interface, the return value is true if subtype implements the interface basetype. In all other cases the return value is false.</returns>
    public static bool IsSubClassOfOrImplements(System.Type subtype, System.Type basetype)
    {
      if(basetype.IsInterface)
      {
        if (subtype == basetype)
          return true;
        else
          return null!=subtype.GetInterface(basetype.ToString());
          //return Array.IndexOf(subtype.GetInterfaces(),basetype)>=0;
      }
      else
      {
        return subtype.IsSubclassOf(basetype) || subtype==basetype;
      }

    }


    /// <summary>
    /// Determines whether or not a given AssemblyName is contained in a list of names.
    /// This is done here by comparing the FullNames.
    /// </summary>
    /// <param name="assemblyNames">List of AssemblyNames.</param>
    /// <param name="searchedName">The AssemblyName for which to determine if it is contained in the list.</param>
    /// <returns>True if it is contained in the list.</returns>
    public static bool Contains(AssemblyName[] assemblyNames, AssemblyName searchedName)
    {
      foreach(AssemblyName assName in assemblyNames)
      {
        if(assName.FullName == searchedName.FullName)
          return true;
      }

      return false;
    }

    /// <summary>
    /// Gets a list of currently loaded assemblies that are dependend on the given base assembly. The base assembly is also in the returned list.
    /// </summary>
    /// <param name="baseAssembly">The base assembly.</param>
    /// <param name="start">Index into the <c>_loadedAssemblies</c> array where to start the search. Set it to 0 if you want a full search.</param>
    /// <returns>All assemblies, that are currently loaded and that references the given base assembly. The base assembly is also in the returned list.</returns>
    public static System.Reflection.Assembly[] GetDependendAssemblies(Assembly baseAssembly, int start)
    {
      if (start >= _loadedAssemblies.Count)
      {
        return new Assembly[0];
      }
      else
      {
        ArrayList list = new ArrayList();

        AssemblyName baseAssemblyName = baseAssembly.GetName();

        for (int i = start; i < _loadedAssemblies.Count; i++)
        {
          Assembly assembly = _loadedAssemblies[i];
          if (Contains(assembly.GetReferencedAssemblies(), baseAssemblyName))
            list.Add(assembly);
          else if (assembly == baseAssembly)
            list.Add(assembly);
        }
         return (Assembly[])list.ToArray(typeof(System.Reflection.Assembly));
      }
    }

    /// <summary>
    /// Returns true if <c>testAssembly</c> is dependent on <c>baseAssembly.</c>
    /// </summary>
    /// <param name="baseAssembly">Base assembly.</param>
    /// <param name="testAssembly">Assembly to test.</param>
    /// <returns>True if <c>testAssembly</c> is dependent on <c>baseAssembly.</returns>
    public static bool IsDependentAssembly(Assembly baseAssembly, Assembly testAssembly)
    {
      return baseAssembly==testAssembly || Contains(testAssembly.GetReferencedAssemblies(), baseAssembly.GetName());
    }


    /// <summary>
    /// This will return a list of types that are subclasses of type basetype or (when basetype is an interface)
    /// implements basetype.
    /// </summary>
    /// <param name="basetype">The basetype.</param>
    /// <returns></returns>
    public static System.Type[] GetSubclassesOf(System.Type basetype)
    {
      ArrayList list = new ArrayList();

      Assembly[] assemblies = GetDependendAssemblies(basetype.Assembly,0);
      foreach(Assembly assembly in assemblies)
      {
        Type[] definedtypes = assembly.GetTypes();
        foreach(Type definedtype in definedtypes)
        {
          if(IsSubClassOfOrImplements(definedtype,basetype))
            list.Add(definedtype);
        } // end foreach type
      } // end foreach assembly 

      return (System.Type[])list.ToArray(typeof(System.Type));
    }

    /// <summary>
    /// This will return a list of types that are subclasses of type basetype or (when basetype is an interface)
    /// implements basetype.
    /// </summary>
    /// <param name="basetype">The basetype.</param>
    /// <returns></returns>
    public static System.Type[] GetNonAbstractSubclassesOf(System.Type basetype)
    {
      ArrayList list = new ArrayList();

      Assembly[] assemblies = GetDependendAssemblies(basetype.Assembly,0);
      foreach(Assembly assembly in assemblies)
      {
        Type[] definedtypes = assembly.GetTypes();
        foreach(Type definedtype in definedtypes)
        {
          if(!definedtype.IsAbstract && 
            IsSubClassOfOrImplements(definedtype,basetype))
            list.Add(definedtype);
        } // end foreach type
      } // end foreach assembly 

      return (System.Type[])list.ToArray(typeof(System.Type));
    }

    /// <summary>
    /// For a given type of attribute, attributeType, this function returns the attribute instances and the class
    /// types this attributes apply to. If the attribute implements the IComparable interface, the list is sorted.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static DictionaryEntry[] GetAttributeInstancesAndClassTypes(System.Type attributeType)
    {
      ArrayList list = new ArrayList();

      System.Reflection.Assembly[] assemblies = GetDependendAssemblies(attributeType.Assembly,0);
      foreach(Assembly assembly in assemblies)
      {
        Type[] definedtypes = assembly.GetTypes();
        foreach(Type definedtype in definedtypes)
        {
          Attribute[] attributes = Attribute.GetCustomAttributes(definedtype,attributeType);
            
          foreach(Attribute att in attributes)
          {
            list.Add(new DictionaryEntry(att,definedtype));
          }
        } // end foreach type
      } // end foreach assembly 

      if(list.Count>1 && IsSubClassOfOrImplements(attributeType,typeof(IComparable)))
        list.Sort(new DictEntryKeyComparer());

      return (DictionaryEntry[])list.ToArray(typeof(DictionaryEntry));
    }

    /// <summary>
    /// For a given type of attribute, attributeType, this function returns the attribute instances and the class
    /// types this attributes apply to. If the attribute implements the IComparable interface, the list is sorted. The attribute has
    /// to implement the <see cref="IClassForClassAttribute" /> interface, and only those attributes are considered, for which the
    /// <see cref="IClassForClassAttribute.TargetType" /> match the type of the target argument.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <param name="target">Only necessary if the attributeType is an <see cref="IClassForClassAttribute" />. In this case only
    /// those attribute instances are returned, where the target object meets the target type of the <see cref="IClassForClassAttribute" />.</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static IAttributeForClassList GetAttributeInstancesAndClassTypesForClass(System.Type attributeType, object target, ref IAttributeForClassListCollection cachedList)
    {
      return GetAttributeInstancesAndClassTypesForClass(attributeType, target, null, ref cachedList);
    }

  
    public interface IAttributeForClassList
    {
      KeyValuePair<Attribute, System.Type> this[int i] { get; }
      int Count { get; }
    }

    public interface IAttributeForClassListCollection
    {
    }

    private class AttributeForClassList : IAttributeForClassList
    {
      private class DictEntryKeyComparer : IComparer<KeyValuePair<Attribute,System.Type>>
      {
        #region IComparer Members

        public int Compare(KeyValuePair<Attribute, System.Type> x, KeyValuePair<Attribute, System.Type> y)
        {
          IComparable xx = (IComparable)x.Key;
          IComparable yy = (IComparable)y.Key;

          return xx.CompareTo(yy);
        }

        #endregion
      }

      /// <summary>
      /// How many assemblies are currently cached into this list.
      /// </summary>
      public int _currentAssemblyCount;

      /// <summary>
      /// The type of the attribute this assembly caches.
      /// </summary>
      public System.Type _attributeType;

      public System.Type _targetType;

      bool _isSortable;

      List<KeyValuePair<Attribute, System.Type>> _list;

      public AttributeForClassList(System.Type attributeType, System.Type targettype)
      {
        _attributeType = attributeType;
        _targetType = targettype;
        _isSortable = IsSubClassOfOrImplements(_attributeType, typeof(IComparable));
      }



      public void Add(Attribute attr, System.Type target)
      {
        if (null == _list)
          _list = new List<KeyValuePair<Attribute, Type>>();
        _list.Add(new KeyValuePair<Attribute, Type>(attr, target));
      }


      public void Sort()
      {
        if (_list!=null && _list.Count > 1 && _isSortable)
          _list.Sort(new DictEntryKeyComparer());
      }

      #region IAttributeForClassList Members

      public KeyValuePair<Attribute, Type> this[int i]
      {
        get { return _list[i]; }
      }

      public int Count
      {
        get { return _list.Count; }
      }

      #endregion
    }

    private class AttributeForClassListCollection : IAttributeForClassListCollection
    {
      /// <summary>
      /// The attribute type this list collection is intended for.
      /// </summary>
      private System.Type _attributeType;
      private List<List<System.Type>> _classesWithMyAttribute = new List<List<Type>>();
      private Dictionary<System.Type, AttributeForClassList> _attributeForClassListCollection;

      public AttributeForClassListCollection(System.Type attributeType)
      {
        _attributeType = attributeType;
      }

      public Type AttributeType { get { return _attributeType; } }
      public int CurrentAssemblyCount { get { return _classesWithMyAttribute.Count; } }


      /// <summary>
      /// Get all classes with the attribute <c>AttributeType</c> starting from a certain assembly.
      /// </summary>
      /// <param name="startAssembly">Index of the first assembly to search for.</param>
      /// <returns>A list of all classes that have the attribute.</returns>
      public List<Type> GetClasses(int startAssembly)
      {
        Update();
        List<Type> list = new List<Type>();
        for (int i = startAssembly; i < _classesWithMyAttribute.Count; i++)
        {
          list.AddRange(_classesWithMyAttribute[i]);
        }
        return list;
      }

      public IAttributeForClassList GetAttributeForClassList(Type myTargetType)
      {
        AttributeForClassList list;
        if (_attributeForClassListCollection == null)
          _attributeForClassListCollection = new Dictionary<Type, AttributeForClassList>();

        if (_attributeForClassListCollection.ContainsKey(myTargetType))
        {
          list = _attributeForClassListCollection[myTargetType];
        }
        else
        {
          list = new AttributeForClassList(_attributeType, myTargetType);
          _attributeForClassListCollection.Add(myTargetType, list);
        }

        if (this._attributeType != list._attributeType)
          throw new ApplicationException("Programming error (attributeType did not match), please inform the author that this exception happened");
        if (list._targetType != myTargetType)
          throw new ApplicationException("Programming error (targetType did not match), please inform the author that this exception happened");

        List<Type> definedtypes = GetClasses(list._currentAssemblyCount);
        list._currentAssemblyCount = CurrentAssemblyCount;

        foreach (Type definedtype in definedtypes)
        {
          Attribute[] attributes = Attribute.GetCustomAttributes(definedtype, _attributeType);

          foreach (Attribute att in attributes)
          {
            if (att is IClassForClassAttribute)
            {
                         
                if (IsSubClassOfOrImplements(myTargetType, ((IClassForClassAttribute)att).TargetType))
                  list.Add(att, definedtype);
            }
          }
        } // end foreach type
        list.Sort();
        return list;
      }

      void Update()
      {
        for (int i = _classesWithMyAttribute.Count; i < _loadedAssemblies.Count; i++)
        {
          Assembly assembly = _loadedAssemblies[i];
          List<Type> typesWithMyAttribute = new List<Type>();
          if (IsDependentAssembly(_attributeType.Assembly, assembly))
          {
            Type[] definedtypes = assembly.GetTypes();
            foreach (Type definedtype in definedtypes)
            {
              Attribute[] attributes = Attribute.GetCustomAttributes(definedtype, _attributeType);
              if (attributes.Length > 0)
                typesWithMyAttribute.Add(definedtype);
            }
          }
          _classesWithMyAttribute.Add(typesWithMyAttribute);
        }
        System.Diagnostics.Debug.Assert(_loadedAssemblies.Count == _classesWithMyAttribute.Count);
      }
    }


    /// <summary>
    /// For a given type of attribute, attributeType, this function returns the attribute instances and the class
    /// types this attributes apply to. If the attribute implements the IComparable interface, the list is sorted. The attribute has
    /// to implement the <see cref="IClassForClassAttribute" /> interface, and only those attributes are considered, for which the
    /// <see cref="IClassForClassAttribute.TargetType" /> match the type of the target argument.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <param name="target">Only necessary if the attributeType is an <see cref="IClassForClassAttribute" />. In this case only
    /// those attribute instances are returned, where the target object meets the target type of the <see cref="IClassForClassAttribute" />.</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static IAttributeForClassList GetAttributeInstancesAndClassTypesForClass(System.Type attributeType, object target, System.Type overrideObjectType, ref IAttributeForClassListCollection cachedList)
    {
      System.Diagnostics.Debug.Assert(IsSubClassOfOrImplements(attributeType,typeof(IClassForClassAttribute)));
      System.Type myTargetType = overrideObjectType != null ? overrideObjectType : target.GetType();

      AttributeForClassListCollection listColl = cachedList as AttributeForClassListCollection;
      if (listColl == null)
      {
        cachedList = listColl = new AttributeForClassListCollection(attributeType);
      }
      else
      {
        if (listColl.AttributeType != attributeType)
          throw new ApplicationException("Programming error (attributeType did not match), please inform the author that this exception happened");
      }

      return listColl.GetAttributeForClassList(myTargetType);

      /*
       AttributeForClassList list;
       if (listColl.ContainsKey(myTargetType))
       {
         list = listColl[myTargetType];
       }
       else
       {
         list = new AttributeForClassList(attributeType,myTargetType);
         listColl.Add(myTargetType, list);
       }

         if (attributeType != list._attributeType)
           throw new ApplicationException("Programming error (attributeType did not match), please inform the author that this exception happened");
         if (list._targetType != (overrideObjectType != null ? overrideObjectType : target.GetType()))
           throw new ApplicationException("Programming error (targetType did not match), please inform the author that this exception happened");
     
        List<Type> definedtypes = listColl.GetClasses(list._currentAssemblyCount);
        list._currentAssemblyCount = listColl.CurrentAssemblyCount;
       
        foreach (Type definedtype in definedtypes)
        {
          Attribute[] attributes = Attribute.GetCustomAttributes(definedtype, attributeType);

          foreach (Attribute att in attributes)
          {
            if (att is IClassForClassAttribute)
            {
              if (overrideObjectType == null)
              {
                if (((IClassForClassAttribute)att).TargetType.IsInstanceOfType(target))
                  list.Add(att, definedtype);
              }
              else
              {
                if (IsSubClassOfOrImplements(overrideObjectType, ((IClassForClassAttribute)att).TargetType))
                  list.Add(att, definedtype);
              }
            }
          }
        } // end foreach type
   

      
      list.Sort();

      return list;
       */
    }

    /// <summary>
    /// Tries to get a class instance for a given attribute type. All loaded assemblies are searched for classes that attributeType applies to,
    /// then for all found classes the instantiation of a class is tried, until a instance is created successfully.
    /// </summary>
    /// <param name="attributeType">The type of attribute  the class(es) to instantiate must be assigned to.</param>
    /// <param name="expectedType">The expected type of return value.</param>
    /// <param name="creationArgs">The creation arguments used to instantiate a class.</param>
    /// <returns>The instance of the first class for which the instantiation was successfull and results in the expectedType. Otherwise null.</returns>
    public static object GetClassInstanceByAttribute(System.Type attributeType, System.Type expectedType, object[] creationArgs)
    {
      object result=null;
      // 1st search for all classes that wear the UserControllerForObject attribute
      DictionaryEntry[] list = ReflectionService.GetAttributeInstancesAndClassTypes(attributeType);

      for(int i=list.Length-1;i>=0;i--)
      {
        if(IsSubClassOfOrImplements( ((System.Type)list[i].Value),expectedType))
        {
          // try to create the class
          try
          {
            result = Activator.CreateInstance((System.Type)list[i].Value,creationArgs);
            break;
          }
          catch(Exception)
          {
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Tries to get a class instance for a given attribute type. All loaded assemblies are searched for classes that attributeType applies to,
    /// then for all found classes the instantiation of a class is tried, until a instance is created successfully. Here, the attributeType has
    /// to implement <see cref="IClassForClassAttribute" />, and creationArg[0] has to match the type in <see cref="IClassForClassAttribute.TargetType" />
    /// </summary>
    /// <param name="attributeType">The type of attribute  the class(es) to instantiate must be assigned to.</param>
    /// <param name="expectedType">The expected type of return value.</param>
    /// <param name="creationArgs">The creation arguments used to instantiate a class.</param>
    /// <returns>The instance of the first class for which the instantiation was successfull and results in the expectedType. Otherwise null.</returns>
    /// <remarks>The instantiation is tried first with the full argument list. If that fails, the last element of the argument list is chopped and the instantiation is tried again.
    /// This process is repeated until the instantiation was successfull or the argument list is empty (empty constructor is tried at last).</remarks>
    public static object GetClassForClassInstanceByAttribute(System.Type attributeType, System.Type expectedType, object[] creationArgs, ref IAttributeForClassListCollection cachedList)
    {
      return GetClassForClassInstanceByAttribute(attributeType, expectedType,  creationArgs, null, ref cachedList);
    }
   

    /// <summary>
    /// Tries to get a class instance for a given attribute type. All loaded assemblies are searched for classes that attributeType applies to,
    /// then for all found classes the instantiation of a class is tried, until a instance is created successfully. Here, the attributeType has
    /// to implement <see cref="IClassForClassAttribute" />, and creationArg[0] has to match the type in <see cref="IClassForClassAttribute.TargetType" />
    /// </summary>
    /// <param name="attributeType">The type of attribute  the class(es) to instantiate must be assigned to.</param>
    /// <param name="expectedType">The expected type of return value.</param>
    /// <param name="creationArgs">The creation arguments used to instantiate a class.</param>
    /// <param name="overrideArgs0Type">Usually null. If you provide a type here, it has to be a base type of the typeof(creationArgs[0]). By this you
    /// can "downgrade" creationArgs[0], so that only attributes for the base type are looked for.</param>
    /// <returns>The instance of the first class for which the instantiation was successfull and results in the expectedType. Otherwise null.</returns>
    /// <remarks>The instantiation is tried first with the full argument list. If that fails, the last element of the argument list is chopped and the instantiation is tried again.
    /// This process is repeated until the instantiation was successfull or the argument list is empty (empty constructor is tried at last).</remarks>
    public static object GetClassForClassInstanceByAttribute(System.Type attributeType, System.Type expectedType, object[] creationArgs, System.Type overrideArgs0Type, ref IAttributeForClassListCollection cachedList)
    {
      object result=null;

     


      // 1st search for all classes that wear the UserControllerForObject attribute
      IAttributeForClassList list = ReflectionService.GetAttributeInstancesAndClassTypesForClass(attributeType,creationArgs[0],overrideArgs0Type, ref cachedList);

      System.Type[][] creationTypes = new Type[creationArgs.Length+1][];
      
   


      for(int i=list.Count-1;i>=0;i--)
      {
        if(!IsSubClassOfOrImplements( (System.Type)list[i].Value,expectedType))
          continue;
        // try to create the class

        System.Type type = (System.Type)list[i].Value;

        //ConstructorInfo[] cinfos = type.GetConstructors();

        for (int j = creationArgs.Length; j >= 0; j--)
        {
          if(creationTypes[j]==null)
          {
            creationTypes[j]=new Type[j];
             for(int k=0;k<j;k++)
                creationTypes[j][k] = creationArgs[k].GetType();
          }

          ConstructorInfo cinfo = type.GetConstructor(creationTypes[j]);
          if (cinfo != null)
          {
            object[] chopped = null;
            if (j < creationArgs.Length)
            {
              chopped = new object[j];
              Array.Copy(creationArgs, chopped, j);
            }

            result = cinfo.Invoke(j==creationArgs.Length ? creationArgs : chopped);

            if (result != null)
              return result;
          }
        }
      }
      return result;
    }
  }
}

