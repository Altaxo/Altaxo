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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Serialization;
using Altaxo.Graph.GUI.GraphControllerMouseHandlers;
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph;

namespace Altaxo.Graph.GUI
{
  /// <summary>
  /// GraphController is our default implementation to control a graph view.
  /// </summary>
  [SerializationSurrogate(0,typeof(GraphController.SerializationSurrogate0))]
  [SerializationVersion(0)]
  [Altaxo.Gui.UserControllerForObject(typeof(GraphDocument))]
  [Altaxo.Gui.ExpectedTypeOfView(typeof(IGraphView))]
  public class GraphController 
    :
    IGraphController,
    System.Runtime.Serialization.IDeserializationCallback
    
  {

    #region Member variables

    // following default unit is point (1/72 inch)
    /// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
    protected const float UnitPerInch = 72;


    /// <summary>Holds the Graph document (the place were the layers, plots, graph elements... are stored).</summary>
    protected GraphDocument m_Graph;

    /// <summary>Holds the view (the window where the graph is visualized).</summary>
    protected IGraphView m_View;
    
    /// <summary>The main menu of this controller.</summary>
    protected System.Windows.Forms.MainMenu m_MainMenu; 
    
    /// <summary>Special menu item to show the currently available plots.</summary>
    protected System.Windows.Forms.MenuItem m_MenuDataPopup;

    /// <summary>
    /// Color for the area of the view, where there is no page.
    /// </summary>
    protected Color m_NonPageAreaColor;

    /// <summary>
    /// Brush to fill the page ground. Since the printable area is filled with another brush, in effect
    /// this brush fills only the non printable margins of the page. 
    /// </summary>
    protected BrushX m_PageGroundBrush;

    /// <summary>
    /// Brush to fill the printable area of the graph.
    /// </summary>
    protected BrushX m_PrintableAreaBrush;

    /// <summary>Current horizontal resolution of the paint method.</summary>
    protected float m_HorizRes;
    
    /// <summary>Current vertical resolution of the paint method.</summary>
    protected float m_VertRes;

    /// <summary>Current zoom factor. If AutoZoom is on, this factor is calculated automatically.</summary>
    protected float m_Zoom;
    
    /// <summary>If true, the view is zoomed so that the page fits exactly into the viewing area.</summary>
    protected bool  m_AutoZoom; // if true, the sheet is zoomed as big as possible to fit into window
    
    /// <summary>Number of the currently selected layer (or -1 if no layer is present).</summary>
    protected int m_CurrentLayerNumber;

    /// <summary>Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
    protected int m_CurrentPlotNumber;
    
    /// <summary>A instance of a mouse handler class that currently handles the mouse events..</summary>
    protected MouseStateHandler m_MouseState;



    /// <summary>
    /// This holds a frozen image of the graph during the moving time
    /// </summary>
    protected Bitmap m_FrozenGraph;

    protected bool   m_FrozenGraphIsDirty;

    /// <summary>
    /// Necessary to determine if deserialization finisher already has finished serialization.
    /// </summary>
    private object m_DeserializationSurrogate;

    #endregion Member variables

    #region Serialization
    /// <summary>Used to serialize the GraphController Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes the GraphController (version 0).
      /// </summary>
      /// <param name="obj">The GraphController to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        GraphController s = (GraphController)obj;
        info.AddValue("AutoZoom",s.m_AutoZoom);
        info.AddValue("Zoom",s.m_Zoom);
        info.AddValue("Graph",s.m_Graph);
      }
      /// <summary>
      /// Deserializes the GraphController (version 0).
      /// </summary>
      /// <param name="obj">The empty GraphController object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized GraphController.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        GraphController s = (GraphController)obj;
        s.SetMemberVariablesToDefault();

        s.m_AutoZoom = info.GetBoolean("AutoZoom");
        s.m_Zoom = info.GetSingle("Zoom");
        s.m_Graph = (GraphDocument)info.GetValue("Graph",typeof(GraphDocument));

