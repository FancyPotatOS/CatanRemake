using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake
{
    public class HexagonGrid<THex, TEdge, TCorner>
    {
        public readonly int sideLength;
        public readonly int arrSize;
        public readonly int[][] range;

        /*  Pixels to move to place correctly in position   */
        public static readonly Point DOWN = new Point(2, 12);
        public static readonly Point DOWNLEFT = new Point(-13, 6);
        public static readonly Point DOWNRIGHT = new Point(15, 6);
        public static readonly Point UP = new Point(-2, -12);
        public static readonly Point UPLEFT = new Point(-15, -6);
        public static readonly Point UPRIGHT = new Point(13, -6);

        public static readonly Point UPLEFTPOINT = new Point(-6, -6);
        public static readonly Point UPRIGHTPOINT = new Point(4, -5);

        public static readonly Point UPEDGE = new Point(-1, -6);
        public static readonly Point UPRIGHTEDGE = new Point(8, -3);
        public static readonly Point UPLEFTEDGE = new Point(-8, -3);

        /// <summary>
        /// All edges of a hexagon
        /// </summary>
        public enum Edges
        {
                        UP, 
            UPLEFT,            UPRIGHT, 

            DOWNLEFT,          DOWNRIGHT,
                        DOWN,
        }

        /// <summary>
        /// All corners of a hexagon
        /// </summary>
        public enum Corners
        {
                UPLEFT,     UPRIGHT,

           LEFT,                     RIGHT,

                DOWNLEFT,   DOWNRIGHT
        }

        THex[,] data;

        TEdge[,][] edgeData;

        TCorner[,][] cornerData;

        /// <summary>
        /// Create a perfect hexagonal structure with a side of length sL
        /// </summary>
        /// <param name="sL"></param>
        public HexagonGrid(int sL)
        {
            // Length of hexagon's side
            sideLength = sL;

            // Size of both dimensions of array
            arrSize = sideLength * 2 - 1;

            // Fill range for each index
            range = new int[arrSize][];
            for (int i = 0; i < arrSize; i++)
            {
                if (i < sideLength)
                {
                    range[i] = new int[] { 0, sideLength + i - 1 };
                }
                else
                {
                    range[i] = new int[] { i - sideLength + 1, 2 * sideLength - 2 };
                }
            }

            // Initialize array
            data = new THex[arrSize, arrSize];
            edgeData = new TEdge[arrSize, arrSize][];
            cornerData = new TCorner[arrSize, arrSize][];
            for (int i = 0; i < arrSize; i++)
            {
                for (int j = 0; j < arrSize; j++)
                {
                    if (InRange(i, j))
                    {
                        edgeData[i, j] = new TEdge[6];
                        cornerData[i, j] = new TCorner[6];
                    }
                }
            }
        }

        /// <summary>
        /// Set item on hex (i, j)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void SetAt(THex item, int i, int j)
        {
            if (!InRange(i, j))
            {
                throw new Exception("(" + i + ", " + j + ") is not in range for the hexagon!");
            }
            else
            {
                data[i, j] = item;
            }
        }

        /// <summary>
        /// Set item on specified edge of hex (i, j)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="edge"></param>
        public void SetAtEdge(TEdge item, int i, int j, Edges edge)
        {
            if (!InRange(i, j))
            {
                throw new Exception("(" + i + ", " + j + ") is not in range for the hexagon!");
            }
            else
            {
                // Set data
                edgeData[i, j][(int)edge] = item;

                // Set other side of edge's hex's data
                if (edge == Edges.UP)
                {
                    if (InRange(i, j + 1))
                    {
                        edgeData[i, j + 1][(int)Edges.DOWN] = item;
                    }
                }
                else if (edge == Edges.DOWN)
                {
                    if (InRange(i, j - 1))
                    {
                        edgeData[i, j - 1][(int)Edges.DOWN] = item;
                    }
                }
                else if (edge == Edges.UPRIGHT)
                {
                    if (InRange(i + 1, j + 1))
                    {
                        edgeData[i + 1, j + 1][(int)Edges.DOWNLEFT] = item;
                    }
                }
                else if (edge == Edges.DOWNLEFT)
                {
                    if (InRange(i - 1, j - 1))
                    {
                        edgeData[i - 1, j - 1][(int)Edges.UPRIGHT] = item;
                    }
                }
                else if (edge == Edges.UPLEFT)
                {
                    if (InRange(i - 1, j))
                    {
                        edgeData[i - 1, j][(int)Edges.DOWNRIGHT] = item;
                    }
                }
                else if (edge == Edges.DOWNRIGHT)
                {
                    if (InRange(i + 1, j))
                    {
                        edgeData[i + 1, j][(int)Edges.UPLEFT] = item;
                    }
                }
            }
        }

        /// <summary>
        /// Set item on specified corner of hex (i, j)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="corner"></param>
        public void SetAtCorner(TCorner item, int i, int j, Corners corner)
        {
            if (!InRange(i, j))
            {
                throw new Exception("(" + i + ", " + j + ") is not in range for the hexagon!");
            }
            else
            {
                // Set data
                cornerData[i, j][(int)corner] = item;

                // Set other side of edge's hex's data
                if (corner == Corners.UPRIGHT)
                {
                    if (InRange(i, j + 1))
                    {
                        cornerData[i, j + 1][(int)Corners.DOWNRIGHT] = item;
                    }
                    if (InRange(i + 1, j + 1))
                    {
                        cornerData[i + 1, j + 1][(int)Corners.LEFT] = item;
                    }
                }
                else if (corner == Corners.RIGHT)
                {
                    if (InRange(i + 1, j + 1))
                    {
                        cornerData[i + 1, j + 1][(int)Corners.DOWNLEFT] = item;
                    }
                    if (InRange(i + 1, j))
                    {
                        cornerData[i + 1, j][(int)Corners.UPLEFT] = item;
                    }
                }
                else if (corner == Corners.DOWNRIGHT)
                {
                    if (InRange(i + 1, j))
                    {
                        cornerData[i + 1, j][(int)Corners.LEFT] = item;
                    }
                    if (InRange(i, j - 1))
                    {
                        cornerData[i, j - 1][(int)Corners.UPRIGHT] = item;
                    }
                }
                else if (corner == Corners.DOWNLEFT)
                {
                    if (InRange(i, j - 1))
                    {
                        cornerData[i, j - 1][(int)Corners.UPLEFT] = item;
                    }
                    if (InRange(i - 1, j - 1))
                    {
                        cornerData[i - 1, j - 1][(int)Corners.RIGHT] = item;
                    }
                }
                else if (corner == Corners.LEFT)
                {
                    if (InRange(i - 1, j - 1))
                    {
                        cornerData[i - 1, j - 1][(int)Corners.UPRIGHT] = item;
                    }
                    if (InRange(i - 1, j))
                    {
                        cornerData[i - 1, j][(int)Corners.DOWNRIGHT] = item;
                    }
                }
                else if (corner == Corners.UPLEFT)
                {
                    if (InRange(i - 1, j))
                    {
                        cornerData[i - 1, j][(int)Corners.RIGHT] = item;
                    }
                    if (InRange(i, j + 1))
                    {
                        cornerData[i, j + 1][(int)Corners.DOWNLEFT] = item;
                    }
                }
            }
        }

        /// <summary>
        /// Get item at hex (i, j)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public THex GetAt(int i, int j)
        {
            if (!InRange(i, j))
            {
                throw new Exception("(" + i + ", " + j + ") is not in range for the hexagon!");
            }
            else
            {
                return data[i, j];
            }
        }

        /// <summary>
        /// Get item on the specified edge of hex (i, j)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public TEdge GetAtEdge(int i, int j, Edges edge)
        {
            if (!InRange(i, j))
            {
                throw new Exception("(" + i + ", " + j + ") is not in range for the hexagon!");
            }
            else
            {
                return edgeData[i, j][(int)edge];
            }
        }

        /// <summary>
        /// Gets item on the specifed corner of hex (i, j)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="corner"></param>
        /// <returns></returns>
        public TCorner GetAtCorner(int i, int j, Corners corner)
        {
            if (!InRange(i, j))
            {
                throw new Exception("(" + i + ", " + j + ") is not in range for the hexagon!");
            }
            else
            {
                return cornerData[i, j][(int)corner];
            }
        }

        /// <summary>
        /// Gets the index in a specified direction from a given index
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="direction">The edge that the given hex and the target hex share</param>
        /// <returns>Index of the hex</returns>
        public int[] GetInDirection(int i, int j, Edges direction)
        {
            // Organized by pairs of inverse
            if (direction == Edges.DOWN)
                return new int[] { i, j - 1 };
            else if (direction == Edges.UP)
                return new int[] { i, j + 1 };
            else if (direction == Edges.UPRIGHT)
                return new int[] { i + 1, j + 1 };
            else if (direction == Edges.DOWNLEFT)
                return new int[] { i - 1, j - 1 };
            else if (direction == Edges.UPLEFT)
                return new int[] { i - 1, j };
            else if (direction == Edges.DOWNRIGHT)
                return new int[] { i + 1, j };
            else
                throw new Exception("Direction given was not in the Edges enum!");
        }

        /// <summary>
        /// Get UPRIGHT/DOWN magnitudes to get to (i, j) hex
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int[] GetURD(int i, int j)
        {
            // (i, i) - (0, j)
            return new int[] { i, i-j };
        }

        /// <summary>
        /// Count the number of hexes
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            int count = 0;

            for (int i = 0; i < sideLength; i++)
            {
                count += range[i][1] - range[i][0];
            }

            return count;
        }

        /// <summary>
        /// Whether hex (i, j) is in range of the hexagon
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public bool InRange(int i, int j)
        {
            // Out of range of range array
            if (i < 0 || arrSize <= i || j < 0 || arrSize <= j)
                return false;
            // Out of range for hexagon
            else if (i < range[j][0] || 
                    range[j][1] < i || 
                    j < range[i][0] || 
                    range[i][1] < j)
                return false;
            else
                return true;
        }
    }
}
