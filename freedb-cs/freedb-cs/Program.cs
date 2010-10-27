namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Globalization;

    public class Program
    {
        private object console = new object();

        public IDictionary<int, int> Run(string dir)
        {
            var years = new Dictionary<int, int>();
            Console.WriteLine("Enumerating files...");
            var source = new List<string>(FS.FindFiles(dir, "*.*"));
            Console.WriteLine("Found {0} files", source.Count);
            Console.WriteLine("Scanning files...");
            int n_scanned = 0;
            Parallel.ForEach(source, /*new ParallelOptions { MaxDegreeOfParallelism = 4 },*/ (f) =>
            {
                //lock (console)
                //{
                //    Console.WriteLine(f);
                //}
                if ((Interlocked.Increment(ref n_scanned) % 10000) == 0)
                {
                    Console.Write(".");
                }

                foreach (var line in File.ReadLines(f))
                {
                    // DYEAR=2007
                    int year;
                    if (line.Length > 6 && line.StartsWith("DYEAR=") && int.TryParse(line.Substring(6), out year))
                    {
                        lock (years)
                        {
                            if (years.ContainsKey(year))
                            {
                                years[year]++;
                            }
                            else
                            {
                                years[year] = 1;
                            }
                        }
                        break;
                    }
                }
            });
            Console.WriteLine("Done");

            return years;
        }

        public static void Main(string[] param)
        {

            try
            {
                if (param.Length < 1 || !Directory.Exists(param[0]))
                {
                    Console.WriteLine("Usage: count <directory>");
                    return;
                }

                var times = new List<double>();
                var now = DateTime.Now;
                var years = new Program().Run(param[0]);
                if (years.Keys.Count > 0)
                {
                    var mostFrequentYear = years.OrderBy(y => -y.Value).First();
                    Console.WriteLine("Year {0,4}: {1}", mostFrequentYear.Key, mostFrequentYear.Value);
                }

                double elapsed = (DateTime.Now - now).TotalSeconds;
                times.Add(elapsed);
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Processing took {0:F2} s", elapsed));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
