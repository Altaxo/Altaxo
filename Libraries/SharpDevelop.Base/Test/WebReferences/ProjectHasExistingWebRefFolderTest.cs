// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 955 $</version>
// </file>

using ICSharpCode.SharpDevelop;
using SD = ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Services.Description;
using System.Web.Services.Discovery;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// Tests that a new web reference does not generate a WebReferencesProjectItem
	/// if the project already contains a web reference folder.
	/// </summary>
	[TestFixture]
	public class ProjectHasExistingWebRefFolderTest
	{
		SD.WebReference webReference;
		DiscoveryClientProtocol protocol;
		ProjectItem webReferencesProjectItem;
		MSBuildProject project;
		
		string name = "localhost";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			project = new MSBuildProject();
			project.Language = "C#";
			WebReferencesProjectItem item = new WebReferencesProjectItem(project);
			item.Include = "Web References\\";
			project.Items.Add(item);
			
			protocol = new DiscoveryClientProtocol();
			DiscoveryDocumentReference discoveryRef = new DiscoveryDocumentReference();
			discoveryRef.Url = updateFromUrl;
			protocol.References.Add(discoveryRef);
			
			ContractReference contractRef = new ContractReference();
			contractRef.Url = "http://localhost/test.asmx?wsdl";
			contractRef.ClientProtocol = new DiscoveryClientProtocol();
			ServiceDescription desc = new ServiceDescription();
			contractRef.ClientProtocol.Documents.Add(contractRef.Url, desc);
			protocol.References.Add(contractRef);
			
			webReference = new SD.WebReference(project, updateFromUrl, name, proxyNamespace, protocol);
			webReferencesProjectItem = WebReferenceTestHelper.GetProjectItem(webReference.Items, "Web References\\", ItemType.WebReferences);
		}
		
		[Test]
		public void ProjectItemContainsWebReferencesFolder()
		{
			Assert.IsTrue(SD.WebReference.ProjectContainsWebReferencesFolder(project));
		}
		
		[Test]
		public void WebReferencesProjectItemDoesNotExist()
		{
			Assert.IsNull(webReferencesProjectItem);
		}
	}
}
