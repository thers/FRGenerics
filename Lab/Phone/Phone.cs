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
using static FRGenerics.NativeExtras.Streaming;

namespace FRGenerics.Lab.Phone
{
    public enum ViewID
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

            if (open || opening || closing)
            {
                Screen.DisplayHelpTextThisFrame("~INPUT_CELLPHONE_CANCEL~ to close phone");

                var renderId = new OutputArgument();
                Function.Call(Hash.GET_MOBILE_PHONE_RENDER_ID, renderId);
                int renderTarget = renderId.GetResult<int>();
                Function.Call(Hash.SET_TEXT_RENDER_ID, renderTarget);

                Cellphone.CallFunction("SET_BACKGROUND_CREW_IMAGE", wallpaper);

                RenderPhoneUI();

                Cellphone.Render2DScreenSpace(new PointF(0f, 0f), new PointF(256f, 256f));

                Function.Call(Hash.SET_TEXT_RENDER_ID, 1);
            }
        }

        public void SetWallpaper(string wallpaper)
        {
            this.wallpaper = wallpaper;
            wallpaperLoaded = false;
        }

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
        }

        public void Close()
        {
            if (!open || opening || closing)
            {
                return;
            }

            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Put_Away", "Phone_SoundSet_Michael", 1);

            Ypos.Start(-20f, -60f, .1f, ScaleFuncs.Linear);
            closing = true;
        }

        private void RenderPhoneUI()
        {
            Cellphone.CallFunction("SET_HEADER", "Shitphone 2000");
            Cellphone.CallFunction(
                "SET_TITLEBAR_TIME",
                World.CurrentDayTime.Hours,
                World.CurrentDayTime.Minutes,
                World.CurrentDayTime.Days
            );
            Cellphone.CallFunction("SET_SIGNAL_STRENGTH", 1);
            Cellphone.CallFunction("SET_THEME", 5);

            // Contacts setup
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION, Cellphone.Handle, "SET_DATA_SLOT");
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int) ViewID.Contacts);

            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, 0); // Index

            // 0 - Missed call present
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, 0);

            // 1 - Name
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, "Chop!");
            Function.Call(Hash._END_TEXT_COMPONENT);

            // 2 - Keep empty
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, "CELL_999");
            Function.Call(Hash._END_TEXT_COMPONENT);

            // 3 - Icon
            Function.Call(Hash._BEGIN_TEXT_COMPONENT, "CELL_2000");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, "CHAR_CHOP");
            Function.Call(Hash._END_TEXT_COMPONENT);

            Function.Call(Hash._POP_SCALEFORM_MOVIE_FUNCTION_VOID);

            // Call screen setup
            //Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION, Cellphone.Handle, "SET_DATA_SLOT");
            //Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)ViewID.CallScreen);
            //Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, 0);
            //Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, 2);

            //Function.Call(Hash._BEGIN_TEXT_COMPONENT, "CELL_CONDFON");
            //Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, "ZED");
            //Function.Call(Hash._END_TEXT_COMPONENT);

            //// Pofile picture
            //Function.Call(Hash._BEGIN_TEXT_COMPONENT, "CELL_386");
            //Function.Call(Hash._END_TEXT_COMPONENT);

            //// Status text
            //Function.Call(Hash._BEGIN_TEXT_COMPONENT, "CELL_217");
            //Function.Call(Hash._END_TEXT_COMPONENT);

            //Function.Call(Hash._POP_SCALEFORM_MOVIE_FUNCTION_VOID);

            // Show view
            Cellphone.CallFunction("DISPLAY_VIEW", (int) ViewID.Contacts, 0);
        }
    }
}
