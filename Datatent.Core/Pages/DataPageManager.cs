﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Datatent.Core.IO;
using Datatent.Core.Service;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using K4os.Compression.LZ4.Internal;

// ReSharper disable HollowTypeName

namespace Datatent.Core.Pages
{
    internal class DataPageManager
    {
        private readonly Memory<byte> _memoryBlockSlice;
        private readonly IDataProcessingPipeline _processingPipeline;

        private Memory<byte> _currentPointer;

        public DataPageManager(Memory<byte> memoryBlockSlice, IDataProcessingPipeline processingPipeline)
        {
            _memoryBlockSlice = memoryBlockSlice;
            _processingPipeline = processingPipeline;
            _currentPointer = memoryBlockSlice.Slice(0);
        }

        private static readonly byte[] PossiblePageTypesAsByte = (byte[]) Enum.GetValues(typeof(PageType));

        public BasePage GetNextPageOfType(PageType pageType)
        {
            var result = HasNextPage(_currentPointer);
            while (result.HasPage)
            {
                _currentPointer = _currentPointer.Slice((int) Constants.PAGE_SIZE_INCL_HEADER);

                if (result.PageType != pageType)
                {
                    result = HasNextPage(_currentPointer);
                }
                else
                {
                    return CheckPageAndInit(result.PageType, _currentPointer);
                }
            }

            return (BasePage) new NullPage();
        }

        private BasePage CheckPageAndInit(PageType pageType, Memory<byte> slice)
        {
            if (pageType == PageType.Data)
            {
                var page = (BasePage) new DataPage(_processingPipeline);
                ((BasePage) page).InitExisting(slice);
                return (page);
            }
            else
                throw new ArgumentException($"{nameof(pageType)} {pageType} is unknown");
        }

        public BasePage GetPageById(uint id)
        {
            if (id > Constants.PAGE_PER_BLOCK)
            {
                return (BasePage) new NullPage();
            }

            if (Constants.PAGE_SIZE_INCL_HEADER * id > _memoryBlockSlice.Length)
            {
                return (BasePage) new NullPage();
            }

            var slice = _memoryBlockSlice.Slice((int) (Constants.PAGE_SIZE_INCL_HEADER * id));
            var result = HasNextPage(slice);

            return CheckPageAndInit(result.PageType, slice);
        }

        private (bool HasPage, PageType PageType, uint Id) HasNextPage(Memory<byte> slice)
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

        public (bool Saved, uint Id) SaveContent(byte[] content)
        {
            // set pointer on start
            _currentPointer = _memoryBlockSlice.Slice(0);

            BasePage basePage = GetNextPageOfType(PageType.Data);
            while (basePage.Header.PageType == PageType.Data)
            {
                var dataPage = (DataPage) basePage;
            }

            return (false, 0);
        }
    }
}