// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.XmlForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ScrollBox : UserControl
	{
		Image  image;
		string text;
		Timer  timer;
		int    scroll = -220;
		
		public int ScrollY {
			get {
				return scroll;
			}
			set {
				scroll = value;
			}
		}
		
		public Image Image {
			get {
				return image;
			}
			set {
				image = value;
			}
		}
		
		public string ScrollText {
			get {
				return text;
			}
			set {
				text =  value;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				timer.Stop();
				foreach (Control ctrl in Controls) {
					ctrl.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		public ScrollBox()
		{
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			Font = resourceService.LoadFont("Tahoma", 10);
			
			text = "\"The most successful method of programming is to begin a program as simply as possible, test it, and then add to the program until it performs the required job.\"\n    -- PDP8 handbook, Pg 9-64\n\n\n";
//			text = "\"The primary purpose of the DATA statement is to give names to constants; instead of referring to pi as 3.141592653589793 at every\n appearance, the variable PI can be given that value with a DATA statement and used instead of the longer form of the constant. This also simplifies modifying the program, should the value of pi change.\"\n    -- FORTRAN manual for Xerox computers\n\n\n";
//			text = "\"No proper program contains an indication which as an operator-applied occurrence identifies an operator-defining occurrence which as an indication-applied occurrence identifies an indication-defining occurrence different from the one identified by the given indication as an indication- applied occurrence.\"\n   -- ALGOL 68 Report\n\n\n";
//			text = "\"The '#pragma' command is specified in the ANSI standard to have an arbitrary implementation-defined effect. In the GNU C preprocessor, `#pragma' first attempts to run the game rogue; if that fails, it tries to run the game hack; if that fails, it tries to run GNU Emacs displaying the Tower of Hanoi; if that fails, it reports a fatal error. In any case, preprocessing does not continue.\"\n   --From an old GNU C Preprocessor document";
			
			timer = new Timer();
			timer.Interval = 20;
			timer.Tick += new EventHandler(ScrollDown);
			timer.Start();
		}
		
		void ScrollDown(object sender, EventArgs e)
		{
			++scroll;
			Refresh();
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			if (image != null) {
				pe.Graphics.DrawImage(image, 0, 0, Width, Height);
			}
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;
			
			g.DrawString(text, Font, Brushes.Black, new Rectangle(Width / 2, 0 - scroll, Width / 2, Height));
			SizeF size = g.MeasureString(text, Font);
			if (scroll > (int)(size.Height + Height)) {
				scroll = -(int)size.Height - Height;
			}
		}
	}
	
	public class CommonAboutDialog : XmlForm
	{
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		public ScrollBox ScrollBox {
			get {
				return (ScrollBox)ControlDictionary["aboutPictureScrollBox"];
			}
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public CommonAboutDialog() : base(propertyService.DataDirectory + @"\resources\dialogs\CommonAboutDialog.xfrm")
		{
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
		}
	}
}
