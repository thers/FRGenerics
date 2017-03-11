using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace FRGenerics.Test
{
    class Tests : BaseScript
    {
        protected Func<string, int> callback;

        public Tests()
        {
            callback = Wrap(Callbackk);

            Tick += OnTick;

            try
            {
                EventHandlers["frg:testEventForCallback"] += new Action<string, string, CallbackDelegate>(TestHandler);
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
            if (Game.IsControlJustReleased(0, Control.PhoneLeft))
            {
                Debug.WriteLine("Triggering...");
                TriggerEvent("frg:testEventForCallback", "lol", "no", callback);
            }
        }

        public static void Callbackk(string val)
        {
            Debug.WriteLine($"From callback {val}");
        }

        public static Func<string, int> Wrap(Action<string> method)
        {
            return (Func<string, int>) delegate (string var)
           {
               method(var);

               return 0;
           };
        }
    }
}
