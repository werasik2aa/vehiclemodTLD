using GameServer;
using UnityEngine;

namespace vehiclemod
{
    public static class NETHost
    {
        private static Packet packet;
        public static void NETSEND(int CarID, string carName)
        {
            if (!CHECK()) return;
            packet.Write(0110);
            packet.Write(CarID);
            packet.Write(carName);
            Send(packet);
        }
        public static void NETSIT(int CarID, int SitID)
        {
            if(!CHECK()) return;
            packet.Write(1111);
            packet.Write(CarID);
            packet.Write(SitID);
            Send(packet);
        }
        public static void NetDeleteCar()
        {
            if(!CHECK()) return;
            packet.Write(0100);
            packet.Write(1);
            Send(packet);
        }
        public static void NetSendDriver(int ID, bool drived)
        {
            if(!CHECK()) return;
            packet.Write(1010);
            packet.Write(ID);
            packet.Write(drived);
            Send(packet);
        }

        public static void NetCar(int CarID, Vector3 Position, Quaternion Rotation)
        {
            if(!CHECK()) return;
            packet.Write(0000);
            packet.Write(CarID);
            packet.Write(Position);
            packet.Write(Rotation);
            Send(packet);
        }
        public static void NetSpawnCar(string name, Vector3 Position, Quaternion Rotation)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE || SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.HOST) return;
            packet.Write(1000);
            packet.Write(name);
            packet.Write(Position);
            packet.Write(Rotation);
            Send(packet);
        }
        public static void NetSound(int ID, bool state)
        {
            if(!CHECK()) return;
            packet.Write(1100);
            packet.Write(ID);
            packet.Write(state);
            Send(packet);
        }
        public static void NetLight(int ID, bool state)
        {
            if(!CHECK()) return;
            packet.Write(1101);
            packet.Write(ID);
            packet.Write(state);
            Send(packet);
        }
        private static bool CHECK()
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE || !main.hook) return false;
            packet = new Packet((int)ClientPackets.CUSTOM);
            return true;
        }
        private static void Send(Packet packet)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.HOST)
                SkyCoop.API.SendDataToEveryone(packet, SkyCoop.API.m_MyClientID, true); //Send data to everyone. Host Sender

            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.CLIENT)
                SkyCoop.API.SendToHost(packet); //Send data to everyone. Client sender
        }
    }
}
