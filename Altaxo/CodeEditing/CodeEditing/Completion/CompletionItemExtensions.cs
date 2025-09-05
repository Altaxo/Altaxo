// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Completion/CompletionItemExtensions.cs

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Tags;

namespace Altaxo.CodeEditing.Completion
{
  public static class CompletionItemExtensions
  {
    internal static Glyph? GetGlyph(this CompletionItem completionItem)
    {
      var tags = completionItem.Tags;
      return GetGlyph(tags);
    }

    public static CompletionDescription GetDescription(this CompletionItem completionItem)
    {
      return CommonCompletionItem.GetDescription(completionItem);
    }

    #region from RoslynPad.Roslyn, CodeActions/CodeActionExtensions.cs
    public static Glyph GetGlyph(ImmutableArray<string> tags)
    {
      foreach (var tag in tags)
      {
        switch (tag)
        {
          case WellKnownTags.Assembly:
            return Glyph.Assembly;

          case WellKnownTags.File:
            return tags.Contains(LanguageNames.VisualBasic) ? Glyph.BasicFile : Glyph.CSharpFile;

          case WellKnownTags.Project:
            return tags.Contains(LanguageNames.VisualBasic) ? Glyph.BasicProject : Glyph.CSharpProject;

          case WellKnownTags.Class:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.ClassProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.ClassPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.ClassInternal,
              _ => Glyph.ClassPublic,
            };
          case WellKnownTags.Constant:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.ConstantProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.ConstantPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.ConstantInternal,
              _ => Glyph.ConstantPublic,
            };
          case WellKnownTags.Delegate:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.DelegateProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.DelegatePrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.DelegateInternal,
              _ => Glyph.DelegatePublic,
            };
          case WellKnownTags.Enum:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.EnumProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.EnumPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.EnumInternal,
              _ => Glyph.EnumPublic,
            };
          case WellKnownTags.EnumMember:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.EnumMemberProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.EnumMemberPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.EnumMemberInternal,
              _ => Glyph.EnumMemberPublic,
            };
          case WellKnownTags.Error:
            return Glyph.Error;

          case WellKnownTags.Event:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.EventProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.EventPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.EventInternal,
              _ => Glyph.EventPublic,
            };
          case WellKnownTags.ExtensionMethod:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.ExtensionMethodProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.ExtensionMethodPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.ExtensionMethodInternal,
              _ => Glyph.ExtensionMethodPublic,
            };
          case WellKnownTags.Field:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.FieldProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.FieldPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.FieldInternal,
              _ => Glyph.FieldPublic,
            };
          case WellKnownTags.Interface:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.InterfaceProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.InterfacePrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.InterfaceInternal,
              _ => Glyph.InterfacePublic,
            };
          case WellKnownTags.Intrinsic:
            return Glyph.Intrinsic;

          case WellKnownTags.Keyword:
            return Glyph.Keyword;

          case WellKnownTags.Label:
            return Glyph.Label;

          case WellKnownTags.Local:
            return Glyph.Local;

          case WellKnownTags.Namespace:
            return Glyph.Namespace;

          case WellKnownTags.Method:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.MethodProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.MethodPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.MethodInternal,
              _ => Glyph.MethodPublic,
            };
          case WellKnownTags.Module:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.ModulePublic,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.ModulePrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.ModuleInternal,
              _ => Glyph.ModulePublic,
            };
          case WellKnownTags.Folder:
            return Glyph.OpenFolder;

          case WellKnownTags.Operator:
            return Glyph.Operator;

          case WellKnownTags.Parameter:
            return Glyph.Parameter;

          case WellKnownTags.Property:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.PropertyProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.PropertyPrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.PropertyInternal,
              _ => Glyph.PropertyPublic,
            };
          case WellKnownTags.RangeVariable:
            return Glyph.RangeVariable;

          case WellKnownTags.Reference:
            return Glyph.Reference;

          case WellKnownTags.NuGet:
            return Glyph.NuGet;

          case WellKnownTags.Structure:
            return GetAccessibility(tags) switch
            {
              Microsoft.CodeAnalysis.Accessibility.Protected => Glyph.StructureProtected,
              Microsoft.CodeAnalysis.Accessibility.Private => Glyph.StructurePrivate,
              Microsoft.CodeAnalysis.Accessibility.Internal => Glyph.StructureInternal,
              _ => Glyph.StructurePublic,
            };
          case WellKnownTags.TypeParameter:
            return Glyph.TypeParameter;

          case WellKnownTags.Snippet:
            return Glyph.Snippet;

          case WellKnownTags.Warning:
            return Glyph.CompletionWarning;

          case WellKnownTags.StatusInformation:
            return Glyph.StatusInformation;
        }
      }

      return Glyph.None;
    }

    private static Microsoft.CodeAnalysis.Accessibility GetAccessibility(ImmutableArray<string> tags)
    {
      if (tags.Contains(WellKnownTags.Public))
      {
        return Microsoft.CodeAnalysis.Accessibility.Public;
      }
      if (tags.Contains(WellKnownTags.Protected))
      {
        return Microsoft.CodeAnalysis.Accessibility.Protected;
      }
      if (tags.Contains(WellKnownTags.Internal))
      {
        return Microsoft.CodeAnalysis.Accessibility.Internal;
      }
      if (tags.Contains(WellKnownTags.Private))
      {
        return Microsoft.CodeAnalysis.Accessibility.Private;
      }
      return Microsoft.CodeAnalysis.Accessibility.NotApplicable;
    }

    #endregion
  }
}
