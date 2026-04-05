#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  /// <summary>
  /// Common view interface for data-source editors with process data, process options, and import options.
  /// </summary>
  public interface ICommonDataSourceView
  {
    /// <summary>
    /// Sets the process-options control.
    /// </summary>
    /// <param name="p">The control object.</param>
    void SetProcessOptionsControl(object p);

    /// <summary>
    /// Sets the import-options control.
    /// </summary>
    /// <param name="p">The control object.</param>
    void SetImportOptionsControl(object p);

    /// <summary>
    /// Sets the process-data control.
    /// </summary>
    /// <param name="p">The control object.</param>
    void SetProcessDataControl(object p);
  }

  /// <summary>
  /// Common data-source view interface that supports data binding.
  /// </summary>
  public interface ICommonDataSourceViewN : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="ExpandCyclingVariableColumnDataSource"/>.
  /// </summary>
  [UserControllerForObject(typeof(ExpandCyclingVariableColumnDataSource))]
  public class ExpandCyclingVariableDataSourceController : DataSourceControllerBase<ExpandCyclingVariableColumnDataSource>
  {
    /// <inheritdoc/>
    protected override IMVCANController GetProcessDataController()
    {

      var processDataController = new ExpandCyclingVariableDataController() { UseDocumentCopy = UseDocument.Directly };
      processDataController.InitializeDocument(_doc.ProcessData);
      Current.Gui.FindAndAttachControlTo(processDataController);
      return processDataController;
    }
  }
}
