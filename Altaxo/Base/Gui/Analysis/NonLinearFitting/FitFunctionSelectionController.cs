#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Reflection;
using Altaxo.Scripting;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{

  public interface IFitFunctionSelectionView
  {
   

    IFitFunctionSelectionViewEventSink Controller { get; set; }
    void InitializeFitFunctionList(DictionaryEntry[] entries, System.Type currentSelection);
    void InitializeDocumentFitFunctionList(DictionaryEntry[] entries, object currentSelection);
    void InitializeUserFitFunctionList(Altaxo.Main.Services.FitFunctionInformation[] entries);

    void SetRtfDocumentation(string rtfString);
    Color GetRtfBackgroundColor();
  }

  public interface IFitFunctionSelectionViewEventSink
  {
    void EhView_SelectionChanged(object selectedtag);
    void EhView_EditItem(object selectedtag);
  }

  public interface IFitFunctionSelectionController : Main.GUI.IMVCAController
  {
    void Refresh();
  }

  public class FitFunctionSelectionController : IFitFunctionSelectionViewEventSink, Main.GUI.IMVCAController
  {
    IFitFunction _doc;
    object _tempdoc;
    IFitFunctionSelectionView _view;

    public FitFunctionSelectionController(IFitFunction doc)
    {
      _doc = doc;
      _tempdoc = doc;
      Initialize();
    }


    public void Refresh()
    {
      Initialize();
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        DictionaryEntry[] classentries = Altaxo.Main.Services.ReflectionService.GetAttributeInstancesAndClassTypes(typeof(FitFunctionClassAttribute));

        SortedList list = new SortedList();

        foreach (DictionaryEntry entry in classentries)
        {
          System.Type definedtype = (System.Type)entry.Value;

          MethodInfo[] methods = definedtype.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
          foreach (MethodInfo method in methods)
          {
            if (method.IsStatic && method.ReturnType != typeof(void) && method.GetParameters().Length==0)
            {
              object[] attribs = method.GetCustomAttributes(typeof(FitFunctionCreatorAttribute), false);
              foreach(FitFunctionCreatorAttribute creatorattrib in attribs)
                list.Add(creatorattrib,method);
            }
          }
        }


        DictionaryEntry[] entries = new DictionaryEntry[list.Count];
        int j = 0;
        foreach (DictionaryEntry entry in list)
        {
          entries[j++] = entry;
        }


        _view.InitializeFitFunctionList(entries, _tempdoc==null ? null : _tempdoc.GetType());
        _view.InitializeDocumentFitFunctionList(GetDocumentEntries(), null);
        _view.InitializeUserFitFunctionList(Current.FitFunctionService.GetUserDefinedFitFunctions());
      }
    }


    DictionaryEntry[] GetDocumentEntries()
    {
      ArrayList arr = new ArrayList();
      foreach(FitFunctionScript func in Current.Project.FitFunctionScripts)
      {
        arr.Add(new DictionaryEntry(func.FitFunctionCategory+"\\"+func.FitFunctionName,func));
      }

      return (DictionaryEntry[])arr.ToArray(typeof(DictionaryEntry));
    }

    public void EhView_SelectionChanged(object selectedtag)
    {
      if(selectedtag is System.Type)
      {
        _tempdoc = System.Activator.CreateInstance((System.Type)selectedtag) as IFitFunction;
      }
      else if (selectedtag is MethodInfo)
      {
        _tempdoc = ((MethodInfo)selectedtag).Invoke(null,new object[]{}) as IFitFunction;
        SetDocumentation((MethodInfo)selectedtag);
      }
      else if (selectedtag is IFitFunction)
      {
        _tempdoc = (IFitFunction)selectedtag;
      }
      else if (selectedtag is Altaxo.Main.Services.FitFunctionInformation)
      {
        _tempdoc = selectedtag;
      }
    }

    private void SetDocumentation(MethodInfo method)
    {
      object[] attribs = method.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
      
      SetDocumentationText(attribs.Length==0 ? string.Empty : ((System.ComponentModel.DescriptionAttribute)attribs[0]).Description);
    }

    private void SetDocumentationText(string resource)
    {
      string[] resources = resource.Split(new char[]{';',' '},StringSplitOptions.RemoveEmptyEntries);

      System.Text.StringBuilder stb = new StringBuilder();

      foreach (string res in resources)
      {
        if (res.StartsWith("p:"))
        {
          // TODO : Load from a bitmap resource here and add the bitmap to the rtf text
         
        }
        else
        {
          string rawtext = Current.ResourceService.GetString(res);
          if(rawtext!=null && rawtext.Length>0)
            ComposeText(stb, rawtext);
        }
      }

      if (stb.Length != 0)
        stb.Append(texttrailer);

      _view.SetRtfDocumentation(stb.ToString());
    }

    string textheader =
   @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fswiss\fcharset0 Arial;}}" +
   @"\viewkind4\uc1\pard\f0\fs20 ";

    string texttrailer = @"}";
    string imageheader = @"{\pict\wmetafile8 ";
    string imagetrailer = "}";

    MathML.Rendering.GraphicsRendering _mmlRendering = new MathML.Rendering.GraphicsRendering();
    void ComposeText(StringBuilder stb, string rawtext)
    {
      if(stb.Length==0)
        stb.Append(textheader);

      int currpos = 0;
      for (; ; )
      {
        int startidx = rawtext.IndexOf("<math>", currpos);
        if (startidx < 0)
          break;
        int endidx = rawtext.IndexOf("</math>", startidx);
        if (endidx < 0)
          break;
        endidx += "</math>".Length;

        // all text from currpos to startidx-1 can be copyied to the stringbuilder
        stb.Append(rawtext, currpos, startidx - currpos);

        // all text from startidx to endidx-1 must be loaded into the control and rendered
        System.IO.StringReader rd = new StringReader(rawtext.Substring(startidx, endidx - startidx));
        MathML.MathMLDocument doc = new MathML.MathMLDocument();
        doc.Load(rd);
        rd.Close();
        _mmlRendering.BackColor = _view.GetRtfBackgroundColor();
        _mmlRendering.MathElement = (MathML.MathMLMathElement)doc.DocumentElement;

        System.Drawing.Image mf = _mmlRendering.GetImage(typeof(Bitmap));
        GraphicsUnit unit = GraphicsUnit.Point;
        RectangleF rect = mf.GetBounds(ref unit);
        string imagetext = _mmlRendering.GetRtfImage(mf);
        stb.Append(imageheader);
        stb.Append(@"\picwgoal" + Math.Ceiling(15 * rect.Width).ToString());
        stb.Append(@"\pichgoal" + Math.Ceiling(15 * rect.Height).ToString());
        stb.Append(" ");
        stb.Append(imagetext);
        stb.Append(imagetrailer);

        currpos = endidx;
      }

      stb.Append(rawtext, currpos, rawtext.Length - currpos);
    }



    public void EhView_EditItem(object selectedtag)
    {
      if (selectedtag is IFitFunction)
      {
        EditItem((IFitFunction)selectedtag);
      }
      else if (selectedtag is Altaxo.Main.Services.FitFunctionInformation)
      {
        IFitFunction func = Altaxo.Main.Services.FitFunctionService.ReadFileBasedFitFunction(selectedtag as Altaxo.Main.Services.FitFunctionInformation);
        EditItem(func);
      }
    }

    void EditItem(IFitFunction func)
    {
      if (null != func)
      {
        object[] args = new object[] { func };
        if (Current.Gui.ShowDialog(args, "Edit fit function script"))
        {
          if (args[0] is FitFunctionScript)
          {
            Altaxo.Gui.Scripting.FitFunctionNameAndCategoryController ctrl = new Altaxo.Gui.Scripting.FitFunctionNameAndCategoryController((FitFunctionScript)args[0]);
            if (Current.Gui.ShowDialog(ctrl, "Store?"))
            {
              // Note: category and/or name can have changed now, so it is more save to
              // completely reinitialize the fit function tree
              Initialize();
            }
          }

        }
      }
    }



      

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

        _view = value as IFitFunctionSelectionView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      
      if(_tempdoc is IFitFunction)
      {
        _doc = (IFitFunction)_tempdoc;
        return true;
      }

      else if (_tempdoc is Main.Services.FitFunctionInformation)
      {
        Main.Services.FitFunctionInformation info = (Main.Services.FitFunctionInformation)_tempdoc;
        _doc = Altaxo.Main.Services.FitFunctionService.ReadFileBasedFitFunction(info);
        return _doc!=null;
      }

      return false;
    }

    #endregion
  }
}
