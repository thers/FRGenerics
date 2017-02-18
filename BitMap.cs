namespace FRGenerics {
  public class BitMap {
    public int Value;

    public static implicit operator BitMap(int i) {
      return new BitMap(i);
    }

    public static implicit operator int(BitMap bm) {
      return bm.Value;
    }

    public BitMap(int value = 0) {
      Value = value;
    }

    public bool Has(int i) {
      return (Value & i) == i;
    }

    public bool HasAny(int i) {
      return (Value & i) != 0;
    }

    public void Set(int i) {
      Value |= i;
    }

    public void Unset(int i) {
      Value ^= i;
    }

    public bool this[int i] => Has(i);

    public override bool Equals(object o) {
      if (o is BitMap) {
        return (o as BitMap).Value == this.Value;
      }

      return false;
    }

    public override int GetHashCode() {
      return Value;
    }

    public static bool operator !=(BitMap bm, int i) {
      return !bm.Has(i);
    }

    public static bool operator ==(BitMap bm, int i) {
      return bm.Has(i);
    }

    public static BitMap operator +(BitMap left, int right) {
      return new BitMap(left.Value | right);
    }

    public static BitMap operator +(BitMap left, BitMap right) {
      return new BitMap(left.Value | right.Value);
    }

    public static BitMap operator -(BitMap left, int right) {
      return new BitMap(left.Value ^ right);
    }

    public static BitMap operator -(BitMap left, BitMap right) {
      return new BitMap(left.Value ^ right.Value);
    }
  }
}
