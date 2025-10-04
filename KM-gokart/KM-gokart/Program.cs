using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
    Alföldi Racing Pilóta Nevelde
    6100, Kiskunfélegyháza 4625 fő út, 4. düllő
    +36-02-001-0911
    alfold-pilota.hu
 */


namespace KM_gokart
{
    internal class Program
    {
        static DateTime mai = DateTime.Now;
        static string ReplaceAccentMark(string item) // segéd függvény
        {
            List<char> ekezetek = new List<char>() {
                'á', 'é', 'ű', 'ú', 'ő', 'ó', 'ü', 'ö', 'í',
                'Á', 'É', 'Ű', 'Ú', 'Ő', 'Ó', 'Ü', 'Ö', 'Í' };
            List<char> ekezetek_ = new List<char>() {
                'a', 'e', 'u', 'u', 'o', 'o', 'u', 'o', 'i',
                'A', 'E', 'U', 'U', 'O', 'O', 'U', 'O', 'I'};
            string res = "";

            for (int i = 0; i < item.Length; i++) // kovács 
            {
                bool cserelve = false; // false false false true false false
                for (int j = 0; j < ekezetek.Count; j++)
                {
                    if (item[i] == ekezetek[j])
                    {
                        res += ekezetek_[j]; // k o v (á)->a
                        cserelve = true; // true
                        break;
                    }
                }
                if (!cserelve)
                {
                    res += item[i]; // k o v c s
                }
            }

            return res; // visszaadom a kész tökéletes, ékezet mentes string-et
        }

        static Dictionary<string, List<string>> Foglalas(List<Versenyzo> versenyzok, int mennyi)
        {
            Dictionary<string, List<string>> foglalasok = new Dictionary<string, List<string>>();

            List<string> goIds = versenyzok.Select(x => x.GetId()).ToList();

            foreach (var item in goIds) { Console.WriteLine(item); }
            Console.WriteLine();
            for (int i = 0; i < mennyi; i++)
            {

                Console.Write("\nGokart-Id: ");
                string goid = Console.ReadLine();

                Console.Write("Időpont (év): ");
                int ev = int.Parse(Console.ReadLine());

                Console.Write("Időpont (hó): ");
                int ho = int.Parse(Console.ReadLine());

                Console.Write("Időpont (nap): ");
                int nap = int.Parse(Console.ReadLine());

                Console.Write("Időpont (óra): ");
                string ora = Console.ReadLine();

                Console.Write("Időre? (1óra - 2óra) percben pl: 110: ");
                int ido = int.Parse(Console.ReadLine());
                bool eldontendo = 120 > ido && ido > 60 ? true : false;
                ido = eldontendo ? ido : 0;

                DateTime idopont = new DateTime(ev, ho, nap, 0, 0, 0);

                if (versenyzok.Any(x => x.GetId() == goid)
            && ido != 0
            && ev >= mai.Year
            && (ho <= 12 || ho >= mai.Month)
            && (nap >= mai.Day || nap <= DateTime.DaysInMonth(mai.Year, mai.Month)))
                {
                    foglalasok.Add(goid, new List<string> { idopont.ToShortDateString(), ido.ToString(), ora });
                }
            }
            return foglalasok;
        }

