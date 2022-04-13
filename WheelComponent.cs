using UnityEngine;
using vehiclemod;
using static UnhollowerBaseLib.IL2CPP;
namespace BringBackComponents
{
    public class WheelComponent
    {
        public static Component GetComponent(Transform Wheel)
        {
            Component request = null;
            try
            {
                Component[] all = Wheel.GetComponents<Component>();
                foreach (var e in all)
                {
                    if (e.ToString().Equals("UnityEngine.WheelCollider") || e.ToString().Contains("UnityEngine.WheelCollider")) request = e;
                }
            }
            catch { }
            return request;
        }
        public static Component AddComponent(Transform Wheel) { Component o = Wheel.gameObject.AddComponent<WheelCollider>(); if (o) return o; else return null; }
        public static bool Set_SteerAngle(Transform Wheel, float Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_steerAngle>("UnityEngine.WheelCollider::set_steerAngle").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static float Get_SteerAngle(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_steerAngle>("UnityEngine.WheelCollider::get_steerAngle").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public static bool Set_MotorTorque(Transform Wheel, float Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_motorTorque>("UnityEngine.WheelCollider::set_motorTorque").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static float Get_MotorTorque(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_motorTorque>("UnityEngine.WheelCollider::get_motorTorque").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public static bool Set_BrakeTorque(Transform Wheel, float Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_brakeTorque>("UnityEngine.WheelCollider::set_brakeTorque").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static float Get_BrakeTorque(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_brakeTorque>("UnityEngine.WheelCollider::get_brakeTorque").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public bool Get_Center(Transform Wheel, out Vector3 Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.get_center>("UnityEngine.WheelCollider::get_center_Injected").Invoke(GetComponent(Wheel).Pointer, out Vector3 v);
                    Value = v;
                    return true;
                }
            }
            catch { }
            Value = Vector3.zero;
            return false;
        }
        public static bool Set_ConfigureVehicleSubsteps(Transform Wheel, ref float speedThreshold, ref int stepsBelowThreshold, ref int stepsAboveThreshold)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.ConfigureVehicleSubsteps>("UnityEngine.WheelCollider::ConfigureVehicleSubsteps").Invoke(GetComponent(Wheel).Pointer, ref speedThreshold, ref stepsBelowThreshold, ref stepsAboveThreshold);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static bool Set_Center(Transform Wheel, Vector3 Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_centerS>("UnityEngine.WheelCollider::set_center_Injected").Invoke(GetComponent(Wheel).Pointer, Value);
                }
            }
            catch { }
            return false;
        }
        public static float Get_Radius(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_radius>("UnityEngine.WheelCollider::get_radius").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public static bool Set_Radius(Transform Wheel, float Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_radius>("UnityEngine.WheelCollider::set_radius").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static float Get_SuspensionDistance(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_suspensionDistance>("UnityEngine.WheelCollider::get_suspensionDistance").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public static bool Set_SuspensionDistance(Transform Wheel, float Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_suspensionDistance>("UnityEngine.WheelCollider::set_suspensionDistance").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static float Get_Mass(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_mass>("UnityEngine.WheelCollider::get_mass").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public static bool Set_Mass(Transform Wheel, float Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_mass>("UnityEngine.WheelCollider::set_mass").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
                return false;
            }
            catch { }
            return false;
        }
        public static float Get_WheelDampingRate(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_wheelDampingRate>("UnityEngine.WheelCollider::get_wheelDampingRate").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public static bool Set_WheelDampingRate(Transform Wheel, float Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_wheelDampingRate>("UnityEngine.WheelCollider::set_wheelDampingRate").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static float Get_ForceAppPointDistance(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_forceAppPointDistance>("UnityEngine.WheelCollider::get_forceAppPointDistance").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public static float Get_RPM(Transform Wheel)
        {
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_rpm>("UnityEngine.WheelCollider::get_rpm").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return 0;
        }
        public static WheelFrictionCurve Get_ForwardFriction(Transform Wheel)
        {
            WheelFrictionCurve we = new WheelFrictionCurve();
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_forwardFriction>("UnityEngine.WheelCollider::get_forwardFriction_Injected").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return we;
        }
        public static WheelFrictionCurve Get_SidewaysFriction(Transform Wheel)
        {
            WheelFrictionCurve we = new WheelFrictionCurve();
            try
            {
                if (GetComponent(Wheel))
                    return ResolveICall<WheelCollider.get_sidewaysFriction>("UnityEngine.WheelCollider::get_sidewaysFriction_Injected").Invoke(GetComponent(Wheel).Pointer);
            }
            catch { }
            return we;
        }
        public static bool Set_ForceAppPointDistance(Transform Wheel, float Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_forceAppPointDistance>("UnityEngine.WheelCollider::set_forceAppPointDistance").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static bool Set_JointSpring(Transform Wheel, JointSpring Value)
        {
            try
            {
                if (GetComponent(Wheel))
                {
                    ResolveICall<WheelCollider.set_suspensionSpring>("UnityEngine.WheelCollider::set_suspensionSpring_Injected").Invoke(GetComponent(Wheel).Pointer, Value);
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static bool Get_WorldPose(Transform Wheel, out Vector3 pos, out Quaternion rot)
        {
            try
            {
                ResolveICall<WheelCollider.GetWorldPose>("UnityEngine.WheelCollider::GetWorldPose").Invoke(GetComponent(Wheel).Pointer, out Vector3 f, out Quaternion d);
                pos = f;
                rot = d;
                return true;
            }
            catch { }
            pos = Vector3.zero;
            rot = Quaternion.Euler(Vector3.zero);
            return false;
        }
    }
}
