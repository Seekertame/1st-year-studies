// Бюрчиев Тимур Зольванович, БПИ 248-1, вариант 11
// https://disk.yandex.ru/d/tu2XoYE5redafA
using System;
using System.Text;
namespace Lab2_2
{
    /// <summary>
    /// Содержит методы для решения задачи.
    /// </summary>
    public class CharJaggedArray
    {
        /// <summary>
        /// _arr - ссылка на массив символьных массивов
        /// </summary>
        private char[][] _arr;
        
        /// <summary>
        /// Связывает _arr с массивом символьных массивов, состоящим из 5 символьных массивов
        /// </summary>
        private void CreateCj()
        {
            _arr = new char[5][];
            for (int i = 0; i < 5; i++)
            {
                _arr[i] = new char[5];
                for (int j = 0; j < 5; j++)
                {
                    _arr[i][j] = i + j >= 4 ? 'X' : 'O';
                }
            }
        }
        /// <summary>
        /// Связывает _arr с массивом символьных массивов, состоящим из N символьных массивов
        /// </summary>
        /// <param name="n">Размерность массива</param>
        private void CreateCj(int n)
        {
            _arr = new char[n][];
            for (int i = 0; i < n; i++)
            {
                _arr[i] = new char[n];
                for (int j = 0; j < n; j++)
                {
                    _arr[i][j] = i + j >= n - 1 ? 'X' : 'O';
                }
            }
        }
        /// <summary>
        /// Возвращает строку с отформатированным представлением структуры данных arr
        /// </summary>
        /// <returns>Строка с отформатированным представлением структуры данных arr</returns>
        public string ArrayToString()
        {
            if (_arr == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _arr.Length; i++)
            {
                sb.Append(string.Join(" ", _arr[i]));
                if (i != _arr.Length - 1)
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString().Trim();
        }
        /// <summary>
        /// Конструктов без параметров
        /// </summary>
        public CharJaggedArray()
        {
            CreateCj();
        }
        /// <summary>
        /// Конструктор с параметром
        /// </summary>
        /// <param name="n">Размерность массива.</param>
        public CharJaggedArray(int n)
        {
            if (n > 0 && n < 17)
            {
                CreateCj(n);
            }
            else
            {
                CreateCj();
            }
        }
    }
    public class Program
    {
        public static void Main()
        {
            CharJaggedArray array5 = new CharJaggedArray();
            Console.WriteLine(array5.ArrayToString());

            CharJaggedArray array7 = new CharJaggedArray(16);
            Console.WriteLine(array7.ArrayToString());

            CharJaggedArray arrayInvalid = new CharJaggedArray(17);
            Console.WriteLine(arrayInvalid.ArrayToString());
        }
    }
}