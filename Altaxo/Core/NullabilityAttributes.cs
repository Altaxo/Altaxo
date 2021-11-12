#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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


namespace System.Diagnostics.CodeAnalysis
{
#if NETFRAMEWORK
  /// <summary>
  /// Fake MayBeNullWhen Attribute that avoids compile errors when using the .NET framework.
  /// </summary>
  public class MaybeNullWhenAttribute : Attribute
  {
    public MaybeNullWhenAttribute(bool _)
    {
    }
  }


  /// <summary>
  /// Fake MayBeNull Attribute that avoids compile errors when using the .NET framework.
  /// </summary>
  public class MaybeNull : Attribute
  {
  }

  /// <summary>
  /// Fake AllowNull Attribute that avoids compile errors when using the .NET framework.
  /// </summary>
  public class AllowNull : Attribute
  {
  }

  /// <summary>
  /// Fake DisallowNull Attribute that avoids compile errors when using the .NET framework.
  /// </summary>
  public class DisallowNull : Attribute
  {
  }

  /// <summary>
  /// Fake DoesNotReturn Attribute that avoids compile errors when using the .NET framework.
  /// </summary>
  public class DoesNotReturn : Attribute
  {
  }

  /// <summary>
  /// Fake NotNull Attribute that avoids compile errors when using the .NET framework.
  /// </summary>
  public class NotNull : Attribute
  {
  }

  //
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
  public sealed class NotNullIfNotNullAttribute : Attribute
  {
    private string _parameterName;
    public string ParameterName
    {
      get
      {
        return _parameterName;
      }
    }

    public NotNullIfNotNullAttribute(string parameterName)
    {
      _parameterName = parameterName;
    }
  }
  

  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  public sealed class MemberNotNullAttribute : Attribute
  {
    public MemberNotNullAttribute(string _)
    {
    }

    public MemberNotNullAttribute(params string[] _)
    {
    }
  }


  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  public sealed class MemberNotNullWhenAttribute : Attribute
  {
    public MemberNotNullWhenAttribute(bool cond, string _)
    {
    }

    public MemberNotNullWhenAttribute(bool cond, params string[] _)
    {
    }
  }
#endif
}
