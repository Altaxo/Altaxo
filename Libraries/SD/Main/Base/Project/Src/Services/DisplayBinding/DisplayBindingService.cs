﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2952 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class handles the installed display bindings
	/// and provides a simple access point to these bindings.
	/// </summary>
	internal static class DisplayBindingService
	{
		const string displayBindingPath = "/SharpDevelop/Workbench/DisplayBindings";
		
		static Properties displayBindingServiceProperties;
		
		static List<DisplayBindingDescriptor> bindings;
		static List<ExternalProcessDisplayBinding> externalProcessDisplayBindings = new List<ExternalProcessDisplayBinding>();
		
		internal static void InitializeService()
		{
			bindings = AddInTree.BuildItems<DisplayBindingDescriptor>(displayBindingPath, null, true);
			displayBindingServiceProperties = PropertyService.Get("DisplayBindingService", new Properties());
			foreach (ExternalProcessDisplayBinding binding in displayBindingServiceProperties.Get("ExternalProcesses", new ExternalProcessDisplayBinding[0])) {
				if (binding != null) {
					AddExternalProcessDisplayBindingInternal(binding);
				}
			}
		}
		
		public static DisplayBindingDescriptor AddExternalProcessDisplayBinding(ExternalProcessDisplayBinding binding)
		{
			if (binding == null)
				throw new ArgumentNullException("binding");
			DisplayBindingDescriptor descriptor = AddExternalProcessDisplayBindingInternal(binding);
			SaveExternalProcessDisplayBindings();
			return descriptor;
		}
		
		static void SaveExternalProcessDisplayBindings()
		{
			displayBindingServiceProperties.Set("ExternalProcesses", externalProcessDisplayBindings.ToArray());
		}
		
		static DisplayBindingDescriptor AddExternalProcessDisplayBindingInternal(ExternalProcessDisplayBinding binding)
		{
			externalProcessDisplayBindings.Add(binding);
			DisplayBindingDescriptor descriptor = new DisplayBindingDescriptor(binding) {
				Id = binding.Id,
				Title = binding.Title
			};
			bindings.Add(descriptor);
			return descriptor;
		}
		
		public static void RemoveExternalProcessDisplayBinding(ExternalProcessDisplayBinding binding)
		{
			if (binding == null)
				throw new ArgumentNullException("binding");
			if (!externalProcessDisplayBindings.Remove(binding))
				throw new ArgumentException("binding was not added");
			SaveExternalProcessDisplayBindings();
			for (int i = 0; i < bindings.Count; i++) {
				if (bindings[i].GetLoadedBinding() == binding) {
					bindings.RemoveAt(i);
					return;
				}
			}
			throw new InvalidOperationException("did not find binding descriptor even though binding was registered");
		}
		
		/// <summary>
		/// Gets the primary display binding for the specified file name.
		/// </summary>
		public static IDisplayBinding GetBindingPerFileName(string filename)
		{
			DisplayBindingDescriptor codon = GetDefaultCodonPerFileName(filename);
			return codon == null ? null : codon.Binding;
		}
		
		/// <summary>
		/// Gets the default primary display binding for the specified file name.
		/// </summary>
		public static DisplayBindingDescriptor GetDefaultCodonPerFileName(string filename)
		{
			string defaultCommandID = displayBindingServiceProperties.Get("Default" + Path.GetExtension(filename).ToLowerInvariant()) as string;
			if (!string.IsNullOrEmpty(defaultCommandID)) {
				foreach (DisplayBindingDescriptor binding in bindings) {
					if (binding.Id == defaultCommandID) {
						if (IsPrimaryBindingValidForFileName(binding, filename)) {
							return binding;
						}
					}
				}
			}
			
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (IsPrimaryBindingValidForFileName(binding, filename)) {
					return binding;
				}
			}
			return null;
		}
		
		public static void SetDefaultCodon(string extension, DisplayBindingDescriptor bindingDescriptor)
		{
			if (bindingDescriptor == null)
				throw new ArgumentNullException("bindingDescriptor");
			if (extension == null)
				throw new ArgumentNullException("extension");
			if (!extension.StartsWith("."))
				throw new ArgumentException("extension must start with '.'");
			
			displayBindingServiceProperties.Set("Default" + extension.ToLowerInvariant(), bindingDescriptor.Id);
		}
		
		/// <summary>
		/// Gets list of possible primary display bindings for the specified file name.
		/// </summary>
		public static IList<DisplayBindingDescriptor> GetCodonsPerFileName(string filename)
		{
			List<DisplayBindingDescriptor> list = new List<DisplayBindingDescriptor>();
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (IsPrimaryBindingValidForFileName(binding, filename)) {
					list.Add(binding);
				}
			}
			return list;
		}
		
		static bool IsPrimaryBindingValidForFileName(DisplayBindingDescriptor binding, string filename)
		{
			if (!binding.IsSecondary && binding.CanOpenFile(filename)) {
				if (binding.Binding != null && binding.Binding.CanCreateContentForFile(filename)) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Attach secondary view contents to the view content.
		/// </summary>
		/// <param name="viewContent">The view content to attach to</param>
		/// <param name="isReattaching">This is a reattaching pass</param>
		public static void AttachSubWindows(IViewContent viewContent, bool isReattaching)
		{
			foreach (DisplayBindingDescriptor binding in bindings) {
				if (binding.IsSecondary && binding.CanOpenFile(viewContent.PrimaryFileName)) {
					ISecondaryDisplayBinding displayBinding = binding.SecondaryBinding;
					if (displayBinding != null
					    && (!isReattaching || displayBinding.ReattachWhenParserServiceIsReady)
					    && displayBinding.CanAttachTo(viewContent))
					{
						IViewContent[] subViewContents = binding.SecondaryBinding.CreateSecondaryViewContent(viewContent);
						if (subViewContents != null) {
							Array.ForEach(subViewContents, viewContent.SecondaryViewContents.Add);
						} else {
							MessageService.ShowError("Can't attach secondary view content. " + binding.SecondaryBinding + " returned null for " + viewContent + ".\n(should never happen)");
						}
					}
				}
			}
		}
	}
}
