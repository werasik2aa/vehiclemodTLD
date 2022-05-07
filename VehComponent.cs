﻿using UnityEngine;
using System;
using BringBackComponents;
using System.Collections.Generic;
using MelonLoader;
using static vehiclemod.data;
using UnhollowerRuntimeLib;

namespace vehiclemod
{
    public class InfoMain
    {
        public Transform m_VehicleMain = null;
        public List<Transform> m_Wheels = new List<Transform>();
        public List<Transform> m_Wheels_main = new List<Transform>();
        public List<Light> m_Lights = new List<Light>();
        public List<Transform> m_Sits = new List<Transform>();
        public Dictionary<int, int> Passangers = new Dictionary<int, int>();
        public int m_Type = -1;
        public int m_OwnerId = -1;
        public int m_MaxSpeed = 0;
        public int m_MaxFuel = 0;
        public string m_OwnerName = "NaN";
        public string m_VehicleName = "NaN";
        public float m_MotorTorque = 0;
        public float m_CurFuel = 0;
        public float m_CurSpeed = 0;
        public bool m_AllowSit = true;
        public bool m_AllowDrive = true;
        public bool m_isDrive = false;
        public bool m_SoundPlay = false;
        public bool m_Light = false;
        public AudioSource m_audio = null;
        public Vector3 m_Position;
        public Quaternion m_Rotation;
    }
    public class VehComponent : MonoBehaviour
    {
        public VehComponent(IntPtr ptr) : base(ptr) { }
        private float move = 0, turn = 0;
        public InfoMain vehicleData = null;
        public Rigidbody m_Rigidbody;
        public bool enableds = false;
        public void NEWDATA(int PlayerId, string name, string PlayerName)
        {
            vehicleData = new InfoMain();
            vehicleData.m_VehicleMain = transform;
            vehicleData.m_audio = transform.GetComponent<AudioSource>();
            m_Rigidbody = transform.GetComponent<Rigidbody>();
            m_Rigidbody.mass = float.Parse(GetInfo(name, "Weight"));
            m_Rigidbody.centerOfMass = Vector3.down;
            vehicleData.m_Type = int.Parse(GetInfo(name, "Type"));
            vehicleData.m_MotorTorque = float.Parse(GetInfo(name, "MotorTorque"));
            vehicleData.m_MaxFuel = int.Parse(GetInfo(name, "MaxFuel"));
            vehicleData.m_MaxSpeed = int.Parse(GetInfo(name, "MaxSpeed"));
            vehicleData.m_VehicleName = name;
            vehicleData.m_OwnerId = PlayerId;
            vehicleData.m_OwnerName = PlayerName;

            foreach (Transform g in transform.GetComponentsInChildren<Transform>())
            {
                if (g.name.StartsWith("Wheel"))
                {
                    if (g.GetChild(0))
                    {
                        g.gameObject.layer = LayerMask.NameToLayer("Player");
                        vehicleData.m_Wheels.Add(g);
                        if (g.name.EndsWith("Main")) vehicleData.m_Wheels_main.Add(g);
                        if (g.GetChild(0)) g.GetChild(0).gameObject.layer = LayerMask.NameToLayer("NPC");
                    } else g.gameObject.layer = LayerMask.NameToLayer("NPC");
                }
                if (g.name.StartsWith("Light"))
                {
                    if (g.GetComponent<Light>())
                        vehicleData.m_Lights.Add(g.GetComponent<Light>());
                    else
                        MelonLogger.Msg("[" + g.name + "] This GameObject doesn't have Light component!");
                }
                if (g.name.StartsWith("Sit")) vehicleData.m_Sits.Add(g);
            }
            if (vehicleData.m_Type <= 1)
            {
                JointSpring aaa = new JointSpring();
                aaa.spring = float.Parse(GetInfo(name, "SpringForce"));
                aaa.damper = float.Parse(GetInfo(name, "DamperForce"));

                foreach (Transform g in vehicleData.m_Wheels)
                {
                    WheelComponent.AddComponent(g);
                    WheelComponent.Set_JointSpring(g, aaa);
                    WheelComponent.Set_Mass(g, m_Rigidbody.mass / 2);
                    WheelComponent.Set_MotorTorque(g, 0);
                    WheelComponent.Set_BrakeTorque(g, 0);
                    if (GetInfo(name, "Radius") != "NaN") WheelComponent.Set_Radius(g, float.Parse(GetInfo(name, "Radius")));
                    if (GetInfo(name, "Center") != "NaN") WheelComponent.Set_Center(g, new Vector3(float.Parse(GetInfo(name, "Center").Split(',')[0]), float.Parse(GetInfo(name, "Center").Split(',')[1]), float.Parse(GetInfo(name, "Center").Split(',')[2])));
                }
            }
            enableds = true;
        }
        public void LateUpdate()
        {
            if (!check()) return;
            vehicleData.m_CurSpeed = m_Rigidbody.velocity.magnitude;
            if (CountPassanger() > 0) foreach (int i in vehicleData.Passangers.Keys) if (SkyCoop.MyMod.players[i]) SkyCoop.MyMod.players[i].SetActive(false); else DeletePassanger(i);
            EngineSoundAndLight();
            SendData();
            if (vehicleData.m_isDrive && (main.targetcar != vehicleData.m_OwnerId || !main.allowdrive)) UpdateCarPosition();
        }
        public void Update()
        {
            move = Input.GetAxis("Vertical");
            turn = Input.GetAxis("Horizontal");
            if (!check()) return;
            if (vehicleData.m_Type <= 1)
            {
                float MotorTorque = 80 * vehicleData.m_MotorTorque * Time.fixedDeltaTime;

                foreach (var wheelpost in vehicleData.m_Wheels)
                {
                    if (wheelpost.GetChild(0))
                    {
                        Transform Wheel_BodyTarget = wheelpost.GetChild(0);
                        WheelComponent.Get_WorldPose(wheelpost, out Vector3 pos, out Quaternion rot);
                        if (vehicleData.m_Type == 0) Wheel_BodyTarget.position = pos;
                        Wheel_BodyTarget.rotation = rot;
                    }
                    if (!vehicleData.m_isDrive)
                    {
                        WheelComponent.Set_BrakeTorque(wheelpost, MotorTorque * 3);
                    }
                    if (vehicleData.m_isDrive && main.targetcar == vehicleData.m_OwnerId && main.allowdrive)
                    {
                        WheelComponent.Set_MotorTorque(wheelpost, MotorTorque * move);
                        if (move == 0) WheelComponent.Set_BrakeTorque(wheelpost, MotorTorque * 3);
                        else WheelComponent.Set_BrakeTorque(wheelpost, 0);
                        if (vehicleData.m_Wheels_main.Contains(wheelpost)) WheelComponent.Set_SteerAngle(wheelpost, Mathf.Clamp(vehicleData.m_CurSpeed * turn + turn * 10, -45, 45));
                        if(vehicleData.m_Type == 1) transform.rotation = Quaternion.LookRotation(transform.forward);
                    }
                }
            }
        }
        private void EngineSoundAndLight() // SOUNDS FOR VEHICLE
        {
            if (!check()) return;
            if (!vehicleData.m_SoundPlay && vehicleData.m_audio.isPlaying) vehicleData.m_audio.Stop();
            if (!vehicleData.m_audio.isPlaying && vehicleData.m_SoundPlay) vehicleData.m_audio.Play();
            if (CountPassanger() == 0) vehicleData.m_SoundPlay = false;
            foreach (var i in vehicleData.m_Lights) i.enabled = vehicleData.m_Light;


            if (vehicleData.m_CurSpeed == 0) vehicleData.m_CurSpeed = 1;
            vehicleData.m_audio.volume = calkrange();
            vehicleData.m_audio.pitch = (0.30f + vehicleData.m_CurSpeed * 0.025f);
            if (vehicleData.m_CurSpeed > 30) vehicleData.m_audio.pitch = (0.25f + vehicleData.m_CurSpeed * 0.015f);
            if (vehicleData.m_CurSpeed > 40) vehicleData.m_audio.pitch = (0.20f + vehicleData.m_CurSpeed * 0.013f);
            if (vehicleData.m_CurSpeed > 49) vehicleData.m_audio.pitch = (0.15f + vehicleData.m_CurSpeed * 0.011f);
            if (vehicleData.m_audio.pitch > 2.0f) vehicleData.m_audio.pitch = 2.0f;
        }
        private float calkrange() //CALCULATE VOLUME DEPENDS ON RANGE -> P-C
        {
            float i = Vector3.Distance(transform.position, main.MyPosition.position);
            i = i / 100;
            i = 1 - i;
            Mathf.Clamp(i, 0, 1);
            return i;
        }
        public void AddPassanger(int from, int where)
        {
            if (!main.hook) return;
            Transform sit = transform.Find("SITS");
            GameObject newob = GameObject.Instantiate(SkyCoop.MyMod.players[from], sit);
            Transform sitpos = vehicleData.m_Sits[where];
            newob.transform.position = sitpos.position;
            newob.transform.rotation = sitpos.rotation;
            newob.name = from.ToString();
            newob.SetActive(true);

            vehicleData.Passangers.Add(from, where);
            newob.AddComponent<IKVH>().Init(vehicleData.m_OwnerId, where);
        }
        public void DeletePassanger(int from)
        {
            if (!main.hook) return;
            GameObject.Destroy(transform.Find("SITS/"+from).gameObject);
            vehicleData.Passangers.Remove(from);
        }
        public void UpdateDriver(bool state)
        {
            vehicleData.m_isDrive = state;
        }
        public bool isDrive()
        {
            if (vehicleData.m_isDrive) return false;
            if (!vehicleData.m_AllowDrive) return false;
            if (!vehicleData.m_isDrive && vehicleData.m_AllowDrive) return true;
            else return false;
        }
        public int CountPassanger()
        {
            return vehicleData.Passangers.Count;
        }
        public void UpdateMainCarData(int PlayerId, bool allowdrive, bool allowsit, bool isDrive, bool sound, bool light, float fuel, string playername)
        {
            vehicleData.m_OwnerId = PlayerId;
            vehicleData.m_OwnerName = playername;
            vehicleData.m_AllowDrive = allowdrive;
            vehicleData.m_AllowSit = allowsit;
            vehicleData.m_CurFuel = fuel;
            vehicleData.m_isDrive = isDrive;
            vehicleData.m_SoundPlay = sound;
            vehicleData.m_Light = light;
        }
        public void UpdateLight(bool state)
        {
            vehicleData.m_Light = state;
            NETHost.NetLight(vehicleData.m_OwnerId, state);
        }
        public void UpdateSound(bool state)
        {
            vehicleData.m_SoundPlay = state;
                NETHost.NetSound(vehicleData.m_OwnerId, state);
        }
        private void SendData()
        {
                if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.HOST)
                {
                    NETHost.NetSendDriver(vehicleData.m_OwnerId, vehicleData.m_isDrive);
                    NETHost.NetLight(vehicleData.m_OwnerId, vehicleData.m_Light);
                    NETHost.NetSound(vehicleData.m_OwnerId, vehicleData.m_SoundPlay);
                    NETHost.NETSEND(vehicleData.m_OwnerId, vehicleData.m_VehicleName);
                }
                if(vehicleData.m_isDrive && (main.targetcar != vehicleData.m_OwnerId || !main.allowdrive)) NETHost.NetCar(vehicleData.m_OwnerId, vehicleData.m_Position, vehicleData.m_Rotation);
        }
        public void UpdateCarPosition()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, vehicleData.m_Rotation, 2f);
            transform.position = Vector3.Lerp(transform.position, vehicleData.m_Position, 15);
            if (Vector3.Distance(transform.position, vehicleData.m_Position) > 10) transform.position = vehicleData.m_Position;
        }
        private bool check()
        {
            if (vehicleData == null || !enableds || !m_Rigidbody) return false;
            else return true;
        }
    }
}
