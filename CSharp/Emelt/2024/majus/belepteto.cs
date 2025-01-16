/*
    Info:
    Esemény kódok: 
        1: belépés a főkapun
        2: kilépés a főkapun
        3: az ebéd kiadása
        4: kölcsönzés a könyvtárban

    Adat formátuma:
        ABCD óó:pp E
    Fontos: Az adatok időrendi sorrendben vannak; Nem kell az adatokat ellenőrizni!
*/

using System;
using System.IO;

namespace belepteto;
class Program
{

    /* -------------------------------------------------------------------------- */

    // Ez a beolvasási hely csak erre a megoldásra érvényes, a .exe fájl miatt. Érettségin elég csak a fájl nevét megadni a beolvasáshoz.
    // pl: new StreamReader("bedat.txt");
    // Viszont az a módszer itt nem fog működni, ha futtatni is akarjuk a programot a .exe fájlból.

    static string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
    static string adatFajlHelye = Path.Combine(projectDir, "forrasok", "bedat.txt");

    /* -------------------------------------------------------------------------- */

    // Az összes esemény listája
    static List<Esemeny> esemenyek = new List<Esemeny>();

    // Érdemes egy struktúrába elhelyezni az adatokat az átláthatóságért
    struct Esemeny
    {
        public string azon;
        public string ido;
        public int esemenyKod;
    }

    static void Main()
    {
        // Érdemes a feladatokat függvényekbe írni az olvashatóság miatt
        Feladat1();
        Feladat2();
        Feladat3();
        Feladat4();
        Feladat5();
        Feladat6();
        Feladat7();
    }

    static void Feladat1()
    {
        Console.WriteLine("1. feladat - Adatbeolvasás");


        // Soronként olvassuk a fájlt...
        using (StreamReader adat = new StreamReader(adatFajlHelye))
        {
            string sor;
            while ((sor = adat.ReadLine()) != null) // Amíg van sor a fájlban
            {
                string[] darabok = sor.Split(" ");
                Esemeny esemeny = new Esemeny();
                esemeny.azon = darabok[0];
                esemeny.ido = darabok[1];
                esemeny.esemenyKod = Convert.ToInt16(darabok[2]);

                esemenyek.Add(esemeny); // Adjuk hozzá ezt az eseményt az összes többihez
            }
        }
    }

    static void Feladat2()
    {
        Console.WriteLine("2. feladat");
        // Ezt szabad csinálni, nem kell az esemény kódokat vizsgálni, mert a feladat leírja, hogy az első esemény egy belépés, az utolsó pedig egy kilépés.
        Console.WriteLine($"Az első tanuló belépési időpontja: {esemenyek[0].ido}\nAz utolsó tanuló kilépési időpontja: {esemenyek[^1].ido}");
    }

    static void Feladat3()
    {
        Console.WriteLine("3. feladat");

        // Keresztül megyünk az összes eseményen és megnézzük, ki mikor jött suliba (esemény kód == 1)
        foreach (var esemeny in esemenyek)
        {
            // A string.Compare függvény megmondja, hogy az adott string betűrendben a másikhoz képest hol helyezkedik el.
            // -1-et ad, ha mögötte, 0-t ha egyenlő és 1-et ha utána jön. Ezzel meg tudjuk különböztetni, hogy egy bizonyos
            // időkereten belül jött-e a diák.
            using (StreamWriter sw = new StreamWriter("kesok.txt"))
            {
                if (string.Compare(esemeny.ido, "07:50") > 0 && string.Compare(esemeny.ido, "08:15") < 0 && esemeny.esemenyKod == 1)
                {
                    sw.WriteLine($"{esemeny.azon} {esemeny.ido}");
                }
            }
        }
    }

    static void Feladat4()
    {
        Console.WriteLine("4. feladat");

        int hanyDarab = 0;
        foreach (var esemeny in esemenyek)
        {
            if (esemeny.esemenyKod == 3)
                hanyDarab += 1;
        }

        Console.WriteLine($"Aznap {hanyDarab} tanuló ebédelt a menzán.");
    }

