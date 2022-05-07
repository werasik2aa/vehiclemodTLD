using UnityEngine;
using GameServer;
using static vehiclemod.data;

namespace vehiclemod
{
    [HarmonyLib.HarmonyPatch(typeof(SkyCoop.API), "CustomEventCallback")]
    public static class SkyCoop_HandleData
    {
        static void Postfix(SkyCoop.API __instance, Packet _pak, int from)
        {
            int packetid = _pak.ReadInt();
            if (packetid == 0110) // CHECK EXIST CAR
            {
                int CarID = _pak.ReadInt();
                string carName = _pak.ReadString();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && !main.vehicles.ContainsKey(CarID)) main.SpawnCar(CarID, main.MyId, carName, Vector3.one, Quaternion.identity);
            }
            if 
                (packetid == 0000) // MOVE CAR
            {
                int ID = _pak.ReadInt();
                Vector3 Position = _pak.ReadVector3();
                Quaternion Rotation = _pak.ReadQuaternion();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID))
                {
                    InfoMain i = GetObj(ID).GetComponent<VehComponent>().vehicleData;
                    i.m_Position = Position; i.m_Rotation = Rotation;
                    GetObj(ID).GetComponent<VehComponent>().UpdateCarPosition();
                }
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
            if (packetid == 1100) // SEND Sound ON
            {
                int ID = _pak.ReadInt();
                bool state = _pak.ReadBool();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) GetObj(ID).GetComponent<VehComponent>().UpdateSound(state);
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
                bool state = _pak.ReadBool();
                if (from == -1 && SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                {
                    from = _pak.ReadInt();
                }
                if (CheckEnv(from) && main.vehicles.ContainsKey(ID)) GetObj(ID).GetComponent<VehComponent>().UpdateLight(state);
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
