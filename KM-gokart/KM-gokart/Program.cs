using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

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
        static string ReplaceAccentMark(string item) // segéd függvény
        {
            List<char> ekezetek = new List<char>() { 'á', 'é', 'ű', 'ú', 'ő', 'ó', 'ü', 'ö', 'í' };
            List<char> ekezetek_ = new List<char>() { 'a', 'e', 'u', 'u', 'o', 'o', 'u', 'o', 'i' };
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
                int randomHo = rand.Next(1, ma.Month +1); // 08
                int randomNap = rand.Next(1, DateTime.DaysInMonth(randomEv, randomHo)); // 13

                DateTime datumString = new DateTime(randomEv, randomHo, randomNap); // 1999. 08. 13.

                string chosenVnev = res_vnev[rand.Next(0, res_vnev.Count - 1)]; // Bajai
                string chosenKnev = res_knev[rand.Next(0, res_knev.Count - 1)]; // Mark

                Versenyzo v = new Versenyzo(
                    chosenVnev, // Bajai
                    chosenKnev, // Mark
                    new DateTime( // DateTime objektum példányosítása
                        randomEv,  // 1999.
                        randomHo, // 08.
                        randomNap), // 13.
                    ma.Year - 18 > randomEv ? true : false, // true -> 2025-18 > 1999
                    "Go-" + chosenVnev + chosenKnev + "-" + datumString.ToString("yyyyMMdd"), // Go-BajaiMark-19990813
                    chosenVnev.ToLower()+"."+chosenKnev.ToLower() + "@gmail.com" // bajai.mark@gmail.com
                    );
                versenyzok.Add(v); // versenyzok listához, a v objektum hozzáadása
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

            public override string ToString()
            {
                return $"{vnev} {knev} {szulDatum.ToShortDateString()} {elmult18} {vAzon} {email}";
            }

        };

        static void Main(string[] args)
        {
            //Console.WriteLine(res_vnev[rand.Next(0, res_vnev.Count - 1)] + " " + res_knev[rand.Next(0, res_knev.Count-1)]);

            List<Versenyzo> versenyzok = CreateVersenyzok(10); 

            foreach (Versenyzo v in versenyzok) 
            {
                Console.WriteLine(v);
            }

            Console.ReadKey();
        }
    }
}