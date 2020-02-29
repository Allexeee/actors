using System;
using Pixeye.Actors;

namespace Source.Runtime
{
  public class ComponentAddComponents
  {
    public indexes componentsId = new indexes(12);
  }

  #region HELPERS

  public static partial class Component
  {
    public const string AddComponents = "Pixeye.Source.ComponentAddComponents";

    public static ref ComponentAddComponents ComponentAddComponents(in this ent entity)
      => ref Storage<ComponentAddComponents>.components[entity.id];
  }

  sealed class StorageComponentAddComponents : Storage<ComponentAddComponents>
  {
    public override ComponentAddComponents Create() => new ComponentAddComponents();

    public override void Dispose(indexes disposed)
    {
      foreach (int id in disposed)
      {
        ref var component = ref components[id];
        component.componentsId.Clear();
      }
    }
  }

  #endregion
}