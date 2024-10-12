#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

#endregion Copyright

namespace Altaxo.Serialization.WITec
{
  public class TDataClass
  {
    public WITecTreeNode Node { get; }
    public WITecTreeNode TData { get; }
    public int ID { get; }
    public string Caption { get; }
    public string ClassName { get; }

    public TDataClass(WITecTreeNode node)
    {
      Node = node;
      ClassName = node.Parent.GetData<string>(GetClassNameOfName(node.Name));
      TData = node.GetChild("TData");
      ID = TData.GetData<int>("ID");
      Caption = TData.GetData<string>("Caption");
    }

    public static string GetClassNameOfName(string name)
    {
      return name.Replace("Data", "DataClassName");
    }
  }
}
