using System.Security.Cryptography;
using System.Text;
using SharpPcap;
using PacketDotNet;
using System.Timers;

namespace PSIP_Project_Adam3;

public class PacketInfo
{
    public string SourceMacAddress { get; set; }
    public int SourcePort { get; set; }
    public int TTL { get; set; }

    public PacketInfo(string sourceMacAddress, int sourcePort, int ttl)
    {
        SourceMacAddress = sourceMacAddress;
        SourcePort = sourcePort;
        TTL = ttl;
    }
}

public class DeviceManager
{
    public static HashSet<string> HashSet = new();
    public static ICaptureDevice device1;
    public static ICaptureDevice device2;
    public static int EthernetPacketCountIN_Left = 0;
    public static int IPPacketCountIN_Left = 0;
    public static int ARPpacketCountIN_Left = 0;
    public static int TCPpacketCountIN_Left = 0;
    public static int UDPpacketCountIN_Left = 0;
    public static int ICMPpacketCountIN_Left = 0;
    public static int HTTPpacketCountIN_Left = 0;
    public static int HTTPSPacketCountIN_Left = 0;

    public static int TotalPacketCountIN_Left = 0;

    //public static int TotalPacketCount = EthernetPacketCount + IPPacketCount+ARPpacketCount + TCPpacketCount + UDPpacketCount + ICMPpacketCount + HTTPpacketCount + HTTPSPacketCount;
    public static int EthernetPacketCountOUT_Left = 0;
    public static int IPPacketCountOUT_Left = 0;
    public static int ARPpacketCountOUT_Left = 0;
    public static int TCPpacketCountOUT_Left = 0;
    public static int UDPpacketCountOUT_Left = 0;
    public static int ICMPpacketCountOUT_Left = 0;
    public static int HTTPpacketCountOUT_Left = 0;
    public static int HTTPSPacketCountOUT_Left = 0;
    public static int TotalPacketCountOUT_Left = 0;

    public static int EthernetPacketCountIN_Right = 0;
    public static int IPPacketCountIN_Right = 0;
    public static int ARPpacketCountIN_Right = 0;
    public static int TCPPacketCountIN_Right = 0;
    public static int UDPPacketCountIN_Right = 0;
    public static int ICMPPacketCountIN_Right = 0;
    public static int HTTPPacketCountIN_Right = 0;
    public static int HTTPSPacketCountIN_Right = 0;
    public static int TotalPacketCountIN_Right = 0;

    public static int EthernetPacketCountOUT_Right = 0;
    public static int IPPacketCountOUT_Right = 0;
    public static int ARPPacketCountOUT_Right = 0;
    public static int TCPPacketCountOUT_Right = 0;
    public static int UDPPacketCountOUT_Right = 0;
    public static int ICMPPacketCountOUT_Right = 0;
    public static int HTTPPacketCountOUT_Right = 0;
    public static int HTTPSPacketCountOUT_Right = 0;
    public static int TotalPacketCountOUT_Right = 0;
    public static Dictionary<string, PacketInfo> packetDictionary = new();
    public static System.Timers.Timer ttl_timer;

    public static void clearMacTable()
    {
        foreach (var entry in packetDictionary) packetDictionary.Remove(entry.Key);
    }

