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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Altaxo.Gui;

namespace Altaxo.Main.Services
{

  /// <summary>
  /// Static functions for searching by attributes.
  /// </summary>
  public class ReflectionService
  {
    #region Construction

    static List<Assembly> _loadedAssemblies;
    static SubClassTypeListCollection _subClassTypeListCollection;
    static ClassesHavingAttributeListCollection _classesHavingAttributeCollection;
    static ReflectionService()
    {
      AppDomain currentDomain = AppDomain.CurrentDomain;
      _loadedAssemblies = new List<Assembly>();
      _loadedAssemblies.AddRange(currentDomain.GetAssemblies());
      currentDomain.AssemblyLoad += new AssemblyLoadEventHandler(EhAssemblyLoaded);
      _subClassTypeListCollection = new SubClassTypeListCollection();
      _classesHavingAttributeCollection = new ClassesHavingAttributeListCollection();
    }

    /// <summary>
    /// Called when a new assembly is loaded into our current application domain.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    static void EhAssemblyLoaded(object sender, AssemblyLoadEventArgs e)
    {
      _loadedAssemblies.Add(e.LoadedAssembly);
    }

    #endregion

    #region Subclass checking and searching


    /// <summary>
    /// Determines whether or not a given subtype is derived from a basetype or implements the interface basetype.
    /// </summary>
    /// <param name="subtype">The subtype.</param>
    /// <param name="basetype">The basetype.</param>
    /// <returns>If basetype is a class type, the return value is true if subtype derives from basetype. If basetype is an interface, the return value is true if subtype implements the interface basetype. In all other cases the return value is false.</returns>
    public static bool IsSubClassOfOrImplements(System.Type subtype, System.Type basetype)
    {
      if (basetype.IsInterface)
      {
        if (subtype == basetype)
          return true;
        else
          return null != subtype.GetInterface(basetype.ToString());
        //return Array.IndexOf(subtype.GetInterfaces(),basetype)>=0;
      }
      else
      {
        return subtype.IsSubclassOf(basetype) || subtype == basetype;
      }

    }


    /// <summary>
    /// Determines whether or not a given subtype is derived from a basetype or implements the interface basetypes.
    /// </summary>
    /// <param name="subtype">The subtype.</param>
    /// <param name="basetypes">The basetypes. To return true, the subclass must implement all the types given here.</param>
    /// <returns>If basetype is a class type, the return value is true if subtype derives from basetype. If basetype is an interface, the return value is true if subtype implements the interface basetype.
    /// If subtype don't implement one of the types given in basetypes, the return value is false.</returns>
    public static bool IsSubClassOfOrImplements(System.Type subtype, System.Type[] basetypes)
    {
      foreach (System.Type t in basetypes)
      {
        if (!IsSubClassOfOrImplements(subtype, t))
          return false;
      }
      return true;
    }



    #endregion

    #region Subclass searching

    /// <summary>
    /// This will return a list of types that are subclasses of type basetype or (when basetype is an interface)
    /// implements basetype.
    /// </summary>
    /// <param name="basetype">The basetype.</param>
    /// <returns></returns>
    public static System.Type[] GetSubclassesOf(System.Type basetype)
    {
      return GetSubclassesOf(new Type[] { basetype });
    }


    /// <summary>
    /// This will return a list of types that are subclasses of all types in basetypes or (when basetype[i] is an interface)
    /// implements basetypes[i].
    /// </summary>
    /// <param name="basetypes">The basetypes.</param>
    /// <returns></returns>
    public static System.Type[] GetSubclassesOf(System.Type[] basetypes)
    {
      SubClassTypeList tlist = _subClassTypeListCollection[basetypes];
      List<Type> list = new List<Type>();
      list.AddRange(tlist);
      return list.ToArray();
    }

    /// <summary>
    /// This will return a list of types that are subclasses of type basetype or (when basetype is an interface)
    /// implements basetype.
    /// </summary>
    /// <param name="basetype">The basetype.</param>
    /// <returns></returns>
    public static System.Type[] GetNonAbstractSubclassesOf(System.Type basetype)
    {
      return GetNonAbstractSubclassesOf(new System.Type[] { basetype });
    }

