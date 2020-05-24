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
    None = 0,
    Assembly = 1,
    BasicFile = 2,
    BasicProject = 3,
    ClassPublic = 4,
    ClassProtected = 5,
    ClassPrivate = 6,
    ClassInternal = 7,
    CSharpFile = 8,
    CSharpProject = 9,
    ConstantPublic = 10,
    ConstantProtected = 11,
    ConstantPrivate = 12,
    ConstantInternal = 13,
    DelegatePublic = 14,
    DelegateProtected = 15,
    DelegatePrivate = 16,
    DelegateInternal = 17,
    EnumPublic = 18,
    EnumProtected = 19,
    EnumPrivate = 20,
    EnumInternal = 21,
    EnumMemberPublic = 22,
    EnumMemberProtected = 23,
    EnumMemberPrivate = 24,
    EnumMemberInternal = 25,
    Error = 26,
    StatusInformation = 27,
    EventPublic = 28,
    EventProtected = 29,
    EventPrivate = 30,
    EventInternal = 31,
    ExtensionMethodPublic = 32,
    ExtensionMethodProtected = 33,
    ExtensionMethodPrivate = 34,
    ExtensionMethodInternal = 35,
    FieldPublic = 36,
    FieldProtected = 37,
    FieldPrivate = 38,
    FieldInternal = 39,
    InterfacePublic = 40,
    InterfaceProtected = 41,
    InterfacePrivate = 42,
    InterfaceInternal = 43,
    Intrinsic = 44,
    Keyword = 45,
    Label = 46,
    Local = 47,
    Namespace = 48,
    MethodPublic = 49,
    MethodProtected = 50,
    MethodPrivate = 51,
    MethodInternal = 52,
    ModulePublic = 53,
    ModuleProtected = 54,
    ModulePrivate = 55,
    ModuleInternal = 56,
    OpenFolder = 57,
    Operator = 58,
    Parameter = 59,
    PropertyPublic = 60,
    PropertyProtected = 61,
    PropertyPrivate = 62,
    PropertyInternal = 63,
    RangeVariable = 64,
    Reference = 65,
    StructurePublic = 66,
    StructureProtected = 67,
    StructurePrivate = 68,
    StructureInternal = 69,
    TypeParameter = 70,
    Snippet = 71,
    CompletionWarning = 72,
    AddReference = 73,
    NuGet = 74,
    TargetTypeMatch = 75
  }
}
