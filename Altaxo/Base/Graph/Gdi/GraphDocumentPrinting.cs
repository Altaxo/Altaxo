using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing.Printing;
using System.Drawing;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// Extension methods for GraphDocument especially for printing.
	/// </summary>
	public static class GraphDocumentPrinting
	{
		/// <summary>
		/// Shows the print dialog to choose the printer, and then prints the document.
		/// </summary>
		/// <param name="doc">The graph document to print out.</param>
		/// <returns>True if user closed the dialog with OK and the document was printed, false if user cancelled the dialog or document failed to print.</returns>
		public static bool ShowPrintDialogAndPrint(this GraphDocument doc)
		{
			try
			{
				if (Current.Gui.ShowPrintDialog())
					if(ShowPrintOptionsDialog(doc))
						doc.Print();

				return true;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(ex.ToString());
			}

			return false;
		}

		/// <summary>
		/// Shows the print preview dialog for the provided graph document.
		/// </summary>
		/// <param name="doc">The graph document for which to show the print preview dialog.</param>
		/// <returns>True if the document was shown in print preview, false if an exceptions was thrown during the print preview.</returns>
		public static bool ShowPrintPreviewDialog(this GraphDocument doc)
		{
			GraphDocumentPrintTask printTask = new GraphDocumentPrintTask(doc);
			printTask.IsPrintPreview = true;
			try
			{
				System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog();
				Current.PrintingService.PrintDocument.PrintPage += printTask.EhPrintPage;
				Current.PrintingService.PrintDocument.QueryPageSettings += printTask.EhQueryPageSettings;
				dlg.Document = Current.PrintingService.PrintDocument;
				dlg.ShowDialog(Current.MainWindow);
				dlg.Dispose();
				return true;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(ex.ToString());
			}
			finally
			{
				Current.PrintingService.PrintDocument.PrintPage -= printTask.EhPrintPage;
				Current.PrintingService.PrintDocument.QueryPageSettings -= printTask.EhQueryPageSettings;
			}
			return false;
		}
		/// <summary>
		/// Shows the dialog to set the printable size of the graph.
		/// </summary>
		/// <param name="doc">Graph document.</param>
		/// <returns>True if user closed the dialog with OK, false if user cancelled the dialog.</returns>
		public static bool ShowPrintableSizeSetupDialog(this GraphDocument doc)
		{
			var options = new Altaxo.Gui.Graph.PrintableAreaSetupOptions();
			options.Area = doc.PrintableBounds;
			object resultobj = options;
			if (Current.Gui.ShowDialog(ref resultobj, "Setup printable area"))
			{
				var result = (Altaxo.Gui.Graph.PrintableAreaSetupOptions)resultobj;
				doc.SetPrintableBounds(result.Area, result.Rescale);
				return true;
			}
			return false;
		}

		public static bool ShowPageSetupDialog(this GraphDocument doc)
		{
			try
			{
				if (Current.Gui.ShowPageSetupDialog())
				{
					if (Current.Gui.YesNoMessageBox("Do you want to resize the graph document to fit into the printable area of the page?", "Question", true))
					{
						doc.SetGraphPageBoundsToPrinterSettings();
						return true;
					}
				}
			}
			catch (Exception exc)
			{
				Current.Gui.ErrorMessageBox(exc.ToString());
			}
			return false;
		}

		/// <summary>
		/// Shows the dialog to set the print options for this document.
		/// </summary>
		/// <param name="doc">The graph document to set the print options for.</param>
		/// <returns>True if user closed the dialog with OK, false if user cancelled the dialog.</returns>
		public static bool ShowPrintOptionsDialog(this GraphDocument doc)
		{
			var options = doc.PrintOptions == null ? new SingleGraphPrintOptions() : (SingleGraphPrintOptions)doc.PrintOptions.Clone();
			object resultobj = options;
			if (Current.Gui.ShowDialog(ref resultobj, "Set print options"))
			{
				var result = (SingleGraphPrintOptions)resultobj;
				doc.PrintOptions = result;
				return true;
			}
			return false;
		}



		/// <summary>
		/// Prints the document on the currently active printer (no Gui interaction). An exception will be thrown
		/// if something fails during printing.
		/// </summary>
		/// <param name="doc">The graph document to print.</param>
		public static void Print(this GraphDocument doc)
		{
			GraphDocumentPrintTask printTask = new GraphDocumentPrintTask(doc);
			Exception ex = null;
			try
			{
					Current.PrintingService.PrintDocument.PrintPage += printTask.EhPrintPage;
					Current.PrintingService.PrintDocument.QueryPageSettings += printTask.EhQueryPageSettings;

					//Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
					Current.PrintingService.PrintDocument.Print();
			}
			catch (Exception exx)
			{
				ex = exx; 
			}
			finally
			{
				Current.PrintingService.PrintDocument.PrintPage -= printTask.EhPrintPage;
				Current.PrintingService.PrintDocument.QueryPageSettings -= printTask.EhQueryPageSettings;
			}

			if(null!=ex)
				throw ex;
		}
	}

	/// <summary>
	/// Manages the print out of a single graph document.
	/// </summary>
	public class GraphDocumentPrintTask
	{
		XYPlotLayerCollection _layers;
		Altaxo.Graph.SingleGraphPrintOptions _printOptions;
		int _page;
		bool _isPrintPreview;

		public bool IsPrintPreview
		{
			get { return _isPrintPreview; }
			set { _isPrintPreview = value; }
		}




		public GraphDocumentPrintTask(GraphDocument doc)
			: this(doc.Layers,doc.PrintOptions)
		{
		}

		public GraphDocumentPrintTask(XYPlotLayerCollection layers, Altaxo.Graph.SingleGraphPrintOptions options)
		{
			_layers = layers;
			_printOptions = options;

			_page = 0;

			if (null == _printOptions)
				_printOptions = new SingleGraphPrintOptions();
		}

		/// <summary>
		/// Infrastructure. Don't use in your own projects. Will be called by PrintDocument during the printing process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void EhQueryPageSettings(object sender, QueryPageSettingsEventArgs e)
		{
			if (_printOptions.RotatePageAutomatically)
			{
				bool needLandscape=false;
				if (_layers.PrintableGraphBounds.Width > _layers.PrintableGraphBounds.Height)
					needLandscape = true;

				e.PageSettings.Landscape = needLandscape;
			}
		}

		/// <summary>
		/// Infrastructure. Don't use in your own projects. Will be called by PrintDocument during the printing process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void EhPrintPage(object sender, PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;
			var savedGraphics = g.Save();
			float hx = e.PageSettings.HardMarginX; // in hundreths of inch
			float hy = e.PageSettings.HardMarginY; // in hundreths of inch
			//g.TranslateTransform(-hx,-hy);

			// First the size of the graph
			// if a fixed zoom factor is set, we use that
			SizeF graphSize = _layers.PrintableGraphBounds.Size;
			float zoom = 1;
			if (_printOptions.UseFixedZoomFactor)
			{
				zoom = (float)_printOptions.ZoomFactor;
				
			}
			else if (_printOptions.FitGraphToPrintIfSmaller)
			{
				float zoomx = e.MarginBounds.Width / graphSize.Width;
				float zoomy = e.MarginBounds.Height / graphSize.Height;
				if (zoomx > 1 && zoomy > 1)
				{
					zoom = Math.Min(zoomx, zoomy) * 72 / 100.0f;
				}
			}
			else if (_printOptions.FitGraphToPrintIfLarger)
			{
				float zoomx = e.MarginBounds.Width / graphSize.Width;
				float zoomy = e.MarginBounds.Height / graphSize.Height;
				if (zoomx < 1 && zoomy < 1)
				{
					zoom = Math.Min(zoomx, zoomy) * 72 / 100.0f;
				}
			}
			graphSize.Width *= zoom;
			graphSize.Height *= zoom;

			// First the location where to start from
			PointF startLocationOnPage = PointF.Empty;
			switch (_printOptions.PrintLocation)
			{
				case  SingleGraphPrintLocation.PrintableAreaLeftUpper:
					startLocationOnPage = e.MarginBounds.Location;
					break;
				case SingleGraphPrintLocation.PageLeftUpper:
					startLocationOnPage = new PointF(0, 0);
					break;
				case SingleGraphPrintLocation.PrintableAreaCenter:
					startLocationOnPage = GetCenter(e.MarginBounds) - GetHalfSize(graphSize);
					break;
				case SingleGraphPrintLocation.PageCenter:
					startLocationOnPage = GetCenter(e.PageBounds) - GetHalfSize(graphSize);
					break;
			}

			PointF startLocationInGraph = new PointF(0, 0);
			if (_printOptions.TilePages)
			{
				startLocationOnPage = e.MarginBounds.Location; // when tiling pages start always from margin bounds
				int columns = (int)Math.Ceiling(graphSize.Width/e.MarginBounds.Width);
				int rows =    (int)Math.Ceiling(graphSize.Height / e.MarginBounds.Height);
				int col = _page % columns;
				int row = _page / columns;

				//startLocationInGraph = new PointF(col * e.MarginBounds.Width / zoom, row * e.MarginBounds.Height / zoom);
				startLocationOnPage.X -= col * e.MarginBounds.Width;
				startLocationOnPage.Y -= row * e.MarginBounds.Height;

				_page++;
				e.HasMorePages = _page < rows * columns;
			}

			if (_printOptions.PrintCropMarks)
			{
				float armLength = Math.Min(e.MarginBounds.Width,e.MarginBounds.Height)/20.0f;
				DrawCross(g, new PointF(e.MarginBounds.Left, e.MarginBounds.Top), armLength);
				DrawCross(g, new PointF(e.MarginBounds.Right,e.MarginBounds.Top), armLength);
				DrawCross(g, new PointF(e.MarginBounds.Left,e.MarginBounds.Bottom), armLength);
				DrawCross(g, new PointF(e.MarginBounds.Right,e.MarginBounds.Bottom), armLength);
			}

		//	DrawCrossedRectangle(g, e.MarginBounds);

			// from here on we have units of Points
			g.PageUnit = GraphicsUnit.Point;

			if(!IsPrintPreview)
				g.TranslateTransform(-hx * 72 / 100.0f, -hy * 72 / 100.0f);
			
			g.TranslateTransform(startLocationOnPage.X*72/100.0f, startLocationOnPage.Y*72/100.0f);

			g.ScaleTransform(zoom, zoom);

			_layers.DoPaint(g, true);

			g.Restore(savedGraphics);
		}

		private void DrawCross(Graphics g, PointF center, float armLength)
		{
			g.DrawLine(Pens.Black, center.X - armLength, center.Y, center.X + armLength, center.Y);
			g.DrawLine(Pens.Black, center.X, center.Y - armLength, center.X, center.Y + armLength);
		}

		private void DrawCrossedRectangle(Graphics g, RectangleF rect)
		{
			g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
			g.DrawLine(Pens.Black, rect.Left, rect.Top, rect.Right, rect.Bottom);
			g.DrawLine(Pens.Black, rect.Left, rect.Bottom, rect.Right, rect.Top);
		}

		private PointF GetCenter(RectangleF r)
		{
			return new PointF(r.X + r.Width / 2, r.Y + r.Height / 2);
		}

		private SizeF GetHalfSize(SizeF s)
		{
			return new SizeF(s.Width / 2, s.Height / 2);
		}
	}
}
