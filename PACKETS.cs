using UnityEngine;
using GameServer;
using SkyCoop;
using static vehiclemod.data;

namespace vehiclemod
{
    [HarmonyLib.HarmonyPatch(typeof(SkyCoop.API), "CustomEventCallback")]
    public static class SkyCoop_HandleData
    {
        static void Postfix(SkyCoop.API __instance, Packet _pak, int from)
        {
            int packetid = _pak.ReadInt();
            if (packetid == 0000) // MOVE CAR
            {
                int ID = _pak.ReadInt();
                Vector3 Position = _pak.ReadVector3();
                Quaternion Rotation = _pak.ReadQuaternion();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) VehicleController.PlayerCarMove(ID, Position, Rotation);
            }

            if (packetid == 1000) // SPAWN CAR
            {
                string name = _pak.ReadString();
                Vector3 Position = _pak.ReadVector3();
                Quaternion Rotation = _pak.ReadQuaternion();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from)) main.SpawnCar(from, SkyCoop.MyMod.playersData[from].m_Levelid, name, Position, Rotation);
            }
            if (packetid == 0011) // UPDATE CAR DATA
            {
                int carid = _pak.ReadInt();
                bool allowdrive = _pak.ReadBool();
                bool allowsit = _pak.ReadBool();
                bool isdrive = _pak.ReadBool();
                bool sound = _pak.ReadBool();
                bool light = _pak.ReadBool();
                float fuel = _pak.ReadFloat();
                string name = _pak.ReadString();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                string plname = SkyCoop.MyMod.playersData[from].m_Name;
                if (CheckEnv(from)) UpdateCarData(carid, name, allowdrive, allowsit, isdrive, sound, light, fuel, plname);
            }
            if (packetid == 1100) // SEND Sound ON
            {
                int ID = _pak.ReadInt();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) GetObj(ID).GetComponent<VehComponent>().UpdateSound();
            }
            if (packetid == 1111) // PASSANGER
            {
                int ID = _pak.ReadInt();
                int SitID = _pak.ReadInt();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) UpdatePassanger(ID, SitID, from);
            }
            if (packetid == 1101) // Turn LIGHT
            {
                int ID = _pak.ReadInt();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) GetObj(ID).GetComponent<VehComponent>().UpdateLight();
            }
            if (packetid == 1010) // SEND Driver
            {
                int ID = _pak.ReadInt();
                bool drived = _pak.ReadBool();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) UpdateDriver(ID, drived);
            }
        }
        private static bool CheckEnv(int from)
        {
            if (from == main.MyId || main.levelid != SkyCoop.MyMod.playersData[from].m_Levelid)
                return false;
            else
                return true;
        }
    }
}