    static void Feladat5()
    {
        Console.WriteLine("5. feladat");
        List<string> megvizsgaltAzonositok = new List<string>();
        int kolcsonzokSzama = 0;
        int menzasokSzama = 0;
        foreach (var esemeny in esemenyek)
        {
            if (megvizsgaltAzonositok.Contains(esemeny.azon))
                continue;

            if (esemeny.esemenyKod == 3)
                menzasokSzama += 1;
            else if (esemeny.esemenyKod == 4)
                kolcsonzokSzama += 1;

            megvizsgaltAzonositok.Add(esemeny.azon);// fix
        }
        Console.WriteLine($"Aznap {kolcsonzokSzama} tanuló kölcsönzött könyvet.");
        if (kolcsonzokSzama > menzasokSzama)
            Console.WriteLine("Többen voltak, mint a menzán");
        else
            Console.WriteLine("Nem voltak többen, mint a menzán");
    }

    static void Feladat6()
    {
        Console.WriteLine("6. feladat");

        for (int i = 0; i < esemenyek.Count; i++)
        {
            if (string.Compare(esemenyek[i].ido, "10:50") > 0 && string.Compare(esemenyek[i].ido, "11:00") < 0 && esemenyek[i].esemenyKod == 1)
            {
                // Ezt megelőzően utoljára mikor lépett be
                int utolsoBelepes = i - 1;
                while (utolsoBelepes >= 0 && !(esemenyek[i].azon == esemenyek[utolsoBelepes].azon && esemenyek[utolsoBelepes].esemenyKod == 1))
                    utolsoBelepes--;
                // A két belépés között szabályosan kiment-e
                int szabalyosKimenes = utolsoBelepes + 1;
                while (szabalyosKimenes < i && !(esemenyek[szabalyosKimenes].azon == esemenyek[i].azon && esemenyek[szabalyosKimenes].esemenyKod == 2))
                    szabalyosKimenes++;
                if (utolsoBelepes > -1 && szabalyosKimenes == i)
                    Console.Write(esemenyek[i].azon + " ");
            }
        }

        Console.WriteLine();
    }

    static void Feladat7()
    {
        Console.WriteLine("7. feladat\n");

        Console.WriteLine("Kérem a tanuló azonosítóját: ");
        string esemenyAzon = Console.ReadLine();

        bool talaltE = false;
        Esemeny e = new Esemeny();

        foreach (var esemeny in esemenyek)
        {
            if (esemeny.azon == esemenyAzon && esemeny.esemenyKod == 1)
            {
                e = esemeny;
                talaltE = true;
            }
        }

        if (!talaltE)
        {
            Console.WriteLine("Ilyen azonosítójú tanuló aznap nem volt az iskolában.");
            return;
        }

        string[] belepDarabok = e.ido.Split(":");
        int belepOra = Convert.ToInt16(belepDarabok[0]);
        int belepPerc = Convert.ToInt16(belepDarabok[1]);
        int belepPercek = belepOra * 60 + belepPerc;

        foreach (var esemeny in esemenyek)
        {
            if (esemeny.azon != esemenyAzon || esemeny.esemenyKod != 2)
                continue;

            string[] kilepDarabok = e.ido.Split(":");
            int kilepOra = Convert.ToInt16(kilepDarabok[0]);
            int kilepPerc = Convert.ToInt16(kilepDarabok[1]);

            int kilepPercek = kilepOra * 60 + kilepPerc;

            int diff = kilepPercek - belepPercek;
            int elteltOrak = diff / 60;
            int elteltPercek = diff % 60;

            Console.WriteLine($"A tanuló érkezése és távozása között {elteltOrak} óra és {elteltPercek} perc telt el."); // fix
        }

    }
}