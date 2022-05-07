using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using static vehiclemod.data;
using UnityEngine.Rendering.PostProcessing;
using UnhollowerRuntimeLib;

namespace vehiclemod
{
    public class main : MelonMod
    {
        //MISC STUFF
        public static List<AssetBundle> lb = new List<AssetBundle>();
        private Ray ray;
        private RaycastHit hit;

        //VEHICLE PART
        public static Dictionary<int, GameObject> vehicles = new Dictionary<int, GameObject>();
        public static Transform MyPosition = null;
        public static int targetcar = 0;

        //LEVEL PART
        public static int levelid = 0;
        public static string levelname = "";
        public static int MyId = 0;
        public static bool hook = false;

        //CHECK PART FOR VEHICLE CONTROLLER
        public static bool isSit = false;
        public static bool allowdrive = false;
        public static bool changedDrivePlace = false;
        public static bool isChat = false;
        private static string MyNick = "Locall";
        public override void OnApplicationStart()
        {
            DirectoryInfo dir = new DirectoryInfo("Mods/vehiclemod");
            DirectoryInfo[] vehicles = dir.GetDirectories("*");
            AssetBundle load = AssetBundle.LoadFromFile("Mods/vehiclemod\\menu.addon");
            ClassInjector.RegisterTypeInIl2Cpp<WheelCollider>();
            ClassInjector.RegisterTypeInIl2Cpp<VehComponent>();
            ClassInjector.RegisterTypeInIl2Cpp<IKVH>();
            foreach (var i in AppDomain.CurrentDomain.GetAssemblies()) if (i.GetName().Name == "SkyCoop") hook = true;
            if (hook) MelonLogger.Msg("Integration with [SkyCoop API]"); else MelonLogger.Msg("[SkyCoop API] NOT FOUND!");
            if (load) lb.Add(load);

            foreach (DirectoryInfo i in vehicles)
            {
                string PATH = i.FullName + "\\";

                using (StreamReader sr = new StreamReader(PATH + "info.ini"))
                {
                    KeyValuePair<string, string>[] dataco = new KeyValuePair<string, string>[1];
                    string name = "NaN";
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line != "" && !line.StartsWith("[") && !line.EndsWith("]") && !line.StartsWith("(") && !line.EndsWith(")"))
                        {
                            string[] data = line.Split('=');
                            if (data[0] == "Prefab-Name") name = data[1];
                            dataco[dataco.Length - 1] = new KeyValuePair<string, string>(data[0], data[1]);
                            Array.Resize(ref dataco, dataco.Length + 1);
                        }
                    }
                    MenuControll.addonData.Add(name, dataco);
                }

