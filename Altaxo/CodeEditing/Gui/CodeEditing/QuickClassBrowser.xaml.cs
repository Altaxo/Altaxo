#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.CodeEditing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.Gui.CodeEditing
{
  /// <summary>
  /// QuickClassBrowser is a bar above the CodeEditor that shows classes and methods contained in the code.
  /// </summary>
  /// <seealso cref="System.Windows.Controls.UserControl" />
  /// <seealso cref="System.Windows.Markup.IComponentConnector" />
  public partial class QuickClassBrowser : UserControl
  {
    #region Item classes

    /// <summary>
    /// Model item for the class ComboBox.
    /// </summary>
    private class ClassItem : IComparable<ClassItem>
    {
      public ClassDeclarationSyntax ClassDeclarationSyntax { get; private set; }
      public ImageSource Image { get; private set; }
      public string Text { get; private set; }

      public bool IsInSamePart { get { return true; } }

      public ClassItem(ClassDeclarationSyntax syntax)
      {
        ClassDeclarationSyntax = syntax;
        Text = syntax.Identifier.Text;

        var glyph = Glyph.ClassPublic;

        foreach (var mod in syntax.Modifiers)
        {
          if (mod.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
          {
            glyph = Glyph.ClassPublic;
            break;
          }
          else if (mod.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword))
          {
            glyph = Glyph.ClassProtected;
            break;
          }
          else if (mod.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword))
          {
            glyph = Glyph.ClassPrivate;
            break;
          }
          else if (mod.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword))
          {
            glyph = Glyph.ClassInternal;
            break;
          }
        }

        Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(glyph);
      }

      public int CompareTo(ClassItem other)
      {
        return string.Compare(Text, other.Text);
      }
    }

    /// <summary>
    /// Model item for the methods-fields-properties-events ComboBox.
    /// </summary>
    private class SyntaxItem
    {
      public SyntaxNode SyntaxNode { get; private set; }
      public ImageSource Image { get; private set; }
      public string Text { get; private set; }

      public bool IsInSamePart { get { return true; } }

      public static readonly Glyph[] fieldGlyphs = new[] { Glyph.FieldPublic, Glyph.FieldProtected, Glyph.FieldPrivate, Glyph.FieldInternal };
      public static readonly Glyph[] enumGlyphs = new[] { Glyph.EnumPublic, Glyph.EnumProtected, Glyph.EnumPrivate, Glyph.EnumInternal };
      public static readonly Glyph[] eventGlyphs = new[] { Glyph.EnumPublic, Glyph.EventProtected, Glyph.EventPrivate, Glyph.EventInternal };
      public static readonly Glyph[] constantGlyphs = new[] { Glyph.ConstantPublic, Glyph.ConstantProtected, Glyph.ConstantPrivate, Glyph.ConstantInternal };
      public static readonly Glyph[] methodGlyphs = new[] { Glyph.MethodPublic, Glyph.MethodProtected, Glyph.MethodPrivate, Glyph.MethodInternal };
      public static readonly Glyph[] propertyGlyphs = new[] { Glyph.PropertyPublic, Glyph.PropertyProtected, Glyph.PropertyPrivate, Glyph.PropertyInternal };
      public static readonly Glyph[] structGlyphs = new[] { Glyph.StructurePublic, Glyph.StructureProtected, Glyph.StructurePrivate, Glyph.StructureInternal };

      public SyntaxItem(SyntaxNode syntax)
      {
        SyntaxNode = syntax;

        int modifierIndex = 0;

        var modifiers = new SyntaxTokenList();

        if (syntax is BaseTypeDeclarationSyntax dtds)
          modifiers = dtds.Modifiers;
        else if (syntax is BaseMethodDeclarationSyntax bmds)
          modifiers = bmds.Modifiers;
        else if (syntax is BasePropertyDeclarationSyntax bpds)
          modifiers = bpds.Modifiers;
        else if (syntax is BaseFieldDeclarationSyntax bfds)
          modifiers = bfds.Modifiers;

        foreach (var mod in modifiers)
        {
          if (mod.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
          {
            modifierIndex = 0;
            break;
          }
          else if (mod.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword))
          {
            modifierIndex = 1;
            break;
          }
          else if (mod.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword))
          {
            modifierIndex = 2;
            break;
          }
          else if (mod.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword))
          {
            modifierIndex = 3;
            break;
          }
        }

        if (syntax is ConstructorDeclarationSyntax cds)
        {
          Text = cds.Identifier.ValueText;
          Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(methodGlyphs[modifierIndex]);
        }
        else if (syntax is MethodDeclarationSyntax mds)
        {
          Text = mds.Identifier.Text;
          Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(methodGlyphs[modifierIndex]);
        }
        else if (syntax is PropertyDeclarationSyntax pds)
        {
          Text = pds.Identifier.Text;
          Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(propertyGlyphs[modifierIndex]);
        }
        else if (syntax is EventDeclarationSyntax eds)
        {
          Text = eds.Identifier.Text;
          Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(eventGlyphs[modifierIndex]);
        }
        else if (syntax is EventFieldDeclarationSyntax evfds)
        {
          Text = evfds.Declaration.Variables[0].Identifier.Text;
          Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(eventGlyphs[modifierIndex]);
        }
        else if (syntax is FieldDeclarationSyntax fds)
        {
          Text = fds.Declaration.Variables[0].Identifier.Text;
          Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(fieldGlyphs[modifierIndex]);
        }
        else if (syntax is EnumDeclarationSyntax enumDs)
        {
          Text = enumDs.Identifier.Text;
          Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(enumGlyphs[modifierIndex]);
        }
        else if (syntax is StructDeclarationSyntax structDs)
        {
          Text = structDs.Identifier.Text;
          Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(structGlyphs[modifierIndex]);
        }
        else
        {
          Text = syntax.ToString();
        }
      }
    }

    #endregion Item classes

    /// <summary>
    /// List of classes that occur in the code.
    /// </summary>
    private List<ClassItem> _classItems = new List<ClassItem>();

    /// <summary>
    /// List of members (methods, events, properties, fields) that occur in the code. The content depend on the selection of the class.
    /// </summary>
    private List<SyntaxItem> _memberItems = new List<SyntaxItem>();

    // Delayed execution - avoid changing combo boxes while the user is browsing the dropdown list.
    private bool _runUpdateWhenDropDownClosed;

    private SyntaxNode _runUpdateWhenDropDownClosedSyntaxRootNode;
    private bool _runSelectItemWhenDropDownClosed;
    private int _runSelectItemWhenDropDownClosedLocation;

    private bool _doJumpOnSelectionChange = true;

    /// <summary>
    /// Action used for jumping to a position inside the current file. Argument is the caret position to jump to.
    /// </summary>
    public Action<int> JumpAction { get; set; }

    public QuickClassBrowser()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Updates the list of available classes.
    /// This causes the classes combo box to lose its current selection,
    /// so the members combo box will be cleared.
    /// </summary>
    public void Update(SyntaxNode documentRootNode)
    {
      _runUpdateWhenDropDownClosed = true;
      _runUpdateWhenDropDownClosedSyntaxRootNode = documentRootNode;
      if (!IsDropDownOpen)
        EhBothComboBoxes_DropDownClosed(null, null);
    }

    /// <summary>
    /// Update the underlying syntax tree to get the list of classes, and for the selected class, the members.
    /// </summary>
    /// <param name="documentRootNode">The document root node of the syntax tree.</param>
    private void InternalUpdateSyntax(SyntaxNode documentRootNode)
    {
      var classItems = new List<ClassItem>(_classItems.Count);
      if (documentRootNode != null)
      {
        var classes = ClassDeclarationVisitor.GetAllClasses(documentRootNode);
        foreach (var c in classes)
        {
          classItems.Add(new ClassItem(c));
        }
      }
      classItems.Sort();

      // test if this is identical
      bool changed = false;

      changed = changed || classItems.Count != _classItems.Count;

      if (!changed)
      {
        for (int i = 0; i < classItems.Count; ++i)
        {
          if (classItems[i].Text != _classItems[i].Text)
          {
            changed = true;
            break;
          }
        }
      }

      if (!changed)
      {
        for (int i = 0; i < _classItems.Count; ++i)
          _classItems[i] = classItems[i];
      }
      else
      {
        _classItems = classItems;
        _doJumpOnSelectionChange = false;
        classComboBox.ItemsSource = _classItems;
        DoSelectItem(_runSelectItemWhenDropDownClosedLocation);
        _doJumpOnSelectionChange = true;
      }
    }

    private bool IsDropDownOpen
    {
      get { return classComboBox.IsDropDownOpen || membersComboBox.IsDropDownOpen; }
    }

    private void EhBothComboBoxes_DropDownClosed(object sender, EventArgs e)
    {
      if (_runUpdateWhenDropDownClosed)
      {
        _runUpdateWhenDropDownClosed = false;
        InternalUpdateSyntax(_runUpdateWhenDropDownClosedSyntaxRootNode);
        _runUpdateWhenDropDownClosedSyntaxRootNode = null;
      }
      if (_runSelectItemWhenDropDownClosed)
      {
        _runSelectItemWhenDropDownClosed = false;
        DoSelectItem(_runSelectItemWhenDropDownClosedLocation);
      }
    }

    /// <summary>
    /// Selects the class and member closest to the specified location.
    /// </summary>
    public void SelectItemAtCaretPosition(int caretOffset)
    {
      _runSelectItemWhenDropDownClosed = true;
      _runSelectItemWhenDropDownClosedLocation = caretOffset;
      if (!IsDropDownOpen)
        EhBothComboBoxes_DropDownClosed(null, null);
    }

    /// <summary>
    /// Determines whether <paramref name="offset"/> lies inside the text span <paramref name="span"/>.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="span">The span.</param>
    /// <returns></returns>
    private bool Intersects(int offset, TextSpan span)
    {
      return span.Start <= offset && offset <= span.End;
    }

    /// <summary>
    /// Selected the class item that is nearest to the caret position.
    /// </summary>
    /// <param name="caretOffset">The caret offset.</param>
    private void DoSelectItem(int caretOffset)
    {
      ClassItem matchClassItemInside = null;
      ClassItem nearestClassItemMatch = null;
      int nearestMatchDistance = int.MaxValue;
      foreach (var item in _classItems)
      {
        var span = item.ClassDeclarationSyntax.GetLocation().SourceSpan;
        if (Intersects(caretOffset, span))
        {
          // when there are multiple matches inside (nested classes), use the last one
          matchClassItemInside = item;
        }
        else
        {
          // Not a perfect match?
          // Try to first the nearest match. We want the classes combo box to always
          // have a class selected if possible.
          int matchDistance = Math.Min(Math.Abs(caretOffset - span.Start), Math.Abs(caretOffset - span.End));
          if (matchDistance < nearestMatchDistance)
          {
            nearestMatchDistance = matchDistance;
            nearestClassItemMatch = item;
          }
        }
      }

      _doJumpOnSelectionChange = false;
      try
      {
        classComboBox.SelectedItem = matchClassItemInside ?? nearestClassItemMatch;
        // the SelectedItem setter will update the list of member items
      }
      finally
      {
        _doJumpOnSelectionChange = true;
      }

      // finished selection of class item

      // now find the nearest member item

      SyntaxItem matchMemberItemInside = null;
      SyntaxItem nearestMemberItemMatch = null;
      nearestMatchDistance = int.MaxValue;
      foreach (var item in _memberItems)
      {
        if (item.IsInSamePart)
        {
          var member = item.SyntaxNode;
          var memberSpan = item.SyntaxNode.GetLocation().SourceSpan;

          if (Intersects(caretOffset, memberSpan))
          {
            matchMemberItemInside = item;
          }
          else
          {
            // Not a perfect match?
            // Try to first the nearest match. We want the classes combo box to always
            // have a class selected if possible.
            int matchDistance = Math.Min(Math.Abs(caretOffset - memberSpan.Start), Math.Abs(caretOffset - memberSpan.End));
            if (matchDistance < nearestMatchDistance)
            {
              nearestMatchDistance = matchDistance;
              nearestMemberItemMatch = item;
            }
          }
        }
      }
      _doJumpOnSelectionChange = false;
      try
      {
        membersComboBox.SelectedItem = matchMemberItemInside ?? nearestMemberItemMatch;
      }
      finally
      {
        _doJumpOnSelectionChange = true;
      }
    }

    private void EhClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (classComboBox.SelectedItem is ClassItem classItem)
      {
        // The selected class was changed.
        // Update the list of member items to be the list of members of the current class.
        var classDeclaration = classItem.ClassDeclarationSyntax;

        var vis = new MethodVisitor();
        classDeclaration.Accept(vis);

        var items = new List<SyntaxItem>(vis.Symbols.Select(x => new SyntaxItem(x)));
        items.Sort((x, y) => string.Compare(x.Text, y.Text));
        if (_doJumpOnSelectionChange)
        {
          JumpTo(classDeclaration.GetLocation().SourceSpan);
        }

        _memberItems = items;
        membersComboBox.ItemsSource = _memberItems;
      }
    }

    private void EhMembersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_doJumpOnSelectionChange && membersComboBox.SelectedItem is SyntaxItem syntaxItem)
      {
        JumpTo(syntaxItem.SyntaxNode.GetLocation().SourceSpan);
      }
    }

    private void JumpTo(TextSpan location)
    {
      if (null != location)
      {
        JumpAction?.Invoke(location.Start);
      }
    }
  }
}
