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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  internal class AxisStyleControllerConditionalGlue
  {
    private AxisStyleCollection _doc;

    public CSAxisInformation AxisInformation { get; private set; }

    public ConditionalDocumentController<AxisStyle> AxisStyleCondController { get; private set; }

    public ConditionalDocumentController<AxisLabelStyle> MajorLabelCondController { get; private set; }

    public ConditionalDocumentController<AxisLabelStyle> MinorLabelCondController { get; private set; }

    private Altaxo.Main.Properties.IReadOnlyPropertyBag _context;

    public AxisStyleControllerConditionalGlue(CSAxisInformation axisInfo, AxisStyleCollection axisStyleCollection)
    {
      _doc = axisStyleCollection;
      AxisInformation = axisInfo;
      InternalInitialize();
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
      set
      {
        if (AxisStyleCondController is null)
          throw new InvalidOperationException("Instance is not initialized!");
        AxisStyleCondController.ViewObject = value;
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
      set
      {
        if (MajorLabelCondController is null)
          throw new InvalidOperationException("Instance is not initialized!");
        MajorLabelCondController.ViewObject = value;
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
      set
      {
        if (MinorLabelCondController is null)
          throw new InvalidOperationException("Instance is not initialized!");
        MinorLabelCondController.ViewObject = value;
      }
    }

    private void OnAxisStyleCreation(AxisStyle result)
    {
      MajorLabelCondController.AnnounceEnabledChanged(result.AreMajorLabelsEnabled);
      MinorLabelCondController.AnnounceEnabledChanged(result.AreMinorLabelsEnabled);
    }

    private void OnAxisStyleRemoval()
    {
      MajorLabelCondController.AnnounceEnabledChanged(false);
      MinorLabelCondController.AnnounceEnabledChanged(false);
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
      bool wasPresentBefore = _doc.Contains(AxisInformation.Identifier);
      _doc.Remove(AxisInformation.Identifier);
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
      MajorLabelCondController.AnnounceEnabledChanged(_doc[AxisInformation.Identifier].AreMajorLabelsEnabled);
      MinorLabelCondController.AnnounceEnabledChanged(_doc[AxisInformation.Identifier].AreMinorLabelsEnabled);
    }

    private AxisLabelStyle CreateMajorLabel()
    {
      bool wasPresentBefore = _doc.Contains(AxisInformation.Identifier);
      AxisStyleCondController.AnnounceEnabledChanged(true);
      var axStyle = _doc[AxisInformation.Identifier];
      axStyle.ShowMajorLabels(_context);
      ((AxisStyleController)AxisStyleCondController.UnderlyingController).AnnounceExternalChangeOfMajorOrMinorLabelState();
      return axStyle.MajorLabelStyle;
    }

    private void RemoveMajorLabel()
    {
      if (_doc.Contains(AxisInformation.Identifier))
      {
        _doc[AxisInformation.Identifier].HideMajorLabels();
        ((AxisStyleController)AxisStyleCondController.UnderlyingController).AnnounceExternalChangeOfMajorOrMinorLabelState();
      }
    }

    private AxisLabelStyle CreateMinorLabel()
    {
      bool wasPresentBefore = _doc.Contains(AxisInformation.Identifier);
      AxisStyleCondController.AnnounceEnabledChanged(true);
      var axStyle = _doc[AxisInformation.Identifier];
      axStyle.ShowMinorLabels(_context);
      ((AxisStyleController)AxisStyleCondController.UnderlyingController).AnnounceExternalChangeOfMajorOrMinorLabelState();
      return axStyle.MinorLabelStyle;
    }

    private void RemoveMinorLabel()
    {
      if (_doc.Contains(AxisInformation.Identifier))
      {
        _doc[AxisInformation.Identifier].HideMinorLabels();
        ((AxisStyleController)AxisStyleCondController.UnderlyingController).AnnounceExternalChangeOfMajorOrMinorLabelState();
      }
    }

    private void InternalInitialize()
    {
      _context = _doc.GetPropertyContext();

      AxisStyleCondController = new ConditionalDocumentController<AxisStyle>(CreateAxisStyle, RemoveAxisStyle, CreateAxisStyleController) { UseDocumentCopy = UseDocument.Directly };
      MajorLabelCondController = new ConditionalDocumentController<AxisLabelStyle>(CreateMajorLabel, RemoveMajorLabel) { UseDocumentCopy = UseDocument.Directly };
      MinorLabelCondController = new ConditionalDocumentController<AxisLabelStyle>(CreateMinorLabel, RemoveMinorLabel) { UseDocumentCopy = UseDocument.Directly };

      if (_doc.Contains(AxisInformation.Identifier))
      {
        var axStyle = _doc[AxisInformation.Identifier];
        AxisStyleCondController.InitializeDocument(axStyle);
        if (axStyle.AreMajorLabelsEnabled)
          MajorLabelCondController.InitializeDocument(axStyle.MajorLabelStyle);
        if (axStyle.AreMinorLabelsEnabled)
          MinorLabelCondController.InitializeDocument(axStyle.MinorLabelStyle);
      }
    }
  }
}
