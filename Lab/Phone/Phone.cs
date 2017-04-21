using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using TinyTween;
using NativeUI;
using static FRGenerics.NativeExtras.Streaming;

namespace FRGenerics.Lab.Phone
{
#region Enums
    public enum Theme
    {
        Silver,
        Blue,
        Green,
        Red,
        Orange,
        Grey,
        Violet
    }

    public enum View
    {
        Any = 0,
        HomeMenu = 1,
        Contacts = 2,
        CallScreen = 4,
        MessageList = 6,
        MessageView = 7,
        EmailList = 8,
        EmailView = 9,
        Settings = 13,
        ToDoList = 14,
        ToDoView = 15,
        MissionRepeat = 18,
        MissionStats = 19,
        JobList = 20,
        EmailResponse = 21,
        XYZData = 24,
        BossJobList = 25,
        BossJobView = 26,
        SecuroServHackingView = 27
    }

    public enum AppIcon
    {
        None = 0,
        CAMERA = 1,
        TEXT_MESSAGE = 2,
        CALENDAR = 3,
        EMAIL = 4,
        CALL = 5,
        EYEFIND = 6,
        MAP = 7,
        APPS = 8,
        MEDIA = 9,
        ATTACHMENT = 10,
        NEW_CONTACT = 11,
        SIDE_TASKS = 12,
        BAWSAQ = 13,
        MULTIPLAYER = 14,
        MUSIC = 15,
        GPS = 16,
        SPARE = 17,
        RINGTONE = 18,
        TEXT_TONE = 19,
        VIBRATE_ON = 20,
        VIBRATE_OFF = 21,
        VOLUME = 22,
        SETTINGS_1 = 23,
        SETTINGS_2 = 24,
        PROFILE = 25,
        SLEEP_MODE = 26,
        MISSED_CALL = 27,
        UNREAD_EMAIL = 28,
        READ_EMAIL = 29,
        REPLY_EMAIL = 30,
        REPLAYMISSION = 31,
        SHITSKIP = 32,
        UNREAD_SMS = 33,
        READ_SMS = 34,
        PLAYER_LIST = 35,
        COP_BACKUP = 36,
        GANG_TAXI = 37,
        REPEAT_PLAY = 38,
        CHECKLIST = 39,
        SNIPER = 40,
        ZIT_IT = 41,
        TRACKIFY = 42,
        SAVE = 43,
        ADD_TAG = 44,
        REMOVE_TAG = 45,
        LOCATION = 46,
        PARTY = 47,
        TICKED = 48,
        BROADCAST = 49,
        GAMEPAD = 50,
        SILENT = 51,
        INVITES_PENDING = 52,
        ON_CALL = 53,
        H_LOCK = 54,
        PUSH_TO_TALK = 55,
        BENNYS = 56,
        GANG = 57,
        TRACKER = 58,
        SIGHT_SEER = 59,
    }

    public enum InputDirection
    {
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }

    public enum SoftKey
    {
        Left = 1,
        Center = 2,
        Right = 3
    }

    public enum SoftKeyIcon
    {
        Blank = 1,
        Select = 2,
        Pages = 3,
        Back = 4,
        Call = 5,
        Hangup = 6,
        HangupHuman = 7,
        Week = 8,
        Keypad = 9,
        Open = 10,
        Reply = 11,
        Delete = 12,
        Yes = 13,
        No = 14,
        Sort = 15,
        Website = 16,
        Police = 17,
        Ambulance = 18,
        Fire = 19,
        Pages2 = 20
    }
    #endregion

    public class Phone
    {
        protected bool initialized = false;

        protected Scaleform Cellphone { get; set; }

        protected string wallpaper;
        protected bool wallpaperLoaded = false;

        protected bool open = false;
        protected bool opening = false;
        protected bool closing = false;

        protected Tween<float> Yrot = new FloatTween();
        protected Tween<float> Ypos = new FloatTween();

        protected float Yangle = 0f;

        public Phone() : this(Wallpaper.iFruitDefault) { }
        public Phone(string wallpaper)
        {
            this.wallpaper = wallpaper;
        }

        protected Text txt = new Text("", new PointF(10, 10), 1f);

