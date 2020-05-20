// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Scripting, Core/Hosting/AssemblyLoader/DesktopAssemblyLoaderImpl.cs

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Roslyn.Utilities;

#if false
namespace Altaxo.CodeEditing.CompilationHandling
{
  internal sealed class DesktopAssemblyLoader : AssemblyLoaderImpl
  {
    private readonly Func<string, Assembly, Assembly> _assemblyResolveHandlerOpt;

    public DesktopAssemblyLoader(InteractiveAssemblyLoader loader)
        : base(null)
    {
      _assemblyResolveHandlerOpt = loader.ResolveAssembly;
      CorLightup.Desktop.AddAssemblyResolveHandler(_assemblyResolveHandlerOpt);
    }

    public override void Dispose()
    {
      if (_assemblyResolveHandlerOpt != null)
      {
        CorLightup.Desktop.RemoveAssemblyResolveHandler(_assemblyResolveHandlerOpt);
      }
    }

    public override Assembly LoadFromStream(Stream peStream, Stream pdbStream)
    {
      byte[] peImage = new byte[peStream.Length];
      TryReadAll(peStream, peImage, 0, peImage.Length);
      if (pdbStream == null)
      {
        return CorLightup.Desktop.LoadAssembly(peImage);
      }

      // TODO: lightup?
      byte[] pdbImage = new byte[pdbStream.Length];
      TryReadAll(pdbStream, pdbImage, 0, pdbImage.Length);
      return Assembly.Load(peImage, pdbImage);
    }

    public override AssemblyAndLocation LoadFromPath(string path)
    {
      // An assembly is loaded into CLR's Load Context if it is in the GAC, otherwise it's loaded into No Context via Assembly.LoadFile(string).
      // Assembly.LoadFile(string) automatically redirects to GAC if the assembly has a strong name and there is an equivalent assembly in GAC.

      var assembly = CorLightup.Desktop.LoadAssembly(path);
      var location = CorLightup.Desktop.GetAssemblyLocation(assembly);
      var fromGac = CorLightup.Desktop.IsAssemblyFromGlobalAssemblyCache(assembly);
      return new AssemblyAndLocation(assembly, location, fromGac);
    }

    /// <summary>
    /// Attempts to read all of the requested bytes from the stream into the buffer
    /// </summary>
    /// <returns>
    /// The number of bytes read. Less than <paramref name="count" /> will
    /// only be returned if the end of stream is reached before all bytes can be read.
    /// </returns>
    /// <remarks>
    /// Unlike <see cref="Stream.Read(byte[], int, int)"/> it is not guaranteed that
    /// the stream position or the output buffer will be unchanged if an exception is
    /// returned.
    /// </remarks>
    public static int TryReadAll(
        Stream stream,
        byte[] buffer,
        int offset,
        int count)
    {
      // The implementations for many streams, e.g. FileStream, allows 0 bytes to be
      // read and returns 0, but the documentation for Stream.Read states that 0 is
      // only returned when the end of the stream has been reached. Rather than deal
      // with this contradiction, let's just never pass a count of 0 bytes
      Debug.Assert(count > 0);

      int totalBytesRead;
      int bytesRead = 0;
      for (totalBytesRead = 0; totalBytesRead < count; totalBytesRead += bytesRead)
      {
        // Note: Don't attempt to save state in-between calls to .Read as it would
        // require a possibly massive intermediate buffer array
        bytesRead = stream.Read(buffer,
                                offset + totalBytesRead,
                                count - totalBytesRead);
        if (bytesRead == 0)
        {
          break;
        }
      }
      return totalBytesRead;
    }
  }
}
#endif
