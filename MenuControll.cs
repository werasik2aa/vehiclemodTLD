using System;
using UnityEngine;
using UnityEngine.UI;


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
        public static void menuc()
        {
            GameObject key1 = GameObject.Instantiate(main.lb[0].LoadAsset<GameObject>("menucars"), Vector3.zero, Quaternion.identity);
            //GameManager.m_ActiveSceneIsRegion;
            key1.transform.Find("Stat").gameObject.SetActive(false);
            key1.transform.Find("Menu").gameObject.SetActive(false);
            key1.transform.Find("MenuCar").gameObject.SetActive(false);
            key1.transform.Find("CarInfo").gameObject.SetActive(false);

            
            CarStat = key1.transform.Find("CarInfo");
            MenuMainCars = key1.transform.Find("Menu");
            Content = MenuMainCars.transform.Find("FULL/WINDOW/CARS");
            MenuMainStatCar = key1.transform.Find("Stat");
            openmenu = key1.transform.Find("MenuCar");

            Fuel = MenuMainStatCar.Find("Fuel").GetComponent<Slider>();
            Speed = MenuMainStatCar.Find("Speed").GetComponent<Slider>();

            CountCars = MenuMainCars.transform.Find("CountCar").GetComponent<Text>();
            CountSpeed = Speed.transform.Find("Content/Icon/Text").GetComponent<Text>();

            NameTag = CarStat.transform.Find("CarTag").GetComponent<Text>();

            openmenu.GetComponent<Button>().onClick.AddListener(new Action(() => Open(0)));

            Content.Find("CAR0").GetComponent<Button>().onClick.AddListener(new Action(() => main.SpawnCar(main.MyId, main.levelid, "sedan", Vector3.zero, Quaternion.identity)));
            Content.Find("CAR1").GetComponent<Button>().onClick.AddListener(new Action(() => main.SpawnCar(main.MyId, main.levelid, "snowcar", Vector3.zero, Quaternion.identity)));
            
        }
        public static void Update(int i)
        {
            if (i == 0 && CountCars)
            {
                CountCars.text = main.vehicles.Count.ToString();
            }
            if (i == 1 && Speed)
            {
                Speed.maxValue = VehicleController.maxspeed;
                Speed.value = VehicleController.curspeed;
                CountSpeed.text = Mathf.RoundToInt(VehicleController.curspeed).ToString();
            }
            if (i == 2 && Fuel)
            {
                Fuel.maxValue = VehicleController.maxfuel;
                Fuel.value = VehicleController.curfuel;
            }
        }
        public static void CarStatScreen(string PlayerName, bool allowsit, bool allowdrive, float fuel)
        {
            string[] ch = new string[3];
            if(allowdrive) ch[0] = "Allowed";
            if (allowsit) ch[1] = "Allowed";
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
                else
                    MenuMainStatCar.gameObject.SetActive(true);
            if (i == 2 && CarStat)
                    CarStat.gameObject.SetActive(true);
            if (i == 22 && CarStat)
                    CarStat.gameObject.SetActive(false);
        }
    }
}
