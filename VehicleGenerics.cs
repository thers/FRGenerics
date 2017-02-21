using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FRGenerics {
  internal class VehicleGenerics {
    public VehicleGenerics() {

    }
  }
}

//void sub_b0fc(auto a_0, auto a_1, auto a_2, auto a_3) {
//  if (VEHICLE::IS_VEHICLE_DRIVEABLE(a_0, 0)) {
//    if (GAMEPLAY::GET_HASH_KEY(&a_1._f1) != 0) {
//      VEHICLE::SET_VEHICLE_NUMBER_PLATE_TEXT(a_0, &a_1._f1);
//    }
//    if ((a_1 >= 0) && (a_1 < VEHICLE::GET_NUMBER_OF_VEHICLE_NUMBER_PLATES())) {
//      VEHICLE::SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX(a_0, a_1);
//    }
//    if (a_1._f42 == ${ sovereign}) {
//      a_1._f5 = 111;
//      a_1._f6 = 111;
//      a_1._f7 = 111;
//      a_1._f8 = 156;
//    }
//    if (GAMEPLAY::IS_BIT_SET(a_1._f4D, 13)) {
//      VEHICLE::SET_VEHICLE_CUSTOM_PRIMARY_COLOUR(a_0, a_1._f47, a_1._f48, a_1._f49);
//    } else {
//      VEHICLE::CLEAR_VEHICLE_CUSTOM_PRIMARY_COLOUR(a_0);
//    }
//    if (GAMEPLAY::IS_BIT_SET(a_1._f4D, 12)) {
//      VEHICLE::SET_VEHICLE_CUSTOM_SECONDARY_COLOUR(a_0, a_1._f47, a_1._f48, a_1._f49);
//    } else {
//      VEHICLE::CLEAR_VEHICLE_CUSTOM_SECONDARY_COLOUR(a_0);
//    }
//    VEHICLE::SET_VEHICLE_COLOURS(a_0, a_1._f5, a_1._f6);
//    if (a_1._f7 < 0) {
//      a_1._f7 = 0;
//    }
//    if (a_1._f8 < 0) {
//      a_1._f8 = 0;
//    }
//    VEHICLE::SET_VEHICLE_EXTRA_COLOURS(a_0, a_1._f7, a_1._f8);
//    if (((GAMEPLAY::IS_BIT_SET(a_1._f4D, 15) || sub_b8d0(a_0)) || ((((a_1._f3E == 0) && (a_1._f3F == 0)) && (a_1._f40 == 0)) && (a_1._f9[20/*1*/] > 0))) && sub_b8bf()) {
//      a_1._f3E = 0;
//      a_1._f3F = 0;
//      a_1._f40 = 0;
//    } else if (((a_1._f3E == 0) && (a_1._f3F == 0)) && (a_1._f40 == 0)) {
//      a_1._f3E = 255;
//      a_1._f3F = 255;
//      a_1._f40 = 255;
//    }
//    VEHICLE::SET_VEHICLE_TYRE_SMOKE_COLOR(a_0, a_1._f3E, a_1._f3F, a_1._f40);
//    if ((a_1._f41 == -1) && (a_1._f42 != ${ granger})) {
//      VEHICLE::SET_VEHICLE_WINDOW_TINT(a_0, 0);
//    } else {
//      VEHICLE::SET_VEHICLE_WINDOW_TINT(a_0, 0);
//      VEHICLE::SET_VEHICLE_WINDOW_TINT(a_0, a_1._f41);
//    }
//    VEHICLE::SET_VEHICLE_TYRES_CAN_BURST(a_0, !GAMEPLAY::IS_BIT_SET(a_1._f4D, 9));
//    if (a_2) {
//      VEHICLE::SET_VEHICLE_DOORS_LOCKED(a_0, a_1._f46);
//    }
//    VEHICLE::_SET_VEHICLE_NEON_LIGHTS_COLOUR(a_0, a_1._f4A, a_1._f4B, a_1._f4C);
//    VEHICLE::_SET_VEHICLE_NEON_LIGHT_ENABLED(a_0, 2, GAMEPLAY::IS_BIT_SET(a_1._f4D, 28));
//    VEHICLE::_SET_VEHICLE_NEON_LIGHT_ENABLED(a_0, 3, GAMEPLAY::IS_BIT_SET(a_1._f4D, 29));
//    VEHICLE::_SET_VEHICLE_NEON_LIGHT_ENABLED(a_0, 0, GAMEPLAY::IS_BIT_SET(a_1._f4D, 30));
//    VEHICLE::_SET_VEHICLE_NEON_LIGHT_ENABLED(a_0, 1, GAMEPLAY::IS_BIT_SET(a_1._f4D, 31));
//    VEHICLE::SET_VEHICLE_IS_STOLEN(a_0, GAMEPLAY::IS_BIT_SET(a_1._f4D, 10));
//    if ((VEHICLE::GET_VEHICLE_LIVERY_COUNT(a_0) > 1) && (a_1._f43 >= 0)) {
//      VEHICLE::SET_VEHICLE_LIVERY(a_0, a_1._f43);
//    }
//    if ((a_1._f45 > -1) && (a_1._f45 < 255)) {
//      if (!VEHICLE::IS_THIS_MODEL_A_BICYCLE(ENTITY::GET_ENTITY_MODEL(a_0))) {
//        if (VEHICLE::IS_THIS_MODEL_A_BIKE(ENTITY::GET_ENTITY_MODEL(a_0))) {
//          if (a_1._f45 == 6) {
//            sub_b86b(a_0, a_1._f45);
//          }
//        } else {
//          sub_b86b(a_0, a_1._f45);
//        }
//      }
//    }
//    if (VEHICLE::IS_VEHICLE_A_CONVERTIBLE(a_0, 0)) {
//      if (((a_1._f44 == 0) || (a_1._f44 == 3)) || (a_1._f44 == 5)) {
//        VEHICLE::RAISE_CONVERTIBLE_ROOF(a_0, 1);
//      } else {
//        VEHICLE::LOWER_CONVERTIBLE_ROOF(a_0, 1);
//      }
//    }
//    if (a_3) {
//      sub_b599(&a_0, &a_1._f9, &a_1._f3B);
//    }
//    if ((!VEHICLE::IS_THIS_MODEL_A_HELI(a_1._f42)) && (!VEHICLE::IS_THIS_MODEL_A_BOAT(a_1._f42))) {
//      for (v_6 = 0; v_6 <= 11; v_6 += 1) {
//        if (GAMEPLAY::IS_BIT_SET(a_1._f4D, sub_b4e9(v_6 + 1))) {
//          if (!VEHICLE::IS_VEHICLE_EXTRA_TURNED_ON(a_0, v_6 + 1)) {
//            VEHICLE::SET_VEHICLE_EXTRA(a_0, v_6 + 1, 0);
//          }
//        } else if (VEHICLE::IS_VEHICLE_EXTRA_TURNED_ON(a_0, v_6 + 1)) {
//          VEHICLE::SET_VEHICLE_EXTRA(a_0, v_6 + 1, 1);
//        }
//      }
//    }
//    if (VEHICLE::IS_THIS_MODEL_A_PLANE(a_1._f42)) {
//      if (!GAMEPLAY::IS_BIT_SET(a_1._f4D, 23)) {
//        if (GAMEPLAY::IS_BIT_SET(a_1._f4D, 22)) {
//          VEHICLE::_SET_VEHICLE_LANDING_GEAR(a_0, 2);
//        } else {
//          VEHICLE::_SET_VEHICLE_LANDING_GEAR(a_0, 3);
//        }
//      } else {
//        VEHICLE::_SET_VEHICLE_LANDING_GEAR(a_0, 4);
//      }
//    }
//    if (GAMEPLAY::IS_BIT_SET(a_1._f4D, 27)) {
//      DECORATOR::DECOR_SET_BOOL(a_0, "IgnoredByQuickSave", 1);
//    } else {
//      DECORATOR::DECOR_SET_BOOL(a_0, "IgnoredByQuickSave", 0);
//    }
//  }
//}

//AUDIO::START_AUDIO_SCENE("DLC_MPHEIST_DRIVE_INTO_GARAGE_SCENE");
//if (l_158 == 4) {
//    AUDIO::TRIGGER_MUSIC_EVENT("MP_MC_RADIO_FADE");
//}
