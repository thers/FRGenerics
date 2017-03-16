namespace FRGenerics
{
    public class BitSet
    {
        public int Value;

        public static implicit operator BitSet(int i)
        {
            return new BitSet(i);
        }

        public static implicit operator int(BitSet bm)
        {
            return bm.Value;
        }

        public BitSet(int value = 0)
        {
            Value = value;
        }

        public bool Has(int i)
        {
            return (Value & i) == i;
        }

        public bool HasAny(int i)
        {
            return (Value & i) != 0;
        }

        public void Set(int i)
        {
            Value |= i;
        }

        public void Unset(int i)
        {
            Value ^= i;
        }

        public bool this[int i] => Has(i);

        public override bool Equals(object o)
        {
            if (o is BitSet)
            {
                return (o as BitSet).Value == this.Value;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator !=(BitSet bm, int i)
        {
            return !bm.Has(i);
        }

        public static bool operator ==(BitSet bm, int i)
        {
            return bm.Has(i);
        }

        public static BitSet operator +(BitSet left, int right)
        {
            return new BitSet(left.Value | right);
        }

        public static BitSet operator +(BitSet left, BitSet right)
        {
            return new BitSet(left.Value | right.Value);
        }

        public static BitSet operator -(BitSet left, int right)
        {
            return new BitSet(left.Value ^ right);
        }

        public static BitSet operator -(BitSet left, BitSet right)
        {
            return new BitSet(left.Value ^ right.Value);
        }
    }
}
