﻿using HarmonyLib;
using System;
using vehiclemod;
namespace vehiclemod
{
    [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadSceneWithLoadingScreen")] // FUCK THE OBJ
    public static class GameManager_LoadSceneWithLoadingScreen
    {
        private static void Prefix(ref string sceneName)
        {
            if (main.levelname != "Empty" && main.levelname != "MainMenu" && main.levelname != "Boot" && main.levelname != "" && GameManager.GetPlayerTransform())
                if (VehicleController.myparent != GameManager.GetPlayerTransform().transform.parent) GameManager.GetPlayerTransform().transform.SetParent(VehicleController.myparent);
        }
    }
    [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadScene", new Type[] { typeof(string), typeof(string) })] // FUCK THE OBJ
    public class GameManager_LoadSceneOverLoad1
    {
        public static void Prefix()
        {
            if (main.levelname != "Empty" && main.levelname != "MainMenu" && main.levelname != "Boot" && main.levelname != "" && GameManager.GetPlayerTransform())
                if (VehicleController.myparent != GameManager.GetPlayerTransform().transform.parent) GameManager.GetPlayerTransform().transform.SetParent(VehicleController.myparent);
        }
    }
    [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadScene", new Type[] { typeof(string) })] // FUCK THE OBJ
    public class GameManager_LoadSceneOverLoad2
    {
        public static void Prefix()
        {
            if (main.levelname != "Empty" && main.levelname != "MainMenu" && main.levelname != "Boot" && main.levelname != "" && GameManager.GetPlayerTransform())
                if (VehicleController.myparent != GameManager.GetPlayerTransform().transform.parent) GameManager.GetPlayerTransform().transform.SetParent(VehicleController.myparent);
        }
    }

}
