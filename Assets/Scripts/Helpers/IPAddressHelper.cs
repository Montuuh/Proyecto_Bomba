using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public static class IPAddressHelper
{
    public static string GetLocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    // Enconde ipaddress to characters between A and Z
    public static string EncodeIPAddress(string ip)
    {
        string encodedIP = "";
        string[] parts = ip.Split('.');
        ip = parts[2..][0] + "." + parts[2..][1];

        foreach (char c in ip)
        {
            if (c == '1')
                encodedIP += 'A';
            else if (c == '2')
                encodedIP += 'B';
            else if (c == '3')
                encodedIP += 'C';
            else if (c == '4')
                encodedIP += 'D';
            else if (c == '5')
                encodedIP += 'E';
            else if (c == '6')
                encodedIP += 'F';
            else if (c == '7')
                encodedIP += 'G';
            else if (c == '8')
                encodedIP += 'H';
            else if (c == '9')
                encodedIP += 'I';
            else if (c == '0')
                encodedIP += 'J';
            else if (c == '.')
                encodedIP += 'Z';
        }
        return encodedIP;
    }

    // Decode ipaddress
    public static string DecodeIPAddress(string ip)
    {
        string decodedIP = "";
        foreach (char c in ip)
        {
            if (c == 'A')
                decodedIP += '1';
            else if (c == 'B')
                decodedIP += '2';
            else if (c == 'C')
                decodedIP += '3';
            else if (c == 'D')
                decodedIP += '4';
            else if (c == 'E')
                decodedIP += '5';
            else if (c == 'F')
                decodedIP += '6';
            else if (c == 'G')
                decodedIP += '7';
            else if (c == 'H')
                decodedIP += '8';
            else if (c == 'I')
                decodedIP += '9';
            else if (c == 'J')
                decodedIP += '0';
            else if (c == 'Z')
                decodedIP += '.';
        }

        return "192.168." + decodedIP;
    }

    // Server code validator
    public static bool IsValidServerCode(string code)
    {
        //if (code.Length != 6)
        //    return false;

        foreach (char c in code)
        {
            if (c != 'A' && c != 'B' && c != 'C' && c != 'D' && c != 'E' && c != 'F' && c != 'G' && c != 'H' && c != 'I' && c != 'J' && c != 'Z')
                return false;
        }
        return true;
    }
}
