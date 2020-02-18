﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface which implements a node for certain algorithmic searches.
public interface INodeSearchable
{
    bool searched
    {
        get;
        set;
    }

    int? Cost
    {
        get;
        set;
    }

    INodeSearchable parent
    {
        get;
        set;
    }
    List<INodeSearchable> children
    {
        get;
        set;
    }

}
