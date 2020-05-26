// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
// Originated from: Roslyn, Features, Common/Glyph.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.Common
{
  // Glyph must be mirrored here because we need it as resource key in Generic.xaml
  // Please keep it up-to-date

  public enum Glyph
  {

        None,

        Assembly,

        BasicFile,
        BasicProject,

        ClassPublic,
        ClassProtected,
        ClassPrivate,
        ClassInternal,

        CSharpFile,
        CSharpProject,

        ConstantPublic,
        ConstantProtected,
        ConstantPrivate,
        ConstantInternal,

        DelegatePublic,
        DelegateProtected,
        DelegatePrivate,
        DelegateInternal,

        EnumPublic,
        EnumProtected,
        EnumPrivate,
        EnumInternal,

        EnumMemberPublic,
        EnumMemberProtected,
        EnumMemberPrivate,
        EnumMemberInternal,

        Error,
        StatusInformation,

        EventPublic,
        EventProtected,
        EventPrivate,
        EventInternal,

        ExtensionMethodPublic,
        ExtensionMethodProtected,
        ExtensionMethodPrivate,
        ExtensionMethodInternal,

        FieldPublic,
        FieldProtected,
        FieldPrivate,
        FieldInternal,

        InterfacePublic,
        InterfaceProtected,
        InterfacePrivate,
        InterfaceInternal,

        Intrinsic,

        Keyword,

        Label,

        Local,

        Namespace,

        MethodPublic,
        MethodProtected,
        MethodPrivate,
        MethodInternal,

        ModulePublic,
        ModuleProtected,
        ModulePrivate,
        ModuleInternal,

        OpenFolder,

        Operator,

        Parameter,

        PropertyPublic,
        PropertyProtected,
        PropertyPrivate,
        PropertyInternal,

        RangeVariable,

        Reference,

        StructurePublic,
        StructureProtected,
        StructurePrivate,
        StructureInternal,

        TypeParameter,

        Snippet,

        CompletionWarning,

        AddReference,
        NuGet,
        TargetTypeMatch

  }
}
