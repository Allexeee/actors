using Source.Runtime;

namespace Pixeye.Actors
{
  public class ProcessorRemoveComponents : Processor<ComponentRemoveComponents>, ITick
  {
    public void Tick(float delta)
    {
      foreach (var entity in source)
      {
        var entityId = entity.id;
        for (int l = 0; l < groups.globalsLen; l++)
        {
          var group = groups.globals[l];
          if (group.HasEntity(entity, out var index))
            if (!group.composition.OverlapComponents(entity))
              group.RemoveAt(index);
        }
      }

      source.length = 0;
    }
  }
}