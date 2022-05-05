using UnityEngine;
using System;
using BringBackComponents;
using System.Collections.Generic;
using MelonLoader;
using static vehiclemod.data;
namespace vehiclemod
{
    public class InfoMain
    {
        public Transform m_VehicleMain = null;
        public List<Transform> m_Wheels = new List<Transform>();
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
        public AudioSource m_audio;
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
                    vehicleData.m_Wheels.Add(g);
                    g.gameObject.layer = LayerMask.NameToLayer("Player");
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
            if (vehicleData.m_Type == 0)
            {
                JointSpring aaa = new JointSpring();
                aaa.spring = float.Parse(GetInfo(name, "SpringForce"));
                aaa.damper = float.Parse(GetInfo(name, "DamperForce"));

                foreach (Transform g in vehicleData.m_Wheels)
                {
                    WheelComponent.AddComponent(g);
                    WheelComponent.Set_JointSpring(g, aaa);
                    WheelComponent.Set_Mass(g, m_Rigidbody.mass/2);
                }
            }
            enableds = true;
        }
        public void LateUpdate()
        {
            if (vehicleData == null || !enableds || !m_Rigidbody) return;
            vehicleData.m_CurSpeed = m_Rigidbody.velocity.magnitude;
            if (CountPassanger() > 0) foreach (int i in vehicleData.Passangers.Keys) SkyCoop.MyMod.players[i].SetActive(false);
            EngineSoundAndLight();
            NETHost.NetPacketStat(vehicleData.m_OwnerId, vehicleData.m_AllowDrive, vehicleData.m_AllowSit, vehicleData.m_isDrive, vehicleData.m_SoundPlay, vehicleData.m_Light, vehicleData.m_CurFuel, vehicleData.m_VehicleName);
        }
        public void FixedUpdate()
        {
            if (vehicleData.m_Type == 0)
            {
                foreach (var wheelpost in vehicleData.m_Wheels)
                {
                    if (!wheelpost || !wheelpost.GetChild(0)) return;
                    Transform Wheel_BodyTarget = wheelpost.GetChild(0);
                    WheelComponent.Get_WorldPose(wheelpost, out Vector3 pos, out Quaternion rot);
                    Wheel_BodyTarget.position = pos;
                    Wheel_BodyTarget.rotation = rot;
                }
            }
        }
        public void Update()
        {
            move = Input.GetAxis("Vertical");
            turn = Input.GetAxis("Horizontal");

            if (vehicleData == null || !enableds || !m_Rigidbody) return;
            if (vehicleData.m_Type == 0)
            {
                float MotorTorque = 100 * vehicleData.m_MotorTorque * Time.fixedDeltaTime;
                if (m_Rigidbody.velocity.magnitude <= 0.05) MotorTorque = 100 * vehicleData.m_MotorTorque * Time.fixedDeltaTime * vehicleData.m_CurSpeed;

                for (int i = 0; i != vehicleData.m_Wheels.Count; i++)
                {
                    if (CountPassanger() == 0 || !vehicleData.m_isDrive) WheelComponent.Set_BrakeTorque(vehicleData.m_Wheels[i], MotorTorque * 3);
                    if (vehicleData.m_isDrive && main.targetcar == vehicleData.m_OwnerId && main.allowdrive)
                    {
                        if (move != 0 && !Input.GetKey(KeyCode.Space))
                        {
                            WheelComponent.Set_BrakeTorque(vehicleData.m_Wheels[i], 0);
                            WheelComponent.Set_MotorTorque(vehicleData.m_Wheels[i], MotorTorque * move);
                        }
                        else
                        {
                            WheelComponent.Set_MotorTorque(vehicleData.m_Wheels[i], 0);
                            WheelComponent.Set_BrakeTorque(vehicleData.m_Wheels[i], MotorTorque * 3);
                        }
                        if (vehicleData.m_Wheels[i].name.StartsWith("WheelMain")) WheelComponent.Set_SteerAngle(vehicleData.m_Wheels[i], Mathf.Clamp(vehicleData.m_CurSpeed * turn + turn * 10, -45, 45));
                    }
                }
            }
        }
        private void EngineSoundAndLight() // SOUNDS FOR VEHICLE
        {
            if (!vehicleData.m_audio) return;

            if (!vehicleData.m_SoundPlay && vehicleData.m_audio.isPlaying) vehicleData.m_audio.Stop();
            if (!vehicleData.m_audio.isPlaying && vehicleData.m_SoundPlay) vehicleData.m_audio.Play();
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
            GameObject newob = GameObject.Instantiate(SkyCoop.MyMod.players[from]);
            Transform sit = vehicleData.m_Sits[where];

            newob.transform.SetParent(sit);
            newob.transform.position = sit.position;
            newob.transform.rotation.SetLookRotation(sit.forward, sit.up);
            newob.SetActive(true);
            newob.name = from.ToString();

            vehicleData.Passangers.Add(from, where);
        }
        public void DeletePassanger(int from)
        {
            vehicleData.Passangers.TryGetValue(from, out int sit);
            GameObject.Destroy(vehicleData.m_Sits[sit].GetChild(0).gameObject);
            vehicleData.Passangers.Remove(from);
        }
        public void UpdateDriver(bool state)
        {
            vehicleData.m_isDrive = state;
        }
        public bool isDrive()
        {
            return vehicleData.m_isDrive & vehicleData.m_AllowDrive;
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
        public void UpdateLight()
        {
            vehicleData.m_Light = !vehicleData.m_Light;
        }
        public void UpdateSound()
        {
            vehicleData.m_SoundPlay = !vehicleData.m_SoundPlay;
        }
    }
}
