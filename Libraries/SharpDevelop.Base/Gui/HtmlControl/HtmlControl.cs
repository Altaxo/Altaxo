using System;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.HtmlControl
{
	public class HtmlControl : AxHost, IWebBrowserEvents
	{
		public const int OLEIVERB_UIACTIVATE = -4;
				
		IWebBrowser control = null;
		AxHost.ConnectionPointCookie cookie;
		
		string url           = "";
		string html          = "";
		string cssStyleSheet = "";
		bool initialized     = false;
		
		public HtmlControl() : base("8856f961-340a-11d0-a96b-00c04fd705a2")
		{
		}
		
		public virtual void RaiseNavigateComplete(string url)
		{
			BrowserNavigateEventArgs e = new BrowserNavigateEventArgs(url, false);
			if (NavigateComplete != null) {
				NavigateComplete(this, e);
			}
		}
		
		public virtual void RaiseBeforeNavigate(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool cancel)
		{
			if (initialized) {
				BrowserNavigateEventArgs e = new BrowserNavigateEventArgs(url, false);
				if (BeforeNavigate != null) {
					BeforeNavigate(this, e);
				}
				cancel = e.Cancel;
			}
		}
		
		public string CascadingStyleSheet {
			get {
				return cssStyleSheet;
			}
			set {
				cssStyleSheet = value;
				ApplyCascadingStyleSheet();
			}
		}
		
		public string Url {
			set {
				this.url = value;
			}
		}
		
		public string Html {
			set {
				this.html = value;
				ApplyBody(html);
			}
		}
		
		protected override void DetachSink()
		{
			try {
				this.cookie.Disconnect();
			} catch {
			}
		}
		
		protected override void CreateSink()
		{
			try {
				this.cookie = new ConnectionPointCookie(this.GetOcx(), this, typeof(IWebBrowserEvents));
			} catch {
			}
		}
		
		protected override void AttachInterfaces()
		{
			try {
				this.control = (IWebBrowser)this.GetOcx();
			} catch {
			}
		}
		
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			
			NavigateComplete += new BrowserNavigateEventHandler(DelayedInitializeCaller);
			
			object flags       = 0;
			object targetFrame = String.Empty;
			object postData    = String.Empty;
			object headers     = String.Empty;
			this.control.Navigate("about:blank", ref flags, ref targetFrame, ref postData, ref headers);
		}
		
		void DelayedInitializeCaller(object sender, BrowserNavigateEventArgs e)
		{
			MethodInvoker mi = new MethodInvoker(this.DelayedInitialize);
			this.BeginInvoke(mi);
			NavigateComplete -= new BrowserNavigateEventHandler(DelayedInitializeCaller);
		}
		
		public void DelayedInitialize()
		{
			initialized = true;
			if (html.Length > 0) {
				ApplyBody(html);
			}
			UIActivate();
			ApplyCascadingStyleSheet();
		}
		
		void UIActivate()
		{
			this.DoVerb(OLEIVERB_UIACTIVATE);
		}
		
		void ApplyBody(string val)
		{
		    try {
				if (control != null) {
					IHTMLElement el    = null;
					IHTMLDocument2 doc = this.control.GetDocument();
					
					if (doc != null) {
						el = doc.GetBody();
					}
					
					if (el != null) {
						UIActivate();
						el.SetInnerHTML(val);
						return;
					}
				}
			} catch {}
		}
		
		void ApplyCascadingStyleSheet()
		{
			if (control != null) {
				IHTMLDocument2 htmlDoc = control.GetDocument();
				if (htmlDoc != null) {
					htmlDoc.CreateStyleSheet(cssStyleSheet, 0);
				}
			}
		}
		
		public event BrowserNavigateEventHandler BeforeNavigate;
		public event BrowserNavigateEventHandler NavigateComplete;
	}
}
