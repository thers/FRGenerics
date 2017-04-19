using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.BaseScript;

namespace FRGenerics.NativeExtras
{
    public static class Streaming
    {
        /// <summary>
        /// Ensures texture loaded
        /// </summary>
        /// <param name="textureDictionary"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static async Task EnsureTXD(string textureDictionary)
        {
            int limit = 5000;

            Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, textureDictionary, 0);

            while (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, textureDictionary) && limit-- > 0)
            {
                await Delay(0);
            }
        }
    }
}
