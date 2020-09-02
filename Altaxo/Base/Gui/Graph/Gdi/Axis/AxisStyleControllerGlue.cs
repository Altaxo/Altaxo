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
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  /// <summary>
  /// Glues a controller for the axis title/line style, and two conditional controllers for the major and minor label styles together. This class should be used if the existence of the axis style is ensured,
  /// i.e. the axis style can neither be removed or created.
  /// If, in contrast, one should be able to remove or create an axis style, you should rather use an instance of the <see cref="AxisStyleControllerConditionalGlue"/> class.
  /// </summary>
  public class AxisStyleControllerGlue
  {
    private AxisStyle _doc;

    public AxisStyleController AxisStyleController { get; private set; }

    public ConditionalDocumentController<AxisLabelStyle> MajorLabelCondController { get; private set; }

    public ConditionalDocumentController<AxisLabelStyle> MinorLabelCondController { get; private set; }

    private Altaxo.Main.Properties.IReadOnlyPropertyBag _context;

    public AxisStyleControllerGlue(AxisStyle axisStyle)
    {
      _doc = axisStyle;
      InternalInitialize();
    }

    public object AxisStyleView
    {
      get
      {
        if (AxisStyleController is null)
          throw new InvalidOperationException("Instance is not initialized!");

        if (AxisStyleController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(AxisStyleController);
        return AxisStyleController.ViewObject;
      }
      set
      {
        if (AxisStyleController is null)
          throw new InvalidOperationException("Instance is not initialized!");
        AxisStyleController.ViewObject = value;
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

    private void EhAxisStyleControllerDirty(IMVCANController ctrl)
    {
      MajorLabelCondController.AnnounceEnabledChanged(_doc.AreMajorLabelsEnabled);
      MinorLabelCondController.AnnounceEnabledChanged(_doc.AreMinorLabelsEnabled);
    }

    private AxisLabelStyle CreateMajorLabel()
    {
      _doc.ShowMajorLabels(_context);
      AxisStyleController.AnnounceExternalChangeOfMajorOrMinorLabelState();
      return _doc.MajorLabelStyle;
    }

    private void RemoveMajorLabel()
    {
      _doc.HideMajorLabels();
      AxisStyleController.AnnounceExternalChangeOfMajorOrMinorLabelState();
    }

    private AxisLabelStyle CreateMinorLabel()
    {
      _doc.ShowMinorLabels(_context);
      AxisStyleController.AnnounceExternalChangeOfMajorOrMinorLabelState();
      return _doc.MinorLabelStyle;
    }

    private void RemoveMinorLabel()
    {
      _doc.HideMinorLabels();
      AxisStyleController.AnnounceExternalChangeOfMajorOrMinorLabelState();
    }

    private void InternalInitialize()
    {
      _context = _doc.GetPropertyContext();

      AxisStyleController = new AxisStyleController() { UseDocumentCopy = UseDocument.Directly };
      AxisStyleController.MadeDirty += EhAxisStyleControllerDirty;

      MajorLabelCondController = new ConditionalDocumentController<AxisLabelStyle>(CreateMajorLabel, RemoveMajorLabel) { UseDocumentCopy = UseDocument.Directly };
      MinorLabelCondController = new ConditionalDocumentController<AxisLabelStyle>(CreateMinorLabel, RemoveMinorLabel) { UseDocumentCopy = UseDocument.Directly };

      AxisStyleController.InitializeDocument(_doc);
      if (_doc.AreMajorLabelsEnabled)
        MajorLabelCondController.InitializeDocument(_doc.MajorLabelStyle);
      if (_doc.AreMinorLabelsEnabled)
        MinorLabelCondController.InitializeDocument(_doc.MinorLabelStyle);
    }
  }
}
