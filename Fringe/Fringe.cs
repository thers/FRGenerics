using System;
using System.Threading.Tasks;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;

namespace FRGenerics.Fringe {
  public class Fringe: BaseScript {
    public Fringe() {
      Tick += OnTick;
    }

    public async Task OnTick() {
      Player me = LocalPlayer;
      Ped mePed = me.Character;

      int interiorId = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, mePed);
      bool validInterior = Function.Call<bool>(Hash.IS_VALID_INTERIOR, interiorId);

      if (validInterior) {
        LockRadarOnInterior(interiorId);
      } else {
        UnlockRadar();
      }

      if (validInterior) {
        InsideInterior(interiorId);
      } else {
        OutsideInterior();
      }

      await Task.FromResult(0);
    }

    protected void UnlockRadar() {
      Function.Call(Hash.UNLOCK_MINIMAP_POSITION);
    }

    protected void LockRadarOnInterior(int interiorId) {
      Function.Call(Hash.SET_RADAR_AS_INTERIOR_THIS_FRAME, interiorId, 100f, 100f, 0, 10);
      Function.Call(Hash._SET_PLAYER_BLIP_POSITION_THIS_FRAME, 100f, 100f);
      Function.Call(Hash.LOCK_MINIMAP_POSITION, 100f, 100f);
    }

    protected void InsideInterior(int interior) {
      PlayerGenerics.DisableInteriorControlsThisFrame();

      Player me = LocalPlayer;

      foreach (Player player in Players) {
        if (me == player) {
          continue;
        }

        PlayerGenerics.SetFlag(player, PlayerFlag.HeadDisplayHidden | PlayerFlag.BlipHidden, true);

        // Player is in interior
        if (PlayerInterior.IsIn(player, interior)) {
          // Player is guest in current player's interior
          if (PlayerInterior.GetInteriorOwner(player) == LocalPlayer.Handle) {
            PlayerGenerics.Show(player);
          } else {
            PlayerGenerics.Hide(player);
          }
        }
      }
    }

    protected void OutsideInterior() {
      Player me = LocalPlayer;

      foreach (Player player in Players) {
        if (me == player) {
          continue;
        }

        if (!PlayerInterior.IsInAny(player)) {
          PlayerGenerics.SetFlag(player, PlayerFlag.HeadDisplayHidden | PlayerFlag.BlipHidden, false);
          PlayerGenerics.Show(player);
        }
      }
    }
  }
}
