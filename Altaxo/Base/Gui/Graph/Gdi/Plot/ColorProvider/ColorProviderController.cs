#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Drawing;
using Altaxo.Collections;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.ColorProvider;
using Altaxo.Graph.Scales;

namespace Altaxo.Gui.Graph.Gdi.Plot.ColorProvider
{
  #region Interfaces

  /// <summary>
  /// Interface that must be implemented by Gui classes that allow to select a color provider.
  /// </summary>
  public interface IColorProviderView
  {
    /// <summary>
    /// Set the list of available color providers.
    /// </summary>
    /// <param name="names">List of items.</param>
    void InitializeAvailableClasses(SelectableListNodeList names);

    /// <summary>
    /// Sets the detailed view for the instance of the color provider.
    /// </summary>
    /// <param name="guiobject"></param>
    void SetDetailView(object guiobject);

    /// <summary>
    /// Set the preview bitmap to be shown in the view.
    /// </summary>
    /// <param name="bitmap">Bitmap to show.</param>
    void SetPreviewBitmap(System.Drawing.Bitmap bitmap);

    /// <summary>
    /// Gets a bitmap with a certain size.
    /// </summary>
    /// <param name="width">Pixel width of the bitmap.</param>
    /// <param name="height">Pixel height of the bitmap.</param>
    /// <returns>A bitmap that can be used for drawing.</returns>
    System.Drawing.Bitmap GetPreviewBitmap(int width, int height);

    /// <summary>
    /// Fired when the selected color provider changed.
    /// </summary>
    event Action ColorProviderChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for ColorProviderController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IColorProviderView))]
  //[UserControllerForObject(typeof(IColorProvider), 101)]
  public class ColorProviderController : MVCANDControllerEditImmutableDocBase<IColorProvider, IColorProviderView>
  {
    protected IMVCAController _detailController;
    protected object _detailView;

    protected SelectableListNodeList _availableClasses;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_detailController, () => _detailController = null);
    }

    public ColorProviderController(Action<IColorProvider> SetInstanceInParentDoc)
      : base(SetInstanceInParentDoc)
    {
    }

    public override void Dispose(bool isDisposing)
    {
      _detailView = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      InitClassTypes(initData);
      InitDetailController(initData);
      CreateAndSetPreviewBitmap();
    }

    public override bool Apply(bool disposeController)
    {
      if (null != _detailController)
      {
        if (!_detailController.Apply(disposeController))
          return false;
      }

      _doc = (IColorProvider)_detailController.ModelObject;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.ColorProviderChanged += EhColorProviderSelectionChanged;
    }

    protected override void DetachView()
    {
      _view.ColorProviderChanged -= EhColorProviderSelectionChanged;

      base.DetachView();
    }

    public void InitClassTypes(bool bInit)
    {
      if (bInit)
      {
        _availableClasses = new SelectableListNodeList();
        Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IColorProvider));
        for (int i = 0; i < classes.Length; i++)
        {
          if (classes[i] == typeof(LinkedScale))
            continue;
          var node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _doc.GetType() == classes[i]);
          _availableClasses.Add(node);
        }
      }

      if (null != _view)
        _view.InitializeAvailableClasses(_availableClasses);
    }

    public void InitDetailController(bool bInit)
    {
      if (bInit)
      {
        object providerObject = _doc;

        if (_detailController is IMVCANDController)
          ((IMVCANDController)_detailController).MadeDirty -= EhDetailsChanged;

        _detailController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { providerObject }, typeof(IMVCANController), UseDocument.Directly);

        if (null != _detailController && GetType() == _detailController.GetType()) // the returned controller is of this common type here -> thus no specialized controller seems to exist for this type of color provider
        {
          _detailController.Dispose();
          _detailController = null;
        }

        if (_detailController is IMVCANDController)
          ((IMVCANDController)_detailController).MadeDirty += EhDetailsChanged;
      }
      if (null != _view)
      {
        _detailView = null == _detailController ? null : _detailController.ViewObject;
        _view.SetDetailView(_detailView);
      }
    }

    private void EhColorProviderSelectionChanged()
    {
      var chosenType = (Type)_availableClasses.FirstSelectedNode.Tag;

      try
      {
        if (chosenType != _doc.GetType())
        {
          // replace the current axis by a new axis of the type axistype
          var oldDoc = _doc;
          var newDoc = (IColorProvider)System.Activator.CreateInstance(chosenType);

          if (newDoc is ColorProviderBase && oldDoc is ColorProviderBase)
          {
            var oldBase = (ColorProviderBase)oldDoc;

            newDoc = ((ColorProviderBase)newDoc)
              .WithColorBelow(oldBase.ColorBelow)
              .WithColorAbove(oldBase.ColorAbove)
              .WithColorInvalid(oldBase.ColorInvalid)
              .WithTransparency(oldBase.Transparency)
              .WithColorSteps(oldBase.ColorSteps);
          }

          _doc = newDoc;
          OnMadeDirty(); // Change for the controller up in hierarchy to grab new document

          InitDetailController(true);
          CreateAndSetPreviewBitmap();
        }
      }
      catch (Exception)
      {
      }
    }

    private void EhDetailsChanged(IMVCANDController ctrl)
    {
      _detailController.Apply(false); // we use the instance directly, thus no further taking of the instance is neccessary here
      _doc = (IColorProvider)(_detailController.ModelObject);
      CreateAndSetPreviewBitmap();
    }

    private Bitmap _previewBitmap;

    private void CreateAndSetPreviewBitmap()
    {
      const int previewWidth = 128;
      const int previewHeight = 16;
      if (null != _view)
      {
        if (null == _previewBitmap)
          _previewBitmap = _view.GetPreviewBitmap(previewWidth, previewHeight); // new Bitmap(previewWidth, previewHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        for (int i = 0; i < previewWidth; i++)
        {
          double relVal = i / (double)(previewWidth - 1);
          Color c = _doc.GetColor(relVal);
          for (int j = 0; j < previewHeight; j++)
            _previewBitmap.SetPixel(i, j, c);
        }

        _view.SetPreviewBitmap(_previewBitmap);
      }
    }
  }
}
