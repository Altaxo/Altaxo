using System;

namespace MathML.Rendering
{
	/**
	* An area type that effectivly hides the sub tree from cursor selection
	* this is typically used for radical elements where we do not ever want the
	* main radical glyph to ever be selected.
	*/
	internal class NonSelectionArea : BinContainerArea
	{
		public NonSelectionArea(Area child) : base(child)
		{
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			return null;
		}

		public override Object Clone()
		{
			return new NonSelectionArea(child);
		}
	}
}
