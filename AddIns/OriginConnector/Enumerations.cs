using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Addins.OriginConnector
{
  public enum OriginObjectType
  {
    None = 0, // Name is not defined.  
    Dataset = 1,
    Worksheet = 2,
    GraphWindow = 3,
    NumericVariable = 4,
    Matrix = 5,
    Tool = 7,
    Macro = 8,
    NotesWindow = 9,
  }

  public enum OriginColumnType
  {
    Y = 1,
    Disregard = 2,
    YError = 3,
    X = 4,
    Label = 5,
    Z = 6,
    XError = 7
  }

}
