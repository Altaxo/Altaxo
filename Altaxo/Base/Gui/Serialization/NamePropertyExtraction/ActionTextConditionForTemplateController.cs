#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main;
using Altaxo.Serialization.NamePropertyExtraction;

namespace Altaxo.Gui.Serialization.NamePropertyExtraction
{

  /// <summary>
  /// Interface for the view of the ActionTestConditionForTemplateController.
  /// </summary>
  public interface IActionTextConditionForTemplateView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing an <see cref="ActionTextConditionForTemplate"/> instance.
  /// </summary>
  [ExpectedTypeOfView(typeof(IActionTextConditionForTemplateView))]
  [UserControllerForObject(typeof(ActionTextConditionForTemplate))]
  public class ActionTextConditionForTemplateController : MVCANControllerEditImmutableDocBase<ActionTextConditionForTemplate, IActionTextConditionForTemplateView>
  {
    /// <summary>
    /// Gets the subcontrollers used by this controller.
    /// </summary>
    /// <returns>An enumeration of subcontrollers.</returns>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the property name to evaluate.
    /// </summary>
    public string PropertyName
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(PropertyName));
        }
      }
    }

    /// <summary>
    /// Gets or sets the condition text used for evaluation.
    /// </summary>
    public string Condition
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Condition));
        }
      }
    }

    /// <summary>
    /// Gets or sets the kind of comparison to use when evaluating the condition.
    /// </summary>
    public ItemsController<StringComparisonKind> ConditionComparisonKind
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ConditionComparisonKind));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether condition comparison is case-sensitive.
    /// </summary>
    public bool IsConditionCaseSensitive
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IsConditionCaseSensitive));
        }
      }
    }

    /// <summary>
    /// Gets or sets the project items used as templates.
    /// </summary>
    public string ProjectItemsUsedAsTemplate
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ProjectItemsUsedAsTemplate));
        }
      }
    }


    /// <summary>
    /// Gets or sets the overwrite behavior for copying project items.
    /// </summary>
    public ItemsController<OverwriteBehavior> OverwriteProjectItems
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(OverwriteProjectItems));
        }
      }
    }


    #endregion Bindings

    /// <summary>
    /// Initializes the controller state from the document.
    /// </summary>
    /// <param name="initData">If set to <see langword="true"/>, initializes binding properties from the document.</param>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        PropertyName = _doc.PropertyName;
        Condition = _doc.Condition;

        var conditionComparisonKindList = new SelectableListNodeList(_doc.ConditionComparisonKind);
        foreach (var item in conditionComparisonKindList)
        {
          item.Text = (StringComparisonKind)(item.Tag) switch
          {
            StringComparisonKind.Equality => "is equal to",
            StringComparisonKind.Inequality => "is not equal to",
            StringComparisonKind.Contains => "contains",
            StringComparisonKind.StartsWith => "starts with",
            StringComparisonKind.EndsWith => "ends with",
            _ => item.Tag.ToString() ?? string.Empty,
          };
        }

        ConditionComparisonKind = new ItemsController<StringComparisonKind>(conditionComparisonKindList);
        IsConditionCaseSensitive = _doc.IsConditionCaseSensitive;
        ProjectItemsUsedAsTemplate = string.Join(Environment.NewLine, _doc.ProjectItemsUsedAsTemplate);
        OverwriteProjectItems = new ItemsController<OverwriteBehavior>(new SelectableListNodeList(_doc.OverwriteProjectItems));
      }
    }

    /// <summary>
    /// Applies the current controller values to the immutable document.
    /// </summary>
    /// <param name="disposeController">If set to <see langword="true"/>, disposes the controller after apply.</param>
    /// <returns><see langword="true"/> if applying succeeded; otherwise <see langword="false"/>.</returns>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        PropertyName = PropertyName,
        Condition = Condition,
        ConditionComparisonKind = ConditionComparisonKind.SelectedValue,
        IsConditionCaseSensitive = IsConditionCaseSensitive,
        ProjectItemsUsedAsTemplate = ProjectItemsUsedAsTemplate.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                    .Where(s => !string.IsNullOrWhiteSpace(s))
                                    .ToImmutableList(),
        OverwriteProjectItems = OverwriteProjectItems.SelectedValue
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
