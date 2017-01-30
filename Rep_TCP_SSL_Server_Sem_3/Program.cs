using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Rep_TCP_SSL_Server_Sem_3
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverCertificateFile = "C:/Users/allan/Certifications/ServerSSL.cer";
            bool clientCerticateRequired = false;
            bool checkCertificateRevocation = true;
            SslProtocols enableSslProtocols = SslProtocols.Tls; //Superseed the former SslProtocols.Ssl.3

            X509Certificate serverCertificate = new X509Certificate(serverCertificateFile, "Secret");

            TcpListener serverSocket = new TcpListener(6789);

            serverSocket.Start();
            Console.WriteLine("Server Started");

            TcpClient connectionSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine("Server activated");

            Stream unsecureStream = connectionSocket.GetStream();

            bool leaveInnerStreamOpen = false;// close the unsecureStream when sslStream is closed
            // Decorator/Wrapper design pattern
            SslStream sslStream = new SslStream(unsecureStream, leaveInnerStreamOpen);
            sslStream.AuthenticateAsServer(serverCertificate, clientCerticateRequired, enableSslProtocols, checkCertificateRevocation);

            Console.WriteLine("Server authenticated");

            StreamReader sr = new StreamReader(sslStream);
            StreamWriter sw = new StreamWriter(sslStream);
            sw.AutoFlush = true;

            string message = sr.ReadLine();
            string answer = "";
            while (message != null && message != "")
            {
                Console.WriteLine("Client: " + message);
                answer = message.ToUpper();
                sw.WriteLine(answer);
                message = sr.ReadLine();

            }

            sslStream.Close();
            connectionSocket.Close();
            serverSocket.Stop();

        }
    }
}
