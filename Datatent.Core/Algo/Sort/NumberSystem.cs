using System;
using System.Runtime.CompilerServices;
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnusedMember.Global

namespace Datatent.Core.Algo.Sort
{
    /// <summary>
    /// The number system that is used
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Bezeichner enthält Typnamen", Justification = "<Ausstehend>")]
    public enum NumberSystem
    {
        /// <summary>
        /// uint/ulong
        /// </summary>
        Unsigned,
        /// <summary>
        /// not common, but can be treated the same as twos-complement for the purposes of sorting
        /// </summary>
        OnesComplement,
        /// <summary>
        /// int/long
        /// </summary>
        TwosComplement,
        /// <summary>
        /// float/double
        /// </summary>
        SignBit
    }
    /// <summary>
    /// Number system
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class NumberSystem<T>
    {
        private static NumberSystem? _value;
        internal static NumberSystem Value => _value
            ?? throw new InvalidOperationException($"No number-system is defined for '{typeof(T).Name}'");


        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Statische Member nicht in generischen Typen deklarieren", Justification = "<Ausstehend>")]
        public static int Length => Unsafe.SizeOf<T>();


        /// <summary>
        /// Initializes the <see cref="NumberSystem{T}"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Statische Felder für Referenztyp inline initialisieren", Justification = "<Ausstehend>")]
        static NumberSystem()
        {
            if (typeof(T) == typeof(sbyte) ||
                typeof(T) == typeof(short) || 
                typeof(T) == typeof(int) ||
                typeof(T) == typeof(long)
                ) _value = NumberSystem.TwosComplement;
            else if (
                typeof(T) == typeof(bool) ||
                typeof(T) == typeof(byte) ||
                typeof(T) == typeof(ushort) ||
                typeof(T) == typeof(char) ||
                typeof(T) == typeof(uint) ||
                typeof(T) == typeof(ulong)
                ) _value = NumberSystem.Unsigned;
            else if (
                typeof(T) == typeof(float) ||
                typeof(T) == typeof(double)
                ) _value = NumberSystem.SignBit;   
        }


        /// <summary>
        /// Sets the specified number system.
        /// </summary>
        /// <param name="numberSystem">The number system.</param>
        /// <exception cref="ArgumentOutOfRangeException">numberSystem</exception>
        /// <exception cref="InvalidOperationException">The number-system for '{typeof(T).Name}' has already been set and cannot be changed</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Statische Member nicht in generischen Typen deklarieren", Justification = "<Ausstehend>")]
        public static void Set(NumberSystem numberSystem)
        {
            var existing = _value;
            if (existing == null)
            {
                switch(numberSystem)
                {
                    case NumberSystem.Unsigned:
                    case NumberSystem.OnesComplement:
                    case NumberSystem.TwosComplement:
                    case NumberSystem.SignBit:
                        _value = numberSystem;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(numberSystem));
                }                
            }
            else if (numberSystem != existing.Value)
            {
                throw new InvalidOperationException($"The number-system for '{typeof(T).Name}' has already been set and cannot be changed");
            }
        }
    }
}
