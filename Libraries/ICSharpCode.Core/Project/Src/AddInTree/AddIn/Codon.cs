﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Represents a node in the add in tree that can produce an item.
	/// </summary>
	public class Codon
	{
		AddIn       addIn;
		string      name;
		Properties  properties;
		ICondition[] conditions;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public AddIn AddIn {
			get {
				return addIn;
			}
		}
		
		public string Id {
			get {
				return properties["id"];
			}
		}
		
		public string InsertAfter {
			get {
				if (!properties.Contains("insertafter")) {
					return "";
				}
				return properties["insertafter"];
			}
			set {
				properties["insertafter"] = value;
			}
		}
		
		public string InsertBefore {
			get {
				if (!properties.Contains("insertbefore")) {
					return "";
				}
				return properties["insertbefore"];
			}
			set {
				properties["insertbefore"] = value;
			}
		}
		
		public string this[string key] {
			get {
				return properties[key];
			}
		}
		
		public Properties Properties {
			get {
				return properties;
			}
		}
		
		public ICondition[] Conditions {
			get {
				return conditions;
			}
		}
		
		public Codon(AddIn addIn, string name, Properties properties, ICondition[] conditions)
		{
			this.addIn      = addIn;
			this.name       = name;
			this.properties = properties;
			this.conditions = conditions;
		}
		
		public ConditionFailedAction GetFailedAction(object caller)
		{
			ConditionFailedAction action = ConditionFailedAction.Nothing;
			foreach (ICondition condition in conditions) {
				try {
					if (!condition.IsValid(caller)) {
						if (condition.Action == ConditionFailedAction.Disable) {
							action = ConditionFailedAction.Disable;
						} else {
							return action = ConditionFailedAction.Exclude;
						}
					}
				} catch {
					LoggingService.Error("Exception while getting failed action from " + addIn.FileName);
					throw;
				}
			}
			return action;
		}
//		
//		public void BinarySerialize(BinaryWriter writer)
//		{
//			writer.Write(AddInTree.GetNameOffset(name));
//			writer.Write(AddInTree.GetAddInOffset(addIn));
//			properties.BinarySerialize(writer);
//		}
//		
		public object BuildItem(object owner, ArrayList subItems)
		{
			try {
				IDoozer doozer = AddInTree.Doozers[Name];
				if (!doozer.HandleConditions && conditions.Length > 0) {
					ConditionFailedAction action = GetFailedAction(owner);
					if (action != ConditionFailedAction.Nothing) {
						return null;
					}
				}
				return doozer.BuildItem(owner, this, subItems);
			} catch (KeyNotFoundException) {
				throw new CoreException("Doozer " + Name + " not found!");
			}
		}
		
		public override string ToString()
		{
			return String.Format("[Codon: name = {0}, addIn={1}]",
			                     name,
			                    addIn.FileName);
		}
	}
}
