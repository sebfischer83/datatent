using System;
using System.Collections.Generic;
using Datatent.Core.Document;
using Datatent.Core.Common;
using Datatent.Core.Scheduler;

namespace Datatent.DummyApplication
{
    class Program
    {
        static List<Address> _addresses1;
        static List<Address> _addresses2;

        static void Main(string[] args)
        {
            Prepare();
            ObjectLayoutInspector.TypeLayout.PrintLayout<Datatent.Core.Document.Document.DocumentHeader>();

            Run();
        }

        private static long Run()
        {
            long l = 0;
            for (int i = 0; i < 8192; i++)
            {
                Address address1 = _addresses1[i];
                for (int j = 0; j < 8192; j++)
                {
                    Address address2 = _addresses2[j];
                    if (Address.AreTheSame(address1, address2))
                        l++;
                }
            }

            return l;
        }

        private static void Prepare()
        {

            _addresses1 = new List<Address>(8192);
            _addresses2 = new List<Address>(8192);

            Random random = new Random();

            for (int i = 0; i < 8192; i++)
            {
                Address address1 = new Address(AddressScope.Page, (ushort)random.Next(1, ushort.MaxValue),
                    (ushort)random.Next(1, ushort.MaxValue), (ushort)random.Next(1, ushort.MaxValue));

                Address address2 = new Address(AddressScope.Page, (ushort)random.Next(1, ushort.MaxValue),
                    (ushort)random.Next(1, ushort.MaxValue), (ushort)random.Next(1, ushort.MaxValue));

                _addresses1.Add(address1);
                _addresses2.Add(address2);
            }
        }
    }
}