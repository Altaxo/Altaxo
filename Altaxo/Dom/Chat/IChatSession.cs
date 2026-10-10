#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Chat
{
  /// <summary>
  /// Represents a conversation with a configured language-model endpoint.
  /// </summary>
  public interface IChatSession : IAsyncDisposable
  {
    /// <summary>
    /// Sends a user prompt and returns the assistant response after any requested Mcp tool calls complete.
    /// </summary>
    /// <param name="prompt">The user's prompt.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <param name="imageData">Optional image data to include with the prompt.</param>
    /// <param name="imageMediaType">The media type of <paramref name="imageData"/>.</param>
    /// <param name="onTextReceived">Optional callback invoked as assistant text arrives.</param>
    /// <returns>The assistant's response.</returns>
    public async Task<string> SendAsync(string prompt, CancellationToken cancellationToken, byte[]? imageData = null, string? imageMediaType = null, Func<string, CancellationToken, ValueTask>? onTextReceived = null)
    {
      var response = new StringBuilder();
      await foreach (var text in StreamAsync(prompt, cancellationToken, imageData, imageMediaType))
      {
        response.Append(text);
        if (onTextReceived is not null)
          await onTextReceived(text, cancellationToken);
      }
      return response.ToString();
    }

    /// <summary>
    /// Sends a prompt and yields assistant text as it is generated.
    /// </summary>
    /// <param name="prompt">The user's prompt.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <param name="imageData">Optional image data to include with the prompt.</param>
    /// <param name="imageMediaType">The media type of <paramref name="imageData"/>.</param>
    /// <returns>Incremental assistant text chunks.</returns>
    public IAsyncEnumerable<string> StreamAsync(string prompt, CancellationToken cancellationToken, byte[]? imageData = null, string? imageMediaType = null);
  }
}
