using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for AxisLinkControl.xaml
	/// </summary>
	public partial class AxisLinkControl : UserControl, IAxisLinkView
	{

		public AxisLinkControl()
		{
			InitializeComponent();
		}

		void EnableCustom(bool bEnab)
		{
			this.m_edLinkAxisOrgA.IsEnabled = bEnab;
			this.m_edLinkAxisOrgB.IsEnabled = bEnab;
			this.m_edLinkAxisEndA.IsEnabled = bEnab;
			this.m_edLinkAxisEndB.IsEnabled = bEnab;
		}


		private void EhLinkStraight_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if(null!=LinkType_Changed && this.m_rbLinkAxisStraight.IsChecked == true)
				LinkType_Changed(true);
		}

		private void EhLinkCustom_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (null != LinkType_Changed && this.m_rbLinkAxisCustom.IsChecked == true)
				LinkType_Changed(false);
		}

		#region IAxisLinkView

		public void LinkType_Initialize(bool isStraight)
		{
			if (isStraight)
			{
				this.m_rbLinkAxisStraight.IsChecked = true;
				EnableCustom(false);
			}
			else
			{
				this.m_rbLinkAxisCustom.IsChecked = true;
				EnableCustom(true);
			}
		}

		public void OrgA_Initialize(string text)
		{
			this.m_edLinkAxisOrgA.Text = text;
		}

		public void OrgB_Initialize(string text)
		{
			this.m_edLinkAxisOrgB.Text = text;
		}

		public void EndA_Initialize(string text)
		{
			this.m_edLinkAxisEndA.Text = text;
		}

		public void EndB_Initialize(string text)
		{
			this.m_edLinkAxisEndB.Text = text;
		}

		public void Enable_OrgAndEnd_Boxes(bool bEnable)
		{
			EnableCustom(bEnable);
		}

		public event Action< ValidationEventArgs<string>> OrgA_Validating;

		public event Action< ValidationEventArgs<string>> OrgB_Validating;

		public event Action< ValidationEventArgs<string>> EndA_Validating;

		public event Action< ValidationEventArgs<string>> EndB_Validating;

		public event Action<bool> LinkType_Changed;

		#endregion

		private void EhOrgA_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != OrgA_Validating)
			{
				OrgA_Validating(e);
			}
		}

		private void EhOrgB_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != OrgB_Validating)
			{
				OrgB_Validating(e);
			}
		}

		private void EhEndA_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != EndA_Validating)
			{
				EndA_Validating(e);
			}
		}

		private void EhEndB_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != EndB_Validating)
			{
				EndB_Validating(e);
			}
		}


	
	}
}
