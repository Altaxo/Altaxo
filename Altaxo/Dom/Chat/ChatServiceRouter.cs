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

using Altaxo.Main.Properties;
using Altaxo.Main.Services;

namespace Altaxo.Chat
{
  /// <summary>
  /// Routes chat sessions to the configured chat provider.
  /// </summary>
  public static class ChatServiceRouter
  {
    /// <summary>
    /// Gets the property key for the current chat endpoint.
    /// </summary>
    public static readonly PropertyKey<IChatService> PropertyKeyChatService = new PropertyKey<IChatService>(
      "4283AA49-A426-47F5-9990-67F2B0141751",
      "Chat\\ChatService",
      PropertyLevel.Application);

    /// <summary>
    /// Gets the currently configured chat service.
    /// </summary>
    /// <returns>The configured chat service, or <see langword="null"/> if none is configured.</returns>
    public static IChatService? GetConfiguredChatService()
    {
      return Current.PropertyService.GetValueOrNull<IChatService>(PropertyKeyChatService, RuntimePropertyKind.UserAndApplicationAndBuiltin);
    }

    /// <summary>
    /// Creates a session from the specified service configuration.
    /// </summary>
    /// <param name="chatService">The configured chat service.</param>
    /// <returns>A new chat session.</returns>
    public static IChatSession CreateSession(IChatService chatService)
    {
      return chatService.CreateSession();
    }



  }
}
