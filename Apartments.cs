using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
using System.Drawing;
using FRGenerics.Drawing;
//using NativeUI;

namespace FRGenerics
{
    public class Apartment
    {
        public int InteriorId;
        public string Name;
        public int Hash;

        public Vector3 MapPosition;
        public Vector3 EnterOutside;
        public Vector3 EntranceInside;
        public Vector3 ExitOutside;
    }

    public enum PlayerApartmentState
    {
        Outside,
        AtEnterOutside,
        AtEntranceInside,
        Inside,
        TransitionInside,
        TransitionOutside
    }

    public class Apartments : BaseScript
    {
        protected static Apartment[] entires = new Apartment[] {
      new Apartment {
        Name = "test",
        InteriorId = 123,
        Hash = 0,
        MapPosition = Vector3.One,
        EnterOutside = Vector3.One,
        EntranceInside = Vector3.One,
        ExitOutside = Vector3.One
      }
    };

        #region Fields
        protected Fringe fringe;
        protected Apartment CurrentApartment;

        protected static float EnterOutsideActivationRange = 4f * 4f;
        protected static float EntranceInsideActivationRange = 4f * 4f;

        protected PlayerApartmentState State = PlayerApartmentState.Outside;
        #endregion

        public Apartments()
        {
            fringe = new Fringe { Players = Players };

            Tick += OnTick;
            Tick += RenderOutsideMarkers;
        }

        /// <summary>
        /// Just do what state says...
        /// </summary>
        /// <returns></returns>
        protected async Task OnTick()
        {
            switch (State)
            {
                case PlayerApartmentState.Outside:
                    Outside();
                    break;

                case PlayerApartmentState.AtEnterOutside:
                    AtEnterOutside();
                    break;

                case PlayerApartmentState.TransitionInside:
                    await TransitionInside();
                    break;

                case PlayerApartmentState.AtEntranceInside:
                    AtEntranceInside();
                    break;

                case PlayerApartmentState.TransitionOutside:
                    await TransitionOutside();
                    break;

                case PlayerApartmentState.Inside:
                default:
                    Inside();
                    break;
            }
        }

