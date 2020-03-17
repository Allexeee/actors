using System;
using Pixeye.Actors;
using UnityEngine;

namespace Source.Runtime
{
  public class ComponentRemoveComponents
  {
  }

  #region HELPERS

  public static partial class Component
  {
    public const string RemoveComponents = "Pixeye.Source.ComponentRemoveComponents";

    public static ref ComponentRemoveComponents ComponentRemoveComponents(in this ent entity)
      => ref Storage<ComponentRemoveComponents>.components[entity.id];
  }

  internal sealed class StorageComponentRemoveComponents : Storage<ComponentRemoveComponents>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Setup() => Instance = new Storage<ComponentRemoveComponents>();

    public override ComponentRemoveComponents Create() => new ComponentRemoveComponents();

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