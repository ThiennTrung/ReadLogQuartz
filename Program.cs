using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace ReadLogQuartz
{
    class Program
    {
        static void Main(string[] args)
        {
            // Mỗi site 1 Thread

            // SG PRO
            Thread t = new Thread(() => {
                ReadFileLog("http://10.8.20.37:6060/QuartzServices/logs");
            });
            t.Start();

            // DN PRO
            Thread t2 = new Thread(() =>
            {
                ReadFileLog("http://118.68.171.147:6060/QuartzServices1/logs/");
            });
            t2.Start();

            Console.ReadLine();
        }


        static void ReadFileLog(string path)
        {
            string pattern = @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}";
            List<string> lines = new List<string>();

            string FileName = string.Format("log-{0}.txt",DateTime.Now.ToString("yyyyMMdd"));
            var webRequest = WebRequest.Create(Path.Combine(path, FileName));

            // ĐỌC FILE
            using var response = webRequest.GetResponse();
            using var content = response.GetResponseStream();
            using var reader = new StreamReader(content);
            string line;
            while ((line = reader.ReadLine()) != null) {  lines.Add(line); }

            // CHECK LỖI
            foreach (string lne in lines.TakeLast(15))
            {
                if (lne.Contains("[ERR]"))
                {
                    Console.WriteLine(lne);
                }
            }

            // CHECK STOP
            string LastLine = lines.LastOrDefault();
            Regex regex = new Regex(pattern);
            Match match = regex.Match(LastLine);
            if (match != null)
            {
                DateTime myDate = DateTime.ParseExact(match.Value, "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                if ((DateTime.Now - myDate).TotalMinutes > 5)
                {
                    Console.WriteLine("QUARTZ DỪNG RỒIIIIIIIIIIIIIIII");
                }
            }
        }

    }
}
