using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Service;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;

namespace Datatent.Core.Pages
{
    internal class NullPage : BasePage
    {
        public NullPage() : base(new DummyDataProcessingPipeline())
        {
            PageHeader header = Header;
            header.PageType = PageType.Null;

            Header = header;
        }
    }
}
