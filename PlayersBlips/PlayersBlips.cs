using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FRGenerics.PlayersBlips {  
  public class PlayersBlips : BaseScript {
    public PlayersBlips() {
      EventHandlers["onClientMapStart"] += new Action<dynamic>((dynamic res) => {
        foreach (var player in Players) {
          Debug.WriteLine("Player: " + player.Name);
        }
      });

      Tick += OnTick;
    }

    public async Task OnTick() {
      await Delay(10);

      foreach (var player in Players) {
        Ped ped = player.Character;

        if (player == Game.Player) {
          //continue;
        }

        // Initializing player blip
        Blip pedBlip = ped.AttachedBlip;
        if (pedBlip == null || pedBlip.Exists() == false) {
          pedBlip = ped.AttachBlip();

          pedBlip.Name = player.Name;
          pedBlip.Scale = .8f;
          pedBlip.IsFriendly = true;
        }
        
        UpdateBlip(player, ped, pedBlip);
      }
    }

    protected void UpdateBlip(Player player, Ped ped, Blip blip) {
      BlipSprite currentSprite = blip.Sprite;

      BlipSprite sprite = BlipSprite.Standard;

      bool showHeading = true;

      if (ped.IsGettingIntoAVehicle) {
        if (ped.LastVehicle != null) {
          if (ped.LastVehicle.AttachedBlip != null) {
            ped.LastVehicle.AttachedBlip.Delete();
          }
        }
      }

      if (ped.IsInVehicle()) {
        Vehicle vehicle = ped.CurrentVehicle;

        sprite = VehiclesBlips.Get(vehicle);
        showHeading = sprite == BlipSprite.Standard;
        //blip.Rotation = (int) vehicle.Heading;
      } else {
        sprite = BlipSprite.Standard;
        showHeading = true;
      }

      if (sprite != currentSprite) {
        blip.Sprite = sprite;
      }

      Function.Call(Hash._SET_BLIP_SHOW_HEADING_INDICATOR, blip, showHeading);
    }
  }
}
