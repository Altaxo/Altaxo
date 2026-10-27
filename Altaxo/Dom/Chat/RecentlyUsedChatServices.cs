#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License
//    as published by the Free Software Foundation; either version 2 of the License, or
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

using System;
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Main.Properties;
using Altaxo.Main.Services;

namespace Altaxo.Chat
{
  /// <summary>
  /// Represents a collection of recently used chat services.
  /// </summary>
  public record RecentlyUsedChatServices
  {
    /// <summary>
    /// Gets the immutable list of recently used chat service instances.
    /// </summary>
    public ImmutableList<IChatService> InnerList
    {
      get => field;
      init { field = value ?? throw new System.ArgumentNullException(nameof(value)); }
    } = ImmutableList<IChatService>.Empty;

    #region Serialization

    /// <summary>
    /// V0: 2026-10-26 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RecentlyUsedChatServices), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RecentlyUsedChatServices)o;
        info.AddArray("List", s.InnerList, s.InnerList.Count);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var innerList = info.GetArrayOfValues<IChatService>("List", null).ToImmutableList();
        return o is RecentlyUsedChatServices rus ?
            rus with
            {
              InnerList = innerList
            } :
            new RecentlyUsedChatServices
            {
              InnerList = innerList
            };
      }
    }

    #endregion Serialization


    /// <summary>
    /// Gets the property key for the recently used chat services.
    /// </summary>
    public static readonly PropertyKey<RecentlyUsedChatServices> PropertyKeyRecentlyUsedChatServices = new(
      "971A1294-9AF2-400A-B747-9B490BE68CCD",
      "Chat\\RecentlyUsedChatServices",
      PropertyLevel.Application);

    /// <summary>
    /// Adds a chat service to the list of recently used chat services.
    /// </summary>
    /// <param name="chatService">The chat service to add.</param>
    public static void AddToRecentlyUsedChatServices(IChatService chatService)
    {
      const int MaxRecentlyUsedChatServices = 10;

      if (chatService is null)
        return;

      var recentlyUsed = Current.PropertyService.GetValueOrNull<RecentlyUsedChatServices>(PropertyKeyRecentlyUsedChatServices, RuntimePropertyKind.UserAndApplicationAndBuiltin);

      if (recentlyUsed is null)
      {
        recentlyUsed = new RecentlyUsedChatServices
        {
          InnerList = new IChatService[] { chatService }.ToImmutableList()
        };
      }
      else
      {
        var arr = recentlyUsed.InnerList.ToArray();
        var idx = arr.IndexOf(chatService);
        if (idx > 0)
        {
          // this means the chat service is already in the list, but not at the top. Move it to the top.
          (arr[idx], arr[0]) = (arr[0], arr[idx]);
        }
        else if (idx < 0)
        {
          // this means the chat service is not in the list. Add it to the top.
          arr = new IChatService[] { chatService }.Concat(recentlyUsed.InnerList).Take(MaxRecentlyUsedChatServices).ToArray();
        }
        recentlyUsed = recentlyUsed with
        {
          InnerList = arr.ToImmutableList()
        };
      }
      Current.PropertyService.SetValue(PropertyKeyRecentlyUsedChatServices, recentlyUsed);
    }
  }
}
