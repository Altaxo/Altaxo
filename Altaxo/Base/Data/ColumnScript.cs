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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Altaxo.Serialization;
using Altaxo.Scripting;

namespace Altaxo.Data
{
  /// <summary>
  /// Holds  column scripts in a hash table. The hash value is the data column the script belongs to.
  /// </summary>
  [Serializable]
  public class ColumnScriptCollection : System.Collections.Hashtable
  {
    /// <summary>
    /// Constructs a empty ColumnScriptCollection
    /// </summary>
    public ColumnScriptCollection()
      : base()
    {
    }
    
    /// <summary>
    /// Special deserialization constructor.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Streaming context.</param>
    public ColumnScriptCollection(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
      : base(info,context)
    {
    }

    /// <summary>
    /// Serialization function.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Streaming context.</param>
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
    {
      base.GetObjectData(info,context);
    }

    /// <summary>
    /// get / set the column scripts associated with the correspondig columns.
    /// </summary>
    public IColumnScriptText this[DataColumn dc]
    {
      get { return base[dc] as IColumnScriptText; }
      set { base[dc]=value; }
    }
  }


 
}