    /// <summary>
    /// This will return a list of types that are subclasses of type basetype or (when basetype is an interface)
    /// implements basetype.
    /// </summary>
    /// <param name="basetypes">The basetype.</param>
    /// <returns></returns>
    public static System.Type[] GetNonAbstractSubclassesOf(System.Type[] basetypes)
    {
      SubClassTypeList tlist = _subClassTypeListCollection[basetypes];

      List<Type> list = new List<Type>();

      foreach (Type definedtype in tlist)
      {
        if (!definedtype.IsAbstract)
          list.Add(definedtype);
      } // end foreach type
      return list.ToArray();
    }

    #endregion

    #region Subclass lists

    /// <summary>
    /// Holds a list of class types that implement a given base type. If the base type is a class type (i.e. not an interface
    /// type), the base type itself is also included in this list.
    /// </summary>
    private class SubClassTypeList : IEnumerable<Type>
    {
      /// <summary>
      /// How many assemblies are currently cached into this list.
      /// </summary>
      int _currentAssemblyCount;

      System.Type _baseType;
      System.Type[] _moreTypes;

      List<System.Type> _listOfTypes;
      List<int> _listOfAssemblies;

      SubClassTypeListCollection _parent;

      public SubClassTypeList(SubClassTypeListCollection parent, System.Type[] basetypes)
      {
        _baseType = basetypes[0];
        _listOfTypes = new List<Type>();
        _parent = parent;

        if (basetypes.Length == 1)
        {
          _listOfAssemblies = new List<int>();
        }
        else
        {
          _moreTypes = new Type[basetypes.Length - 1];
          Array.Copy(basetypes, 1, _moreTypes, 0, basetypes.Length - 1);
        }

      }

      private void UpdateForOneBasetype()
      {
        int loadedAssemblyCount = _loadedAssemblies.Count;
        if (_currentAssemblyCount == loadedAssemblyCount)
          return;

        Assembly baseassembly = _baseType.Assembly;
        for (int i = _currentAssemblyCount; i < loadedAssemblyCount; i++)
        {
          Assembly assembly = _loadedAssemblies[i];
          if (!IsDependentAssembly(baseassembly, assembly))
            continue;

          Type[] definedtypes = assembly.GetTypes();
          foreach (Type definedtype in definedtypes)
          {
            if (IsSubClassOfOrImplements(definedtype, _baseType))
            {
              _listOfTypes.Add(definedtype);
              _listOfAssemblies.Add(i);
            }
          }
        }
        _currentAssemblyCount = loadedAssemblyCount;
      }

      private void UpdateForManyBasetypes()
      {
        int loadedAssemblyCount = _loadedAssemblies.Count;
        if (_currentAssemblyCount == loadedAssemblyCount)
          return;

        SubClassTypeList tl = _parent[new Type[] { _baseType }];

        foreach (Type definedtype in tl.GetSubTypeRange(_currentAssemblyCount, loadedAssemblyCount - _currentAssemblyCount))
        {
          if (IsSubClassOfOrImplements(definedtype, _moreTypes))
            _listOfTypes.Add(definedtype);
        }

        _currentAssemblyCount = loadedAssemblyCount;
      }

      public void Update()
      {
        if (null != _moreTypes)
          UpdateForManyBasetypes();
        else
          UpdateForOneBasetype();
      }

      public IEnumerator<Type> GetEnumerator()
      {
        return _listOfTypes.GetEnumerator();
      }
      IEnumerator IEnumerable.GetEnumerator()
      {
        return _listOfTypes.GetEnumerator();
      }


      IEnumerable<Type> GetSubTypeRange(int firstAssembly, int count)
      {
        int nextAssembly = firstAssembly + count;
        int upper, lower;
        for (upper = _listOfAssemblies.Count - 1; upper >= 0; upper--)
          if (_listOfAssemblies[upper] < nextAssembly)
            break;

        for (lower = upper; lower >= 0; lower--)
          if (_listOfAssemblies[lower] < firstAssembly)
            break;

        if (lower < 0) lower = 0;

        for (int i = lower; i <= upper; i++)
          yield return _listOfTypes[i];
      }

      public int Count
      {
        get { return _listOfTypes == null ? 0 : _listOfTypes.Count; }
      }






    }

    private class SubClassTypeListCollection
    {
      private class TypeArrayComparer : IEqualityComparer<Type[]>
      {
        #region IEqualityComparer<Type[]> Members

        public bool Equals(Type[] x, Type[] y)
        {
          if (x.Length != y.Length)
            return false;
          for (int i = 0; i < x.Length; i++)
            if (x[i] != y[i])
              return false;

          return true;
        }

