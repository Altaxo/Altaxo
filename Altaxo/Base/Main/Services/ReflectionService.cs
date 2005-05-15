using System;
using System.Collections;
using System.Reflection;
using Altaxo.Main.GUI;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Static functions for searching by attributes.
	/// </summary>
  public class ReflectionService
  {

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
        return Array.IndexOf(subtype.GetInterfaces(),basetype)>=0;
      }
      else
      {
        return subtype.IsSubclassOf(basetype);
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
    /// <returns>All assemblies, that are currently loaded and that references the given base assembly. The base assembly is also in the returned list.</returns>
    public static System.Reflection.Assembly[] GetDependendAssemblies(Assembly baseAssembly)
    {
      ArrayList list = new ArrayList();

      AssemblyName baseAssemblyName = baseAssembly.GetName();

      System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach(Assembly assembly in assemblies)
      {
       if(Contains(assembly.GetReferencedAssemblies(),baseAssemblyName))
          list.Add(assembly); 
      }
      list.Add(baseAssembly);

      return (Assembly[])list.ToArray(typeof(System.Reflection.Assembly));
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

      Assembly[] assemblies = GetDependendAssemblies(basetype.Assembly);
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
    /// For a given type of attribute, attributeType, this function returns the attribute instances and the class
    /// types this attributes apply to. If the attribute implements the IComparable interface, the list is sorted.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static DictionaryEntry[] GetAttributeInstancesAndClassTypes(System.Type attributeType)
    {
      ArrayList list = new ArrayList();

      System.Reflection.Assembly[] assemblies = GetDependendAssemblies(attributeType.Assembly);
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
    /// to implement the <see>IClassForClassAttribute</see> interface, and only those attributes are considered, for which the
    /// <see>IClassForClassAttribute.TargetType</see> match the type of the target argument.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static DictionaryEntry[] GetAttributeInstancesAndClassTypesForClass(System.Type attributeType, object target)
    {
      System.Diagnostics.Debug.Assert(IsSubClassOfOrImplements(attributeType,typeof(IClassForClassAttribute)));

      ArrayList list = new ArrayList();

      System.Reflection.Assembly[] assemblies = GetDependendAssemblies(attributeType.Assembly);
      foreach(Assembly assembly in assemblies)
      {
        Type[] definedtypes = assembly.GetTypes();
        foreach(Type definedtype in definedtypes)
        {
          Attribute[] attributes = Attribute.GetCustomAttributes(definedtype,attributeType);
            
          foreach(Attribute att in attributes)
          {
            if(att is IClassForClassAttribute)
              if(((IClassForClassAttribute)att).TargetType.IsInstanceOfType(target))
                list.Add(new DictionaryEntry(att,definedtype));
          }
        } // end foreach type
      } // end foreach assembly 

      if(list.Count>1 && IsSubClassOfOrImplements(attributeType,typeof(IComparable)))
        list.Sort(new DictEntryKeyComparer());

      return (DictionaryEntry[])list.ToArray(typeof(DictionaryEntry));
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
    /// to implement <see>IClassForClassAttribute</see>, and creationArg[0] has to match the type in <see>IClassForClassAttribute.TargetType</see>
    /// </summary>
    /// <param name="attributeType">The type of attribute  the class(es) to instantiate must be assigned to.</param>
    /// <param name="expectedType">The expected type of return value.</param>
    /// <param name="creationArgs">The creation arguments used to instantiate a class.</param>
    /// <returns>The instance of the first class for which the instantiation was successfull and results in the expectedType. Otherwise null.</returns>
    /// <remarks>The instantiation is tried first with the full argument list. If that fails, the last element of the argument list is chopped and the instantiation is tried again.
    /// This process is repeated until the instantiation was successfull or the argument list is empty (empty constructor is tried at last).</remarks>
    public static object GetClassForClassInstanceByAttribute(System.Type attributeType, System.Type expectedType, object[] creationArgs)
    {
      object result=null;
      // 1st search for all classes that wear the UserControllerForObject attribute
      DictionaryEntry[] list = ReflectionService.GetAttributeInstancesAndClassTypesForClass(attributeType,creationArgs[0]);

      for(int i=list.Length-1;i>=0;i--)
      {
        if(!IsSubClassOfOrImplements( (System.Type)list[i].Value,expectedType))
          continue;
        // try to create the class
        try
        {
          result = Activator.CreateInstance((System.Type)list[i].Value,creationArgs);
          break;
        }
        catch(Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(ex.ToString());
        }

        for(int l=creationArgs.Length-1;l>=0;l--)
        {
          object[] choppedArgs = new object[l];
          Array.Copy(creationArgs,choppedArgs,l);
          // try to create the class finally without args
          try
          {
            result = Activator.CreateInstance((System.Type)list[i].Value,choppedArgs);
            break;
          }
          catch(Exception ex)
          {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
          }
        }
        if(result!=null)
          break;
      }
      return result;
    }
  }
}

