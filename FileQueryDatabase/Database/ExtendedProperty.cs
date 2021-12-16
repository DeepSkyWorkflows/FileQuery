// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Text;
using MetadataExtractor;

namespace FileQueryDatabase.Database
{
    /// <summary>
    /// Manual boxing implementation with metadata about a value.
    /// </summary>
    public class ExtendedProperty : IEquatable<ExtendedProperty>, IComparable<ExtendedProperty>
    {
        /// <summary>
        /// The value.
        /// </summary>
        private object val;

        /// <summary>
        /// Gets a null wrapper.
        /// </summary>
        public static ExtendedProperty NULL => new ()
        {
            Name = string.Empty,
            Value = null,
        };

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <remarks>
        /// Setting this value will also trigger setting up the string-related values for fast comparisons.
        /// </remarks>
        public object Value
        {
            get => val;
            set
            {
                val = value;
                if (val != null && val.GetType() == typeof(string))
                {
                    LowerInvariant = ((string)val).Trim().ToLowerInvariant();
                    ValueToString = (string)val;
                    return;
                }

                if (val != null && val is StringValue sv)
                {
                    ValueToString = (sv.Encoding ?? Encoding.UTF8).GetString(sv.Bytes);
                    LowerInvariant = ValueToString.ToLowerInvariant();
                }

                LowerInvariant = null;

                if (val is Type type)
                {
                    ValueToString = type.Name;
                }
                else
                {
                    ValueToString = value == null ? "NULL" : value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        public string ValueToString { get; private set; }

        /// <summary>
        /// Gets the lower invariant string representation.
        /// </summary>
        public string LowerInvariant { get; private set; }

        /// <summary>
        /// Equal operator implementation.
        /// </summary>
        /// <param name="left">Left property.</param>
        /// <param name="right">Right property.</param>
        /// <returns>A value indicating whether the two are equal.</returns>
        public static bool operator ==(ExtendedProperty left, ExtendedProperty right) =>
            !(left is null) && !(right is null) && left.Equals(right);

        /// <summary>
        /// Not equal operator implementation.
        /// </summary>
        /// <param name="left">Left property.</param>
        /// <param name="right">Right property.</param>
        /// <returns>A value indicating whether the two are not equal.</returns>
        public static bool operator !=(ExtendedProperty left, ExtendedProperty right) =>
            !(left == right);

        /// <summary>
        /// Less than operator implementation.
        /// </summary>
        /// <param name="left">Left property.</param>
        /// <param name="right">Right property.</param>
        /// <returns>A value indicating whether the left is less than the right.</returns>
        public static bool operator <(ExtendedProperty left, ExtendedProperty right) =>
            !(left is null) && !(right is null) && left.CompareTo(right) == -1;

        /// <summary>
        /// Less than or equal to operator implementation.
        /// </summary>
        /// <param name="left">Left property.</param>
        /// <param name="right">Right property.</param>
        /// <returns>A value indicating whether the left is less than or equal to the right.</returns>
        public static bool operator <=(ExtendedProperty left, ExtendedProperty right)
        {
            if (left is null || right is null)
            {
                return false;
            }

            var val = left.CompareTo(right);
            return val == -1 || val == 0;
        }

        /// <summary>
        /// Greater than operator implementation.
        /// </summary>
        /// <param name="left">Left property.</param>
        /// <param name="right">Right property.</param>
        /// <returns>A value indicating whether the left is greater than the right.</returns>
        public static bool operator >(ExtendedProperty left, ExtendedProperty right) =>
            !(left is null) && !(right is null) && left.CompareTo(right) > 0;

        /// <summary>
        /// Greater than or equal to operator implementation.
        /// </summary>
        /// <param name="left">Left property.</param>
        /// <param name="right">Right property.</param>
        /// <returns>A value indicating whether the left is greater than or equal to the right.</returns>
        public static bool operator >=(ExtendedProperty left, ExtendedProperty right) =>
                !(left is null) && !(right is null) && left.CompareTo(right) >= 0;

        /// <summary>
        /// Implement comparisons.
        /// </summary>
        /// <param name="other">The <see cref="ExtendedProperty"/> to compare to.</param>
        /// <returns>A value indicating the result of the comparison.</returns>
        public int CompareTo(ExtendedProperty other)
        {
            if (other == null || other.Value == null)
            {
                return -2;
            }

            if (LowerInvariant != null)
            {
                if (other.LowerInvariant == null)
                {
                    return -2;
                }

                return LowerInvariant.CompareTo(other.LowerInvariant);
            }

            if (Value is IComparable thisValue && other.Value is IComparable otherValue)
            {
                return thisValue.CompareTo(otherValue);
            }

            return ValueToString.CompareTo(other.ValueToString);
        }

        /// <summary>
        /// String contains.
        /// </summary>
        /// <param name="pat">The pattern to check for.</param>
        /// <returns>A value indicating whether it contains the pattern.</returns>
        public bool Contains(ExtendedProperty pat)
        {
            return StringOp(pat, contains: true);
        }

        /// <summary>
        /// String starts with.
        /// </summary>
        /// <param name="pat">The pattern to check for.</param>
        /// <returns>A value indicating whether it starts with the pattern.</returns>
        public bool StartsWith(ExtendedProperty pat)
        {
            return StringOp(pat, startsWith: true);
        }

        /// <summary>
        /// String ends with.
        /// </summary>
        /// <param name="pat">The pattern to check for.</param>
        /// <returns>A value indicating whether it ends with the pattern.</returns>
        public bool EndsWith(ExtendedProperty pat)
        {
            return StringOp(pat);
        }

        /// <summary>
        /// Equals implementtation.
        /// </summary>
        /// <param name="other">The other entity to compare.</param>
        /// <returns>A value indicating whether the two are equal.</returns>
        public bool Equals(ExtendedProperty other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Value as string.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return $"{Name}:{ValueToString}";
        }

        /// <summary>
        /// Hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Object equals implementation.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value indicating whether they are equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is ExtendedProperty ep)
            {
                return ((IEquatable<ExtendedProperty>)this).Equals(ep);
            }

            return false;
        }

        /// <summary>
        /// String conversion.
        /// </summary>
        /// <param name="tgt">Target as string.</param>
        /// <returns>The string value.</returns>
        private static string AsString(object tgt)
        {
            return tgt is string str ? str :
                (tgt is StringValue sv ? sv.Encoding.GetString(sv.Bytes) : null);
        }

        /// <summary>
        /// Implement string operations.
        /// </summary>
        /// <param name="pat">The pattern to check.</param>
        /// <param name="contains">A value indicating whether to perform a contains.</param>
        /// <param name="startsWith">A value indicating whether to perform a starts with.</param>
        /// <returns>A value indicating whether the pattern matched.</returns>
        private bool StringOp(ExtendedProperty pat, bool contains = false, bool startsWith = false)
        {
            if (Value == null || pat == null || pat.Value == null)
            {
                return false;
            }

            var str = AsString(Value);
            if (str != null)
            {
                var pattern = AsString(pat.Value);
                if (pattern != null)
                {
                    return contains ? str.Contains(pattern, StringComparison.InvariantCultureIgnoreCase) :
                        (startsWith ? str.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase) :
                        str.EndsWith(pattern, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            return false;
        }
    }
}
