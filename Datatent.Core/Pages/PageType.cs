using System;

namespace Datatent.Core.Pages
{
    internal enum PageType : byte
    {
        Header = 1,
        Data  = 2,
        Index = 3,
        Empty = 0,
        Null = 9
    }

    internal static class PageTypeHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static bool EnumMatchType(PageType type, Type classType)
        {
            if (type == PageType.Data && classType == typeof(DataPage))
                return true;

            return false;
        }
    }
}