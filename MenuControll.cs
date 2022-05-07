using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static vehiclemod.data;


namespace vehiclemod
{
    public class MenuControll
    {
        // BODY OF CANVAS
        public static Transform openmenu;
        private static Transform Content;
        public static Transform MenuMainCars;
        private static Transform MenuMainStatCar;
        private static Transform CarStat;

        // INFO COMPONENT
        private static Slider Fuel;
        private static Slider Speed;
        private static Text CountCars;
        private static Text CountSpeed;
        private static Text NameTag;
        private static GameObject template;

        public static Dictionary<string, KeyValuePair<string, string>[]> addonData = new Dictionary<string, KeyValuePair<string, string>[]>();
        public static void menuc()
        {
            GameObject key1 = GameObject.Instantiate(main.lb[0].LoadAsset<GameObject>("menucars"), Vector3.zero, Quaternion.identity);
            key1.transform.Find("Stat").gameObject.SetActive(false);
            key1.transform.Find("Menu").gameObject.SetActive(false);
            key1.transform.Find("MenuCar").gameObject.SetActive(false);
            key1.transform.Find("CarInfo").gameObject.SetActive(false);
            key1.transform.Find("Templates").gameObject.SetActive(false);

            CarStat = key1.transform.Find("CarInfo");
            MenuMainCars = key1.transform.Find("Menu");
            Content = MenuMainCars.transform.Find("FULL/WINDOW/CARS");
            MenuMainStatCar = key1.transform.Find("Stat");
            openmenu = key1.transform.Find("MenuCar");
            template = key1.transform.Find("Templates/CARBTN").gameObject;

            Fuel = MenuMainStatCar.Find("Fuel").GetComponent<Slider>();
            Speed = MenuMainStatCar.Find("Speed").GetComponent<Slider>();

            CountCars = MenuMainCars.transform.Find("CountCar").GetComponent<Text>();
            CountSpeed = Speed.transform.Find("Content/Icon/Text").GetComponent<Text>();

            NameTag = CarStat.transform.Find("CarTag").GetComponent<Text>();

            openmenu.GetComponent<Button>().onClick.AddListener(new Action(() => Open(0)));

            ADDCARS();
        }
        private static void ADDCARS()
        {
            foreach (var i in addonData)
            {
                GameObject item = GameObject.Instantiate(template);
                item.transform.SetParent(Content);

                Button Car = item.GetComponent<Button>();
                Text Tag = item.transform.Find("Info").GetComponent<Text>();
                RawImage Icon = item.transform.Find("Icon").GetComponent<RawImage>();
                Action spawncar = new Action(() => main.SpawnCar(main.MyId, main.levelid, GetInfo(i.Key, "Prefab-Name"), Vector3.zero, Quaternion.identity));


                Car.onClick.AddListener(spawncar);
                Tag.text = GetInfo(i.Key, "Descriotion").Replace("/n", Environment.NewLine);

                item.SetActive(true);
            }
        }
        public static void Update(int i)
        {
            if (!GetObj(main.targetcar)) return;
            InfoMain gg = GetObj(main.targetcar).GetComponent<VehComponent>().vehicleData;
            if (i == 0 && CountCars)
            {
                CountCars.text = main.vehicles.Count.ToString();
            }
            if (i == 1 && Speed)
            {
                Speed.maxValue = gg.m_MaxSpeed;
                Speed.value = gg.m_CurSpeed;
                CountSpeed.text = Mathf.RoundToInt(gg.m_CurSpeed).ToString();
            }
            if (i == 2 && Fuel)
            {
                Fuel.maxValue = gg.m_MaxFuel;
                Fuel.value = gg.m_CurFuel;
            }
        }
        public static void CarStatScreen(string PlayerName, bool allowsit, bool allowdrive, float fuel)
        {
            string[] ch = new string[3];
            if (allowsit)
                ch[0] = "Allowed";
            else
                ch[0] = "Deny";

            if (allowdrive) 
                ch[1] = "Allowed";
            else
                ch[1] = "Deny";

            if (fuel >= 0) ch[2] = Mathf.Round(fuel).ToString();
            if (NameTag) NameTag.text = "Vehicle by: " + PlayerName + Environment.NewLine + "Can you SIT?: " + ch[0] + Environment.NewLine + "CAN You DRIVE?: " + ch[1] + Environment.NewLine + "FUEL: "+ch[2];
        }
        public static void Open(int i)
        {
            if (i == 0 && MenuMainCars)
                if (MenuMainCars.gameObject.active)
                    MenuMainCars.gameObject.SetActive(false);
                else
                    MenuMainCars.gameObject.SetActive(true);

            if (i == 1 && MenuMainStatCar)
                if (MenuMainStatCar.gameObject.active)
                    MenuMainStatCar.gameObject.SetActive(false);
            if (i == 11 && MenuMainStatCar)
                MenuMainStatCar.gameObject.SetActive(true);

            if (i == 2 && CarStat)
                    CarStat.gameObject.SetActive(true);
            if (i == 22 && CarStat)
                    CarStat.gameObject.SetActive(false);
        }
    }
}
