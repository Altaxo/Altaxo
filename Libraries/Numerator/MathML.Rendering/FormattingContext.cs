//This file is part of MathML.Rendering, a library for displaying mathml
//Copyright (C) 2003, Andy Somogyi
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//For details, see http://numerator.sourceforge.net, or send mail to
//(slightly obfuscated for spam mail harvesters)
//andy[at]epsilon3[dot]net

using System;
using System.Drawing;
using System.Diagnostics;
using Scaled = System.Single;

namespace MathML.Rendering
{
	internal class FormattingContext
	{
		/// <summary>
		/// default contstuctor
		/// </summary>
		public FormattingContext()
		{
			size = Evaluate(new Length(LengthType.Pt, DefaultFontPointSize));
			minSize  = Evaluate(new Length(LengthType.Pt, 6));
			actualSize = size;
		}

		/// <summary>
		/// default contstuctor
		/// </summary>
		public FormattingContext(int fontSize)
		{
			size = Evaluate(new Length(LengthType.Pt, fontSize));
			minSize  = Evaluate(new Length(LengthType.Pt, 6));
			actualSize = size;
		}

		/// <summary>
		/// make a new copy of an existing context
		/// </summary>
		public FormattingContext(FormattingContext ctx)
		{
			this.actualSize = ctx.actualSize;
			this.BackgroundColor = ctx.BackgroundColor;
			this.Color = ctx.Color;
			this.DisplayStyle = ctx.DisplayStyle;
			this.minSize = ctx.minSize;
			this.scriptLevel = ctx.scriptLevel;
			this.size = ctx.size;
			this.SizeMultiplier = ctx.SizeMultiplier;
			this.Stretch = ctx.Stretch;
			this.cacheArea = ctx.cacheArea;
			this.Parens = ctx.Parens;
		}

		public const int DefaultFontPointSize = 13;

		/// <summary>
		/// The current font size, this is the current emHeight of the font to used
		/// for creating areas when a node is formatted.
		/// </summary>
		public int Size
		{
			get { return size; }
			set 
			{
				size = Math.Max(value, minSize);
				actualSize = size;
			}
		}

		/// <summary>
		/// evaluate a length using the current font size (in pixels)
		/// as the default size
		/// </summary>
		public int Evaluate(Length length)
		{
			return Evaluate(length, Size);
		}

        /// <summary>
		/// evaluate a length to a true size in pixels. 
		/// this calculation is affected by the current font, and 
		/// by any previous (parent) style node. In gtkmathview, this 
		/// method was part of the graphic device, but I think that it
		/// makes more sense as part of the formatting context. This
		/// calculation is heavily dependent on the current state of
		/// formatting, and the formatting context manages that 
		/// state, so lenght evaluation should go here.
        /// </summary>
        /// <param name="length">a Math Length if the length is not valid, 
        /// the default valud is returned</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
		public int Evaluate(Length length, float defaultValue)
		{
			int result = 0;
			switch(length.Type)
			{
				case LengthType.Big:
				{
					result = 20;
				} break;
				case LengthType.Cm:
				{
					result = GraphicDevice.CMsToPixels(length.Value);
				} break;
				case LengthType.Em:
				{
					result = (int)(defaultValue * length.Value);
				} break;
				case LengthType.Ex:
				{
					result = (int)(GraphicDevice.Ex(this) * length.Value);
				} break;
				case LengthType.In:
				{
					result = GraphicDevice.InchesToPixels(length.Value);
				} break;
				case LengthType.Infinity:
				{
					Debug.Assert(false);
				} break;
				case LengthType.Medium:
				{
					result = (int)(medium * defaultValue);
				} break;
				case LengthType.Mm:
				{
					result = GraphicDevice.MMsToPixels(length.Value);
				} break;
				case LengthType.NegativeMedium:
				{
					result = (int)(negativeMedium * defaultValue);
				} break;
				case LengthType.NegativeThick:
				{
					result = (int)(negativeThick * defaultValue);
				} break;
				case LengthType.NegativeThin:
				{
					result = (int)(negativeThin * defaultValue);
				} break;
				case LengthType.NegativeVeryThick:
				{
					result = (int)(negativeVeryThick * defaultValue);
				} break;
				case LengthType.NegativeVeryThin:
				{
					result = (int)(negativeVeryThin * defaultValue);
				} break;
				case LengthType.NegativeVeryVeryThick:
				{
					result = (int)(negativeVeryVeryThick * defaultValue);
				} break;
				case LengthType.NegativeVeryVeryThin:
				{
					result = (int)(negativeVeryVeryThin * defaultValue);
				} break;
				case LengthType.Normal:
				{
					result = (int)defaultValue;
				} break;
				case LengthType.Pc:
				{
					result = GraphicDevice.PicasToPixels(length.Value);
				} break;
				case LengthType.Percentage:
				{
					result = (int)(length.Value * defaultValue / 100.0f);
				} break;
				case LengthType.Pt:
				{
					result = GraphicDevice.PointsToPixels(length.Value);
				} break;
				case LengthType.Pure:
				{
					Debug.Assert(false);
				} break;
				case LengthType.Px:
				{
					result = (int)length.Value;
				} break;
				case LengthType.Small:
				{
					Debug.Assert(false);
				} break;
				case LengthType.Thick:
				{
					result = (int)(thick * defaultValue);
				} break;
				case LengthType.Thin:
				{
					result = (int)(thin * defaultValue);
				} break;
				case LengthType.Undefined:
				{
					Debug.Assert(false);
				} break;
				case LengthType.VeryThick:
				{
					result = (int)(veryThick * defaultValue);
				} break;
				case LengthType.VeryThin:
				{
					result = (int)(veryThin * defaultValue);
				} break;
				case LengthType.VeryVeryThick:
				{
					result = (int)(veryVeryThick * defaultValue);
				} break;
				case LengthType.VeryVeryThin:
				{
					result = (int)(veryVeryThin * defaultValue);
				} break;
			}
			return result;
		}
	
