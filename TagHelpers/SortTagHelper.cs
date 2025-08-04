using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ToDoList.Models;

namespace ToDoList.TagHelpers
{
    public class SortTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        public SortState ThisSortState { get; set; }
        public SortState SelectedSortState { get; set; }
        public string? Action { get; set; }
        public bool IsAsc { get; set; }
        public Dictionary<string, string> PageUrlValues { get; set; } = new();

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        public SortTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(Action);

            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "a";

            if (PageUrlValues.ContainsKey("sortState"))
                PageUrlValues["sortState"] = ThisSortState.ToString();
            else
                PageUrlValues.Add("sortState", ThisSortState.ToString());

            string? url = urlHelper.Action(Action, PageUrlValues);
            output.Attributes.SetAttribute("href", url);

            if (SelectedSortState == ThisSortState)
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("glyphicon");

                if (IsAsc == true)
                    tag.AddCssClass("glyphicon-chevron-up");
                else
                    tag.AddCssClass("glyphicon-chevron-down");

                output.PreContent.AppendHtml(tag);
            }
        }
    }
}
