/**
 * Yksinkertainen SMTP-asiakas, joka lähettää kovakoodatun viestin sähköpostipalvelimelle.
 * @author: Roope Kivioja
 * @date: 08.09.2014
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace SMTPAsiakas
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                s.Connect("localhost", 25000);
            }
            catch (Exception ex)
            {
                Console.Write("Virhe: " + ex.Message);
                Console.ReadKey();
                return;
            }

            NetworkStream ns = new NetworkStream(s);

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            String email = "testiposti";

            Boolean on = true;
            String viesti = "";

            while (on)
            {
                //luetaan
                viesti = sr.ReadLine();
                Console.WriteLine(viesti);
                String[] status = viesti.Split(' ');

                switch (status[0])
                {
                    case "220":
                        sw.WriteLine("HELO jyu.fi");
                        break;
                    case "250":
                        switch (status[1])
                        {
                            case "2.0.0":
                                sw.WriteLine("QUIT");
                                break;
                            case "2.1.0":
                                sw.WriteLine("RCPT TO: roope.kivioja@gmail.com");
                                break;
                            case "2.1.5":
                                sw.WriteLine("DATA");
                                break;
                            default: //250 ITKP104 Postipalvelin HELO 127.0.0.1[127.0.0.1], good to see you!
                                sw.WriteLine("MAIL FROM: roope.kivioja@gmail.com");
                                break;
                        } //switch
                        break;
                    case "221":
                        on = false;
                        break;
                        
                    case "354":
                        sw.WriteLine(email);
                        sw.WriteLine("\r\n");
                        sw.WriteLine("\r\n.\r\n");
                        break;

                    default:
                        Console.WriteLine("Virhe...");
                        sw.WriteLine("QUIT");
                        break;
                } //switch
                sw.Flush();
            } //while 

            Console.ReadKey();

            sw.Close();
            sr.Close();
            ns.Close();
            s.Close();
        }
    }
}
