using System;
using Altaxo.Graph;
using Altaxo.Main.GUI;

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
    void InitializeAvailableStyleList(string[] names);
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
  public interface IXYPlotStyleCollectionController : Main.GUI.IMVCAController
  {
    event EventHandler CollectionChangeCommit;
    void OnCollectionChangeCommit();
  }

 
  #endregion

  [UserControllerForObject(typeof(XYPlotStyleCollection))]
  public class XYPlotStyleCollectionController : IXYPlotStyleCollectionViewEventSink, IXYPlotStyleCollectionController
  {
    protected IXYPlotStyleCollectionView _view;
    protected XYPlotStyleCollection _doc;
    protected System.Collections.ArrayList _tempdoc;

    System.Type[] _plotStyleTypes;

    


    public XYPlotStyleCollectionController(XYPlotStyleCollection doc)
    {
      _doc = doc;
      _tempdoc = new System.Collections.ArrayList();
      for(int i=0;i<_doc.Count;i++)
        _tempdoc.Add(_doc[i]);
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
       _plotStyleTypes = Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(I2DPlotStyle));
       string[] names = new string[_plotStyleTypes.Length];
       for(int i=0;i<names.Length;i++)
         names[i] = Current.Gui.GetUserFriendlyClassName(_plotStyleTypes[i]);

       if(_view!=null)
         _view.InitializeAvailableStyleList(names);
     }

    public void InitializePredefinedStyles()
    {
   
      string[] names = XYPlotStyleCollectionTemplates.GetAvailableNamesPlusCustom();
      int idx = XYPlotStyleCollectionTemplates.GetIndexOfAvailableNamesPlusCustom(_doc);
      if(_view!=null)
        _view.InitializePredefinedStyles(names,idx);
    }

    #region IXYPlotStyleCollectionViewEventSink Members

    public virtual void EhView_AddStyle(int[] selindices, int nstyle)
    {
      I2DPlotStyle style = (I2DPlotStyle)Activator.CreateInstance(this._plotStyleTypes[nstyle]);
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

        XYPlotStyleCollection template = XYPlotStyleCollectionTemplates.GetTemplate(selectedindex - 1);
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
      _doc.AddRange((I2DPlotStyle[])_tempdoc.ToArray(typeof(I2DPlotStyle)));
      _doc.EndUpdate();
     return true;
    }

    #endregion
  }

 
}
