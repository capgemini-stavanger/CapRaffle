using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapRaffle.Models;
using System.Text;

namespace CapRaffle.HtmlHelpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            if (pagingInfo.TotalPages < 6)
            {
                for (int i = 1; i <= pagingInfo.TotalPages; i++)
                {
                    TagBuilder tag = GenerateLink(i, pagingInfo);
                    result.Append(tag.ToString());
                }
                return MvcHtmlString.Create(result.ToString());
            }
            else
            {
                TagBuilder tag = GenerateLink(1, pagingInfo);
                result.Append(tag.ToString());

                //Display dots if there is a gap between page 1 and n-2.
                if (pagingInfo.CurrentPage - 4 > 0) result.Append("..");

                //Dispaly links to n-2 n-1 n n+1 n+2
                for (int i = pagingInfo.CurrentPage-2; i <= pagingInfo.CurrentPage+2; i++)
                {
                    if (i > 1 && i < (pagingInfo.TotalPages))
                    {
                        tag = GenerateLink(i, pagingInfo);
                        result.Append(tag.ToString());
                    }
                }
                //Display dots if there is a gap between last page and n+2
                if (pagingInfo.TotalPages - pagingInfo.CurrentPage > 3) result.Append("..");

                tag = GenerateLink(pagingInfo.TotalPages, pagingInfo);
                result.Append(tag.ToString());
                return MvcHtmlString.Create(result.ToString());
            }
        }
        
        private static TagBuilder GenerateLink(int page, PagingInfo pagingInfo) 
        {
            string archive = "";
            if (pagingInfo.Archive) archive = "&Archive=true";
            TagBuilder tag = new TagBuilder("a");
            tag.MergeAttribute("href", "?page="+page + archive);
            tag.InnerHtml = page+"";
            if (page == pagingInfo.CurrentPage) tag.AddCssClass("selected");
            return tag;
        }
    }
}