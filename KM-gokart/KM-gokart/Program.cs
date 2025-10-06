using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

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
                return $"|{vnev,-13} | {knev,-13} | {szulDatum.ToShortDateString(),-10} | {elmult18,-6} | {vAzon,-35} | {email,-35} |";
            }
        };

        static DateTime mai = DateTime.Now;

        static List<string> fejlec = new List<string>()
        {
            "Alföldi Racing Pilóta Nevelde",
            "6100, Kiskunfélegyháza 4625 fő út, 4. düllő",
            "+36-02-001-0911",
            "alfoldi-pilota.hu"
        };

        static Dictionary<int, string> opportunities = new Dictionary<int, string>()
        {
            {1, "Versenyzők megtekintése" },
            {2, "Foglalások megtekintése" },
            {3, "Foglalás" },
            {4, "Foglalás módosítás" },
            {5, "Kilépés" },
        };

        static Dictionary<string, List<string>> foglalasok = new Dictionary<string, List<string>>();

        static void Fejlec()
        {
            for (int i = 0; i < 46; i++) Console.Write("_");
            Console.WriteLine();
            foreach (string f in fejlec)
            {
                Console.Write($"|{f,-45}|\n");
            }
            Console.Write("|");
            for (int i = 0; i < 45; i++) Console.Write("_");
            Console.WriteLine("|");
            Console.WriteLine();
        }

        static void ChangeAppointment()
        {

            List<string> foglalasokDatum = new List<string>();

            foreach (var item in foglalasok.Values) 
            {
                foglalasokDatum.Add(item[0]);
            }

            string cim = "Jelenlegi foglalások: ";
            Console.WriteLine(cim);
            for (int i = 0; i < cim.Length; i++) Console.Write("-");
            Console.WriteLine();

            foreach (KeyValuePair<string, List<string>> f in foglalasok)
            {
                Console.WriteLine($"\t{f.Key}: dátum:{f.Value[0]} tartam(óra):{f.Value[1]} óra:{f.Value[2]}");
            }

            Console.WriteLine();
            Console.Write("Cserélni(mennyit): ");
            int mennyi = int.Parse(Console.ReadLine());

            for (int i = 0; i < mennyi; i++)
            {
                Console.Write("Gokart Azonosító: ");
                string goid = Console.ReadLine();
                Console.WriteLine();

                if (foglalasok.ContainsKey(goid))
                {
                    Console.WriteLine("Cserélendő adatok: ");

                    Console.Write("Dátum(éééé. hh. nn.): ");
                    string datum = Console.ReadLine();
                    DateTime Datum = new DateTime();

                    foreach (char d in datum)
                    {
                        string[] resz = datum.Split('.');
                        int ev = int.Parse(resz[0].Trim());
                        int ho = int.Parse(resz[1].Trim());
                        int nap = int.Parse(resz[2].Trim());
                        Datum = new DateTime(ev, ho, nap);
                    }

                    Console.Write("Óra: ");
                    int ora = int.Parse(Console.ReadLine());

                    Console.Write("Tartam(1óra vagy 2óra): ");
                    int tartam = int.Parse(Console.ReadLine());

                    if (foglalasokDatum.Any(x => Convert.ToDateTime(x) < Datum))
                    {
                        foglalasok[goid] = new List<string>
                        {
                            Datum.ToShortDateString(),
                            tartam.ToString(),
                            ora.ToString()
                        };
                    }
                    else 
                    {
                        Console.WriteLine("Nem lehetséges");
                    }
                }
            }
        }

        static string ReplaceAccentMark(string item)
        {
            List<char> ekezetek = new List<char>() {
                'á', 'é', 'ű', 'ú', 'ő', 'ó', 'ü', 'ö', 'í',
                'Á', 'É', 'Ű', 'Ú', 'Ő', 'Ó', 'Ü', 'Ö', 'Í' };
            List<char> ekezetek_ = new List<char>() {
                'a', 'e', 'u', 'u', 'o', 'o', 'u', 'o', 'i',
                'A', 'E', 'U', 'U', 'O', 'O', 'U', 'O', 'I'};
            string res = "";

            for (int i = 0; i < item.Length; i++)
            {
                bool cserelve = false;
                for (int j = 0; j < ekezetek.Count; j++)
                {
                    if (item[i] == ekezetek[j])
                    {
                        res += ekezetek_[j];
                        cserelve = true;
                        break;
                    }
                }
                if (!cserelve)
                {
                    res += item[i];
                }
            }

            return res;
        }

        static void Appoint(List<Versenyzo> versenyzok, int mennyi)
        {

            List<string> goIds = versenyzok.Select(x => x.GetId()).ToList();

            foreach (var item in goIds) { Console.WriteLine(item); }
            Console.WriteLine();

            string cim = "Foglalási szabályok";
            Console.WriteLine(cim);
            for (int i = 0; i < cim.Length; i++) Console.Write("-");
            Console.WriteLine("\n\t×8-19-ig");
            Console.WriteLine("\t×min 1óra, max 2óra");
            Console.WriteLine("\t×Az aktuális naptól, hónap végéig");
            Console.WriteLine("\t×Egy pilóta egyszer foglalhat");

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
                string ora = Console.ReadLine().Trim();

                Console.Write("Időre? (1óra vagy 2óra): ");
                int ido = int.Parse(Console.ReadLine().Trim());
                bool eldontendo = 2 >= ido && ido >= 1 ? true : false;
                ido = eldontendo ? ido : 0;

                DateTime idopont = new DateTime(ev, ho, nap);

                List<int> vegekUj = new List<int>();
                List<int> vegekRegi = new List<int>();


                for (int j = 0; j < ido; j++)
                {
                    vegekUj.Add(int.Parse(ora) + j);
                }


                foreach (var f in foglalasok.Values)
                {
                    if (f[0] == idopont.ToShortDateString())
                    {
                        int kezd = int.Parse(f[2]);
                        int tartam = int.Parse(f[1]);
                        for (int j = 0; j < tartam; j++)
                        {
                            vegekRegi.Add(kezd + j);
                        }
                    }
                }


                bool atfedes = vegekUj.Intersect(vegekRegi).Any();

                if (!atfedes)
                {
                    if (versenyzok.Any(x => x.GetId() == goid)
                        && ido != 0
                        && ev == mai.Year
                        && (ho <= 12 && ho == mai.Month)
                        && (nap >= mai.Day && nap <= DateTime.DaysInMonth(mai.Year, mai.Month))
                        && int.Parse(ora) >= 8 && int.Parse(ora) < 19 && int.Parse(ora) + ido <= 19)
                    {
                        foglalasok.Add(goid, new List<string> { idopont.ToShortDateString(), ido.ToString(), ora });
                    }
                }
                else
                {
                    Console.WriteLine("Nem lehetséges");
                }
            }
        }


        static List<Versenyzo> CreateVersenyzok(int count)
        {
            List<string> knev = File.ReadAllLines("keresztnevek.txt").ToList();
            List<string> vnev = File.ReadAllLines("vezeteknevek.txt").ToList();

            List<string> res_knev = new List<string>();
            List<string> res_vnev = new List<string>();

            foreach (string sor in knev)
            {
                string[] reszek = sor.Split(',');
                foreach (string resz in reszek)
                {
                    if (resz.Contains("'"))
                    {

                        string a = resz.Replace("'", "");
                        res_knev.Add(ReplaceAccentMark(a).Trim());
                    }
                }
            }

            foreach (string sor in vnev)
            {
                string[] reszek = sor.Split(',');
                foreach (string resz in reszek)
                {
                    if (resz.Contains("'"))
                    {

                        string a = resz.Replace("'", "");
                        res_vnev.Add(ReplaceAccentMark(a).Trim());
                    }
                }
            }

            List<Versenyzo> versenyzok = new List<Versenyzo>();

            Random rand = new Random();
            DateTime ma = DateTime.Now;

            for (int i = 0; i < count; i++)
            {
                int randomEv = rand.Next(1950, ma.Year + 1);
                int randomHo = rand.Next(1, ma.Month + 1);
                int randomNap = rand.Next(1, DateTime.DaysInMonth(randomEv, randomHo));

                DateTime datumString = new DateTime(randomEv, randomHo, randomNap);

                string chosenVnev = res_vnev[rand.Next(0, res_vnev.Count - 1)];
                string chosenKnev = res_knev[rand.Next(0, res_knev.Count - 1)];

                versenyzok.Add(new Versenyzo(
                    chosenVnev,
                    chosenKnev,
                    new DateTime(
                        randomEv,
                        randomHo,
                        randomNap),
                    ma.Year - 18 > randomEv ? true : false,
                    "Go-" + chosenVnev + chosenKnev + "-" + datumString.ToString("yyyyMMdd"),
                    chosenVnev.ToLower() + "." + chosenKnev.ToLower() + "@gmail.com"
                    ));
            }
            return versenyzok;
        }

        static void PrintOut(Dictionary<string, List<string>> foglalasok)
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
                List<int> tartamLista = new List<int>();

                for (int i = 0; i < datumok.Count; i++)
                {
                    if (datumok[i][0] == idopont)
                    {
                        orak.Add(int.Parse(datumok[i][2]));
                        tartamLista.Add(int.Parse(tartamok[i]));
                    }
                }

                for (int k = 0; k < 11; k++)
                {
                    int ora = 8 + k;
                    bool foglalt = false;

                    for (int i = 0; i < orak.Count; i++)
                    {
                        int kezd = orak[i];
                        int tartam = int.Parse(tartamok[i]);
                        int veg = kezd + tartam;


                        if (ora >= kezd && ora < veg)
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

            foglalasok = new Dictionary<string, List<string>>();

            Random rnd = new Random();

            List<Versenyzo> versenyzok = CreateVersenyzok(rnd.Next(20, 150+1));

            bool run = true;
            bool kiiratva = false;

            while (run)
            {
                Fejlec();
                Console.WriteLine("Üdvözöllek az Alföldi Racing Pilóta Nevelde gokart pályán!\n");

                foreach (KeyValuePair<int, string> o in opportunities)
                {
                    Console.WriteLine("\t" + o.Key + ": " + o.Value);
                }
                Console.WriteLine();

                Console.Write(">: ");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        Fejlec();
                        if (!(kiiratva))
                        {
                            Console.WriteLine("\nVersenyzők: " + " " + versenyzok.Count);
                            foreach (var v in versenyzok)
                            {
                                Console.Write(v + "\n");
                            }
                        }
                        kiiratva = true;
                        Console.WriteLine();
                        System.Threading.Thread.Sleep(2000);
                        break;
                    case 2:
                        Console.Clear();
                        Fejlec();
                        Console.WriteLine();
                        string cim = "Jelenlegi foglalások";
                        Console.WriteLine(cim);
                        for (int i = 0; i < cim.Length; i++) Console.Write("-");
                        Console.WriteLine();
                        foreach (KeyValuePair<string, List<string>> f in foglalasok)
                        {
                            Console.WriteLine($"\t{f.Key}: dátum:{f.Value[0]} tartam(óra):{f.Value[1]} óra:{f.Value[2]}");
                        }
                        Console.WriteLine();
                        PrintOut(foglalasok);

                        System.Threading.Thread.Sleep(2000);
                        break;

                    case 3:
                        Console.Clear();
                        Fejlec();
                        Console.Write("Hány emberre szeretnél foglalni: ");
                        int mennyi = int.Parse(Console.ReadLine());
                        Appoint(versenyzok, mennyi);
                        Console.WriteLine();
                        System.Threading.Thread.Sleep(2000);
                        Console.Clear();
                        break;
                    case 4:
                        Console.Clear();
                        Fejlec();
                        ChangeAppointment();
                        Console.WriteLine();
                        System.Threading.Thread.Sleep(2000);
                        Console.Clear();
                        break;
                    case 5:
                        Console.WriteLine("A program leállt.");
                        run = false;
                        break;
                }
            }
            Console.ReadKey();


        }
    }
}

