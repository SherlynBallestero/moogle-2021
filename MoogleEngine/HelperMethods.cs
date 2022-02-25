using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
namespace MoogleEngine
{
    

    public class HelperMethods 
    {
        public string path;
        public  HelperMethods(string path)
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
        public  static bool Find(string term, string route)
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
        ///metodo para dejar en las palabras solo numeros y letras,quitando signos de puntuacion y normalizando las palabras
        /// </summary>
        public static string[] TokenWords(string phrase)
        {
            string[] words = phrase.Split(' ');
            string[] newWords = new string[words.Length];
            int index = 0;
            string w = "";
            bool change = false;
            foreach (string word in words)
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (char.IsLetter(word[i]) || char.IsNumber(word[i]))
                    {
                        w += word[i];
                    }
                    else
                    {
                        //change se hace false cuando hubo un caso en q se encontro con un char que no es letra
                        change = true;
                    }
                }
                w = Regex.Replace(w.Normalize(System.Text.NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
                newWords[index] = w;
                w = "";
                index++;
                change = false;
            }
            return newWords;
        }
        ///<summary>
        ///metodo para concatenar dos arrays,te devuelve un nuevo array con la union de los dos arrays que se le pasan como entrada en dodne ningun elemento estara repetido y no habrán elementos nulos o espacios vacios.
        /// </summary>
        public static string[] Concat(string[] a, string[] b)
        {
            string[] c = { " " };
            string[] concated = new string[a.Length + b.Length];

            if (a is null)
            {
                if (b is null)
                {
                    return c;
                }
                else
                {
                    return b;
                }
            }
            else if (b is null) return a;
            for (int i = 0, j = 0; i < concated.Length; i++)
            {
                if (i < a.Length)
                {
                    if (!FindInArray(concated, a[i]) && !(String.IsNullOrEmpty(a[i])))
                        concated[i] = a[i];
                }
                else
                {
                    if (!FindInArray(concated, b[j]) && !(String.IsNullOrEmpty(b[j])))
                        concated[i] = b[j];
                    j++;
                }
            }
            concated = NullDelet(concated);
            return concated;
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
        ///metodo para ordenar un array de mayor a menor
        /// </summary>
        ///<param name="index">en index obtendremos los indices ordenados luego de cambiar los elementos del array de lugar,es decir si cambiamos los elementos de un array en las posiciones 2 y 1 obtendremos index={0,2,1}  </param>
        public  double[] Ordenar(double[] a, out double[] index)
        {
            index = new double[a.Length];
            double[] aux = new double[a.Length];
            double temp = 0;
            for (int i = 0; i < index.Length; i++)
            {
                aux[i] = a[i];
                index[i] = i;
            }
            //metodo burbuja de ordenación de array          
            for (int i = 0; i < a.Length - 1; i++)
                for (int j = i + 1; j < a.Length; j++)
                {
                    if (a[i] < a[j])
                    {
                        temp = a[i];
                        a[i] = a[j];
                        a[j] = temp;
                    }

                }
            //obtención del indice correspondiente a cada elemento que fue cambiado en el orden para poder obtener el archivo corresp[ondiente a cada score]
            for (int i = 0; i < a.Length; i++)
                for (int j = i; j < a.Length; j++)
                {
                    if (a[i] == aux[j])
                    {
                        temp = index[i];
                        index[i] = index[j];
                        index[j] = temp;

                        break;
                    }
                }
            return a;
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
                if (a[i] != " " && a[i] != null)
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
        ///Metodo para concatenar dos arrays de enteros
        ///</summary>
        public static int[] ConcatInt(int[] a,int[] b)
        {
            int[] concated=new int[a.Length+b.Length];
            int[]c={-1};
            //creo que no debe haber ningun problema con eso pq no tiene pq llegar el caso en que aux sea 
            //{-1} dado que se le pasan arrays de int con las posiciones en que se encuentra una palabra 
            //dada para cierto doc,hay que ver cuando esa palabra no se encuentra en el documento que es
            // lo q me da en las posiciones,en el caso de que obtenga -1 habria que ver no vaya a ser que mande a indexar en el documento en menos 1
            
             if (a is null)
            {
                if (b is null)
                {
                    return c;
                }
                else
                {
                    return b;
                }
            }
            else if (b is null) return a;
            for (int i = 0, j = 0; i < concated.Length; i++)
            {
                if (i < a.Length)
                {
                        concated[i] = a[i];
                }
                else
                {
                        concated[i] = b[j];
                    j++;
                }
            }
            return concated;
        }
        ///<summary>
        ///En este proceso se obtiene en un array cada palabra que existe en total de todos los documentos sin 
        ///repeticion y asegurando que no hayan espacios en blanco o elementos null.
        ///</summary>   
        public static string[] WordsInCollection(string route)
        {
            
            string[] file = Directory.GetFiles(route, "*.txt");
            string[] words = { " " };
            string[] aux = { " " };
            for (int i = 0; i < file.Length; i++)
            {
                string line = System.IO.File.ReadAllText(file[i]);
                aux = TokenWords(line);
                words = Concat(words, aux);

            }
            words = NullDelet(words);
            return words;
        }
        ///<summary>
        ///Metodo margesort para ordenar list<int,string>
        ///</summary> 
        public static void MargeSortToList(List<(int,string)> list)
        {
            MargeSortToList(list,0,list.Count-1);
        }
        public static void MargeSortToList(List<(int,string)> list,int start, int end)
        {
            //condicion de parada
            if(start==end)return;
            //buscando el medio de la lista de elementos
            int mit=(start+end)/2;
            //ordenar1ra mitad y luego la segunda
            MargeSortToList(list,start,mit);
            MargeSortToList(list,mit+1,end);
            Marge(list,start,mit,mit+1,end);
            //copiar los elementos de aux a x
            list=new List<(int, string)>();          
            list=CopyListToList(list);
        } 
        public static List<(int,string)> Marge(List<(int,string)> list,int start1,int end1,int start2,int end2)
        {
            int a=start1;
            int b=start2;
            List<(int,string)> answer=new List<(int, string)>();
            int t=(end1-start1)+(end2-start2);
            for(int i=0;i<t;i++)
            {
                if(b!=list.Count)
                {
                    if(a>end1 && b<=end2)
                    {
                        answer.Add(list[b]);
                        b++;
                    }
                    if(b>end2 && a<=end1)
                    {
                        answer.Add(list[a]);
                        a++;
                    }
                          if(b<=end2 && a<=end1)
                    {
                        if(list[a].Item1>+list[b].Item1)
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
                    if(a<=end1)
                    {
                        answer.Add(list[a]);
                        a++;
                    }
                }
            }
            return answer;
        }
        public  static List<(int,string)> CopyListToList(  List<(int,string)> list)
        {
            List<(int,string)>answer=new List<(int, string)>();
            for(int i=0;i<list.Count;i++)
            {
                answer[i]=list[i];
            }
            return answer;
            
            
        }
        ///<summary>
        ///Metodo margesort para ordenar list<double,string>
        ///</summary> 
        public static void MargeSortToListDouble(List<(double,string)> list)
        {
            MargeSortToListDouble(list,0,list.Count-1);
        }
        public static void MargeSortToListDouble(List<(double,string)> list,int start, int end)
        {
            //condicion de parada
            if(start==end)return;
            //buscando el medio de la lista de elementos
            int mit=(start+end)/2;
            //ordenar1ra mitad y luego la segunda
            MargeSortToListDouble(list,start,mit);
            MargeSortToListDouble(list,mit+1,end);
            MargeDouble(list,start,mit,mit+1,end);
            //copiar los elementos de aux a x
            list=new List<(double, string)>();          
            list=CopyListToListDouble(list);
        } 
        public static List<(double,string)> MargeDouble(List<(double,string)> list,int start1,int end1,int start2,int end2)
        {
            int a=start1;
            int b=start2;
            List<(double,string)> answer=new List<(double, string)>();
            int t=(end1-start1)+(end2-start2);
            for(int i=0;i<t;i++)
            {
                if(b!=list.Count)
                {
                    if(a>end1 && b<=end2)
                    {
                        answer.Add(list[b]);
                        b++;
                    }
                    if(b>end2 && a<=end1)
                    {
                        answer.Add(list[a]);
                        a++;
                    }
                          if(b<=end2 && a<=end1)
                    {
                        if(list[a].Item1>+list[b].Item1)
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
                    if(a<=end1)
                    {
                        answer.Add(list[a]);
                        a++;
                    }
                }
            }
            return answer;
        }
        public  static List<(double,string)> CopyListToListDouble(  List<(double,string)> list)
        {
            List<(double,string)>answer=new List<(double, string)>();
            for(int i=0;i<list.Count;i++)
            {
                answer[i]=list[i];
            }
            return answer;
            
            
        }
    }
}