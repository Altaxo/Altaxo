using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph
{
	using Altaxo.Graph.Gdi;

	/// <summary>
	/// Designates where a graph will be printed onto a printer page.
	/// </summary>
	public enum SingleGraphPrintLocation
	{
		/// <summary>Graph will be printed starting at the upper left corner of the printable area.</summary>
		PrintableAreaLeftUpper,
		/// <summary>Graph will be printed starting at the upper left corner of the page (ignoring the page borders).</summary>
		PageLeftUpper,
		/// <summary>Graph will be printed in the center of the printable area.</summary>
		PrintableAreaCenter,
		/// <summary>Graph will be printed in the center of the page.</summary>
		PageCenter
	}

	/// <summary>
	/// Holds options to control the printing process of a graph document.
	/// </summary>
	public class SingleGraphPrintOptions
	{
		/// <summary>If graph is smaller than the printable area, the graph will be zoomed to fill the printable area.</summary>
		bool _fitGraphToPrintIfSmaller;
		
		/// <summary>If graph is larger than the printable area, the graph will be shrinked to fill the printable area.</summary>
		bool _fitGraphToPrintIfLarger;

		/// <summary>If true, the graph will be zoomed by a fixed zoom factor.</summary>
		bool _useFixedZoomFactor;

		/// <summary>Zoom factor used to zoom the graph.</summary>
		double _zoomFactor;

		/// <summary>If graph is larger than the printable area, the graph will be tiled onto multiple pages.</summary>
		bool _tilePages;

		/// <summary>If neccessary, the printer page will be rotated to better fit the graph.</summary>
		bool _rotatePageAutomatically;

		/// <summary>Crop marks will be printed at the corners of the printable area.</summary>
		bool _printCropMarks;

		/// <summary>Designates where the graph will be printed.</summary>
		SingleGraphPrintLocation _printLocation;

		public SingleGraphPrintOptions()
		{
			_zoomFactor = 1;
		}

		public void CopyFrom(SingleGraphPrintOptions from)
		{
			this._fitGraphToPrintIfSmaller = from._fitGraphToPrintIfSmaller;
			this._fitGraphToPrintIfLarger = from._fitGraphToPrintIfLarger;
			this._useFixedZoomFactor = from._useFixedZoomFactor;
			this._zoomFactor = from._zoomFactor;
			this._tilePages = from._tilePages;
			this._rotatePageAutomatically = from._rotatePageAutomatically;
			this._printCropMarks = from._printCropMarks;
			this._printLocation = from._printLocation;
		}

		public object Clone()
		{
			var r = new SingleGraphPrintOptions();
			r.CopyFrom(this);
			return r;
		}


		/// <summary>If graph is smaller than the printable area, the graph will be zoomed to fill the printable area.</summary>
		[System.ComponentModel.Category("Document size")]
		public bool FitGraphToPrintIfSmaller
		{
			get { return _fitGraphToPrintIfSmaller; }
			set { _fitGraphToPrintIfSmaller = value; }
		}


		/// <summary>If graph is larger than the printable area, the graph will be shrinked to fill the printable area.</summary>
		[System.ComponentModel.Category("Document size")]
		public bool FitGraphToPrintIfLarger
		{
			get { return _fitGraphToPrintIfLarger; }
			set { _fitGraphToPrintIfLarger = value; }
		}

		/// <summary>If true, the graph will be zoomed by a fixed zoom factor.</summary>
		[System.ComponentModel.Category("Document size")]
		public bool UseFixedZoomFactor
		{
			get { return _useFixedZoomFactor; }
			set { _useFixedZoomFactor = value; }
		}

		/// <summary>Zoom factor used to zoom the graph.</summary>
		[System.ComponentModel.Category("Document size")]
		public double ZoomFactor
		{
			get { return _zoomFactor; }
			set { _zoomFactor = value; }
		}

		/// <summary>If graph is larger than the printable area, the graph will be tiled onto multiple pages.</summary>
		public bool TilePages
		{
			get { return _tilePages; }
			set { _tilePages = value; }
		}


		/// <summary>If neccessary, the printer page will be rotated to better fit the graph.</summary>
		[System.ComponentModel.Category("Document location")]
		public bool RotatePageAutomatically
		{
			get { return _rotatePageAutomatically; }
			set { _rotatePageAutomatically = value; }
		}


		/// <summary>Crop marks will be printed at the corners of the printable area.</summary>
		public bool PrintCropMarks
		{
			get { return _printCropMarks; }
			set { _printCropMarks = value; }
		}


		/// <summary>Designates where the graph will be printed.</summary>
		[System.ComponentModel.Category("Document location")]
		public SingleGraphPrintLocation PrintLocation
		{
			get { return _printLocation; }
			set { _printLocation = value; }
		}

		/// <summary>
		/// Get the scale factor and the start location of an docoment onto a page.
		/// </summary>
		/// <param name="PageBounds">Bounds of the page in 1/100 inch.</param>
		/// <param name="MarginBounds">Margins of the page in 1/100 inch.</param>
		/// <param name="graphSize">Size of the document in 1/72 inch.</param>
		/// <param name="zoom">Returns the zoom factor to use for the document.</param>
		/// <param name="startLocationOnPage">Returns the start location onto the page in 1/100 inch.</param>
		public void GetZoomAndStartLocation(RectangleF PageBounds, RectangleF MarginBounds, SizeF graphSize, out float zoom, out PointF startLocationOnPage, bool usePrintingUnits)
		{
			// First the size of the graph
			// if a fixed zoom factor is set, we use that

			if (usePrintingUnits) // recalculate everything in units of points (1/72 inch)
			{
				PageBounds = PageBounds.Scale(72.0 / 100);
				MarginBounds = MarginBounds.Scale(72.0 / 100);
			}

			zoom = 1;
			if (this.UseFixedZoomFactor)
			{
				zoom = (float)this.ZoomFactor;

			}
			else if (this.FitGraphToPrintIfSmaller)
			{
				float zoomx = MarginBounds.Width / graphSize.Width;
				float zoomy = MarginBounds.Height / graphSize.Height;
				if (zoomx > 1 && zoomy > 1)
				{
					zoom = Math.Min(zoomx, zoomy);
				}
			}
			else if (this.FitGraphToPrintIfLarger)
			{
				float zoomx = MarginBounds.Width / graphSize.Width;
				float zoomy = MarginBounds.Height / graphSize.Height;
				if (zoomx < 1 && zoomy < 1)
				{
					zoom = Math.Min(zoomx, zoomy);
				}
			}
			graphSize.Width *= zoom;
			graphSize.Height *= zoom;

			// First the location where to start from
			startLocationOnPage = PointF.Empty;
			switch (this.PrintLocation)
			{
				case SingleGraphPrintLocation.PrintableAreaLeftUpper:
					startLocationOnPage = MarginBounds.Location;
					break;
				case SingleGraphPrintLocation.PageLeftUpper:
					startLocationOnPage = new PointF(0, 0);
					break;
				case SingleGraphPrintLocation.PrintableAreaCenter:
					startLocationOnPage = MarginBounds.Center() - graphSize.Half();
					break;
				case SingleGraphPrintLocation.PageCenter:
					startLocationOnPage = PageBounds.Center() - graphSize.Half();
					break;
			}

			if (usePrintingUnits)
			{
				startLocationOnPage = startLocationOnPage.Scale(100.0 / 72);
			}
		}

		
	}
}
