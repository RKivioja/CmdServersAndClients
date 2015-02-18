using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace PeliPalvelin
{
    class Program
    {
        static String erotin = " ";

        static void Main(string[] args)
        {
            Socket palvelin;
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 9999);
            try
            {
                palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                palvelin.Bind(iep);
            }
            catch
            {
                return;
            }

            String STATE = "WAIT";

            Boolean on = true;
            int vuoro = -1;
            int Pelaajat = 0;
            int Quit_ACK = 0;
            int luku = -1;
            int arvaus = -1;
            EndPoint[] Pelaaja = new EndPoint[2];
            String[] Nimi = new string[2];

            while (on)
            {
                
                IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = (EndPoint)(client);
                String[] kehys = new String[4];
                if (STATE != "END")
                    kehys = Vastaanota(palvelin, ref remote);

                switch (STATE)
                {
                    case "WAIT":
                        switch (kehys[0])
                        {
                            case "JOIN":
                                Pelaaja[Pelaajat] = remote;
                                Nimi[Pelaajat] = kehys[1];
                                Pelaajat++;
                                if (Pelaajat == 1)
                                {
                                    Laheta(palvelin, Pelaaja[0], "ACK 201 JOIN OK");
                                }
                                else if (Pelaajat == 2)
                                {
                                    //arvotaan aloittaja
                                    Random rand = new Random();
                                    int aloittaja = rand.Next(0, 1);
                                    vuoro = aloittaja;

                                    //arvotaan oikea luku
                                    luku = rand.Next(0, 10);

                                    Laheta(palvelin, Pelaaja[aloittaja], "ACK 202 " + Nimi[Flip(aloittaja)]);
                                    Laheta(palvelin, Pelaaja[Flip(aloittaja)], "ACK 203 " + Nimi[(aloittaja)]);

                                    STATE = "GAME";
                                }
                                else
                                {
                                    Console.WriteLine("Liian monta pelaajaa!");
                                    Laheta(palvelin, Pelaaja[2], "400");
                                    
                                }

                                break;
                            default:
                                break;
                        }
                        break;

                    case "GAME":
                        switch (kehys[0])
                        {
                            case "DATA":
                                try
                                {
                                    arvaus = Int32.Parse(kehys[1]);
                                }
                                catch 
                                {
                                    Console.WriteLine("Asiakkaan lähettämä teksti ei kelvannut parseroitavaksi!");
                                }

                                if (arvaus == luku)
                                {
                                    Console.WriteLine("Pelaaja " + Pelaaja[vuoro] + " arvasi oikein");
                                    STATE = "END";
                                }
                                else
                                {
                                    Laheta(palvelin, Pelaaja[vuoro], "ACK 300 DATA OK");
                                    Laheta(palvelin, Pelaaja[Flip(vuoro)], "DATA " + arvaus.ToString());
                                    vuoro = Flip(vuoro);

                                    STATE = "WAIT_ACK";
                                }
                                break;
                            default:
                                Laheta(palvelin, Pelaaja[vuoro], "400"); //määrittelemätön virhe
                                Console.WriteLine("Virhe: " + kehys[0]);
                                break;
                        }
                        break;

                    case "WAIT_ACK":
                        switch (kehys[0])
                        {
                            case "ACK":
                                switch (kehys[1])
                                {
                                    case "300":
                                        STATE = "GAME";
                                        break;
                                    default:
                                        Laheta(palvelin, Pelaaja[vuoro], "400");
                                        Console.WriteLine("Virhe: " + kehys[1]);
                                        break;
                                }
                                break;
                            
                            default:
                                Laheta(palvelin, Pelaaja[vuoro], "403");
                                Console.WriteLine("Virhe: " + kehys[0]);
                                break;
                        }
                        break;

                    case "END":
                        Laheta(palvelin, Pelaaja[vuoro], "QUIT 501 Voitit pelin! Oikea numero oli: " + luku);
                        Laheta(palvelin, Pelaaja[Flip(vuoro)], "QUIT 502 Hävisit pelin! Oikea numero oli: " + luku);
                        while (Quit_ACK < Pelaajat)
                        {
                            switch (kehys[0])
                            {
                                case "ACK":
                                    switch (kehys[1])
                                    {
                                        case "500":
                                            on = false;
                                            break;
                                        default:
                                            Laheta(palvelin, Pelaaja[vuoro], "400");
                                            Console.WriteLine("Virhe: " + kehys[1]);
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        
                        break;

                    default:
                        Console.WriteLine("Errors... ");
                        break;
                }
                //TILA = CLOSED
            }
        }
            private static void Laheta(Socket s, EndPoint ep, String msg)
        {
            s.SendTo(Encoding.ASCII.GetBytes(msg), ep);
        }

        private static String[] Vastaanota(Socket s, ref EndPoint remote)
        {   
            int paljon = 0;
            
            byte[] rec = new byte[256];

            paljon = s.ReceiveFrom(rec, ref remote);

            String rec_string = Encoding.ASCII.GetString(rec, 0, paljon);
            char[] delin = { ' ' };
            String[] palat = rec_string.Split(delin);

            if (palat.Length < 2) Console.WriteLine("Vastaanotettiin virheellinen määrä tekstiä");
            
            return palat;
        }

        /**
         * Vaihdetaan vuorossa olevaa pelaajaa
         * @param int i - Pelaajan indeksi vaidetaan vastakkaiseksi (2 pelaajaa: 0 -> 1 tai 1 -> 0)
         */
        private static int Flip(int i)
        {
            return 1 - i;
        }

    }
}
