using System;

namespace Sudoku.CSharp.CommonImpl
{
    internal struct NumbersBinary
    {
        public NumbersBinary(Int32 value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
        }

        public Int32 Value { get; }

        public Boolean Equals(NumbersBinary other)
        {
            return Value == other.Value;
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is NumbersBinary && Equals((NumbersBinary)obj);
        }

        public override Int32 GetHashCode()
        {
            return Value;
        }

        public static NumbersBinary operator &(NumbersBinary left, NumbersBinary right)
        {
            return new NumbersBinary(left.Value & right.Value);
        }

        public static NumbersBinary operator &(NumbersBinary left, Int32 right)
        {
            return new NumbersBinary(left.Value & right);
        }

        public static NumbersBinary operator &(Int32 left, NumbersBinary right)
        {
            return new NumbersBinary(left & right.Value);
        }

        public static NumbersBinary operator |(NumbersBinary left, NumbersBinary right)
        {
            return new NumbersBinary(left.Value | right.Value);
        }

        public static NumbersBinary operator |(NumbersBinary left, Int32 right)
        {
            return new NumbersBinary(left.Value | right);
        }

        public static NumbersBinary operator |(Int32 left, NumbersBinary right)
        {
            return new NumbersBinary(left | right.Value);
        }

        public static Boolean operator ==(NumbersBinary left, NumbersBinary right)
        {
            return left.Equals(right);
        }

        public static Boolean operator ==(NumbersBinary left, Int32 right)
        {
            return left.Value == right;
        }

        public static Boolean operator ==(Int32 left, NumbersBinary right)
        {
            return left == right.Value;
        }

        public static Boolean operator !=(NumbersBinary left, NumbersBinary right)
        {
            return !left.Equals(right);
        }

        public static Boolean operator !=(NumbersBinary left, Int32 right)
        {
            return left.Value != right;
        }

        public static Boolean operator !=(Int32 left, NumbersBinary right)
        {
            return left != right.Value;
        }
    }
}