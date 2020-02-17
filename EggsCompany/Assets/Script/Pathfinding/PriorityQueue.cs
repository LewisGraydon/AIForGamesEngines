using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//A specialized queue class that will be used for best first searches.
//A sorting algorithm will sort entries based on the provided heuristic.
public class PriorityQueue<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
{

    #region CONSTRUCTORS

    public PriorityQueue()
    {

    }

    public PriorityQueue(IEnumerable<T> collection)
    {

    }

    public PriorityQueue(int capacity)
    {

    }

    #endregion

    #region PROPERTIES

    public int Count { get; }

    public bool IsSynchronized => throw new NotImplementedException();

    public object SyncRoot => throw new NotImplementedException();

    #endregion




    public bool IsEmpty()
    {
        return true;

    }

    public void EnqueuePriority()
    {

    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    //public T DequeuePriority()
    //{
    //    return item;
    //}

}

