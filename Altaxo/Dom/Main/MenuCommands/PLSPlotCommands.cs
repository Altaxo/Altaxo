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

#nullable disable warnings

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Queries the preferred number of PLS factors.
  /// </summary>
  public class PLSQuestPreferredNumberOfFactors : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.QuestPreferredNumberOfFactors(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots predicted versus actual Y values for a PLS model.
  /// </summary>
  public class PLSPlotPredictedVersusActualYIndividually : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotPredictedVersusActualY(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots cross-predicted versus actual Y values for a PLS model.
  /// </summary>
  public class PLSPlotCrossPredictedVersusActualYIndividually : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotCrossPredictedVersusActualY(ctrl.DataTable, true);
    }
  }

  /// <summary>
  /// Plots Y residuals for a PLS model.
  /// </summary>
  public class PLSPlotYResidualsIndividually : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotYResiduals(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots Y cross-residuals for a PLS model.
  /// </summary>
  public class PLSPlotYCrossResidualsIndividually : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotYCrossResiduals(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots X residuals for a PLS model.
  /// </summary>
  public class PLSPlotXResidualsIndividually : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotXResiduals(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots X cross-residuals for a PLS model.
  /// </summary>
  public class PLSPlotXCrossResidualsIndividually : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotXCrossResiduals(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots the PRESS value for a PLS model.
  /// </summary>
  public class PLSPlotPRESSValue : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotPRESSValue(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots the cross-validated PRESS value for a PLS model.
  /// </summary>
  public class PLSPlotCrossPRESSValue : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotCrossPRESSValue(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots X leverage values for a PLS model.
  /// </summary>
  public class PLSPlotXLeverage : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotXLeverage(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots preprocessed spectra for a PLS model.
  /// </summary>
  public class PLSPlotPreprocessedSpectra : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotPreprocessedSpectra(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Plots prediction scores for a PLS model.
  /// </summary>
  public class PLSPlotPredictionScores : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PlotPredictionScores(ctrl.DataTable);
    }
  }
}
