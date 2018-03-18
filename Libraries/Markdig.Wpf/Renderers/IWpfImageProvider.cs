// Copyright (c) 2018 Dr. Dirk Lellinger. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Markdig.Renderers
{
    /// <summary>
    /// Interface used to provide Wpf images for a <see cref="FlowDocument"/>.
    /// </summary>
    public interface IWpfImageProvider
    {
        /// <summary>
        /// Gets the item for an inline image.
        /// This should normally be a <see cref="InlineUIContainer"/> with an image as its child,
        /// but it can be any <see cref="TextElement"/>, e.g. a <see cref="Run"/> with an error message if the image is not available.
        /// </summary>
        /// <param name="url">The URL used to retrieve the image.</param>
        /// <param name="inlineItemIsErrorMessage">If true, the returned item is an error message. This flag can be used e.g. to apply a different style to such an item.</param>
        /// <returns>If this image provider could handle the Url, the return value is
        /// the inline text element containing the image, or anything else (e.g. a <see cref="Run"/> containing an error message.
        /// If this image provider was not able to handle the provided url, the return value is null.</returns>
        /// <remarks>There is no need to apply a style to the image, as long as it packed in the <see cref="InlineUIContainer"/>.
        /// Assigment of a style is done during rendering.</remarks>
        Inline GetInlineItem(string url, out bool inlineItemIsErrorMessage);

        /// <summary>
        /// Removes all cached images and other resources, in order to
        /// force a fresh retrieval of those resources.
        /// </summary>
        void ClearCache();
    }
}
