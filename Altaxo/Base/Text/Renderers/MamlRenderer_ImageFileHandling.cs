#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Altaxo.Collections;
using Markdig.Renderers;

namespace Altaxo.Text.Renderers
{
  public partial class MamlRenderer : TextRendererBase<MamlRenderer>
  {
    public void StorePngImageFile(Stream imageStream, string contentHash)
    {
      var fullFileName = Path.Combine(BasePathName, ImageFolderName, contentHash + ".png");

      using (var outStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
      {
        imageStream.CopyTo(outStream);
        outStream.Close();
      }

      _imageFileNames.Add(fullFileName);
    }

    /// <summary>
    /// Removes the old image files. This function will work only if there is a dedicated image folder, i.e. <see cref="ImageFileNames"/> has a value.
    /// </summary>
    public static void RemoveOldContentsOfImageFolder(string fullImageFolderName)
    {
      var dir = new DirectoryInfo(fullImageFolderName);
      if (!dir.Exists)
        return;

      var filesToDelete = new HashSet<string>();
      foreach (var extension in new string[] { ".png", ".tif", ".jpg", ".jpeg", ".bmp" })
      {
        filesToDelete.AddRange(dir.GetFiles("????????????????????????????????" + extension).Select(x => x.FullName));
      }

      // now delete the files
      foreach (var file in filesToDelete)
        File.Delete(file);
    }
  }
}
