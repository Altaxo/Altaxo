// Copyright (c) 2016-2017 Nicolas Musset, 2018 Dr. Dirk Lellinger. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Markdig.Annotations;
using Markdig.Syntax.Inlines;
using Markdig.Wpf;

namespace Markdig.Renderers.Wpf.Inlines
{
    /// <summary>
    /// A WPF renderer for a <see cref="LinkInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Wpf.WpfObjectRenderer{Markdig.Syntax.Inlines.LinkInline}" />
    public class LinkInlineRenderer : WpfObjectRenderer<LinkInline>
    {
        /// <inheritdoc/>
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] LinkInline link)
        {
            var url = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;

            if (link.IsImage)
            {
                var inline = renderer.ImageProvider.GetInlineItem(url, out var inlineItemIsErrorMessage);

                double? width = null, height = null;

                if (link.ContainsData(typeof(Markdig.Renderers.Html.HtmlAttributes)))
                {
                    var htmlAttributes = (Markdig.Renderers.Html.HtmlAttributes)link.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
                    if (null != htmlAttributes.Properties)
                    {
                        foreach (var entry in htmlAttributes.Properties)
                        {
                            switch (entry.Key.ToLowerInvariant())
                            {
                                case "width":
                                    width = GetLength(entry.Value);
                                    break;

                                case "height":
                                    height = GetLength(entry.Value);
                                    break;
                            }
                        }
                    }
                }

                if (null != inline)
                {
                    inline.Tag = link;

                    if (inline is InlineUIContainer container && container.Child is Image image)
                    {
                        renderer.Styles.ApplyImageStyle(image);
                        if (width.HasValue && height.HasValue)
                        {
                            // then we do a non-uniform stretching
                            if (image.Source.Width >= width.Value && image.Source.Height >= height.Value)
                            {
                                image.Width = width.Value;
                                image.Height = height.Value;
                                image.Stretch = System.Windows.Media.Stretch.Fill;
                            }
                            else
                            {
                                // we have to use scale to up-scale the image
                                image.LayoutTransform = new System.Windows.Media.ScaleTransform(width.Value / image.Source.Width, height.Value / image.Source.Height);
                            }
                        }
                        else if (width.HasValue)
                        {
                            if (image.Source.Width >= width.Value)
                            {
                                image.Width = width.Value;
                                image.Stretch = System.Windows.Media.Stretch.Uniform;
                            }
                            else
                            {
                                // we have to use scale to up-scale the image
                                double scale = width.Value / image.Source.Width;
                                image.LayoutTransform = new System.Windows.Media.ScaleTransform(scale, scale);
                            }
                        }
                        else if (height.HasValue)
                        {
                            if (image.Source.Height >= height.Value)
                            {
                                image.Height = height.Value;
                                image.Stretch = System.Windows.Media.Stretch.Uniform;
                            }
                            else
                            {
                                // we have to use scale to up-scale the image
                                double scale = height.Value / image.Source.Height;
                                image.LayoutTransform = new System.Windows.Media.ScaleTransform(scale, scale);
                            }
                        }
                        else // neither width nor height provided
                        {
                            // it seems like a bug (or a feature?) in Wpf that to determine the size of the image,
                            // _not_ the width and height property of the image source is used.
                            // Instead it seems here that the PixelWidth and the PixelHeight is used and interpreted
                            // as 1/96th inch.
                            // We correct for that by assigning the image the width and height of the imageSource
                            image.Width = image.Source.Width;
                            image.Height = image.Source.Height;
                            image.Stretch = System.Windows.Media.Stretch.Uniform;
                        }
                    }

                    renderer.WriteInline(inline);
                }
            }
            else
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                {
                    url = "#";
                }

                var hyperlink = new Hyperlink
                {
                    Command = Commands.Hyperlink,
                    CommandParameter = url,
                    NavigateUri = new Uri(url, UriKind.RelativeOrAbsolute),
                    ToolTip = link.Title != string.Empty ? link.Title : null,
                    Tag = link
                };

                renderer.Styles.ApplyHyperlinkStyle(hyperlink);
                renderer.Push(hyperlink);
                renderer.WriteChildren(link);
                renderer.Pop();
            }
        }

        /// <summary>
        /// Gets the length in 1/96th inch.
        /// </summary>
        /// <param name="lenString">The length string.</param>
        /// <returns></returns>
        private double? GetLength(string lenString)
        {
            if (string.IsNullOrEmpty(lenString))
                return null;

            lenString = lenString.ToLowerInvariant().Trim();

            double factor = 1;
            string numberString = lenString;

            if (lenString.EndsWith("pt"))
            {
                factor = 96 / 72.0;
                numberString = lenString.Substring(0, lenString.Length - 2);
            }
            else if (lenString.EndsWith("cm"))
            {
                factor = 96 / 2.54;
                numberString = lenString.Substring(0, lenString.Length - 2);
            }
            else if (lenString.EndsWith("mm"))
            {
                factor = 96 / 25.4;
                numberString = lenString.Substring(0, lenString.Length - 2);
            }
            else if (lenString.EndsWith("px"))
            {
                factor = 1;
                numberString = lenString.Substring(0, lenString.Length - 2);
            }

            if (double.TryParse(numberString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var result))
            {
                return result * factor;
            }
            return null;
        }
    }
}
