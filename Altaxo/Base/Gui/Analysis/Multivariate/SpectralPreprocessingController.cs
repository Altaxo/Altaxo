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

#nullable disable
using System;
using Altaxo.Calc.Regression.Multivariate;

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces

  public interface ISpectralPreprocessingView
  {
    void InitializeMethod(SpectralPreprocessingMethod method);

    void InitializeDetrending(int detrending);

    void InitializeEnsembleScale(bool ensScale);

    event Action<SpectralPreprocessingMethod> MethodChanged;

    event Action<int> DetrendingChanged;

    event Action<bool> EnsembleScaleChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Controls the SpectralPreprocessingControl GUI for choosing <see cref="SpectralPreprocessingOptions" />
  /// </summary>
  [ExpectedTypeOfView(typeof(ISpectralPreprocessingView))]
  public class SpectralPreprocessingController : IMVCAController
  {
    private ISpectralPreprocessingView _view;
    private SpectralPreprocessingOptions _doc;

    /// <summary>
    /// Constructor. Supply a document to control here.
    /// </summary>
    /// <param name="doc">The instance of option to set-up.</param>
    public SpectralPreprocessingController(SpectralPreprocessingOptions doc)
    {
      _doc = doc;
    }

    private void SetElements(bool bInit)
    {
      if (_view is not null)
      {
        _view.InitializeMethod(_doc.Method);
        _view.InitializeDetrending(_doc.DetrendingOrder);
        _view.InitializeEnsembleScale(_doc.EnsembleScale);
      }
    }

    /// <summary>
    /// Get/sets the GUI element that this controller controls.
    /// </summary>
    public ISpectralPreprocessingView View
    {
      get { return _view; }
      set
      {
        if (_view is not null)
        {
          _view.MethodChanged -= EhView_MethodChanged;
          _view.DetrendingChanged -= EhView_DetrendingChanged;
          _view.EnsembleScaleChanged -= EhView_EnsembleScaleChanged;
        }

        _view = value;

        if (_view is not null)
        {
          SetElements(false); // set only the view elements, dont't initialize the variables

          _view.MethodChanged += EhView_MethodChanged;
          _view.DetrendingChanged += EhView_DetrendingChanged;
          _view.EnsembleScaleChanged += EhView_EnsembleScaleChanged;
        }
      }
    }

    /// <summary>
    /// Returns the document.
    /// </summary>
    public SpectralPreprocessingOptions Doc
    {
      get { return _doc; }
    }

    public void EhView_MethodChanged(SpectralPreprocessingMethod newvalue)
    {
      _doc.Method = newvalue;
    }

    public void EhView_DetrendingChanged(int newvalue)
    {
      _doc.DetrendingOrder = newvalue;
    }

    public void EhView_EnsembleScaleChanged(bool newvalue)
    {
      _doc.EnsembleScale = newvalue;
    }

    #region IApplyController Members

    public bool Apply(bool disposeController)
    {
      // nothing to do since all is done
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        View = value as ISpectralPreprocessingView;
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members
  }
}
