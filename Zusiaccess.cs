// Decompiled with JetBrains decompiler
// Type: ZusiPicLib.OverhangData
// Assembly: ZusiPicLib, Version=2.4.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 6843D1EB-8C04-48CA-81CF-1A8E2DBC0D7F
// Assembly location: D:\data\Development\ZUSI-Tools\_updated_sources\ZusiPicLib_0.0.0\ZusiPicLib.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using ZusiKlassenLib;
using ZusiKlassenLib.Common;

#nullable disable
namespace ZusiPicLib
{
  public class NativeMethods
  {
    [DllImport("kernel32", CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
    private static extern System.IntPtr LoadLibrary(string fileName);
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeLibrary(IntPtr hModule);
    [DllImport("kernel32")]
    private static extern IntPtr GetProcAddress(System.IntPtr hModule, String procname);

    private static NativeMethods Current
    {
      get
      {
        var thr = System.Threading.Thread.CurrentThread;
        NativeMethods value;
        if (!threadDict.TryGetValue(thr, out value))
        {
          value = new NativeMethods();
          threadDict.Add(thr, value);
        }
        return value;
      }
    }
    private static System.Collections.Generic.Dictionary<System.Threading.Thread, NativeMethods> threadDict = new System.Collections.Generic.Dictionary<System.Threading.Thread, NativeMethods>();

    private IntPtr LibraryAddress = IntPtr.Zero;
    private bool LoadedTried = false;
    private int LoadingError = 0;
    private bool LoadingMethodError = false;
    private void LoadLibraryAddress(IEnumerable<string> zusiExeDir)
    {
      LoadedTried = true;
      string dllName = "z3strbie.dll";
      if (System.IntPtr.Size == 8)
        dllName = "z3strbie.64.dll";
      foreach (string pfad1 in zusiExeDir)
      {
        if (!System.IO.File.Exists(System.IO.Path.Combine(pfad1, dllName)))
          continue;
        LoadingError = 0;
        //LibraryAddress = LoadLibrary(Datei.TryFindFirstExistingFile(zusiExeDir, dllName));
        LibraryAddress = LoadLibrary(System.IO.Path.Combine(pfad1, dllName));
        //System.Console.WriteLine("load step ptr {0}", LibraryAddress);
        LoadingError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
        if (LibraryAddress == IntPtr.Zero)
          continue;
        else
          break;
      }
      //LoadingError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
      //System.Console.WriteLine("load res ptr {1} {0}", LoadingError, LibraryAddress);
      if (LibraryAddress != IntPtr.Zero)
        LoadingError = 0;
    }
    private TDelegate GetUnmanagedFunction<TDelegate>(string functionName) where TDelegate : class
    {
      IntPtr p = GetProcAddress(LibraryAddress, functionName);
      // Failure is a common case, especially for adaptive code.
      if (p == IntPtr.Zero)
      {
        LoadingMethodError = true;
        throw new MissingMethodException(functionName);
      }
      Delegate function = Marshal.GetDelegateForFunctionPointer(p, typeof(TDelegate));
      // Ideally, we'd just make the constraint on TDelegate be
      // System.Delegate, but compiler error CS0702 (constrained can't be System.Delegate)
      // prevents that. So we make the constraint system.object and do the cast from object-->TDelegate.
      object o = function;
      return (TDelegate)o;
    }

    private enum TD3DXImageFileformat
    {
      D3DXIFF_BMP,
      D3DXIFF_JPG,
      D3DXIFF_TGA,
      D3DXIFF_PNG,
      D3DXIFF_DDS,
      D3DXIFF_PPM,
      D3DXIFF_DIB,
      D3DXIFF_HDR
    }

    private static IEnumerable<string> GetZusiExeDirs()
    {
      IEnumerable<string> exeDirs = new List<string>();

      exeDirs = exeDirs.Append(Zusi.ZusiPath);

      return exeDirs;
    }


    private void LoadDll()
    {
      if (!LoadedTried)
        ReloadDllInst(GetZusiExeDirs());

      if (LoadingError != 0)
        throw new BadImageFormatException(System.Runtime.InteropServices.Marshal.GetExceptionForHR(LoadingError).Message, (System.IntPtr.Size == 8) ? "z3strbie.64.dll" : "z3strbie.dll");
    }
    public static void ReloadDll()
    {
      ReloadDll(GetZusiExeDirs());
    }
    public static void ReloadDll(IEnumerable<string> zusiExeDir)
    {
      Current.ReloadDllInst(zusiExeDir);
    }
    private void ReloadDllInst(IEnumerable<string> zusiExeDir)
    {
      LoadLibraryAddress(zusiExeDir);
      if (LoadingError != 0)
      {
        var errMsg = System.Runtime.InteropServices.Marshal.GetExceptionForHR(LoadingError);
        throw new BadImageFormatException(errMsg == null ? ((new BadImageFormatException()).Message + " (HRES: " + LoadingError.ToString("X8") + ")") : errMsg.Message, (System.IntPtr.Size == 8) ? "z3strbie.64.dll" : "z3strbie.dll");
      }
      LoadingMethodError = false;
      TempSpeichern_st3_fkt = GetUnmanagedFunction<TempSpeichern_st3_del>("TempSpeichern_st3");
      ls3Aktualisierer_fkt = GetUnmanagedFunction<ls3Aktualisierer_del>("ls3Aktualisierer");
      ls3Vorschau_fkt = GetUnmanagedFunction<ls3Vorschau_del>("ls3Vorschau");
      ftdVorschau_fkt = GetUnmanagedFunction<ftdVorschau_del>("ftdVorschau");
      EigeneDatenVerzeichnis_fkt = GetUnmanagedFunction<EigeneDatenVerzeichnis_del>("EigeneDatenVerzeichnis");
      dllVersion_fkt = GetUnmanagedFunction<dllVersion_del>("dllVersion");
    }

    private delegate void TempSpeichern_st3_del(string Arbeitsverzeichnis, string DateiName);
    private TempSpeichern_st3_del TempSpeichern_st3_fkt = null;
    public static void TempSpeichern_st3(string Arbeitsverzeichnis, string DateiName)
    {
      var inst = Current;
      inst.LoadDll();
      inst.TempSpeichern_st3_fkt(Arbeitsverzeichnis, DateiName);
    }

    private delegate bool ls3Aktualisierer_del(string Arbeitsverzeichnis, string DateiName, byte Modus);
    private ls3Aktualisierer_del ls3Aktualisierer_fkt = null;
    public static bool ls3Aktualisierer(string Arbeitsverzeichnis, string DateiName, byte Modus)
    {
      var inst = Current;
      inst.LoadDll();
      return inst.ls3Aktualisierer_fkt(Arbeitsverzeichnis, DateiName, Modus);
    }

    public enum ls3Ansicht
    {
      Fahrzeugansicht,
      Seitlich1,
      Seitlich2,
      Seitenansicht,
      Seitenansicht2
    }

    private delegate bool ls3Vorschau_del(string Arbeitsverzeichnis, string DateiName, string BMPDateiname, byte Modus, System.IntPtr DXHandle, int BMPWidth, int BMPHeight, TD3DXImageFileformat BMPFormat);
    private ls3Vorschau_del ls3Vorschau_fkt = null;
    //Achtung: Darf nur vom selben Thread aus aufgerufen werden!
    public static string ls3Vorschau(string[] ZusiDataDirs, string DateiNameRelativ, ls3Ansicht Modus, System.Drawing.Size size, string cachefilename, string cachepath)
    {
      var inst = Current;
      inst.LoadDll();

      string writePfad = System.IO.Path.Combine(cachepath, cachefilename);
      if (System.IO.File.Exists(writePfad + ".png") == false)
      {
        bool res = inst.ls3Vorschau_fkt(ZusiDataDirs[0], DateiNameRelativ, writePfad, (byte)Modus, IntPtr.Zero, size.Width, size.Height, TD3DXImageFileformat.D3DXIFF_PNG);
      }

      if (System.IO.File.Exists(writePfad + ".png"))
      {
       return writePfad + ".png";
      }
      else
        return null;
    }


    //function ftdVorschau(Arbeitsverzeichnis,                // Datenverzeichnis der Zusi-Daten
    //                      DateiName,                         // Dateiname der ftd-Datei relativ zum Datenverzeichnis 
    //                      BMPDateiname:PChar;                // voller Pfad der zu erzeugenden Bilddatei
    //                      Modus:Byte;                        // bisher ohne Funktion
    //                      DXHandle:THandle;                  // Handle des aufrufenden Programms
    //                      AnsichtNr,                         // laufende Nummer der Grafikansicht
    //                      BMPWidth, BMPHeight:integer;       // Abmessungen des Bilds
    //                      BMPFormat:TD3DXImageFileformat;    // Dateityp des Bilds, siehe unten
    //                      var x, y:integer                    // Rückgabe: Auflösung der Grafikansicht
    //                      ):Boolean; stdcall;                      



    private delegate bool ftdVorschau_del(string Arbeitsverzeichnis, string DateiName, string BMPDateiname, byte Modus, System.IntPtr DXHandle, int AnsichtNr, int BMPWidth, int BMPHeight, TD3DXImageFileformat BMPFormat, out int x, out int y);
    private ftdVorschau_del ftdVorschau_fkt = null;
    public static System.Drawing.Image ftdVorschau(string[] ZusiDataDirs, string DateiNameRelativ, int AnsichtNr, System.Drawing.Size size, out System.Drawing.Size resolution)
    {
      var inst = Current;
      inst.LoadDll();

      string writePfad = System.IO.Path.Combine(ZusiDataDirs[0], @"\Temp\vorschau");
      byte Modus = 0;
      int x = 0;
      int y = 0;

      bool res =
      inst.ftdVorschau_fkt(ZusiDataDirs[0], DateiNameRelativ,
        writePfad, (byte)Modus, IntPtr.Zero, AnsichtNr, size.Width, size.Height, TD3DXImageFileformat.D3DXIFF_PNG, out x, out y);
      resolution = new System.Drawing.Size(x, y);

      if (System.IO.File.Exists(writePfad + ".png"))
      {
        var img = System.Drawing.Image.FromFile(writePfad + ".png");
        System.IO.File.Delete(writePfad + ".png");
        return img;
      }
      else
        return null;
    }
    private const int MAX_PATH = 260;

    private delegate void EigeneDatenVerzeichnis_del(byte[] Arbeitsverzeichnis);
    private EigeneDatenVerzeichnis_del EigeneDatenVerzeichnis_fkt = null;
    public static string EigeneDatenVerzeichnis()
    {
      var inst = Current;
      inst.LoadDll();
      byte[] barr = new byte[MAX_PATH];
      inst.EigeneDatenVerzeichnis_fkt(barr);
      string s = System.Text.Encoding.Default.GetString(barr).Replace("\0", "");
      return s;
    }

    private delegate void dllVersion_del(byte[] Arbeitsverzeichnis);
    private dllVersion_del dllVersion_fkt = null;
    public static string dllVersion()
    {
      var inst = Current;
      inst.LoadDll();
      byte[] barr = new byte[MAX_PATH];
      inst.dllVersion_fkt(barr);
      string s = System.Text.Encoding.Default.GetString(barr).Replace("\0", "");
      return s;
    }
  }
}
