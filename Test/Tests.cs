using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
using System.Drawing;
using NativeUI;

namespace FRGenerics.Test
{
    class Tests : BaseScript
    {
        protected Text numMods;
        protected MenuPool pool;
        protected UIMenu modTypesMenu;
        protected UIMenu modMenu;

        protected bool inst = false;
        protected bool inMenu = false;
        protected bool builtModTypes = false;

        public Tests()
        {
            pool = new MenuPool();
            modTypesMenu = new UIMenu("Mods", "Car mods");

            pool.Add(modTypesMenu);
            pool.RefreshIndex();

            Tick += OnTick;

            try
            {
                EventHandlers["tessst"] += new Action<string, string, CallbackDelegate>(TestHandler);
                Debug.WriteLine("Mounted event handler");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Uh-huh");
                Debug.WriteLine(e.Message);
            }
        }

        protected void TestHandler(string key, string value, CallbackDelegate callback)
        {
            Debug.WriteLine($"Huh: {key} => {value}");
            callback("YES");
        }

        private async Task OnTick()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;

                if (!inst)
                {
                    vehicle.Mods.InstallModKit();
                    inst = true;
                }

                if (inMenu && !builtModTypes)
                {
                    modTypesMenu.Clear();

                    foreach (var mod in vehicle.Mods.GetAllMods())
                    {
                        modTypesMenu.AddItem(new UIMenuItem(mod.LocalizedModTypeName));
                    }

                    pool.RefreshIndex();
                    builtModTypes = true;
                }

                if (Game.IsControlJustReleased(0, Control.PhoneLeft))
                {
                    //CallbackDelegate rf = Callbackk;

                    //var ugh = rf.Method.DeclaringType?.GetFields(BindingFlags.NonPublic |
                    //                                                    BindingFlags.Instance |
                    //                                                    BindingFlags.Public |
                    //                                                    BindingFlags.Static).FirstOrDefault(a => a.FieldType == typeof(RemoteFunctionReference));
                    //TriggerEvent("tessst", "lol", "no", rf);

                    //Func<string, int> cb = (val) => {
                    //    Debug.WriteLine($"AWww yiiis {val}");

                    //    return 0;
                    //};

                    TriggerEvent("tessst", "lol", "no", Wrap(Callbackk));
                    //modTypesMenu.Visible = !modTypesMenu.Visible;
                    //inMenu = !inMenu;
                }

                modTypesMenu.MouseControlsEnabled = false;
                pool.ControlDisablingEnabled = false;
                pool.MouseEdgeEnabled = false;
                pool.ProcessMenus();
            }
            else
            {
                if (modMenu != null)
                {
                    modMenu.Visible = false;
                }

                modTypesMenu.Visible = false;
                inMenu = false;
                builtModTypes = false;
            }
        }

        public static void Callbackk(string val)
        {
            Debug.WriteLine($"AWww yiiis {val}");
        }

        public static Func<string, int> Wrap(Action<string> method)
        {
            return (Func <string, int>) delegate (string var)
            {
                method(var);

                return 0;
            };
        }
    }
}
