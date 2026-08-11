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

#nullable enable
namespace Altaxo.Main
{
  /// <summary>
  /// Stores information about how a graph is shown in the graph view.
  /// </summary>
  public class ActionViewLayout : IProjectItemPresentationModel
  {
    private ActionDocument _document;

    /// <summary>Initializes a new instance of the <see cref="ActionViewLayout"/> class.</summary>
    /// <param name="document">The graph document.</param>
    public ActionViewLayout(ActionDocument document)
    {
      _document = document;
    }

    private ActionViewLayout(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _document = null!;
    }

    #region Serialization

    /// <summary>
    /// 2026-08-11 V0: initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ActionViewLayout), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      private AbsoluteDocumentPath? _pathToDocument;
      private ActionViewLayout? _actionViewLayout;

      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ActionViewLayout)o;
        info.AddValue("DocumentPath", AbsoluteDocumentPath.GetAbsolutePath(s._document));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ActionViewLayout?)o ?? new ActionViewLayout(info);

        var surr = new XmlSerializationSurrogate0
        {
          _actionViewLayout = s,
          _pathToDocument = (AbsoluteDocumentPath)info.GetValue("DocumentPath", s)
        };
        info.DeserializationFinished += surr.EhDeserializationFinished;

        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
      {
        var o = AbsoluteDocumentPath.GetObject(_pathToDocument!, (Main.IDocumentNode)documentRoot);
        if (o is ActionDocument ad && _actionViewLayout is not null)
        {
          _actionViewLayout._document = (ActionDocument)o;
          info.DeserializationFinished -= EhDeserializationFinished;
        }
      }
    }

    #endregion Serialization

    /// <summary>Get the instance of the action document that is shown in the view.</summary>
    public ActionDocument Document => _document;

    /// <inheritdoc />
    IProjectItem IProjectItemPresentationModel.Document => _document;
  }
}