        s.m_DeserializationSurrogate = this;
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphController),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      Main.DocumentPath _PathToGraph;
      GraphController   _GraphController;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GraphController s = (GraphController)obj;
        info.AddValue("AutoZoom",s.m_AutoZoom);
        info.AddValue("Zoom",s.m_Zoom);
        info.AddValue("Graph",Main.DocumentPath.GetAbsolutePath(s.m_Graph));
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        GraphController s = null!=o ? (GraphController)o : new GraphController(null,true);
        s.m_AutoZoom = info.GetBoolean("AutoZoom");
        s.m_Zoom = info.GetSingle("Zoom");
        s.m_Graph = null;
        
        XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
        surr._GraphController = s;
        surr._PathToGraph = (Main.DocumentPath)info.GetValue("Graph",s);
        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
        
        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        object o = Main.DocumentPath.GetObject(_PathToGraph,documentRoot,_GraphController);
        if(o is GraphDocument)
        {
          _GraphController.Doc = o as GraphDocument;
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
        }
      }
    }


    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      if(null!=this.m_DeserializationSurrogate && obj is DeserializationFinisher)
      {
        m_DeserializationSurrogate=null;

        // first finish the document
        DeserializationFinisher finisher = new DeserializationFinisher(this);
        
        m_Graph.OnDeserialization(finisher);


        // create the menu
        this.InitializeMenu();

        // set the menu of this class
        m_View.GraphMenu = this.m_MainMenu;


        // restore event chain to GraphDocument
        m_Graph.Changed += new EventHandler(this.EhGraph_Changed);
        m_Graph.Layers.LayerCollectionChanged += new EventHandler(this.EhGraph_LayerCollectionChanged);


        // Ensure the current layer and plot numbers are valid
        this.EnsureValidityOfCurrentLayerNumber();
        this.EnsureValidityOfCurrentPlotNumber();
      }
    }
    #endregion

    #region Constructors


    /// <summary>
    /// Set the member variables to default values. Intended only for use in constructors and deserialization code.
    /// </summary>
    protected virtual void SetMemberVariablesToDefault()
    {
      m_NonPageAreaColor = Color.Gray;
    
      m_PageGroundBrush = new BrushX(Color.LightGray);

      m_PrintableAreaBrush = new BrushX(Color.Snow);

      m_HorizRes  = 300;
    
      m_VertRes = 300;

      m_Zoom  = 0.4f;
    
      // If true, the view is zoomed so that the page fits exactly into the viewing area.</summary>
      m_AutoZoom = true; // if true, the sheet is zoomed as big as possible to fit into window
    
    
      // Number of the currently selected layer (or -1 if no layer is present).</summary>
      m_CurrentLayerNumber = -1;
    
      // Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
      m_CurrentPlotNumber = -1;
    
      // Currently selected GraphTool.</summary>
      // m_CurrentGraphTool = GraphTools.ObjectPointer;
    
      // A instance of a mouse handler class that currently handles the mouse events..</summary>
      m_MouseState= new ObjectPointerMouseHandler(this);

      

      // This holds a frozen image of the graph during the moving time
      m_FrozenGraph=null;
    }

    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.    
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    public GraphController(GraphDocument graphdoc)
      : this(graphdoc,false)
    {
    }

    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    /// <param name="bDeserializationConstructor">If true, this is a special constructor used only for deserialization, where no graphdoc needs to be supplied.</param>
    public GraphController(GraphDocument graphdoc, bool bDeserializationConstructor)
    {
      SetMemberVariablesToDefault();
    
      if(null!=graphdoc)
        this.Doc = graphdoc;
      else if(null==graphdoc && !bDeserializationConstructor)
        throw new ArgumentNullException("graphdoc","GraphDoc must not be null");

      this.InitializeMenu();

      if(null!=Doc && 0==Doc.Layers.Count)
        Doc.CreateNewLayerNormalBottomXLeftY();
    }


    static GraphController()
    {
      // register here editor methods
      LayerController.RegisterEditHandlers();
      XYPlotLayer.PlotItemEditorMethod = new DoubleClickHandler(EhEditPlotItem);
      TextGraphic.PlotItemEditorMethod = new DoubleClickHandler(EhEditPlotItem);
      TextGraphic.TextGraphicsEditorMethod = new DoubleClickHandler(EhEditTextGraphics);
    }

    #endregion // Constructors

    #region Menu Definition


    /// <summary>
    /// Creates the default menu of a graph view.
    /// </summary>
    /// <remarks>In case there is already a menu here, the old menu is overwritten.</remarks>
    public void InitializeMenu()
    {
      int index=0, index2=0;
      MenuItem mi;

      m_MainMenu = new MainMenu();

      // File Menu
      // **********************************************************
      mi = new MenuItem("&File");
      mi.Index=0;
      mi.MergeOrder=0;
      mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
      m_MainMenu.MenuItems.Add(mi);
      index = m_MainMenu.MenuItems.Count-1;

      // File - Page Setup
      mi = new MenuItem("Page Setup..");
      mi.Click += new EventHandler(EhMenuFilePageSetup_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // File - Print Preview
      mi = new MenuItem("Print Preview..");
      mi.Click += new EventHandler(EhMenuFilePrintPreview_OnClick);
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // File - Print 
      mi = new MenuItem("Print..");
      mi.Click += new EventHandler(EhMenuFilePrint_OnClick);
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // File - Save Graph As
      mi = new MenuItem("Save Graph As..");
      mi.Click += new EventHandler(EhMenuFileSaveGraphAs_OnClick);
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // File - Export (Popup)
      // ------------------------------------------------------------------
      mi = new MenuItem("Export");
      //mi.Popup += new EventHandler(MenuFileExport_OnPopup);
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);
      index2 = m_MainMenu.MenuItems[index].MenuItems.Count-1;

      // File - Export - Metafile 
      mi = new MenuItem("Metafile");
      mi.Click += new EventHandler(EhMenuFileExportMetafile_OnClick);
      m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);


      // Edit (Popup)
      // ****************************************************************** 
      mi = new MenuItem("Edit");
      mi.Index=1;
      mi.MergeOrder=1;
      mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
      m_MainMenu.MenuItems.Add(mi);
      index = m_MainMenu.MenuItems.Count-1;

      // Edit - NewLayer (Popup)
      // ------------------------------------------------------------------
      mi = new MenuItem("New layer(axes)");
      //mi.Popup += new EventHandler(MenuFileExport_OnPopup);
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);
      index2 = m_MainMenu.MenuItems[index].MenuItems.Count-1;

      // Edit - NewLayer - Normal:Bottom X and Left Y 
      mi = new MenuItem("(Normal): Bottom X + Left Y ");
      mi.Click += new EventHandler(EhMenuEditNewlayerNormalBottomXLeftY_OnClick);
      m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

      // Edit - NewLayer - "(Linked: Top X + Right Y" 
      mi = new MenuItem("(Linked: Top X + Right Y");
      mi.Click += new EventHandler(EhMenuEditNewlayerLinkedTopXRightY_OnClick);
      m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

      // Edit - NewLayer - "(Linked): Top X" 
      mi = new MenuItem("(Linked): Top X");
      mi.Click += new EventHandler(EhMenuEditNewlayerLinkedTopX_OnClick);
      m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

      // Edit - NewLayer - "(Linked): Right Y" 
      mi = new MenuItem("(Linked): Right Y");
      mi.Click += new EventHandler(EhMenuEditNewlayerLinkedRightY_OnClick);
      m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

      // Edit - NewLayer - "(Linked): Top X + Right Y + X Axis Straight" 
      mi = new MenuItem("(Linked): Top X + Right Y + X Axis Straight");
      mi.Click += new EventHandler(EhMenuEditNewlayerLinkedTopXRightYXAxisStraight_OnClick);
      m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);


      // Data (Popup)
      // ****************************************************************** 
      mi = new MenuItem("Data");
      mi.Index=3;
      mi.MergeOrder=3;
      mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
      m_MenuDataPopup = mi; // store this for later manimpulation
      m_MainMenu.MenuItems.Add(mi);
      index = m_MainMenu.MenuItems.Count-1;


      // Graph (Popup)
      // ****************************************************************** 
      mi = new MenuItem("Graph");
      mi.Index=4;
      mi.MergeOrder=4;
      mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
      m_MainMenu.MenuItems.Add(mi);
      index = m_MainMenu.MenuItems.Count-1;

      // Graph - Duplicate
      mi = new MenuItem("Duplicate Graph");
      mi.Click += new EventHandler(EhMenuGraphDuplicate_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // Graph - NewLayerLegend
      mi = new MenuItem("New layer legend");
      mi.Click += new EventHandler(EhMenuGraphNewLayerLegend_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // Graph - XYPlotLayer control
      mi = new MenuItem("XYPlotLayer control");
      mi.Click += new EventHandler(EhMenuGraphLayer_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // Graph - Add curve plot
      mi = new MenuItem("Add Curve Plot");
      mi.Click += new EventHandler(EhMenuGraphAddCurvePlot_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);
    }


    /// <summary>
    /// Updates a special menu item, the data item, with the currently available plot names. The active plot is marked with a
    /// check.
    /// </summary>
    public void UpdateDataPopup()
    {
      if(null==this.m_MenuDataPopup)
        return; // as long there is no menu, we cannot do it

      // first delete old menuitems
      this.m_MenuDataPopup.MenuItems.Clear();


      // check there is at least one layer
      if(m_Graph.Layers.Count==0)
        return; // there is no layer, we can not have items in the data menu

      // now it is save to get the active layer
      int actLayerNum = this.CurrentLayerNumber;
      XYPlotLayer actLayer = this.Layers[actLayerNum];

      // then append the plot associations of the actual layer

      int actPA = CurrentPlotNumber;
      int len = actLayer.PlotItems.Flattened.Length;
      for(int i = 0; i<len; i++)
      {
        IGPlotItem pa = actLayer.PlotItems.Flattened[i];
        DataMenuItem mi = new DataMenuItem(pa.ToString(), new EventHandler(EhMenuData_Data));
        mi.Checked = (i==actPA);
        mi.PlotItemNumber = i;
        this.m_MenuDataPopup.MenuItems.Add(mi);
      }
    }




    #endregion // Menu definition

    #region Menu event handlers

    /// <summary>
    /// Handler for the menu item "File" - "Setup Page".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuFilePageSetup_OnClick(object sender, System.EventArgs e)
    {
      try
      {
        Current.Gui.ShowPageSetupDialog();
      }
      catch(Exception exc)
      {
        MessageBox.Show(exc.ToString(),"Exception occured!");
      }
    }

    /// <summary>
    /// Handler for the menu item "File" - "Print".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuFilePrint_OnClick(object sender, System.EventArgs e)
    {
      try
      {
        if(Current.Gui.ShowPrintDialog())
        {
          Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
          Current.PrintingService.PrintDocument.Print();
        }
      }
      catch(Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(this.m_View.Window,ex.ToString());
      }
      finally
      {
        Current.PrintingService.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
      }
    }
  

    /// <summary>
    /// Handler for the menu item "File" - "Print Preview".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuFilePrintPreview_OnClick(object sender, System.EventArgs e)
    {
      try
      {
        System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog();
        Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
        dlg.Document = Current.PrintingService.PrintDocument;
        dlg.ShowDialog(this.m_View.Window);
        dlg.Dispose();
      }
      catch(Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(this.m_View.Window,ex.ToString());
      }
      finally
      {
        Current.PrintingService.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.EhPrintPage);
      }
    }


    protected void EhMenuFileSaveGraphAs_OnClick(object sender, System.EventArgs e)
    {
      System.IO.Stream myStream ;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
      saveFileDialog1.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*"  ;
      saveFileDialog1.FilterIndex = 1 ;
      saveFileDialog1.RestoreDirectory = true ;
 
      if(saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = saveFileDialog1.OpenFile()) != null)
        {
          Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
          info.BeginWriting(myStream);
          info.AddValue("Graph",this.Doc);
          info.EndWriting();
          myStream.Close();
        }
      }
    }

    /// <summary>
    /// Handler for the menu item "File" - "Export Metafile".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuFileExportMetafile_OnClick(object sender, System.EventArgs e)
    {
      System.IO.Stream myStream ;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
      saveFileDialog1.Filter = "Windows Metafiles (*.emf)|*.emf|All files (*.*)|*.*"  ;
      saveFileDialog1.FilterIndex = 2 ;
      saveFileDialog1.RestoreDirectory = true ;
 
      if(saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = saveFileDialog1.OpenFile()) != null)
        {
          this.SaveAsMetafile(myStream);
          myStream.Close();
        } // end openfile ok
      } // end dlgresult ok

    }


    /// <summary>
    /// Handler for the menu item "Edit" - "New layer(axes)" - "Normal: Bottom X Left Y".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuEditNewlayerNormalBottomXLeftY_OnClick(object sender, System.EventArgs e)
    {
      m_Graph.CreateNewLayerNormalBottomXLeftY();
    }

    /// <summary>
    /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuEditNewlayerLinkedTopXRightY_OnClick(object sender, System.EventArgs e)
    {
      m_Graph.CreateNewLayerLinkedTopXRightY(CurrentLayerNumber);
    }

    /// <summary>
    /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuEditNewlayerLinkedTopX_OnClick(object sender, System.EventArgs e)
    {
    
    }

    /// <summary>
    /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Right Y".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuEditNewlayerLinkedRightY_OnClick(object sender, System.EventArgs e)
    {
    
    }



    /// <summary>
    /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y, X axis straight ".
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuEditNewlayerLinkedTopXRightYXAxisStraight_OnClick(object sender, System.EventArgs e)
    {
      m_Graph.CreateNewLayerLinkedTopXRightY_XAxisStraight(CurrentLayerNumber);
    }


    /// <summary>
    /// Duplicates the Graph and the Graph view to a new one.
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuGraphDuplicate_OnClick(object sender, System.EventArgs e)
    {
      GraphDocument newDoc = new GraphDocument(this.Doc);
      Current.ProjectService.CreateNewGraph(newDoc);
    }

    private void EhMenuGraphLayer_OnClick(object sender, System.EventArgs e)
    {
      EnsureValidityOfCurrentLayerNumber();
      if(null!=this.ActiveLayer)
        LayerController.ShowDialog(this.ActiveLayer);
    }

    private void EhMenuGraphAddCurvePlot_OnClick(object sender, System.EventArgs e)
    {
      EnsureValidityOfCurrentLayerNumber();
      this.Doc.Layers[this.CurrentLayerNumber].PlotItems.Add(new XYFunctionPlotItem(new XYFunctionPlotData(new PolynomialFunction(new double[]{0,0,1})),new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line)));
    }

    /// <summary>
    /// Handler for the menu item "Graph" - "New layer legend.
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void EhMenuGraphNewLayerLegend_OnClick(object sender, System.EventArgs e)
    {
      m_Graph.Layers[CurrentLayerNumber].CreateNewLayerLegend();
    }


    /// <summary>
    /// Handler for all submenu items of the data popup.".
    /// </summary>
    /// <param name="sender">The menuitem, must be of type <see cref="DataMenuItem"/>.</param>
    /// <param name="e">Not used.</param>
    /// <remarks>The handler either checks the menuitem, if it was unchecked. If it was already checked,
    /// it shows the LineScatterPlotStyleControl into a dialog box.
    /// </remarks>
    private void EhMenuData_Data(object sender, System.EventArgs e)
    {
      DataMenuItem dmi = (DataMenuItem)sender;

      if(!dmi.Checked)
      {
        // if the menu item was not checked before, check it now
        // by making the plot association shown by the menu item
        // the actual plot association
        int actLayerNum = this.CurrentLayerNumber;
        XYPlotLayer actLayer = this.Layers[actLayerNum];
        if(null!=actLayer && dmi.PlotItemNumber<actLayer.PlotItems.Flattened.Length)
        {
          dmi.Checked=true;
          CurrentPlotNumber = dmi.PlotItemNumber;
        }
      }
      else
      {
        // if it was checked before, then bring up the plot style dialog
        // of the plot association represented by this menu item
        int actLayerNum = this.CurrentLayerNumber;
        XYPlotLayer actLayer = this.Layers[actLayerNum];
        IGPlotItem pa = actLayer.PlotItems.Flattened[CurrentPlotNumber];

        Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()),true);
      }
      
        

    }


    
    #endregion // Menu event handlers

    #region IGraphController interface definitions

    /// <summary>
    /// This returns the GraphDocument that is managed by this controller.
    /// </summary>
    public GraphDocument Doc
    {
      get { return m_Graph; }
      set
      {
        GraphDocument oldDoc=m_Graph;
        m_Graph = value;
        if(!object.ReferenceEquals(m_Graph,oldDoc))
        {
          if(oldDoc!=null)
          {
            oldDoc.Changed -= new EventHandler(this.EhGraph_Changed);
            oldDoc.Layers.LayerCollectionChanged -= new EventHandler(this.EhGraph_LayerCollectionChanged);
            oldDoc.BoundsChanged -= new EventHandler(this.EhGraph_BoundsChanged);
            oldDoc.NameChanged -= new Main.NameChangedEventHandler(this.EhGraphDocumentNameChanged);
          }
          if(m_Graph!=null)
          {
            m_Graph.Changed += new EventHandler(this.EhGraph_Changed);
            m_Graph.Layers.LayerCollectionChanged += new EventHandler(this.EhGraph_LayerCollectionChanged);
            m_Graph.BoundsChanged += new EventHandler(this.EhGraph_BoundsChanged);
            m_Graph.NameChanged += new Main.NameChangedEventHandler(this.EhGraphDocumentNameChanged);

            // Ensure the current layer and plot numbers are valid
            this.EnsureValidityOfCurrentLayerNumber();
            this.EnsureValidityOfCurrentPlotNumber();
          }
        }
      }
    }

    /// <summary>
    /// Returns the view that this controller controls.
    /// </summary>
    /// <remarks>Setting the view is only neccessary on deserialization, so the controller
    /// can restrict setting the view only the own view is still null.</remarks>
    public IGraphView View
    {
      get { return m_View; }
      set
      {
        IGraphView oldView = m_View;
        m_View = value;

        if(null!=oldView)
        {
          oldView.GraphMenu = null; // don't let the old view have the menu
          oldView.Controller = null; // no longer the controller of this view
        }

        if(null!=m_View)
        {
          m_View.Controller = this;
          m_View.GraphMenu = m_MainMenu;
          m_View.NumberOfLayers = m_Graph.Layers.Count;
          m_View.CurrentLayer = this.CurrentLayerNumber;
          //m_View.CurrentGraphTool = this.CurrentGraphTool;
        
          // Adjust the zoom level just so, that area fits into control
          Graphics grfx = m_View.CreateGraphGraphics();
          this.m_HorizRes = grfx.DpiX;
          this.m_VertRes = grfx.DpiY;
          grfx.Dispose();

          // Calculate the zoom if Autozoom is on - simulate a SizeChanged event of the view to force calculation of new zoom factor
          this.EhView_GraphPanelSizeChanged(new EventArgs());

          // set the menu of this class
          m_View.GraphMenu = this.m_MainMenu;
          m_View.NumberOfLayers = m_Graph.Layers.Count; // tell the view how many layers we have
        
        }
      }
    }

    /// <summary>
    /// Creates a default view object.
    /// </summary>
    /// <returns>The default view object, or null if there is no default view object.</returns>
    public virtual object CreateDefaultViewObject()
    {
      this.View = new GraphView();
      return this.View;
    }

    /// <summary>
    /// Handles the selection of the current layer by the <b>user</b>.
    /// </summary>
    /// <param name="currLayer">The current layer number as selected by the user.</param>
    /// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
    public virtual void EhView_CurrentLayerChoosen(int currLayer, bool bAlternative)
    {
      int oldCurrLayer = this.CurrentLayerNumber;
      this.CurrentLayerNumber = currLayer;


      // if we have clicked the button already down then open the layer dialog
      if(null!=ActiveLayer && currLayer==oldCurrLayer && false==bAlternative)
      {
        LayerController.ShowDialog(ActiveLayer);
        //LayerDialog dlg = new LayerDialog(ActiveLayer,LayerDialog.Tab.Scale,EdgeType.Bottom);
        //dlg.ShowDialog(this.m_View.Window);
      }
    }

    /// <summary>
    /// The controller should show a data context menu (contains all plots of the currentLayer).
    /// </summary>
    /// <param name="currLayer">The layer number. The controller has to make this number the CurrentLayerNumber.</param>
    /// <param name="parent">The parent control which is the parent of the context menu.</param>
    /// <param name="pt">The location where the context menu should be shown.</param>
    public virtual void EhView_ShowDataContextMenu(int currLayer, System.Windows.Forms.Control parent, Point pt)
    {
      int oldCurrLayer = this.CurrentLayerNumber;
      this.CurrentLayerNumber = currLayer;


      if(null!=this.ActiveLayer)
      {
        // then append the plot associations of the actual layer
        ContextMenu contextMenu = new ContextMenu();

        int actPA = CurrentPlotNumber;
        int len = ActiveLayer.PlotItems.Flattened.Length;
        for(int i = 0; i<len; i++)
        {
          IGPlotItem pa = ActiveLayer.PlotItems.Flattened[i];
          DataMenuItem mi = new DataMenuItem(pa.ToString(), new EventHandler(EhMenuData_Data));
          mi.Checked = (i==actPA);
          mi.PlotItemNumber = i;
          contextMenu.MenuItems.Add(mi);
            
        }
        contextMenu.Show(parent,pt);
      }
    }

    /// <summary>
    /// This function is called if the user changed the GraphTool.
    /// </summary>
    /// <param name="currGraphToolType">The type of the new selected GraphTool.</param>
    public virtual void EhView_CurrentGraphToolChoosen(System.Type currGraphToolType)
    {
      this.CurrentGraphToolType = currGraphToolType;
    }

    /// <summary>
    /// Called if a key is pressed in the view.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    public bool EhView_ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if(this.m_MouseState!=null)
        return this.m_MouseState.ProcessCmdKey(ref msg, keyData);
      else
        return false;
    }

    
    /// <summary>
    /// Called if the host window is about to be closed.
    /// </summary>
    /// <returns>True if the closing should be canceled, false otherwise.</returns>
    public bool HostWindowClosing()
    {
      if(!Current.ApplicationIsClosing)
      {

        System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this.m_View.Window,"Do you really want to close this graph?","Attention",System.Windows.Forms.MessageBoxButtons.YesNo);

        if(dlgres==System.Windows.Forms.DialogResult.No)
        {
          return true; // cancel the closing
        }
      }
      return false;
    }

    /// <summary>
    /// Called by the host window after the host window was closed.
    /// </summary>
    public void HostWindowClosed()
    {
      Current.ProjectService.RemoveGraph(this);
    }


    /// <summary>
    /// Handles the event when the size of the graph area is changed.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    public virtual void EhView_GraphPanelSizeChanged(EventArgs e)
    {
      if(m_AutoZoom)
      {
        this.m_Zoom = CalculateAutoZoom();
    
        // System.Console.WriteLine("h={0}, v={1} {3} {4} {5}",zoomh,zoomv,UnitPerInch,this.ClientSize.Width,this.m_HorizRes, this.m_PageBounds.Width);
        // System.Console.WriteLine("SizeX = {0}, zoom = {1}, dpix={2},in={3}",this.ClientSize.Width,this.m_Zoom,this.m_HorizRes,this.ClientSize.Width/(this.m_HorizRes*this.m_Zoom));
      
        m_View.GraphScrollSize= new Size(0,0);
      }
      else
      {
        double pixelh = System.Math.Ceiling(m_Graph.PageBounds.Width*this.m_HorizRes*this.m_Zoom/(UnitPerInch));
        double pixelv = System.Math.Ceiling(m_Graph.PageBounds.Height*this.m_VertRes*this.m_Zoom/(UnitPerInch));
        m_View.GraphScrollSize = new Size((int)pixelh,(int)pixelv);
      }

    }


    /// <summary>
    /// Handles the mouse up event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    public virtual void EhView_GraphPanelMouseUp(System.Windows.Forms.MouseEventArgs e)
    {
      m_MouseState.OnMouseUp(e);
    }

    /// <summary>
    /// Handles the mouse down event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    public virtual void EhView_GraphPanelMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
      m_MouseState.OnMouseDown(e);
    }

    /// <summary>
    /// Handles the mouse move event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    public virtual void EhView_GraphPanelMouseMove(System.Windows.Forms.MouseEventArgs e)
    {
      m_MouseState.OnMouseMove(e);
    }

    /// <summary>
    /// Handles the click onto the graph event in the controller class.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    public virtual void EhView_GraphPanelMouseClick(System.EventArgs e)
    {
      m_MouseState.OnClick(e);
    }

    /// <summary>
    /// Handles the double click onto the graph event in the controller class.
    /// </summary>
    /// <param name="e"></param>
    public virtual void EhView_GraphPanelMouseDoubleClick(System.EventArgs e)
    {
      m_MouseState.OnDoubleClick(e);
    }

    /// <summary>
    /// Handles the paint event of that area, where the graph is shown.
    /// </summary>
    /// <param name="e">The paint event args.</param>
    public virtual void EhView_GraphPanelPaint(System.Windows.Forms.PaintEventArgs e)
    {
      if(!e.ClipRectangle.IsEmpty)
        this.DoPaint(e.Graphics,false);
    }

    #endregion // IGraphView interface definitions

    #region GraphDocument event handlers

    /// <summary>
    /// Handler of the event LayerCollectionChanged of the graph document. Forces to
    /// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
    /// <param name="sender">The sender of the event (the GraphDocument).</param>
    /// <param name="e">The event arguments.</param>
    protected void EhGraph_LayerCollectionChanged(object sender, System.EventArgs e)
    {
      int oldActiveLayer = this.m_CurrentLayerNumber;

      // Ensure that the current layer and current plot are valid anymore
      EnsureValidityOfCurrentLayerNumber();

      if(oldActiveLayer!=this.m_CurrentLayerNumber)
      {
        if(View!=null)
          View.CurrentLayer = this.m_CurrentLayerNumber;
      }

      // even if the active layer number not changed, it can be that the layer itself has changed from
      // one to another, so make sure that the current plot number is valid also
      EnsureValidityOfCurrentPlotNumber();

      // make sure the view knows about when the number of layers changed
      if(View!=null)
        View.NumberOfLayers = m_Graph.Layers.Count;
    }


    /// <summary>
    /// Called if something in the <see cref="GraphDocument"/> changed.
    /// </summary>
    /// <param name="sender">Not used (always the GraphDocument).</param>
    /// <param name="e">The EventArgs.</param>
    protected void EhGraph_Changed(object sender, System.EventArgs e)
    {
     

      // if something changed on the graph, make sure that the layer and plot number reflect this changed
      this.EnsureValidityOfCurrentLayerNumber();
      this.EnsureValidityOfCurrentPlotNumber();
      
      RefreshGraph();
    }

    /// <summary>
    /// Handler of the event LayerCollectionChanged of the graph document. Forces to
    /// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
    /// <param name="sender">The sender of the event (the GraphDocument).</param>
    /// <param name="e">The event arguments.</param>
    protected void EhGraph_BoundsChanged(object sender, System.EventArgs e)
    {
      this.m_FrozenGraphIsDirty = true;

      if(View!=null)
      {
        if(this.AutoZoom)
          this.RefreshAutoZoom();
        View.InvalidateGraph();
      }    
    }

    public void EhGraphDocumentNameChanged(object sender, Main.NameChangedEventArgs e)
    {
      if (View != null)
        View.GraphViewTitle = Doc.Name;

      this.TitleName = Doc.Name;
    }

    /// <summary>
    /// This is the whole name of the content, e.g. the file name or
    /// the url depending on the type of the content.
    /// </summary>
    public string TitleName
    {
      get
      {
        return this.Doc.Name;
      }
      set
      {
        OnTitleNameChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Is called each time the name for the content has changed.
    /// </summary>
    public event EventHandler TitleNameChanged;

    protected virtual void OnTitleNameChanged(System.EventArgs e)
    {
      if (null != TitleNameChanged)
        TitleNameChanged(this, e);
    }

    #endregion // GraphDocument event handlers

    #region Other event handlers
    
    /// <summary>
    /// Called from the system to print out a page.
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="ppea">PrintPageEventArgs used to retrieve the graphic context for printing.</param>
    public void EhPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs ppea)
    {
      Graphics g = ppea.Graphics;
      float hx = ppea.PageSettings.HardMarginX; // in hundreths of inch
      float hy = ppea.PageSettings.HardMarginY; // in hundreths of inch
      g.PageUnit = GraphicsUnit.Point;
      g.TranslateTransform(-hx*72/100.0f,-hy*72/100.0f);
      DoPaint(g,true);
    }

    

    /// <summary>
    /// This is called if the host window is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EhParentWindowSelected(object sender, EventArgs e)
    {
      if(View!=null)
        View.OnViewSelection();
    }

    /// <summary>
    /// This is called if the host window is deselected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EhParentWindowDeselected(object sender, EventArgs e)
    {
      if(View!=null)
        View.OnViewDeselection();
    }

 
    /// <summary>
    /// Handles the double click event onto a plot item.
    /// </summary>
    /// <param name="hit">Object containing information about the double clicked object.</param>
    /// <returns>True if the object should be deleted, false otherwise.</returns>
    protected static bool EhEditPlotItem(IHitTestObject hit)
    {
      XYPlotLayer actLayer = hit.ParentLayer;
      IGPlotItem pa = (IGPlotItem)hit.HittedObject;


      // get plot group
      PlotGroupStyleCollection plotGroup = pa.ParentCollection.GroupStyles;

      Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()),true);

      return false;
    }

    /// <summary>
    /// Handles the double click event onto a plot item.
    /// </summary>
    /// <param name="hit">Object containing information about the double clicked object.</param>
    /// <returns>True if the object should be deleted, false otherwise.</returns>
    protected static bool EhEditTextGraphics(IHitTestObject hit)
    {
      XYPlotLayer layer = hit.ParentLayer;
      TextGraphic tg = (TextGraphic)hit.HittedObject;

      bool shouldDeleted = false;

      object tgoo = tg;
      if (Current.Gui.ShowDialog(ref tgoo, "Edit text", true))
      {
        tg = (TextGraphic)tgoo;
        if (tg == null || tg.Empty)
        {
          if (null != hit.Remove)
            shouldDeleted = hit.Remove(hit);
          else
            shouldDeleted = false;
        }
        else
        {
          if (layer.ParentLayerList != null)
            layer.ParentLayerList.EhChildChanged(layer, EventArgs.Empty);
        }
      }
      /*
      TextControlDialog dlg = new TextControlDialog(layer,tg);
      if(DialogResult.OK==dlg.ShowDialog(Current.MainWindow))
      {
        if(!dlg.SimpleTextGraphics.Empty)
        {
          tg.CopyFrom(dlg.SimpleTextGraphics);
        }
        else // item is empty, so must be deleted in the layer and in the selectedObjects
        {
          if(null!=hit.Remove)
            shouldDeleted = hit.Remove(hit);
          else
            shouldDeleted = false;
        }
        // note the chante in the text graphics object
        if(layer.ParentLayerList!=null)
          layer.ParentLayerList.EhChildChanged(layer,EventArgs.Empty);
      }
      */


      return shouldDeleted;
    }

    #endregion

    #region Methods


   
    /// <summary>
    /// Saves the graph as an enhanced windows metafile into the stream <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <returns>Null if successfull, error description otherwise.</returns>
    public string SaveAsMetafile(System.IO.Stream stream)
    {
      try
      {
        Altaxo.Graph.Procedures.Export.SaveAsMetafile(this.Doc, stream, 300);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
      return null;
    }

    /// <summary>
    /// Saves the graph as an tiff file into the stream <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="imageFormat">The format of the destination image.</param>
    /// <returns>Null if successfull, error description otherwise.</returns>
    public string SaveAsBitmap(System.IO.Stream stream, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat)
    {
      Graph.Procedures.Export.SaveAsBitmap(m_Graph, stream, dpiResolution, imageFormat);
      return null;
    }

    private void DoPaint(Graphics g, bool bForPrinting)
    {
      if(bForPrinting)
      {
        DoPaintUnbuffered(g,bForPrinting);
      }
      else
      {

        if(m_FrozenGraph==null || m_FrozenGraph.Width!=m_View.GraphSize.Width || m_FrozenGraph.Height!=m_View.GraphSize.Height)
        {
          if(m_FrozenGraph!=null)
          {
            m_FrozenGraph.Dispose();
            m_FrozenGraph = null;
          }
        
          // create a frozen bitmap of the graph
          // using(Graphics g = m_View.CreateGraphGraphics())
          
          m_FrozenGraph = new Bitmap(m_View.GraphSize.Width,m_View.GraphSize.Height,g);
          m_FrozenGraphIsDirty = true;
        }

        if(m_FrozenGraph==null)
        {
          DoPaintUnbuffered(g,bForPrinting);
        }
        else if(m_FrozenGraphIsDirty)
        {
          using(Graphics gbmp = Graphics.FromImage(m_FrozenGraph))
          {
            DoPaintUnbuffered(gbmp,false);
            m_FrozenGraphIsDirty=false;
          }
         
          g.DrawImageUnscaled(m_FrozenGraph,0,0,m_View.GraphSize.Width,m_View.GraphSize.Height);
          ScaleForPaint(g,bForPrinting);
        }
        else
        {
          g.DrawImageUnscaled(m_FrozenGraph,0,0,m_View.GraphSize.Width,m_View.GraphSize.Height);
          ScaleForPaint(g,bForPrinting); // to be in the same state as when drawing unbuffered
        }
         
        // special painting depending on current selected tool
        this.m_MouseState.AfterPaint(g);
      }
    }

    /// <summary>
    /// This functions scales the graphics context to be ready for painting.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="bForPrinting">Indicates if the contexts is to be scaled
    /// for printing purposed (true) or for painting to the screen (false).</param>
    private void ScaleForPaint(Graphics g, bool bForPrinting)
    {
      // g.SmoothingMode = SmoothingMode.AntiAlias;
      // get the dpi settings of the graphics context,
      // for example; 96dpi on screen, 600dpi for the printer
      // used to adjust grid and margin sizing.
      this.m_HorizRes = g.DpiX;
      this.m_VertRes = g.DpiY;

      g.PageUnit = GraphicsUnit.Point;

      if (bForPrinting)
      {
        g.PageScale = 1;
      }
      else
      {
        g.PageScale = this.m_Zoom;
        float pointsh = UnitPerInch * m_View.GraphScrollPosition.X / (this.m_HorizRes * this.m_Zoom);
        float pointsv = UnitPerInch * m_View.GraphScrollPosition.Y / (this.m_VertRes * this.m_Zoom);
        g.TranslateTransform(pointsh, pointsv);
      }
    }

    /// <summary>
    /// Central routine for painting the graph. The painting can either be on the screen (bForPrinting=false), or
    /// on a printer or file (bForPrinting=true).
    /// </summary>
    /// <param name="g">The graphics context painting to.</param>
    /// <param name="bForPrinting">If true, margins and background are not painted, as is usefull for printing.
    /// Also, if true, the scale is temporarely set to 1.</param>
    private void DoPaintUnbuffered(Graphics g, bool bForPrinting)
    {
      try
      {
        ScaleForPaint(g,bForPrinting);

        if(!bForPrinting)
        {
          g.Clear(this.m_NonPageAreaColor);
          // Fill the page with its own color
          g.FillRectangle(m_PageGroundBrush,m_Graph.PageBounds);
          g.FillRectangle(m_PrintableAreaBrush,m_Graph.PrintableBounds);
          // DrawMargins(g);
        }

        System.Console.WriteLine("Paint with zoom {0}",this.m_Zoom);
        // handle the possibility that the viewport is scrolled,
        // adjust my origin coordintates to compensate
        Point pt = m_View.GraphScrollPosition;
        

        // Paint the graph now
        g.TranslateTransform(m_Graph.PrintableBounds.X,m_Graph.PrintableBounds.Y); // translate the painting to the printable area
        m_Graph.DoPaint(g,bForPrinting);

       

      
      }
      catch(System.Exception ex)
      {
        g.PageUnit = GraphicsUnit.Point;
        g.PageScale=1;

        // System.Windows.Forms.MessageBox.Show(this.m_View.Window,ex.ToString());
      
        g.DrawString(ex.ToString(),
          new System.Drawing.Font("Arial",10),
          System.Drawing.Brushes.Black,
          m_Graph.PrintableBounds);

      
      }

    }


    #endregion // Methods

    #region Properties
    public event EventHandler CurrentGraphToolChanged;


    /// <summary>
    /// Get/sets the currently active GraphTool.
    /// </summary>
    public System.Type CurrentGraphToolType
    {
      get 
      {
        return m_MouseState.GetType();
      }
      set
      {
        
        if(m_MouseState==null || m_MouseState.GetType() != value)
        {
          m_MouseState = (MouseStateHandler)System.Activator.CreateInstance(value,new object[]{this});
        
          if(CurrentGraphToolChanged!=null)
            CurrentGraphToolChanged(this,EventArgs.Empty);
        }
      }
    }

   

    /// <summary>
    /// Returns the layer collection. Is the same as m_GraphDocument.XYPlotLayer.
    /// </summary>
    public XYPlotLayerCollection Layers
    {
      get { return m_Graph.Layers; }
    }


    /// <summary>
    /// Returns the currently active layer, or null if there is no active layer.
    /// </summary>
    public XYPlotLayer ActiveLayer
    {
      get
      {
        return this.m_CurrentLayerNumber<0 ? null : m_Graph.Layers[this.m_CurrentLayerNumber]; 
      }     
    }

    /// <summary>
    /// check the validity of the CurrentLayerNumber and correct it
    /// </summary>
    public void EnsureValidityOfCurrentLayerNumber()
    {
      if(m_Graph.Layers.Count>0) // if at least one layer is present
      {
        if(m_CurrentLayerNumber<0)
          CurrentLayerNumber=0;
        else if(m_CurrentLayerNumber>=m_Graph.Layers.Count)
          CurrentLayerNumber=m_Graph.Layers.Count-1;
      }
      else // no layers present
      {
        if(-1!=m_CurrentLayerNumber)
          CurrentLayerNumber=-1;
      }
    }

    /// <summary>
    /// Get / sets the currently active layer by number.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CurrentLayerNumber
    {
      get
      {
        EnsureValidityOfCurrentLayerNumber();
        return m_CurrentLayerNumber;
      }
      set
      {
        int oldValue = this.m_CurrentLayerNumber;

        // negative values are only accepted if there is no layer
        if(value<0 && m_Graph.Layers.Count>0)
          throw new ArgumentOutOfRangeException("CurrentLayerNumber",value,"Accepted values must be >=0 if there is at least one layer in the graph!");

        if(value>=m_Graph.Layers.Count)
          throw new ArgumentOutOfRangeException("CurrentLayerNumber",value,"Accepted values must be less than the number of layers in the graph(currently " + m_Graph.Layers.Count.ToString() + ")!");

        m_CurrentLayerNumber = value<0 ? -1 : value;

        // if something changed
        if(oldValue!=this.m_CurrentLayerNumber)
        {
          // reflect the change in layer number in the layer tool bar
          if(null!=View)
            View.CurrentLayer = this.m_CurrentLayerNumber;

          // since the layer changed, also the plots changed, so the menu has
          // to reflect the new plots
          this.UpdateDataPopup();
        }
      }
    }


    /// <summary>
    /// This ensures that the current plot number is valid. If there is no plot on the currently active layer,
    /// the current plot number is set to -1.
    /// </summary>
    public void EnsureValidityOfCurrentPlotNumber()
    {
      EnsureValidityOfCurrentLayerNumber();

      // if XYPlotLayer don't exist anymore, correct CurrentLayerNumber and ActualPlotAssocitation
      if(null!=ActiveLayer) // if the ActiveLayer exists
      {
        // if the XYColumnPlotData don't exist anymore, correct it
        if(ActiveLayer.PlotItems.Flattened.Length>0) // if at least one plotitem exists
        {
          if(m_CurrentPlotNumber<0)
            CurrentPlotNumber=0;
          else if(m_CurrentPlotNumber>ActiveLayer.PlotItems.Flattened.Length)
            CurrentPlotNumber = 0;
        }
        else
        {
          if(-1!=m_CurrentPlotNumber)
            CurrentPlotNumber=-1;
        }
      }
      else // if no layer anymore
      {
        if(-1!=m_CurrentPlotNumber)
          CurrentPlotNumber=-1;
      }
    }

    /// <summary>
    /// Get / sets the currently active plot by number.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CurrentPlotNumber 
    {
      get 
      {
        return m_CurrentPlotNumber;
      }
      set
      {
        if(CurrentLayerNumber>=0 && 0!=this.m_Graph.Layers[CurrentLayerNumber].PlotItems.Flattened.Length && value<0)
          throw new ArgumentOutOfRangeException("CurrentPlotNumber",value,"CurrentPlotNumber has to be greater or equal than zero");

        if(CurrentLayerNumber>=0 && value>=m_Graph.Layers[CurrentLayerNumber].PlotItems.Flattened.Length)
          throw new ArgumentOutOfRangeException("CurrentPlotNumber",value,"CurrentPlotNumber has to  be lesser than actual count: " + m_Graph.Layers[CurrentLayerNumber].PlotItems.Flattened.Length.ToString());

        m_CurrentPlotNumber = value<0 ? -1 : value;

        this.UpdateDataPopup();
      }
    }


    #endregion // Properties

    #region Editing selected objects


    /// <summary>
    /// Returns the number of selected objects into this graph.
    /// </summary>
    public int NumberOfSelectedObjects
    {
      get
      {
        if (m_MouseState is ObjectPointerMouseHandler)
          return ((ObjectPointerMouseHandler)m_MouseState).NumberOfSelectedObjects;
        else
          return 0;
      }
    }

    /// <summary>
    /// Remove all selected objects of this graph.
    /// </summary>
    public void RemoveSelectedObjects()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)m_MouseState).RemoveSelectedObjects();
    }

    /// <summary>
    /// Copy the selected objects of this graph to the clipboard.
    /// </summary>
    public void CopySelectedObjectsToClipboard()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)m_MouseState).CopySelectedObjectsToClipboard();
    }

    /// <summary>
    /// Copy the selected objects of this graph to the clipboard.
    /// </summary>
    public void CutSelectedObjectsToClipboard()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)m_MouseState).CutSelectedObjectsToClipboard();
    }

    public void PasteObjectsFromClipboard()
    {
      GraphDocument gd = this.Doc;
      System.Windows.Forms.DataObject dao = System.Windows.Forms.Clipboard.GetDataObject() as System.Windows.Forms.DataObject;

      string[] formats = dao.GetFormats();
      System.Diagnostics.Trace.WriteLine("Available formats:");

      if (dao.GetDataPresent("Altaxo.Graph.GraphObjectList"))
      {
        object obj = dao.GetData("Altaxo.Graph.GraphObjectList");

        // if at this point obj is a memory stream, you probably have forgotten the deserialization constructor of the class you expect to deserialize here
        if (obj is ArrayList)
        {
          ArrayList list = (ArrayList)obj;
          foreach (object item in list)
          {
            if(item is GraphicBase)
              this.ActiveLayer.GraphObjects.Add(item as GraphicBase);
          }
        }
        return;
      }
      if (dao.ContainsFileDropList())
      {
        bool bSuccess = false;
        System.Collections.Specialized.StringCollection coll = dao.GetFileDropList();
        foreach (string filename in coll)
        {
          ImageProxy img;
          try
          {
            img = ImageProxy.FromFile(filename);
            if (img != null)
            {
              SizeF size = this.ActiveLayer.Size;
              size.Width /= 2;
              size.Height /= 2;
              EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointF.Empty, size, img);
              this.ActiveLayer.GraphObjects.Add(item);
              bSuccess = true;
              continue;
            }
          }
          catch (Exception)
          {
          }
        }
        if (bSuccess)
          return;
      }
    
      if (dao.GetDataPresent(typeof(System.Drawing.Imaging.Metafile)))
      {
        System.Drawing.Imaging.Metafile img = dao.GetData(typeof(System.Drawing.Imaging.Metafile)) as System.Drawing.Imaging.Metafile;
        if (img != null)
        {
          SizeF size = this.ActiveLayer.Size;
          size.Width /= 2;
          size.Height /= 2;
          EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointF.Empty, size, ImageProxy.FromImage(img));
          this.ActiveLayer.GraphObjects.Add(item);
          return;
        }
      }
      if (dao.ContainsImage())
      {
        Image img = dao.GetImage();
        if (img != null)
        {
          SizeF size = this.ActiveLayer.Size;
          size.Width /= 2;
          size.Height /= 2;
          EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointF.Empty, size, ImageProxy.FromImage(img));
          this.ActiveLayer.GraphObjects.Add(item);
          return;
        }
      }
    }

    #endregion

    #region Arrangement of selected objects

    /// <summary>
    /// Arranges the objects so they share the bottom boundary of the last selected object.
    /// </summary>
    public void ArrangeBottom()
    {
        if (m_MouseState is ObjectPointerMouseHandler)
          ((ObjectPointerMouseHandler)m_MouseState).ArrangeBottomToBottom();
    }

    /// <summary>
    /// Arranges the objects so they share the top boundary of the last selected object.
    /// </summary>
    public void ArrangeTop()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)m_MouseState).ArrangeTopToTop();
    }

    /// <summary>
    /// Arranges the objects so they share the left boundary of the last selected object.
    /// </summary>
    public void ArrangeLeft()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)m_MouseState).ArrangeLeftToLeft();
    }

    /// <summary>
    /// Arranges the objects so they share the right boundary of the last selected object.
    /// </summary>
    public void ArrangeRight()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)m_MouseState).ArrangeRightToRight();
    }

    /// <summary>
    /// Arranges the objects so they share the vertical middle line of the last selected object.
    /// </summary>
    public void ArrangeHorizontal()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)m_MouseState).ArrangeHorizontal();
    }


    /// <summary>
    /// Arranges the objects so they share the horizontal middle line of the last selected object.
    /// </summary>
    public void ArrangeVertical()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)m_MouseState).ArrangeVertical();
    }

    /// <summary>
    /// Arranges the objects so they their vertical middle line is uniform spaced between the first and the last selected object.
    /// </summary>
    public void ArrangeHorizontalTable()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)m_MouseState).ArrangeHorizontalTable();
    }

    /// <summary>
    /// Arranges the objects so they their horizontal middle line is uniform spaced between the first and the last selected object.
    /// </summary>
    public void ArrangeVerticalTable()
    {
      if (m_MouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)m_MouseState).ArrangeVerticalTable();
    }


    #endregion

    #region Scaling and Positioning

    /// <summary>
    /// Zoom value of the graph view.
    /// </summary>
    public float Zoom
    {
      get
      {
        return m_Zoom;
      }
      set
      {
        if( value > 0.05 )
          m_Zoom = value;
        else
          m_Zoom = 0.05f;
      } 
    }

    /// <summary>
    /// Enables / disable the autozoom feature.
    /// </summary>
    /// <remarks>If autozoom is enables, the zoom factor is calculated depending on the size of the
    /// graph view so that the graph fits best possible inside the view.</remarks>
    public bool AutoZoom
    {
      get
      {
        return this.m_AutoZoom;
      }
      set
      {
        this.m_AutoZoom = value;
        if(this.m_AutoZoom)
        {
          RefreshAutoZoom();
        }
      }
    }


    /// <summary>
    /// Does a complete new drawing of the graph, even if the graph is cached in a bitmap.
    /// </summary>
    public void RefreshGraph()
    {
      this.m_FrozenGraphIsDirty = true;
      
      if(null!=View) 
        m_View.InvalidateGraph();
    }

    /// <summary>
    /// If the graph is cached, this causes an immediate redraw of the client area using the cached bitmap.
    /// If not cached, this simply invalidates the client area.
    /// </summary>
    public void RepaintGraphArea()
    {
      if(View==null)
        return;

      if(this.m_FrozenGraph != null && !this.m_FrozenGraphIsDirty)
      {
        using(Graphics g = this.View.CreateGraphGraphics())
        {
          this.DoPaint(g,false);
        }
      }
      else
      {
        this.View.InvalidateGraph();
      }
        
    }

    /// <summary>
    /// Recalculates and sets the value of m_Zoom so the whole page is visible
    /// </summary>
    protected void RefreshAutoZoom()
    {
      this.m_Zoom = CalculateAutoZoom();
      m_View.GraphScrollSize = new Size(0,0);
      RefreshGraph();
    }

    /// <summary>
    /// Factor for horizontal conversion of page units (points=1/72 inch) to pixel.
    /// The resolution used for this is <see cref="m_HorizRes"/>.
    /// </summary>
    /// <returns>The factor described above.</returns>
    public float HorizFactorPageToPixel()
    {
      return this.m_HorizRes*this.m_Zoom/UnitPerInch;
    }

    /// <summary>
    /// Factor for vertical conversion of page units (points=1/72 inch) to pixel.
    /// The resolution used for this is <see cref="m_VertRes"/>.
    /// </summary>
    /// <returns>The factor described above.</returns>
    public float VertFactorPageToPixel()
    {
      return this.m_VertRes*this.m_Zoom/UnitPerInch;
    }

    /// <summary>
    /// Converts page coordinates (in points=1/72 inch) to pixel units. Uses the resolutions <see cref="m_HorizRes"/>
    /// and <see cref="m_VertRes"/> for calculation-
    /// </summary>
    /// <param name="pagec">The page coordinates to convert.</param>
    /// <returns>The coordinates as pixel coordinates.</returns>
    public PointF PageCoordinatesToPixel(PointF pagec)
    {
      return new PointF(pagec.X*HorizFactorPageToPixel(),pagec.Y*VertFactorPageToPixel());
    }

    /// <summary>
    /// Converts pixel coordinates to page coordinates (in points=1/72 inch). Uses the resolutions <see cref="m_HorizRes"/>
    /// and <see cref="m_VertRes"/> for calculation-
    /// </summary>
    /// <param name="pixelc">The pixel coordinates to convert.</param>
    /// <returns>The coordinates as page coordinates (points=1/72 inch).</returns>
    public PointF PixelToPageCoordinates(PointF pixelc)
    {
      return new PointF(pixelc.X/HorizFactorPageToPixel(),pixelc.Y/VertFactorPageToPixel());
    }

    /// <summary>
    /// Converts page coordinates (in points=1/72 inch) to pixel coordinates . Uses the resolutions <see cref="m_HorizRes"/>
    /// and <see cref="m_VertRes"/> for calculation-
    /// </summary>
    /// <param name="pagec">The page coordinates to convert (points=1/72 inch).</param>
    /// <returns>The coordinates as pixel coordinates.</returns>
    public PointF PageToPixelCoordinates(PointF pagec)
    {
      return new PointF(pagec.X*HorizFactorPageToPixel(),pagec.Y*VertFactorPageToPixel());
    }

    /// <summary>
    /// Converts x,y differences in pixels to the corresponding
    /// differences in page coordinates
    /// </summary>
    /// <param name="pixeldiff">X,Y differences in pixel units</param>
    /// <returns>X,Y differences in page coordinates</returns>
    public PointF PixelToPageDifferences(PointF pixeldiff)
    {
      return new PointF(pixeldiff.X/HorizFactorPageToPixel(),pixeldiff.Y/VertFactorPageToPixel());
    }

    /// <summary>
    /// converts from pixel to printable area coordinates
    /// </summary>
    /// <param name="pixelc">pixel coordinates as returned by MouseEvents</param>
    /// <returns>coordinates of the printable area in 1/72 inch</returns>
    public PointF PixelToPrintableAreaCoordinates(PointF pixelc)
    {
      PointF r = PixelToPageCoordinates(pixelc);
      r.X -= m_Graph.PrintableBounds.X;
      r.Y -= m_Graph.PrintableBounds.Y;
      return r;
    }

    /// <summary>
    /// converts printable area  to pixel coordinates
    /// </summary>
    /// <param name="printc">Printable area coordinates.</param>
    /// <returns>Pixel coordinates as returned by MouseEvents</returns>
    public PointF PrintableAreaToPixelCoordinates(PointF printc)
    {
      printc.X += m_Graph.PrintableBounds.X;
      printc.Y += m_Graph.PrintableBounds.Y;
      return PageToPixelCoordinates(printc);
     
      
    }

    /// <summary>
    /// This calculates the zoom factor using the size of the graph view, so that the page fits
    /// best into the view.
    /// </summary>
    /// <returns>The calculated zoom factor.</returns>
    protected virtual float CalculateAutoZoom()
    {
      float zoomh = (UnitPerInch*m_View.GraphSize.Width/this.m_HorizRes)/m_Graph.PageBounds.Width;
      float zoomv = (UnitPerInch*m_View.GraphSize.Height/this.m_VertRes)/m_Graph.PageBounds.Height;
      return System.Math.Min(zoomh,zoomv);

    }
    /// <summary>
    /// Translates the graphics properties to graph (printable area), so that the beginning of printable area now is (0,0) and the units are in Points (1/72 inch)
    /// </summary>
    /// <param name="g">Graphics to translate</param>
    void TranslateGraphicsToGraphUnits(Graphics g)
    {
      g.PageUnit = GraphicsUnit.Point;
      g.PageScale = this.m_Zoom;

      // the graphics path was returned in printable area ("graph") coordinates
      // thats why we have to shift our coordinate system to printable area coordinates also
      float pointsh = UnitPerInch*m_View.GraphScrollPosition.X/(this.m_HorizRes*this.m_Zoom);
      float pointsv = UnitPerInch*m_View.GraphScrollPosition.Y/(this.m_VertRes*this.m_Zoom);
      pointsh += m_Graph.PrintableBounds.X;
      pointsv += m_Graph.PrintableBounds.Y; 

      // shift the coordinates to page coordinates
      g.TranslateTransform(pointsh,pointsv);
    }

    #endregion // Scaling, Converting


    
  
 

    /// <summary>
    /// Looks for a graph object at pixel position <paramref name="pixelPos"/> and returns true if one is found.
    /// </summary>
    /// <param name="pixelPos">The pixel coordinates (graph panel coordinates)</param>
    /// <param name="plotItemsOnly">If true, only the plot items where hit tested.</param>
    /// <param name="foundObject">Found object if there is one found, else null</param>
    /// <param name="foundInLayerNumber">The layer the found object belongs to, otherwise 0</param>
    /// <returns>True if a object was found at the pixel coordinates <paramref name="pixelPos"/>, else false.</returns>
    public bool FindGraphObjectAtPixelPosition(PointF pixelPos, bool plotItemsOnly, out IHitTestObject foundObject, out int foundInLayerNumber)
    {
      // search for a object first
      PointF mousePT = PixelToPrintableAreaCoordinates(pixelPos);

      for(int nLayer=0;nLayer<Layers.Count;nLayer++)
      {
        XYPlotLayer layer = Layers[nLayer];
        foundObject = layer.HitTest(mousePT, plotItemsOnly);
        if(null!=foundObject)
        {
          foundInLayerNumber = nLayer;
          return true;
        }
      }
      foundObject=null;
      foundInLayerNumber=0;
      return false;
    }

    /// <summary>
    /// Draws immediately a selection rectangle around the object <paramref name="graphObject"/>.
    /// </summary>
    /// <param name="graphObject">The graph object for which a selection rectangle has to be drawn.</param>
    /// <param name="nLayer">The layer number the <paramref name="graphObject"/> belongs to.</param>
    public void DrawSelectionRectangleImmediately(IHitTestObject graphObject, int nLayer)
    {
      using(Graphics g = m_View.CreateGraphGraphics())
      {
        // now translate the graphics to graph units and paint all selection path
        this.TranslateGraphicsToGraphUnits(g);
        //        g.DrawPath(Pens.Blue,Layers[nLayer].LayerToGraphCoordinates(graphObject.SelectionPath)); // draw the selection path
        g.DrawPath(Pens.Blue,graphObject.SelectionPath); // draw the selection path
      }   
    }

    #region IWorkbenchContentController Members


    public void CloseView()
    {
      this.View = null;
    }

    public void CreateView()
    {
      if(View==null)
      {
        View = new GraphView();
      }
    }


    #endregion

    #region IMVCController
    /// <summary>
    /// Returns the view that shows the model.
    /// </summary>
    public object ViewObject
    {
      get { return View; }
      set { View = value as IGraphView; }
    }
    /// <summary>
    /// Returns the model (document) that this controller controls
    /// </summary>
    public object ModelObject 
    {
      get { return this.Doc; }
    }

    #endregion

    #region Inner Classes

    /// <summary>
    /// Used as menu items in the Data menu popup. Stores the plot number the menu item represents.
    /// </summary>
    public class DataMenuItem : MenuItem
    {
      /// <summary>The plot number this menu item represents.</summary>
      public int PlotItemNumber=0;

      /// <summary>Creates the default menu item (PlotItemNumber is 0).</summary>
      public DataMenuItem() {}

      /// <summary>Creates a menuitem with text and a handler.</summary>
      /// <param name="t">The text the menuitem shows.</param>
      /// <param name="e">The handler in case the menu item is clicked.</param>
      public DataMenuItem(string t, EventHandler e) : base(t,e) {}
    }

    #endregion // Inner Classes

  }
}
