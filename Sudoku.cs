using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Sudoku
    {
        private int counter = 0;
        public int difficultyLvl { get; set; }
        public int[,] solution { get; } = new int[9, 9];
        public int[,] puzzle { get; }

        public Sudoku(int difficulty)
        {
            difficultyLvl = difficulty;
            solvePuzzle(solution);
            puzzle = (int[,])generatePuzzle(solution).Clone();
        }

        //public void sayHello(string name)
        //{
        //    int[,] square = new int[3, 3];
        //    copyArray(arr, square, 0, 6);
        //    Array.Copy(arr, 0, square, 0, 3);
        //    Array.Copy(arr, 9, square, 3, 3);
        //    Array.Copy(arr, 18, square, 6, 3);
        //    Console.Write(square.Length);

        //    for (int i = 0; i < 9; i++)
        //    {
        //        for (int j = 0; j < 9; j++)
        //        {
        //            Console.Write(solution[i, j]);
        //        }
        //        Console.WriteLine(" ");
        //    }

        //    Console.WriteLine("------------");

        //    for (int i = 0; i < 9; i++)
        //    {
        //        for (int j = 0; j < 9; j++)
        //        {
        //            Console.Write(puzzle[i, j]);
        //        }
        //        Console.WriteLine(" ");
        //    }



        //    for (int x = 0; x < 3; x++)
        //    {
        //        for (int y = 0; y < 3; y++)
        //        {
        //            Console.Write(square[x, y]);
        //        }
        //        Console.WriteLine(" ");
        //    }


        //    int[] row =
        //    Enumerable.Range(0, arr.GetLength(0))
        //        .Select(x => arr[x, 8])
        //        .ToArray();

        //    for (int x = 0; x < 9; x++)
        //    {
        //        Console.Write(row[x]);
        //    }
        //    Console.WriteLine(Enumerable.Range(0, arr.GetLength(0))
        //        .Select(x => arr[x, 8])
        //        .ToArray().Contains(2));

        //    Row
        //     Enumerable.Range(0, matrix.GetLength(1)).Select(x => matrix[rowNumber, x]).ToArray();

        //    Column
        //     Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, columnNumber]).ToArray();


        //}

        private int countLeft(int[,] arr)
        {
            int count = 0;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (arr[i, j] != 0)
                    {
                        count++;
                    }
                }

            }
            return count;
        }
        private void copyArray(int[,] a, int[,] b, int row, int start)
        {
            Array.Copy(a, start + row * 9, b, 0, 3);
            Array.Copy(a, start + 9 + row * 9, b, 3, 3);
            Array.Copy(a, start + 18 + row * 9, b, 6, 3);
        }

        private bool checkPuzzle(int[,] arr)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (arr[i, j] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool solvePuzzle(int[,] arr)
        {
            int row = 0;
            int col = 0;
            int[] values = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Random rng = new Random();
            int[] randomValue = values.OrderBy(x => rng.Next()).ToArray();
            for (int i = 0; i < 81; i++)
            {
                row = i / 9;
                col = i % 9;

                if (arr[row, col] == 0)
                {
                    foreach (int value in randomValue)
                    {
                        if (!Enumerable.Range(0, arr.GetLength(1)).Select(x => arr[row, x]).ToArray().Contains(value))
                        {
                            if (!Enumerable.Range(0, arr.GetLength(0)).Select(x => arr[x, col]).ToArray().Contains(value))
                            {
                                int[,] square = new int[3, 3];
                                if (row < 3)
                                {
                                    if (col < 3)
                                    {
                                        copyArray(arr, square, 0, 0);
                                    }
                                    else if (col < 6)
                                    {
                                        copyArray(arr, square, 0, 3);
                                    }
                                    else
                                    {
                                        copyArray(arr, square, 0, 6);
                                    }
                                }
                                else if (row < 6)
                                {
                                    if (col < 3)
                                    {
                                        copyArray(arr, square, 3, 0);
                                    }
                                    else if (col < 6)
                                    {
                                        copyArray(arr, square, 3, 3);
                                    }
                                    else
                                    {
                                        copyArray(arr, square, 3, 6);
                                    }
                                }
                                else
                                {
                                    if (col < 3)
                                    {
                                        copyArray(arr, square, 6, 0);
                                    }
                                    else if (col < 6)
                                    {
                                        copyArray(arr, square, 6, 3);
                                    }
                                    else
                                    {
                                        copyArray(arr, square, 6, 6);
                                    }
                                }
                                if (!Enumerable.Range(0, square.GetLength(0)).Select(x => square[x, 0]).ToArray().Contains(value) &&
                                !Enumerable.Range(0, square.GetLength(0)).Select(x => square[x, 1]).ToArray().Contains(value) &&
                                !Enumerable.Range(0, square.GetLength(0)).Select(x => square[x, 2]).ToArray().Contains(value))
                                {
                                    arr[row, col] = value;
                                    if (checkPuzzle(arr))
                                    {
                                        return true;
                                    }
                                    else if (solvePuzzle(arr))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
            arr[row, col] = 0;
            return false;
        }

        private bool countSolutions(int[,] arr)
        {
            int row = 0;
            int col = 0;
            int[] values = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Random rng = new Random();
            int[] randomValue = values.OrderBy(x => rng.Next()).ToArray();
            for (int i = 0; i < 81; i++)
            {
                row = i / 9;
                col = i % 9;

                if (arr[row, col] == 0)
                {
                    foreach (int value in randomValue)
                    {
                        if (!Enumerable.Range(0, arr.GetLength(1)).Select(x => arr[row, x]).ToArray().Contains(value))
                        {
                            if (!Enumerable.Range(0, arr.GetLength(0)).Select(x => arr[x, col]).ToArray().Contains(value))
                            {
                                int[,] square = new int[3, 3];
                                if (row < 3)
                                {
                                    if (col < 3)
                                    {
                                        copyArray(arr, square, 0, 0);
                                    }
                                    else if (col < 6)
                                    {
                                        copyArray(arr, square, 0, 3);
                                    }
                                    else
                                    {
                                        copyArray(arr, square, 0, 6);
                                    }
                                }
                                else if (row < 6)
                                {
                                    if (col < 3)
                                    {
                                        copyArray(arr, square, 3, 0);
                                    }
                                    else if (col < 6)
                                    {
                                        copyArray(arr, square, 3, 3);
                                    }
                                    else
                                    {
                                        copyArray(arr, square, 3, 6);
                                    }
                                }
                                else
                                {
                                    if (col < 3)
                                    {
                                        copyArray(arr, square, 6, 0);
                                    }
                                    else if (col < 6)
                                    {
                                        copyArray(arr, square, 6, 3);
                                    }
                                    else
                                    {
                                        copyArray(arr, square, 6, 6);
                                    }
                                }
                                if (!Enumerable.Range(0, square.GetLength(0)).Select(x => square[x, 0]).ToArray().Contains(value) &&
                                !Enumerable.Range(0, square.GetLength(0)).Select(x => square[x, 1]).ToArray().Contains(value) &&
                                !Enumerable.Range(0, square.GetLength(0)).Select(x => square[x, 2]).ToArray().Contains(value))
                                {
                                    arr[row, col] = value;
                                    if (checkPuzzle(arr))
                                    {
                                        counter += 1;
                                        break;
                                    }
                                    else if (countSolutions(arr))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
            arr[row, col] = 0;
            return false;
        }

        private int[,] generatePuzzle(int[,] a)
        {
            int level = difficultyLvl;
            int[,] clone = (int[,])solution.Clone();
            Random rng = new Random();
            int backup;
            while (countLeft(clone) > 7 * level)
            {
                int row = rng.Next(0, 9);
                int col = rng.Next(0, 9);
                while (clone[row, col] == 0)
                {
                    row = rng.Next(0, 9);
                    col = rng.Next(0, 9);
                }
                backup = clone[row, col];
                clone[row, col] = 0;
                counter = 0;
                int[,] copy = (int[,])clone.Clone();
                countSolutions(copy);
                if (counter != 1)
                {
                    clone[row, col] = backup;

                }
            }
            return clone;
        }
    }
}