        public int GetHashCode(Type[] obj)
        {
          int result = 0;
          foreach (Type t in obj)
            result += t.GetHashCode();

          return result;
        }

        #endregion
      }

      Dictionary<Type[], SubClassTypeList> _list = new Dictionary<Type[], SubClassTypeList>(new TypeArrayComparer());

      public SubClassTypeList this[Type[] types]
      {
        get
        {
          if (!_list.ContainsKey(types))
            CreateList(types);

          SubClassTypeList result = _list[types];
          result.Update();
          return result;
        }
      }

      private void CreateList(Type[] types)
      {
        SubClassTypeList list = new SubClassTypeList(this, types);
        list.Update();
        _list.Add(types, list);
      }


    }

    #endregion

    #region Assembly dependency checking

    /// <summary>
    /// Determines whether or not a given AssemblyName is contained in a list of names.
    /// This is done here by comparing the FullNames.
    /// </summary>
    /// <param name="assemblyNames">List of AssemblyNames.</param>
    /// <param name="searchedName">The AssemblyName for which to determine if it is contained in the list.</param>
    /// <returns>True if it is contained in the list.</returns>
    public static bool Contains(AssemblyName[] assemblyNames, AssemblyName searchedName)
    {
      foreach (AssemblyName assName in assemblyNames)
      {
        if (assName.FullName == searchedName.FullName)
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
        List<Assembly> list = new List<Assembly>();

        AssemblyName baseAssemblyName = baseAssembly.GetName();

        for (int i = start; i < _loadedAssemblies.Count; i++)
        {
          Assembly assembly = _loadedAssemblies[i];
          if (Contains(assembly.GetReferencedAssemblies(), baseAssemblyName))
            list.Add(assembly);
          else if (assembly == baseAssembly)
            list.Add(assembly);
        }
        return list.ToArray();
      }
    }

    /// <summary>
    /// Gets a list of currently loaded assemblies that are dependend on the given base assembly. The base assembly is also in the returned list.
    /// </summary>
    /// <param name="baseAssembly">The base assembly.</param>
    /// <param name="assembliesToTest">All assemblies that should be tested for dependence on the base assembly.</param>
    /// <returns>All assemblies, that are currently loaded and that references the given base assembly. The base assembly is also in the returned list.</returns>
    public static IEnumerable<Assembly> GetDependendAssemblies(Assembly baseAssembly, IEnumerable<Assembly> assembliesToTest)
    {
      List<Assembly> list = new List<Assembly>();

      AssemblyName baseAssemblyName = baseAssembly.GetName();

      foreach (Assembly assembly in assembliesToTest)
      {
        if (Contains(assembly.GetReferencedAssemblies(), baseAssemblyName))
          list.Add(assembly);
        else if (assembly == baseAssembly)
          list.Add(assembly);
      }
      return list;
    }



    /// <summary>
    /// Gets a list of currently loaded assemblies that are dependend on the given types. This includes also the assembly itself, where the type(s) are defined, and all
    /// assemblies dependent on that.
    /// </summary>
    /// <param name="types">One or more types.</param>
    /// <param name="start">Index into the <c>_loadedAssemblies</c> array where to start the search. Set it to 0 if you want a full search.</param>
    /// <returns>All assemblies, that are currently loaded and that references the given base assembly. The base assembly is also in the returned list.</returns>
    public static System.Reflection.Assembly[] GetDependendAssemblies(System.Type[] types, int start)
    {
      if (start >= _loadedAssemblies.Count)
      {
        return new Assembly[0];
      }
      else
      {
        List<Assembly> list = new List<Assembly>();

        List<AssemblyName> nameList = new List<AssemblyName>();
        foreach (Type t in types)
        {
          AssemblyName name = Assembly.GetAssembly(t).GetName();
          if (!nameList.Contains(name))
            nameList.Add(name);
        }
        for (int i = start; i < _loadedAssemblies.Count; i++)
        {
          Assembly testassembly = _loadedAssemblies[i];
          AssemblyName testassemblyname = testassembly.GetName();
          foreach (AssemblyName name in nameList)
          {
            if (testassemblyname.FullName == name.FullName || Contains(testassembly.GetReferencedAssemblies(), name))
            {
              list.Add(testassembly);
              break;
            }
          }
        }
        return list.ToArray();
      }
    }

