using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FRGenerics
{
    public class Interior
    {
        /// <summary>
        /// Returns interior entity is in
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public static int GetFromEntity(Entity ent)
        {
            return Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, ent);
        }

        /// <summary>
        /// Returns interior at given coordinates
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static int GetFromCoords(Vector3 coords)
        {
            return Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, coords.X, coords.Y, coords.Z);
        }

        /// <summary>
        /// Returns true if entity is in any interior, false otherwise
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public static bool IsEntityInAny(Entity ent)
        {
            return IsValid(GetFromEntity(ent));
        }

        /// <summary>
        /// Waits for interior to load
        /// </summary>
        /// <param name="interiorId"></param>
        public static async void WaitForLoad(int interiorId)
        {
            if (!IsValid(interiorId))
            {
                throw new InvalidInteriorException();
            }

            while (!Function.Call<bool>(Hash.IS_INTERIOR_READY, interiorId))
            {
                await BaseScript.Delay(0);
            }
        }

        public static bool IsValid(int interiorId)
        {
            return Function.Call<bool>(Hash.IS_VALID_INTERIOR, interiorId);
        }
    }

    public class InvalidInteriorException : Exception { }
}
