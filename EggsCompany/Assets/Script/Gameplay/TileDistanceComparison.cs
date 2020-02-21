using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Something tells me this code smells bad but hopefully this is a temp implementation.
public class TileDistanceComparison : IComparer<INodeSearchable>
{
    public int Compare(INodeSearchable x, INodeSearchable y)
    {
        Tile testx;
        Tile testy;
        testx = x as Tile;
        testy = y as Tile;


        if (testx .distanceToTarget > testy.distanceToTarget)
        {
            return 1;
        }
        else if (testx.distanceToTarget < testy.distanceToTarget)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}


