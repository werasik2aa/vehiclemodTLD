using Il2CppSystem.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace vehiclemod
{
    public static class data
    {
        public static GameObject LoadObject(string name)
        {
            GameObject i = null;
            foreach (AssetBundle lb in main.lb)
            {
                if (lb.LoadAsset<GameObject>(name)) { i = lb.LoadAsset<GameObject>(name); break; };
            }
            return i;
        }
        public static void UpdateCarData(int carid, bool allowdrive, bool allowsit, bool isDrive, bool sound, bool light, float fuel, string playername)
        {
            GetObj(carid).GetComponent<VehComponent>().UpdateMainCarData(carid, allowdrive, allowsit, isDrive, sound, light, fuel, playername);
        }
        public static void UpdateDriver(int CarID, bool state)
        {
            GetObj(CarID).GetComponent<VehComponent>().UpdateDriver(state);
        }
        public static bool isDrive(int ID)
        {
            return GetObj(ID).GetComponent<VehComponent>().isDrive();
        }
        public static GameObject GetObj(int ID)
        {
            if (main.vehicles.Count <= 0) return null;
            main.vehicles.TryGetValue(ID, out GameObject g);
            return g;
        }
        public static int CountPassangers(int CarID)
        {
            return GetObj(CarID).GetComponent<VehComponent>().CountPassanger();
        }
        public static void UpdatePassanger(int CarID, int Number, int from)
        {
            if (Number == 0)
                GetObj(CarID).GetComponent<VehComponent>().DeletePassanger(from);
            else
                GetObj(CarID).GetComponent<VehComponent>().AddPassanger(from, Number);
        }
        public static Component CopyComponent(Component which, GameObject to)
        {
            Component added = to.AddComponent(which.GetIl2CppType());

            FieldInfo[] fields = which.GetIl2CppType().GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(to.GetComponent(which.GetIl2CppType()), field.GetValue(which));
                if (field.Name == "volumeTrigger") return added;
            }
            return added;
        }
        public static String GetInfo(string addon, string what)
        {
            MenuControll.addonData.TryGetValue(addon, out KeyValuePair<string, string>[] keyvals);
            foreach (var ff in keyvals)
            {
                if (what == ff.Key) { return ff.Value; };
            }
            return "NaN";
        }
    }
}
