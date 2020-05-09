/*===============================================================
Product:    EntityEngine
Developer:  Dimitry Pixeye - info@pixeye,games
Company:    Homebrew
Date:       7/25/2018 11:49 AM
================================================================*/

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Pixeye.Actors
{
  [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
  public abstract class Storage
  {
    public static Storage[] All = new Storage[32];

    public readonly int ComponentId;
    public readonly int Generation;
    public readonly int ComponentMask;

    private static    int     _lastId;
    internal readonly indexes toDispose = new indexes(Framework.Settings.SizeEntities);

    internal abstract Type GetComponentType();

    internal static void DisposeSelf()
    {
      //	GroupInt

      for (int i = 0; i < _lastId; i++)
      {
        All[i].Dispose(All[i].toDispose);
        // All[i].toDispose.length = 0;
      }
    }

    public Storage()
    {
      if (_lastId == All.Length)
      {
        var l = _lastId << 1;
        Array.Resize(ref All, l);
      }

      ComponentId      = _lastId++;
      All[ComponentId] = this;

      ComponentMask = 1 << (ComponentId % 32);
      Generation    = ComponentId / 32;
    }

    internal virtual void Dispose(indexes disposed)
    {
      
    }
  }


  [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks | Option.DivideByZeroChecks, false)]
  public class Storage<T> : Storage where T: class, new()
  {
    public static readonly Storage<T> Instance = new Storage<T>();

    public static readonly T[] Components = new T[Framework.Settings.SizeEntities];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual T Create()
    {
      return new T();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override Type GetComponentType()
    {
      return typeof(T);
    }

    internal override void Dispose(indexes disposed)
    {
      foreach (var index in disposed)
      {
        Components[index] = null;
      }
    }
  }
}