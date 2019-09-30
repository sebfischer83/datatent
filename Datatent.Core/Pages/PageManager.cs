using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Datatent.Core.IO;
using K4os.Compression.LZ4.Internal;

// ReSharper disable HollowTypeName

namespace Datatent.Core.Pages
{
    internal class PageManager
    {
        private readonly Memory<byte> _memory;
        private Memory<byte> _currentPointer;

        public PageManager(Memory<byte> memory)
        {
            _memory = memory;
            _currentPointer = memory.Slice(0);
        }

        private static readonly byte[] PossiblePageTypesAsByte = (byte[]) Enum.GetValues(typeof(PageType));
        
        public T GetNextPageOfType<T>() where T : BasePage
        {
            var result = HasNextPage(_currentPointer);
            while (result.HasPage)
            {
                _currentPointer = _currentPointer.Slice((int)Constants.PAGE_SIZE_INCL_HEADER);

                if (!PageTypeHelper.EnumMatchType(result.PageType, typeof(T)))
                {
                    result = HasNextPage(_currentPointer);
                }
                else
                {
                    return CheckPageAndInit<T>(result.PageType, _currentPointer);
                }
            }
            
            return (T)(BasePage)new NullPage();
        }

        private T CheckPageAndInit<T>(PageType pageType, Memory<byte> slice) where T : BasePage
        {
            if (pageType == PageType.Data)
            {
                var page = (BasePage)new DataPage();
                ((BasePage)page).InitExisting(_currentPointer);
                return (T)(page);
            }
            else
                throw new ArgumentException(nameof(T));
        }

        public T GetPageById<T>(uint id) where T : BasePage
        {
            var slice = _memory.Slice((int) (Constants.PAGE_HEADER_SIZE + Constants.PAGE_SIZE * id));
            var result = HasNextPage(slice);
            if (!PageTypeHelper.EnumMatchType(result.PageType, typeof(T)))
            {
                return (T)(BasePage) new NullPage();
            }

            return CheckPageAndInit<T>(result.PageType, slice);
        }

        public (bool HasPage, PageType PageType, uint Id) HasNextPage(Memory<byte> slice)
        {
            if (Constants.PAGE_SIZE_INCL_HEADER > slice.Length)
                return (false, PageType.Empty, 0);
            var pageTypeByte = slice.Span[BasePage.PAGE_TYPE];
            if (pageTypeByte == 0x00)
                return (false, PageType.Empty, 0);
            if (!PossiblePageTypesAsByte.Contains(pageTypeByte))
                return (false, PageType.Empty, 0);
            var id = 0;
            return ((bool HasPage, PageType PageType, uint Id)) (true, pageTypeByte, id);
        }

    }
}
