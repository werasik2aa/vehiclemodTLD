using GameServer;
using UnityEngine;

namespace vehiclemod
{
    public static class NETHost
    {
        public static void NetPacketStat(int carId, bool allowdrive, bool allowsit, bool isdrive, bool sound, bool light, float fuel, string name)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE || SkyCoop.API.m_ClientState != SkyCoop.API.SkyCoopClientState.HOST) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(0011);
            packet.Write(carId);
            packet.Write(allowdrive);
            packet.Write(allowsit);
            packet.Write(isdrive);
            packet.Write(sound);
            packet.Write(light);
            packet.Write(Mathf.Round(fuel));
            packet.Write(name);
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

        public static void NetCar(int CarID, Vector3 Position, Quaternion Rotation)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(0000);
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
        public static void NetSound(int ID, bool state)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(1100);
            packet.Write(ID);
            packet.Write(state);

            Send(packet);
        }
        public static void NetLight(int ID, bool state)
        {
            if (SkyCoop.API.m_ClientState == SkyCoop.API.SkyCoopClientState.NONE) return;
            Packet packet = packet = new Packet((int)ClientPackets.CUSTOM);

            packet.Write(1101);
            packet.Write(ID);
            packet.Write(state);

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
