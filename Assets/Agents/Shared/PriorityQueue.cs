using System.Collections.Generic;
using System;

/// <summary>
/// Implements a Priority Queue because target Dot net version does not have one
/// </summary>
/// <typeparam name="TElement">Type of Element</typeparam>
/// <typeparam name="IComparable<P>">Type of Priority</typeparam>
public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private readonly List<Tuple<TElement, TPriority>> queue;

    public int Count { get { return queue.Count; } }

    public PriorityQueue()
    {
        queue = new List<Tuple<TElement, TPriority>>();
    }

    /// <summary>
    /// Adds an element to the queue with a priority
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="priority">Item's priority</param>
    public void Enqueue(TElement item, TPriority priority)
    {
        // add to queue
        queue.Add(new Tuple<TElement, TPriority>(item, priority));
        
        // reorder
        int index = Count - 1;
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (queue[parentIndex].Item2.CompareTo(queue[index].Item2) > 0)
            {
                Swap(parentIndex, index);
                index = parentIndex;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Returns the heighest priority Item
    /// </summary>
    /// <returns>Highest priority item</returns>
    public TElement Dequeue()
    {
        if (Count == 0)
            throw new InvalidOperationException("Priority queue is empty.");

        // get item
        TElement item = queue[0].Item1;
        queue[0] = queue[Count - 1];
        queue.RemoveAt(Count - 1);

        // reorder
        int index = 0;
        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestChildIndex = index;

            if (leftChildIndex < Count && queue[leftChildIndex].Item2.CompareTo(queue[smallestChildIndex].Item2) < 0)
                smallestChildIndex = leftChildIndex;
            if (rightChildIndex < Count && queue[rightChildIndex].Item2.CompareTo(queue[smallestChildIndex].Item2) < 0)
                smallestChildIndex = rightChildIndex;

            if (smallestChildIndex != index)
            {
                Swap(index, smallestChildIndex);
                index = smallestChildIndex;
            }
            else
            {
                break;
            }
        }

        return item;
    }

    public bool IsEmpty()
    {
        return Count == 0;
    }

    private void Swap(int index1, int index2)
    {
        Tuple<TElement, TPriority> temp = queue[index1];
        queue[index1] = queue[index2];
        queue[index2] = temp;
    }
}

