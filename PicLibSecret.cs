// Decompiled with JetBrains decompiler
// Type: ZusiPicLib.PicLibSecret
// Assembly: ZusiPicLib, Version=2.4.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 6843D1EB-8C04-48CA-81CF-1A8E2DBC0D7F
// Assembly location: D:\data\Development\ZUSI-Tools\_updated_sources\ZusiPicLib_0.0.0\ZusiPicLib.dll

using Sovoma;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZusiPicLib.Properties;

#nullable disable
namespace ZusiPicLib
{
  public sealed class PicLibSecret : Secret
  {
    public static readonly Secret Secret = (Secret) new PicLibSecret();

    private PicLibSecret()
      : base(PicLibSecret.CreateKey())
    {
    }

    //private static byte[] CreateKey()
    //{
    //  byte[] destination = new byte[32];
    //  Rectangle rect = new Rectangle(131, 57, 8, 1);
    //  using (Bitmap nopic = Resources.nopic)
    //  {
    //    BitmapData bitmapdata = nopic.LockBits(rect, ImageLockMode.ReadOnly, nopic.PixelFormat);
    //    Marshal.Copy(bitmapdata.Scan0, destination, 0, destination.Length);
    //    nopic.UnlockBits(bitmapdata);
    //  }
    //  return destination;
    //}

        private static byte[] CreateKey()
        {
            byte[] destination = new byte[32];
            Rectangle rect = new Rectangle(131, 57, 8, 1);
            using (Bitmap nopic = Resources.nopic)
            {
                BitmapData bitmapdata = nopic.LockBits(rect, ImageLockMode.ReadOnly, nopic.PixelFormat);
                try
                {
                    Marshal.Copy(bitmapdata.Scan0, destination, 0, destination.Length);
                }
                finally
                {
                    nopic.UnlockBits(bitmapdata);
                }
            }
            return destination;
        }



    }
}