                load = AssetBundle.LoadFromFile(PATH + GetInfo(i.Name, "FileName"));
                if (load)
                {
                    lb.Add(load);
                    MelonLogger.Msg("[PREINIT] [" + GetInfo(i.Name, "FileName") + "] Successfull Loaded");
                }
                else
                    MelonLogger.Msg("[PREINIT] [" + GetInfo(i.Name, "FileName") + "] File corrupted or not supported. Or maybe not found");
            }
            MelonLogger.Msg("[PREINIT] Vehicle mod loaded and vehicle files too");
        }
        public override void OnSceneWasLoaded(int level, string name)
        {
            levelid = level;
            levelname = name;
            if (hook) MyNick = SkyCoop.MyMod.MyChatName;
            if (levelname != "Empty" && levelname != "MainMenu" && levelname != "Boot")
            {
                MelonLogger.Msg("[InitCanvas] Spawning Menu on Level:> " + levelname);
                MenuControll.menuc();
            }
            if (GameManager.GetVpFPSPlayer()) VehicleController.myparent = GameManager.GetVpFPSPlayer().transform.parent;
        }
        public override void OnUpdate()
        {
            if (hook) if (SkyCoop.MyMod.LoadingScreenIsOn && VehicleController.myparent)
            {
                GameManager.GetVpFPSPlayer().transform.SetParent(VehicleController.myparent);
            }
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "" || !GameManager.GetPlayerTransform()) return;//check
            if (!VehicleController.cameracar)
            {
                VehicleController.cameracar = GameObject.Instantiate(new GameObject("00100"), Vector3.zero, Quaternion.identity).transform;
                VehicleController.cameracar.gameObject.SetActive(false);
                VehicleController.cameracar.gameObject.AddComponent<Camera>().CopyFrom(GameManager.GetVpFPSCamera().m_Camera.GetComponent<Camera>());
                CopyComponent(GameManager.GetVpFPSCamera().m_Camera.GetComponent<CameraEffects>(), VehicleController.cameracar.gameObject);
                CopyComponent(GameManager.GetVpFPSCamera().m_Camera.GetComponent<FlareLayer>(), VehicleController.cameracar.gameObject);
                CopyComponent(GameManager.GetVpFPSCamera().m_Camera.GetComponent<PostProcessLayer>(), VehicleController.cameracar.gameObject);
                VehicleController.cameracar.gameObject.GetComponent<PostProcessLayer>().volumeTrigger = VehicleController.cameracar;
            }
            if (hook) MyId = SkyCoop.API.m_MyClientID;
            if (hook) isChat = SkyCoop.MyMod.chatInput.IsActive();
            ray = GameManager.GetVpFPSCamera().m_Camera.ScreenPointToRay(Input.mousePosition);
            MyPosition = GameManager.GetVpFPSPlayer().transform;

            if (Input.GetKeyDown(KeyCode.L) && !isChat && isSit && allowdrive) GetObj(targetcar).GetComponent<VehComponent>().vehicleData.m_Light = !GetObj(targetcar).GetComponent<VehComponent>().vehicleData.m_Light;
            // SIT EXECUTE
            if (Input.GetKeyDown(KeyCode.E) && !isChat && !InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled)
            {
                if (!isSit && Physics.Raycast(ray, out hit, 3f))
                {
                    GameObject car = hit.collider.gameObject;
                    if (!car.GetComponent<VehComponent>()) return;
                    int number = car.GetComponent<VehComponent>().vehicleData.m_OwnerId;
                    allowdrive = isDrive(number);
                    targetcar = number;
                }
                if (targetcar > -1) VehicleController.SitCar(targetcar);
            }
            if (Input.GetKeyDown(KeyCode.Mouse2)) VehicleController.fps = !VehicleController.fps;
            MenuControll.openmenu.gameObject.active = InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled;
            if (!InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled) MenuControll.MenuMainCars.gameObject.SetActive(false);
        }
        public override void OnFixedUpdate()
        {
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "" || !GameManager.GetPlayerTransform()) return;
            if (isSit) VehicleController.MoveDrive(targetcar);
        }
        public override void OnLateUpdate()
        {
            if (levelname == "Empty" || levelname == "MainMenu" || levelname == "Boot" || levelname == "" || !GameManager.GetPlayerTransform()) return;

            MenuControll.Update(0); // COUNT CARS
            MenuControll.Update(1); // COOUNT SPEED
            MenuControll.Update(2); // COUNT FUEL
            if (!isSit)
            {
                MenuControll.Open(1);
            }
            if (vehicles.Count > 0 && Physics.Raycast(ray, out hit, 3f) && !isSit && !isChat && !InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled)
            {
                GameObject ho = hit.transform.gameObject;
                if (!ho.GetComponent<VehComponent>()) return;
                InfoMain values = ho.GetComponent<VehComponent>().vehicleData;
                MenuControll.Open(2);
                MenuControll.CarStatScreen(values.m_OwnerName, values.m_AllowSit, isDrive(values.m_OwnerId), values.m_CurFuel);
            }
            else
                MenuControll.Open(22);
        }
        public static bool deletecar(int PlayerId, int who)
        {
            if (GetObj(PlayerId) && isDrive(PlayerId) && CountPassangers(PlayerId) == 0)
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
            string playername;

            if (hook) if (MyId == PlayerId) playername = MyNick; else playername = SkyCoop.MyMod.playersData[PlayerId].m_Name;
            else playername = "Locall";

            key1.AddComponent<VehComponent>().NEWDATA(PlayerId, name, playername);

            if (MyId == PlayerId) NETHost.NetSpawnCar(name, key1.transform.position, key1.transform.rotation);
            key1.gameObject.layer = LayerMask.NameToLayer("Player");
            vehicles.Add(PlayerId, key1);
        }
        private void loot(GameObject bagage)
        {
            GameObject conta = GameObject.Instantiate(Resources.Load<GameObject>("CONTAINER_MetalBox"), Vector3.one, Quaternion.identity);
            conta.transform.root.position = bagage.transform.position;
            conta.transform.SetParent(bagage.transform);
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
