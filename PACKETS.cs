using UnityEngine;
using GameServer;
using static vehiclemod.data;

namespace vehiclemod
{
    [HarmonyLib.HarmonyPatch(typeof(SkyCoop.API), "CustomEventCallback")]
    public static class SkyCoop_HandleData
    {
        public static void Postfix(SkyCoop.API __instance, Packet _pak, int from)
        {
            int packetid = _pak.ReadInt(); // WHAT TO GET
            if (packetid == 0000) // MOVE CAR
            {
                int ID = _pak.ReadInt();
                Vector3 Position = _pak.ReadVector3();
                Quaternion Rotation = _pak.ReadQuaternion();

                if(from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && checkmy(ID)) GetObj(ID).GetComponent<VehComponent>().MoveIt(Position, Rotation);
            }
            if (packetid == 0100) // DELETE CAR
            {
                int ID = _pak.ReadInt();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && checkmy(ID)) { GameObject.Destroy(GetObj(ID)); main.vehicles.Remove(ID); }
            }
            if (packetid == 1000) // SPAWN CAR
            {
                int i = _pak.ReadInt();
                string name = _pak.ReadString();
                Vector3 Position = _pak.ReadVector3();
                Quaternion Rotation = _pak.ReadQuaternion();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && !main.vehicles.ContainsKey(i)) main.SpawnCar(i, SkyCoop.MyMod.playersData[from].m_Levelid, name, Position, Rotation);
            }
            if (packetid == 1100) // SEND Sound ON
            {
                int ID = _pak.ReadInt();
                bool lig = _pak.ReadBool();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && checkmy(ID)) GetObj(ID).GetComponent<VehComponent>().UpdateSound(lig);
            }
            if (packetid == 1111) // PASSANGER
            {
                int ID = _pak.ReadInt();
                int SitID = _pak.ReadInt();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && checkmy(ID)) UpdatePassanger(ID, SitID, from);
            }
            if (packetid == 1101) // Turn LIGHT
            {
                int ID = _pak.ReadInt();
                bool state = _pak.ReadBool();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from) && checkmy(ID)) GetObj(ID).GetComponent<VehComponent>().UpdateLight(state);
            }
            if (packetid == 1010) // SEND Driver
            {
                int ID = _pak.ReadInt();
                bool drived = _pak.ReadBool();

                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT) from = _pak.ReadInt();
                if (CheckEnv(from)) UpdateDriver(ID, drived);
            }
        }
        private static bool CheckEnv(int from)
        {
            if (from == main.MyId || main.levelid != SkyCoop.MyMod.playersData[from].m_Levelid) return false;
            return true;
        }
        private static bool checkmy(int ID)
        {
            if (main.vehicles.ContainsKey(ID) && !(main.allowdrive && main.targetcar == GetObj(ID).GetComponent<VehComponent>().vehicleData.m_OwnerId)) return true;
            return false;
        }
    }
}
