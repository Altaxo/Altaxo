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
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Collections;

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces

  public interface ISpectralPreprocessingView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Controls the SpectralPreprocessingControl GUI for choosing <see cref="SpectralPreprocessingOptions" />
  /// </summary>
  [ExpectedTypeOfView(typeof(ISpectralPreprocessingView))]
  [UserControllerForObject(typeof(SpectralPreprocessingOptions))]
  public class SpectralPreprocessingController : MVCANControllerEditOriginalDocBase<SpectralPreprocessingOptions, ISpectralPreprocessingView>
  {
    public SpectralPreprocessingController()
    {
    }

    /// <summary>
    /// Constructor. Supply a document to control here.
    /// </summary>
    /// <param name="doc">The instance of option to set-up.</param>
    public SpectralPreprocessingController(SpectralPreprocessingOptions doc)
    {
      _doc = doc;
      Initialize(true);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _ensembleScale;

    public bool EnsembleScale
    {
      get => _ensembleScale;
      set
      {
        if (!(_ensembleScale == value))
        {
          _ensembleScale = value;
          OnPropertyChanged(nameof(EnsembleScale));
        }
      }
    }

    public SelectableListNodeList SpectralPreprocessingMethods { get; } = new SelectableListNodeList();

    public SelectableListNodeList DetrendingMethods { get; } = new SelectableListNodeList();

    #endregion

    void InitializeSpectralPreprocessingMethods()
    {
      foreach (SpectralPreprocessingMethod v in Enum.GetValues(typeof(SpectralPreprocessingMethod)))
      {
        SpectralPreprocessingMethods.Clear();

        var text = v switch
        {
          SpectralPreprocessingMethod.None => "None",
          SpectralPreprocessingMethod.FirstDerivative => "1st derivative",
          SpectralPreprocessingMethod.SecondDerivative => "2nd derivative",
          SpectralPreprocessingMethod.MultiplicativeScatteringCorrection => "MSC",
          SpectralPreprocessingMethod.StandardNormalVariate => "SNV",
          _ => Enum.GetName(typeof(SpectralPreprocessingMethod), v)
        };

        SpectralPreprocessingMethods.Add(new SelectableListNode(text, v, v == _doc.Method));
      }
    }

    void InitializeDetrendingMethods()
    {
      DetrendingMethods.Clear();
      DetrendingMethods.Add(new SelectableListNode("None", -1, -1 == _doc.DetrendingOrder));
      DetrendingMethods.Add(new SelectableListNode("Spectrum mean", 0, 0 == _doc.DetrendingOrder));
      DetrendingMethods.Add(new SelectableListNode("Linear", 1, 1 == _doc.DetrendingOrder));
      DetrendingMethods.Add(new SelectableListNode("Quadratic", 2, 2 == _doc.DetrendingOrder));
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        InitializeSpectralPreprocessingMethods();
        InitializeDetrendingMethods();
        EnsembleScale = _doc.EnsembleScale;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.Method = SpectralPreprocessingMethods.FirstSelectedNode?.Tag is SpectralPreprocessingMethod m ? m : SpectralPreprocessingMethod.None;
      _doc.DetrendingOrder = DetrendingMethods.FirstSelectedNode?.Tag is int d ? d : -1;
      _doc.EnsembleScale = EnsembleScale;
      return ApplyEnd(true, disposeController);
    }
  }
}

