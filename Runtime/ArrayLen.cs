using UnityEngine;

namespace Pixeye.Actors
{
  public class ArrayLen<T>
  {
    private T[] source;
    private int length;

    public ArrayLen(int count)
    {
      source = new T[count];
    }

    public void Add(T obj)
    {
#if UNITY_EDITOR
      if (length >= source.Length)
      {
        //TODO: Описание ошибки
        Debug.Log("ERROR");
        return;
      }
#endif
      source[length++] = obj;
    }
  }
}