using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Graph.Graph2D;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph.Graph2D
{
	public interface IResizeGraphView
	{
		void SetReportOfOldValues(string report);

		void SetReportOfDerivedValues(string report);

		void SetOldRootLayerSize(PointD2D size);

		void SetOldStandardFont(string font);

		void SetOldStandardFontSize(double size);

		void SetOldStandardLineThickness(double thickness);

		void SetOldStandardMajorTickLength(double value);

		bool IsNewRootLayerSizeChosen { get; }
		PointD2D NewRootLayerSize { get; }
		bool IsNewStandardFontFamilyChosen { get; }
		string NewStandardFontFamily { get; }

		bool IsResetAllFontsToStandardFontFamilyChosen { get; }

		bool IsNewStandardFontSizeChosen { get; }
		double NewStandardFontSize { get; }

		SelectableListNodeList ActionsForFontSize { set; }

		SelectableListNodeList ActionsForSymbolSize { set; }

		SelectableListNodeList ActionsForLineThickness { set; }
		bool IsUserDefinedLineThicknessChosen { get; }
		double UserDefinedLineThickness { get; }

		SelectableListNodeList ActionsForTickLength { set; }
		bool IsUserDefinedMajorTickLengthChosen { get; }
		double UserDefinedMajorTickLength { get; }

		/// <summary>
		/// Occurs when either chosen font family or size changed.
		/// </summary>
		event Action FontChanged;
	}

	[ExpectedTypeOfView(typeof(IResizeGraphView))]
	[UserControllerForObject(typeof(ResizeGraphOptions))]
	public class ResizeGraphController : MVCANControllerEditOriginalDocBase<ResizeGraphOptions, IResizeGraphView>
	{
		private SelectableListNodeList _actionsForFontSize;
		private SelectableListNodeList _actionsForSymbolSize;
		private SelectableListNodeList _actionsForLineThickness;
		private SelectableListNodeList _actionsForTickLength;

		private string _reportOfOldValues;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_actionsForFontSize = new SelectableListNodeList(_doc.ActionForFontSize);
				_actionsForSymbolSize = new SelectableListNodeList(_doc.ActionForSymbolSize);
				_actionsForLineThickness = new SelectableListNodeList(_doc.ActionForLineThickness);
				_actionsForTickLength = new SelectableListNodeList(_doc.ActionForTickLength);

				var cult = Altaxo.Settings.GuiCulture.Instance;
				var stb = new StringBuilder();
				stb.AppendFormat(cult, "Root layer size: ({0} pt x {1} pt) = ({2} mm x {3} mm)", _doc.OldRootLayerSize.X, _doc.OldRootLayerSize.Y, _doc.OldRootLayerSize.X * 25.4 / 72, _doc.OldRootLayerSize.Y * 25.4 / 72);
				stb.AppendLine();
				stb.AppendFormat(cult, "Standard font family: {0}", string.IsNullOrEmpty(_doc.OldStandardFontFamily) ? "<not set>" : _doc.OldStandardFontFamily);
				stb.AppendLine();
				stb.AppendFormat(cult, "Standard font size: {0}", _doc.OldStandardFontSize.HasValue ? _doc.OldStandardFontSize.Value.ToString(cult) + " pt" : "<not set>");
				stb.AppendLine();
				stb.AppendFormat(cult, "Standard line width: {0}", _doc.OldLineThickness.HasValue ? _doc.OldLineThickness.Value.ToString(cult) + " pt" : "<not set>");
				_reportOfOldValues = stb.ToString();
			}

			if (null != _view)
			{
				_view.SetReportOfOldValues(_reportOfOldValues);
				_view.SetOldRootLayerSize(_doc.OldRootLayerSize);
				_view.SetOldStandardFont(_doc.OldStandardFontFamily);
				if (_doc.OldStandardFontSize.HasValue)
					_view.SetOldStandardFontSize(_doc.OldStandardFontSize.Value);
				if (_doc.OldMajorTickLength.HasValue)
					_view.SetOldStandardMajorTickLength(_doc.OldMajorTickLength.Value);

				if (_doc.OldLineThickness.HasValue)
					_view.SetOldStandardLineThickness(_doc.OldLineThickness.Value);

				_view.ActionsForFontSize = _actionsForFontSize;
				_view.ActionsForSymbolSize = _actionsForSymbolSize;
				_view.ActionsForLineThickness = _actionsForLineThickness;
				_view.ActionsForTickLength = _actionsForTickLength;
			}
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.FontChanged += EhFontChanged;
		}

		protected override void DetachView()
		{
			_view.FontChanged -= EhFontChanged;
			base.DetachView();
		}

		private void EhFontChanged()
		{
			var fontFamily = _view.IsNewStandardFontFamilyChosen ? _view.NewStandardFontFamily : _doc.OldStandardFontFamily;
			var fontSize = _view.IsNewStandardFontSizeChosen ? _view.NewStandardFontSize : _doc.OldStandardFontSize ?? 12;
			var font = Altaxo.Graph.Gdi.GdiFontManager.GetFontX(fontFamily, fontSize, Altaxo.Drawing.FontXStyle.Regular);

			var bag = new Altaxo.Main.Properties.PropertyBag();
			bag.SetValue(Altaxo.Graph.Gdi.GraphDocument.PropertyKeyDefaultFont, font);
			var newLineWidth = Altaxo.Graph.Gdi.GraphDocument.GetDefaultPenWidth(bag);
			var newMajorTickLength = Altaxo.Graph.Gdi.GraphDocument.GetDefaultMajorTickLength(bag);

			var cult = Altaxo.Settings.GuiCulture.Instance;
			var stb = new StringBuilder();
			stb.AppendFormat(cult, "Standard font family: {0}", fontFamily);
			stb.AppendLine();
			stb.AppendFormat(cult, "Standard font size: {0} pt", fontSize);
			stb.AppendLine();
			stb.AppendFormat(cult, "Derived line width: {0} pt", newLineWidth);
			stb.AppendLine();
			stb.AppendFormat(cult, "Derived major tick length: {0} pt", newMajorTickLength);
			_view.SetReportOfDerivedValues(stb.ToString());

			if (!_view.IsUserDefinedLineThicknessChosen)
				_view.SetOldStandardLineThickness(newLineWidth);

			if (!_view.IsUserDefinedMajorTickLengthChosen)
				_view.SetOldStandardMajorTickLength(newMajorTickLength);
		}

		public override bool Apply(bool disposeController)
		{
			if (_view.IsNewRootLayerSizeChosen)
				_doc.NewRootLayerSize = _view.NewRootLayerSize;
			else
				_doc.NewRootLayerSize = null;

			if (_view.IsNewStandardFontFamilyChosen)
				_doc.NewStandardFontFamily = _view.NewStandardFontFamily;
			else
				_doc.NewStandardFontFamily = null;

			_doc.OptionResetAllFontsToStandardFont = _view.IsResetAllFontsToStandardFontFamilyChosen;

			if (_view.IsNewStandardFontSizeChosen)
				_doc.NewStandardFontSize = _view.NewStandardFontSize;
			else
				_doc.NewStandardFontSize = null;

			_doc.ActionForFontSize = (ResizeGraphOptions.ScalarSizeActions)_actionsForFontSize.FirstSelectedNode.Tag;

			_doc.ActionForSymbolSize = (ResizeGraphOptions.ScalarSizeActions)_actionsForSymbolSize.FirstSelectedNode.Tag;

			_doc.ActionForLineThickness = (ResizeGraphOptions.ScalarSizeActions)_actionsForLineThickness.FirstSelectedNode.Tag;

			if (_view.IsUserDefinedLineThicknessChosen)
				_doc.UserDefinedLineThickness = _view.UserDefinedLineThickness;
			else
				_doc.UserDefinedLineThickness = null;

			_doc.ActionForTickLength = (ResizeGraphOptions.ScalarSizeActions)_actionsForTickLength.FirstSelectedNode.Tag;

			if (_view.IsUserDefinedMajorTickLengthChosen)
				_doc.UserDefinedMajorTickLength = _view.UserDefinedMajorTickLength;
			else
				_doc.UserDefinedMajorTickLength = null;

			return ApplyEnd(true, disposeController);
		}

		public static ResizeGraphOptions _lastUsedInstance;

		/// <summary>
		/// Shows the resize graph dialog and if Ok, the graph is resized afterwards.
		/// </summary>
		/// <param name="doc">The graph document to resize.</param>
		public static void ShowResizeGraphDialog(Altaxo.Graph.Gdi.GraphDocument doc)
		{
			var resizeOptions = null == _lastUsedInstance ? new ResizeGraphOptions() : (ResizeGraphOptions)_lastUsedInstance.Clone();

			resizeOptions.InitializeOldValues(doc);

			var controller = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { resizeOptions }, typeof(IMVCANController));

			if (Current.Gui.ShowDialog(controller, "Resize graph"))
			{
				resizeOptions = (ResizeGraphOptions)controller.ModelObject;
				resizeOptions.ResizeGraph(doc);
				_lastUsedInstance = resizeOptions;
			}
		}

		/// <summary>
		/// Shows the resize graph dialog and if Ok, the graph is resized afterwards.
		/// </summary>
		/// <param name="docs">The graph documents to resize.</param>
		/// <returns>True if the graphs were resized; false otherwise.</returns>
		/// <remarks>The old values shown in the dialog are taken from the first graph in the enumeration.</remarks>
		public static bool ShowResizeGraphDialog(IEnumerable<Altaxo.Graph.Gdi.GraphDocument> docs)
		{
			var resizeOptions = null == _lastUsedInstance ? new ResizeGraphOptions() : (ResizeGraphOptions)_lastUsedInstance.Clone();

			var docEnum = docs.GetEnumerator();

			bool result = false;
			try
			{
				if (!docEnum.MoveNext())
					return result; // Enumeration is empty

				resizeOptions.InitializeOldValues(docEnum.Current);

				var controller = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { resizeOptions }, typeof(IMVCANController));

				if (Current.Gui.ShowDialog(controller, "Resize graph"))
				{
					resizeOptions = (ResizeGraphOptions)controller.ModelObject;

					do
					{
						resizeOptions.ResizeGraph(docEnum.Current);
					} while (docEnum.MoveNext());

					_lastUsedInstance = resizeOptions;
					result = true;
				}
			}
			finally
			{
				docEnum.Dispose();
			}
			return result;
		}
	}
}