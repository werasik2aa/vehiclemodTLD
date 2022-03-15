﻿using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using SkyCoop;
using System;
using System.IO;
using System.Collections;
using static vehiclemod.data;

namespace vehiclemod
{
    public class main : MelonMod
    {
        //MISC STUFF
        public static List<AssetBundle> lb = new List<AssetBundle>();
        private Ray ray;
        private RaycastHit hit;
        private static bool Paused;
        public static bool light;

        //VEHICLE PART
        public static Dictionary<int, GameObject> vehicles = new Dictionary<int, GameObject>();
        public static Dictionary<int, String> vehicledata = new Dictionary<int, String>();
        public static Dictionary<int, bool> drivers = new Dictionary<int, bool>();

        public static Transform MyPosition = null;
        public static int targetcar = 0;

        //LEVEL PART
        public static int levelid = 0;
        public static string levelname = "";
        public static int MyId = 0;
        private static string MyNick;

        //CHECK PART FOR VEHICLE CONTROLLER
        public static bool isSit = false;
        public static bool allowdrive = false;
        private static bool allowsit = true;

        public override void OnApplicationStart()
        {
            DirectoryInfo dir = new DirectoryInfo("Mods/vehiclemod");
            DirectoryInfo[] vehicles = dir.GetDirectories("*");
            AssetBundle load = AssetBundle.LoadFromFile("Mods/vehiclemod\\menu.addon");
            if (load) lb.Add(load);
            load = null;
            foreach (DirectoryInfo i in vehicles)
            {
                string Name = "NaN";
                string Description = "NaN";
                string Icon = "NaN";
                string FileName = "NaN";
                string PATH = i.FullName+"\\";
                using (StreamReader sr = new StreamReader(PATH + "info.ini"))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line != "" && !line.StartsWith("[") && !line.EndsWith("]") && !line.StartsWith("(") && !line.EndsWith(")"))
                        {
                            string[] data = line.Split('=');
                            if (data[0] == "Prefab-Name") Name = data[1];
                            if (data[0] == "Descriotion") Description = data[1];
                            if (data[0] == "FileName") FileName = data[1];
                            if (data[0] == "Icon") Icon = data[1];
                        }
                    }
                }
                MenuControll.info.Add((PATH + "=" + FileName + "=" + Icon + "=" + Name + "=" + Description).Split('='));

