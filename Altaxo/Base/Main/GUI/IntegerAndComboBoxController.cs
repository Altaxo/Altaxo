using System;

namespace Altaxo.Main.GUI
{
	#region Interfaces
	public interface IIntegerAndComboBoxController : Main.GUI.IMVCController, Main.GUI.IApplyController
	{
		/// <summary>
		/// Get/sets the view this controller controls.
		/// </summary>
		IIntegerAndComboBoxView View { get; set; }

		void EhView_IntegerChanged(int val, ref bool bCancel);

		void EhView_ComboBoxSelectionChanged(ListBoxEntry selectedItem);

	}

	public interface IIntegerAndComboBoxView : Main.GUI.IMVCView
	{

		/// <summary>
		/// Get/sets the controller of this view.
		/// </summary>
		IIntegerAndComboBoxController Controller { get; set; }

		/// <summary>
		/// Gets the hosting parent form of this view.
		/// </summary>
		System.Windows.Forms.Form Form	{	get; }

		
		void ComboBox_Initialize(ListBoxEntry[] items, ListBoxEntry defaultItem);

	
		void ComboBoxLabel_Initialize(string text);

		
		void IntegerEdit_Initialize(int min, int max, int val);

		void IntegerLabel_Initialize(string text);

		
	
	}
	#endregion

	/// <summary>
	/// Summary description for IntegerAndComboBoxController.
	/// </summary>
	public class IntegerAndComboBoxController : IIntegerAndComboBoxController
	{
		protected IIntegerAndComboBoxView m_View;
		protected string m_IntegerLabelText;
		protected string m_ComboBoxLabelText;
		protected int m_IntegerMinimum;
		protected int m_IntegerMaximum;
		protected int m_IntegerValue;
		protected ListBoxEntry[] m_ComboBoxItems;
		protected ListBoxEntry m_SelectedItem;

		
		public IntegerAndComboBoxController(string integerLabel, int intMin, int intMax, int intVal, string comboBoxLabel, ListBoxEntry[] items, int defaultItem)
		{
			m_IntegerLabelText = integerLabel;
			m_IntegerMinimum = intMin;
			m_IntegerMaximum = intMax;
			m_IntegerValue		= intVal;
			m_ComboBoxLabelText = comboBoxLabel;
			m_ComboBoxItems = items;
			m_SelectedItem = items[defaultItem];

			SetElements(true);
		}


		public void SetElements(bool bInit)
		{
			if(bInit)
			{
			}

			if(View!=null)
			{
				View.ComboBoxLabel_Initialize(m_ComboBoxLabelText);
				View.ComboBox_Initialize(m_ComboBoxItems,m_SelectedItem);
				View.IntegerLabel_Initialize(m_IntegerLabelText);
				View.IntegerEdit_Initialize(m_IntegerMinimum,m_IntegerMaximum,m_IntegerValue);
			}
		}

		public IIntegerAndComboBoxView View
		{
			get { return m_View; }
			set
			{
				if(null!=m_View)
					m_View.Controller = null;

				m_View = value;

				if(null!=m_View)
				{
					m_View.Controller = this;
					SetElements(false);
				}
			}
		}
	
		public bool Apply()
		{
			return true; // all is done on the fly, we don't need actions here
		}

		public ListBoxEntry SelectedItem
		{
			get { return m_SelectedItem; }
		}

		public int IntegerValue
		{
			get { return this.m_IntegerValue; }
		}

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				
				return View;
			}
			set
			{
				View = value as IIntegerAndComboBoxView;
			}
		}

		#endregion

		#region IIntegerAndComboBoxController Members

		public void EhView_IntegerChanged(int val, ref bool bCancel)
		{
			m_IntegerValue = val;
			bCancel = false;
		}

		public void EhView_ComboBoxSelectionChanged(ListBoxEntry selectedItem)
		{
			m_SelectedItem = selectedItem;
		}

		#endregion
	}



}
