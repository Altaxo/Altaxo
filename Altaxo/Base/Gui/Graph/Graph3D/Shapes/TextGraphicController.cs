#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#nullable disable
using System.Collections.Generic;
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Graph3D.Shapes
{
  using System.Windows.Input;
  using Altaxo.Drawing.D3D;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.Shapes;
  using Altaxo.Gui.Common.Drawing.D3D;
  using Altaxo.Gui.Graph.Graph3D.Background;
  using Altaxo.Units;

  #region Interfaces

  public interface ITextGraphicView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(TextGraphic))]
  [ExpectedTypeOfView(typeof(ITextGraphicView))]
  public class TextGraphicController : MVCANControllerEditOriginalDocBase<TextGraphic, ITextGraphicView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_locationController, () => LocationController = null);
      yield return new ControllerAndSetNullMethod(_backgroundController, () => BackgroundController = null);
      yield return new ControllerAndSetNullMethod(_fontController, () => FontController = null);
    }

    public TextGraphicController()
    {
      CmdNormal = new RelayCommand(EhView_NormalClick);
      CmdBold = new RelayCommand(EhView_BoldClick);
      CmdItalic = new RelayCommand(EhView_ItalicClick);
      CmdUnderline = new RelayCommand(EhView_UnderlineClick);
      CmdStrikeout = new RelayCommand(EhView_StrikeoutClick);
      CmdSupIndex = new RelayCommand(EhView_SupIndexClick);
      CmdSubIndex = new RelayCommand(EhView_SubIndexClick);
      CmdGreek = new RelayCommand(EhView_GreekClick);
      CmdMoreModifiers = new RelayCommand<string>(EhView_MoreModifiersClick);

    }

    #region Bindings

    public ICommand CmdNormal { get; }
    public ICommand CmdBold { get; }
    public ICommand CmdItalic { get; }
    public ICommand CmdUnderline { get; }
    public ICommand CmdStrikeout { get; }
    public ICommand CmdSupIndex { get; }
    public ICommand CmdSubIndex { get; }
    public ICommand CmdGreek { get; }
    public ICommand CmdMoreModifiers { get; }

    private IMVCANController _locationController;

    public IMVCANController LocationController
    {
      get => _locationController;
      set
      {
        if (!(_locationController == value))
        {
          _locationController?.Dispose();
          _locationController = value;
          OnPropertyChanged(nameof(LocationController));
        }
      }
    }

    private string _editText;

    public string EditText
    {
      get => _editText;
      set
      {
        if (!(_editText == value))
        {
          _editText = value;
          OnPropertyChanged(nameof(EditText));
          _doc.Text = value;
          ++DocumentVersion;
        }
      }
    }

    private (int Start, int Length) _textSelection;

    public (int Start, int Length) TextSelection
    {
      get => _textSelection;
      set
      {
        if (!(_textSelection == value))
        {
          _textSelection = value;
          OnPropertyChanged(nameof(TextSelection));
        }
      }
    }

    private BackgroundStyleController _backgroundController;

    public BackgroundStyleController BackgroundController
    {
      get => _backgroundController;
      set
      {
        if (!(_backgroundController == value))
        {
          if (_backgroundController is { } oldC)
          {
            oldC.MadeDirty -= EhBackgroundStyleChanged;
          }
          _backgroundController?.Dispose();
          _backgroundController = value;
          if (_backgroundController is { } newC)
          {
            newC.MadeDirty += EhBackgroundStyleChanged;
          }

          OnPropertyChanged(nameof(BackgroundController));
        }
      }
    }

    private void EhBackgroundStyleChanged(IMVCANDController obj)
    {
      _doc.Background = BackgroundController.Doc;
      ++DocumentVersion;
    }

    private FontX3DController _fontController;

    public FontX3DController FontController
    {
      get => _fontController;
      set
      {
        if (!(_fontController == value))
        {
          if (_fontController is { } oldC)
          {
            oldC.MadeDirty -= EhFontChanged;
          }
          _fontController?.Dispose();
          _fontController = value;
          if (_fontController is { } newC)
          {
            newC.MadeDirty += EhFontChanged;
          }
          OnPropertyChanged(nameof(FontController));
        }
      }
    }

    public void EhFontChanged(IMVCAController _)
    {
      _doc.Font = FontController.Doc;
      ++DocumentVersion;
    }

    private IMaterial _fontBrush;

    public IMaterial FontBrush
    {
      get => _fontBrush;
      set
      {
        if (!(_fontBrush == value))
        {
          _fontBrush = value;
          OnPropertyChanged(nameof(FontBrush));
          _doc.TextFillBrush = value;
          ++DocumentVersion;
        }
      }
    }


    public QuantityWithUnitGuiEnvironment LineSpacingEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _lineSpacing;

    public DimensionfulQuantity LineSpacing
    {
      get => _lineSpacing;
      set
      {
        if (!(_lineSpacing == value))
        {
          _lineSpacing = value;
          OnPropertyChanged(nameof(LineSpacing));
          _doc.LineSpacing = value.AsValueInSIUnits;
          ++DocumentVersion;
        }
      }
    }

    private int _documentVersion;

    public int DocumentVersion
    {
      get => _documentVersion;
      set
      {
        if (!(_documentVersion == value))
        {
          _documentVersion = value;
          OnPropertyChanged(nameof(DocumentVersion));
          OnPropertyChanged(nameof(Doc));
        }
      }
    }



    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
        Current.Gui.FindAndAttachControlTo(locationController);
        LocationController = locationController;
        BackgroundController = new BackgroundStyleController(_doc.Background);
        FontController = new FontX3DController(_doc.Font);
        LineSpacing = new DimensionfulQuantity(_doc.LineSpacing, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineSpacingEnvironment.DefaultUnit);

        // fill the color dialog box
        FontBrush = _doc.TextFillBrush;
        EditText = _doc.Text;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_locationController.Apply(disposeController))
        return false;

      if (!object.ReferenceEquals(_doc.Location, _locationController.ModelObject))
        _doc.Location.CopyFrom((ItemLocationDirect)_locationController.ModelObject);

      return ApplyEnd(true, disposeController);
    }

    public void InsertBeforeAndAfterSelectedText(string insbefore, string insafter)
    {
      if (0 != TextSelection.Length)
      {
        // insert \b( at beginning of selection and ) at the end of the selection
        int len = TextSelection.Length;
        int start = TextSelection.Start;
        int end = start + len;
        EditText = EditText.Substring(0, start) + insbefore + EditText.Substring(start, end - start) + insafter + EditText.Substring(end, EditText.Length - end);

        // now select the text plus the text before and after
        TextSelection = (start, end - start + insbefore.Length + insafter.Length);
        OnPropertyChanged(nameof(TextSelection));
      }
    }

    public void RevertToNormal()
    {
      // remove a backslash x ( at the beginning and the closing brace at the end of the selection
      if (TextSelection.Length >= 4)
      {
        int len = TextSelection.Length;
        int start = TextSelection.Start;
        int end = start + len;

        if (EditText[start] == '\\' && EditText[start + 2] == '(' && EditText[end - 1] == ')')
        {
          EditText = EditText.Substring(0, start)
            + EditText.Substring(start + 3, end - start - 4)
            + EditText.Substring(end, EditText.Length - end);

          // now select again the rest of the text
          TextSelection = (start, end - start - 4);
        }
      }
    }

    public void EhView_BoldClick()
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\b(", ")");
    }

    public void EhView_ItalicClick()
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\i(", ")");
    }

    public void EhView_UnderlineClick()
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\u(", ")");
    }

    public void EhView_SupIndexClick()
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\+(", ")");
    }

    public void EhView_SubIndexClick()
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\-(", ")");
    }

    public void EhView_GreekClick()
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\g(", ")");
    }

    public void EhView_MoreModifiersClick(string parameter)
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      EditText = EditText + (string)parameter;
    }

    public void EhView_NormalClick()
    {
      RevertToNormal();
    }

    public void EhView_StrikeoutClick()
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\s(", ")");
    }
  }
}