                load = AssetBundle.LoadFromFile(PATH + FileName);
                if (load)
                {
                    lb.Add(load);
                    MelonLogger.Msg("[PREINIT] [" + FileName + "] Successfull Loaded");
                }
                else
                    MelonLogger.Msg("[PREINIT] [" + FileName + "] File corrupted or not supported. Or maybe not found");
            }

            MelonLogger.Msg("[PREINIT] Vehicle mod loaded and vehicle files too");
        }
        public override void OnSceneWasLoaded(int level, string name)
        {
            levelid = level;
            levelname = name;
            Paused = false;

            MelonLogger.Msg("[Garbage Collector] Clearing Vehicle list on level:> " + levelname);
            vehicles.Clear();
            vehicledata.Clear();
            drivers.Clear();

            allowdrive = false;
            isSit = false; // SET FALSE BECAUSE LEVEL CHANGED AND CAR ERASED
            targetcar = -1;
            if (levelname != "Empty" && levelname != "MainMenu" && levelname != "Boot")
            {
                MelonLogger.Msg("[InitCanvas] Spawning Menu on Level:> " + levelname);
                MenuControll.menuc();
                Transform newcam = GameObject.Instantiate(new GameObject("00100"), Vector3.zero, Quaternion.identity).transform;
                newcam.gameObject.SetActive(false);
                VehicleController.cameracar = newcam;

            }
           
            VehicleController.myparent = GameManager.GetVpFPSPlayer().transform.parent;
            
        }
        public override void OnUpdate()
        {
            if (VehicleController.move != 0 && main.allowdrive) //ACELERATION
                VehicleController.accel += 5 * Time.deltaTime;
            else
                VehicleController.accel = 1;
            if (SkyCoop.MyMod.LoadingScreenIsOn && VehicleController.myparent)
            {
                GameManager.GetVpFPSPlayer().transform.SetParent(VehicleController.myparent);
            }
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "" || !GameManager.GetPlayerTransform()) return;
            VehicleController.turn = Input.GetAxis("Horizontal");
            VehicleController.move = Input.GetAxis("Vertical");
            MyId = API.m_MyClientID;
            MyNick = SkyCoop.MyMod.MyChatName;
            if(vehicles.Count > 0) ray = GameManager.GetVpFPSCamera().m_Camera.ScreenPointToRay(Input.mousePosition);
            MyPosition = GameManager.GetVpFPSPlayer().transform;
            if (Input.GetKeyDown(KeyCode.L))
            {
                if(isSit && allowdrive)
                {
                    bool cur;
                    if (light)
                        cur = false;
                    else
                        cur = true;

                    VehicleController.CarLight(targetcar, cur);
                    NETHost.NetLightOn(targetcar, cur);
                }
            }
            // fuck
            if (Input.GetKeyDown(KeyCode.E))
            {
                int number = -1;
                int chair = 0;
                if (!isSit && Physics.Raycast(ray, out hit, 5f))
                {
                    if(!VehicleController.cameracar.GetComponent<Camera>())
                        VehicleController.cameracar.gameObject.AddComponent<Camera>().CopyFrom(GameManager.GetVpFPSCamera().m_Camera.GetComponent<Camera>());
                    GameObject car = hit.collider.gameObject;
                    if (car.name == "BAGAGE")
                    {
                        loot(car);
                        MelonLogger.Msg("[Interact] Bagage> " + car.name);
                        return;
                    }
                    try { number = int.Parse(car.name); } catch { return; };
                    if(!vehicles.ContainsKey(number)) return;

                    allowdrive = !isDrive(number);

                    targetcar = number;
                }

                if (allowdrive) 
                    chair = 1;
                else
                    chair = 2;

                if (isSit) chair = 0;

                if(targetcar > -1) VehicleController.SitCar(targetcar, chair);
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))

                if (VehicleController.cameracar && VehicleController.fps == false)
                    VehicleController.fps = true;
                else
                    VehicleController.fps = false;
            if (MenuControll.openmenu && MenuControll.MenuMainCars)
            {
                if (Input.GetKey(KeyCode.Space))
                    MenuControll.openmenu.gameObject.SetActive(true);
                else
                {
                    MenuControll.openmenu.gameObject.SetActive(false);
                    if (MenuControll.MenuMainCars) MenuControll.MenuMainCars.gameObject.SetActive(false);
                }
            }
        }
        private IEnumerator sendMycarPos(float waitTime)
        {
            NETHost.NetPacketStat(isDrive(MyId), allowsit, VehicleController.curfuel, CarData(MyId)[0]);
            yield return new WaitForSeconds(waitTime);
            if (GetObj(MyId))
                NETHost.NetCar(MyId, CarData(MyId)[0], GetObj(MyId).transform.position, GetObj(MyId).transform.rotation);
        }
        public override void OnLateUpdate()
        {
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "" || !GameManager.GetPlayerTransform()) return;

            if (!isSit)
            {
                GameManager.GetVpFPSPlayer().transform.SetParent(VehicleController.myparent);
            }

            if (vehicles.ContainsKey(MyId) && !isDrive(MyId))
                sendMycarPos(2);
            if (isSit)
            {
                SkyCoop.MyMod.MyAnimState = "Sit";
            }
            MenuControll.Update(0); // COUNT CARS
            MenuControll.Update(1); // COOUNT SPEED
            MenuControll.Update(2); // COUNT FUEL

            if (vehicles.Count > 0 && Physics.Raycast(ray, out hit, 3f) && !isSit)
            {
                GameObject ho = hit.transform.gameObject;
                if (!(ho.name.Length > 2))
                {
                    int number;
                    string othname = "Unknown";
                    try { number = int.Parse(ho.name); } catch { return; }
                    if (!vehicledata.ContainsKey(number)) return;
                    if (!GetObj(number).transform.parent.Find("CAMERACENTER")) return;

                    othname = SkyCoop.MyMod.playersData[number].m_Name;
                    if (number == MyId) othname = MyNick;

                    MenuControll.Open(2);
                    MenuControll.CarStatScreen(othname, bool.Parse(CarData(number)[0]), !isDrive(number), Mathf.Round(VehicleController.curfuel));
                }
                else
                {
                    MenuControll.Open(22);
                }
            }
            else
                MenuControll.Open(22);
        }
        public override void OnFixedUpdate()
        {
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "" || !GameManager.GetPlayerTransform()) return;
            if (vehicles.Count != 0) foreach (var i in vehicles) if (i.Value) VehicleController.wheel(int.Parse(i.Value.name));
            if (isSit) VehicleController.MoveDrive(targetcar);
        }
        public static bool deletecar(int PlayerId, int who)
        {
            if (GetObj(PlayerId) && !isDrive(PlayerId))
            {
                MelonLogger.Msg("[Car spawner] Car Already exist, Deleting it:> " + PlayerId);
                UpdateDriver(PlayerId, false);
                if (who == 0)
                {
                    GameManager.GetVpFPSPlayer().transform.SetParent(VehicleController.myparent);
                    if (targetcar != -1)
                        VehicleController.cameracar.SetParent(null);
                }
                GameObject.Destroy(GetObj(PlayerId));
                vehicles.Remove(PlayerId);
                return true;
            }
            return false;
        }
        public static void SpawnCar(int PlayerId, int SceneId, string name, Vector3 Position, Quaternion Rotation)
        {
            if (Position == Vector3.zero && isSit) return;
            bool a = vehicles.ContainsKey(PlayerId);

            if (PlayerId != MyId) if (a) return;

            if (PlayerId == MyId && a) if (deletecar(MyId, 0)) NETHost.NetDeleteCar();


            GameObject key1 = LoadObject(name);

            if (!key1) { MelonLogger.Msg("[Car spawner] This Car Doesn't exist: " + name); return; }
            if (Position == Vector3.zero)
                key1 = GameObject.Instantiate(key1, MyPosition.position + MyPosition.up * 2 + MyPosition.forward * 6f, Rotation);
            else
                key1 = GameObject.Instantiate(key1, Position, Rotation);

            MelonLogger.Msg("[Car spawner] CarId |> " + PlayerId + " :Scene: " + SceneId + " :AT: " + Position.ToString() + " <|");

            key1.name = PlayerId.ToString();
            key1.tag = "CarVehicleMod";
            key1.layer = LayerMask.NameToLayer("Player");

            GameObject[] par = key1.GetComponentsInChildren<GameObject>();
            foreach (GameObject b in par)
            {
                b.layer = LayerMask.NameToLayer("NPC");
                if (b.name == "BAGAGE") b.layer = LayerMask.NameToLayer("Container");
            }

            vehicles.Add(PlayerId, key1);
            if (MyId == PlayerId)
            {
                NETHost.NetSpawnCar(name, key1.transform.position, key1.transform.rotation);
            }
            UpdateDriver(PlayerId, false);
            UpdateCarData(PlayerId, true, true, 50, name);
        }
        private void loot(GameObject bagage)
        {
            Container container = bagage.GetComponent<Container>();

            if (!container)
            {
                var guidCmp = bagage.GetComponent<ObjectGuid>();
                bagage.AddComponent<ObjectGuid>();
                guidCmp = bagage.GetComponent<ObjectGuid>();
                guidCmp.Set(SkyCoop.MyMod.level_guid);
                bagage.AddComponent<Container>();
                container.GetComponent<Container>().m_Inspected = true;
            }
            container.m_NotPopulated = false;
            container.m_GearToInstantiate.Clear();
            container.UpdateContainer();
            container.name = "BAGAGE";
            container.m_CapacityKG = 100f;
            InterfaceManager.m_Panel_Container.SetContainer(container, container.m_LocalizedDisplayName.Text());;
            container.Open();
        }
        public static bool allowed(GameObject go)
        {

            var container = go.GetComponent<Container>();

            if (!container)
                return false;

            if (go.GetComponent<Lock>().m_LockState == LockState.Locked)
                return false;

            if (!container.IsInspected())
                return false;

            if (!container.IsEmpty())
                return false;

            return true;
        }
    }
}
