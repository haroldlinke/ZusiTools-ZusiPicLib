// Decompiled with JetBrains decompiler
// Type: ZusiPicLib.ZusiPictureManager
// Assembly: ZusiPicLib, Version=2.4.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 6843D1EB-8C04-48CA-81CF-1A8E2DBC0D7F
// Assembly location: D:\data\Development\ZUSI-Tools\_updated_sources\ZusiPicLib_0.0.0\ZusiPicLib.dll

using log4net;
using Sovoma;
using Sovoma.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZusiPicLib.Properties;

#nullable disable
namespace ZusiPicLib
{
  public class ZusiPictureManager : Singleton<ZusiPictureManager>, ISingletonBase, IDisposable
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(ZusiPictureManager));
    private static readonly OverhangData _defaultOverhangData = new OverhangData();
    private static readonly ushort _magicZip = 19280;
    private bool _alternativeSourcesAllowed = true;
    private BitmapImage _nopic;
    private ZipArchive _zipArchive;
    private Dictionary<string, OverhangData> _overhangData = new Dictionary<string, OverhangData>();
    private string _randomName;
    private NativeMethods _nativeMethods = null;

    public static bool IsClean
    {
      get
      {
        try
        {
          return Singleton<ZusiPictureManager>.Instance.OpenArchive();
        }
        finally
        {
          Singleton<ZusiPictureManager>.Instance.ReleaseArchive();
        }
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object)this);
    }

    public void Initialize()
    {
      try
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Resources.nopic.Save((Stream)memoryStream, ImageFormat.Png);
          memoryStream.Position = 0L;
          this._nopic = new BitmapImage();
          this._nopic.BeginInit();
          this._nopic.StreamSource = (Stream)memoryStream;
          this._nopic.CacheOption = BitmapCacheOption.OnLoad;
          this._nopic.EndInit();
        }
      }
      catch (Exception ex)
      {
        ZusiPictureManager._log.Debug((object)ex.ToString());
      }
      _nativeMethods = new();
    }

    protected void Dispose(bool disposing)
    {
      if (disposing)
        this.ReleaseArchive();
      //base.Dispose(disposing);
    }

    private void AllowAlternativeSources_impl()
    {
      if (this._alternativeSourcesAllowed)
        return;
      this.ReleaseArchive();
      this._alternativeSourcesAllowed = true;
    }

    private string AssembleZipFileName(
      ZusiPictureManager.AssembleStrategy strategy,
      bool createDirectory)
    {
      string str;
      if (strategy != ZusiPictureManager.AssembleStrategy.Default && strategy == ZusiPictureManager.AssembleStrategy.Alternative)
      {
        str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZusiPicLib");
        if (createDirectory && !Directory.Exists(str))
          Directory.CreateDirectory(str);
      }
      else
        str = Path.Combine(Path.GetDirectoryName(WpfAssembly.GetMainAssembly().Location), "Resources");
      return Path.Combine(str, "pictures.dat");
    }

    private void ForbidAlternativeSources_impl()
    {
      if (!this._alternativeSourcesAllowed)
        return;
      this.ReleaseArchive();
      this._alternativeSourcesAllowed = false;
    }

    private ushort GetMagicWord(string filename)
    {
      using (FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read))
      {
        if (input.Length <= 2L)
          return ushort.MaxValue;
        using (BinaryReader binaryReader = new BinaryReader((Stream)input))
          return binaryReader.ReadUInt16();
      }
    }

    private string CreateFromDir_impl(string directory, bool encrypted)
    {
      this.ReleaseArchive();
      string fromDirImpl = this.AssembleZipFileName(ZusiPictureManager.AssembleStrategy.Alternative, true);
      if (File.Exists(fromDirImpl))
        File.Delete(fromDirImpl);
      string str = string.Format("{0}{1}", (object)Path.GetTempPath().EnsureTrailingBackslash(), (object)Guid.NewGuid().ToString("N"));
      if (File.Exists(str))
        File.Delete(str);
      try
      {
        ZipFile.CreateFromDirectory(directory, str);
        if (encrypted)
          CipherFile.EncryptFile2(str, fromDirImpl, PicLibSecret.Secret);
        else
          File.Move(str, fromDirImpl);
      }
      finally
      {
        try
        {
          File.Delete(str);
        }
        catch
        {
        }
      }
      return fromDirImpl;
    }

    private bool OpenArchive()
    {
      if (this._zipArchive == null)
      {
        ZusiPictureManager.AssembleStrategy[] assembleStrategyArray;
        if (!this._alternativeSourcesAllowed)
          assembleStrategyArray = new ZusiPictureManager.AssembleStrategy[1];
        else
          assembleStrategyArray = new ZusiPictureManager.AssembleStrategy[2]
          {
            ZusiPictureManager.AssembleStrategy.Alternative,
            ZusiPictureManager.AssembleStrategy.Default
          };
        bool flag = false;
        foreach (ZusiPictureManager.AssembleStrategy strategy in assembleStrategyArray)
        {
          string str = this.AssembleZipFileName(strategy, false);
          if (File.Exists(str))
          {
            try
            {
              if ((int)this.GetMagicWord(str) == (int)ZusiPictureManager._magicZip)
              {
                this._zipArchive = ZipFile.OpenRead(str);
              }
              else
              {
                this._randomName = string.Format("{0}{1}", (object)Path.GetTempPath().EnsureTrailingBackslash(), (object)Guid.NewGuid().ToString("N"));
                CipherFile.DecryptFile2(str, this._randomName, PicLibSecret.Secret);
                this._zipArchive = ZipFile.OpenRead(this._randomName);
              }
              ZipArchiveEntry entry = this._zipArchive.GetEntry("overhang.dat");
              if (entry != null)
              {
                using (Stream serializationStream = entry.Open())
#pragma warning disable SYSLIB0011 // Typ oder Element ist veraltet
                  this._overhangData = new BinaryFormatter().Deserialize(serializationStream) as Dictionary<string, OverhangData>;
#pragma warning restore SYSLIB0011 // Typ oder Element ist veraltet
              }
              else
                ZusiPictureManager._log.Warn((object)"zip file is missing overhang data");
              flag = true;
              break;
            }
            catch (Exception ex)
            {
              ZusiPictureManager._log.Error((object)ex.ToString());
              break;
            }
          }
        }
        if (!flag)
          ZusiPictureManager._log.Warn((object)"the picture archiv could not be found");
      }
      return this._zipArchive != null;
    }

    private void ReleaseArchive()
    {
      Disposable.Dispose<ZipArchive>(ref this._zipArchive);
      if (string.IsNullOrEmpty(this._randomName))
        return;
      if (!File.Exists(this._randomName))
        return;
      try
      {
        File.Delete(this._randomName);
      }
      catch
      {
      }
    }


    public static ImageSource ConvertToImageSource(System.Drawing.Image image)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        // Save the System.Drawing.Image to the MemoryStream in PNG format
        image.Save(memoryStream, ImageFormat.Png);
        memoryStream.Position = 0; // Reset the stream position

        // Create a BitmapImage from the MemoryStream
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memoryStream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Load the image into memory
        bitmapImage.EndInit();
        bitmapImage.Freeze(); // Freeze for performance benefits

        return bitmapImage; // Return as ImageSource
      }
    }

    private ImageSource GetPicture_impl(
     string name,
     string ls3_filename,
     int majorVariantId,
     int minorVariantId,
     System.Drawing.Size picsize,
     bool rotated,
     string cachefilename,
     string cachepath,
     out OverhangData overhangData,
     int objectsize = 0)
    {
      if (true) //this.OpenArchive())
      {
        string lower = (string.Format("{0}-{1}-{2}", (object)name, (object)majorVariantId, (object)minorVariantId) + (rotated ? "-r" : "")).ToLower();
        //overhangData = this._overhangData == null || !this._overhangData.ContainsKey(lower) ? (OverhangData)null : this._overhangData[lower];
        overhangData = _defaultOverhangData;
        string str = lower + ".png";
        //string filename = name + "-" + idHaupt.ToString() + "-" + IdNeben.ToString();
        if (true)
        {
          string Arbeitsverzeichnis = "C:\\Program Files\\Zusi3\\_ZusiData";
          string[] ZusiDataDirs = { Arbeitsverzeichnis, "test" };
          string DateiNameRelativ = ls3_filename; // "RollingStock\\Deutschland\\Epoche3\\Dieselloks\\BRD\\V60\\3D-Daten\\db_260_ar.lod.ls3";
          NativeMethods.ls3Ansicht mode = NativeMethods.ls3Ansicht.Seitenansicht;

          //      public enum ls3Ansicht
          //{
          //  Fahrzeugansicht,
          //  Seitlich1,
          //  Seitlich2,
          //  Seitenansicht
          //}
          //int w =  31 * 20;
          //int h = 7 * 20;
          //System.Drawing.Size size = new System.Drawing.Size(w, h);
          
          string filename = NativeMethods.ls3Vorschau(ZusiDataDirs, DateiNameRelativ, mode, picsize, cachefilename, cachepath);
          if (System.IO.File.Exists(filename))
          {
            // Create a BitmapImage from a file
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();
            bitmap.Freeze(); // Freeze for performance benefits

            int x = picsize.Height / 2;
            int x2 = objectsize;

            if (objectsize == 0)
            {
              x2 = picsize.Width;
            }
            

            CroppedBitmap cropbitmap = new CroppedBitmap(bitmap, new Int32Rect(x, 0,x2, picsize.Height));

            return cropbitmap;
          }
          else
          {
            overhangData = _defaultOverhangData;
            return (ImageSource)this._nopic;
          }

        }
        else
        {
          string writePfad = "C:/testimg/" + str;
          if (System.IO.File.Exists(writePfad))
          {
            var img = System.Drawing.Image.FromFile(writePfad);
            //System.IO.File.Delete(writePfad + ".png");

            return ConvertToImageSource(img);
          }
        }
      }

      overhangData = _defaultOverhangData;
      return (ImageSource)this._nopic;

    }

    private ImageSource GetPicture_impl2(
      string name,
      int majorVariantId,
      int minorVariantId,
      bool rotated,
      out OverhangData overhangData)
    {
      //if (this.OpenArchive())
      //{
      string lower = (string.Format("{0}-{1}-{2}", (object)name, (object)majorVariantId, (object)minorVariantId) + (rotated ? "-r" : "")).ToLower();
      overhangData = overhangData = ZusiPictureManager._defaultOverhangData; //this._overhangData == null || !this._overhangData.ContainsKey(lower) ? (OverhangData)null : this._overhangData[lower];
      string str = lower + ".png";
      ZipArchiveEntry entry = this._zipArchive.GetEntry(str);
      if (entry != null)
      {
        byte[] buffer = new byte[entry.Length];
        using (Stream stream = entry.Open())
          stream.Read(buffer, 0, (int)entry.Length);
        using (MemoryStream memoryStream = new MemoryStream(buffer))
        {
          BitmapImage pictureImpl = new BitmapImage();
          pictureImpl.BeginInit();
          pictureImpl.CacheOption = BitmapCacheOption.OnLoad;
          pictureImpl.StreamSource = (Stream)memoryStream;
          pictureImpl.EndInit();
          return (ImageSource)pictureImpl;
        }
      }
        else
          ZusiPictureManager._log.Debug((object)str);
      overhangData = ZusiPictureManager._defaultOverhangData;
      return (ImageSource)this._nopic;
    }

    private ReadOnlyCollection<ZipArchiveEntry> GetPictures_impl()
    {
      if (!this.OpenArchive())
        return (ReadOnlyCollection<ZipArchiveEntry>)null;
      return this._zipArchive?.Entries;
    }

    public static string CreateFromDir(string directory, bool encrypted)
    {
      return Singleton<ZusiPictureManager>.Instance.CreateFromDir_impl(directory, encrypted);
    }

    public static new void Destroy()
    {
      if (!Singleton<ZusiPictureManager>.CheckInstance())
        return;
      Singleton<ZusiPictureManager>.Instance.Dispose();
    }

    public static void AllowAlternativeSources()
    {
      Singleton<ZusiPictureManager>.Instance.AllowAlternativeSources_impl();
    }

    public static void ForbidAlternativeSources()
    {
      Singleton<ZusiPictureManager>.Instance.ForbidAlternativeSources_impl();
    }

    public static ReadOnlyCollection<ZipArchiveEntry> GetPictures()
    {
      return Singleton<ZusiPictureManager>.Instance.GetPictures_impl();
    }

    public static ImageSource GetPicture(
      string name,
      string ls3_filename,
      int majorVariantId,
      int minorVariantId,
      System.Drawing.Size picsize,
      bool rotated,
      string cachefilename,
      string cachepath,
      out OverhangData overhangData,
      int objectsize = 0
      )
    {
      return Singleton<ZusiPictureManager>.Instance.GetPicture_impl(name, ls3_filename, majorVariantId, minorVariantId, picsize, rotated, cachefilename, cachepath, out overhangData, objectsize = objectsize);
    }

    private enum AssembleStrategy
    {
      Default,
      Alternative,
    }
  }
}
