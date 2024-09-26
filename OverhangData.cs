// Decompiled with JetBrains decompiler
// Type: ZusiPicLib.OverhangData
// Assembly: ZusiPicLib, Version=2.4.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 6843D1EB-8C04-48CA-81CF-1A8E2DBC0D7F
// Assembly location: D:\data\Development\ZUSI-Tools\_updated_sources\ZusiPicLib_0.0.0\ZusiPicLib.dll

using System;
using System.Windows;

#nullable disable
namespace ZusiPicLib
{
  [Serializable]
  public class OverhangData
  {
    private readonly int _front;
    private readonly int _rear;

    public int Front => this._front;

    public int Rear => this._rear;

    internal OverhangData()
    {
    }

    public OverhangData(int front, int rear)
    {
      this._front = front;
      this._rear = rear;
    }

    public Thickness GetThickness(bool flipped)
    {
      return !flipped ? new Thickness((double) this._front, 0.0, (double) this._rear, 0.0) : new Thickness((double) this._rear, 0.0, (double) this._front, 0.0);
    }

    public override string ToString()
    {
      return string.Format("Front: {0}, Rear: {1}", (object) this._front, (object) this._rear);
    }
  }
}
