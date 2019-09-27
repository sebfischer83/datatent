using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.IO;

namespace Datatent.Core.Pages
{
    internal static class PageReader
    {
        public static T GetPage<T>(Memory<byte> arraySlice, DatatentSettings datatentSettings) where T: BasePage, new()
        {
            if (Constants.PAGE_SIZE + Constants.PAGE_HEADER_SIZE > arraySlice.Length)
                throw new ArgumentException($"The array is too small to contain a page");

            var pageType = (PageType) arraySlice.Span[BasePage.PAGE_TYPE];
            if (!PageTypeHelper.EnumMatchType(pageType, typeof(T)))
                throw new ArgumentException($"{pageType} is not the same as given as T");

            if (pageType == PageType.Data)
            {
                var page = (T) (BasePage) new DataPage();

                return page;
            }

            throw new ArgumentException(nameof(T));
        }
    }
}
