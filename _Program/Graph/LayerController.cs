using System;

namespace Altaxo.Graph
{
	#region Interfaces
	public interface ILayerController
	{
	
	}

	public interface ILayerView
	{

		ILayerController Controller { get; set; }

		System.Windows.Forms.Form Form	{	get; }

		void AddTab(System.Windows.Forms.Control window, string name);
	}
	#endregion

	/// <summary>
	/// Summary description for LayerController.
	/// </summary>
	public class LayerController
	{
		protected ILayerView m_View;

		protected Layer m_Layer;

		protected AxisScaleController[] m_AxisScaleController;

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

			View.AddTab(new AxisScaleControl(),"Scale");
		}
	}
}
