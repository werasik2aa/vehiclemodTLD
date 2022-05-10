using HarmonyLib;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
namespace vehiclemod
{
    [HarmonyPatch(typeof(WheelCollider), "LoadWheelCollider")] /* https://github.com/werasik2aa/BringBackComponents */
    public class WheelCollider : Collider
    {
        public WheelCollider(IntPtr Value) : base (Value) { }
        public float radius;
        public bool isGrounded { get; }
        public float steerAngle;
        public float brakeTorque;
        public float motorTorque;
        public WheelFrictionCurve sidewaysFriction;
        public WheelFrictionCurve forwardFriction;
        public float wheelDampingRate;
        public float mass;
        public float forceAppPointDistance;
        public JointSpring suspensionSpring;
        public float suspensionDistance;
        public float sprungMass;
        public Vector3 center;
        public float rpm;
        public delegate void ConfigureVehicleSubsteps(IntPtr ptr, ref float speedThreshold, ref int stepsBelowThreshold, ref int stepsAboveThreshold);
        public delegate void GetWorldPose(IntPtr ptr, out Vector3 ptr2, out Quaternion ptr3);
        public delegate Vector3 get_center(IntPtr ptr, out Vector3 ptr2);
        public delegate void set_centerS(IntPtr ptr, Vector3 Value);
        public delegate WheelFrictionCurve get_forwardFriction(IntPtr ptr); //FORWARD FR
        public delegate void set_forwardFriction(IntPtr ptr, WheelFrictionCurve Value);
        public delegate WheelFrictionCurve get_sidewaysFriction(IntPtr ptr); //SIDEWAY FR
        public delegate void set_sidewaysFriction(IntPtr ptr, WheelFrictionCurve Value);
        public delegate JointSpring get_suspensionSpring(IntPtr ptr); //SUS SPRING
        public delegate void set_suspensionSpring(IntPtr ptr, JointSpring Value);
        public delegate void ResetSprungMasses();
        public delegate float get_motorTorque(IntPtr ptr); //Torque motor
        public delegate void set_motorTorque(IntPtr ptr, float Value);
        public delegate float get_brakeTorque(IntPtr ptr); // BrakeToque
        public delegate void set_brakeTorque(IntPtr ptr, float Value);
        public delegate float get_steerAngle(IntPtr ptr); // steerAngle
        public delegate void set_steerAngle(IntPtr ptr, float Value);
        public delegate float get_radius(IntPtr ptr); // Radius
        public delegate void set_radius(IntPtr ptr, float Value);
        public delegate float get_sprungMass(IntPtr ptr); // MASS sprung
        public delegate void set_sprungMass(IntPtr ptr, float Value);
        public delegate float get_suspensionDistance(IntPtr ptr); // Suspension dist
        public delegate void set_suspensionDistance(IntPtr ptr, float Value);
        public delegate float get_mass(IntPtr ptr); //MASS
        public delegate void set_mass(IntPtr ptr, float Value);
        public delegate float get_wheelDampingRate(IntPtr ptr); // WHEEL damping rate
        public delegate void set_wheelDampingRate(IntPtr ptr, float Value);
        public delegate float get_forceAppPointDistance(IntPtr ptr); // WHEEL damping rate
        public delegate void set_forceAppPointDistance(IntPtr ptr, float Value);
        public delegate float get_rpm(IntPtr ptr); //RMP
    }
    [HarmonyPatch(typeof(GameManager), "LoadSceneWithLoadingScreen")] // FUCK THE OBJ
    public static class GameManager_LoadSceneWithLoadingScreen
    {
        private static void Prefix()
        {
            if (main.levelname != "Empty" && main.levelname != "MainMenu" && main.levelname != "Boot" && main.levelname != "" && GameManager.GetPlayerTransform())
                if (VehicleController.myparent != GameManager.GetPlayerTransform().transform.parent) GameManager.GetPlayerTransform().transform.SetParent(VehicleController.myparent);
            if(data.GetObj(main.targetcar)) GameObject.DontDestroyOnLoad(data.GetObj(main.targetcar));
            if(main.isSit) main.changedDrivePlace = true;
        }
    }
    [HarmonyPatch(typeof(GameManager), "LoadScene", new Type[] { typeof(string), typeof(string) })] // FUCK THE OBJ
    public class GameManager_LoadSceneOverLoad1
    {
        public static void Prefix()
        {
            if (main.levelname != "Empty" && main.levelname != "MainMenu" && main.levelname != "Boot" && main.levelname != "" && GameManager.GetPlayerTransform())
                if (VehicleController.myparent != GameManager.GetPlayerTransform().transform.parent) GameManager.GetPlayerTransform().transform.SetParent(VehicleController.myparent);
            if (data.GetObj(main.targetcar)) GameObject.DontDestroyOnLoad(data.GetObj(main.targetcar));
            if (main.isSit) main.changedDrivePlace = true;
        }
    }
    [HarmonyPatch(typeof(GameManager), "LoadScene", new Type[] { typeof(string) })] // FUCK THE OBJ
    public class GameManager_LoadSceneOverLoad2
    {
        public static void Prefix()
        {
            if (main.levelname != "Empty" && main.levelname != "MainMenu" && main.levelname != "Boot" && main.levelname != "" && GameManager.GetPlayerTransform())
                if (VehicleController.myparent != GameManager.GetPlayerTransform().transform.parent) GameManager.GetPlayerTransform().transform.SetParent(VehicleController.myparent);
            if (data.GetObj(main.targetcar)) GameObject.DontDestroyOnLoad(data.GetObj(main.targetcar));
            if (main.isSit) main.changedDrivePlace = true;
        }
    }
}
