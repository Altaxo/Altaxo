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
using System.Windows.Forms;
using Crownwood.Magic.Common;

namespace Crownwood.Magic.Docking
{
    public class HotZoneNull : HotZone
    {
        public HotZoneNull(Rectangle hotArea)
            : base(hotArea, hotArea)
        {
        }

        public override void DrawIndicator(Point mousePos) {}
        public override void RemoveIndicator(Point mousePos) {}
    }
}