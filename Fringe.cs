using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FRGenerics
{
    public class Fringe
    {
        public PlayerList Players;

        public void InsideInterior(int interior, int hash, Vector3 pos)
        {
            PlayerGenerics.DisableInteriorControlsThisFrame();
            
            Function.Call(Hash.SET_RADAR_AS_INTERIOR_THIS_FRAME, hash, pos.X, pos.Y, pos.Z, 0);
            Function.Call(Hash._SET_PLAYER_BLIP_POSITION_THIS_FRAME, pos.X, pos.Y);

            Player me = Game.Player;

            foreach (Player player in Players)
            {
                if (me == player)
                {
                    continue;
                }

                PlayerGenerics.SetFlag(player, PlayerFlag.HeadDisplayHidden | PlayerFlag.BlipHidden, true);

                // Player is in interior
                if (PlayerInterior.IsIn(player, interior))
                {
                    // Player is guest in current player's interior
                    if (PlayerInterior.GetInteriorOwner(player) == Game.Player.Handle)
                    {
                        PlayerGenerics.Show(player);
                    }
                    else
                    {
                        PlayerGenerics.Hide(player);
                    }
                }
            }
        }

        public void OutsideInterior()
        {
            Function.Call(Hash.SET_RADAR_AS_EXTERIOR_THIS_FRAME);
            Function.Call(Hash.UNLOCK_MINIMAP_POSITION);

            foreach (Player player in Players)
            {
                if (Game.Player == player)
                {
                    continue;
                }

                if (!PlayerInterior.IsInAny(player))
                {
                    PlayerGenerics.SetFlag(player, PlayerFlag.HeadDisplayHidden | PlayerFlag.BlipHidden, false);
                    PlayerGenerics.Show(player);
                }
            }
        }
    }
}
