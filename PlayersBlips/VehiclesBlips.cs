using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FRGenerics.PlayersBlips {
  public sealed class VehiclesBlips {
    public static Dictionary<VehicleHash, BlipSprite> PlanesOverrides = new Dictionary<VehicleHash, BlipSprite> {
      { VehicleHash.Besra, (BlipSprite) 424 },
      { VehicleHash.Hydra, (BlipSprite) 424 },
      { VehicleHash.Lazer, (BlipSprite) 424 }
    };

    /// <summary>
    /// Returns vehicle blip sprite
    /// </summary>
    /// <param name="vehicle"></param>
    /// <returns></returns>
    public static BlipSprite Get(Vehicle vehicle) {
      VehicleHash modelHash = (VehicleHash) vehicle.Model.Hash;

      switch ((VehicleClass) Function.Call<int>(Hash.GET_VEHICLE_CLASS, vehicle)) {
        case VehicleClass.Helicopters:
          return BlipSprite.HelicopterAnimated;

        case VehicleClass.Planes:
          return GetPlane(modelHash);

        case VehicleClass.Boats:
          return BlipSprite.Speedboat;
      }

      switch (modelHash) {
        case VehicleHash.Insurgent:
        case VehicleHash.Insurgent2:
        case VehicleHash.Limo2:
          return BlipSprite.GunCar;

        case VehicleHash.Rhino:
          return BlipSprite.Tank;
      }

      return BlipSprite.Standard;
    }

    public static BlipSprite GetPlane(VehicleHash hash) {
      switch (hash) {
        case VehicleHash.Blimp:
        case VehicleHash.Blimp2:
          return BlipSprite.Blimp;

        case VehicleHash.Hydra:
        case VehicleHash.Lazer:
        case VehicleHash.Besra:
          return (BlipSprite) 424;
      }

      return BlipSprite.Plane;
    }
  }
}
