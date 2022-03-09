using MelonLoader;
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

        //VEHICLE PART
        public static Dictionary<int, GameObject> vehicles = new Dictionary<int, GameObject>();
        public static Dictionary<int, String> vehicledata = new Dictionary<int, String>();
        public static Dictionary<int, bool> drivers = new Dictionary<int, bool>();

        public static Transform MyPosition = null;
        public static GameObject targetcar = null;

        //LEVEL PART
        public static int levelid = 0;
        private string levelname = "";
        public static int MyId = 0;
        private static string MyNick;

        //CHECK PART FOR VEHICLE CONTROLLER
        public static bool isSit = false;
        public static bool allowdrive = false;
        private static bool allowsit = true;

        public override void OnApplicationStart()
        {
            DirectoryInfo dir = new DirectoryInfo("Mods/vehiclemod");
            FileInfo[] addons = dir.GetFiles("*.addon");
            foreach (FileInfo i in addons)
            {
                AssetBundle load = AssetBundle.LoadFromFile("Mods/vehiclemod\\" + i.Name);
                if (load)
                {
                    MelonLogger.Msg("[PREINIT] Loaded :CAR|ADDON: " + i.Name); lb.Add(load);
                }
                else
                    MelonLogger.Msg("[PREINIT] [" + i.Name + "] File corrupted or not supported");
            }

            MelonLogger.Msg("[PREINIT] Vehicle mod loaded and .addon files too");
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
            targetcar = null;
            if (levelname != "Empty" && levelname != "MainMenu" && levelname != "Boot")
            {
                MelonLogger.Msg("[InitCanvas] Spawning Menu on Level:> " + levelname);
                MenuControll.menuc();
                Transform newcam = GameObject.Instantiate(new GameObject("00100"), Vector3.zero, Quaternion.identity).transform;
                newcam.gameObject.SetActive(false);
                newcam.gameObject.AddComponent<Camera>();
                VehicleController.cameracar = newcam;

            }
            if (GetObj(MyId))
            {
                NETHost.NetPacketStat(bool.Parse(CarData(MyId)[0]), allowsit, VehicleController.curfuel, CarData(MyId)[0]);
            }
        }
        public override void OnUpdate()
        {
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "") return;
            VehicleController.turn = Input.GetAxis("Horizontal");
            VehicleController.move = Input.GetAxis("Vertical");
            MyId = API.m_MyClientID;
            MyNick = SkyCoop.MyMod.MyChatName;
            ray = GameManager.GetVpFPSCamera().m_Camera.ScreenPointToRay(Input.mousePosition);
            MyPosition = GameManager.GetPlayerTransform().transform;
            if (Input.GetKeyDown(KeyCode.Escape)) if (Paused) Paused = false; else Paused = true;
            if (Input.GetKeyDown(KeyCode.G)) if (Physics.Raycast(ray, out hit, 5f)) { 
                    MelonLogger.Msg(hit.transform.gameObject.name);
                    foreach (GameObject i in hit.transform.parent)
                    {
                        MelonLogger.Msg(i.name);
                    }
                }
                    if (Input.GetKeyDown(KeyCode.E))// f////////////////////////////////////////////////
            {
                int number = -1;
                if (!isSit && Physics.Raycast(ray, out hit, 5f))
                {
                    GameObject car = hit.transform.gameObject;
                    if (car.name == "BAGAGE")
                    {
                        loot(car);
                        return;
                    }
                    try { number = int.Parse(car.name); } catch (Exception e) { return; };

                    if (!VehicleController.cameracar) return;
                    if (!car.transform.Find("CAMERACENTER")) return;
                    if (!car.transform.Find("SIT1")) return;

                    allowdrive = !isDrive(number);

                    targetcar = car;
                }
                VehicleController.SitCar(targetcar);
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))

                if (VehicleController.cameracar && VehicleController.fps == false)
                    VehicleController.fps = true;
                else
                    VehicleController.fps = false;

            if (Paused && MenuControll.openmenu)
                MenuControll.openmenu.gameObject.SetActive(true);

            else if (MenuControll.openmenu && !Paused)
            {
                MenuControll.openmenu.gameObject.SetActive(false);
                if (MenuControll.MenuMainCars) MenuControll.MenuMainCars.gameObject.SetActive(false);
            }
        }
        private IEnumerator sendMycarPos(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if(GetObj(MyId)) 
                NETHost.NetCar(MyId, data.CarData(MyId)[0], data.GetObj(MyId).transform.position, data.GetObj(MyId).transform.rotation);
        }
        public override void OnLateUpdate()
        {
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "") return;

            if (vehicles.ContainsKey(MyId) && !isDrive(MyId))
                sendMycarPos(2);

            MenuControll.Update(0); // COUNT CARS
            MenuControll.Update(1); // COOUNT SPEED
            MenuControll.Update(2); // COUNT FUEL

            if (Physics.Raycast(ray, out hit, 3f) && !isSit)
            {
                GameObject ho = hit.transform.gameObject;
                if (ho.name.Length <= 2)
                {
                    int number;
                    string othname = "Unknown";
                    try { number = int.Parse(ho.name); } catch (Exception e) { return; }
                    if (!vehicledata.ContainsKey(number)) return;
                    othname = SkyCoop.MyMod.playersData[number].m_Name;
                    if (number == MyId) othname = MyNick;
                    if (!vehicledata.ContainsKey(number)) return;

                    MenuControll.Open(2);
                    MenuControll.CarStatScreen(othname, bool.Parse(CarData(number)[0]), !isDrive(number), Mathf.Round(VehicleController.curfuel));
                } else {
                    MenuControll.Open(22);
                }
            }
            else
                MenuControll.Open(22);
        }
        public override void OnFixedUpdate()
        {
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "") return;
            if (vehicles.Count != 0) foreach (var i in vehicles) if (i.Value) VehicleController.wheel(i.Value);
            if (isSit) VehicleController.MoveDrive(targetcar);
        }
        public static Boolean deletecar(int PlayerId, int who)
        {
            if (GetObj(PlayerId)) {
                MelonLogger.Msg("[Car spawner] Car Already exist, Deleting it:> " + PlayerId);
                GameObject.Destroy(GetObj(PlayerId));
                vehicles.Remove(PlayerId);
                if(who == 0) vehicledata.Remove(PlayerId);
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


            GameObject key1 = data.LoadObject(name);

            if (!key1) { MelonLogger.Msg("[Car spawner] This Car Doesn't exist: " + name); return; }
            if (Position == Vector3.zero)
                key1 = GameObject.Instantiate(key1, MyPosition.position + MyPosition.up * 2 + MyPosition.forward * 6f, Rotation);
            else
                key1 = GameObject.Instantiate(key1, Position, Rotation);

            MelonLogger.Msg("[Car spawner] CarId |> " + PlayerId + " :Scene: " + SceneId + " :AT: " + Position.ToString() + " <|");

            key1.name = PlayerId.ToString();
            key1.tag = "CarVehicleMod";
            key1.layer = LayerMask.NameToLayer("Player");

            Transform[] i = key1.GetComponentsInChildren<Transform>();

            foreach (Transform b in i)
            {
                b.gameObject.layer = LayerMask.NameToLayer("NPC");
            }

            vehicles.Add(PlayerId, key1);
            if (MyId == PlayerId)
            {
                NETHost.NetSpawnCar(name, key1.transform.position, key1.transform.rotation);
            }
            data.UpdateDriver(PlayerId, false);
            data.UpdateCarData(PlayerId, true, true, 50, name);
        }
        private void loot(GameObject bagage)
        {
            var container = bagage.GetComponent<Container>();
            var guidCmp = bagage.GetComponent<ObjectGuid>();
            string guid = bagage.GetComponent<ObjectGuid>().m_Guid;
            if (!guidCmp)
            {
                bagage.AddComponent<ObjectGuid>();
            }

            if (guid == null)
                guidCmp.Set(MyId.ToString());

            if (!container)
            {
                bagage.AddComponent<Container>();
                container.GetComponent<Container>().m_Inspected = true;
            }

            guidCmp.Set(bagage.transform.parent.name.ToString());
        }
    }
}