        public async Task Update()
        {
            if (!initialized)
            {
                Cellphone = new Scaleform("cellphone_ifruit");

                initialized = true;
            }

            if (!wallpaperLoaded)
            {
                await EnsureTXD(wallpaper);
                wallpaperLoaded = true;
            }

            if (opening)
            {
                if (Ypos.State == TweenState.Stopped && Yrot.State == TweenState.Stopped)
                {
                    open = true;
                    opening = false;
                }
                else if (Ypos.State == TweenState.Running || Yrot.State == TweenState.Running)
                {
                    Ypos.Update(Game.LastFrameTime);
                    Yrot.Update(Game.LastFrameTime);

                    Function.Call(Hash.SET_MOBILE_PHONE_POSITION, 50.0f, Ypos.CurrentValue, -60.0f);
                    Function.Call(Hash.SET_MOBILE_PHONE_ROTATION, -90f, Yrot.CurrentValue, 0f, true);
                }
            }

            if (closing)
            {
                if (Ypos.State == TweenState.Stopped)
                {
                    closing = false;
                    open = false;
                    Function.Call(Hash.DESTROY_MOBILE_PHONE);
                }
                else if (Ypos.State == TweenState.Running)
                {
                    Ypos.Update(Game.LastFrameTime);

                    Function.Call(Hash.SET_MOBILE_PHONE_POSITION, 50.0f, Ypos.CurrentValue, -60.0f);
                }
            }

            if (open || opening)
            {
                Screen.DisplayHelpTextThisFrame("~INPUT_CELLPHONE_CANCEL~ to close phone");
            }

            if (open || opening || closing)
            {
                var renderId = new OutputArgument();
                Function.Call(Hash.GET_MOBILE_PHONE_RENDER_ID, renderId);
                int renderTarget = renderId.GetResult<int>();
                Function.Call(Hash.SET_TEXT_RENDER_ID, renderTarget);

                Cellphone.CallFunction("SET_BACKGROUND_CREW_IMAGE", wallpaper);

                RenderPhoneUI();

                Cellphone.Render2DScreenSpace(new PointF(0f, 0f), new PointF(256f, 256f));

                Function.Call(Hash.SET_TEXT_RENDER_ID, 1);
            }

            if (open)
            {
                await HandleInput();
                txt.Draw();
            }
        }

        /// <summary>
        /// Set phone wallpaper
        /// </summary>
        /// <param name="wallpaper"></param>
        public void SetWallpaper(string wallpaper)
        {
            this.wallpaper = wallpaper;
            wallpaperLoaded = false;
        }

        /// <summary>
        /// Open phone up
        /// </summary>
        public void Open()
        {
            if (open || opening || closing)
            {
                return;
            }

            Function.Call(Hash.CREATE_MOBILE_PHONE, 0);
            Function.Call(Hash.SET_MOBILE_PHONE_SCALE, 250f);

            Ypos.Start(-60f, -20f, .3f, ScaleFuncs.Linear);
            Yrot.Start(-90f, 0f, .3f, ScaleFuncs.Linear);
            opening = true;
            
            // Init
            SetHomeMenuApp(0, AppIcon.BENNYS, "Benny's shit", 10);
            SetHomeMenuApp(1, AppIcon.CAMERA, "Cameroon");
            SetHomeMenuApp(2, AppIcon.MULTIPLAYER, "Multiplier");
            SetHomeMenuApp(3, AppIcon.PLAYER_LIST, "BAWSAQ");
            SetHomeMenuApp(4, AppIcon.SETTINGS_2, "GPS");
            SetHomeMenuApp(5, AppIcon.NEW_CONTACT, "New contact");
            SetHomeMenuApp(6, AppIcon.EMAIL, "Music");
            SetHomeMenuApp(7, AppIcon.BROADCAST, "Party");
            SetHomeMenuApp(8, AppIcon.CHECKLIST, "Profile");

            // Show view
            Cellphone.CallFunction("DISPLAY_VIEW", (int)View.HomeMenu, 0);
        }

        /// <summary>
        /// Hide phone
        /// </summary>
        public void Close()
        {
            if (!open || opening || closing)
            {
                return;
            }

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Put_Away", "Phone_SoundSet_Michael", 1);

            Ypos.Start(-20f, -60f, .1f, ScaleFuncs.Linear);
            closing = true;
            open = false;
        }

        /// <summary>
        /// Set colour theme
        /// </summary>
        /// <param name="theme"></param>
        public void SetTheme(Theme theme)
        {
            Cellphone.CallFunction("SET_THEME", (int)theme);
        }

