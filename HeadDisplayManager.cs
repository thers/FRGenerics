using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FRGenerics {
  public enum HeadDisplayFlag {
    TextWithOutline,
    NoneEmpty,
    HealthBar,
    AudioSpeaker,
    FlagOrPaused,
    Flag,
    PassiveMode,
    WantedStar,
    SteeringWheel,
    Headset,
    HighlightPlayer,
    TextNoOutline,
    ArrowDown,
    BreifCase,
    LittleUser,
    RankNumber,
    VoiceIndicator
  }

  public class HeadDisplayManager: BaseScript {
    public HeadDisplayManager() {
      Tick += OnTick;
    }

    protected async Task OnTick() {
      foreach (Player player in Players) {
        // No need to make head display for youself, huh
        if (player == LocalPlayer) {
          //continue;
        }
        
        if (PlayerGenerics.HasFlag(player, PlayerFlag.HeadDisplayHidden)) {
          Hide(player);
        } else {
          Show(player);
        }
      }

      await Delay(10);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void Hide(Player player) {
      int headDisplayId = GetHeadDisplay(player);

      foreach (HeadDisplayFlag flag in Enum.GetValues(typeof(HeadDisplayFlag))) {
        Function.Call(Hash._SET_HEAD_DISPLAY_FLAG, headDisplayId, flag, false);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void Show(Player player) {
      int headDisplayId = GetHeadDisplay(player);
      int level = player.WantedLevel;

      // Show the name of player
      Function.Call(Hash._SET_HEAD_DISPLAY_FLAG, headDisplayId, HeadDisplayFlag.TextWithOutline, true);

      // Show/hide wanted level
      if (level > 0) {
        Function.Call(Hash._SET_HEAD_DISPLAY_FLAG, headDisplayId, HeadDisplayFlag.WantedStar, true);
        Function.Call(Hash._SET_HEAD_DISPLAY_WANTED, headDisplayId, level);
      } else {
        Function.Call(Hash._SET_HEAD_DISPLAY_FLAG, headDisplayId, HeadDisplayFlag.WantedStar, false);
      }
    }

    protected int clr = 200;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int GetHeadDisplay(Player player) {
      int id = Function.Call<int>(
        Hash._CREATE_HEAD_DISPLAY,
        player.Character,
        player.Name,
        false,
        false,
        "ONEE",
        false
      );

      //Function.Call((Hash) 0x31698AA80E0223F8, id);

      //Function.Call(
      //  (Hash) 0x6DD05E9D83EFA4C9,
      //  id,
      //  player.Name,
      //  false,
      //  false,
      //  "ONEE",
      //  0,
      //  255,
      //  255,
      //  255
      //);

      Function.Call(Hash._SET_HEAD_DISPLAY_FLAG, id, HeadDisplayFlag.SteeringWheel, true);
      Function.Call((Hash) 0x613ED644950626AE, id, HeadDisplayFlag.SteeringWheel, 24);
      Function.Call((Hash) 0x613ED644950626AE, id, 0, 24);


      //Function.Call((Hash) 0x7B7723747CCB55B6, id, "huh/123");

      //for (int i = 0; i <= 16; i++) {
      //  if (i != 1 && i != 0) {
      //    if (i == 2) {
      //      Function.Call((Hash) 0x3158C77A7E888AB4, id, 28);
      //    } else {
      //      Function.Call(Hash._SET_HEAD_DISPLAY_FLAG, id, i, true);
      //    }

      //    if (i != 2 && i != 0) {
      //      Function.Call((Hash) 0xD48FE545CD46F857, id, i, 150);
      //    }
      //  }
      //}

      return id;
    }
  }
}
