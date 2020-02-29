using System;
using Pixeye.Actors;

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

  sealed class StorageComponentReleaseEntity : Storage<ComponentReleaseEntity>
  {
    // public override ComponentReleaseEntity Create() => new ComponentReleaseEntity();

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