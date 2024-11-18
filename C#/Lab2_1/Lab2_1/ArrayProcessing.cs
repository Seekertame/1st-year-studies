using System;
using System.Text;

namespace Lab2_1
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ArrayProcessing
    {
        internal static int[][] IntArrayGenerate1(int n)
        {
            if (!(0 < n && n < 20))
            {
                return new int[0][];
            }

            int[][] array = new int[n][];
            int k = 1;
            for (int i = 0; i < n; i++)
            {
                array[i] = new int[k];
                int value = -1 * (k - 1) / 2;
                for (int j = 0; j < k; j++)
                {
                    array[i][j] = value;
                    value += 1;
                    Console.Write($"{array[i][j]} ");
                }
                k += 2;
                Console.WriteLine();
            }

            return array;
        }

        internal static string FormattedPrint1(int[][] array)
        {
            if (array.GetUpperBound(0) == 0)
            {
                return "";
            }
            StringBuilder answer = new StringBuilder();
            
            int len = string.Join(" ", array[^1]).Length;
            for (int i = 0; i < array.Length; i++)
            {
                int lenLine = string.Join(" ", array[i]).Length;
                string spaces = new string(' ', (len - lenLine) / 2);
                string line = spaces + string.Join(" ", array[i]) + spaces;
                answer.Append(line + "\n");
            }

            return answer.ToString();
        }

        internal static int[][] IntArrayGenerate2(int n)
        {
            if (!(0 < n && n < 50))
            {
                return new int[0][];
            }

            int[][] array = new int[n][];
            for (int i = 0; i < n; i++)
            {
                array[i] = new int[i + 1];
                for (int j = 0; j < i + 1; j++)
                {
                    array[i][j] = j;
                    Console.Write($"{array[i][j]} ");
                }
                Console.WriteLine();
            }

            return array;
        }

        internal static string FormattedPrint2(int[][] array)
        {
            if (array.GetUpperBound(0) == 0)
            {
                return "";
            }

            StringBuilder answer = new StringBuilder();
            int maxLen = string.Join(" ", array[^1]).Length;

            for (int i = 0; i < array.Length; i++)
            {
                int curLen = string.Join(" ", array[i]).Length;
                string spaces = new string(' ', maxLen - curLen);
                answer.Append(spaces + string.Join(" ", array[i]) + "\n");
            }

            return answer.ToString();
        }
        
        internal static char[][] JaggedCharGen(int n)
        {
            if (!(0 < n && n < 25))
            {
                return new char[0][];
            }

            Random random = new Random(100);
            
            char[][] array = new char[n][];
            for (int i = 0; i < n; i++)
            {
                array[i] = new char[i + 2];
                for (int j = 0; j < i + 2; j++)
                {
                    array[i][j] = (char)('a' + random.Next(0, 26));
                }
            }

            return array;
        }

        internal static string FormattedPrint3(char[][] array)
        {
            if (array.GetUpperBound(0) == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                sb.Append(string.Join("", array[i]) + "\n");
            }

            return sb.ToString();
        }
        internal static void Main()
        {
            Console.Write(FormattedPrint1(IntArrayGenerate1(19)));
            //Console.Write(FormattedPrint2(IntArrayGenerate2(49)));
            //Console.Write(FormattedPrint3(JaggedCharGen(6)));
        }
    }
}