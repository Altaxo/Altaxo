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
    /// For a given type of attribute, attributeType, this function returns the attribute instances and the class
    /// types this attributes apply to. If the attribute implements the IComparable interface, the list is sorted.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static DictionaryEntry[] GetAttributeInstancesAndClassTypes(System.Type attributeType)
    {
      ArrayList list = new ArrayList();
      Assembly attributeAssembly = attributeType.Assembly;

      System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach(Assembly assembly in assemblies)
      {
        if(Array.IndexOf(assembly.GetReferencedAssemblies(),attributeAssembly)<0)
          continue; // this is not depended on this assembly the attribute is defined in, so we can ignore this
        
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

      if(list.Count>1 && attributeType.IsSubclassOf(typeof(IComparable)))
        list.Sort(new DictEntryKeyComparer());

      return (DictionaryEntry[])list.ToArray(typeof(DictionaryEntry));
    }

    /// <summary>
    /// For a given type of attribute, attributeType, this function returns the attribute instances and the class
    /// types this attributes apply to. If the attribute implements the IComparable interface, the list is sorted.
    /// </summary>
    /// <param name="attributeType">The type of attribute (this has to be a class attribute type).</param>
    /// <returns>A list of dictionary entries. The keys are the attribute instances, the values are the class types this attributes apply to.</returns>
    public static DictionaryEntry[] GetAttributeInstancesAndClassTypesForClass(System.Type attributeType, object target)
    {
      ArrayList list = new ArrayList();
      Assembly attributeAssembly = attributeType.Assembly;

      System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach(Assembly assembly in assemblies)
      {
        if(Array.IndexOf(assembly.GetReferencedAssemblies(),attributeAssembly)<0)
          continue; // this is not depended on this assembly the attribute is defined in, so we can ignore this
        
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

      if(list.Count>1 && attributeType.IsSubclassOf(typeof(IComparable)))
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
        if(((System.Type)list[i].Value).IsSubclassOf(expectedType))
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
    /// then for all found classes the instantiation of a class is tried, until a instance is created successfully.
    /// </summary>
    /// <param name="attributeType">The type of attribute  the class(es) to instantiate must be assigned to.</param>
    /// <param name="expectedType">The expected type of return value.</param>
    /// <param name="creationArgs">The creation arguments used to instantiate a class.</param>
    /// <returns>The instance of the first class for which the instantiation was successfull and results in the expectedType. Otherwise null.</returns>
    public static object GetClassForClassInstanceByAttribute(System.Type attributeType, System.Type expectedType, object[] creationArgs)
    {
      object result=null;
      // 1st search for all classes that wear the UserControllerForObject attribute
      DictionaryEntry[] list = ReflectionService.GetAttributeInstancesAndClassTypesForClass(attributeType,creationArgs[0]);

      for(int i=list.Length-1;i>=0;i--)
      {
        if(((System.Type)list[i].Value).IsSubclassOf(expectedType))
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
  }
}

