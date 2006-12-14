using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Plot.Groups
{
  public interface IPlotGroupStyleCollection : IEnumerable<IPlotGroupStyle>, ICloneable
  {

    /// <summary>
    /// Adds a group style as child of an already present group style.
    /// </summary>
    /// <param name="groupStyle">The group style to add.</param>
    /// <param name="parentGroupStyleType">The type of the group style that is the parent of the group style to add. A group style of this type must be already present in the collection.</param>
    void Add(IPlotGroupStyle groupStyle, Type parentGroupStyleType);
    
    /// <summary>
    /// Adds a group style to this collection.
    /// </summary>
    /// <param name="groupStyle">The group style to add.</param>
    void Add(IPlotGroupStyle groupStyle);

    /// <summary>
    /// Removes all group styles from the collection.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes a group style of the given type from the collection.
    /// </summary>
    /// <param name="groupType"></param>
    void RemoveType(Type groupType);

    /// <summary>
    /// Test whether or not a group style of the given type is present in this collection.
    /// </summary>
    /// <param name="groupStyleType">Type of the group style.</param>
    /// <returns>True if a group style of the given type is contained in the collection.</returns>
    bool ContainsType(Type groupStyleType);

    /// <summary>
    /// Number of group styles in the collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Tests if the group style of the given type has a child group style and returns the type of the child group style (or null if no such child exists).
    /// </summary>
    /// <param name="groupStyleType">Type of the group style to test.</param>
    /// <returns>Type of the child group style.</returns>
    Type GetChildTypeOf(Type groupStyleType);

   
    /// <summary>
    /// Returns the group style instance of the type given.
    /// </summary>
    /// <param name="groupStyleType">The type of the group style.</param>
    /// <returns>The instance of the given type, if it is contained in the collection. Throws an exeption, if no such instance (of the given type) is contained in the collection.</returns>
    IPlotGroupStyle GetPlotGroupStyle(Type groupStyleType);

    /// <summary>
    /// Returns the plot group strictness of this plot group, i.e. how the plot group is updated.
    /// </summary>
    PlotGroupStrictness PlotGroupStrictness { get; }
  
    void BeginPrepare();

    
    void EndPrepare();
    
    
    void BeginApply();
    void EndApply();
    void OnBeforeApplication(Type groupStyleType);

    void PrepareStep();
    void Step(int step);
  }
}
