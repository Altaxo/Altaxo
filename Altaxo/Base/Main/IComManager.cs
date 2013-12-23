using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Interface to the Com (Component object model) manager.
	/// </summary>
	public interface IComManager
	{
		/// <summary>
		/// Gets a value indicating whether Altaxo is in embedded mode, i.e. a object (graph document) is embedded in another application. If <c>true</c>, the properties
		/// <see cref="ContainerApplicationName"/>, <see cref="ContainerDocumentName"/> and <see cref="EmbeddedObject"/> can be used to set the title in the title bar of Altaxo.
		/// </summary>
		/// <value>
		///   <c>true</c> if Altaxo is in embedded mode; otherwise, <c>false</c>.
		/// </value>
		bool IsInEmbeddedMode { get; }

		/// <summary>
		/// If <see cref="IsInEmbeddedMode"/> is <c>true</c>, this property gets the name of the container application.
		/// </summary>
		/// <value>
		/// The name of the container application.
		/// </value>
		string ContainerApplicationName { get; }

		/// <summary>
		/// If <see cref="IsInEmbeddedMode"/> is <c>true</c>, this property gets the name of the container document.
		/// </summary>
		/// <value>
		/// The name of the container document.
		/// </value>
		string ContainerDocumentName { get; }

		/// <summary>
		/// If <see cref="IsInEmbeddedMode"/> is <c>true</c>, this property gets the embedded object (for instance the graph document that is embedded).
		/// </summary>
		/// <value>
		/// The embedded object.
		/// </value>
		object EmbeddedObject { get; }


		/// <summary>
		/// Registers the application for COM.
		/// </summary>
		void RegisterApplicationForCom();

		/// <summary>
		/// Unregisters the application for COM.
		/// </summary>
		void UnregisterApplicationForCom();

		/// <summary>
		/// Gets the documents COM object for the document. The document's Com object must at least implement <see cref="System.Runtime.InteropServices.ComTypes.IDataObject"/>.
		/// </summary>
		/// <param name="altaxoObject">The altaxo object (for instance graph document).</param>
		/// <returns>The document's Com object.</returns>
		System.Runtime.InteropServices.ComTypes.IDataObject GetDocumentsComObjectForDocument(object altaxoObject);
	}
}
