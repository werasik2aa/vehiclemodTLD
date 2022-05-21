using UnityEngine;
using GameServer;
using static vehiclemod.data;
using MelonLoader;

namespace vehiclemod
{
    [HarmonyLib.HarmonyPatch(typeof(SkyCoop.API), "CustomEventCallback")]
    public static class SkyCoop_HandleData
    {
        public static void Postfix(SkyCoop.API __instance, Packet _pak, int from)
        {
            int packetid = _pak.ReadInt(); // WHAT TO GET
            int ID = _pak.ReadInt();
            if (packetid == 0000) // MOVE CAR
            {
                Vector3 Position = _pak.ReadVector3();
                Quaternion Rotation = _pak.ReadQuaternion();

                if(from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) UpdateCar(ID, Position, Rotation);
            }
            if (packetid == 0100) // DELETE CAR
            {
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) if(main.deletecar(ID)) MelonLogger.Msg("Deleted NET Vehicle: "+ID);
            }
            if (packetid == 1000) // SPAWN CAR
            {
                string name = _pak.ReadString();
                Vector3 Position = _pak.ReadVector3();
                Quaternion Rotation = _pak.ReadQuaternion();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && !main.vehicles.ContainsKey(ID)) main.SpawnCar(ID, SkyCoop.MyMod.playersData[from].m_Levelid, name, Position, Rotation);
            }
            if (packetid == 1100) // SEND Sound
            {
                bool state = _pak.ReadBool();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) UpdateSound(ID, state);
            }
            if (packetid == 1111) // PASSANGER
            {
                int SitID = _pak.ReadInt();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) UpdatePassanger(ID, SitID, from);
            }
            if (packetid == 1101) // Turn LIGHT
            {
                bool state = _pak.ReadBool();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) UpdateLight(ID, state);
            }
            if (packetid == 1010) // SEND Driver
            {
                bool state = _pak.ReadBool();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from)) UpdateDriver(ID, state);
            }
        }
        private static bool CheckEnv(int from)
        {
            if (from == main.MyId || main.levelid != SkyCoop.MyMod.playersData[from].m_Levelid) return false;
            return true;
        }
    }
}
