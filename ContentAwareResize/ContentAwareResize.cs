using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ContentAwareResize
{
    // ***************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // ***************
    public class ContentAwareResize
    {
        public struct coord
        {
            public int row;
            public int column;
        }
        //========================================================================================================
        //Your Code is Here:
        //===================
        /// <summary>
        /// Develop an efficient algorithm to get the minimum vertical seam to be removed
        /// </summary>
        /// <param name="energyMatrix">2D matrix filled with the calculated energy for each pixel in the image</param>
        /// <param name="Width">Image's width</param>
        /// <param name="Height">Image's height</param>
        /// <returns>BY REFERENCE: The min total value (energy) of the selected seam in "minSeamValue" & List of points of the selected min vertical seam in seamPathCoord</returns
        public static int modifiedMin(int first, int second, int third, int[,] path, int i, int j)
        {
            if (first <= second && first <= third)
            {
                path[i, j] = -1;
                return first;
            }
            if (second <= first && second <= third)
            {
                path[i, j] = 1;
                return second;
            }
            path[i, j] = 0;
            return third;
        }
        int FindPath(int[,] Mat, int W, int H, int x, int y, int[,] dp, int[,] path)
        {
            if (x + 1 == H)
                return Mat[x, y];
            if (dp[x, y] != Int32.MaxValue)
                return dp[x, y];

            int R1 = Int32.MaxValue;
            int R2 = Int32.MaxValue;

            if (y - 1 >= 0)
                R1 = FindPath(Mat, W, H, x + 1, y - 1, dp, path);
            if (y + 1 < W)
                R2 = FindPath(Mat, W, H, x + 1, y + 1, dp, path);
            int R3 = FindPath(Mat, W, H, x + 1, y, dp, path);
            int answer = modifiedMin(R1, R2, R3, path, x, y);


            return dp[x, y] = Mat[x, y] + answer;
        }
        public void CalculateSeamsCost(int[,] energyMatrix, int Width, int Height, ref int minSeamValue, ref List<coord> seamPathCoord)
        {
            seamPathCoord = new List<coord>();

            int start = -1;
            minSeamValue = Int32.MaxValue;
            int[,] dp = new int[Height, Width];
            int[,] path = new int[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    dp[i, j] = Int32.MaxValue;
                }
            }
            for (int k = 0; k < Width; k++)
            {
                int holder = FindPath(energyMatrix, Width, Height, 0, k, dp, path);
                if (holder < minSeamValue)
                {
                    minSeamValue = holder;
                    start = k;
                }
            }
            for (int i = 0; i < Height; i++)
            {
                int val = path[i, start];
                coord co = new coord();
                co.row = i;
                co.column = start + val;
                seamPathCoord.Add(co);
                start += val;
            }

        }

        #region DON'TCHANGETHISCODE
        public MyColor[,] _imageMatrix;
        public int[,] _energyMatrix;
        public int[,] _verIndexMap;
        public ContentAwareResize(string ImagePath)
        {
            _imageMatrix = ImageOperations.OpenImage(ImagePath);
            _energyMatrix = ImageOperations.CalculateEnergy(_imageMatrix);
            int _height = _energyMatrix.GetLength(0);
            int _width = _energyMatrix.GetLength(1);
        }
        public void CalculateVerIndexMap(int NumberOfSeams, ref int minSeamValueFinal, ref List<coord> seamPathCoord)
        {
            int Width = _imageMatrix.GetLength(1);
            int Height = _imageMatrix.GetLength(0);

            int minSeamValue = -1;
            _verIndexMap = new int[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    _verIndexMap[i, j] = int.MaxValue;

            bool[] RemovedSeams = new bool[Width];
            for (int j = 0; j < Width; j++)
                RemovedSeams[j] = false;

            for (int s = 1; s <= NumberOfSeams; s++)
            {
                CalculateSeamsCost(_energyMatrix, Width, Height, ref minSeamValue, ref seamPathCoord);
                minSeamValueFinal = minSeamValue;

                //Search for Min Seam # s
                int Min = minSeamValue;

                //Mark all pixels of the current min Seam in the VerIndexMap
                if (seamPathCoord.Count != Height)
                    throw new Exception("You selected WRONG SEAM");
                for (int i = Height - 1; i >= 0; i--)
                {
                    if (_verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column] != int.MaxValue)
                    {
                        string msg = "overalpped seams between seam # " + s + " and seam # " + _verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column];
                        throw new Exception(msg);
                    }
                    _verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column] = s;
                    //remove this seam from energy matrix by setting it to max value
                    _energyMatrix[seamPathCoord[i].row, seamPathCoord[i].column] = 100000;
                }

                //re-calculate Seams Cost in the next iteration again
            }
        }
        public void RemoveColumns(int NumberOfCols)
        {
            int Width = _imageMatrix.GetLength(1);
            int Height = _imageMatrix.GetLength(0);
            _energyMatrix = ImageOperations.CalculateEnergy(_imageMatrix);

            int minSeamValue = 0;
            List<coord> seamPathCoord = null;
            //CalculateSeamsCost(_energyMatrix,Width,Height,ref minSeamValue, ref seamPathCoord);
            CalculateVerIndexMap(NumberOfCols, ref minSeamValue, ref seamPathCoord);

            MyColor[,] OldImage = _imageMatrix;
            _imageMatrix = new MyColor[Height, Width - NumberOfCols];
            for (int i = 0; i < Height; i++)
            {
                int cnt = 0;
                for (int j = 0; j < Width; j++)
                {
                    if (_verIndexMap[i, j] == int.MaxValue)
                        _imageMatrix[i, cnt++] = OldImage[i, j];
                }
            }

        }
        #endregion
    }
}