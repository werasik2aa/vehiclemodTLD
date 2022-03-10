using GameServer;
using UnityEngine;

namespace vehiclemod
{
    public static class NETHost
    {
        public static void NetPacketStat(bool allowdrive, bool allowsit, float fuel, string name)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(0011);
            packet.Write(allowdrive);
            packet.Write(allowsit);
            packet.Write(Mathf.Round(fuel));
            packet.Write(name);
            Send(packet);
        }
        public static void NetSendMyCar(string CarName, Vector3 pos, Quaternion rot)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(1011);
            packet.Write(CarName);
            packet.Write(pos);
            packet.Write(rot);
            Send(packet);
        }
        public static void NETSIT(int CarID, int SitID)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(1111);
            packet.Write(CarID);
            packet.Write(SitID);
            Send(packet);
        }
        public static void NetDeleteCar()
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(0100);
            packet.Write(1);

            Send(packet);
        }
        public static void NetSendDriver(int ID, bool drived)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(1010);
            packet.Write(ID);
            packet.Write(drived);

            Send(packet);
        }

        public static void NetCar(int CarID, string CarName, Vector3 Position, Quaternion Rotation)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(0000);
            packet.Write(CarName);
            packet.Write(CarID);
            packet.Write(Position);
            packet.Write(Rotation);

            Send(packet);
        }
        public static void NetSpawnCar(string name, Vector3 Position, Quaternion Rotation)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(1000);
            packet.Write(name);
            packet.Write(Position);
            packet.Write(Rotation);

            Send(packet);
        }
        public static void NetSoundOn(int curspeed, int ID)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(1100);
            packet.Write(curspeed);
            packet.Write(ID);

            Send(packet);
        }
        public static void NetSoundOff(int ID)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(1110);
            packet.Write(ID);

            Send(packet);
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
