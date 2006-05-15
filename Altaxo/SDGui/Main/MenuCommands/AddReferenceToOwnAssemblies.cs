#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;
using ICSharpCode.Core;

using System.Collections;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal;
using ICSharpCode.SharpDevelop.Project;

namespace Altaxo.Main.Commands
{


  public class AddReferenceToOwnAssemblies : AbstractCommand
  {
  


    public override void Run()
    {

      
      // get the parser service
      HelperProject project = new HelperProject();
      System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach(System.Reflection.Assembly assembly in assemblies)
      {

        // for now, reference only that assemblies that contain "Altaxo" in their name
        // since otherwise the internal database becomes crazy large
        if(assembly.ToString().ToLower().IndexOf("altaxo")<0)
          continue;

        // System.Diagnostics.Trace.WriteLine("Try to add assembly to reference:" + assembly.ToString());

        ICSharpCode.SharpDevelop.Project.ReferenceProjectItem reference
          = new ICSharpCode.SharpDevelop.Project.ReferenceProjectItem(project, assembly.Location);

        project.Items.Add(reference);
        ParserService.CreateProjectContentForAddedProject(project);
      }
    }


    /// <summary>
    /// This class'es purpose in only to add assembly references to the parser service,
    /// since direct adding of assemblies is not possible in #D 0.98
    /// </summary>
    private class HelperProject : ICSharpCode.SharpDevelop.Project.IProject
    {
      //ICSharpCode.SharpDevelop.Project.Collections.ProjectReferenceCollection _projectReferences = new ICSharpCode.SharpDevelop.Internal.Project.Collections.ProjectReferenceCollection();

      #region IProject Members

     

      public string BaseDirectory
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      /// <summary>
      /// Marks a project for needing recompilation.
      /// </summary>
      public bool IsDirty 
      {
        get { return false; }
        set {}
      }

      /// <summary>
      /// The standad namespace new classes will be assigned to.
      /// </summary>
      public string StandardNamespace 
      {
        get { return "Altaxo"; }
        set {}
      }

     

      public bool IsFileInProject(string fileName)
      {
        throw new NotImplementedException();
      }

      public bool IsCompileable(string fileName)
      {
        throw new NotImplementedException();
      }

     

      public string Description
      {
        get
        {
          throw new NotImplementedException();
        }
        set
        {
          throw new NotImplementedException();
        }
      }

      public event System.EventHandler NameChanged;

     

      public string ProjectType
      {
        get
        {
          return "C#";
        }
      }

     

      public void LoadProject(string fileName)
      {
        throw new NotImplementedException();
      }

      public string Name
      {
        get
        {
          throw new NotImplementedException();
        }
        set
        {
          throw new NotImplementedException();
        }
      }

      public string GetParseableFileContent(string fileName)
      {
        throw new NotImplementedException();
      }

      public void CopyReferencesToOutputPath(bool force)
      {
        throw new NotImplementedException();
      }

      public void CopyReferencesToPath(string destination, bool force)
      {
        throw new NotImplementedException();
      }
      public void CopyReferencesToPath(string destination, bool force, ArrayList alreadyCopiedReferences)
      {
        throw new NotImplementedException();
      }

      public bool EnableViewState
      {
        get
        {
          throw new NotImplementedException();
        }
        set
        {
          throw new NotImplementedException();
        }
      }

    

      public void SaveProject(string fileName)
      {
        throw new NotImplementedException();
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
        // TODO:  Add HelperProject.Dispose implementation
      }

      #endregion

      #region IProject Members

      public System.Collections.Generic.List<ProjectItem> Items
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public System.Collections.Generic.List<ProjectSection> ProjectSections
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public string Language
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public IAmbience Ambience
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public string FileName
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string Directory
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public string Configuration
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string Platform
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string AssemblyName
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string DocumentationFileName
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public string OutputAssemblyFullPath
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public OutputType OutputType
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string RootNamespace
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string AppDesignerFolder
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public System.Collections.Generic.List<string> GetConfigurationNames()
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public System.Collections.Generic.List<string> GetPlatformNames()
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public bool CanCompile(string fileName)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public void Save()
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public void Save(string fileName)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public bool IsStartable
      {
        get { throw new Exception("The method or operation is not implemented."); }
      }

      public void Start(bool withDebugging)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public ParseProjectContent CreateProjectContent()
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public ProjectItem CreateProjectItem(string itemType)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public void Build(MSBuildEngineCallback callback)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public void Rebuild(MSBuildEngineCallback callback)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public void Clean(MSBuildEngineCallback callback)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public void Publish(MSBuildEngineCallback callback)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      #endregion

      #region ISolutionFolder Members

      public ISolutionFolderContainer Parent
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string TypeGuid
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string IdGuid
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      public string Location
      {
        get
        {
          throw new Exception("The method or operation is not implemented.");
        }
        set
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }

      #endregion

      #region IMementoCapable Members

      public Properties CreateMemento()
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public void SetMemento(Properties memento)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      #endregion
    }

  }

}
