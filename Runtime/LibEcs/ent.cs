//  Project : ecs
// Contacts : Pix - info@pixeye.games
//     Date : 3/16/2019 


using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Pixeye.Actors
{
  [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
  public unsafe struct ent
  {
    //===============================//
    // Released entities
    //===============================//

    internal static ents entStack = new ents(Framework.Settings.SizeEntities);
    internal static int  size     = sizeof(ent);
    internal static int  lastID;

    //===============================//
    // Entity
    //===============================//
    public int id;

    public bool Exist
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => id > 0;
    }

    private ent(int value)
    {
      id = value;
    }

    public override int GetHashCode()
    {
      return ((id << 5) + id);
    }

    public override string ToString()
    {
      return id.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ent other)
    {
      return id == other.id;
    }

    public override bool Equals(object obj)
    {
      return obj is ent other && Equals(other);
    }

    //===============================//
    // Operators
    //===============================//
    public static implicit operator int(ent value)
    {
      return value.id;
    }

    public static implicit operator ent(int value)
    {
      ent e = new ent(value);
      return e;
    }

    public static bool operator ==(ent arg1, ent arg2)
    {
      return arg1.id == arg2.id;
    }

    public static bool operator !=(ent arg1, ent arg2)
    {
      return !(arg1 == arg2);
    }

    public void Add<T>(out T component) where T : class, new()
    {
      component = Storage<T>.components[id];
      if (component == null)
        component = Storage<T>.components[id] = new T();

      AddComponent(Storage<T>.componentId);

      Toolbox.Get<ProcessorAddComponents>().source.Insert(this);
    }

    public void Add<T>(T component) where T : class
    {
      Storage<T>.components[id] = component;

      AddComponent(Storage<T>.componentId);

      Toolbox.Get<ProcessorAddComponents>().source.Insert(this);
    }

    public void Remove<T>() where T : class
    {
      RemoveComponent(Storage<T>.componentId);
      Toolbox.Get<ProcessorRemoveComponents>().source.Insert(this);
    }

    public void Release()
    {
      RemoveComponentsAll();
      Toolbox.Get<ProcessorReleaseEntity>().source.Insert(this);
      id = 0;
    }

    private void AddComponent(int componentId)
    {
      Entity.Generations[id, Storage.Generations[componentId]] |= componentId;
      Entity.entities[id].Add(componentId);
    }

    private void RemoveComponent(int componentId)
    {
      Entity.Generations[id, Storage.Generations[componentId]] &= ~componentId;
      Entity.entities[id].Remove(componentId);

      Storage.All[componentId].toDispose.Add(id);
    }

    private void RemoveComponentsAll()
    {
      ref var entityCache = ref Entity.entities[id];

      for (int j = 0; j < entityCache.componentsAmount; j++)
      {
        var componentID = entityCache.componentsIds[j];
        var generation  = Storage.Generations[componentID];
        var mask        = Storage.Masks[componentID];

        Entity.Generations[id, generation] &= ~mask;
        Storage.All[entityCache.componentsIds[j]].toDispose.Add(id);
      }
    }

    //===============================//
    // Utils
    //===============================//

    [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has<T>()
    {
      return (Entity.Generations[id, Storage<T>.Generation] & Storage<T>.componentId) == Storage<T>.componentId;
    }

    [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has<T, Y>()
    {
      return Has<T>() && Has<Y>();
    }

    [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has<T, Y, U>()
    {
      return Has<T>() && Has<Y>() && Has<U>();
    }
  }
}