using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;

namespace Pastebin_Brute
{
    class Scraper
    {
        public static bool ForceStop { get; set; } = false;
        public static CountdownEvent SignalCountdown;
        public static void Scrape()
        {
            string[] proxies = File.ReadAllLines(Environment.CurrentDirectory + @"/proxies.txt");
            using (SignalCountdown = new CountdownEvent(1))
            {
                while (!ForceStop)
                {
                    for (int i = 0; i < proxies.Length; i++)
                    {
                        WebProxy prox = new WebProxy(proxies[i].Split(':')[0], int.Parse(proxies[i].Split(':')[1]));
                        string id = Generator.UniqueID(8);

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://pastebin.com/raw/{id}");

                        request.Method = "GET";
                        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows CE)";

                        request.Proxy = prox;
                        Console.WriteLine(id);
                        request.BeginGetResponse(WebCallback, request);
                    }
                }
                SignalCountdown.Signal();
                SignalCountdown.Wait();
            }
        }

        public static int Hits { get; set; } = 0;

        public static void UpdateTitle()
        {
            Console.Title = $"Pastebin Brute - {Hits}";
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }

        public static void WebCallback(IAsyncResult result)
        {
            try
            {
                HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
                Hits++;
                UpdateTitle();
                string body = ReadStreamFromResponse(response);
                Console.WriteLine(body);

            }
            catch (WebException ex)
            {

            }
            finally
            {

            }
        }
        
    }

    public class Generator
    {
        private const string Values = "ABCDEFGHIJKLMNOPQRSTUVWXYZadbcdefghijklmnopqrstuvwxyz1234567890";
        private static Random rnd = new Random();

        public static string UniqueID(int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
                builder.Append(Values[rnd.Next(0, Values.Length-1 )]);

            return builder.ToString();
        }
    }
}
