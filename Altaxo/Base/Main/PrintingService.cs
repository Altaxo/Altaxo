using System;

namespace Altaxo.Main
{

	/// <summary>
	/// This interface provides access to printers and forms.
	/// </summary>
	public interface IPrintingService 
	{
		/// <summary>
		/// Returns the current print document for this instance of the application.
		/// This contains settings that store the current printer, paper size, orientation and so on.
		/// </summary>
		System.Drawing.Printing.PrintDocument PrintDocument	{	get ; }
		
		
		/// <summary>
		/// Returns the current printing page setup dialog for this instance of the application.
		/// </summary>
		System.Windows.Forms.PageSetupDialog PageSetupDialog	{	get; }

		/// <summary>
		/// Returns the print dialog for the current instance of this application.
		/// </summary>
		System.Windows.Forms.PrintDialog PrintDialog { get; }
	}


	/// <summary>
	/// Summary description for PrintingService.
	/// </summary>
	public class PrintingService :  ICSharpCode.Core.Services.AbstractService, IPrintingService
	{
	
		private System.Windows.Forms.PageSetupDialog m_PageSetupDialog;

		private System.Drawing.Printing.PrintDocument m_PrintDocument;

		private System.Windows.Forms.PrintDialog m_PrintDialog;

		public PrintingService()
		{
	

			// we initialize the printer variables
			m_PrintDocument = new System.Drawing.Printing.PrintDocument();
			// we set the print document default orientation to landscape
			m_PrintDocument.DefaultPageSettings.Landscape=true;
			m_PageSetupDialog = new System.Windows.Forms.PageSetupDialog();
			m_PageSetupDialog.Document = m_PrintDocument;
			m_PrintDialog = new System.Windows.Forms.PrintDialog();
			m_PrintDialog.Document = m_PrintDocument;
		}

		#region Properties


		

		public  System.Windows.Forms.PageSetupDialog PageSetupDialog
		{
			get { return m_PageSetupDialog; }
		}

		public  System.Drawing.Printing.PrintDocument PrintDocument
		{
			get { return m_PrintDocument; }
		}


		public System.Windows.Forms.PrintDialog PrintDialog
		{
			get { return m_PrintDialog; }
		}


	

		#endregion


	}
}