        #region Outside
        /// <summary>
        /// Searching for apartments outdoors entrance
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Outside()
        {
            fringe.OutsideInterior();

            foreach (Apartment apt in entires)
            {
                if (PlayerIsInRangeSquared(apt.EnterOutside, EnterOutsideActivationRange))
                {
                    CurrentApartment = apt;
                    State = PlayerApartmentState.AtEnterOutside;
                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AtEnterOutside()
        {
            // If player stepped outside switch state to outside
            if (!PlayerIsInRangeSquared(CurrentApartment.EnterOutside, EnterOutsideActivationRange))
            {
                CurrentApartment = null;
                State = PlayerApartmentState.Outside;
                return;
            }

            BeginTransitionInside();

            // Uncomment when lazy tard will make a menu...
            //var playersInside = GetOwnersInsideInterior(CurrentApartment.InteriorId);

            //if (playersInside.Count == 0) {
            //  BeginTransitionInside();
            //} else {
            //  ShowOwnerSelectionMenu(playersInside);
            //}
        }

        protected void BeginTransitionInside(Player owner = null)
        {
            State = PlayerApartmentState.TransitionInside;

            Game.Player.CanControlCharacter = false;
            Game.PlayerPed.IsInvincible = true;
            Game.PlayerPed.Weapons.Select(PlayerGenerics.Unarmed);

            PlayerGenerics.SetFlag(Game.Player, PlayerFlag.Hidden, true);
            PlayerGenerics.SetFlag(Game.Player, PlayerFlag.BlipHidden, true);

            PlayerInterior.SetInteriorId(Game.Player, CurrentApartment.InteriorId);
            if (owner == null)
            {
                PlayerInterior.SetInterOwnerToMyself(Game.Player);
            }
            else
            {
                PlayerInterior.SetInteriorOwner(Game.Player, owner);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected async Task TransitionOutside()
        {
            Screen.Fading.FadeOut(200);
            while (!Screen.Fading.IsFadedOut)
            {
                await Delay(1);
            }

            await TeleportPlayerTo(CurrentApartment.ExitOutside, 1f);

            Game.Player.CanControlCharacter = true;
            Game.PlayerPed.IsInvincible = false;

            Screen.Fading.FadeIn(200);
            while (!Screen.Fading.IsFadedIn)
            {
                await Delay(1);
            }

            State = PlayerApartmentState.Outside;
        }
        #endregion

        #region Inside
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AtEntranceInside()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Inside()
        {
            fringe.InsideInterior(
              CurrentApartment.InteriorId,
              CurrentApartment.Hash,
              CurrentApartment.MapPosition
            );
        }

        protected void BeginTransitionOutside()
        {
            State = PlayerApartmentState.TransitionOutside;

            Game.Player.CanControlCharacter = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected async Task TransitionInside()
        {
            Screen.Fading.FadeOut(200);
            while (!Screen.Fading.IsFadedOut)
            {
                await Delay(1);
            }

            await TeleportPlayerTo(CurrentApartment.EntranceInside, 1f);
            await PlayerInterior.Load(CurrentApartment.InteriorId);

            Screen.Fading.FadeIn(200);
            while (!Screen.Fading.IsFadedIn)
            {
                await Delay(1);
            }

            Game.Player.CanControlCharacter = true;

            State = PlayerApartmentState.Inside;
        }
        #endregion

        /// <summary>
        /// Awaitable player teleportation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="heading"></param>
        /// <returns></returns>
        protected async Task TeleportPlayerTo(Vector3 position, float heading)
        {
            Function.Call(Hash.CLEAR_FOCUS);
            Function.Call(
              Hash.START_PLAYER_TELEPORT,
              Game.Player,
              position.X,
              position.Y,
              position.Z,
              heading,
              true,
              true,
              true
            );

            while (Function.Call<bool>(Hash.IS_PLAYER_TELEPORT_ACTIVE))
            {
                await Delay(1);
            }
        }

        /// <summary>
        /// Checks if player is in squared range
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        protected bool PlayerIsInRangeSquared(Vector3 pos, float range)
        {
            return Vector3.DistanceSquared(Game.PlayerPed.Position, pos) < range;
        }

        /// <summary>
        /// Returns list of players who are inside given interior and are owners of this interior
        /// </summary>
        /// <param name="interiorId"></param>
        /// <returns></returns>
        protected List<Player> GetOwnersInsideInterior(int interiorId)
        {
            var list = new List<Player>();

            foreach (Player player in Players)
            {
                if (PlayerInterior.IsIn(player, interiorId) && PlayerInterior.IsOwner(player))
                {
                    list.Add(player);
                }
            }

            return list;
        }

        #region Markers
        public static Vector3 markerPutDown = new Vector3(0f, 0f, 3f);
        public static Vector3 markerDir = new Vector3();
        public static Vector3 markerRot = new Vector3();
        public static Vector3 markerScale = new Vector3(4f);
        public static Color markerColour = KnownColorTable.GimmeTheColor(KnownColor.AliceBlue);

        /// <summary>
        /// Just renders markers /shrug
        /// </summary>
        /// <returns></returns>
        protected async Task RenderOutsideMarkers()
        {
            foreach (var apt in entires)
            {
                var pos = apt.EnterOutside;

                World.DrawMarker(
                  MarkerType.VerticalCylinder,
                  apt.EnterOutside - markerPutDown,
                  markerDir,
                  markerRot,
                  markerScale,
                  markerColour
                );
            }

            await Task.FromResult(0);
        }
        #endregion

        #region EnterOutsideMenu
        //protected MenuPool eomMenuPool = new MenuPool();
        //protected UIMenu eomMenu = new UIMenu("", "");

        //protected bool open = false;

        //protected void ShowOwnerSelectionMenu(List<Player> players) {
        //  var mine = new UIMenuItem("Enter my apartments");
        //  mine.Activated += MineActivated;

        //  foreach(Player player in players) {
        //    var playerClosure = player;
        //    var item = new UIMenuItem(player.Name);
        //    item.Activated += (UIMenu sender, UIMenuItem selectedItem) => {
        //      BeginTransitionInside(player);
        //    };
        //  }

        //  eomMenu.AddItem(mine);

        //  eomMenuPool.RefreshIndex();

        //  eomMenu.Visible = true;
        //  eomMenuPool.ProcessMenus();
        //}

        //private void MineActivated(UIMenu sender, UIMenuItem selectedItem) {
        //  BeginTransitionInside();
        //}
        #endregion
    }
}
