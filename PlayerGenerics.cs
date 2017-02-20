using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FRGenerics {
  [Flags]
  public enum PlayerFlag: int {
    None = 0,
    Loaded = 1 << 0,
    Hidden = 1 << 1,
    Passive = 1 << 2,
    BlipHidden = 1 << 3,
    HeadDisplayHidden = 1 << 4,

    Unk2 = 1 << 5,
    Unk3 = 1 << 6,
    Unk4 = 1 << 7,
    Unk5 = 1 << 8,
    Unk6 = 1 << 9,
    Unk7 = 1 << 10,
    Unk8 = 1 << 11,
    Unk9 = 1 << 12,
    Unk10 = 1 << 13,
    Unk12 = 1 << 14,
    Unk13 = 1 << 15,
    Unk14 = 1 << 16,
    Unk15 = 1 << 17,
    Unk16 = 1 << 18,
    Unk17 = 1 << 19,
    Unk18 = 1 << 20,
    Unk20 = 1 << 21,
    Unk21 = 1 << 22,
    Unk22 = 1 << 23,
    Unk23 = 1 << 24,
    Unk24 = 1 << 25,
    Unk25 = 1 << 26,
    Unk26 = 1 << 27,
    Unk27 = 1 << 28,
    Unk28 = 1 << 29,
    Unk29 = 1 << 30,
    Unk30 = 1 << 31,
  }

  public sealed class PlayerProperties {
    public static string InteriorId = "_FRGInteriorId";
    public static string InteriorOwner = "_FRGInteriorOwner";
    public static string Flags = "_FRGFlags";
  }

  public class PlayerGenerics {
    public static readonly Control[] controlsToDisable = {
      Control.Aim,
      Control.Attack,
      Control.SelectWeapon,
      Control.Jump,
      Control.Cover,
      (Control) 53, // WeaponSpecial
      (Control) 54, // WeaponSpecial2
      Control.VehicleAim,
      Control.VehicleAttack,
      Control.VehicleAttack2,
      Control.VehicleHeadlight,
      Control.VehicleSelectNextWeapon,
      Control.VehicleSelectPrevWeapon,
      Control.VehicleJump,
      Control.VehicleFlyAttack,
      Control.VehicleFlyAttack2,
      Control.MeleeAttackLight,
      Control.MeleeAttackHeavy,
      Control.MeleeBlock,
      Control.SelectWeaponUnarmed,
      Control.SelectWeaponMelee,
      Control.SelectWeaponHandgun,
      Control.SelectWeaponShotgun,
      Control.SelectWeaponSmg,
      Control.SelectWeaponAutoRifle,
      Control.SelectWeaponSniper,
      Control.SelectWeaponHeavy,
      Control.SelectWeaponSpecial,
      Control.VehicleGunLeft,
      Control.VehicleGunRight,
      Control.VehicleGunUp,
      Control.VehicleGunDown
    };

    public const int DefaultFlags = 0;

    public static bool HasFlag(Player player, PlayerFlag flag) {
      return GetFlags(player).Has((int) flag);
    }

    public static void SetFlag(Player player, PlayerFlag flag, bool toggle) {
      BitMap flags = GetFlags(player);

      if (toggle) {
        flags.Set((int) flag);
      } else {
        flags.Unset((int) flag);
      }

      EntityDecoration.Set(player.Character, PlayerProperties.Flags, flags);
    }

    public static BitMap GetFlags(Player player) {
      Ped character = player.Character;

      if (EntityDecoration.ExistOn(character, PlayerProperties.Flags)) {
        return EntityDecoration.Get<int>(character, PlayerProperties.Flags);
      }

      return DefaultFlags;
    }

    public static void Hide(Player player) {
      Function.Call(Hash.SET_PLAYER_INVISIBLE_LOCALLY, player, true);
      Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, Game.PlayerPed, player.Character, true);
    }

    public static void Show(Player player) {
      Function.Call(Hash.SET_PLAYER_INVISIBLE_LOCALLY, player, true);
      Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, Game.PlayerPed, player.Character, true);
    }

    public static void DisableInteriorControlsThisFrame() {
      for (short i = 0; i < controlsToDisable.Length; i++) {
        Game.DisableControlThisFrame(0, controlsToDisable[i]);
      }
    }
  }

  public class PlayerInterior {
    public static bool IsIn(Player player, int interiorId) {
      if (EntityDecoration.ExistOn(player.Character, PlayerProperties.InteriorId)) {
        return EntityDecoration.Get<int>(player.Character, PlayerProperties.InteriorId) == interiorId;
      }

      return false;
    }

    public static bool IsInAny(Player player) {
      if (!EntityDecoration.ExistOn(player.Character, PlayerProperties.InteriorId)) {
        return false;
      }

      int interiorId = GetInteriorId(player);

      return IsValid(interiorId);
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

    public static bool IsValid(int interiorId) {
      return Function.Call<bool>(Hash.IS_VALID_INTERIOR, interiorId);
    }
  }
}
