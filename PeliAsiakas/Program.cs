using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace PeliAsiakas
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 9999);

            EndPoint Pep = (IPEndPoint)ep;

            Laheta(palvelin, Pep, "JOIN asiakas");

            Boolean on = true;
            String TILA = "JOIN";

            while (on)
            {
                String[] palat = Vastaanota(palvelin);

                switch (TILA)
                {
                    case "JOIN":
                        switch (palat[0])
                        {
                            case "ACK":
                                switch (palat[1])
                                {
                                    
                                    case "201":
                                        Console.WriteLine("Odotetaan toista pelaajaa");
                                        break;
                                    case "202":
                                        Console.WriteLine("Vastustajasi on: " + palat[2]);
                                        Console.WriteLine("Anna numero: ");
                                        String luku = Console.ReadLine();
                                        Laheta(palvelin, Pep, "DATA " + luku);
                                        TILA = "GAME";
                                        break;
                                    case "203":
                                        Console.WriteLine("Vastustaja saa aloittaa ");
                                        TILA = "GAME";
                                        break;
                                    default:
                                        Console.WriteLine("Virhe: " + palat[0]);
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine("Virhe: " + palat[0]);
                                break;
                        }
                        break;
                    case "GAME":
                        switch (palat[0])
                        {
                            case "ACK":
                                switch (palat[1])
                                {
                                    case "500":
                                        break;
                                    case "407":
                                        Console.WriteLine("Arvauksen täytyy olla numero!");
                                        //palvelin ei osaa jatkaa peliä, joten:
                                        Console.WriteLine("Peli päättyi.");
                                        break;
                                    case "300":
                                        Console.WriteLine("Odotetaan vastustajan arvausta...");
                                        break;
                                    default:
                                        Console.WriteLine("Virhe: " + palat[0]);
                                        break;
                                }
                                break;
                            
                            case "DATA":
                                Console.WriteLine("Anna numero: ");
                                String luku = Console.ReadLine();
                                Laheta(palvelin, Pep, "ACK 300");
                                Laheta(palvelin, Pep, "DATA " + luku);
                                break;
                            
                            case "QUIT":
                                switch (palat[1])
                                {
                                    case "501":
                                        Console.WriteLine("Voitit pelin!");
                                        Console.WriteLine("Paina jotain näppäintä.");
                                        Console.ReadKey();
                                        Laheta(palvelin, Pep, "ACK 500");
                                        Console.WriteLine("Peli päättyi.");
                                        break;
                                    case "502":
                                        Console.WriteLine("Hävisit pelin!");
                                        Console.WriteLine("Paina jotain näppäintä.");
                                        Console.ReadKey();
                                        Laheta(palvelin, Pep, "ACK 500");
                                        Console.WriteLine("Peli päättyi.");
                                        break;
                                    default:
                                        Console.WriteLine("Virhe: " + palat[0]);
                                        break;
                                }
                                break;

                            default:
                                Console.WriteLine("Virhe: " + palat[1]);
                                break;
                        }
                        break;
                }
            }
        }

        private static void Laheta(Socket s, EndPoint ep, String msg)
        {
            s.SendTo(Encoding.ASCII.GetBytes(msg), ep);
        }

        private static String[] Vastaanota(Socket s)
        {   
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Palvelinep = (EndPoint)remote;
            
            int paljon = 0;
            
            byte[] rec = new byte[256];

            paljon = s.ReceiveFrom(rec, ref Palvelinep);

            String rec_string = Encoding.ASCII.GetString(rec, 0, paljon);
            char[] delin = { ' ' };
            String[] palat = rec_string.Split(delin);

            if (palat.Length < 2) Console.WriteLine("Vastaanotettiin virheellinen määrä tekstiä");
            
            return palat;
        }
    }
}
