/**
 * Yksinkertainen TCP-palvelin, joka kaiuttaa asiakkaan lähettämän viestin takaisin.
 * @author: Roope Kivioja
 * @date: 08.09.2014
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TCPpalvelin
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket Palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 25000);

            Palvelin.Bind(iep);

            Palvelin.Listen(5);
            
            Socket Asiakas = Palvelin.Accept();

            IPEndPoint iap = (IPEndPoint)Asiakas.RemoteEndPoint;

            Console.WriteLine("Yhteys osoitteesta: {0} portista {1}", iap.Address, iap.Port);

            NetworkStream ns = new NetworkStream(Asiakas);

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            String rec = sr.ReadLine();

            sw.WriteLine("Roopen palvelin;" + rec);
            sw.Flush();
            
            Asiakas.Close();
            
            Console.ReadKey();

            Palvelin.Close();
        }
    }
}
