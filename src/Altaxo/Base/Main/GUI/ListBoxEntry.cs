using System;

namespace Altaxo.Main.GUI
{
	public class ListBoxEntry
	{
		protected string m_DisplayText;
		protected object m_Tag;

		public ListBoxEntry(string text, object tag)
		{
			this.DisplayText = text;
			this.Tag         = tag;
		}

		public string DisplayText
		{
			get { return m_DisplayText; }
			set { m_DisplayText = value; }
		}

		public object Tag
		{
			get { return m_Tag; }
			set { m_Tag = value; }
		}

		public override string ToString()
		{
			return m_DisplayText;
		}

			
	}
}
