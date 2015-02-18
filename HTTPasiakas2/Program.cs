/**
 * Yksinkertainen TCP-asiakas, joka osaa lähettää viestin palvelimelle 
 * ja ottaa vastaan viestin palvelimelta.
 * @author: Roope Kivioja
 * @date: 08.09.2014
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace TCPasiakas
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            s.Connect("localhost", 25000);

            String snd = "kissa istuu puussa\r\n";
            
            byte[] buffer = Encoding.ASCII.GetBytes(snd);

            s.Send(buffer);

            String viesti = "";
            int count = 0;

            do
            {
                byte[] rec = new byte[1024];

                count = s.Receive(rec);
                Console.Write("Tavuja vastaanotettu " + count + "\r\n");

                viesti += Encoding.ASCII.GetString(rec, 0, count);
            } while (count > 0);
      
            int merkki = viesti.LastIndexOf(";");
            
            String nimi = viesti.Remove(merkki);
            String teksti = viesti.Substring(merkki+1);

            Console.WriteLine("palvelin: " + nimi);
            Console.WriteLine("teksti: " + teksti);

            Console.ReadKey();
            s.Close();
        }
    }
}