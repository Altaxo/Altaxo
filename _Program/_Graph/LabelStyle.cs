using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
	public abstract class LabelStyle : LayerEdge
	{
		public LabelStyle(EdgeType st)
			: base(st)
		{
		}

		public abstract void Paint(Graphics g, Layer layer, Axis axis, XYLayerAxisStyle axisstyle);


	}


	/// <summary>
	/// Summary description for SimpleLabelStyle.
	/// </summary>
	public class SimpleLabelStyle : LabelStyle
	{
		protected Font m_Font = new Font(FontFamily.GenericSansSerif,18,GraphicsUnit.World);

		public SimpleLabelStyle(EdgeType st)
		 : base(st)
		{
		}

		public override void Paint(Graphics g, Layer layer, Axis axis, XYLayerAxisStyle axisstyle)
		{
			SizeF layerSize = layer.layerSize;
			PointF orgP = GetOrg(layerSize);
			PointF endP = GetEnd(layerSize);
			PointF outVector = OuterVector;
			float dist_x = axisstyle.OuterDistance+axisstyle.GetOffset(layerSize); // Distance from axis tick point to label
			float dist_y = axisstyle.OuterDistance+axisstyle.GetOffset(layerSize); // y distance from axis tick point to label

			dist_x += this.m_Font.SizeInPoints/3; // add some space to the horizontal direction in order to separate the chars a little from the ticks

			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			StringFormat strfmt = StringFormat.GenericTypographic;
			strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			// next statement is necessary to have a consistent string length both
			// on 0 degree rotated text and rotated text
			// without this statement, the text is fitted to the pixel grid, which
			// leads to "steps" during scaling
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

			// set the alignment and line alignment of the strings
			switch(this.m_StyleType)
			{
				case EdgeType.Bottom:
					strfmt.LineAlignment = StringAlignment.Near;
					strfmt.Alignment     = StringAlignment.Center;
					dist_x = 0;
					break;
				case EdgeType.Top:
					strfmt.LineAlignment = StringAlignment.Far;
					strfmt.Alignment     = StringAlignment.Center;
					dist_x = 0;
					dist_y = -dist_y;
					break;
				case EdgeType.Left:
					strfmt.LineAlignment = StringAlignment.Center;
					strfmt.Alignment     = StringAlignment.Far;
					dist_x = -dist_x;
					dist_y = 0;
					break;
				case EdgeType.Right:
					strfmt.LineAlignment = StringAlignment.Center;
					strfmt.Alignment     = StringAlignment.Near;
					dist_y = 0;
					break;
			}


			// print the major ticks
			double[] majorticks = axis.GetMajorTicks();
			for(int i=0;i<majorticks.Length;i++)
			{
				double r = axis.PhysicalToNormal(majorticks[i]);
				PointF tickorg = GetEdgePoint(layerSize,r);

				string txt = majorticks[i].ToString();
				g.DrawString(txt, m_Font, Brushes.Black, tickorg.X + dist_x, tickorg.Y + dist_y, strfmt);
			}




		}

	}
}
