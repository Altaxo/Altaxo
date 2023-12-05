#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using Altaxo.Data;
using Altaxo.Gui.Data;
using Altaxo.Science.Spectroscopy.DarkSubtraction;

namespace Altaxo.Gui.Science.Spectroscopy.DarkSubtraction
{
  public interface ISpectrumSubtractionView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(ISpectrumSubtractionView))]
  [UserControllerForObject(typeof(SpectrumSubtraction))]
  public class SpectrumSubtractionController : MVCANControllerEditImmutableDocBase<SpectrumSubtraction, ISpectrumSubtractionView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(ProxyController, () => { ProxyController = null!; });
    }

    #region Bindings

    private DataTableXYColumnProxyController _proxyController;

    public DataTableXYColumnProxyController ProxyController
    {
      get => _proxyController;
      set
      {
        if (!(_proxyController == value))
        {
          _proxyController?.Dispose();
          _proxyController = value;
          OnPropertyChanged(nameof(ProxyController));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);
      if (initData)
      {
        DataTableXYColumnProxy? proxy = null;
        if (_doc.XYDataOrigin.HasValue)
        {
          var v = _doc.XYDataOrigin.Value;
          if (Current.Project.DataTableCollection.TryGetValue(v.TableName, out var table))
          {
            var xc = table.DataColumns.TryGetColumn(v.XColumnName);
            var yc = table.DataColumns.TryGetColumn(v.YColumnName);
            if (xc is not null && yc is not null)
            {
              proxy = new DataTableXYColumnProxy(table, xc, yc);
            }
          }
        }
        var proxyController = new DataTableXYColumnProxyController();
        proxyController.InitializeDocument(proxy);
        Current.Gui.FindAndAttachControlTo(proxyController);
        ProxyController = proxyController;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (ProxyController is not null)
      {
        if (!ProxyController.Apply(disposeController))
          return ApplyEnd(false, disposeController);

        var proxy = (DataTableXYColumnProxy)ProxyController.ModelObject;

        var table = proxy.DataTable;
        if (table is null)
        {
          Current.Gui.ErrorMessageBox("Please choose a valid table");
          return ApplyEnd(false, disposeController);
        }
        var groupNumber = proxy.GroupNumber;
        var xcol = (DataColumn?)proxy.XColumn;
        if (xcol is null)
        {
          Current.Gui.ErrorMessageBox("Please choose a valid x-column");
          return ApplyEnd(false, disposeController);
        }
        var xcolName = table.DataColumns.GetColumnName(xcol);

        var ycol = (DataColumn?)proxy.YColumn;
        if (ycol is null)
        {
          Current.Gui.ErrorMessageBox("Please choose a valid y-column");
          return ApplyEnd(false, disposeController);
        }
        var ycolName = table.DataColumns.GetColumnName(ycol);
        var len = Math.Min(xcol.Count, ycol.Count);

        var arr = new (double x, double y)[len];
        for (int i = 0; i < arr.Length; ++i)
          arr[i] = (xcol[i], ycol[i]);

        _doc = new SpectrumSubtraction()
        {
          XYDataOrigin = (table.Name, groupNumber, xcolName, ycolName),
          XYCurve = arr.ToImmutableArray(),
        };
      }

      return ApplyEnd(true, disposeController);
    }


  }

}
