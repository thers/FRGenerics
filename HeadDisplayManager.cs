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
    NoneEmpty2,
    PassiveMode,
    WantedStar,
    SteeringWheel,
    Headset,
    HighlightPlayer,
    TextNoOutline,
    ArrowDown,
    BreifCase,
    LittleUser,
    RankNumber
  }

  public class HeadDisplayManager: BaseScript {
    public HeadDisplayManager() {
      Tick += OnTick;
    }

    protected async Task OnTick() {
      foreach (Player player in Players) {
        // No need to make head display for youself, huh
        if (player == LocalPlayer) {
          continue;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int GetHeadDisplay(Player player) {
      return Function.Call<int>(Hash._CREATE_HEAD_DISPLAY, player.Character, player.Name, false, false, "", false);
    }
  }
}
