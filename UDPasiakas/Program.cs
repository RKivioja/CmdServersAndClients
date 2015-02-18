/**
 * UDP-asiakas UDP-chattipalvelinta varten.
 * @author: Roope Kivioja
 * @date: 08.09.2014
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace UDPasiakas
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            int port = 9999;

            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, port);
            byte[] rec = new byte[256];

            EndPoint ep = (EndPoint)iep;
            s.ReceiveTimeout = 1000;
            String msg;
            Boolean on = true;
            do
            {
                Console.Write(">");
                msg = Console.ReadLine();

                if (msg.Equals('q'))
                {
                    on = false;
                }
                else
                {
                    s.SendTo(Encoding.ASCII.GetBytes(msg), ep);

                    while (!Console.KeyAvailable)
                    {
                        IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint Palvelinep = (EndPoint)remote;
                        int paljon = 0;

                        try
                        {
                            paljon = s.ReceiveFrom(rec, ref Palvelinep);

                            String rec_string = Encoding.ASCII.GetString(rec, 0, paljon);
                            char[] delin = { ';' };
                            String[] palat = rec_string.Split(delin, 2);
                            if (palat.Length != 2)
                            {
                                Console.WriteLine("Virheellinen määrä tekstiä");
                            }
                            else
                            {
                                Console.WriteLine("{0}: {1}", palat[0], palat[1]);
                            }
                        }
                        catch
                        {
                            //odotellaan
                        }
                    }
                }
            
            } while(on);
            
            Console.ReadKey();
            s.Close();
            }
        }
    }