        /// <summary>
        /// Set header text
        /// </summary>
        /// <param name="text"></param>
        public void SetHeader(string text)
        {
            Cellphone.CallFunction("SET_HEADER", text);
        }

        /// <summary>
        /// Set raw contact entry
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="iconName"></param>
        /// <param name="hasMissedCall"></param>
        public void SetContactRaw(int index, string name, string iconName = "", bool hasMissedCall = false)
        {
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION, Cellphone.Handle, "SET_DATA_SLOT");
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)View.Contacts);

            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, 0); // Index

            // 0 - Missed call present
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, hasMissedCall);

            // 1 - Name
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, name);
            Function.Call(Hash._END_TEXT_COMPONENT);

            // 2 - Keep empty
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, "CELL_999");
            Function.Call(Hash._END_TEXT_COMPONENT);

            // 3 - Icon
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, "CELL_2000");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, iconName);
            Function.Call(Hash._END_TEXT_COMPONENT);

            Function.Call(Hash._POP_SCALEFORM_MOVIE_FUNCTION_VOID);
        }

        /// <summary>
        /// Setup call screen
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="callerIconNameGXT"></param>
        /// <param name="statusGXT"></param>
        public void SetupCallScreen(string caller, string callerIconNameGXT, string statusGXT)
        {
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION, Cellphone.Handle, "SET_DATA_SLOT");
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)View.CallScreen);

            // ???
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, 0);
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, 2);

            Function.Call(Hash._BEGIN_TEXT_COMPONENT, "CELL_CONDFON");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, caller);
            Function.Call(Hash._END_TEXT_COMPONENT);

            // Pofile picture
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, callerIconNameGXT);
            Function.Call(Hash._END_TEXT_COMPONENT);

            // Status text
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, statusGXT);
            Function.Call(Hash._END_TEXT_COMPONENT);

            Function.Call(Hash._POP_SCALEFORM_MOVIE_FUNCTION_VOID);
        }

        private async Task HandleInput()
        {
            if (Game.IsControlJustReleased(1, Control.PhoneUp))
            {
                Cellphone.CallFunction("SET_INPUT_EVENT", (int)InputDirection.Up);
            }

            if (Game.IsControlJustReleased(1, Control.PhoneDown))
            {
                Cellphone.CallFunction("SET_INPUT_EVENT", (int)InputDirection.Down);
            }

            if (Game.IsControlJustReleased(1, Control.PhoneLeft))
            {
                Cellphone.CallFunction("SET_INPUT_EVENT", (int)InputDirection.Left);
            }

            if (Game.IsControlJustReleased(1, Control.PhoneRight))
            {
                Cellphone.CallFunction("SET_INPUT_EVENT", (int)InputDirection.Right);
            }
        }

        private void RenderPhoneUI()
        {
            Cellphone.CallFunction(
                "SET_TITLEBAR_TIME",
                World.CurrentDayTime.Hours,
                World.CurrentDayTime.Minutes,
                World.CurrentDayTime.Days
            );
            Cellphone.CallFunction("SET_SIGNAL_STRENGTH", 1);

            SetSoftKey(SoftKey.Center, SoftKeyIcon.Fire, UnknownColors.OrangeRed);
        }

        private void SetHomeMenuApp(int index, AppIcon icon, string name = "", int notifications = 0, float opacity = 1f)
        {
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION, Cellphone.Handle, "SET_DATA_SLOT");
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)View.HomeMenu);

            // Index
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, index);

            // 0 - Icon
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)icon);

            // 1 - Notifications count
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, notifications);

            // 2 - Name
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, name);
            Function.Call(Hash._END_TEXT_COMPONENT);

            // 3 - Opacity
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)(opacity * 100f));

            Function.Call(Hash._POP_SCALEFORM_MOVIE_FUNCTION_VOID);
        }

        public void SetSoftKey(SoftKey index, SoftKeyIcon icon, Color color)
        {
            Cellphone.CallFunction(
                "SET_SOFT_KEYS",
                (int)index,
                true,
                (int)icon
            );

            Cellphone.CallFunction(
                "SET_SOFT_KEYS_COLOUR",
                (int)index,
                (int)color.R,
                (int)color.G,
                (int)color.B
            );
        }
    }
}
