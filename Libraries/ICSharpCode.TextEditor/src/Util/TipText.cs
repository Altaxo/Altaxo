// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="?" email="?"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Drawing.Text;

namespace ICSharpCode.TextEditor.Util
{
	class CountTipText: TipText
	{
		float triHeight = 10;
		float triWidth  = 10;
			
		public CountTipText(Graphics graphics, Font font, string text) : base(graphics, font, text)
		{
		}
		
		void DrawTriangle(float x, float y, bool flipped)
		{
			base.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(192, 192, 192)), new RectangleF(x, y, triHeight, triHeight));
			float triHeight2 = triHeight / 2;
			float triHeight4 = triHeight / 4;
			if (flipped) {
				base.Graphics.FillPolygon(new SolidBrush(Color.Black), new PointF[] {
					new PointF(x,                y + triHeight2 - triHeight4),
					new PointF(x + triWidth / 2, y + triHeight2 + triHeight4),
					new PointF(x + triWidth,     y + triHeight2 - triHeight4),
				});
				
			} else {
				base.Graphics.FillPolygon(new SolidBrush(Color.Black), new PointF[] {
					new PointF(x,                y +  triHeight2 + triHeight4),
					new PointF(x + triWidth / 2, y +  triHeight2 - triHeight4),
					new PointF(x + triWidth,     y +  triHeight2 + triHeight4),
				});
			}
		}
	
		public override void Draw(PointF location)
		{
			if (tipText != null && tipText.Length > 0) {
				base.Draw(new PointF(location.X + triWidth + 4, location.Y));
				DrawTriangle(location.X + 2, location.Y + 2, false);
				DrawTriangle(location.X + base.AllocatedSize.Width - triWidth  - 2, location.Y + 2, true);
			}
		}
		
		protected override void OnMaximumSizeChanged()
		{
			if (IsTextVisible()) {
				SizeF tipSize = Graphics.MeasureString
					(tipText, tipFont, MaximumSize,
					 GetInternalStringFormat());
				tipSize.Width += triWidth * 2 + 8;
				SetRequiredSize(tipSize);
			} else {
				SetRequiredSize(SizeF.Empty);
			}
		}
		
	}
	
	class TipText: TipSection
	{
		protected StringAlignment horzAlign;
		protected StringAlignment vertAlign;	
		protected Color           tipColor;
		protected Font            tipFont;
		protected StringFormat    tipFormat;
		protected string          tipText;
        
		public TipText(Graphics graphics, Font font, string text):
			base(graphics)
		{
			tipFont = font; tipText = text;
			
			Color               = SystemColors.InfoText;
			HorizontalAlignment = StringAlignment.Near;
			VerticalAlignment   = StringAlignment.Near;
		}
		
		public override void Draw(PointF location)
		{
			if (IsTextVisible()) {
				RectangleF drawRectangle = new RectangleF
					(location, AllocatedSize);
				
				Graphics.DrawString(tipText, tipFont,
				                    new SolidBrush(Color),
				                    drawRectangle,
				                    GetInternalStringFormat());   
			}
		}
		
		protected StringFormat GetInternalStringFormat()
		{
			if (tipFormat == null) {
				tipFormat = CreateTipStringFormat(horzAlign, vertAlign);
			}
			
			return tipFormat;
		}
		
		protected override void OnMaximumSizeChanged()
		{
			base.OnMaximumSizeChanged();
			
			if (IsTextVisible()) {
				SizeF tipSize = Graphics.MeasureString
					(tipText, tipFont, MaximumSize,
					 GetInternalStringFormat());
				
				SetRequiredSize(tipSize);
			} else {
				SetRequiredSize(SizeF.Empty);
			}
		}
		
		static StringFormat CreateTipStringFormat(StringAlignment horizontalAlignment, StringAlignment verticalAlignment)
		{
			StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone();
			format.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces;
			// note: Align Near, Line Center seemed to do something before
			
			format.Alignment     = horizontalAlignment;
			format.LineAlignment = verticalAlignment;
			
			return format;
		}
		
		protected bool IsTextVisible()
		{
			return tipText != null && tipText.Length > 0;
		}
		
		public Color Color {
			get {
				return tipColor;
			}
			set {
				tipColor = value;
			}
		}
		
		public StringAlignment HorizontalAlignment {
			get {
				return horzAlign;
			}
			set {
				horzAlign = value;
				tipFormat = null;
			}
		}
		
		public StringAlignment VerticalAlignment {
			get {
				return vertAlign;
			}
			set {
				vertAlign = value;
				tipFormat = null;
			}
		}
	}
}
