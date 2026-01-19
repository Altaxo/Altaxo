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

#nullable disable
using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Graph.Graph3D.Plot;
using Altaxo.Gui.Graph.Gdi.Plot;

namespace Altaxo.Gui.Graph.Graph3D.Plot
{

  /// <summary>
  /// Controls the option tab page in the <see cref="DataMeshPlotItem"/> dialog. This tab page allows only
  /// to save the image to clipboard or disc, thus the document is not really controlled.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDensityImagePlotItemOptionView))]
  public class DataMeshPlotItemOptionController : MVCANControllerEditOriginalDocBase<DataMeshPlotItem, IDensityImagePlotItemOptionView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public DataMeshPlotItemOptionController()
    {
      CmdCopyImageToClipboard = new RelayCommand(EhCopyImageToClipboard);
      CmdSaveImageToDisc = new RelayCommand(EhSaveImageToDisc);
    }

    protected override void Initialize(bool initData)
    {
      // base.Initialize(initData); // no base initialize because we dont want to suspend the doc (this is only a helper controller)
    }

    #region Bindings

    public ICommand CmdCopyImageToClipboard { get; }
    public ICommand CmdSaveImageToDisc { get; }

    #endregion

    public override bool Apply(bool disposeController)
    {
      return true;
    }

    private void EhCopyImageToClipboard()
    {
      var bitmap = _doc.GetPixelwiseImage();
      bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
      var dao = Current.Gui.GetNewClipboardDataObject();
      dao.SetData(bitmap.GetType(), bitmap);
      Current.Gui.SetClipboardDataObject(dao);
    }

    private void EhSaveImageToDisc()
    {
      var saveOptions = new Gui.SaveFileOptions() { Title = "Choose a file name to save the image" };
      saveOptions.AddFilter("*.png", "Png files (*.png)");
      saveOptions.AddFilter("*.tif", "Tiff files (*.tif)");
      if (!Current.Gui.ShowSaveFileDialog(saveOptions))
        return;

      var bitmap = _doc.GetPixelwiseImage();
      bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);

      bitmap.Save(saveOptions.FileName);
    }
  }
}
