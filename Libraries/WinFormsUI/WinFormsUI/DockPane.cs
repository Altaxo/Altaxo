// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WeifenLuo.WinFormsUI
{
	public class DockPane : UserControl
	{
		protected enum DockPaneAppearance
		{
			ToolWindow,
			Document
		}

		protected enum HitTestArea
		{
			Caption,
			TabStrip,
			Content,
			None
		}

		protected struct HitTestResult
		{
			public HitTestArea HitArea;
			public int Index;

			public HitTestResult(HitTestArea hitTestArea)
			{
				HitArea = hitTestArea;
				Index = -1;
			}

			public HitTestResult(HitTestArea hitTestArea, int index)
			{
				HitArea = hitTestArea;
				Index = index;
			}
		}

		private const int WM_DOCKSTATECHANGED = (int)Win32.Msgs.WM_USER + 1;

		static private Bitmap ImageDockWindowCloseEnabled;
		static private Bitmap ImageDockWindowCloseDisabled;
		static private Bitmap ImageAutoHideYes;
		static private Bitmap ImageAutoHideNo;
		static private StringFormat StringFormatDockWindowCaption;
		static private StringFormat StringFormatDockWindowTab;
		static private string ToolTipDockWindowClose;
		static private string ToolTipAutoHide;

		static private Bitmap ImageDocumentWindowCloseEnabled;
		static private Bitmap ImageDocumentWindowCloseDisabled;
		static private Bitmap ImageScrollLeftEnabled;
		static private Bitmap ImageScrollLeftDisabled;
		static private Bitmap ImageScrollRightEnabled;
		static private Bitmap ImageScrollRightDisabled;
		static private string ToolTipDocumentWindowClose;
		static private string ToolTipScrollLeft;
		static private string ToolTipScrollRight;
		static private StringFormat StringFormatDocumentWindowTab;

		static DockPane()
		{
			//For Tool Window style
			ImageDockWindowCloseEnabled = ResourceHelper.LoadBitmap("DockPane.ToolWindowCloseEnabled.bmp");
			ImageDockWindowCloseDisabled = ResourceHelper.LoadBitmap("DockPane.ToolWindowCloseDisabled.bmp");
			ImageAutoHideYes = ResourceHelper.LoadBitmap("DockPane.AutoHideYes.bmp");
			ImageAutoHideNo = ResourceHelper.LoadBitmap("DockPane.AutoHideNo.bmp");

			StringFormatDockWindowCaption = new StringFormat();
			StringFormatDockWindowCaption.Trimming = StringTrimming.EllipsisCharacter;
			StringFormatDockWindowCaption.LineAlignment = StringAlignment.Center;
			StringFormatDockWindowCaption.FormatFlags = StringFormatFlags.NoWrap;

			StringFormatDockWindowTab = new StringFormat(StringFormat.GenericTypographic);
			StringFormatDockWindowTab.Trimming = StringTrimming.EllipsisCharacter;
			StringFormatDockWindowTab.LineAlignment = StringAlignment.Center;
			StringFormatDockWindowTab.FormatFlags = StringFormatFlags.NoWrap;

			ToolTipDockWindowClose = ResourceHelper.GetString("DockPane.ToolTipDockWindowClose");
			ToolTipAutoHide = ResourceHelper.GetString("DockPane.ToolTipAutoHide");

			//For Document style
			ImageDocumentWindowCloseEnabled = ResourceHelper.LoadBitmap("DockPane.DocumentCloseEnabled.bmp");
			ImageDocumentWindowCloseDisabled = ResourceHelper.LoadBitmap("DockPane.DocumentCloseDisabled.bmp");
			ImageScrollLeftEnabled = ResourceHelper.LoadBitmap("DockPane.ScrollLeftEnabled.bmp");
			ImageScrollLeftDisabled = ResourceHelper.LoadBitmap("DockPane.ScrollLeftDisabled.bmp");
			ImageScrollRightEnabled = ResourceHelper.LoadBitmap("DockPane.ScrollRightEnabled.bmp");
			ImageScrollRightDisabled = ResourceHelper.LoadBitmap("DockPane.ScrollRightDisabled.bmp");
			ToolTipDocumentWindowClose = ResourceHelper.GetString("DockPane.ToolTipDocumentWindowClose");
			ToolTipScrollLeft = ResourceHelper.GetString("DockPane.ToolTipScrollLeft");
			ToolTipScrollRight = ResourceHelper.GetString("DockPane.ToolTipScrollRight");

			StringFormatDocumentWindowTab = new StringFormat(StringFormat.GenericTypographic);
			StringFormatDocumentWindowTab.Alignment = StringAlignment.Center;
			StringFormatDocumentWindowTab.Trimming = StringTrimming.EllipsisPath;
			StringFormatDocumentWindowTab.LineAlignment = StringAlignment.Center;
			StringFormatDocumentWindowTab.FormatFlags = StringFormatFlags.NoWrap;
		}

		private InertButton m_buttonDockWindowClose, m_buttonAutoHide;
		private InertButton m_buttonDocumentWindowClose, m_buttonScrollLeft, m_buttonScrollRight;
		private NestedDockingStatus m_nestedDockingNormal, m_nestedDockingFloat;
		private int m_offset = 0;
		private ToolTip m_toolTip;

		public DockPane(DockContent content, DockState visibleState, bool isHidden)
		{
			InternalConstruct(content, visibleState, isHidden, null, false, Rectangle.Empty);
		}

		public DockPane(DockContent content, DockState visibleState, bool isHidden, FloatWindow floatWindow)
		{
			InternalConstruct(content, visibleState, isHidden, floatWindow, false, Rectangle.Empty);
		}

		public DockPane(DockContent content, Rectangle floatWindowBounds)
		{
			InternalConstruct(content, DockState.Float, false, null, true, floatWindowBounds);
		}

		private void InternalConstruct(DockContent content, DockState visibleState, bool isHidden, FloatWindow floatWindow, bool flagBounds, Rectangle floatWindowBounds)
		{
			if (content == null)
				throw new ArgumentNullException(ResourceHelper.GetString("DockPane.Constructor.NullContent"));

			if (content.DockPanel == null)
				throw new ArgumentException(ResourceHelper.GetString("DockPane.Constructor.NullDockPanel"));

			if (visibleState == DockState.Hidden || visibleState == DockState.Unknown)
				throw new ArgumentException(ResourceHelper.GetString("DockPane.VisibleState.InvalidState"));

			SuspendLayout();
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.Selectable, true);

			m_contents = new DockContentCollection();
			m_dockPanel = content.DockPanel;
			m_dockPanel.AddPane(this);
			m_components = new Container();
			m_toolTip = new ToolTip(Components);

			m_buttonDockWindowClose = new InertButton(ImageDockWindowCloseEnabled, ImageDockWindowCloseDisabled);
			m_buttonAutoHide = new InertButton();

			m_buttonDockWindowClose.ToolTipText = ToolTipDockWindowClose;
			m_buttonDockWindowClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonDockWindowClose.Click += new EventHandler(this.Close_Click);

			m_buttonAutoHide.ToolTipText = ToolTipAutoHide;
			m_buttonAutoHide.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonAutoHide.Click += new EventHandler(AutoHide_Click);

			m_buttonDocumentWindowClose = new InertButton(ImageDocumentWindowCloseEnabled, ImageDocumentWindowCloseDisabled);
			m_buttonScrollLeft = new InertButton(ImageScrollLeftEnabled, ImageScrollLeftDisabled);
			m_buttonScrollRight = new InertButton(ImageScrollRightEnabled, ImageScrollRightDisabled);

			m_buttonDocumentWindowClose.ToolTipText = ToolTipDocumentWindowClose;
			m_buttonDocumentWindowClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonDocumentWindowClose.Click += new EventHandler(this.Close_Click);

			m_buttonScrollLeft.Enabled = false;
			m_buttonScrollLeft.ToolTipText = ToolTipScrollLeft;
			m_buttonScrollLeft.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonScrollLeft.Click += new EventHandler(ScrollLeft_Click);

			m_buttonScrollRight.Enabled = false;
			m_buttonScrollRight.ToolTipText = ToolTipScrollRight;
			m_buttonScrollRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonScrollRight.Click += new EventHandler(ScrollRight_Click);

			m_splitter = new DockPaneSplitter(this);

			Controls.AddRange(new Control[] {	m_buttonAutoHide,
												m_buttonDockWindowClose,
												m_buttonScrollLeft,
												m_buttonScrollRight,
												m_buttonDocumentWindowClose	});

			m_nestedDockingNormal = new NestedDockingStatus(this);
			m_nestedDockingFloat = new NestedDockingStatus(this);

			Font = SystemInformation.MenuFont;

			if (floatWindow != null)
				FloatWindow = floatWindow;
			else if (flagBounds)
				DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this, floatWindowBounds);

			content.Pane = this;
			m_visibleState = visibleState;
			m_isHidden = isHidden;
			SetDockState();
			ResumeLayout();
		}

		private void Close_Click(object sender, EventArgs e)
		{
			CloseActiveContent();
			if (ActiveContent != null)
				ActiveContent.Activate();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Components.Dispose();
				Contents.Dispose();

				m_visibleState = DockState.Unknown;
				m_isHidden = false;
				m_dockState = DockState.Unknown;

				if (FloatWindow != null)
					FloatWindow = null;

				if (DockWindow != null)
					DockWindow = null;

				if (DockPanel != null)
				{
					DockPanel.RemovePane(this);
					m_dockPanel = null;
				}

				m_splitter.Dispose();
			}
			base.Dispose(disposing);
		}

		private DockContent m_activeContent = null;
		public virtual DockContent ActiveContent
		{
			get	{	return m_activeContent;	}
			set
			{
				if (ActiveContent == value)
					return;

				if (value != null)
				{
					if (GetIndexOfVisibleContents(value) == -1)
						throw(new InvalidOperationException(ResourceHelper.GetString("DockPane.ActiveContent.InvalidValue")));
				}
				else
				{
					if (CountOfVisibleContents != 0)
						throw(new InvalidOperationException(ResourceHelper.GetString("DockPane.ActiveContent.InvalidValue")));
				}

				DockContent oldValue = m_activeContent;

				if (DockPanel.ActiveAutoHideContent == oldValue)
					DockPanel.ActiveAutoHideContent = null;

				m_activeContent = value;
				if (m_activeContent != null)
					EnsureTabVisible(m_activeContent);

				if (FloatWindow != null)
					FloatWindow.SetText();

				Invalidate();
				DockPanel.RefreshActiveWindow();
			}
		}
		
		private bool m_allowRedocking = true;
		public virtual bool AllowRedocking
		{
			get	{	return m_allowRedocking;	}
			set	{	m_allowRedocking = value;	}
		}

		protected virtual Rectangle CaptionRectangle
		{
			get	{	return Appearance == DockPaneAppearance.ToolWindow ? CaptionRectangle_ToolWindow : Rectangle.Empty;	}
		}

		private Rectangle CaptionRectangle_ToolWindow
		{
			get
			{
				if (!HasCaption)
					return Rectangle.Empty;

				Rectangle rectWindow = DisplayingRectangle;
				int x, y, width;
				x = rectWindow.X;
				y = rectWindow.Y;
				width = rectWindow.Width;

				int height = Font.Height + MeasureToolWindowCaption.TextGapTop + MeasureToolWindowCaption.TextGapBottom;

				if (height < ImageDockWindowCloseEnabled.Height + MeasureToolWindowCaption.ButtonGapTop + MeasureToolWindowCaption.ButtonGapBottom)
					height = ImageDockWindowCloseEnabled.Height + MeasureToolWindowCaption.ButtonGapTop + MeasureToolWindowCaption.ButtonGapBottom;

				return new Rectangle(x, y, width, height);
			}
		}

		public virtual string CaptionText
		{
			get	{	return ActiveContent == null ? string.Empty : ActiveContent.Text;	}
		}

		private IContainer m_components;
		protected IContainer Components
		{
			get	{	return m_components;	}
		}

		protected virtual Rectangle ContentRectangle
		{
			get
			{
				Rectangle rectWindow = DisplayingRectangle;
				Rectangle rectCaption = CaptionRectangle;
				Rectangle rectTabStrip = GetTabStripRectangle();

				int x = rectWindow.X;
				int y = rectWindow.Y + (rectCaption.IsEmpty ? 0 : rectCaption.Height) +
					(DockState == DockState.Document ? rectTabStrip.Height : 0);
				int width = rectWindow.Width;
				int height = rectWindow.Height - rectCaption.Height - rectTabStrip.Height;

				return new Rectangle(x, y, width, height);
			}
		}

		private DockContentCollection m_contents;
		public DockContentCollection Contents
		{
			get	{	return m_contents;	}
		}

		public int CountOfVisibleContents
		{
			get
			{
				int count = 0;
				foreach (DockContent content in Contents)
				{
					if (!content.IsHidden)
						count ++;
				}
				return count;
			}
		}

		protected override Size DefaultSize
		{
			get	{	return Size.Empty;	}
		}

		private DockPanel m_dockPanel;
		public DockPanel DockPanel
		{
			get	{	return m_dockPanel;	}
		}

		protected bool HasCaption
		{
			get
			{	
				if (DockState == DockState.Document ||
					DockState == DockState.Hidden ||
					DockState == DockState.Unknown ||
					(DockState == DockState.Float && FloatWindow.DisplayingList.Count <= 1))
					return false;
				else
					return true;
			}
		}

		private bool m_isActivated = false;
		public bool IsActivated
		{
			get	{	return m_isActivated;	}
		}
		internal void SetIsActivated(bool value)
		{
			if (m_isActivated == value)
				return;

			m_isActivated = value;
			if (DockState != DockState.Document)
				Invalidate();
			OnIsActivatedChanged(EventArgs.Empty);
		}

		private bool m_isActiveDocumentPane = false;
		public bool IsActiveDocumentPane
		{
			get	{	return m_isActiveDocumentPane;	}
		}
		internal void SetIsActiveDocumentPane(bool value)
		{
			if (m_isActiveDocumentPane == value)
				return;

			m_isActiveDocumentPane = value;
			if (DockState == DockState.Document)
				Refresh();
			OnIsActiveDocumentPaneChanged(EventArgs.Empty);
		}

		public bool IsDockStateValid(DockState dockState)
		{
			foreach (DockContent content in Contents)
				if (!content.IsDockStateValid(dockState))
					return false;

			return true;
		}

		public bool IsAutoHide
		{
			get	{	return DockHelper.IsDockStateAutoHide(DockState);	}
		}

		private bool m_isHidden = false;
		public bool IsHidden
		{
			get	{	return m_isHidden;	}
			set
			{
				if (m_isHidden == value)
					return;

				m_isHidden = value;
				SetDockState();
			}
		}

		private DockPaneSplitter m_splitter;
		internal DockPaneSplitter Splitter
		{
			get	{	return m_splitter;	}
		}

		protected DockPaneAppearance Appearance
		{
			get	{	return (DockState == DockState.Document) ? DockPaneAppearance.Document : DockPaneAppearance.ToolWindow;	}
		}

		private Rectangle TabStripRectangle
		{
			get	{	return GetTabStripRectangle();	}
		}

		protected virtual Rectangle DisplayingRectangle
		{
			get	{	return ClientRectangle;	}
		}

		public void Activate()
		{
			if (IsHidden)
				Show();

			if (DockHelper.IsDockStateAutoHide(DockState) && DockPanel.ActiveContent != ActiveContent)
				DockPanel.ActiveAutoHideContent = ActiveContent;
				
			if (!IsActivated)
				Focus();
		}

		private void AutoHide_Click(object sender, EventArgs e)
		{
			DockState = DockHelper.ToggleAutoHideState(DockState);
			if (!IsAutoHide)
				Activate();
		}

		internal void AddContent(DockContent content)
		{
			if (Contents.Contains(content))
				return;

			Contents.Add(content);
			content.SetParent(this);
			SetDockState();
			if (!content.IsHidden && CountOfVisibleContents == 1)
			{
				ActiveContent = content;
				Refresh();
			}
		}

		private void CalculateTabs()
		{
			if (Appearance == DockPaneAppearance.ToolWindow)
				CalculateTabs_ToolWindow();
			else
				CalculateTabs_Document();
		}

		private void CalculateTabs_ToolWindow()
		{
			if (CountOfVisibleContents <= 1 || IsAutoHide)
				return;

			Rectangle rectTabStrip = GetTabStripRectangle();

			//////////////////////////////////////////////////////////////////////////////
			/// Calculate tab widths
			/// //////////////////////////////////////////////////////////////////////////
			int countOfVisibleContents = CountOfVisibleContents;
			int[] maxWidths = new int[countOfVisibleContents];
			bool[] flags = new bool[countOfVisibleContents];
			for (int i=0; i<countOfVisibleContents; i++)
			{
				maxWidths[i] = GetTabOriginalWidth(i);
				flags[i] = false;
			}

			// Set tab whose max width less than average width
			bool anyWidthWithinAverage = true;
			int totalWidth = rectTabStrip.Width - MeasureToolWindowTab.StripGapLeft - MeasureToolWindowTab.StripGapRight;
			int totalAllocatedWidth = 0;
			int averageWidth = totalWidth / countOfVisibleContents;
			int remainedContents = countOfVisibleContents;
			for (anyWidthWithinAverage=true; anyWidthWithinAverage && remainedContents>0;)
			{
				anyWidthWithinAverage = false;
				for (int i=0; i<countOfVisibleContents; i++)
				{
					if (flags[i])
						continue;

					DockContent content = GetVisibleContent(i);

					if (maxWidths[i] <= averageWidth)
					{
						flags[i] = true;
						content.TabWidth = maxWidths[i];
						totalAllocatedWidth += content.TabWidth;
						anyWidthWithinAverage = true;
						remainedContents--;
					}
				}
				if (remainedContents != 0)
					averageWidth = (totalWidth - totalAllocatedWidth) / remainedContents;
			}

			// If any tab width not set yet, set it to the average width
			if (remainedContents > 0)
			{
				int roundUpWidth = (totalWidth - totalAllocatedWidth) - (averageWidth * remainedContents);
				for (int i=0; i<countOfVisibleContents; i++)
				{
					if (flags[i])
						continue;

					DockContent content = GetVisibleContent(i);

					flags[i] = true;
					if (roundUpWidth > 0)
					{
						content.TabWidth = averageWidth + 1;
						roundUpWidth --;
					}
					else
						content.TabWidth = averageWidth;
				}
			}

			//////////////////////////////////////////////////////////////////////////////
			/// Set the X position of the tabs
			/////////////////////////////////////////////////////////////////////////////
			int x = rectTabStrip.X + MeasureToolWindowTab.StripGapLeft;
			for (int i=0; i<countOfVisibleContents; ++i)
			{
				DockContent content = GetVisibleContent(i);
				content.TabX = x;
				x += content.TabWidth;
			}
		}

		private void CalculateTabs_Document()
		{
			int countOfVisibleContents = CountOfVisibleContents;
			if (countOfVisibleContents == 0)
				return;

			Rectangle rectTabStrip = GetTabStripRectangle();
			int x = rectTabStrip.X + MeasureDocumentTab.TabGapLeft + m_offset;
			for (int i=0; i<countOfVisibleContents; i++)
			{
				DockContent content = GetVisibleContent(i);
				content.TabX = x;
				content.TabWidth = Math.Min(GetTabOriginalWidth(i), MeasureDocumentTab.TabMaxWidth);
				x += content.TabWidth;
			}
		}

		public virtual void Close()
		{
			Dispose();
		}

		protected virtual void CloseActiveContent()
		{
			DockContent content = ActiveContent;

			if (content == null)
				return;

			if (content.HideOnClose)
				content.Hide();
			else
			{
				///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				/// Workaround for .Net Framework bug: removing control from Form may cause form
				/// unclosable.
				///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				Form form = FindForm();
				if (ContainsFocus)
				{
					if (form is FloatWindow)
					{
						((FloatWindow)form).DummyControl.Focus();
						form.ActiveControl = ((FloatWindow)form).DummyControl;
					}
					else if (DockPanel != null)
					{
						DockPanel.DummyControl.Focus();
						if (form != null)
							form.ActiveControl = DockPanel.DummyControl;
					}
				}
				//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				
				content.Close();
			}
		}

		protected virtual void DrawCaption(Graphics g)
		{
			if (Appearance == DockPaneAppearance.Document)
				return;

			Rectangle rectCaption = CaptionRectangle;

			if (rectCaption.IsEmpty)
				return;

			Brush brushBackGround = IsActivated ? SystemBrushes.ActiveCaption : SystemBrushes.Control;

			g.FillRectangle(brushBackGround, rectCaption);

			if (!IsActivated)
			{
				g.DrawLine(SystemPens.GrayText, rectCaption.X + 1, rectCaption.Y, rectCaption.X + rectCaption.Width - 2, rectCaption.Y);
				g.DrawLine(SystemPens.GrayText, rectCaption.X + 1, rectCaption.Y + rectCaption.Height - 1, rectCaption.X + rectCaption.Width - 2, rectCaption.Y + rectCaption.Height - 1);
				g.DrawLine(SystemPens.GrayText, rectCaption.X, rectCaption.Y + 1, rectCaption.X, rectCaption.Y + rectCaption.Height - 2);
				g.DrawLine(SystemPens.GrayText, rectCaption.X + rectCaption.Width - 1, rectCaption.Y + 1, rectCaption.X + rectCaption.Width - 1, rectCaption.Y + rectCaption.Height - 2);
			}

			m_buttonDockWindowClose.BackColor = m_buttonAutoHide.BackColor = (IsActivated ? SystemColors.ActiveCaption : SystemColors.Control);
			m_buttonDockWindowClose.ForeColor = m_buttonAutoHide.ForeColor = (IsActivated ? SystemColors.ActiveCaptionText : SystemColors.ControlText);
			m_buttonDockWindowClose.BorderColor = m_buttonAutoHide.BorderColor = (IsActivated ? SystemColors.ActiveCaptionText : Color.Empty);	

			m_buttonDockWindowClose.Enabled = ActiveContent.CloseButton;

			Rectangle rectCaptionText = rectCaption;
			rectCaptionText.X += MeasureToolWindowCaption.TextGapLeft;
			rectCaptionText.Width = rectCaption.Width - MeasureToolWindowCaption.ButtonGapRight
				- MeasureToolWindowCaption.ButtonGapLeft
				- MeasureToolWindowCaption.ButtonGapBetween - 2 * m_buttonDockWindowClose.Width
				- MeasureToolWindowCaption.TextGapLeft - MeasureToolWindowCaption.TextGapRight;
			rectCaptionText.Y += MeasureToolWindowCaption.TextGapTop;
			rectCaptionText.Height -= MeasureToolWindowCaption.TextGapTop + MeasureToolWindowCaption.TextGapBottom;
			Brush brush = IsActivated ? SystemBrushes.FromSystemColor(SystemColors.ActiveCaptionText)
				: SystemBrushes.FromSystemColor(SystemColors.ControlText);
			g.DrawString(CaptionText, Font, brush, rectCaptionText, StringFormatDockWindowCaption);
		}

		protected virtual void DrawTab(Graphics g, DockContent content, Rectangle rect)
		{
			if (Appearance == DockPaneAppearance.ToolWindow)
				DrawTab_ToolWindow(g, content, rect);
			else
				DrawTab_Document(g, content, rect);
		}

		private void DrawTab_ToolWindow(Graphics g, DockContent content, Rectangle rect)
		{
			Rectangle rectIcon = new Rectangle(
				rect.X + MeasureToolWindowTab.ImageGapLeft,
				rect.Y + rect.Height - 1 - MeasureToolWindowTab.ImageGapBottom - MeasureToolWindowTab.ImageHeight,
				MeasureToolWindowTab.ImageWidth, MeasureToolWindowTab.ImageHeight);
			Rectangle rectText = rectIcon;
			rectText.X += rectIcon.Width + MeasureToolWindowTab.ImageGapRight;
			rectText.Width = rect.Width - rectIcon.Width - MeasureToolWindowTab.ImageGapLeft - 
				MeasureToolWindowTab.ImageGapRight - MeasureToolWindowTab.TextGapRight;

			if (ActiveContent == content)
			{
				g.FillRectangle(SystemBrushes.Control, rect);
				g.DrawLine(SystemPens.ControlText,
					rect.X, rect.Y + rect.Height - 1, rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
				g.DrawLine(SystemPens.ControlText,
					rect.X + rect.Width - 1, rect.Y, rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
				g.DrawString(content.TabText, Font, SystemBrushes.ControlText, rectText, StringFormatDockWindowTab);
			}
			else
			{
				if (GetIndexOfVisibleContents(ActiveContent) !=
					GetIndexOfVisibleContents(content) + 1)
					g.DrawLine(SystemPens.GrayText,
						rect.X + rect.Width - 1, rect.Y + 3, rect.X + rect.Width - 1, rect.Y + rect.Height - 4);
				g.DrawString(content.TabText, Font, SystemBrushes.FromSystemColor(SystemColors.ControlDarkDark), rectText, StringFormatDockWindowTab);
			}

			if (rect.Contains(rectIcon))
				g.DrawIcon(content.Icon, rectIcon);
		}

		private void DrawTab_Document(Graphics g, DockContent content, Rectangle rect)
		{
			Rectangle rectText = rect;
			rectText.X += MeasureDocumentTab.TextExtraWidth / 2;
			rectText.Width -= MeasureDocumentTab.TextExtraWidth;
			if (ActiveContent == content)
			{
				g.FillRectangle(SystemBrushes.Control, rect);
				g.DrawLine(SystemPens.ControlText,
					rect.X + rect.Width - 1, rect.Y,
					rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
				if (IsActiveDocumentPane)
				{
					using (Font boldFont = new Font(this.Font, FontStyle.Bold))
					{
						g.DrawString(content.Text, boldFont, SystemBrushes.ControlText, rectText,StringFormatDocumentWindowTab);
					}
				}
				else
					g.DrawString(content.Text, Font, SystemBrushes.FromSystemColor(SystemColors.ControlDarkDark), rectText, StringFormatDocumentWindowTab);
			}
			else
			{
				if (GetIndexOfVisibleContents(ActiveContent) != GetIndexOfVisibleContents(content) + 1)
					g.DrawLine(SystemPens.GrayText,
						rect.X + rect.Width - 1, rect.Y,
						rect.X + rect.Width - 1, rect.Y + rect.Height - 1 - MeasureDocumentTab.TabGapTop);

				g.DrawString(content.Text, Font, SystemBrushes.FromSystemColor(SystemColors.GrayText), rectText, StringFormatDocumentWindowTab);
			}
		}

		protected virtual void DrawTabStrip(Graphics g)
		{
			if (Appearance == DockPaneAppearance.ToolWindow)
				DrawTabStrip_ToolWindow(g);
			else
				DrawTabStrip_Document(g);
		}

		private void DrawTabStrip_ToolWindow(Graphics g)
		{
			CalculateTabs();

			Rectangle rectTabStrip = GetTabStripRectangle();

			if (rectTabStrip.IsEmpty)
				return;

			// Paint the background
			g.FillRectangle(Brushes.WhiteSmoke, rectTabStrip);
			g.DrawLine(SystemPens.ControlText, rectTabStrip.Left, rectTabStrip.Top,
				rectTabStrip.Right, rectTabStrip.Top);

			for (int i=0; i<CountOfVisibleContents; i++)
				DrawTab(g, GetVisibleContent(i), GetTabRectangle(i));
		}

		private void DrawTabStrip_Document(Graphics g)
		{
			CalculateTabs();

			int countOfVisibleContents = CountOfVisibleContents;
			if (countOfVisibleContents == 0)
				return;

			Rectangle rectTabStrip = GetTabStripRectangle();

			// Paint the background
			g.FillRectangle(Brushes.WhiteSmoke, rectTabStrip);

			// Set position and size of the buttons
			int buttonWidth = ImageDocumentWindowCloseEnabled.Width;
			int buttonHeight = ImageDocumentWindowCloseEnabled.Height;
			int height = rectTabStrip.Height - MeasureDocumentTab.ButtonGapTop - MeasureDocumentTab.ButtonGapBottom;
			if (buttonHeight < height)
			{
				buttonWidth = buttonWidth * (height / buttonHeight);
				buttonHeight = height;
			}
			Size buttonSize = new Size(buttonWidth, buttonHeight);
			m_buttonDocumentWindowClose.Size = m_buttonScrollLeft.Size = m_buttonScrollRight.Size = buttonSize;
			int x = rectTabStrip.X + rectTabStrip.Width - MeasureDocumentTab.TabGapLeft
				- MeasureDocumentTab.ButtonGapRight - buttonWidth;
			int y = rectTabStrip.Y + MeasureDocumentTab.ButtonGapTop;
			m_buttonDocumentWindowClose.Location = new Point(x, y);
			Point point = m_buttonDocumentWindowClose.Location;
			point.Offset(-(MeasureDocumentTab.ButtonGapBetween + buttonWidth), 0);
			m_buttonScrollRight.Location = point;
			point.Offset(-(MeasureDocumentTab.ButtonGapBetween + buttonWidth), 0);
			m_buttonScrollLeft.Location = point;

			m_buttonDocumentWindowClose.BackColor = m_buttonScrollRight.BackColor	= m_buttonScrollLeft.BackColor = Color.WhiteSmoke;
			m_buttonDocumentWindowClose.ForeColor = m_buttonScrollRight.ForeColor	= m_buttonScrollLeft.ForeColor = SystemColors.ControlDarkDark;
			m_buttonDocumentWindowClose.BorderColor = m_buttonScrollRight.BorderColor	= m_buttonScrollLeft.BorderColor = SystemColors.ControlDarkDark;

			// Draw the tabs
			Rectangle rectTabOnly = GetTabStripRectangle(true);
			Rectangle rectTab = Rectangle.Empty;
			g.SetClip(rectTabOnly, CombineMode.Replace);
			for (int i=0; i<countOfVisibleContents; i++)
			{
				rectTab = GetTabRectangle(i);
				if (rectTab.IntersectsWith(rectTabOnly))
					DrawTab(g, GetVisibleContent(i), rectTab);
			}

			m_buttonScrollLeft.Enabled = (m_offset < 0);
			m_buttonScrollRight.Enabled = rectTab.Right > rectTabOnly.Right;
			m_buttonDocumentWindowClose.Enabled = ActiveContent.CloseButton;
		}

		private void EnsureTabVisible(DockContent content)
		{
			if (Appearance != DockPaneAppearance.Document)
				return;

			CalculateTabs();

			Rectangle rectTabStrip = GetTabStripRectangle(true);
			Rectangle rectTab = GetTabRectangle(GetIndexOfVisibleContents(content));

			if (rectTab.Right > rectTabStrip.Right)
			{
				m_offset -= rectTab.Right - rectTabStrip.Right;
				rectTab.X -= rectTab.Right - rectTabStrip.Right;
			}

			if (rectTab.Left < rectTabStrip.Left)
				m_offset += rectTabStrip.Left - rectTab.Left;

			Invalidate();
		}

		private HitTestResult GetHitTest()
		{
			return GetHitTest(Control.MousePosition);
		}

		protected HitTestResult GetHitTest(Point ptMouse)
		{
			HitTestResult hitTestResult = new HitTestResult(HitTestArea.None, -1);

			ptMouse = PointToClient(ptMouse);

			Rectangle rectCaption = CaptionRectangle;
			if (rectCaption.Contains(ptMouse))
				return new HitTestResult(HitTestArea.Caption, -1);

			Rectangle rectContent = ContentRectangle;
			if (rectContent.Contains(ptMouse))
				return new HitTestResult(HitTestArea.Content, -1);

			Rectangle rectTabStrip = GetTabStripRectangle(true);
			if (rectTabStrip.Contains(ptMouse))
			{
				for (int i=0; i<CountOfVisibleContents; i++)
				{
					Rectangle rectTab = GetTabRectangle(i);
					rectTab.Intersect(rectTabStrip);
					if (rectTab.Contains(ptMouse))
						return new HitTestResult(HitTestArea.TabStrip, i);
				}
				return new HitTestResult(HitTestArea.TabStrip, -1);
			}

			return new HitTestResult(HitTestArea.None, -1);
		}


		internal int GetIndexOfVisibleContents(DockContent content)
		{
			if (content == null)
				return -1;

			if (content.IsHidden)
				return -1;

			int index = -1;
			foreach (DockContent c in Contents)
			{
				if (!c.IsHidden)
					index++;

				if (c == content)
					return index;
			}
			return -1;
		}

		private int GetTabOriginalWidth(int index)
		{
			if (Appearance == DockPaneAppearance.ToolWindow)
				return GetTabOriginalWidth_ToolWindow(index);
			else
				return GetTabOriginalWidth_Document(index);
		}

		private int GetTabOriginalWidth_ToolWindow(int index)
		{
			DockContent content = GetVisibleContent(index);
			using (Graphics g = CreateGraphics())
			{
				SizeF sizeString = g.MeasureString(content.TabText, Font);
				return MeasureToolWindowTab.ImageWidth + (int)sizeString.Width + 1 + MeasureToolWindowTab.ImageGapLeft
					+ MeasureToolWindowTab.ImageGapRight + MeasureToolWindowTab.TextGapRight;
			}
		}

		private int GetTabOriginalWidth_Document(int index)
		{
			DockContent content = GetVisibleContent(index);

			using (Graphics g = CreateGraphics())
			{
				SizeF sizeText;
				if (content == ActiveContent && IsActiveDocumentPane)
				{
					using (Font boldFont = new Font(this.Font, FontStyle.Bold))
					{
						sizeText = g.MeasureString(content.Text, boldFont, MeasureDocumentTab.TabMaxWidth, StringFormatDocumentWindowTab);
					}
				}
				else
					sizeText = g.MeasureString(content.Text, Font, MeasureDocumentTab.TabMaxWidth, StringFormatDocumentWindowTab);

				return (int)sizeText.Width + 1 + MeasureDocumentTab.TextExtraWidth;
			}
		}

		private Rectangle GetTabRectangle(int index)
		{
			if (Appearance == DockPaneAppearance.ToolWindow)
				return GetTabRectangle_ToolWindow(index);
			else
				return GetTabRectangle_Document(index);
		}

		private Rectangle GetTabRectangle_ToolWindow(int index)
		{
			Rectangle rectTabStrip = GetTabStripRectangle();

			DockContent content = GetVisibleContent(index);
			return new Rectangle(content.TabX, rectTabStrip.Y, content.TabWidth, rectTabStrip.Height);
		}

		private Rectangle GetTabRectangle_Document(int index)
		{
			Rectangle rectTabStrip = GetTabStripRectangle();
			DockContent content = GetVisibleContent(index);

			return new Rectangle(content.TabX, rectTabStrip.Y + MeasureDocumentTab.TabGapTop,
				content.TabWidth, rectTabStrip.Height - MeasureDocumentTab.TabGapTop);
		}

		private Rectangle GetTabStripRectangle()
		{
			return GetTabStripRectangle(false);
		}

		private Rectangle GetTabStripRectangle(bool tabOnly)
		{
			if (Appearance == DockPaneAppearance.ToolWindow)
				return GetTabStripRectangle_ToolWindow(tabOnly);
			else
				return GetTabStripRectangle_Document(tabOnly);
		}

		protected Rectangle GetTabStripRectangle_ToolWindow(bool tabOnly)
		{
			if (CountOfVisibleContents <= 1 || IsAutoHide)
				return Rectangle.Empty;

			Rectangle rectWindow = DisplayingRectangle;

			int width = rectWindow.Width;
			int height = Math.Max(Font.Height, MeasureToolWindowTab.ImageHeight)
				+ MeasureToolWindowTab.ImageGapTop
				+ MeasureToolWindowTab.ImageGapBottom;
			int x = rectWindow.X;
			int y = rectWindow.Bottom - height;
			Rectangle rectCaption = CaptionRectangle;
			if (rectCaption.Contains(x, y))
				y = rectCaption.Y + rectCaption.Height;

			return new Rectangle(x, y, width, height);
		}

		protected Rectangle GetTabStripRectangle_Document(bool tabOnly)
		{
			if (CountOfVisibleContents == 0)
				return Rectangle.Empty;

			Rectangle rectWindow = DisplayingRectangle;
			int x = rectWindow.X;
			int y = rectWindow.Y;
			int width = rectWindow.Width;
			int height = Math.Max(Font.Height + MeasureDocumentTab.TabGapTop + MeasureDocumentTab.TextExtraHeight,
				ImageDocumentWindowCloseEnabled.Height + MeasureDocumentTab.ButtonGapTop + MeasureDocumentTab.ButtonGapBottom);

			if (tabOnly)
			{
				x += MeasureDocumentTab.TabGapLeft;
				width -= MeasureDocumentTab.TabGapLeft + 
					MeasureDocumentTab.TabGapRight +
					MeasureDocumentTab.ButtonGapRight +
					m_buttonDocumentWindowClose.Width +
					m_buttonScrollRight.Width +
					m_buttonScrollLeft.Width +
					2 * MeasureDocumentTab.ButtonGapBetween;
			}

			return new Rectangle(x, y, width, height);
		}

		private Region GetTestDropOutline(DockStyle dockStyle, int contentIndex)
		{
			if (Appearance == DockPaneAppearance.ToolWindow)
				return GetTestDropOutline_ToolWindow(dockStyle, contentIndex);
			else
				return GetTestDropOutline_Document(dockStyle, contentIndex);
		}

		private Region GetTestDropOutline_ToolWindow(DockStyle dock, int contentIndex)
		{
			int dragSize = MeasurePane.DragSize;

			if (dock != DockStyle.Fill)
			{
				Rectangle rect = DisplayingRectangle;
				if (dock == DockStyle.Right)
					rect.X += rect.Width / 2;
				if (dock == DockStyle.Bottom)
					rect.Y += rect.Height / 2;
				if (dock == DockStyle.Left || dock == DockStyle.Right)
					rect.Width -= rect.Width / 2;
				if (dock == DockStyle.Top || dock == DockStyle.Bottom)
					rect.Height -= rect.Height / 2;
				rect.Location = PointToScreen(rect.Location);

				return DrawHelper.CreateDragOutline(rect, dragSize);
			}
			else
			{
				Rectangle[] rects = new Rectangle[3];
				rects[2] = Rectangle.Empty;
				rects[0] = DisplayingRectangle;
				if (contentIndex != -1)
					rects[1] = GetTabRectangle(contentIndex);
				else
					rects[1] = Rectangle.Empty;

				if (!rects[1].IsEmpty)
				{
					rects[0].Height = rects[1].Top - rects[0].Top;
					rects[2].X = rects[1].X + dragSize;
					rects[2].Y = rects[1].Y - dragSize;
					rects[2].Width = rects[1].Width - 2 * dragSize;
					rects[2].Height = 2 * dragSize;
				}

				rects[0].Location = PointToScreen(rects[0].Location);
				rects[1].Location = PointToScreen(rects[1].Location);
				rects[2].Location = PointToScreen(rects[2].Location);
				return DrawHelper.CreateDragOutline(rects, dragSize);
			}
		}

		private Region GetTestDropOutline_Document(DockStyle dock, int contentIndex)
		{
			int dragSize = MeasurePane.DragSize;

			Rectangle[] rects = new Rectangle[3];

			rects[0] = DisplayingRectangle;
			if (dock == DockStyle.Right)
				rects[0].X += rects[0].Width / 2;
			else if (dock == DockStyle.Bottom)
				rects[0].Y += rects[0].Height / 2;
			if (dock == DockStyle.Left || dock == DockStyle.Right)
				rects[0].Width -= rects[0].Width / 2;
			else if (dock == DockStyle.Top || dock == DockStyle.Bottom)
				rects[0].Height -= rects[0].Height / 2;

			if (dock != DockStyle.Fill)
				rects[1] = new Rectangle(rects[0].X + MeasureDocumentTab.TabGapLeft, rects[0].Y,
					rects[0].Width - 2 * MeasureDocumentTab.TabGapLeft, GetTabStripRectangle().Height);
			else if (contentIndex != -1)
				rects[1] = GetTabRectangle(contentIndex);
			else
				rects[1] = GetTabRectangle(0);

			rects[0].Y = rects[1].Top + rects[1].Height;
			rects[0].Height -= rects[1].Height;
			rects[2] = new Rectangle(rects[1].X + dragSize, rects[0].Y - dragSize,
				rects[1].Width - 2 * dragSize, 2 * dragSize);

			rects[0].Location = PointToScreen(rects[0].Location);
			rects[1].Location = PointToScreen(rects[1].Location);
			rects[2].Location = PointToScreen(rects[2].Location);
			return DrawHelper.CreateDragOutline(rects, dragSize);
		}

		public DockContent GetVisibleContent(int index)
		{
			int currentIndex = -1;
			foreach (DockContent content in Contents)
			{
				if (!content.IsHidden)
					currentIndex ++;

				if (currentIndex == index)
					return content;
			}
			throw(new ArgumentOutOfRangeException());
		}

		public new void Hide()
		{
			IsHidden = true;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			SetInertButtons();

			Rectangle rectContent = ContentRectangle;

			foreach (DockContent content in Contents)
			{
				content.Parent = this;
				content.Visible = (content == ActiveContent);
				content.Bounds = rectContent;
			}

			base.OnLayout(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			HitTestResult hitTestResult = GetHitTest();
			HitTestArea hitArea = hitTestResult.HitArea;
			int index = hitTestResult.Index;
			string toolTip = string.Empty;

			base.OnMouseMove(e);

			if (hitArea == HitTestArea.TabStrip && index != -1)
			{
				Rectangle rectTab = GetTabRectangle(index);
				if (GetVisibleContent(index).ToolTipText != null)
					toolTip = GetVisibleContent(index).ToolTipText;
				else if (rectTab.Width < GetTabOriginalWidth(index))
					toolTip = GetVisibleContent(index).TabText;
			}

			if (m_toolTip.GetToolTip(this) != toolTip)
			{
				m_toolTip.Active = false;
				m_toolTip.SetToolTip(this, toolTip);
				m_toolTip.Active = true;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (ActiveContent != null)
			{
				SetInertButtons();
				DrawCaption(e.Graphics);
				DrawTabStrip(e.Graphics);
				PerformLayout();
			}

			base.OnPaint(e);
		}

		public override void Refresh()
		{
			base.Refresh();
			if (DockHelper.IsDockStateAutoHide(DockState) && DockPanel != null)
			{
				DockPanel.Invalidate();
				DockPanel.PerformLayout();
			}
		}

		internal void RemoveContent(DockContent content)
		{
			if (!Contents.Contains(content))
				return;
			
			int index = GetIndexOfVisibleContents(content);

			Contents.Remove(content);
			content.SetParent(null);
			SetDockState();
			ValidateActiveContent();

			if (Contents.Count == 0)
				Dispose();

			if (index != -1)
				Invalidate();
		}

		private void ScrollLeft_Click(object sender, EventArgs e)
		{
			Rectangle rectTabStrip = GetTabStripRectangle(true);

			int index;
			for (index=0; index<CountOfVisibleContents; index++)
				if (GetTabRectangle(index).IntersectsWith(rectTabStrip))
					break;

			Rectangle rectTab = GetTabRectangle(index);
			if (rectTab.Left < rectTabStrip.Left)
				m_offset += rectTabStrip.Left - rectTab.Left;
			else if (index == 0)
				m_offset = 0;
			else
				m_offset += rectTabStrip.Left - GetTabRectangle(index - 1).Left;

			Invalidate();
		}
	
		private void ScrollRight_Click(object sender, EventArgs e)
		{
			Rectangle rectTabStrip = GetTabStripRectangle(true);

			int index;
			int countOfVisibleContents = CountOfVisibleContents;
			for (index=0; index<countOfVisibleContents; index++)
				if (GetTabRectangle(index).IntersectsWith(rectTabStrip))
					break;

			if (index + 1 < countOfVisibleContents)
			{
				m_offset -= GetTabRectangle(index + 1).Left - rectTabStrip.Left;
				CalculateTabs();
			}

			Rectangle rectLastTab = GetTabRectangle(countOfVisibleContents - 1);
			if (rectLastTab.Right < rectTabStrip.Right)
				m_offset += rectTabStrip.Right - rectLastTab.Right;

			Invalidate();
		}

		private void SetInertButtons()
		{
			if (DockState == DockState.Document)
			{
				m_buttonAutoHide.Visible =	m_buttonDockWindowClose.Visible = false;
				m_buttonScrollLeft.Visible = m_buttonScrollRight.Visible = m_buttonDocumentWindowClose.Visible = true;
			}
			else if (DockState == DockState.Float)
			{
				m_buttonAutoHide.Visible = m_buttonScrollLeft.Visible = m_buttonScrollRight.Visible = m_buttonDocumentWindowClose.Visible = false;
				m_buttonDockWindowClose.Visible = FloatWindow.DisplayingList.Count > 1;
			}
			else
			{
				m_buttonAutoHide.Visible =	m_buttonDockWindowClose.Visible = true;
				m_buttonScrollLeft.Visible = m_buttonScrollRight.Visible = m_buttonDocumentWindowClose.Visible = false;
			}
		
			// set the size and location for close and auto-hide buttons
			Rectangle rectCaption = CaptionRectangle;
			int buttonWidth = ImageDockWindowCloseEnabled.Width;
			int buttonHeight = ImageDockWindowCloseEnabled.Height;
			int height = rectCaption.Height - MeasureToolWindowCaption.ButtonGapTop - MeasureToolWindowCaption.ButtonGapBottom;
			if (buttonHeight < height)
			{
				buttonWidth = buttonWidth * (height / buttonHeight);
				buttonHeight = height;
			}
			m_buttonDockWindowClose.SuspendLayout();
			m_buttonAutoHide.SuspendLayout();
			Size buttonSize = new Size(buttonWidth, buttonHeight);
			m_buttonDockWindowClose.Size = m_buttonAutoHide.Size = buttonSize;
			int x = rectCaption.X + rectCaption.Width - 1 - MeasureToolWindowCaption.ButtonGapRight - m_buttonDockWindowClose.Width;
			int y = rectCaption.Y + MeasureToolWindowCaption.ButtonGapTop;
			Point point = m_buttonDockWindowClose.Location = new Point(x, y);
			point.Offset(-(m_buttonAutoHide.Width + MeasureToolWindowCaption.ButtonGapBetween), 0);
			m_buttonAutoHide.Location = point;
			m_buttonAutoHide.ImageEnabled = IsAutoHide ? ImageAutoHideYes : ImageAutoHideNo;
			m_buttonDockWindowClose.ResumeLayout();
			m_buttonAutoHide.ResumeLayout();
		}

		public void SetContentIndex(DockContent content, int index)
		{
			int oldIndex = Contents.IndexOf(content);
			if (oldIndex == -1)
				throw(new ArgumentException(ResourceHelper.GetString("DockPane.SetContentIndex.InvalidContent")));

			if (index < 0 || index > Contents.Count - 1)
				if (index != -1)
					throw(new ArgumentOutOfRangeException(ResourceHelper.GetString("DockPane.SetContentIndex.InvalidIndex")));
				
			if (oldIndex == index)
				return;
			if (oldIndex == Contents.Count - 1 && index == -1)
				return;

			Contents.Remove(content);
			if (index == -1)
				Contents.Add(content);
			else if (oldIndex < index)
				Contents.AddAt(content, index - 1);
			else
				Contents.AddAt(content, index);

			Refresh();
		}

		private void SetParent()
		{
			if (DockState == DockState.Unknown || DockState == DockState.Hidden)
			{
				SetParent(DockPanel.DummyControl);
				Splitter.Parent = DockPanel.DummyControl;
			}
			else if (DockState == DockState.Float)
			{
				SetParent(FloatWindow);
				Splitter.Parent = FloatWindow;
			}
			else if (DockHelper.IsDockStateAutoHide(DockState))
			{
				SetParent(DockPanel.AutoHideWindow);
				Splitter.Parent = DockPanel.DummyControl;
			}
			else
			{
				SetParent(DockPanel.DockWindows[DockState]);
				Splitter.Parent = this.Parent;
			}
		}

		private void SetParent(Control value)
		{
			if (Parent == value)
				return;

			Control oldParent = Parent;

			///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			/// Workaround for .Net Framework bug: removing control from Form may cause form
			/// unclosable. Set focus to another dummy control.
			///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			Form form = FindForm();
			if (ContainsFocus)
			{
				if (form is FloatWindow)
				{
					((FloatWindow)form).DummyControl.Focus();
					form.ActiveControl = ((FloatWindow)form).DummyControl;
				}
				else if (DockPanel != null)
				{
					DockPanel.DummyControl.Focus();
					if (form != null)
						form.ActiveControl = DockPanel.DummyControl;
				}
			}
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			Parent = value;
			m_splitter.Parent = value;
		}

		public new void Show()
		{
			IsHidden = false;
			Activate();
		}

		internal void TestDrop(DragHandler dragHandler, Point pt)
		{
			if (DockState == DockState.Document)
				DockPanel.TestDrop(dragHandler, pt);

			if (dragHandler.DropTarget.DropTo != null)
				return;

			if (DockHelper.IsDockStateAutoHide(DockState))
				return;

			if (!dragHandler.IsDockStateValid(DockState))
				return;

			if (dragHandler.DragSource == DragSource.FloatWindow &&
				FloatWindow == dragHandler.DragControl)
				return;

			if (dragHandler.DragSource == DragSource.Pane &&
				dragHandler.DragControl == this)
				return;

			if (dragHandler.DragSource == DragSource.Content && 
				dragHandler.DragControl == this &&
				DockState == DockState.Document &&
				CountOfVisibleContents == 1)
				return;

			Point ptClient = PointToClient(pt);
			Rectangle rectPane = DisplayingRectangle;
			int dragSize = MeasurePane.DragSize;
			if (ptClient.Y - rectPane.Top >= 0 && ptClient.Y - rectPane.Top < dragSize)
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Top);
			else if (rectPane.Bottom - ptClient.Y >= 0 && rectPane.Bottom - ptClient.Y < dragSize)
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Bottom);
			else if (rectPane.Right - ptClient.X >= 0 && rectPane.Right - ptClient.X < dragSize)
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Right);
			else if (ptClient.X - rectPane.Left >= 0 && ptClient.X - rectPane.Left < dragSize)
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Left);
			else
			{
				if (rectPane.Height <= GetTabStripRectangle().Height)
					return;

				HitTestResult hitTestResult = GetHitTest(pt);
				if (hitTestResult.HitArea == HitTestArea.Caption)
					dragHandler.DropTarget.SetDropTarget(this, -1);
				else if (hitTestResult.HitArea == HitTestArea.TabStrip && hitTestResult.Index != -1)
					dragHandler.DropTarget.SetDropTarget(this, hitTestResult.Index);
				else if (DockState == DockState.Float && !HasCaption &&
					((ptClient.Y - rectPane.Top >= dragSize && ptClient.Y - rectPane.Top < 2 * dragSize) ||
					(rectPane.Bottom - ptClient.Y >= dragSize && rectPane.Bottom - ptClient.Y < 2 * dragSize) ||
					(rectPane.Right - ptClient.X >= dragSize && rectPane.Right - ptClient.X < 2 * dragSize) ||
					(ptClient.X - rectPane.Left >= dragSize && ptClient.X - rectPane.Left < 2 * dragSize)))
					dragHandler.DropTarget.SetDropTarget(this, -1);
				else
					return;
			}

			if (dragHandler.DropTarget.SameAsOldValue)
				return;

			dragHandler.DragOutline = GetTestDropOutline(dragHandler.DropTarget.Dock, dragHandler.DropTarget.ContentIndex);
		}

		internal void ValidateActiveContent()
		{
			if (ActiveContent == null)
				return;

			if (GetIndexOfVisibleContents(ActiveContent) >= 0)
				return;

			DockContent prevVisible = null;
			for (int i=Contents.IndexOf(ActiveContent)-1; i>=0; i--)
				if (!Contents[i].IsHidden)
				{
					prevVisible = Contents[i];
					break;
				}

			DockContent nextVisible = null;
			for (int i=Contents.IndexOf(ActiveContent)+1; i<Contents.Count; i++)
				if (!Contents[i].IsHidden)
				{
					nextVisible = Contents[i];
					break;
				}

			if (prevVisible != null)
				ActiveContent = prevVisible;
			else if (nextVisible != null)
				ActiveContent = nextVisible;
			else
				ActiveContent = null;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == (int)Win32.Msgs.WM_MOUSEACTIVATE)
			{
				Activate();
				base.WndProc(ref m);
				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDOWN)
			{
				HitTestResult hitTestResult = GetHitTest();
				if (hitTestResult.HitArea == HitTestArea.TabStrip && hitTestResult.Index != -1)
				{
					DockContent content = GetVisibleContent(hitTestResult.Index);
					if (ActiveContent != content)
					{
						ActiveContent = content;
						Update();
					}
					if (DockPanel.AllowRedocking && AllowRedocking && ActiveContent.AllowRedocking)
						DockPanel.DragHandler.BeginDragContent(this, DisplayingRectangle);
				}
				else if (hitTestResult.HitArea == HitTestArea.Caption &&
					DockPanel.AllowRedocking && AllowRedocking &&
					!DockHelper.IsDockStateAutoHide(DockState))
					DockPanel.DragHandler.BeginDragPane(this, CaptionRectangle.Location);
				else
					base.WndProc(ref m);
				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_RBUTTONDOWN)
			{
				HitTestResult hitTestResult = GetHitTest();
				if (hitTestResult.HitArea == HitTestArea.TabStrip && hitTestResult.Index != -1)
				{
					DockContent content = 
						GetVisibleContent(hitTestResult.Index);
					if (ActiveContent != content)
					{
						ActiveContent = content;
						Update();
					}
					if (content.TabPageContextMenu != null) 
					{
						content.TabPageContextMenu.Show(this, this.PointToClient(Control.MousePosition));
					}
				}
			}
			else if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
			{
				base.WndProc(ref m);

				HitTestResult hitTestResult = GetHitTest();
				if (hitTestResult.HitArea == HitTestArea.Caption)
				{

					if (DockHelper.IsDockStateAutoHide(DockState))
					{
						DockPanel.ActiveAutoHideContent = null;
						return;
					}

					if (DockPanel.AllowRedocking && DockHelper.IsDockStateDocked(DockState) && IsDockStateValid(DockState.Float))
					{
						DockState = DockState.Float;
						Activate();
					}
					else if (DockPanel.AllowRedocking && DockState == DockState.Float && DockWindow != null && IsDockStateValid(DockWindow.DockState))
					{
						DockState = DockWindow.DockState;
						Activate();
					}
				}
				else if (DockPanel.AllowRedocking && hitTestResult.HitArea == HitTestArea.TabStrip && hitTestResult.Index != -1)
				{
					DockContent content = GetVisibleContent(hitTestResult.Index);
					if (content.IsDockStateValid(DockState.Float))
					{
						DockState dockState = DockState;
						content.DockState = DockState.Float;
						if (content.Pane != this)
						{
							DockAlignment alignment = (dockState == DockState.DockLeft || dockState == DockState.DockRight) ? DockAlignment.Bottom : DockAlignment.Right;
							content.Pane.AddToDockList(DockListContainer, this, alignment, 0.5);
						}
					}
				}

				return;
			}

			base.WndProc(ref m);
			return;
		}

		private static readonly object DockStateChangedEvent = new object();
		public event EventHandler DockStateChanged
		{
			add	{	Events.AddHandler(DockStateChangedEvent, value);	}
			remove	{	Events.RemoveHandler(DockStateChangedEvent, value);	}
		}
		protected virtual void OnDockStateChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[DockStateChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object IsActivatedChangedEvent = new object();
		public event EventHandler IsActivatedChanged
		{
			add	{	Events.AddHandler(IsActivatedChangedEvent, value);	}
			remove	{	Events.RemoveHandler(IsActivatedChangedEvent, value);	}
		}
		protected virtual void OnIsActivatedChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[IsActivatedChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object IsActiveDocumentPaneChangedEvent = new object();
		public event EventHandler IsActiveDocumentPaneChanged
		{
			add	{	Events.AddHandler(IsActiveDocumentPaneChangedEvent, value);	}
			remove	{	Events.RemoveHandler(IsActiveDocumentPaneChangedEvent, value);	}
		}
		protected virtual void OnIsActiveDocumentPaneChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[IsActiveDocumentPaneChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		public DockWindow DockWindow
		{
			get	{	return (m_nestedDockingNormal.DockList == null) ? null : m_nestedDockingNormal.DockList.Container as DockWindow;	}
			set
			{
				DockWindow oldValue = DockWindow;
				if (oldValue == value)
					return;

				if (value != null)
					AddToDockList(value);
				else if (oldValue != null)
					RemoveFromDockList(oldValue);
			}
		}

		public FloatWindow FloatWindow
		{
			get	{	return (m_nestedDockingFloat.DockList == null) ? null : m_nestedDockingFloat.DockList.Container as FloatWindow;	}
			set
			{
				FloatWindow oldValue = FloatWindow;
				if (oldValue == value)
					return;

				if (value != null)
					AddToDockList(value);
				else if (oldValue != null)
					RemoveFromDockList(oldValue);
			}
		}

		public NestedDockingStatus GetNestedDockingStatus(bool isFloat)
		{
			return isFloat ? m_nestedDockingFloat : m_nestedDockingNormal;
		}

		public NestedDockingStatus NestedDockingStatus
		{
			get
			{
				if (DockState == DockState.Float)
					return m_nestedDockingFloat;
				else if (DockHelper.IsDockWindowState(DockState))
					return m_nestedDockingNormal;
				else
					return null;
			}
		}
		
		public IDockListContainer DockListContainer
		{
			get
			{
				if (DockState == DockState.Float)
					return m_nestedDockingFloat.DockList.Container;
				else if (DockHelper.IsDockWindowState(DockState))
					return m_nestedDockingNormal.DockList.Container;
				else
					return null;
			}
		}

		private DockState m_visibleState = DockState.Unknown;
		public DockState VisibleState
		{
			get	{	return m_visibleState;	}
			set
			{
				if (m_visibleState == value)
					return;

				if (value == DockState.Unknown || value == DockState.Hidden)
					throw new InvalidOperationException(ResourceHelper.GetString("DockPane.VisibleState.InvalidState"));

				m_visibleState = value;
				SetDockState();
			}
		}
		private DockState m_dockState = DockState.Unknown;
		public DockState DockState
		{
			get	{	return m_dockState;	}
			set
			{
				if (m_dockState == value)
					return;

				if (value == DockState.Unknown)
					throw new InvalidOperationException(ResourceHelper.GetString("DockPane.VisibleState.InvalidState"));
				if (value == DockState.Hidden)
					IsHidden = true;
				else
				{
					m_isHidden = false;
					m_visibleState = value;
					SetDockState();
				}
			}
		}
		internal void SetDockState()
		{
			DockState value, oldDockState;

			if (CountOfVisibleContents == 0)
				value = DockState.Unknown;
			else if (IsHidden)
				value = DockState.Hidden;
			else
				value = m_visibleState;

			if (m_dockState == value)
				return;

			oldDockState = m_dockState;
			FloatWindow oldFloatWindow = oldDockState == DockState.Float ? FloatWindow : null;
			m_dockState = value;

			if (DockState == DockState.Float && FloatWindow == null)
				DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this);

			if (DockHelper.IsDockWindowState(DockState) && (DockWindow == null || DockWindow.DockState != DockState))
				DockWindow = DockPanel.DockWindows[DockState];

			SetParent();

			foreach (DockContent content in Contents)
				content.SetDockState();

			if (oldFloatWindow != null && !oldFloatWindow.IsDisposed)
				oldFloatWindow.PerformLayout();
			else if (DockHelper.IsDockStateAutoHide(oldDockState))
				DockPanel.AutoHideWindow.RefreshActiveContent();
			else if (DockHelper.IsDockWindowState(oldDockState))
				DockPanel.DockWindows[oldDockState].PerformLayout();

			if (DockState == DockState.Float)
				FloatWindow.PerformLayout();
			else if (DockHelper.IsDockStateAutoHide(DockState))
				DockPanel.AutoHideWindow.RefreshActiveContent();
			else if (DockHelper.IsDockWindowState(DockState))
				DockPanel.DockWindows[DockState].PerformLayout();

			if (DockHelper.IsDockStateAutoHide(oldDockState) ||
				DockHelper.IsDockStateAutoHide(DockState))
			{
				DockPanel.Invalidate();
				DockPanel.PerformLayout();
			}

			DockPanel.RefreshActiveWindow();

			//*** Use PostMessage instead
			User32.PostMessage(this.Handle, WM_DOCKSTATECHANGED, 0, 0);
			OnDockStateChanged(EventArgs.Empty);
		}

		public void AddToDockList(IDockListContainer container)
		{
			if (container == null)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullContainer"));

			DockAlignment alignment;
			if (container.DockState == DockState.DockLeft || container.DockState == DockState.DockRight)
				alignment = DockAlignment.Bottom;
			else
				alignment = DockAlignment.Right;

			DockPane prevPane = null;
			for (int i=container.DockList.Count-1; i>=0; i++)
				if (container.DockList[i] != this)
				{
					prevPane = container.DockList[i];
					break;
				}
			AddToDockList(container, prevPane, alignment, 0.5);
		}

		public void AddToDockList(IDockListContainer container, DockPane prevPane, DockAlignment alignment, double proportion)
		{
			if (container == null)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullContainer"));

			int count = container.DockList.Count;
			if (container.DockList.Contains(this))
				count --;
			if (prevPane == null && count > 0)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullPrevPane"));

			if (prevPane != null && !container.DockList.Contains(prevPane))
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NoPrevPane"));

			if (prevPane == this)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.SelfPrevPane"));

			IDockListContainer oldContainer = (container.DockState == DockState.Float)
				? FloatWindow as IDockListContainer : DockWindow as IDockListContainer;
			if (oldContainer != null)
				oldContainer.DockList.Remove(this);
			container.DockList.Add(this);
			NestedDockingStatus status = GetNestedDockingStatus(container.DockState == DockState.Float);
			status.SetStatus(container.DockList, prevPane, alignment, proportion);

			DockState oldState = DockState;
			if (oldContainer != null && oldContainer.DockState == oldState)
					DockState = container.DockState;

			if (oldState == DockState.Float && DockState == DockState.Float && oldContainer != container)
				SetParent();

			if (oldContainer != null)
			{	
				if (oldContainer.DockState == DockState.Float && oldContainer.DockList.Count == 0)
					((FloatWindow)oldContainer).Close();
				else if (oldState == oldContainer.DockState)
					((Control)oldContainer).PerformLayout();
			}

			if (container != null && container != oldContainer)
				((Control)container).PerformLayout();
		}

		public void RemoveFromDockList(IDockListContainer container)
		{
			if (!container.DockList.Contains(this))
				return;

			container.DockList.Remove(this);
			if (DockState == container.DockState)
				DockState = DockState.Unknown;
			if (container.DockState == DockState.Float && container.DockList.Count == 0)
				((FloatWindow)container).Close();
			else
				((Control)container).PerformLayout();
		}

		public void SetNestedDockingProportion(double proportion)
		{
			SetNestedDockingProportion(proportion, DockState == DockState.Float);
		}

		public void SetNestedDockingProportion(double proportion, bool isFloat)
		{
			NestedDockingStatus status = this.GetNestedDockingStatus(isFloat);
			status.SetStatus(status.DockList, status.PrevPane, status.Alignment, proportion);
			if ((this.DockState == DockState.Float) == isFloat)
				((Control)this.DockListContainer).PerformLayout();
		}
	}
}