    public static void clearStats()
    {
        EthernetPacketCountIN_Left = 0;
        IPPacketCountIN_Left = 0;
        ARPpacketCountIN_Left = 0;
        TCPpacketCountIN_Left = 0;
        UDPpacketCountIN_Left = 0;
        ICMPpacketCountIN_Left = 0;
        HTTPpacketCountIN_Left = 0;
        HTTPSPacketCountIN_Left = 0;
        TotalPacketCountIN_Left = 0;
        EthernetPacketCountOUT_Left = 0;
        IPPacketCountOUT_Left = 0;
        ARPpacketCountOUT_Left = 0;
        TCPpacketCountOUT_Left = 0;
        UDPpacketCountOUT_Left = 0;
        ICMPpacketCountOUT_Left = 0;
        HTTPpacketCountOUT_Left = 0;
        HTTPSPacketCountOUT_Left = 0;
        TotalPacketCountOUT_Left = 0;
        EthernetPacketCountIN_Right = 0;
        IPPacketCountIN_Right = 0;
        ARPpacketCountIN_Right = 0;
        TCPPacketCountIN_Right = 0;
        UDPPacketCountIN_Right = 0;
        ICMPPacketCountIN_Right = 0;
        HTTPPacketCountIN_Right = 0;
        HTTPSPacketCountIN_Right = 0;
        TotalPacketCountIN_Right = 0;
        EthernetPacketCountOUT_Right = 0;
        IPPacketCountOUT_Right = 0;
        ARPPacketCountOUT_Right = 0;
        TCPPacketCountOUT_Right = 0;
        UDPPacketCountOUT_Right = 0;
        ICMPPacketCountOUT_Right = 0;
        HTTPPacketCountOUT_Right = 0;
        HTTPSPacketCountOUT_Right = 0;
        TotalPacketCountOUT_Right = 0;
    }

    public static void decrementTime()
    {
        ttl_timer = new System.Timers.Timer(1000);
        ttl_timer.Elapsed += decrementTTL;
        ttl_timer.AutoReset = true;
        ttl_timer.Start();
    }

    public static void decrementTTL(object sender, ElapsedEventArgs e)
    {
        List<string> keysToRemove = new List<string>();
        foreach (var entry in packetDictionary)
        {
            entry.Value.TTL -= 1;
            if (entry.Value.TTL == 0) keysToRemove.Add(entry.Key);
        }

        foreach (var entry in keysToRemove)
        {
            Console.WriteLine("Entry outdated. DELETED");
            Console.WriteLine($"{entry}");
            packetDictionary.Remove(entry);
        }

        //Console.WriteLine("Current Packet Dictionary:");
        foreach (var entry in packetDictionary)
            Console.WriteLine(
                $"MAC Hash: {entry.Key}, MAC: {entry.Value.SourceMacAddress}, Port: {entry.Value.SourcePort}, TTL: {entry.Value.TTL}");
    }

