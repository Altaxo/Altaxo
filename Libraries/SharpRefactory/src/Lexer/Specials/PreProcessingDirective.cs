using System;
using System.Text;
using System.CodeDom;
using System.Collections;

namespace ICSharpCode.SharpRefactory.Parser
{
	public class PreProcessingDirective
	{
		string cmd;
		string arg;
		
		public string Cmd {
			get {
				return cmd;
			}
			set {
				cmd = value;
			}
		}
		
		public string Arg {
			get {
				return arg;
			}
			set {
				arg = value;
			}
		}
		
		public PreProcessingDirective(string cmd, string arg)
		{
			this.cmd = cmd;
			this.arg = arg;
		}
	}
}
