using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pastebin_Brute
{
    class Program
    {
        static void Main(string[] args)
        {
            Scraper.Scrape();
            Console.ReadKey(true);
        }
    }
}
