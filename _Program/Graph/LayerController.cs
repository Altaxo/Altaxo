using System;

namespace Altaxo.Graph
{
	#region Interfaces
	public interface ILayerController: Main.IApplyController
	{
		void EhView_PageChanged();
		void EhView_SecondChoiceChanged(int index, string item);

	}

	public interface ILayerView
	{

		ILayerController Controller { get; set; }

		System.Windows.Forms.Form Form	{	get; }

		void AddTab(System.Windows.Forms.Control window, string name);

		System.Windows.Forms.Control CurrentTabContent { get; }

		void InitializeSecondaryChoice(string[] names, string name);

	}
	#endregion

	/// <summary>
	/// Summary description for LayerController.
	/// </summary>
	public class LayerController : ILayerController
	{
		protected ILayerView m_View;

		protected Layer m_Layer;

		private EdgeType m_CurrentEdge;

		enum ElementType { Unique, HorzVert, Edge };

		protected AxisScaleController[] m_AxisScaleController;

		
		public int CurrHorzVertIdx
		{
			get 
			{
				return (m_CurrentEdge==EdgeType.Left || m_CurrentEdge==EdgeType.Right) ? 0 : 1;
			}
		}
		public int CurrEdgeIdx
		{
			get 
			{
				return (int)m_CurrentEdge;
			}
		}

	


		public LayerController(Layer layer)
		{
			m_Layer = layer;

			m_AxisScaleController = new AxisScaleController[2]{
																													new AxisScaleController(m_Layer,AxisScaleController.AxisDirection.Horizontal),
																													new AxisScaleController(m_Layer,AxisScaleController.AxisDirection.Vertical)
																												};


			if(null!=View)
				SetViewElements();
		}

		public ILayerView View
		{
			get { return m_View; }
			set 
			{
				m_View = value;
				if(View!=null)
					SetViewElements();
			}
		}

		void SetViewElements()
		{
			if(null==View)
				return;

			// add all necessary Tabs
			View.AddTab(new AxisScaleControl(),"Scale");

			// Set the controller of the current visible Tab
			SetCurrentTabController();
		}



		void SetCurrentTabController()
		{
			if(View.CurrentTabContent is IAxisScaleView)
			{
				SetHorzVertSecondaryChoice();
				m_AxisScaleController[CurrHorzVertIdx].View = (IAxisScaleView)View.CurrentTabContent; 
			}
		}

		void SetHorzVertSecondaryChoice()
		{
			string[] names = new string[2]{"Vertical","Horizontal"};
			string name = names[CurrHorzVertIdx];
			View.InitializeSecondaryChoice(names,name);
		}

		public void EhView_PageChanged()
		{
			SetCurrentTabController();
		}

		public void EhView_SecondChoiceChanged(int index, string item)
		{
		}


		public static bool ShowDialog(System.Windows.Forms.Form parentWindow, Layer layer)
		{
			LayerController	ctrl = new LayerController(layer);
			LayerControl view = new LayerControl();
			ctrl.View = view;

			Main.DialogShellController dsc = new Main.DialogShellController(
				new Main.DialogShellView(view), ctrl);

			return dsc.ShowDialog(parentWindow);
		}


		#region IApplyController Members

		public bool Apply()
		{
			int i;
			// do the apply for all controllers that are allocated so far
			for(i=0;i<1;i++)
			{
				if(!m_AxisScaleController[i].Apply())
				{
					return false;
				}
			}


			return true;
		}

		#endregion
	}
}
