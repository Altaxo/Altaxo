﻿#region Copyright

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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;

namespace Altaxo.Graph.Gdi
{
  public static class GraphDocumentLayerCreation
  {
    #region XYPlotLayer Creation

    private static XYPlotLayer CreateNewLayerAtSamePosition(this GraphDocument doc, IEnumerable<int> linklayernumber)
    {
      if(!(doc.RootLayer.IsValidIndex(linklayernumber, out var oldLayer)))
        throw new ArgumentOutOfRangeException("index was not valid");

      IItemLocation location;
      if (oldLayer.Location is ItemLocationByGrid)
      {
        location = (IItemLocation)oldLayer.Location.Clone();
      }
      else if (oldLayer.Location is ItemLocationDirect itemLocationDirect)
      {
        // 1. check if it is possible to create a grid in the parent layer of the old layer
        if (oldLayer.ParentLayer is not null && oldLayer.ParentLayer.CanCreateGridForLocation(itemLocationDirect))
        {
          var gridCell = oldLayer.ParentLayer.CreateGridForLocation(itemLocationDirect) ?? throw new InvalidProgramException();
          oldLayer.Location = gridCell.Clone();
          location = gridCell.Clone();
        }
        else // if we can not create a grid, then we must set the location to that of the the oldLayer, but of course then the two layer locations are not linked
        {
          location = (IItemLocation)oldLayer.Location.Clone();
        }
      }
      else
      {
        throw new NotImplementedException("Location type not implemented");
      }

      var newLayer = new XYPlotLayer(oldLayer.ParentLayer!, location);
      doc.RootLayer.InsertLast(linklayernumber, newLayer);

      return newLayer;
    }

    /// <summary>
    /// Creates a new layer with bottom x axis and left y axis, which is not linked.
    /// </summary>
    public static void CreateNewLayerNormalBottomXLeftY(this GraphDocument doc)
    {
      var context = doc.GetPropertyHierarchy();
      var location = new ItemLocationDirect
      {
        PositionX = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.X),
        PositionY = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.Y),
        SizeX = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.X),
        SizeY = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.Y)
      };

      var newlayer = new XYPlotLayer(doc.RootLayer, location);
      doc.RootLayer.Layers.Add(newlayer);
      newlayer.CreateDefaultAxes(context);
    }

    /// <summary>
    /// Creates a new layer with top x axis, which is linked to the same position with top x axis and right y axis.
    /// </summary>
    public static void CreateNewLayerLinkedTopX(this GraphDocument doc, IEnumerable<int> linklayernumber)
    {
      var context = doc.GetPropertyHierarchy();
      var newlayer = CreateNewLayerAtSamePosition(doc, linklayernumber);
      newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1), context);
    }

    /// <summary>
    /// Creates a new layer with right y axis, which is linked to the same position with top x axis and right y axis.
    /// </summary>
    public static void CreateNewLayerLinkedRightY(this GraphDocument doc, IEnumerable<int> linklayernumber)
    {
      var context = doc.GetPropertyHierarchy();
      var newlayer = CreateNewLayerAtSamePosition(doc, linklayernumber);
      newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1), context);
    }

    /// <summary>
    /// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis.
    /// </summary>
    public static void CreateNewLayerLinkedTopXRightY(this GraphDocument doc, IEnumerable<int> linklayernumber)
    {
      var context = doc.GetPropertyHierarchy();
      var newlayer = CreateNewLayerAtSamePosition(doc, linklayernumber);
      newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1), context);
      newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1), context);
    }

    /// <summary>
    /// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis. The x axis is linked straight to the x axis of the linked layer.
    /// </summary>
    public static void CreateNewLayerLinkedTopXRightY_XAxisStraight(this GraphDocument doc, IEnumerable<int> linklayernumber)
    {
      var context = doc.GetPropertyHierarchy();
      var newlayer = CreateNewLayerAtSamePosition(doc, linklayernumber);

      var isValidIndex = doc.RootLayer.IsValidIndex(linklayernumber, out var oldLayer);
      var linkedLayerAsXYPlotLayer = oldLayer as XYPlotLayer;

      if (linkedLayerAsXYPlotLayer is not null)
      {
        // create a linked x axis of the same type than in the linked layer
        var scaleLinkedTo = linkedLayerAsXYPlotLayer.Scales.X;
        var xScale = new Scales.LinkedScale((Scales.Scale)scaleLinkedTo.Clone());
        newlayer.Scales[0] = xScale;
        xScale.ScaleLinkedTo = scaleLinkedTo; // only now can we set the ScaleLinkedTo, because by now xScale should have a parent object
      }

      // set enabling of axis
      newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1), context);
      newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1), context);
    }

    #endregion XYPlotLayer Creation
  }
}
