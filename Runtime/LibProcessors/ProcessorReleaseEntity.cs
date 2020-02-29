using Source.Runtime;

namespace Pixeye.Actors
{
  public class ProcessorReleaseEntity : Processor<ComponentReleaseEntity>, ITick
  {
    public void Tick(float delta)
    {
      foreach (var entity in source)
      {
        for (int l = 0; l < groups.globalsLen; l++)
        {
          var group = groups.globals[l];
          if (group.HasEntity(entity, out var index))
            group.RemoveAt(index);
        }
      }

      source.length = 0;
    }
  }
}