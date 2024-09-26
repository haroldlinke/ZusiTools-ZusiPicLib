// Decompiled with JetBrains decompiler
// Type: ZusiPicLib.Properties.Resources
// Assembly: ZusiPicLib, Version=2.4.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 6843D1EB-8C04-48CA-81CF-1A8E2DBC0D7F
// Assembly location: D:\data\Development\ZUSI-Tools\_updated_sources\ZusiPicLib_0.0.0\ZusiPicLib.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace ZusiPicLib.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ZusiPicLib.Properties.Resources.resourceMan == null)
          ZusiPicLib.Properties.Resources.resourceMan = new ResourceManager("ZusiPicLib.Properties.Resources", typeof (ZusiPicLib.Properties.Resources).Assembly);
        return ZusiPicLib.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ZusiPicLib.Properties.Resources.resourceCulture;
      set => ZusiPicLib.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap nopic
    {
      get
      {
        return (Bitmap) ZusiPicLib.Properties.Resources.ResourceManager.GetObject(nameof (nopic), ZusiPicLib.Properties.Resources.resourceCulture);
      }
    }
  }
}
