using UnityEngine;
using GameServer;
using SkyCoop;

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
                string name = _pak.ReadString();
                int ID = _pak.ReadInt();
                Vector3 Position = _pak.ReadVector3();
                Quaternion Rotation = _pak.ReadQuaternion();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (API.m_ClientState == API.SkyCoopClientState.HOST) API.SendDataToEveryone(_pak, from, true);
                if (CheckEnv(from)) VehicleController.PlayerCarMove(ID, name, Position, Rotation);
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
                if (API.m_ClientState == API.SkyCoopClientState.HOST) API.SendDataToEveryone(_pak, from, true);
                if (CheckEnv(from)) main.SpawnCar(from, SkyCoop.MyMod.playersData[from].m_Levelid, name, Position, Rotation);
            }
            if (packetid == 0011) // UPDATE CAR DATA
            {
                bool allowdrive = _pak.ReadBool();
                bool allowsit = _pak.ReadBool();
                float fuel = _pak.ReadFloat();
                string name = _pak.ReadString();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (API.m_ClientState == API.SkyCoopClientState.HOST) API.SendDataToEveryone(_pak, from, true);
                if (CheckEnv(from)) data.UpdateCarData(from, allowdrive, allowsit, fuel, name);
            }
            if (packetid == 1010) // SEND Driver
            {
                int ID = _pak.ReadInt();
                bool drived = _pak.ReadBool();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (API.m_ClientState == API.SkyCoopClientState.HOST) API.SendDataToEveryone(_pak, from, true);
                if (CheckEnv(from)) data.UpdateDriver(ID, drived);
            }
            if (packetid == 1100) // SEND Sound ON
            {
                int curspeed = _pak.ReadInt();
                int ID = _pak.ReadInt();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (API.m_ClientState == API.SkyCoopClientState.HOST) API.SendDataToEveryone(_pak, from, true);
                if (CheckEnv(from)) VehicleController.engineSound(curspeed, ID);
            }
            if (packetid == 1110) // SEND Sound OFF
            {
                int ID = _pak.ReadInt();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (API.m_ClientState == API.SkyCoopClientState.HOST) API.SendDataToEveryone(_pak, from, true);
                if (CheckEnv(from)) VehicleController.engineSoundOff(ID);
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