    public static void ListDevices()
    {
        var devices = CaptureDeviceList.Instance;
        Console.WriteLine($"Devices: {devices.Count}");

        try
        {
            device1 = devices[4];
            device2 = devices[5];

            if (device1 != null)
            {
                Console.WriteLine($"Opening device1: {device1.Name}");
                device1.Open(DeviceModes.Promiscuous | DeviceModes.NoCaptureLocal);
                device1.OnPacketArrival += Device_OnPacketArrival1;
                device1.StartCapture();
            }

            if (device2 != null)
            {
                Console.WriteLine($"Opening device2: {device2.Name}");
                device2.Open(DeviceModes.Promiscuous | DeviceModes.NoCaptureLocal);
                device2.OnPacketArrival += Device_OnPacketArrival2;
                device2.StartCapture();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing devices: {ex.Message}");
        }
    }


    public static void Device_OnPacketArrival1(object sender, PacketCapture e)
    {
        try
        {
            if (device2 == null || !device2.Started)
            {
                Console.WriteLine("Device2 disconnected. Skipping packet forwarding.");
                return;
            }
            TotalPacketCountIN_Left++;
            TotalPacketCountOUT_Right++;
            var rawPacket = e.GetPacket();
            var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            var dev2 = device2 as IInjectionDevice;
            Task.Run(() =>
            {
                try
                {
                    dev2?.SendPacket(packet);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Error forwarding packet to device2: " );
                }
            });
            // This may block if device2 is disconnected
            count_packets1(packet);
            processPacket(packet, 1);
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error processing packet on device1: {ex.Message}");
        }
    }

    public static void Device_OnPacketArrival2(object sender, PacketCapture e)
    {
        try
        {
            if (device1 == null || !device1.Started)
            {
                Console.WriteLine("Device1 disconnected. Skipping packet forwarding.");
                return;
            }
            TotalPacketCountIN_Right++;
            TotalPacketCountOUT_Left++;

           
            var rawPacket = e.GetPacket();
            var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            var dev1 = device1 as IInjectionDevice;
            Task.Run(() =>
            {
                try
                {
                    dev1?.SendPacket(packet);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Error forwarding packet to device2: " );
                }
            });
           

            count_packets2(packet);
            processPacket(packet, 2);
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error processing packet on device2:");
        }
    }

    public static void decideDestination(Packet p, int incomingPort)
    {
        var packet = p.Extract<EthernetPacket>();
        if (packet != null)
        {
            var destinationMacAddress = packet.DestinationHardwareAddress.ToString();
            var hashedMacAddress = GetMacAddressHash(destinationMacAddress);
            if (packetDictionary.ContainsKey(hashedMacAddress))
            {
                if (packetDictionary[hashedMacAddress].SourcePort == 1)
                {
                    var dev1 = device1 as IInjectionDevice;
                    Task.Run(() =>
                    {
                        try
                        {
                            dev1?.SendPacket(packet);
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine("Error forwarding packet to device2: " );
                        }
                    });
                }

                if (packetDictionary[hashedMacAddress].SourcePort == 2)
                {
                    var dev2 = device2 as IInjectionDevice;
                    Task.Run(() =>
                    {
                        try
                        {
                            dev2?.SendPacket(packet);
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine("Error forwarding packet to device2: " );
                        }
                    });
                }
            }
        }
    }

    public static void processPacket(Packet p, int port)
    {
        var packet = p.Extract<EthernetPacket>();
        if (packet != null && p.Extract<IcmpV4Packet>() != null)
        {
            var sourceMacAddress = packet.SourceHardwareAddress.ToString();
            var hashedMacAddress = GetMacAddressHash(sourceMacAddress);
            if (packetDictionary.ContainsKey(hashedMacAddress))
            {
                packetDictionary[hashedMacAddress].TTL = 15;
                if (packetDictionary[hashedMacAddress].SourcePort != port)
                {
                    Console.WriteLine("New Port value");
                    packetDictionary[hashedMacAddress].SourcePort = port;
                }

                AccessPacketInfo(hashedMacAddress);
                return;
            }

            Console.WriteLine($"Added new mac Address :{sourceMacAddress}");
            packetDictionary.Add(hashedMacAddress, new PacketInfo(sourceMacAddress, port, 15));
            //HashSet.Add(hashedMacAddress);
        }
    }

    public static void AccessPacketInfo(string packetHash)
    {
        if (packetDictionary.ContainsKey(packetHash))
        {
            var packetInfo = packetDictionary[packetHash];
            Console.WriteLine(
                $"MAC Address: {packetInfo.SourceMacAddress}, Port: {packetInfo.SourcePort}, TTL: {packetInfo.TTL}");
        }
        else
        {
            Console.WriteLine("Packet not found in the dictionary.");
        }
    }


    public static string GetMacAddressHash(string macAddress)
    {
        // Ensure the MAC address is in the correct format (e.g., "00-14-22-01-23-45")
        if (string.IsNullOrEmpty(macAddress))
            throw new ArgumentException("MAC address cannot be null or empty.");

        // Remove any non-alphanumeric characters (like dashes, colons) from the MAC address
        macAddress = macAddress.Replace("-", "").Replace(":", "");

        // Create the MD5 hash from the MAC address string
        using (var md5 = MD5.Create())
        {
            var data = Encoding.UTF8.GetBytes(macAddress); // Convert MAC address to byte array
            var hashBytes = md5.ComputeHash(data); // Compute the MD5 hash
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Convert hash to a string
        }
    }

    /*############################## COUNT PACKETS FOR DEVICE 1####################################### */
    public static void count_packets1(Packet packet)
    {
        TotalPacketCountIN_Left++;
        TotalPacketCountOUT_Right++;

        var ethernetPacket = packet.Extract<EthernetPacket>();
        if (ethernetPacket != null)
        {
            EthernetPacketCountIN_Left++;
            EthernetPacketCountOUT_Right++;
        }

        var ipPacket = packet.Extract<IPv4Packet>();
        if (ipPacket != null)
        {
            IPPacketCountIN_Left++;
            IPPacketCountOUT_Right++;
        }

        var arpPacket = packet.Extract<ArpPacket>();
        if (arpPacket != null)
        {
            ARPpacketCountIN_Left++;
            ARPPacketCountOUT_Right++;
        }

        var tcpPacket = packet.Extract<TcpPacket>();
        if (tcpPacket != null)
        {
            TCPpacketCountIN_Left++;
            TCPPacketCountOUT_Right++;
            if (tcpPacket.DestinationPort == 80)
            {
                HTTPpacketCountIN_Left++;
                HTTPPacketCountOUT_Right++;
            }

            if (tcpPacket.DestinationPort == 443)
            {
                HTTPSPacketCountIN_Left++;
                HTTPSPacketCountOUT_Right++;
            }
        }

        var udpPacket = packet.Extract<UdpPacket>();
        if (udpPacket != null)
        {
            UDPpacketCountIN_Left++;
            UDPPacketCountOUT_Right++;
        }

        var icmpPacket = packet.Extract<IcmpV4Packet>();
        if (icmpPacket != null)
        {
            ICMPpacketCountIN_Left++;
            ICMPPacketCountOUT_Right++;
        }
        /*else
        {
         Console.WriteLine("Unrecognized packet");
        }*/
    }

    /*############################## COUNT PACKETS FOR DEVICE 2####################################### */
    public static void count_packets2(Packet packet)
    {
        TotalPacketCountIN_Right++;
        TotalPacketCountOUT_Left++;

        var ethernetPacket = packet.Extract<EthernetPacket>();
        if (ethernetPacket != null)
        {
            EthernetPacketCountIN_Right++;
            EthernetPacketCountOUT_Left++;
        }

        var ipPacket = packet.Extract<IPv4Packet>();
        if (ipPacket != null)
        {
            IPPacketCountIN_Right++;
            IPPacketCountOUT_Left++;
        }

        var arpPacket = packet.Extract<ArpPacket>();
        if (arpPacket != null)
        {
            ARPpacketCountIN_Right++;
            ARPpacketCountOUT_Left++;
        }

        var tcpPacket = packet.Extract<TcpPacket>();
        if (tcpPacket != null)
        {
            TCPPacketCountIN_Right++;
            TCPpacketCountOUT_Left++;
            if (tcpPacket.DestinationPort == 80)
            {
                HTTPPacketCountIN_Right++;
                HTTPpacketCountOUT_Left++;
            }

            if (tcpPacket.DestinationPort == 443)
            {
                HTTPSPacketCountIN_Right++;
                HTTPSPacketCountOUT_Left++;
            }
        }

        var udpPacket = packet.Extract<UdpPacket>();
        if (udpPacket != null)
        {
            UDPPacketCountIN_Right++;
            UDPpacketCountOUT_Left++;
        }

        var icmpPacket = packet.Extract<IcmpV4Packet>();
        if (icmpPacket != null)
        {
            ICMPPacketCountIN_Right++;
            ICMPpacketCountOUT_Left++;
        }
        /*else
        {
         Console.WriteLine("Unrecognized packet");
        }*/
    }
}

/* public static void MonitorDevices()
 {
  Task.Run(async () =>
  {
   while (true)
   {
    await Task.Delay(1000);
    try
    {
     if (device1 != null && !device1.Started)
     {
      Console.WriteLine("Reconnecting to device 1");
      device1.Open(DeviceModes.Promiscuous | DeviceModes.NoCaptureLocal);
      device1.StartCapture();
     }

     if (device2 != null && !device2.Started)
     {
      Console.WriteLine("Reconecting to device 2");
      device2.Open(DeviceModes.Promiscuous | DeviceModes.NoCaptureLocal);
      device2.StartCapture();
     }
    }
    catch (Exception ex)
    {
     Console.WriteLine("Error reconnecting the device");
    }
   }
  });
 } */