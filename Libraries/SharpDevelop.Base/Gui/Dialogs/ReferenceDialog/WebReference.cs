// created on 10/11/2002 at 2:08 PM

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Xml;
using System.Net;
using System.Web.Services.Description;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	/// Summary description for WebReference.
	/// </summary>
	public class WebReference
	{	
		///
		/// <summary>Creates a ServiceDescription object from a valid URI</summary>
		/// 
		public static ServiceDescription ReadServiceDescription(string uri)
		{
			ServiceDescription desc = null;
			
			try {
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
				request.Timeout = 10000; 
				WebResponse response  = request.GetResponse();
			
				desc = ServiceDescription.Read(response.GetResponseStream());
				response.Close();
				desc.RetrievalUrl = uri;
			} catch (Exception) {				
				// possibly error reading WSDL?
				return null;
			} 		
			if(desc.Services.Count == 0)
				return null;
			
			return desc;
		}
		
		///
		/// <summary>Generates a valid directory from a URI</summary>
		/// 
		public static string GetDirectoryFromUri(string uri)
		{
			// TODO: construct the namespace using the URL in the WSDL
			string tmp = uri;
			if(uri.IndexOf("://") > -1) {
				tmp = uri.Substring(uri.IndexOf("://") + 3);
			}
			tmp = tmp.Substring(0, tmp.LastIndexOf("/"));			
			string[] dirs = tmp.Split(new Char[] {'/'});
						
			StringBuilder savedir = new StringBuilder();
			savedir.Append(dirs[0]);
		
			return savedir.ToString();
		}
		
		///
		/// <summary>Generates a valid Namespace from a URI</summary>
		/// 
		public static string GetNamespaceFromUri(string uri)
		{
			// TODO: construct the namespace using the URL in the WSDL
			string tmp = uri;
			if(uri.IndexOf("://") > -1) {
				tmp = uri.Substring(uri.IndexOf("://") + 3);
			}
			tmp = tmp.Substring(0, tmp.LastIndexOf("/"));			
			string[] dirs = tmp.Split(new Char[] {'/'});
											
			return(dirs[0]);			
		}
		
		
		public static ProjectReference GenerateWebProxyDLL(IProject project, ServiceDescription desc)
		{
			ProjectReference refInfo = null;
			
			string serviceName = String.Empty;
			if(desc.Services.Count > 0) {
				serviceName = desc.Services[0].Name;
			} else {
				serviceName = "UnknownService";
			}									
								
			string nmspace = GetNamespaceFromUri(desc.RetrievalUrl);
			
			StringBuilder savedir = new StringBuilder();
			savedir.Append(project.BaseDirectory);
			savedir.Append(Path.DirectorySeparatorChar);
			savedir.Append("Web References");			
			// second, create the path if it doesn't exist
			DirectoryInfo di;		
			if(!Directory.Exists(savedir.ToString()))
			{
				di = Directory.CreateDirectory(savedir.ToString());
			}
			
			// generate the assembly
			ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
			importer.AddServiceDescription(desc, null, null);
			
			CodeNamespace codeNamespace = new CodeNamespace(nmspace);
			CodeCompileUnit codeUnit = new CodeCompileUnit();
			codeUnit.Namespaces.Add(codeNamespace);
			ServiceDescriptionImportWarnings warnings = importer.Import(codeNamespace, codeUnit);
			
			CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();			
			System.CodeDom.Compiler.ICodeCompiler compiler;
			
			
			if(provider != null) {
				compiler = provider.CreateCompiler();
				CompilerParameters parms = new CompilerParameters();
				parms.ReferencedAssemblies.Add("System.Dll");
				parms.ReferencedAssemblies.Add("System.Xml.Dll");
				parms.ReferencedAssemblies.Add("System.Web.Services.Dll");
				parms.OutputAssembly = project.BaseDirectory + Path.DirectorySeparatorChar + "WebReferences" + Path.DirectorySeparatorChar + nmspace + ".Reference.Dll";
				CompilerResults results = compiler.CompileAssemblyFromDom(parms, codeUnit);
				Assembly assembly = results.CompiledAssembly;
				
				if(assembly != null) {
					refInfo = new ProjectReference();
					refInfo.ReferenceType = ReferenceType.Assembly;
					refInfo.Reference = parms.OutputAssembly;
				}
			}
			
			return refInfo;
		}
		
		///
		/// <summary>Generates a Web Service proxy DLL from a URI</summary>
		/// 
		public static ProjectReference GenerateWebProxyDLL(IProject project, string url)
		{							
			ServiceDescription desc = ReadServiceDescription(url);						
			return GenerateWebProxyDLL(project, desc);					
		}
		
		public static void GenerateWebProxyCode(string proxyNamespace, string fileName, ServiceDescription desc)
		{
			// generate the assembly
			ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
			importer.AddServiceDescription(desc, null, null);
			
			CodeNamespace codeNamespace = new CodeNamespace(proxyNamespace);
			CodeCompileUnit codeUnit = new CodeCompileUnit();
			codeUnit.Namespaces.Add(codeNamespace);
			ServiceDescriptionImportWarnings warnings = importer.Import(codeNamespace, codeUnit);
			
			CodeDomProvider provider;
			System.CodeDom.Compiler.ICodeGenerator generator;
			
			switch(Path.GetExtension(fileName).ToLower()) {
				case ".cs":
					provider = new Microsoft.CSharp.CSharpCodeProvider();
					break;
				case ".vb":
					provider = new Microsoft.VisualBasic.VBCodeProvider();					
					break;
							
				default:
					// extension not supported
					provider = null;			
					break;
			}
			
			if(provider != null) {				
				StreamWriter sw = new StreamWriter(fileName);

				generator = provider.CreateGenerator();
				CodeGeneratorOptions options = new CodeGeneratorOptions();
				options.BracingStyle = "C";
				generator.GenerateCodeFromCompileUnit(codeUnit, sw, options);
				sw.Close();
			}
		}
		
		public static ArrayList GenerateWebProxyCode(string proxyNamespace, string referenceName, IProject project, ServiceDescription desc)
		{
			ArrayList fileList = null;			
			string webRefFolder = "Web References";
				
			StringBuilder savedir = new StringBuilder();
			savedir.Append(project.BaseDirectory);
			savedir.Append(Path.DirectorySeparatorChar);
			savedir.Append(webRefFolder);
			savedir.Append(Path.DirectorySeparatorChar);
			
			referenceName = GetReferenceName(project, savedir.ToString(), referenceName);
			savedir.Append(referenceName);
			
			// second, create the path if it doesn't exist
			Directory.CreateDirectory(savedir.ToString());
			
			String ext = String.Empty;
			switch(project.ProjectType) {
				case "C#":
					ext = ".cs";
					break;
				case "VBNET":
					ext = ".vb";
					break;
							
				default:
					break;
			}

			string filename = String.Concat(savedir.ToString(), Path.DirectorySeparatorChar, GetServiceName(desc), ext);
			string wsdlfilename = String.Concat(savedir.ToString(), Path.DirectorySeparatorChar, GetServiceName(desc), ".wsdl");
			
			GenerateWebProxyCode(proxyNamespace, filename, desc);

			if(File.Exists(filename)) 
			{
				fileList = new ArrayList();
				
				// add project files to the list
				ProjectFile pfile = new ProjectFile();
				
				pfile.Name = project.BaseDirectory + Path.DirectorySeparatorChar + webRefFolder;
				pfile.BuildAction = BuildAction.Nothing;					
				pfile.Subtype = Subtype.WebReferences;
				pfile.DependsOn = String.Empty;
				pfile.Data = String.Empty;										
				fileList.Add(pfile);
				
				// the Web Reference
				pfile = new ProjectFile();
				pfile.Name = savedir.ToString();
				pfile.BuildAction = BuildAction.Nothing;
				pfile.Subtype = Subtype.WebReference;
				pfile.DependsOn = project.BaseDirectory + Path.DirectorySeparatorChar + webRefFolder;
				pfile.Data = desc.RetrievalUrl;					
				fileList.Add(pfile);										
				
				// the Web Reference Proxy
				pfile = new ProjectFile();
				pfile.Name = filename;
				pfile.BuildAction = BuildAction.Compile;
				pfile.Subtype = Subtype.Code;
				pfile.Data = proxyNamespace;
				pfile.DependsOn = savedir.ToString();
				fileList.Add(pfile);										
				
				// the WSDL File used to generate the Proxy
				desc.Write(wsdlfilename);
				pfile = new ProjectFile();
				pfile.Name = wsdlfilename;
				pfile.BuildAction = BuildAction.Nothing;
				pfile.Subtype = Subtype.Code;
				pfile.Data = "WSDL";
				pfile.DependsOn = savedir.ToString();
				fileList.Add(pfile);
			}
			
			return fileList;
		}
	
		public static string GetServiceName(ServiceDescription desc)
		{
			string serviceName = String.Empty;
			if(desc.Services.Count > 0) {
				serviceName = desc.Services[0].Name;
			} else {
				serviceName = "UnknownService";
			}	
			
			return serviceName;
		}
		
		/// <summary>
		/// Returns the reference name.  If the folder that will contain the 
		/// web reference already exists this method looks for a new folder by 
		/// adding a digit to the end of the reference name.
		/// </summary>
		static string GetReferenceName(IProject project, string baseFolderPath, string baseReferenceName)
		{						
			// if it is already in the project, or it does exists we get a new name.
			int count = 1;
			string referenceName = baseReferenceName;
			string folder = Path.Combine(baseFolderPath, baseReferenceName);
			while (project.IsFileInProject(folder) || Directory.Exists(folder)) {
				referenceName = String.Concat(baseReferenceName, count.ToString());
				folder = Path.Combine(baseFolderPath, referenceName);
				++count;
			}	
			
			return referenceName;
		}
	}
}
