using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Datatent.Core.Scheduler;

namespace Datatent.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Address
    {
        public readonly ulong FullAddress;
        public ushort Document => (ushort) FullAddress;
        public ushort Page => (ushort) ((uint) FullAddress >> 16);
        public ushort Block => (ushort)(FullAddress >> 32);

        public AddressScope Scope => (AddressScope) (FullAddress >> 48);
        
        #region ctor

        public Address(ulong fullAddress)
        {
            FullAddress = fullAddress;
        }

        public Address(AddressScope scope, ulong fullAddress)
        {
            if (((AddressScope) (fullAddress >> 48)) != scope)
            {
                fullAddress ^= (ulong) ((fullAddress >> 48)) << 48;
                fullAddress ^= (ulong) scope << 48;
            }

            FullAddress = fullAddress;
        }

        public Address(AddressScope scope, ushort block) : this(scope, block, 0)
        {

        }

        public Address(AddressScope scope, ushort block, ushort page) : this(scope, block, 0, 0)
        {

        }

        public Address(AddressScope scope, ushort block,  ushort page, ushort document)
        {
            ulong a = 0;
            a = (ulong) block << 32 | (uint) page << 16 | (ushort) document;
            a |= ((ulong)scope << 48);

            FullAddress = a;
        }

        #endregion

        public (uint offset, uint length) ToPosition()
        {
            uint offset = Constants.DATABASE_HEADER_SIZE;
            uint length;
            switch (Scope)
            {
                case AddressScope.Block:
                    length = Constants.BLOCK_SIZE_INCL_HEADER;
                    offset += this.Block * Constants.BLOCK_SIZE_INCL_HEADER;
                    break;
                case AddressScope.Page:
                    length = Constants.PAGE_SIZE_INCL_HEADER;
                    offset += this.Block * Constants.BLOCK_SIZE_INCL_HEADER;
                    offset += this.Page * Constants.PAGE_SIZE_INCL_HEADER;
                    break;
                case AddressScope.Document:
                    throw new NotSupportedException(nameof(Scope));
                default:
                    throw new InvalidOperationException(nameof(Scope));
            }

            return (offset, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static implicit operator ulong(Address a) => a.FullAddress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        public static implicit operator Address(ulong u) => new Address(u);

        public ulong FromUInt64(ulong u)
        {
            return new Address(u);
        }

        public ulong ToUInt64()
        {
            return this.FullAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var bits = BitConverter.GetBytes(FullAddress).Select(b => Convert.ToInt16((byte) b).ToString(CultureInfo.InvariantCulture)).Aggregate((s, s1) => s += s1);

            return $"{Convert.ToString((long) FullAddress, 2).PadLeft(64, '0')} {bits.PadLeft(8, '0')}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AreTheSame(Address a, bool ignoreScope = false)
        {
            if (!ignoreScope && this.Scope != a.Scope)
            {
                return false;
            }

            switch (Scope)
            {
                case AddressScope.Block:
                    return this.Block == a.Block;
                case AddressScope.Page:
                    return this.Block == a.Block && this.Page == a.Page;
                case AddressScope.Document:
                    return this.Block == a.Block && this.Page == a.Page && this.Document == a.Document;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid scope {this.Scope} {a.Scope}");
            }
        }

        public static bool AreTheSame(Address a1, Address a2, bool ignoreScope = false)
        {
            return a1.AreTheSame(a2, ignoreScope);
        }


        public override bool Equals(object obj)
        {
            if (!(obj is Address))
                return false;

            var addr = (Address) obj;
            return this == addr;
        }

        public static bool operator ==(Address address1, Address address2)
        {
            return address1.AreTheSame(address2);
        }

        public static bool operator !=(Address address1, Address address2)
        {
            return !(address1 == address2);
        }
    }
}