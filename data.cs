using System;
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
        public static String[] CarData(int ID)
        {
                if (main.vehicledata.TryGetValue(ID, out String ss))
                    return ss.Split('+');
                else
                    return null;
        }
        public static void UpdateCarData(int PlayerId, bool allowdrive, bool allowsit, float fuel, string name)
        {

            if (main.vehicledata.TryGetValue(PlayerId, out string s))
            {
                string[] ss = s.Split('+');
                if (bool.Parse(ss[0]) != allowdrive || bool.Parse(ss[1]) != allowsit || int.Parse(ss[2]) != fuel || ss[3] != name)
                    main.vehicledata[PlayerId] = allowdrive + "+" + allowsit + "+" + fuel + "+" + name + "-1";
            }
            else
                main.vehicledata.Add(PlayerId, allowdrive + "+" + allowsit + "+" + fuel + "+" + name + "-1");

        }
        public static void UpdateDriver(int ID, bool state)
        {
            if (main.drivers.TryGetValue(ID, out bool b))
                    main.drivers[ID] = state;
                else
                    main.drivers.Add(ID, state);
        }
        public static bool isDrive(int ID)
        {
                if (main.drivers.TryGetValue(ID, out bool i))
                    return i;
                else
                    return false;
        }
        public static GameObject GetObj(int ID)
        {
            if (main.vehicles.Count == 0) return null;
            if (main.vehicles.TryGetValue(ID, out GameObject g))
                return g;
            else
                return null;
        }
    }
}
