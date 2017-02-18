using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
using FRGenerics.PlayersBlips;

namespace FRGenerics.Fringe {
  public class Fringe: BaseScript {
    protected Text InteriorIndicator = new Text("Interior: nope", new PointF(10f, 10f), .7f);

    protected volatile bool InInteriorNow = false;

    public Fringe() {
      Tick += OnTick;
    }

    public async Task OnTick() {
      int interiorId = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.PlayerPed);

      bool validInterior = Function.Call<bool>(Hash.IS_VALID_INTERIOR, interiorId);

      if (validInterior) {
        InteriorIndicator.Caption = $"Interior: {interiorId}";
        Function.Call(Hash.SET_RADAR_AS_INTERIOR_THIS_FRAME, 0x5f2d1b2f, 100f, 100f, 0, 10);
        Function.Call(Hash._SET_PLAYER_BLIP_POSITION_THIS_FRAME, 100f, 100f);
        Function.Call(Hash.LOCK_MINIMAP_POSITION, 100f, 100f);
        //Function.Call(Hash._CENTER_PLAYER_ON_RADAR_THIS_FRAME);
      } else {
        Function.Call(Hash.UNLOCK_MINIMAP_POSITION);
        InteriorIndicator.Caption = "Interior: nope";
      }

      if (validInterior) {
        // Player came into interior
        //InInteriorNow = true;
        HidePlayersInThisInterior(interiorId);
      } else {
        ShowPlayersOutsideAnyInterior();
      }

      Function.Call(Hash._SET_RADAR_ZOOM_LEVEL_THIS_FRAME, 0);
      InteriorIndicator.Draw();


      /// Fucking hell...
      if (Game.IsControlJustReleased(0, Control.PhoneUp)) {
        var g6_p1 = new Vector3(193.9493f, -1004.425f, -99.99999f);

        var g10_p1 = new Vector3(223.4f, -1001f, -100f);

        Game.PlayerPed.Position = g10_p1;
      }

      if (Game.IsControlJustReleased(0, Control.PhoneDown)) {
        Game.PlayerPed.Position = new Vector3(0f, 0f, 70f);
      }

      await Task.FromResult(0);
    }

    protected void HidePlayersInThisInterior(int interior) {
      foreach (Player player in Players) {
        if (Game.Player == player) {
          TogglePlayerHeadDisplay(player, false);
          continue;
        }

        if (IsInInterior(interior, player.Character)) {
          Function.Call(Hash.SET_PLAYER_INVISIBLE_LOCALLY, player, true);
          Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, Game.PlayerPed, player.Character, true);
        }
      }
    }

    protected void ShowPlayersOutsideAnyInterior() {
      Player currentPlayer = Game.Player;
      Ped playerPed = Game.PlayerPed;

      foreach (Player player in Players) {
        if (currentPlayer == player) {
          TogglePlayerHeadDisplay(player, true);
          continue;
        }

        if (IsInAnyInterior(player.Character)) {
          Function.Call(Hash.SET_PLAYER_INVISIBLE_LOCALLY, player, false);
          Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, playerPed, player.Character, false);
        }
      }
    }

    protected bool IsInAnyInterior(Entity ent) {
      int interior = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, ent);

      return Function.Call<bool>(Hash.IS_VALID_INTERIOR, interior);
    }

    protected bool IsInInterior(int interior, Entity ent) {
      int actualInterior = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, ent);

      if (!Function.Call<bool>(Hash.IS_VALID_INTERIOR, actualInterior)) {
        return false;
      }

      return actualInterior == interior;
    }

    protected void TogglePlayerHeadDisplay(Player player, bool toggle) {
      int headDisplay = Function.Call<int>(Hash._CREATE_HEAD_DISPLAY, player.Character, player.Name, false, true, "", false);
      Function.Call((Hash) 0xEE76FF7E6A0166B0, headDisplay, true);

      if (Function.Call<bool>(Hash._HAS_HEAD_DISPLAY_LOADED, headDisplay)) {
        //for (int i = 1; i <= 15; i++) {
        //  Function.Call(Hash._SET_HEAD_DISPLAY_FLAG, headDisplay, i, false);
        //}
        
        //Function.Call(Hash._SET_HEAD_DISPLAY_FLAG, headDisplay, 16, false);
        //Function.Call((Hash) 0xD48FE545CD46F857, headDisplay, 16, 150);
        //Function.Call((Hash) 0xD48FE545CD46F857, headDisplay, 8, 150);
        //Function.Call((Hash) 0xA67F9C46D612B6F1, headDisplay, true);
      }
    }
  }
}
