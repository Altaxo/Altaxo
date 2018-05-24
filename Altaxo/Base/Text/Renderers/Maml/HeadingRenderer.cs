#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text.Renderers.Maml
{
	public class HeadingRenderer : MamlObjectRenderer<HeadingBlock>
	{
		protected override void Write(MamlRenderer renderer, HeadingBlock obj)
		{
			bool newFileWasStarted = renderer.TryStartNewMamlFile(obj);

			if (newFileWasStarted)
			{
			}
			else
			{
				// Ensure we have the sections element somewhere down the line ...
				// we need (obj.Level - renderer.SplitLevel) section elements down the stack
				int numberOfSectionsElementsRequired = obj.Level - renderer.SplitLevel - 1;
				int numberOfSectionsElementsOnStack = renderer.NumberOfElementsOnStack(MamlElements.sections);

				// Push sections element if required
				for (int i = 0; i < numberOfSectionsElementsRequired - numberOfSectionsElementsOnStack; ++i)
					renderer.Push(MamlElements.sections);

				if (numberOfSectionsElementsOnStack > 0 && numberOfSectionsElementsRequired > 0)
				{
					// Or pop sections elements if required
					for (int i = 0; i < numberOfSectionsElementsOnStack - numberOfSectionsElementsRequired; ++i)
						renderer.PopToBefore(MamlElements.sections);
				}

				if (numberOfSectionsElementsRequired == 0)
				{
					renderer.PopToBefore(MamlElements.developerConceptualDocument);
				}

				// Find a unique address in order for AutoOutline to work
				var attr = (Markdig.Renderers.Html.HtmlAttributes)obj.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
				string uniqueAddress = attr?.Id; // this header has a user defined address
				if (string.IsNullOrEmpty(uniqueAddress))
					renderer.HeaderGuids.TryGetValue(obj.Span.Start, out uniqueAddress); // use the guid generated from the hierarchy of titles
				if (string.IsNullOrEmpty(uniqueAddress))
					uniqueAddress = Guid.NewGuid().ToString(); // fallback (should not happen): Create Guid

				renderer.Push(MamlElements.section, new[] { new KeyValuePair<string, string>("address", uniqueAddress) });

				renderer.Push(MamlElements.title);
				renderer.WriteLeafInline(obj);
				renderer.EnsureLine();
				renderer.PopTo(MamlElements.title);
				renderer.Push(MamlElements.content);
			}
		}
	}
}
