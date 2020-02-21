using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//A comparer that sorts int? null values to the back of a list, as oppsed to the front.
public class SortNullToBack : IComparer<INodeSearchable>
{
    public int Compare(INodeSearchable x, INodeSearchable y)
    {
        if(x.Cost == null && y.Cost == null)
        {
            return 0;
        }
        else if (x.Cost == null)
        {
            return 1;
        }
        else if (y.Cost == null)
        {
            return -1;
        }

        if(x.Cost > y.Cost)
        {
            return 1;
        }
        else if (x.Cost < y.Cost)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}

