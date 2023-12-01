using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.T.Until
{
    public class NaturalSortComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return NaturalCompare.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class NaturalCompare
    {
        public static int Compare(string x, string y, StringComparison comparison)
        {
            var partsX = x.Split(Path.DirectorySeparatorChar);
            var partsY = y.Split(Path.DirectorySeparatorChar);

            for (int i = 0; i < Math.Min(partsX.Length, partsY.Length); i++)
            {
                int result = ComparePart(partsX[i], partsY[i], comparison);
                if (result != 0)
                {
                    return result;
                }
            }

            return partsX.Length.CompareTo(partsY.Length);
        }

        private static int ComparePart(string x, string y, StringComparison comparison)
        {
            int indexX = 0, indexY = 0;
            while (indexX < x.Length && indexY < y.Length)
            {
                if (char.IsDigit(x[indexX]) && char.IsDigit(y[indexY]))
                {
                    // Compare numbers numerically
                    int numX = 0;
                    while (indexX < x.Length && char.IsDigit(x[indexX]))
                    {
                        numX = numX * 10 + (x[indexX] - '0');
                        indexX++;
                    }

                    int numY = 0;
                    while (indexY < y.Length && char.IsDigit(y[indexY]))
                    {
                        numY = numY * 10 + (y[indexY] - '0');
                        indexY++;
                    }

                    int numComparison = numX.CompareTo(numY);
                    if (numComparison != 0)
                    {
                        return numComparison;
                    }
                }
                else
                {
                    // Compare strings
                    int stringComparison = string.Compare(x, indexX, y, indexY, 1, comparison);
                    if (stringComparison != 0)
                    {
                        return stringComparison;
                    }
                }

                indexX++;
                indexY++;
            }

            return 0;
        }
    }
}
