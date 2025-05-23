﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main
{
  /// <summary>
  /// Interface to the Com (Component object model) manager.
  /// </summary>
  public interface IComManager
  {
    /// <summary>
    /// Processes the startup arguments that are given in the command to start the application.
    /// If the return value is true, we carry on and start the application.
    /// If the return value is false, we terminate the application immediately.
    /// </summary>
    /// <param name="args">The startup arguments.</param>
    /// <returns>True if the application startup should be proceeded; false if the application should exit immediately.</returns>
    bool ProcessStartupArguments(params string[] args);

    /// <summary>
    /// Gets a value indicating whether the application was started with the -embedding argument.
    /// </summary>
    /// <value>
    /// <c>true</c> if the application was started with -embedding argument; otherwise, <c>false</c>.
    /// </value>
    bool ApplicationWasStartedWithEmbeddingArg { get; }

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

    /// <summary>
    /// Gets the documents data object for the document. The document's data object must only implement <see cref="System.Runtime.InteropServices.ComTypes.IDataObject"/>.
    /// </summary>
    /// <param name="altaxoObject">The altaxo object (for instance graph document).</param>
    /// <returns>The document's data object.</returns>
    System.Runtime.InteropServices.ComTypes.IDataObject GetDocumentsDataObjectForDocument(object altaxoObject);

    /// <summary>
    /// Starts the ComManager.
    /// </summary>
    void StartLocalServer();

    /// <summary>
    /// Stops the ComManager.
    /// </summary>
    void StopLocalServer();
  }
}
