#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.BaselineEstimation;
using Altaxo.Science.Spectroscopy.BaselineEvaluation;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Science.Spectroscopy.Cropping;
using Altaxo.Science.Spectroscopy.DarkSubtraction;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;
using Altaxo.Science.Spectroscopy.Normalization;
using Altaxo.Science.Spectroscopy.Resampling;
using Altaxo.Science.Spectroscopy.Sanitizing;
using Altaxo.Science.Spectroscopy.Smoothing;
using Altaxo.Science.Spectroscopy.SpikeRemoval;

namespace Altaxo.Gui.Science.Spectroscopy
{

  [UserControllerForObject(typeof(SpectralPreprocessingOptionsList))]
  [UserControllerForObject(typeof(SpectralPreprocessingOptions))]
  [ExpectedTypeOfView(typeof(ISpectralPreprocessingOptionsView))]
  public class SpectralPreprocessingController : SpectralPreprocessingControllerBase<SpectralPreprocessingOptionsBase>
  {
    protected override IEnumerable<(string Label, object Doc, Func<IMVCANController> GetController)> GetComponents()
    {
      return GetComponents(_doc);
    }

    public static IEnumerable<(string Label, object Doc, Func<IMVCANController> GetController)> GetComponents(SpectralPreprocessingOptionsBase _doc)
    {
      foreach (var processor in _doc)
      {
        switch (processor)
        {
          case ISanitizer sanitizer:
            yield return ("Sanitizing", sanitizer, () => new Sanitizing.SanitizingController());
            break;
          case ISpikeRemoval spikeRemoval:
            yield return ("SpikeRemoval", spikeRemoval, () => new SpikeRemoval.SpikeRemovalController());
            break;
          case IDarkSubtraction darksubtraction:
            yield return ("Dark", darksubtraction, () => new DarkSubtraction.DarkSubtractionController());
            break;
          case IYCalibration yCalibration:
            yield return ("YCal", yCalibration, () => new Calibration.YCalibrationController());
            break;
          case IXCalibration xCalibration:
            yield return ("XCal", xCalibration, () => new Calibration.XCalibrationController());
            break;
          case ISmoothing smoothing:
            yield return ("Smoothing", smoothing, () => new Smoothing.SmoothingController());
            break;
          case IBaselineEstimation baselineEstimation:
            yield return ("Baseline", baselineEstimation, () => new BaselineEstimation.BaselineEstimationController());
            break;
          case IBaselineEvaluation baselineEvaluation:
            yield return ("BaselineCurve", baselineEvaluation, () => new BaselineEvaluation.BaselineEvaluationController());
            break;
          case IResampling resampling and not ICropping:
            yield return ("Resample", resampling, () => new Resampling.ResamplingController());
            break;
          case ICropping cropping:
            yield return ("Cropping", cropping, () => new Cropping.CroppingController());
            break;
          case INormalization normalization:
            yield return ("Normalization", normalization, () => new Normalization.NormalizationController());
            break;
          case IEnsemblePreprocessor ensemble:
            yield return ("Ensemble", ensemble, () => new EnsembleProcessing.EnsembleProcessingController());
            break;
          default:
            throw new NotImplementedException($"Processor type {processor?.GetType()} is not implemented");
        }
      }
    }

    protected override void UpdateDoc(object model, int index)
    {
      _doc = UpdateDoc(_doc, model, index);
    }

    public static SpectralPreprocessingOptionsBase UpdateDoc(SpectralPreprocessingOptionsBase _doc, object model, int index)
    {
      if (model is not ISingleSpectrumPreprocessor)
      {
        return _doc;
      }

      if (_doc is SpectralPreprocessingOptions doc1)
      {
        switch (model)
        {
          case null:
            break;
          case ISanitizer sa:
            _doc = doc1 with { Sanitizer = sa };
            break;
          case IDarkSubtraction subtraction:
            _doc = doc1 with { DarkSubtraction = subtraction };
            break;
          case ISpikeRemoval sr:
            _doc = doc1 with { SpikeRemoval = sr };
            break;
          case IYCalibration yca:
            _doc = doc1 with { YCalibration = yca };
            break;
          case IXCalibration xca:
            _doc = doc1 with { XCalibration = xca };
            break;
          case INormalization no:
            _doc = doc1 with { Normalization = no };
            break;
          case ICropping cr:
            _doc = doc1 with { Cropping = cr };
            break;
          case IBaselineEstimation be:
            _doc = doc1 with { BaselineEstimation = be };
            break;
          case ISmoothing sm:
            _doc = doc1 with { Smoothing = sm };
            break;
        }
      }
      else if (_doc is SpectralPreprocessingOptionsList doc2)
      {
        var ele = doc2.ToList();
        if (index < doc2.Count)
          ele[index] = (ISingleSpectrumPreprocessor)model;
        else
          ele.Add((ISingleSpectrumPreprocessor)model);
        _doc = new SpectralPreprocessingOptionsList(ele);
      }
      else
      {
        throw new NotImplementedException();
      }

      return _doc;
    }

    protected override SpectralPreprocessingOptionsBase InternalPreprocessingOptions
    {
      get => _doc;
      set => _doc = value;
    }


    protected override bool ApplyEnd(bool applyResult, bool disposeController)
    {
      // clean the SpectralPreprocessingOptionsList by dropping non-elements
      if (applyResult == true)
      {
        if (_doc is SpectralPreprocessingOptionsList doclist)
        {
          _doc = SpectralPreprocessingOptionsList.CreateWithoutNoneElements(doclist);
        }
      }

      return base.ApplyEnd(applyResult, disposeController);
    }

  }
}
