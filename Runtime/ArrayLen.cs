using UnityEngine;

namespace Pixeye.Actors
{
  public class ArrayLen<T>
  {
    internal T[] source;
    internal int length;

    private const int defaultSize = 5;
    
    public ArrayLen()
    {
      source = new T[defaultSize];
      length = 0;
    }
    
    public ArrayLen(int count)
    {
      source = new T[count > 0 ? count : defaultSize];
      length = 0;
    }

//     public void Add(T obj)
//     {
// #if UNITY_EDITOR
//       if (length >= source.Length)
//       {
//         //TODO: Описание ошибки
//         Debug.Log("ERROR");
//         return;
//       }
// #endif
//       source[length++] = obj;
//     }
  }
}