using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{

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
	}
}
