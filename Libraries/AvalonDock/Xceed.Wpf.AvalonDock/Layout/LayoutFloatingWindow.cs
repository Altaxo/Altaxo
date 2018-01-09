﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Xceed.Wpf.AvalonDock.Layout
{
    [Serializable]
    public abstract class LayoutFloatingWindow : LayoutElement, ILayoutContainer, IXmlSerializable
  {
        public LayoutFloatingWindow()
        { 

        }


        public abstract IEnumerable<ILayoutElement> Children { get; }

        public abstract void RemoveChild(ILayoutElement element);

        public abstract void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);

        public abstract int ChildrenCount { get; }

        public abstract bool IsValid { get; }

        public XmlSchema GetSchema()
        {
          return null;
        }

        public abstract void ReadXml( XmlReader reader );

        public virtual void WriteXml( XmlWriter writer )
        {
          foreach( var child in Children )
          {
            var type = child.GetType();
            var serializer = new XmlSerializer( type );
            serializer.Serialize( writer, child );
          }
        }

  }
}
