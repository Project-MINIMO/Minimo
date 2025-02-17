using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private readonly List<(T item, float priority)> _elements = new();

    public int Count => _elements.Count;

    public void Enqueue(T item, float priority)
    {
        _elements.Add((item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 1; i < _elements.Count; i++)
        {
            if (_elements[i].priority < _elements[bestIndex].priority)
            {
                bestIndex = i;
            }
        }

        T bestItem = _elements[bestIndex].item;
        _elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public T Find(Func<T, bool> predicate)
    {
        foreach (var element in _elements)
        {
            if (predicate(element.item))
            {
                return element.item;
            }
        }
        return default;
    }
}