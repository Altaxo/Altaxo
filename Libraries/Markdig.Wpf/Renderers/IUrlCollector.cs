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
    /// Collects referenced Urls from a parsed markdown document.
    /// </summary>
    public interface IUrlCollector
    {
        /// <summary>
        /// Adds an URL to this collector.
        /// </summary>
        /// <param name="isImage">if set to <c>true</c>, this Url represents an image.</param>
        /// <param name="url">The URL.</param>
        /// <param name="urlSpanStart">Start of the url in the source text.</param>
        /// <param name="urlSpanEnd">End of the url in the source text.</param>
        void AddUrl(bool isImage, string url, int urlSpanStart, int urlSpanEnd);

        /// <summary>
        /// Announces that the collection process has finished. The class can freeze all members to make it effecively readonly.
        /// </summary>
        void Freeze();
    }
}
