using System;

namespace Altaxo.Graph
{
	#region Interfaces
	public interface ILayerController: Gui.IApplyController
	{
		void EhView_PageChanged(string firstChoice);
		void EhView_SecondChoiceChanged(int index, string item);

	}

	public interface ILayerView
	{

		ILayerController Controller { get; set; }

		System.Windows.Forms.Form Form	{	get; }

		void AddTab(string name, string text);

		System.Windows.Forms.Control CurrentContent { get; set; }

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

		private string   m_CurrentPage;
		private EdgeType m_CurrentEdge;

		Main.IMVCController m_CurrentController;

		enum ElementType { Unique, HorzVert, Edge };

		protected ILayerPositionController m_LayerPositionController;
		protected ILineScatterLayerContentsController m_LayerContentsController;
		protected IAxisScaleController[] m_AxisScaleController;
		protected ITitleFormatLayerController[] m_TitleFormatLayerController;

		
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

			m_LayerContentsController = new LineScatterLayerContentsController(m_Layer);

			m_LayerPositionController = new LayerPositionController(m_Layer);

			m_AxisScaleController = new AxisScaleController[2]{
																													new AxisScaleController(m_Layer,AxisScaleController.AxisDirection.Vertical),
																													new AxisScaleController(m_Layer,AxisScaleController.AxisDirection.Horizontal)
																												};

			m_TitleFormatLayerController = new TitleFormatLayerController[4]{
																																				new TitleFormatLayerController(m_Layer,EdgeType.Left),
																																				new TitleFormatLayerController(m_Layer,EdgeType.Bottom),
																																				new TitleFormatLayerController(m_Layer,EdgeType.Right),
																																				new TitleFormatLayerController(m_Layer,EdgeType.Top)
																																			};


			m_CurrentPage = "Scale";
			m_CurrentEdge = EdgeType.Bottom;

			if(null!=View)
				SetViewElements();
		}

		public ILayerView View
		{
			get { return m_View; }
			set 
			{
				if(null!=m_View)
				{
					m_View.Controller = null;
				}

				m_View = value;
				
				if(null!=m_View)
				{
					m_View.Controller = this;
					SetViewElements();
				}
			}
		}

		void SetViewElements()
		{
			if(null==View)
				return;

			// add all necessary Tabs
			View.AddTab("Scale","Scale");
			View.AddTab("TitleAndFormat","Title&&Format");
			View.AddTab("Contents","Contents");
			View.AddTab("Position","Position");

			// Set the controller of the current visible Tab
			SetCurrentTabController(true);
		}



		void SetCurrentTabController(bool pageChanged)
		{
			if(null!=m_CurrentController) 
				m_CurrentController.ViewObject=null; // detach current view

			switch(m_CurrentPage)
			{
				case "Contents":
					if(pageChanged)
					{
						SetLayerSecondaryChoice();
						View.CurrentContent = new LineScatterLayerContentsControl();
					}

					m_CurrentController = m_LayerContentsController;
					break;
				case "Position":
					if(pageChanged)
					{
						SetLayerSecondaryChoice();
						View.CurrentContent = new LayerPositionControl();
					}

					m_CurrentController = m_LayerPositionController;
					break;
				case "Scale":
					if(pageChanged)
					{
						SetHorzVertSecondaryChoice();
						View.CurrentContent = new AxisScaleControl();
					}

					m_CurrentController = m_AxisScaleController[CurrHorzVertIdx];
					
					
					break;
				case "TitleAndFormat":
					if(pageChanged)
					{
						SetEdgeSecondaryChoice();
						View.CurrentContent = new TitleFormatLayerControl();
					}
					m_CurrentController = m_TitleFormatLayerController[CurrEdgeIdx];
					break;

			}

		if(null!=m_CurrentController)
			m_CurrentController.ViewObject = View.CurrentContent; 
		}


		void SetLayerSecondaryChoice()
		{
			string[] names = new string[1]{"Layer"};
			string name = names[0];
			View.InitializeSecondaryChoice(names,name);
		}

		void SetHorzVertSecondaryChoice()
		{
			string[] names = new string[2]{"Vertical","Horizontal"};
			string name = names[CurrHorzVertIdx];
			View.InitializeSecondaryChoice(names,name);
		}

		void SetEdgeSecondaryChoice()
		{
			string[] names = new string[4]{"Left","Bottom","Right","Top"};
			string name = names[CurrEdgeIdx];
			View.InitializeSecondaryChoice(names,name);
		}
	
		public void EhView_PageChanged(string firstChoice)
		{
			m_CurrentPage = firstChoice;
			SetCurrentTabController(true);
		}

		public void EhView_SecondChoiceChanged(int index, string item)
		{
			switch(item)
			{
				case "Left":
					this.m_CurrentEdge = EdgeType.Left;
					break;
				case "Bottom":
					this.m_CurrentEdge = EdgeType.Bottom;
					break;
				case "Right":
					this.m_CurrentEdge = EdgeType.Right;
					break;
				case "Top":
					this.m_CurrentEdge = EdgeType.Top;
					break;
				case "Horizontal":
					if(this.m_CurrentEdge!=EdgeType.Bottom && this.m_CurrentEdge!=EdgeType.Top)
						this.m_CurrentEdge=EdgeType.Bottom;
					break;
				case "Vertical":
					if(this.m_CurrentEdge!=EdgeType.Left && this.m_CurrentEdge!=EdgeType.Right)
						this.m_CurrentEdge=EdgeType.Left;
					break;
			}
			SetCurrentTabController(false);
		}


		public static bool ShowDialog(System.Windows.Forms.Form parentWindow, Layer layer)
		{
			LayerController	ctrl = new LayerController(layer);
			LayerControl view = new LayerControl();
			ctrl.View = view;

			Gui.DialogShellController dsc = new Gui.DialogShellController(
				new Gui.DialogShellView(view), ctrl);

			return dsc.ShowDialog(parentWindow);
		}


		#region IApplyController Members

		public bool Apply()
		{
			int i;

			if(!this.m_LayerContentsController.Apply())
				return false;

			if(!this.m_LayerPositionController.Apply())
				return false;

			// do the apply for all controllers that are allocated so far
			for(i=0;i<2;i++)
			{
				if(!m_AxisScaleController[i].Apply())
				{
					return false;
				}
			}


			for(i=0;i<4;i++)
			{
				if(!m_TitleFormatLayerController[i].Apply())
				{
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}
