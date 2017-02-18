using CitizenFX.Core;

namespace FRGenerics.Fringe {
  internal class PlayerInterior {
    public static bool IsIn(Player player, int interiorId) {
      if (EntityDecoration.ExistOn(player.Character, PlayerProperties.InteriorId)) {
        return EntityDecoration.Get<int>(player.Character, PlayerProperties.InteriorId) == interiorId;
      }

      return false;
    }

    public static int GetInteriorId(Player player) {
      return EntityDecoration.Get<int>(player.Character, PlayerProperties.InteriorId);
    }

    public static int GetInteriorOwner(Player player) {
      return EntityDecoration.Get<int>(player.Character, PlayerProperties.InteriorOwner);
    }

    public static bool IsOwner(Player player) {
      return GetInteriorOwner(player) == -1;
    }

    public static void SetInteriorId(Player player, int interiorId) {
      EntityDecoration.Set(player.Character, PlayerProperties.InteriorId, interiorId);
    }

    public static void SetInteriorOwner(Player player, Player owner) {
      EntityDecoration.Set(player.Character, PlayerProperties.InteriorOwner, owner.Handle);
    }

    public static void SetInterOwnerToMyself(Player player) {
      EntityDecoration.Set(player.Character, PlayerProperties.InteriorOwner, -1);
    }
  }
}
