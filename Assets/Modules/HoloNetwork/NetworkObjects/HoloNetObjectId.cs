using System;

namespace HoloNetwork.NetworkObjects {

  [Serializable]
  public struct HoloNetObjectId {

    public uint authorId;
    public uint objectId;

    private static HoloNetObjectId _empty = new HoloNetObjectId(0, UInt32.MaxValue);
    public static HoloNetObjectId empty => _empty;

    public bool isEmpty => this == empty;

    public HoloNetObjectId(uint authorId, uint objectId) {
      this.authorId = authorId;
      this.objectId = objectId;
    }

    public bool Equals(HoloNetObjectId other) {
      return authorId == other.authorId && objectId == other.objectId;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is HoloNetObjectId && Equals((HoloNetObjectId) obj);
    }

    public static bool operator !=(HoloNetObjectId lhs, HoloNetObjectId rhs) {
      return !lhs.Equals(rhs);
    }

    public static bool operator ==(HoloNetObjectId lhs, HoloNetObjectId rhs) {
      return lhs.Equals(rhs);
    }

    public override int GetHashCode() {
      unchecked {
        return ((int) authorId * 397) ^ (int) objectId;
      }
    }

    public override string ToString() {
      return $"ObjectId({authorId},{objectId})";
    }

  }

}