        static List<Versenyzo> CreateVersenyzok(int count) // függvény
        {
            List<string> knev = File.ReadAllLines("keresztnevek.txt").ToList(); // beolvasom a txt fájlt, és elmentem Listába (ToList())
            List<string> vnev = File.ReadAllLines("vezeteknevek.txt").ToList(); // beolvasom a txt fájlt, és elmentem Listába (ToList())

            List<string> res_knev = new List<string>();
            List<string> res_vnev = new List<string>();

            foreach (string sor in knev) // bejárom a knev-et
            {
                string[] reszek = sor.Split(','); // split-elem "," alapján, ebből kapom meg a reszek tömböt
                foreach (string resz in reszek) // bejárom a részek tömböt
                {
                    if (resz.Contains("'")) // részek tömb elemeinél megvizsgálom van-e benne "'"
                    {
                        // ha van
                        string a = resz.Replace("'", ""); // a Replace metódussal kicserélem a "'" -> üresre ""
                        res_knev.Add(ReplaceAccentMark(a).Trim()); // res_vnev listához adom a kapott vnev-et, majd
                                                                   // ReplaceAccentMark függvénnyel megoldom,
                                                                   // hogy ne legyen ékezetes
                                                                   // és végül eltüntetem a felesleges white space-eket a Trim metódussal
                    }
                }
            }

            foreach (string sor in vnev) // bejárom a vnev-et
            {
                string[] reszek = sor.Split(','); // split-elem "," alapján, ebből kapom meg a reszek tömböt
                foreach (string resz in reszek) // bejárom a részek tömböt
                {
                    if (resz.Contains("'")) // részek tömb elemeinél megvizsgálom van-e benne "'"
                    {
                        // ha van
                        string a = resz.Replace("'", ""); // a Replace metódussal kicserélem a "'" -> üresre ""
                        res_vnev.Add(ReplaceAccentMark(a).Trim()); // res_vnev listához adom a kapott vnev-et, majd
                                                                   // ReplaceAccentMark függvénnyel megoldom,
                                                                   // hogy ne legyen ékezetes
                                                                   // és végül eltüntetem a felesleges white space-eket a Trim metódussal
                    }
                }
            }

            List<Versenyzo> versenyzok = new List<Versenyzo>();

            Random rand = new Random(); // random objektum példányosítása
            DateTime ma = DateTime.Now; // jelenlegi dátum

            for (int i = 0; i < 10; i++)
            {
                int randomEv = rand.Next(1950, ma.Year + 1); // 1999
                int randomHo = rand.Next(1, ma.Month + 1); // 08
                int randomNap = rand.Next(1, DateTime.DaysInMonth(randomEv, randomHo)); // 13

                DateTime datumString = new DateTime(randomEv, randomHo, randomNap); // 1999. 08. 13.

                string chosenVnev = res_vnev[rand.Next(0, res_vnev.Count - 1)]; // Bajai
                string chosenKnev = res_knev[rand.Next(0, res_knev.Count - 1)]; // Mark

                versenyzok.Add(new Versenyzo(
                    chosenVnev, // Bajai
                    chosenKnev, // Mark
                    new DateTime( // DateTime objektum példányosítása
                        randomEv,  // 1999.
                        randomHo, // 08.
                        randomNap), // 13.
                    ma.Year - 18 > randomEv ? true : false, // true -> 2025-18 > 1999
                    "Go-" + chosenVnev + chosenKnev + "-" + datumString.ToString("yyyyMMdd"), // Go-BajaiMark-19990813
                    chosenVnev.ToLower() + "." + chosenKnev.ToLower() + "@gmail.com" // bajai.mark@gmail.com
                    )); // versenyzok listához, a v objektum hozzáadása
            }
            return versenyzok; // visszadja a versenyzok listát, versenyzo objektumokkal
        }

        public class Versenyzo
        {
            private string vnev;
            private string knev;
            private DateTime szulDatum;
            private bool elmult18;
            private string vAzon;
            private string email;

            public Versenyzo(string vnev, string knev, DateTime szulDatum, bool elmult18, string vAzon, string email)
            {
                this.vnev = vnev;
                this.knev = knev;
                this.szulDatum = szulDatum;
                this.elmult18 = elmult18;
                this.vAzon = vAzon;
                this.email = email;
            }

            public string GetId()
            {
                return vAzon;
            }

            public override string ToString()
            {
                return $"{vnev} {knev} {szulDatum.ToShortDateString()} {elmult18} {vAzon} {email}";
            }

        };

        static void kiiratas(Dictionary<string, List<string>> foglalasok)
        {
            int nap = mai.Day;

            List<List<string>> datumok = foglalasok.Values.ToList();

            List<string> tartamok = new List<string>();

            foreach (var item in datumok)
            {
                tartamok.Add(item[1]);
            }

            Console.WriteLine("\t\t8-9\t9-10\t10-11\t11-12\t12-13\t13-14\t15-16\t16-17\t17-18\t18-19", -100);

            for (int x = 0; x < DateTime.DaysInMonth(mai.Year, mai.Month) - mai.Day + 1; x++)
            {
                string idopont = new DateTime(mai.Year, mai.Month, nap).ToShortDateString();

                Console.Write($"\n{idopont} \t");

                List<int> orak = new List<int>();
                foreach (var d in datumok)
                {
                    if (d[0] == idopont)
                    {
                        orak.Add(int.Parse(d[2]));
                    }
                }

                for(int k = 0; k < 11; k++) 
                {
                int ora = 8 + k;
                bool foglalt = false;

                for (int i = 0; i < orak.Count; i++)
                    {
                    int kezd = orak[i];                
                    int tartam = int.Parse(tartamok[i]); 
                    int veg = kezd + tartam / 60;      


                    if (ora >= kezd && ora <= veg)
                    {
                        foglalt = true;
                        break;
                    }
                }

                if (foglalt)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("X\t");
                    }
                else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("O\t");
                    }
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                nap++;
            }
        }

            static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine(res_vnev[rand.Next(0, res_vnev.Count - 1)] + " " + res_knev[rand.Next(0, res_knev.Count-1)]);

            List<Versenyzo> versenyzok = CreateVersenyzok(10);
            Dictionary<string, List<string>> foglalasok = Foglalas(versenyzok, 2);

            kiiratas(foglalasok);

            Console.ReadKey();

            
        }
    }
}

