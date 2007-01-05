#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Gui.Graph
{
  #region interfaces
  /// <summary>
  /// Summary description for XYPlotStyleCollectionController.
  /// </summary>
  public interface IXYPlotStyleCollectionView
  {
    IXYPlotStyleCollectionViewEventSink Controller { get; set; }
    void InitializePredefinedStyles(string[] names, int selindex);
    void InitializeStyleList(string[] names, int[] selindices);
    void InitializeAvailableStyleList(List<string> names);
  }
  /// <summary>
  /// Summary description for XYPlotStyleCollectionController.
  /// </summary>
  public interface IXYPlotStyleCollectionViewEventSink
  {
    void EhView_AddStyle(int[] selindices, int style);
    void EhView_StyleUp(int[] selindices);
    void EhView_StyleDown(int[] selindices);
    void EhView_StyleEdit(int[] selindices);
    void EhView_StyleRemove(int[] selindices);
    void EhView_PredefinedStyleSelected(int selectedindex);
  }
  /// <summary>
  /// Summary description for XYPlotStyleCollectionController.
  /// </summary>
  public interface IXYPlotStyleCollectionController : IMVCANController
  {
    event EventHandler CollectionChangeCommit;
    void OnCollectionChangeCommit();
  }

 
  #endregion

  [UserControllerForObject(typeof(G2DPlotStyleCollection))]
  [ExpectedTypeOfView(typeof(IXYPlotStyleCollectionView))]
  public class XYPlotStyleCollectionController : IXYPlotStyleCollectionViewEventSink, IXYPlotStyleCollectionController
  {
    protected IXYPlotStyleCollectionView _view;
    protected G2DPlotStyleCollection _doc;
    protected System.Collections.ArrayList _tempdoc;

    List<System.Type> _plotStyleTypes;


    public XYPlotStyleCollectionController()
    {
    }

    public XYPlotStyleCollectionController(G2DPlotStyleCollection doc)
    {
      _doc = doc;
      _tempdoc = new System.Collections.ArrayList();
      for(int i=0;i<_doc.Count;i++)
        _tempdoc.Add(_doc[i]);
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0)
        return false;
      G2DPlotStyleCollection doc = args[0] as G2DPlotStyleCollection;
      if (doc == null)
        return false;
      
      _doc = doc;
      _tempdoc = new System.Collections.ArrayList();
      for (int i = 0; i < _doc.Count; i++)
        _tempdoc.Add(_doc[i]);

      Initialize();
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    public void Initialize()
    {
      UpdateStyleList(new int[0]);
      InitializeAvailableStyleList();
      InitializePredefinedStyles();
    }

    public void UpdateStyleList(int[] selIndices)
    {
      if(_view!=null)
      {
        string[] names = new string[_tempdoc.Count];
        for(int i=0;i<names.Length;i++)
          names[i] = Current.Gui.GetUserFriendlyClassName(_tempdoc[i].GetType());

        _view.InitializeStyleList(names,selIndices);
      }
    }

    public void InitializeAvailableStyleList()
    {
      Type[] avtypes = Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IG2DPlotStyle));
      _plotStyleTypes = new List<Type>();
      List<string> names = new List<string>();
      for (int i = 0; i < avtypes.Length; i++)
      {
        if (avtypes[i] != typeof(G2DPlotStyleCollection))
        {
          _plotStyleTypes.Add(avtypes[i]);
          names.Add(Current.Gui.GetUserFriendlyClassName(avtypes[i]));
        }
      }

      if(_view!=null)
        _view.InitializeAvailableStyleList(names);
    }

    public void InitializePredefinedStyles()
    {
   
      string[] names = G2DPlotStyleCollectionTemplates.GetAvailableNamesPlusCustom();
      int idx = G2DPlotStyleCollectionTemplates.GetIndexOfAvailableNamesPlusCustom(_doc);
      if(_view!=null)
        _view.InitializePredefinedStyles(names,idx);
    }

    #region IXYPlotStyleCollectionViewEventSink Members

    public virtual void EhView_AddStyle(int[] selindices, int nstyle)
    {
      IG2DPlotStyle style = (IG2DPlotStyle)Activator.CreateInstance(this._plotStyleTypes[nstyle]);
      IPlotArea layer = Main.DocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
      G2DPlotItem plotitem = Main.DocumentPath.GetRootNodeImplementing<G2DPlotItem>(_doc);
      if(layer!=null && plotitem!=null)
        _doc.PrepareNewSubStyle(style, layer, plotitem.GetRangesAndPoints(layer));

      _tempdoc.Add(style);
      UpdateStyleList(new int[]{});

      OnCollectionChangeCommit();
    }

    void Tempdoc_ExchangeItemPositions(int i, int j)
    {
      object oi = _tempdoc[i];
      _tempdoc[i] = _tempdoc[j];
      _tempdoc[j] = oi;

    }

    public virtual void EhView_StyleUp(int[] selIndices)
    {
      if(selIndices.Length==0 || selIndices[0]==0)
        return;

      // Presumption: the first selected index is greater than 0
      for(int i=0;i<selIndices.Length;i++)
      {
        int idx = selIndices[i];
        Tempdoc_ExchangeItemPositions(idx,idx-1);
        selIndices[i]--; // for new list selection
      }

      UpdateStyleList(selIndices);

      OnCollectionChangeCommit();
      
    }

    public virtual void EhView_StyleDown(int[] selIndices)
    {
      if(selIndices.Length==0 || selIndices[selIndices.Length-1]==(_tempdoc.Count-1))
        return;

      // Presumption: the first selected index is greater than 0
      for(int i=selIndices.Length-1;i>=0;i--)
      {
        int idx = selIndices[i];
        Tempdoc_ExchangeItemPositions(idx,idx+1);
        selIndices[i]++; // for new list selection
      }

      UpdateStyleList(selIndices);

      OnCollectionChangeCommit();
    }

    public virtual void EhView_StyleEdit(int[] selindices)
    {
      
    }

    public virtual void EhView_StyleRemove(int[] selindices)
    {
      for(int i=selindices.Length-1;i>=0;--i)
        _tempdoc.RemoveAt(selindices[i]);

      UpdateStyleList(new int[0]);
      OnCollectionChangeCommit();
    }

    public void EhView_PredefinedStyleSelected(int selectedindex)
    {
      if (selectedindex == 0)
        return;

      G2DPlotStyleCollection template = G2DPlotStyleCollectionTemplates.GetTemplate(selectedindex - 1);
      _tempdoc.Clear();
      for (int i = 0; i < template.Count; i++)
        _tempdoc.Add(template[i]);

      UpdateStyleList(new int[0]);
      OnCollectionChangeCommit();
    }

    #endregion

    #region CollectionController Members

    public event EventHandler CollectionChangeCommit;
    public virtual void OnCollectionChangeCommit()
    {
      if(CollectionChangeCommit!=null)
        CollectionChangeCommit(this,EventArgs.Empty);
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IXYPlotStyleCollectionView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public virtual object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    #endregion

    #region IApplyController Members

    public virtual bool Apply()
    {
      _doc.BeginUpdate();
      _doc.Clear();
      _doc.AddRange((IG2DPlotStyle[])_tempdoc.ToArray(typeof(IG2DPlotStyle)));
      _doc.EndUpdate();
      return true;
    }

    #endregion
  }

 
}
