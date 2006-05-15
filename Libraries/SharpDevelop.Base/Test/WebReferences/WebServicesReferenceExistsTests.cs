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
	/// Tests that the generated project items for a web reference do not include
	/// a reference to System.Web.Services if one already exists in the project.
	/// </summary>
	[TestFixture]
	public class WebServicesReferenceExistsTest
	{
		SD.WebReference webReference;
		DiscoveryClientProtocol protocol;
		ReferenceProjectItem webServicesReferenceProjectItem;
		MSBuildProject project;
		
		string name = "localhost";
		string proxyNamespace = "WebReferenceNamespace";
		string updateFromUrl = "http://localhost/test.asmx";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			project = new MSBuildProject();
			project.FileName = "C:\\projects\\test\\foo.csproj";
			project.Language = "C#";
			
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(project, "System.Web.Services");
			project.Items.Add(referenceItem);
			
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
			webServicesReferenceProjectItem = (ReferenceProjectItem)WebReferenceTestHelper.GetProjectItem(webReference.Items, ItemType.Reference);
		}
		
		[Test]
		public void WebServicesReferenceItemDoesNotExist()
		{
			Assert.IsNull(webServicesReferenceProjectItem);
		}
		
		[Test]
		public void WebServicesReferenceDoesNotExist1()
		{
			MSBuildProject project = new MSBuildProject();
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(project, "System.Windows.Forms");
			project.Items.Add(referenceItem);
			
			Assert.IsFalse(SD.WebReference.ProjectContainsWebServicesReference(project));
		}
		
		[Test]
		public void WebServicesReferenceExists1()
		{
			MSBuildProject project = new MSBuildProject();
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(project, "system.web.services");
			project.Items.Add(referenceItem);
			
			Assert.IsTrue(SD.WebReference.ProjectContainsWebServicesReference(project));
		}
		
		[Test]
		public void WebServicesReferenceExists2()
		{
			MSBuildProject project = new MSBuildProject();
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(project, "System.Web.Services, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
			project.Items.Add(referenceItem);
			
			Assert.IsTrue(SD.WebReference.ProjectContainsWebServicesReference(project));
		}
	}
}