    /// <summary>
    /// Returns true if <c>testAssembly</c> is dependent on <c>baseAssembly.</c>
    /// </summary>
    /// <param name="baseAssembly">Base assembly.</param>
    /// <param name="testAssembly">Assembly to test.</param>
    /// <returns>True if <c>testAssembly</c> is dependent on <c>baseAssembly</c>.</returns>
    public static bool IsDependentAssembly(Assembly baseAssembly, Assembly testAssembly)
    {
      return baseAssembly == testAssembly || Contains(testAssembly.GetReferencedAssemblies(), baseAssembly.GetName());
    }

    #endregion

    #region Attribute handling - class searching

    public interface IAttributeForClassList : IEnumerable<KeyValuePair<Attribute,Type>>
    {
      IEnumerable<Type> Types { get; }
    }


    private class AttributeDictEntryComparer : IComparer<KeyValuePair<Attribute, Type>>
    {
      public int Compare(KeyValuePair<Attribute, Type> x, KeyValuePair<Attribute, Type> y)
      {
        IComparable xx = (IComparable)x.Key;
        IComparable yy = (IComparable)y.Key;

        return xx.CompareTo(yy);
      }
    }


    /// <summary>
    /// For a given type of attribute, attributeType, this function returns the class
    /// types this attributes apply to. The list is not sorted.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <param name="inherit">If true, takes into account also classes that have the attribute not applied directly, but inherited from a base class.</param>
    /// <returns>A list of types that have the provided attribute type.</returns>
    public static IEnumerable<Type> GetUnsortedClassTypesHavingAttribute(System.Type attributeType, bool inherit)
    {
      return _classesHavingAttributeCollection[attributeType].GetAllClassesWithMyAttribute();
    }