		/// <summary>
		/// the actual font size. this value is not to be used for creating fonts, 
		/// instead, use the Size value. This value is the true value of the font, 
		/// not taking into acount the min and max font sizes.
		/// </summary>
		public int actualSize;

		/// <summary>
		/// the MathVariant from a presentation token node
		/// </summary>
		public MathVariant MathVariant = MathVariant.Normal;

		/// <summary>
		/// color used for drawing
		/// </summary>
		public Color Color;

		/// <summary>
		/// background color used for drawing
		/// </summary>
		public Color BackgroundColor;

		/// <summary>
		/// number of nested scripts
		/// </summary>
		public int ScriptLevel
		{
			get { return scriptLevel; }
			set
			{
				int d = value - scriptLevel;
				actualSize = (int)(actualSize * Math.Pow(SizeMultiplier, d));
				Size = actualSize;
			}
		}

		/// <summary>
		/// should the current area be cached?
		/// </summary>
		public bool cacheArea = true;

		/// <summary>
		/// currently used for content areas. If true, an apply element should use parens, as
		/// it is inside a function or some other item that does not need parens.
		/// </summary>
		public bool Parens = false;

		/// <summary>
		/// minimum font size that the script can be reduced to. This 
		/// defauts to the value of a 6 point font.
		/// </summary>
		private int minSize;

		/// <summary>
		/// true if formulas must be formated in display mode
		/// </summary>
		public MathML.Display DisplayStyle;

		/// <summary>
		/// amount by which the font size is multiplied when the script level
		/// is incresed or decreased by one
		/// </summary>
		public float SizeMultiplier = 0.71f;

		/// <summary>
		/// the extent at which the node is being asked to stretch to
		/// </summary>
		public BoundingBox Stretch = BoundingBox.New();
        
		/// <summary>
		/// the font size
		/// </summary>
		private int size;

		/// <summary>
		/// the script level
		/// </summary>
		private int scriptLevel = 0;

		/// <summary>
		/// scale factors for calculating thickneseses
		/// </summary>
		private float veryVeryThin = 1.0f/18.0f;
		private float veryThin = 2.0f/18.0f;
		private float thin = 3.0f/18.0f;
		private float medium = 4.0f/18.0f;
		private float thick = 5.0f/18.0f;
		private float veryThick = 6.0f/18.0f;
		private float veryVeryThick = 7.0f/18.0f;

		private float negativeVeryVeryThin = 1.0f/18.0f;
		private float negativeVeryThin = 2.0f/18.0f;
		private float negativeThin = 3.0f/18.0f;
		private float negativeMedium = 4.0f/18.0f;
		private float negativeThick = 5.0f/18.0f;
		private float negativeVeryThick = 6.0f/18.0f;
		private float negativeVeryVeryThick = 7.0f/18.0f;
	}
}
