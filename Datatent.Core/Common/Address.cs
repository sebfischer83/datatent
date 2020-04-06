using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Datatent.Core.Scheduler;
// ReSharper disable UnusedMember.Global

namespace Datatent.Core.Common
{
    /// <summary>
    /// Describes a position of data in the database.
    /// </summary>
    /// <remarks>
    /// Each address has a scope that shows which granularity the address had.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1066:Typ {0} muss IEquatable<T> implementieren, weil er Equals überschreibt", Justification = "<Ausstehend>")]
    public readonly struct Address 
    {
        /// <summary>
        /// The full address. Can only be compared if the scope is taken into account.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Sichtbare Instanzfelder nicht deklarieren", Justification = "Perfomance reason")]
        public readonly ulong FullAddress;

        /// <summary>
        /// Gets the document number part of the address.
        /// </summary>
        /// <value>
        /// The document number.
        /// </value>
        public ushort Document => (ushort) FullAddress;
        /// <summary>
        /// Gets the page number part of the address.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value> 
        public ushort Page => (ushort) ((uint) FullAddress >> 16);
        /// <summary>
        /// Gets the block number part of the address.
        /// </summary>
        /// <value>
        /// The block number.
        /// </value>
        public ushort Block => (ushort)(FullAddress >> 32);

        /// <summary>
        /// Gets the scope of the address.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public AddressScope Scope => (AddressScope) (FullAddress >> 48);

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> struct.
        /// </summary>
        /// <param name="fullAddress">The full address.</param>
        public Address(ulong fullAddress)
        {
            FullAddress = fullAddress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> struct.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fullAddress">The full address.</param>
        public Address(AddressScope scope, ulong fullAddress)
        {
            if (((AddressScope) (fullAddress >> 48)) != scope)
            {
                fullAddress ^= fullAddress >> 48 << 48;
                fullAddress ^= (ulong) scope << 48;
            }

            FullAddress = fullAddress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> struct.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        public Address(AddressScope scope, ushort block) : this(scope, block, 0)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> struct.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="page">The page.</param>
        public Address(AddressScope scope, ushort block, ushort page) : this(scope, block, page, 0)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> struct.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="page">The page.</param>
        /// <param name="document">The document.</param>
        public Address(AddressScope scope, ushort block,  ushort page, ushort document)
        {
            var a = (ulong) block << 32 | (uint) page << 16 | document;
            a |= ((ulong)scope << 48);

            FullAddress = a;
        }

        #endregion
        
        /// <summary>
        /// Converts to position in the file.
        /// Scope Document and Page are equal and only return the start of the Page.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Scope</exception>
        /// <exception cref="InvalidOperationException">Scope</exception>
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
                    length = Constants.PAGE_SIZE_INCL_HEADER;
                    offset += this.Block * Constants.BLOCK_SIZE_INCL_HEADER;
                    offset += this.Page * Constants.PAGE_SIZE_INCL_HEADER;
                    break;
                default:
                    throw new InvalidOperationException(nameof(Scope));
            }

            return (offset, length);
        }


        /// <summary>
        /// Performs an implicit conversion from <see cref="Address"/> to <see cref="System.UInt64"/>.
        /// </summary>
        /// <param name="a">a.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ulong(Address a) => a.FullAddress;

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.UInt64"/> to <see cref="Address"/>.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Address(ulong u) => new Address(u);

        /// <summary>
        /// From int64 to Address.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <returns></returns>
        public static ulong FromUInt64(ulong u)
        {
            return new Address(u);
        }

        /// <summary>
        /// Converts to uint64.
        /// </summary>
        /// <returns></returns>
        public ulong ToUInt64()
        {
            return this.FullAddress;
        }

        /// <summary>
        /// Print the address in a human readable form.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var bits = BitConverter.GetBytes(FullAddress).Select(b => Convert.ToInt16(b).ToString(CultureInfo.InvariantCulture)).Aggregate((s, s1) => s + s1);

            return $"{Enum.GetName(typeof(AddressScope), Scope)} {Convert.ToString((long) FullAddress, 2).PadLeft(64, '0')} {bits.PadLeft(8, '0')}";
        }

        /// <summary>
        /// Compare this address with the given one. Scope can be ignored.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="ignoreScope">if set to <c>true</c> [ignore scope].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid scope {this.Scope} {a.Scope}</exception>
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

        /// <summary>
        /// Compare 2 address. Scope can be ignored.
        /// </summary>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <param name="ignoreScope">if set to <c>true</c> [ignore scope].</param>
        /// <returns></returns>
        public static bool AreTheSame(Address a1, Address a2, bool ignoreScope = false)
        {
            return a1.AreTheSame(a2, ignoreScope);
        }


        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Address))
                return false;

            var addr = (Address) obj;
            return this == addr;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="address1">The address1.</param>
        /// <param name="address2">The address2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Address address1, Address address2)
        {
            return address1.AreTheSame(address2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="address1">The address1.</param>
        /// <param name="address2">The address2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Address address1, Address address2)
        {
            return !(address1 == address2);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return FullAddress.GetHashCode() ^ Scope.GetHashCode();
        }
    }
}