using System;
using System.Threading.Tasks;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;

namespace FRGenerics.Fringe {
  public class Fringe: BaseScript {
    protected Text InteriorIndicator = new Text("Interior: nope", new PointF(10f, 10f), .4f);
    protected Text RT = new Text("RAY hit: nope", new PointF(10f, 40f), .4f);

    public Fringe() {
      Tick += OnTick;
    }
    
    protected Camera ncam;

    public async Task OnTick() {
      Player me = LocalPlayer;
      Ped mePed = me.Character;

      int interiorId = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, mePed);
      bool validInterior = Function.Call<bool>(Hash.IS_VALID_INTERIOR, interiorId);

      if (validInterior) {
        LockRadarOnInterior(interiorId);
        InteriorIndicator.Caption = $"Interior: {interiorId}";
      } else {
        UnlockRadar();
        InteriorIndicator.Caption = "Interior: nope";
      }

      if (validInterior) {
        InsideInterior(interiorId);
      } else {
        OutsideInterior();
      }

      Function.Call(Hash._SET_RADAR_ZOOM_LEVEL_THIS_FRAME, 0);
      InteriorIndicator.Draw();
      RT.Draw();

      /// Fucking hell...
      if (Game.IsControlJustReleased(0, Control.PhoneUp)) {
        //var g6_p1 = new Vector3(193.9493f, -1004.425f, -99.99999f);
        //var g10_p1 = new Vector3(223.4f, -1001f, -100f);

        //Game.PlayerPed.Position = g10_p1;
        //playerCam = World.RenderingCamera;
        //Function.Call(Hash.LOAD_SCENE, -834.79f, 289.09f, 95.895f);

        ncam = World.CreateCamera(new Vector3(-834.79f, 289.09f, 95.895f), Vector3.Zero, 60f);
        ncam.PointAt(LocalPlayer.Character);

        World.RenderingCamera.InterpTo(ncam, 10000, true, true);
        //World.RenderingCamera = ncam;
      }

      if (Game.IsControlJustReleased(0, Control.PhoneDown)) {
        Debug.WriteLine("Whoa");
        //Game.PlayerPed.Position = new Vector3(0f, 0f, 70f);

        if (ncam != null) {
          ncam.Delete();
          ncam = null;
        }

        var wtf1 = new OutputArgument();
        var wtf2 = new OutputArgument();

        Function.Call(Hash.GET_SCREEN_RESOLUTION, wtf1, wtf2);

        Debug.WriteLine(wtf1.GetResult<int>() + ":" + wtf2.GetResult<int>());

        World.RenderingCamera = null;
      }

      Vector3 pt = Game.PlayerPed.GetOffsetPosition(new Vector3(0, 20f, 0f));
      
      Function.Call(
        Hash.DRAW_LINE,
        Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z,
        pt.X, pt.Y, pt.Z,
        255, 255, 0, 255
      );

      int rt = Function.Call<int>(
        (Hash) 0x377906D8A31E5586,
        Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z,
        pt.X, pt.Y, pt.Z,
        1 | 2 | 16,
        //1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 256,
        Game.PlayerPed,
        0
      );

      var hit = new OutputArgument();
      var endCoord = new OutputArgument();
      var surfaceNormal = new OutputArgument();
      var entityHit = new OutputArgument();

      Function.Call(Hash.GET_SHAPE_TEST_RESULT, rt, hit, endCoord, surfaceNormal, entityHit);

      bool hasHit = hit.GetResult<bool>();
      int entity = entityHit.GetResult<int>();
      Vector3 end = endCoord.GetResult<Vector3>();

      if (hasHit) {
        RT.Caption = "RAY hit: " +
          (hasHit ? "yes" : "no") + ", " +
          (hasHit ? (endCoord.GetResult<Vector3>().ToString()) : "") + ", " +
          (new Vehicle(entity)).DisplayName.ToString();

        Function.Call(
          Hash.DRAW_LINE,
          Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z,
          end.X, end.Y, end.Z,
          255, 0, 255, 255
        );
      } else {
        RT.Caption = "RAY hit: none";
      }

      //World.RenderingCamera.PointAt(LocalPlayer.Character);
      //Debug.WriteLine("Cam active: " + (World.RenderingCamera.IsActive ? "yes" : "no"));
      //Debug.WriteLine("Cam exist: " + (World.RenderingCamera.Exists() ? "yes" : "no"));

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
