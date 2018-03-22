// Copyright (c) 2018 Dr. Dirk Lellinger. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Markdig.Renderers
{
    /// <summary>
    /// Basic implementation of a <see cref="IWpfImageProvider"/>. Use <see cref="Instance"/> to get an instance of this class.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.IWpfImageProvider" />
    public class WpfImageProviderBase : IWpfImageProvider
    {
        public static IWpfImageProvider Instance { get; private set; } = new WpfImageProviderBase();

        protected WpfImageProviderBase()
        {
        }

        /// <inheritdoc/>
        public virtual Inline GetInlineItem(string url, out bool inlineItemIsErrorMessage)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                inlineItemIsErrorMessage = false;
                return null;
            }
            try
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute))
                };
                inlineItemIsErrorMessage = false;
                return new InlineUIContainer(image);
            }
            catch (Exception ex)
            {
                inlineItemIsErrorMessage = true;
                return new Run(string.Format("ERROR RENDERING '{0}' ({1})", url, ex.Message));
            }
        }

        /// <inheritdoc/>
        public virtual void ClearCache()
        {
            // nothing to do here since not image is cached here
        }

        public virtual IUrlCollector CreateUrlCollector()
        {
            return null; // not interested here in collecting Urls
        }

        public virtual void UpdateUrlCollector(IUrlCollector collector, long updateSequenceNumber)
        {
            throw new InvalidOperationException("If CreateUrlCollector has been overridden, UpdateUrlCollector also must be overridden.");
        }
    }
}
