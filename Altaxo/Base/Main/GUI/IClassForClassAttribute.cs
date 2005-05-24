using System;

namespace Altaxo.Main.GUI
{
  /// <summary>
  /// Implemented by class attributes, where one class (that has the attribute assigned to) is responsible for another type of class (the destination class type).
  /// </summary>
  public interface IClassForClassAttribute
  {

    /// <summary>
    /// The destination class type.
    /// </summary>
    System.Type TargetType { get; }

    /// <summary>
    /// The priority. Attributes with higher priority are preferred over such attributes with lower priority.
    /// </summary>
    int Priority { get; }
  }
}