    /// <summary>
    /// For a given type of attribute, attributeType, this function returns the class
    /// types this attributes apply to. The list is sorted if attributeType implements the IComparable interface.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <param name="inherit">If true, classes where taken into account, where the attribute is not applied directly to that class, but to a base class of that class.</param>
    /// <returns>A list of types that have the provided attribute type.</returns>
    public static IEnumerable<Type> GetSortedClassTypesHavingAttribute(System.Type attributeType, bool inherit)
    {
      if(IsSubClassOfOrImplements(attributeType,typeof(IComparable)))
      {
        IEnumerable<Type> types = _classesHavingAttributeCollection[attributeType].GetAllClassesWithMyAttribute();
        SortedDictionary<Attribute, Type> _sortedList = new SortedDictionary<Attribute,Type>();
        foreach (Type definedtype in types)
        {
          Attribute.GetCustomAttributes(definedtype, attributeType, inherit);
          object[] attributes = definedtype.GetCustomAttributes(attributeType, inherit);
          foreach (Attribute attribute in attributes)
            _sortedList.Add(attribute, definedtype);
        }
        return new List<Type>(_sortedList.Values);
      }
      else
      {
        return GetUnsortedClassTypesHavingAttribute(attributeType, inherit);        
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
    /// <param name="cachedList">Cached list of already observed classes.</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static IAttributeForClassList GetAttributeInstancesAndClassTypesForClass(System.Type attributeType, object target, ref IAttributeForClassListCollection cachedList)
    {
      return GetAttributeInstancesAndClassTypesForClass(attributeType, target, null);
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
    /// <param name="overrideObjectType">If the target is null, or under special circumstances, the type to look for deviates from the type 
    /// provided by the <c>target</c> argument. In this cases you can provide a override type that is used.</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static IAttributeForClassList GetAttributeInstancesAndClassTypesForClass(
      System.Type attributeType,
      object target,
      System.Type overrideObjectType)
    {
      System.Diagnostics.Debug.Assert(IsSubClassOfOrImplements(attributeType, typeof(IClassForClassAttribute)));
      System.Type myTargetType = overrideObjectType != null ? overrideObjectType : target.GetType();
      return _classesHavingAttributeCollection[attributeType, myTargetType];
    }

    #endregion

    #region Attribute handling - class instantiating

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
      object result = null;
      // 1st search for all classes that wear the UserControllerForObject attribute
      IEnumerable<Type> list = ReflectionService.GetSortedClassTypesHavingAttribute(attributeType, false);

      foreach(Type definedType in list)
      {
        if (IsSubClassOfOrImplements(definedType, expectedType))
        {
          // try to create the class
          try
          {
            result = Activator.CreateInstance(definedType, creationArgs);
            break;
          }
          catch (Exception)
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
    /// <param name="expectedTypes">The expected type(s) of return value.</param>
    /// <param name="creationArgs">The creation arguments used to instantiate a class.</param>
    /// <returns>The instance of the first class for which the instantiation was successfull and results in the expectedType. Otherwise null.</returns>
    /// <remarks>The instantiation is tried first with the full argument list. If that fails, the last element of the argument list is chopped and the instantiation is tried again.
    /// This process is repeated until the instantiation was successfull or the argument list is empty (empty constructor is tried at last).</remarks>
    public static object GetClassForClassInstanceByAttribute(System.Type attributeType, System.Type[] expectedTypes, object[] creationArgs)
    {
      return GetClassForClassInstanceByAttribute(attributeType, expectedTypes, creationArgs, null);
    }



    /// <summary>
    /// Tries to get a class instance for a given attribute type. All loaded assemblies are searched for classes that attributeType applies to,
    /// then for all found classes the instantiation of a class is tried, until a instance is created successfully. Here, the attributeType has
    /// to implement <see cref="IClassForClassAttribute" />, and creationArg[0] has to match the type in <see cref="IClassForClassAttribute.TargetType" />
    /// </summary>
    /// <param name="attributeType">The type of attribute  the class(es) to instantiate must be assigned to.</param>
    /// <param name="expectedTypes">The expected type of return value.</param>
    /// <param name="creationArgs">The creation arguments used to instantiate a class.</param>
    /// <param name="overrideArgs0Type">Usually null. If you provide a type here, it has to be a base type of the typeof(creationArgs[0]). By this you
    /// can "downgrade" creationArgs[0], so that only attributes for the base type are looked for.</param>
    /// <returns>The instance of the first class for which the instantiation was successfull and results in the expectedType. Otherwise null.</returns>
    /// <remarks>The instantiation is tried first with the full argument list. If that fails, the last element of the argument list is chopped and the instantiation is tried again.
    /// This process is repeated until the instantiation was successfull or the argument list is empty (empty constructor is tried at last).</remarks>
    public static object GetClassForClassInstanceByAttribute(System.Type attributeType, System.Type[] expectedTypes, object[] creationArgs, System.Type overrideArgs0Type)
    {
      // 1st search for all classes that wear the UserControllerForObject attribute
      IAttributeForClassList list = ReflectionService.GetAttributeInstancesAndClassTypesForClass(attributeType, creationArgs[0], overrideArgs0Type);
      return CreateInstanceFromList(list, expectedTypes, creationArgs);
    }

    /// <summary>
    /// Tries to get a class instance for a given attribute type. All loaded assemblies are searched for classes that attributeType applies to,
    /// then for all found classes the instantiation of a class is tried, until a instance is created successfully. Here, the attributeType has
    /// to implement <see cref="IClassForClassAttribute" />, and creationArg[0] has to match the type in <see cref="IClassForClassAttribute.TargetType" />
    /// </summary>
    /// <param name="list">The list of types to try for creation.</param>
    /// <param name="expectedTypes">The expected types of return value.</param>
    /// <param name="creationArgs">The creation arguments used to instantiate a class.</param>
    /// <returns>The instance of the first class for which the instantiation was successfull and results in the expectedType. Otherwise null.</returns>
    /// <remarks>The instantiation is tried first with the full argument list. If that fails, the last element of the argument list is chopped and the instantiation is tried again.
    /// This process is repeated until the instantiation was successfull or the argument list is empty (empty constructor is tried at last).</remarks>
    public static object CreateInstanceFromList(IAttributeForClassList list, System.Type[] expectedTypes, object[] creationArgs)
    {
      object result = null;

      // evaluate the len of the creation args without null's as arguments
      int trueArgLen = creationArgs.Length;
      for (int i = 0; i < creationArgs.Length; i++)
      {
        if (creationArgs[i] == null)
        {
          trueArgLen = i;
          break;
        }
      }

      System.Type[][] creationTypes = new Type[trueArgLen + 1][];

      foreach(Type definedType in list.Types)
      {
        if (!IsSubClassOfOrImplements(definedType, expectedTypes))
          continue;
        // try to create the class

        for (int j = trueArgLen; j >= 0; j--)
        {
          if (creationTypes[j] == null)
          {
            creationTypes[j] = new Type[j];
            for (int k = j - 1; k >= 0; k--)
            {
              creationTypes[j][k] = creationArgs[k].GetType();
            }
          }

          ConstructorInfo cinfo = definedType.GetConstructor(creationTypes[j]);
          if (cinfo != null)
          {
            object[] chopped = null;
            if (j < creationArgs.Length)
            {
              chopped = new object[j];
              Array.Copy(creationArgs, chopped, j);
            }

            result = cinfo.Invoke(j == creationArgs.Length ? creationArgs : chopped);

            if (result != null)
              return result;
          }
        }
      }
      return result;
    }

    #endregion

    #region Attribute lists




    /// <summary>
    /// Maintains all classes that have a special attribute.
    /// </summary>
    private class ClassesHavingAttributeList
    {
      /// <summary>
      /// The attribute type this list collection is intended for.
      /// </summary>
      /// <example>Most used in Altaxo is the UserControllerForObject attribute.</example>
      protected System.Type _attributeType;

      /// <summary>
      /// Maintains a list of all classes in all assemblies that have the attribute of type _attributeType applied.
      /// Outer list is numbered by the assemblies which contains the classes. Inner list is the list of class types that have the attribute.
      /// </summary>
      protected List<List<System.Type>> _classesWithMyAttribute = new List<List<Type>>();


      public ClassesHavingAttributeList(System.Type attributeType)
      {
        _attributeType = attributeType;
      }

      public Type AttributeType { get { return _attributeType; } }
      public int CurrentAssemblyCount { get { return _classesWithMyAttribute.Count; } }


      /// <summary>
      /// Get all classes with the attribute <c>AttributeType</c>.
      /// </summary>
      /// <returns>A list of all classes that have the attribute.</returns>
      public IEnumerable<Type> GetAllClassesWithMyAttribute()
      {
        return GetClassesWithMyAttribute(0);
      }

      /// <summary>
      /// Get all classes with the attribute <c>AttributeType</c> starting from a certain assembly.
      /// </summary>
      /// <param name="startAssembly">Index of the first assembly to search for.</param>
      /// <returns>A list of all classes that have the attribute.</returns>
      public IEnumerable<Type> GetClassesWithMyAttribute(int startAssembly)
      {
        Update();
        List<Type> list = new List<Type>();
        for (int i = startAssembly; i < _classesWithMyAttribute.Count; i++)
        {
          list.AddRange(_classesWithMyAttribute[i]);
        }
        return list;
      }

      public void Update()
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
              Attribute[] attributes = Attribute.GetCustomAttributes(definedtype, _attributeType, true);
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
    /// Maintains a list of ClassForClass attributes and the appropriate target types of this attribute.
    /// If the attribute type allows sorting, the list is sorted.
    /// </summary>
    /// <example>A prominent class-for-class attribute type is the 
    /// UserControllerForObjectAttribute attribute.
    /// The list is collection all classes with the same target type (for instance some plot item type).
    /// The list maintains the class types (i.e. all controllers that can cope with the plot item) and the
    /// instance of the attribute that this class is applied to.
    ///</example>
    private class ClassesHavingCfCAttributeTargetingTypeList : IAttributeForClassList
    {

      /// <summary>
      /// How many assemblies are currently cached into this list.
      /// </summary>
      public int _currentAssemblyCount;

      /// <summary>
      /// The type of the attribute this assembly caches.
      /// </summary>
      /// <example>A prominent class-for-class attribute type is the 
      /// UserControllerForObjectAttribute attribute.</example>
      public System.Type _attributeType;

      /// <summary>
      /// The class type the attribute targets.
      /// </summary>
      public System.Type _targetType;

      bool _isSortable;

      /// <summary>
      /// Maintains a list of attribute instances and the class type this attribute is applied to.
      /// </summary>
      List<KeyValuePair<Attribute, System.Type>> _list;

      public ClassesHavingCfCAttributeTargetingTypeList(System.Type attributeType, System.Type targettype)
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
        if (_list != null && _list.Count > 1 && _isSortable)
          _list.Sort(new AttributeDictEntryComparer());
      }
     
      #region IEnumerable<KeyValuePair<Attribute,Type>> Members

      public IEnumerator<KeyValuePair<Attribute, Type>> GetEnumerator()
      {
        return _list.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      IEnumerator IEnumerable.GetEnumerator()
      {
        return _list.GetEnumerator();
      }

      #endregion

      #region IAttributeForClassList Members

      public IEnumerable<Type> Types
      {
        get
        {
          if (_list != null)
          {
            foreach (KeyValuePair<Attribute, Type> entry in _list)
              yield return entry.Value;
          }
        }
      }

      #endregion
    }

    /// <summary>
    /// This interface is merely for filtering the argument of different functions.
    /// </summary>
    public interface IAttributeForClassListCollection
    {
    }

    private class ClassesHavingClassForClassAttributeList : ClassesHavingAttributeList, IAttributeForClassListCollection
    {
      /// <summary>
      /// Dictionary: Key is the target type of the attribute. Value is a list of objects that have the attribute
      /// of type _attributeType, and this attribute targets the type given in the key.
      /// </summary>
      private Dictionary<System.Type, ClassesHavingCfCAttributeTargetingTypeList> _attributeForClassListCollection;

      public ClassesHavingClassForClassAttributeList(System.Type attributeType)
        : base(attributeType)
      {
      }

      /// <summary>
      /// Gets a list of classes that have a attribute of type _attributeType, that targets the type given in the argument myTargetType.
      /// </summary>
      /// <param name="myTargetType">Type the attribute instances of the classes to return targets.</param>
      /// <returns>List of classes that have the attribute of type _attributeType, that targets myTargetType. If the attribute
      /// is sortable, the list returned is sorted by the attribute instances.</returns>
      public IAttributeForClassList GetClassesTargeting(Type myTargetType)
      {
        ClassesHavingCfCAttributeTargetingTypeList list;
        if (_attributeForClassListCollection == null)
          _attributeForClassListCollection = new Dictionary<Type, ClassesHavingCfCAttributeTargetingTypeList>();

        if (_attributeForClassListCollection.ContainsKey(myTargetType))
        {
          list = _attributeForClassListCollection[myTargetType];
        }
        else
        {
          list = new ClassesHavingCfCAttributeTargetingTypeList(_attributeType, myTargetType);
          _attributeForClassListCollection.Add(myTargetType, list);
        }

        if (this._attributeType != list._attributeType)
          throw new ApplicationException("Programming error (attributeType did not match), please inform the author that this exception happened");
        if (list._targetType != myTargetType)
          throw new ApplicationException("Programming error (targetType did not match), please inform the author that this exception happened");


        // Update the list
        IEnumerable<Type> definedtypes = GetClassesWithMyAttribute(list._currentAssemblyCount);
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
    }

    private class ClassesHavingAttributeListCollection
    {
      private class TypeArrayComparer : IEqualityComparer<Type[]>
      {
        #region IEqualityComparer<Type[]> Members

        public bool Equals(Type[] x, Type[] y)
        {
          if (x.Length != y.Length)
            return false;
          for (int i = 0; i < x.Length; i++)
            if (x[i] != y[i])
              return false;

          return true;
        }

        public int GetHashCode(Type[] obj)
        {
          int result = 0;
          foreach (Type t in obj)
            result += t.GetHashCode();

          return result;
        }

        #endregion
      }

      Dictionary<Type, ClassesHavingAttributeList> _list = new Dictionary<Type, ClassesHavingAttributeList>();

      public ClassesHavingAttributeList this[Type attributetype]
      {
        get
        {
          if (!_list.ContainsKey(attributetype))
            CreateList(attributetype);

          ClassesHavingAttributeList result = _list[attributetype];
          result.Update();
          return result;
        }
      }

      public IAttributeForClassList this[Type attributetype, Type myTargetType]
      {
        get
        {
          if (!_list.ContainsKey(attributetype))
          {
            if (!IsSubClassOfOrImplements(attributetype, typeof(IClassForClassAttribute)))
              throw new ArgumentException("This function must only be called with attribute types which implement the IClassForClassAttribute interface.");
            CreateList(attributetype);
          }

          ClassesHavingClassForClassAttributeList result = (ClassesHavingClassForClassAttributeList)_list[attributetype];
          return result.GetClassesTargeting(myTargetType);
        }
      }

      private void CreateList(Type attributetype)
      {
        if (IsSubClassOfOrImplements(attributetype, typeof(IClassForClassAttribute)))
          _list.Add(attributetype, new ClassesHavingClassForClassAttributeList(attributetype));
        else
          _list.Add(attributetype, new ClassesHavingAttributeList(attributetype));
      }


    }

    #endregion
  }
}

