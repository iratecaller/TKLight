using System;
using System.Collections.Generic;

namespace TKLight
{
    public class PositiveFraction : IComparable, IComparable<PositiveFraction>, IEqualityComparer<PositiveFraction>
    {
        public PositiveFraction()
        {
            _numerator = 0;
            _denominator = 1;
        }

        public static PositiveFraction operator *(PositiveFraction a, PositiveFraction b)
        {
            return new PositiveFraction(a._numerator * b._numerator, a._denominator * b._denominator);
        }

        public static PositiveFraction operator +(PositiveFraction a, PositiveFraction b)
        {
            return new PositiveFraction(a._numerator * b._denominator + b._numerator * a._denominator, a._denominator * b._denominator);
        }

        public PositiveFraction(uint a, uint b)
        {
            if (b == 0)
                throw new ArithmeticException("Denominator cannot be zero.");
            _numerator = a;
            _denominator = b;
        }

        private static uint gcf(uint a, uint b)
        {
            while (b != 0)
            {
                uint temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private static uint lcm(uint a, uint b)
        {
            return (a / gcf(a, b)) * b;
        }

        public static uint LCMNumerator(params PositiveFraction[] f)
        {
            uint ret = 0;

            ret = lcm(f[0]._numerator, f[1]._numerator);

            for (int i = 2; i < f.Length; i++)
            {
                lcm(ret, f[i]._numerator);
            }
            return ret;
        }

        private uint _numerator;
        private uint _denominator;

        public void Simplify()
        {
            uint gcd = _numerator;
            uint b = _denominator;
            while (b > 0)
            {
                uint rem = gcd % b;
                gcd = b;
                b = rem;
            }

            _numerator /= gcd;
            _denominator /= gcd;
        }

        public int Numerator
        {
            get
            {
                return (int)_numerator;
            }
        }

        public int Denominator
        {
            get
            {
                return (int)_denominator;
            }
        }

        public void Set(int numerator, int denominator)
        {
            if (numerator < 0 || denominator < 0) throw new ArgumentException("Must be positive");
            if (denominator == 0) throw new ArithmeticException("Denominator can't be zero");
            _numerator = (uint)numerator;
            _denominator = (uint)denominator;
        }

        public override string ToString()
        {
            return _numerator + "/" + _denominator;
        }

        public string AsString()
        {
            return ToString();
        }

        public static PositiveFraction Parse(string s)
        {
            PositiveFraction f = null;
            var parts = s.Split('/');
            if (parts.Length == 2)
            {
                uint a, b;

                if (uint.TryParse(parts[0], out a) && uint.TryParse(parts[1], out b))
                {
                    f = new PositiveFraction(a, b);
                }
            }

            if (f == null) throw new ArgumentException("Can't parse \"" + s + "\" as fraction");

            return f;
        }

        public static bool TryParse(string s, out PositiveFraction f)
        {
            bool ret = false;

            f = null;

            var parts = s.Split('/');
            if (parts.Length == 2)
            {
                uint a, b;

                if (uint.TryParse(parts[0], out a) && uint.TryParse(parts[1], out b))
                {
                    f = new PositiveFraction(a, b);
                    ret = true;
                }
            }

            return ret;
        }

        public int CompareTo(PositiveFraction other)
        {
            var f = other;
            if (Numerator == f.Numerator && Denominator == f.Denominator)
                return 0;
            return -1;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(PositiveFraction))
                return CompareTo((PositiveFraction)obj);

            return -1;
        }

        public override bool Equals(object obj)
        {
            if (obj is PositiveFraction)
            {
                var f = (PositiveFraction)obj;
                if (f.CompareTo(this) == 0)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Numerator, Denominator).GetHashCode();
        }

        public bool Equals(PositiveFraction x, PositiveFraction y)
        {
            if (x.CompareTo(y) == 0)
                return true;
            return false;
        }

        public int GetHashCode(PositiveFraction obj)
        {
            return obj.GetHashCode();
        }
    }
}