// *****************************************************************************
// 
//  (c) Crownwood Consulting Limited 2002-2003
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Crownwood Consulting 
//	Limited, Crownwood, Bracknell, Berkshire, England and are supplied subject 
//  to licence terms.
// 
//  Magic Version 1.7.4.0 	www.dotnetmagic.com
// *****************************************************************************

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Crownwood.Magic.Win32;

namespace Crownwood.Magic.Common
{
    public class ColorHelper
    {
        public static Color TabBackgroundFromBaseColor(Color backColor)
        {
            Color backIDE;

            // Check for the 'Classic' control color
            if ((backColor.R == 212) &&
                (backColor.G == 208) &&
                (backColor.B == 200))
            {
                // Use the exact background for this color
                backIDE = Color.FromArgb(247, 243, 233);
            }
            else
            {
                // Check for the 'XP' control color
                if ((backColor.R == 236) &&
                    (backColor.G == 233) &&
                    (backColor.B == 216))
                {
                    // Use the exact background for this color
                    backIDE = Color.FromArgb(255, 251, 233);
                }
                else
                {
                    // Calculate the IDE background color as only half as dark as the control color
                    int red = 255 - ((255 - backColor.R) / 2);
                    int green = 255 - ((255 - backColor.G) / 2);
                    int blue = 255 - ((255 - backColor.B) / 2);
                    backIDE = Color.FromArgb(red, green, blue);
                }
            }
                        
            return backIDE;
        }
	
		public static Color CalculateColor(Color front, Color back, int alpha)
		{
			// Use alpha blending to brigthen the colors but don't use it
			// directly. Instead derive an opaque color that we can use.
			Color frontColor = Color.FromArgb(255, front);
			Color backColor = Color.FromArgb(255, back);

			float frontRed = frontColor.R;
			float frontGreen = frontColor.G;
			float frontBlue = frontColor.B;
			float backRed = backColor.R;
			float backGreen = backColor.G;
			float backBlue = backColor.B;

			float fRed = (frontRed * alpha / 255) + backRed * ((float)(255-alpha)/255);
			float fGreen = (frontGreen * alpha / 255) + backGreen * ((float)(255-alpha)/255);
			float fBlue = (frontBlue * alpha / 255) + backBlue * ((float)(255-alpha)/255);

			byte newRed = (byte)fRed;
			byte newGreen = (byte)fGreen;
			byte newBlue = (byte)fBlue;

			return  Color.FromArgb(255, newRed, newGreen, newBlue);
		}
	}
}


