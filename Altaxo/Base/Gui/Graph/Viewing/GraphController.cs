using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Viewing
{
	using Altaxo.Graph;
	using Altaxo.Graph.Gdi;

	public class GraphController : IGraphController
	{
		IGuiDependentGraphController _guiDependentController;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GUI.GraphController", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphController), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			Main.DocumentPath _PathToGraph;
			GraphController _GraphController;

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphController s = (GraphController)obj;
				info.AddValue("AutoZoom", s._guiDependentController.IsAutoZoomActive);
				info.AddValue("Zoom", s._guiDependentController.ZoomFactor);
				info.AddValue("Graph", Main.DocumentPath.GetAbsolutePath(s.Doc));
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				GraphController s = null != o ? (GraphController)o : new GraphController(null, true);
				s._guiDependentController.IsAutoZoomActive = info.GetBoolean("AutoZoom");
				s._guiDependentController.ZoomFactor = info.GetSingle("Zoom");

				XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
				surr._GraphController = s;
				surr._PathToGraph = (Main.DocumentPath)info.GetValue("Graph", s);
				info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

				return s;
			}

			private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
			{
				object o = Main.DocumentPath.GetObject(_PathToGraph, documentRoot, _GraphController);
				if (o is GraphDocument)
				{
					_GraphController._guiDependentController.InternalInitializeGraphDocument(o as GraphDocument);
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
				}
			}
		}

		#endregion


		#region Constructors

		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.    
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		public GraphController(GraphDocument graphdoc)
			: this(graphdoc, false)
		{
		}

		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		/// <param name="bDeserializationConstructor">If true, this is a special constructor used only for deserialization, where no graphdoc needs to be supplied.</param>
		public GraphController(GraphDocument graphdoc, bool bDeserializationConstructor)
		{
			Current.Gui.InstrumentControllerWithGuiDependentFunctions(this);
			if (null == _guiDependentController)
				throw new ApplicationException("Gui dependent worksheet controller was not set - it is null!");

			if (null != graphdoc)
				this._guiDependentController.InternalInitializeGraphDocument(graphdoc); // Using DataTable here wires the event chain also
			else if (!bDeserializationConstructor)
				throw new ArgumentNullException("Leaving the graphdoc null in constructor is not supported here");
		}

		/// <summary>
		/// This function is intended for internal purposes only. It sets the Gui dependent controller instance that will do all the work.
		/// </summary>
		/// <param name="guiController">The gui dependent controller.</param>
		public void InternalSetGuiController(IGuiDependentGraphController guiController)
		{
			if (null == guiController)
				throw new ArgumentNullException("guiController");

			_guiDependentController = guiController;
		}

		#endregion

		#region IGraphController Members

		public Altaxo.Graph.Gdi.GraphDocument Doc
		{
			get { return _guiDependentController.Doc; }
		}

		public Altaxo.Graph.Gdi.XYPlotLayer ActiveLayer
		{
			get { return _guiDependentController.ActiveLayer; }
		}

		public int CurrentPlotNumber
		{
			get
			{
				return _guiDependentController.CurrentPlotNumber;
			}
			set
			{
				_guiDependentController.CurrentPlotNumber = value;
			}
		}

		public void EnsureValidityOfCurrentLayerNumber()
		{
			_guiDependentController.EnsureValidityOfCurrentLayerNumber();
		}

		public void EnsureValidityOfCurrentPlotNumber()
		{
			_guiDependentController.EnsureValidityOfCurrentPlotNumber();
		}

		public void RefreshGraph()
		{
			_guiDependentController.RefreshGraph();
		}

		#endregion

		public IGuiDependentGraphController InternalGetGuiController()
		{
			return _guiDependentController;
		}
	}
}
