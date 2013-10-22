using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CsQuery;
using Nustache.Mvc;
using Sundown;
using System.Linq;

namespace Nustache.Mvc3.Example.ViewEngine
{
    public class NustacheMarkdownView : NustacheView
    {
        private readonly Regex markdownSections = new Regex(Regex.Escape("<markdown>") + "(.*?)" + Regex.Escape("</markdown>"), 
            RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        private readonly static HtmlRenderMode htmlRenderMode = new HtmlRenderMode
        {
            SkipHtml = false,
            SkipStyle = false,
            SkipImages = false,
            SkipExpandTabs = false,
            SafeLink = false,
            TOC = false,
            HardWrap = false,
            UseXHTML = true,
            Escape = false,
            SkipLinks = false
        };

        private readonly static HtmlRenderer htmlRenderer = new HtmlRenderer(htmlRenderMode);

        private readonly static MarkdownExtensions markDownExtensions = new MarkdownExtensions
        {
            Autolink = true,
            Tables = true,
            FencedCode = true,
            Strikethrough = true,
            SpaceHeaders = true,
            SuperScript = true,
            LaxSpacing = true
        };

        public NustacheMarkdownView(NustacheViewEngine engine, ControllerContext controllerContext, string viewPath, string masterPath)
            : base(engine, controllerContext, viewPath, masterPath)
        {
        }

        protected override string PreprocessHook(string template)
        {
            using (var markdown = new Markdown(htmlRenderer, markDownExtensions, maxNesting: 16))
            {
                string renderedTemplate = markdownSections.Replace(template, match =>
                {
                    return markdown.Transform(match.Groups[1].Value);
                });

                return renderedTemplate;
            }
        }
    }
}