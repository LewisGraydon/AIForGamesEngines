﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//A comparer that sorts int? null values to the back of a list, as oppsed to the front. Uses TotalCost to sort.
public class SortNullToBackByTotalCost : IComparer<INodeSearchable>
{
    public int Compare(INodeSearchable x, INodeSearchable y)
    {
        if(!x.TotalCost.HasValue)
        {
            if(!y.TotalCost.HasValue)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if(!y.TotalCost.HasValue)
            {
                return -1;
            }
            else
            {
                if (x.TotalCost > y.TotalCost)
                {
                    return 1;
                }
                else if (x.TotalCost < y.TotalCost)
                {
                    return -1;
                }

                else
                {
                    return 0;
                }
            }
        }
    }
}

