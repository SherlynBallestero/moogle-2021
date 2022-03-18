using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
namespace MoogleEngine
{


    public class HelperMethods
    {
        public string path;
        public HelperMethods(string path)
        {
            this.path = path;
        }

        ///<summary>
        ///método para dividir oraciones obteniendo cada palabra por separada 
        /// </summary>
        /// <param name="phrase">Frase para dividir en palabras.</param>
        public static string[] SplitPhraseInWords(string phrase)
        {
            string[] words = phrase.Split(' ');
            string w = "";
            for (int i = 0; i < words.Length; i++)
            {
                //normalizacion las palabras antes de guardarlas
                w = Regex.Replace(words[i].Normalize(System.Text.NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
                words[i] = w.ToLower();
            }
            return words;
        }

        ///<summary>
        ///metodo para buscar cierta palabra en el conjunto de documentos
        /// </summary>
        /// <param name="term">termino que se busca entre los documentos.</param>
        /// <param name="route">ruta de acceso a una serie de archivo(un documento)s entre los que buscar.</param>
        public static bool Find(string term, string route)
        {
            StreamReader str = new StreamReader(route);
            string line = str.ReadLine();
            string[] words;
            while (line != null)
            {
                words = SplitPhraseInWords(line);
                foreach (string word in words)
                {
                    if (word == term) return true;
                }
                line = str.ReadLine();

            }
            return false;
        }
        ///<summary>
        ///metodo para inicializar un array con todos sus elementos en cero
        /// </summary>
        public static double[] fillWithZeros(double[] a)
        {
            double[] answer = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                answer[i] = 0;
            }
            return answer;
        }
     
        ///<summary>
        ///Devuelve true si el parametro b se encuentra en el array de string a
        ///</summary>
        public static bool FindInArray(string[] a, string b)
        {
            if (!(a is null))
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (!(String.IsNullOrEmpty(a[i])) && a[i].Equals(b))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        ///<summary>
        ///metodo para eleiminar todos los elementos nulos o vacios de un array dado.
        /// </summary>
        public static string[] NullDelet(string[] a)
        {
            List<string> moment = new List<string>();
            string[] answer;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != " " && a[i] != "" && a[i] != null)
                {
                    moment.Add(a[i]);
                }

            }
            answer = moment.ToArray();
            return answer;
        }
        ///<summary>
        ///metodo para saber cuantas veces podemos encontrar un elemento en un array
        /// </summary>
        public static double CountWordInArray(string[] a, string b)
        {
            double count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b)
                {
                    count++;
                }
            }
            return count;
        }

        ///<summary>
        ///Metodo margesort para ordenar list<int,string>
        ///</summary> 
        public static void MargeSortToList(List<(int, string)> list)
        {
            MargeSortToList(list, 0, list.Count - 1);
        }
        public static void MargeSortToList(List<(int, string)> list, int start, int end)
        {
            //condicion de parada
            if (start == end) return;
            //buscando el medio de la lista de elementos
            int mit = (start + end) / 2;
            //ordenar1ra mitad y luego la segunda
            MargeSortToList(list, start, mit);
            MargeSortToList(list, mit + 1, end);
            Marge(list, start, mit, mit + 1, end);
            //copiar los elementos de aux a x
            list = new List<(int, string)>();
            list = CopyListToList(list);
        }
        ///<summary>
        ///Metodo auxiliar para el MargeSortToList
        ///</summary> 
        public static List<(int, string)> Marge(List<(int, string)> list, int start1, int end1, int start2, int end2)
        {
            int a = start1;
            int b = start2;
            List<(int, string)> answer = new List<(int, string)>();
            int t = (end1 - start1) + (end2 - start2);
            for (int i = 0; i < t; i++)
            {
                if (b != list.Count)
                {
                    if (a > end1 && b <= end2)
                    {
                        answer.Add(list[b]);
                        b++;
                    }
                    if (b > end2 && a <= end1)
                    {
                        answer.Add(list[a]);
                        a++;
                    }
                    if (b <= end2 && a <= end1)
                    {
                        if (list[a].Item1 > +list[b].Item1)
                        {
                            answer.Add(list[a]);
                            a++;
                        }
                        else
                        {
                            answer.Add(list[b]);
                            b++;
                        }
                    }

                }
                else
                {
                    if (a <= end1)
                    {
                        answer.Add(list[a]);
                        a++;
                    }
                }
            }
            return answer;
        }
        ///<summary>
        ///Metodo para copiar una lista en otra(empleado para evitar problemas de referencia)
        ///</summary> 
        public static List<(int, string)> CopyListToList(List<(int, string)> list)
        {
            List<(int, string)> answer = new List<(int, string)>();
            for (int i = 0; i < list.Count; i++)
            {
                answer[i] = list[i];
            }
            return answer;

        }
        ///<summary>
        ///Metodo para copiar una lista de string en una cadena.
        ///</summary> 
        public static string WordListToString(List<string> vect)
        {
            StringBuilder cad = new StringBuilder();

            for (int i = 0; i < vect.Count; i++)
            {
                if (i > 0) cad.Append(" ");
                cad.Append(vect[i]);
            }

            return cad.ToString();
        }
    }
}