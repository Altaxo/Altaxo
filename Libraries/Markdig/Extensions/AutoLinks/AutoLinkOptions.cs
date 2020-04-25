namespace Markdig.Extensions.AutoLinks
{
    public class AutoLinkOptions
    {
        public AutoLinkOptions()
        {
            ValidPreviousCharacters = "*_~(";
        }

        public string ValidPreviousCharacters { get; set; }

        /// <summary>
        /// Should the link open in a new window when clicked (false by default)
        /// </summary>
        public bool OpenInNewWindow { get; set; }

        /// <summary>
        /// Should a www link be prefixed with https:// instead of http:// (false by default)
        /// </summary>
        public bool UseHttpsForWWWLinks { get; set; }
    }
}
