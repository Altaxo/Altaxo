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
using System.ComponentModel;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Common;
#nullable disable
namespace Altaxo.Gui.Graph.Gdi.Axis
{
  public class AxisStyleControllerConditionalGlue : INotifyPropertyChanged
  {
    private AxisStyleCollection _doc;

    public CSAxisInformation AxisInformation { get; private set; }

   

    private Altaxo.Main.Properties.IReadOnlyPropertyBag _context;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    public AxisStyleControllerConditionalGlue(CSAxisInformation axisInfo, AxisStyleCollection axisStyleCollection)
    {
      _doc = axisStyleCollection;
      AxisInformation = axisInfo;
      _context = _doc.GetPropertyContext();
    }

    #region Bindings

    private ConditionalDocumentController<AxisStyle> _axisStyleCondController;

    public ConditionalDocumentController<AxisStyle> AxisStyleCondController
    {
      get
      {
        if(_axisStyleCondController is null)
        {
          _axisStyleCondController = new ConditionalDocumentController<AxisStyle>(CreateAxisStyle, RemoveAxisStyle, CreateAxisStyleController) { UseDocumentCopy = UseDocument.Directly };
          if (_doc.Contains(AxisInformation.Identifier))
          {
            var axStyle = _doc[AxisInformation.Identifier];
            _axisStyleCondController.InitializeDocument(axStyle);
          }
        }
        return _axisStyleCondController;
      }
    }


    private ConditionalDocumentController<AxisLabelStyle> _majorLabelCondController;

    public ConditionalDocumentController<AxisLabelStyle> MajorLabelCondController
    {
      get
      {
        if(_majorLabelCondController is null)
        {
          _majorLabelCondController = new ConditionalDocumentController<AxisLabelStyle>(CreateMajorLabel, RemoveMajorLabel) { UseDocumentCopy = UseDocument.Directly };
          if (_doc.Contains(AxisInformation.Identifier))
          {
            var axStyle = _doc[AxisInformation.Identifier];
            if (axStyle.AreMajorLabelsEnabled)
              _majorLabelCondController.InitializeDocument(axStyle.MajorLabelStyle);
          }
        }
        return _majorLabelCondController;
      }
    }

    private ConditionalDocumentController<AxisLabelStyle> _minorLabelCondController;

    public ConditionalDocumentController<AxisLabelStyle> MinorLabelCondController
    {
      get
      {
        if(_minorLabelCondController is null)
        {
          _minorLabelCondController = new ConditionalDocumentController<AxisLabelStyle>(CreateMinorLabel, RemoveMinorLabel) { UseDocumentCopy = UseDocument.Directly };
          if (_doc.Contains(AxisInformation.Identifier))
          {
            var axStyle = _doc[AxisInformation.Identifier];
            if (axStyle.AreMinorLabelsEnabled)
              _minorLabelCondController.InitializeDocument(axStyle.MinorLabelStyle);
          }
        }
        return _minorLabelCondController;
      }
    }

    public IConditionalDocumentView AxisStyleCondView
    {
      get
      {
        if (AxisStyleCondController is null)
          throw new InvalidOperationException("Instance is not initialized!");

        if (AxisStyleCondController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(AxisStyleCondController);
        return (IConditionalDocumentView)AxisStyleCondController.ViewObject;
      }
    }

    public IConditionalDocumentView MajorLabelCondView
    {
      get
      {
        if (MajorLabelCondController is null)
          throw new InvalidOperationException("Instance is not initialized!");

        if (MajorLabelCondController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(MajorLabelCondController);
        return (IConditionalDocumentView)MajorLabelCondController.ViewObject;
      }
    }

    public IConditionalDocumentView MinorLabelCondView
    {
      get
      {
        if (MinorLabelCondController is null)
          throw new InvalidOperationException("Instance is not initialized!");

        if (MinorLabelCondController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(MinorLabelCondController);
        return (IConditionalDocumentView)MinorLabelCondController.ViewObject;
      }
    }

    #endregion

    private void OnAxisStyleCreation(AxisStyle result)
    {
      MajorLabelCondController.IsConditionalViewEnabled = result.AreMajorLabelsEnabled;
      MinorLabelCondController.IsConditionalViewEnabled = result.AreMinorLabelsEnabled;
    }

    private void OnAxisStyleRemoval()
    {
      MajorLabelCondController.IsConditionalViewEnabled = false;
      MinorLabelCondController.IsConditionalViewEnabled = false;
    }

    private AxisStyle CreateAxisStyle()
    {
      bool wasPresentBefore = _doc.Contains(AxisInformation.Identifier);
      var result = _doc.AxisStyleEnsured(AxisInformation.Identifier);
      if (!wasPresentBefore)
      {
        OnAxisStyleCreation(result);
      }
      return result;
    }

    private void RemoveAxisStyle()
    {
      bool wasPresentBefore = _doc.Remove(AxisInformation.Identifier);
      if (wasPresentBefore)
      {
        ((AxisStyleController)AxisStyleCondController.UnderlyingController).MadeDirty -= EhAxisStyleControllerDirty;
        OnAxisStyleRemoval();
      }
    }

    private IMVCANController CreateAxisStyleController(AxisStyle doc, UseDocument useDocumentCopy)
    {
      var result = (AxisStyleController)Current.Gui.GetControllerAndControl(new object[] { doc }, typeof(AxisStyleController), useDocumentCopy);
      result.MadeDirty += EhAxisStyleControllerDirty;
      return result;
    }

    private void EhAxisStyleControllerDirty(IMVCANController ctrl)
    {
      MajorLabelCondController.IsConditionalViewEnabled = _doc[AxisInformation.Identifier].AreMajorLabelsEnabled;
      MinorLabelCondController.IsConditionalViewEnabled = _doc[AxisInformation.Identifier].AreMinorLabelsEnabled;
    }

    private AxisLabelStyle CreateMajorLabel()
    {
      bool wasPresentBefore = _doc.Contains(AxisInformation.Identifier);
      AxisStyleCondController.IsConditionalViewEnabled = true;
      var axStyle = _doc[AxisInformation.Identifier];
      axStyle.ShowMajorLabels(_context);
      ((AxisStyleController)AxisStyleCondController.UnderlyingController).AnnounceExternalChangeOfMajorOrMinorLabelState();
      return axStyle.MajorLabelStyle;
    }

    private void RemoveMajorLabel()
    {
      if (_doc.TryGetValue(AxisInformation.Identifier, out var axisStyle))
      {
        axisStyle.HideMajorLabels();
        ((AxisStyleController)AxisStyleCondController.UnderlyingController).AnnounceExternalChangeOfMajorOrMinorLabelState();
      }
    }

    private AxisLabelStyle CreateMinorLabel()
    {
      bool wasPresentBefore = _doc.Contains(AxisInformation.Identifier);
      AxisStyleCondController.IsConditionalViewEnabled = true;
      var axStyle = _doc[AxisInformation.Identifier];
      axStyle.ShowMinorLabels(_context);
      ((AxisStyleController)AxisStyleCondController.UnderlyingController).AnnounceExternalChangeOfMajorOrMinorLabelState();
      return axStyle.MinorLabelStyle;
    }

    private void RemoveMinorLabel()
    {
      if (_doc.TryGetValue(AxisInformation.Identifier, out var axisStyle))
      {
        axisStyle.HideMinorLabels();
        ((AxisStyleController)AxisStyleCondController.UnderlyingController!).AnnounceExternalChangeOfMajorOrMinorLabelState();
      }
    }

  }
}
