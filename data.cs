using Il2CppSystem.Reflection;
using MelonLoader;
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
        public static String[] CarData(int ID)
        {
            if (main.vehicledata.TryGetValue(ID, out String[] ss))
                return ss;
            else
                return null;
        }
        public static void UpdateCarData(int PlayerId, bool allowdrive, bool allowsit, float fuel, string name)
        {
            string[] newvals = new string[4];
            newvals[0] = allowdrive.ToString();
            newvals[1] = allowsit.ToString();
            newvals[2] = fuel.ToString();
            newvals[3] = name;

            if (main.vehicledata.TryGetValue(PlayerId, out string[] ss))
            {
                if (bool.Parse(ss[0]) != allowdrive || bool.Parse(ss[1]) != allowsit || int.Parse(ss[2]) != fuel || ss[3] != name)
                    main.vehicledata[PlayerId] = newvals;
            }
            else
                main.vehicledata.Add(PlayerId, newvals);

        }
        public static void UpdateDriver(int ID, bool state)
        {
            if (main.drivers.TryGetValue(ID, out bool b))
                main.drivers[ID] = state;
            else
                main.drivers.Add(ID, state);
        }
        public static Boolean isDrive(int ID)
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
        public static void Hide()
        {
            foreach (var i in main.passanger)
            {
                if (SkyCoop.MyMod.players[i.Key]) SkyCoop.MyMod.players[i.Key].SetActive(false);
            }
        }
        public static int CountPassangers(int ID)
        {
            int i = 0;
            foreach (var j in main.passanger)
            {
                if (j.Value[0] == ID)
                {
                    i = i + 1;
                }
            }
            return i;
        }
        public static void UpdatePassanger(int CarID, int Number, int from)
        {
            int[] i = new int[2];
            i[0] = CarID;
            i[1] = Number;

            if (!GetObj(CarID)) return;
            if (Number > 0)
            {
                if (!main.passanger.ContainsKey(from))
                {
                    main.passanger.Add(from, i);
                    GameObject newob = GameObject.Instantiate(SkyCoop.MyMod.players[from]);
                    Transform sit = GetObj(CarID).transform.Find("SIT" + Number);
                    if (!sit || !newob) return;
                    newob.transform.SetParent(sit);
                    newob.transform.position = sit.position;
                    newob.transform.LookAt(sit.transform.forward * 2f);
                    newob.SetActive(true);
                    newob.name = from.ToString();
                }
            }
            else
            {
                if (main.passanger.ContainsKey(from))
                {
                    main.passanger.TryGetValue(from, out int[] data);
                    GameObject player = GetObj(data[0]).transform.Find("SIT" + data[1] + "/" + from).gameObject;
                    if (!player) return;
                    GameObject.Destroy(player);
                    SkyCoop.MyMod.players[from].SetActive(true);
                    main.passanger.Remove(from);
                }
            }
        }
        public static Component CopyComponent(Component which, GameObject to)
        {
            Component added = to.AddComponent(which.GetIl2CppType());

            FieldInfo[] fields = which.GetIl2CppType().GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(to.GetComponent(which.GetIl2CppType()), field.GetValue(which));
                if (field.Name == "volumeTrigger") return added;
                MelonLogger.Msg(which.name + " : " + field.Name + " : " + field.GetValue(which).ToString());
            }
            return added;
        }
        public static void Acceleration()
        {
            if (VehicleController.move != 0)
            {
                if (VehicleController.maxspeed - 1 > VehicleController.curspeed)
                {
                    if (VehicleController.move > 0)
                        VehicleController.accel += 2f * Time.deltaTime;
                    else
                        VehicleController.accel -= 2f * Time.deltaTime;
                }
            }
            else
            {
                if (VehicleController.accel > 0)
                    VehicleController.accel -= 1f * Time.deltaTime;
                else
                    VehicleController.accel += 1f * Time.deltaTime;
            }
            if (VehicleController.prevspeed >= VehicleController.curspeed && VehicleController.curspeed <= 0.3) VehicleController.accel = 0;
            if (!main.isSit) VehicleController.accel = 0;
        }
        public static float calkrange(float i) //CALCULATE VOLUME DEPENDS ON RANGE -> P-C
        {
            i = i / 100;
            i = 1 - i;
            Mathf.Clamp(i, 0, 1);
            return i;
        }
        public static String GetInfo(string addon, string what)
        {
            MenuControll.addonData.TryGetValue(addon, out KeyValuePair<string, string>[] keyvals);
            foreach (var ff in keyvals)
            {
                if(what == ff.Key) return ff.Value;
            }
            return "NaN";
        }
    }
}
