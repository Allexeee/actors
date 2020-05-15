//  Project : ecs
// Contacts : Pix - info@pixeye.games
//     Date : 3/16/2019 


using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Pixeye.Actors
{
  [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
  public unsafe class ent: IComparable<ent>
  {
    //===============================//
    // Released entities
    //===============================//

    internal static Stack<ent> entStack = new Stack<ent>(Framework.Settings.SizeEntities);

    // internal static int  size     = sizeof(ent);
    internal static int lastID;

    //===============================//
    // Entity
    //===============================//
    public int id;

    public indexes wasInGroupAdded = new indexes();
    public indexes wasInGroupRemoved = new indexes();

    // public bool Exist
    // {
    //   [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //   get => id > 0;
    // }

    public ent()
    {
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
    
    public int CompareTo(ent other)
    {
      if (other.id > id) return 1;
      if (other.id < id) return -1;
      return 0;
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

    public void AddGet<T>(out T component) where T : class, new()
    {
      component = Storage<T>.Components[id];
      if (component == null)
      {
        component = Storage<T>.Components[id] = new T();
        ProcessorUpdateGroups.SourceAdd.Add(this);
        AddComponent(Storage<T>.Instance.ComponentId);
      }
    }

    public void Add<T>(T component) where T : class, new()
    {
      Storage<T>.Components[id] = component;

      AddComponent(Storage<T>.Instance.ComponentId);

      ProcessorUpdateGroups.SourceAdd.Add(this);
    }

    public void Remove<T>() where T : class, new()
    {
      RemoveComponent(Storage<T>.Instance.ComponentId);
      ProcessorUpdateGroups.SourceRemove.Add(this);
    }

    public void Release()
    {
      // RemoveComponentsAll();
      // entStack.Add(this);
      // debug.log($"Release {id}");
      ProcessorUpdateGroups.SourceRemove.Add(this);
      ProcessorUpdateGroups.SourceRelease.Add(this);

      // Toolbox.Get<ProcessorReleaseEntity>().source.Insert(this);
      // id = 0;
    }

    private void AddComponent(int componentId)
    {
      Entity.Generations[id, Storage.All[componentId].Generation] |= Storage.All[componentId].ComponentMask;
      Entity.entities[id].Add(componentId);
    }

    private void RemoveComponent(int componentId)
    {
      Entity.Generations[id, Storage.All[componentId].Generation] &= ~Storage.All[componentId].ComponentMask;
      Entity.entities[id].Remove(componentId);

      // Storage.All[componentId].toDispose.Add(id);
    }

    private void RemoveComponentsAll()
    {
      ref var entityCache = ref Entity.entities[id];

      for (int j = 0; j < entityCache.componentsAmount; j++)
      {
        RemoveComponent(entityCache.componentsIds[j]);
        // var componentID = entityCache.componentsIds[j];
        // var generation  = Storage.Generations[componentID];
        // var mask        = Storage.Masks[componentID];
        //
        // Entity.Generations[id, generation] &= ~mask;
        // Storage.All[entityCache.componentsIds[j]].toDispose.Add(id);
      }
    }

    //===============================//
    // Utils
    //===============================//

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has(int componentId)
    {
      return (Entity.Generations[id, Storage.All[componentId].Generation] & Storage.All[componentId].ComponentMask) == Storage.All[componentId].ComponentMask;
    }

    [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has<T>()
      where T : class, new()
    {
      return (Entity.Generations[id, Storage<T>.Instance.Generation] & Storage<T>.Instance.ComponentMask) == Storage<T>.Instance.ComponentMask;
    }

    [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has<T, Y>()
      where T : class, new()
      where Y : class, new()
    {
      return Has<T>() && Has<Y>();
    }

    [Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has<T, Y, U>()
      where T : class, new()
      where Y : class, new()
      where U : class, new()
    {
      return Has<T>() && Has<Y>() && Has<U>();
    }
  }

  public static class EntityHelper
  {
    public static bool Exist(this ent entity)
    {
      return !ReferenceEquals(entity, null) && entity.id != 0;
    }
  }
}