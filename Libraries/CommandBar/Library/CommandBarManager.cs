// ---------------------------------------------------------
// Windows Forms CommandBar Control
// Copyright (C) 2001-2003 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder
// roeder@aisto.com
// ---------------------------------------------------------
namespace Reflector.UserInterface
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.Runtime.InteropServices;
	using System.Security.Permissions;
	using System.Windows.Forms;

	public class CommandBarManager : Control
	{
		private CommandBarCollection commandBars;
	
		public CommandBarManager()
		{
			this.SetStyle(ControlStyles.UserPaint, false);
			this.TabStop = false;
			this.Dock = DockStyle.Top;
			this.commandBars = new CommandBarCollection(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.commandBars = null;
			}

			base.Dispose(disposing);
		}
	
		public CommandBarCollection CommandBars
		{
			get 
			{ 
				return this.commandBars; 
			}
		}
	
		protected override Size DefaultSize
		{
			get 
			{ 
				return new Size(100, 22 * 2); 
			}
		}

		protected override void CreateHandle()
		{
			if (!this.RecreatingHandle)
			{
				NativeMethods.INITCOMMONCONTROLSEX init = new NativeMethods.INITCOMMONCONTROLSEX();
				init.Size = Marshal.SizeOf(typeof(NativeMethods.INITCOMMONCONTROLSEX));
				init.Flags = NativeMethods.ICC_BAR_CLASSES | NativeMethods.ICC_COOL_CLASSES;
				NativeMethods.InitCommonControlsEx(init);
			}

			base.CreateHandle();
		}

		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)] 
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ClassName = NativeMethods.REBARCLASSNAME;
				createParams.Style = NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE | NativeMethods.WS_CLIPCHILDREN | NativeMethods.WS_CLIPSIBLINGS;
				createParams.Style |= NativeMethods.CCS_NODIVIDER | NativeMethods.CCS_NOPARENTALIGN | NativeMethods.CCS_NORESIZE;
				createParams.Style |= NativeMethods.RBS_VARHEIGHT | NativeMethods.RBS_BANDBORDERS | NativeMethods.RBS_AUTOSIZE;
				return createParams;
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			this.ReleaseBands();
			
			this.BeginUpdate();
	
			for (int i = 0; i < this.commandBars.Count; i++)
			{
				NativeMethods.REBARBANDINFO bandInfo = this.CreateBandInfo(i);
				NativeMethods.SendMessage(this.Handle, NativeMethods.RB_INSERTBAND, i, ref bandInfo);
			}

			this.UpdateSize();

			this.EndUpdate();
			
			this.CaptureBands();
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)] 
		protected override void WndProc(ref Message message)
		{
			base.WndProc(ref message);

			switch (message.Msg)
			{
				case NativeMethods.WM_NOTIFY:
				case NativeMethods.WM_NOTIFY + NativeMethods.WM_REFLECT:
				{
					NativeMethods.NMHDR note = (NativeMethods.NMHDR)message.GetLParam(typeof(NativeMethods.NMHDR));
					switch (note.code)
					{
						case NativeMethods.RBN_HEIGHTCHANGE:
							this.UpdateSize();
							break;

						case NativeMethods.RBN_CHEVRONPUSHED:
							this.NotifyChevronPushed(ref message);
							break;
					}
				}
					break;
			}
		}
	
		private void NotifyChevronPushed(ref Message message)
		{
			NativeMethods.NMREBARCHEVRON nrch = (NativeMethods.NMREBARCHEVRON)message.GetLParam(typeof(NativeMethods.NMREBARCHEVRON));
			int index = nrch.wID - 0xEB00;
			if ((index < this.commandBars.Count) && (this.commandBars[index] != null))
			{
				Point point = new Point(nrch.rc.left, nrch.rc.bottom);
				this.commandBars[index].Show(this, point);
			}
		}

		private void BeginUpdate()
		{
			NativeMethods.SendMessage(Handle, NativeMethods.WM_SETREDRAW, 0, 0);
		}

		private void EndUpdate()
		{
			NativeMethods.SendMessage(Handle, NativeMethods.WM_SETREDRAW, 1, 0);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)] 
		public override bool PreProcessMessage(ref Message msg)
		{
			foreach (CommandBar commandBar in this.commandBars)
			{
				if (commandBar.PreProcessMessage(ref msg))
				{
					return true;
				}
			}

			return false;
		}

		private void UpdateBand(CommandBar commandBar)
		{
			if (this.IsHandleCreated)
			{
				this.BeginUpdate();

				for (int i = 0; i < this.commandBars.Count; i++)
				{
					NativeMethods.REBARBANDINFO rbbi = new NativeMethods.REBARBANDINFO();
					rbbi.cbSize = Marshal.SizeOf(typeof(NativeMethods.REBARBANDINFO));
					rbbi.fMask = NativeMethods.RBBIM_STYLE | NativeMethods.RBBIM_ID | NativeMethods.RBBIM_TEXT | NativeMethods.RBBIM_STYLE | NativeMethods.RBBIM_CHILD | NativeMethods.RBBIM_SIZE | NativeMethods.RBBIM_CHILDSIZE | NativeMethods.RBBIM_IDEALSIZE;
					NativeMethods.SendMessage(this.Handle, NativeMethods.RB_GETBANDINFO, i, ref rbbi);

					if (commandBar.Handle == rbbi.hwndChild)
					{
						if ((rbbi.cyMinChild != commandBar.Height) || (rbbi.cx != commandBar.Width) || (rbbi.cxIdeal != commandBar.Width))
						{
							rbbi.cyMinChild = commandBar.Height;
							rbbi.cx = commandBar.Width;
 							rbbi.cxIdeal = commandBar.Width;
							NativeMethods.SendMessage(this.Handle, NativeMethods.RB_SETBANDINFO, i, ref rbbi);
						}
					}
				}

				this.UpdateSize();

				this.EndUpdate();
			}
		}

		private void UpdateSize()
		{
			int height = NativeMethods.SendMessage(this.Handle, NativeMethods.RB_GETBARHEIGHT, 0, 0);
			this.Height = height + 1;
		}

		private NativeMethods.REBARBANDINFO CreateBandInfo(int index)
		{
			CommandBar commandBar = this.commandBars[index];

			NativeMethods.REBARBANDINFO rbbi = new NativeMethods.REBARBANDINFO();
			rbbi.cbSize = Marshal.SizeOf(typeof(NativeMethods.REBARBANDINFO));
			rbbi.fMask = 0;
			rbbi.clrFore = 0;
			rbbi.clrBack = 0;
			rbbi.iImage = 0;
			rbbi.hbmBack = IntPtr.Zero;
			rbbi.lParam = 0;
			rbbi.cxHeader = 0;

			rbbi.fMask |= NativeMethods.RBBIM_ID;
			rbbi.wID = 0xEB00 + index;

			if ((commandBar.Text != null) && (commandBar.Text.Length != 0))
			{
				rbbi.fMask |= NativeMethods.RBBIM_TEXT;
				rbbi.lpText = Marshal.StringToHGlobalUni(commandBar.Text);
				rbbi.cch = (commandBar.Text == null) ? 0 : commandBar.Text.Length;
			}

			int pad = 0;
			foreach (CommandBarItem item in commandBar.Items)
			{
				if ( item is CommandBarSeparator)
					pad += 2;
			}

			rbbi.fMask |= NativeMethods.RBBIM_STYLE;
			rbbi.fStyle = NativeMethods.RBBS_CHILDEDGE | NativeMethods.RBBS_FIXEDBMP | NativeMethods.RBBS_GRIPPERALWAYS;
			if (commandBar.NewLine == true)
				rbbi.fStyle |= NativeMethods.RBBS_BREAK;
			if (commandBar.UseChevron == true)
				rbbi.fStyle |= NativeMethods.RBBS_USECHEVRON;

			rbbi.fMask |= NativeMethods.RBBIM_CHILD;
			rbbi.hwndChild = commandBar.Handle; 

			rbbi.fMask |= NativeMethods.RBBIM_CHILDSIZE;
			rbbi.cyMinChild = commandBar.Height;
			rbbi.cxMinChild = 0; //commandBar.Width;
			rbbi.cyChild = 0;
			rbbi.cyMaxChild = commandBar.Height; 
			rbbi.cyIntegral = commandBar.Height;
		
			rbbi.fMask |= NativeMethods.RBBIM_SIZE;
			rbbi.cx = commandBar.Width + pad + 14;

			rbbi.fMask |= NativeMethods.RBBIM_IDEALSIZE;
			rbbi.cxIdeal = commandBar.Width + pad + 14;

			return rbbi;				
		}

		internal void UpdateBands()
		{
			if (this.IsHandleCreated)
			{
				this.RecreateHandle();
			}
		}

		private void CommandBar_HandleCreated(object sender, EventArgs e)
		{
			this.ReleaseBands();
			CommandBar commandBar = (CommandBar) sender;
			this.UpdateBand(commandBar);
			this.CaptureBands();
		}
	
		private void CommandBar_TextChanged(object sender, EventArgs e)
		{
			CommandBar commandBar = (CommandBar) sender;
			this.UpdateBand(commandBar);
		}

		/* 
		private void CommandBar_Resize(object sender, EventArgs e)
		{
			this.ReleaseBands();
			CommandBar commandBar = (CommandBar) sender;
			this.UpdateBand(commandBar);
			this.CaptureBands();
		}
		*/

		private void CaptureBands()
		{
			foreach (CommandBar commandBar in this.commandBars)
			{
				commandBar.HandleCreated += new EventHandler(this.CommandBar_HandleCreated);
				commandBar.TextChanged += new EventHandler(this.CommandBar_TextChanged);
				// TODO commandBar.Resize += new EventHandler(this.CommandBar_Resize);
			}
		}

		private void ReleaseBands()
		{
			foreach (CommandBar commandBar in this.commandBars)
			{
				commandBar.HandleCreated -= new EventHandler(this.CommandBar_HandleCreated);
				commandBar.TextChanged -= new EventHandler(this.CommandBar_TextChanged);
				// TODO commandBar.Resize -= new EventHandler(this.CommandBar_Resize);
			}
		}
	}
}
