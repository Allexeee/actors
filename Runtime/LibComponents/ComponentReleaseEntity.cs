using System;
using Pixeye.Actors;
using UnityEngine;

namespace Source.Runtime
{
  public class ComponentReleaseEntity
  {
  }

  #region HELPERS

  public static partial class Component
  {
    public const string ReleaseEntity = "Pixeye.Source.ComponentReleaseEntity";

    public static ref ComponentReleaseEntity ComponentReleaseEntity(in this ent entity)
      => ref Storage<ComponentReleaseEntity>.components[entity.id];
  }

  internal sealed class StorageComponentReleaseEntity : Storage<ComponentReleaseEntity>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Setup() => Instance = new Storage<ComponentReleaseEntity>();

    public override ComponentReleaseEntity Create() => new ComponentReleaseEntity();

    public override void Dispose(indexes disposed)
    {
      foreach (int id in disposed)
      {
        ref var component = ref components[id];
      }
    }
  }

  #endregion
}