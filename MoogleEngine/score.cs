using System.Collections.Generic;
using System.Collections;
using System;
using System.Text.RegularExpressions;

namespace MoogleEngine
{

    public class score
    {
  
        public score()
        {

        }

        ///<summary>
        /////Con este metodo se buscara la frecuencia con que aparece un termino t en un documento d
        ///</summary>
        public double TF(string t, string pathToFolder)
        {
            double count = 0;
           
            string[] words=File.ReadAllText(pathToFolder).Split(' ');
                foreach (string word in words)
                {
                      if (word == t) count++;
                }

            return count;
        }
        ///<summary>
        ///retorna Frecuencia Inversa de Documento, medida para conocer que tan raro es el término t en una colección de documentos dada.
        /// </summary> 
        /// <param name="route">Ruta hacia la carpeta en que se encuentran los archivos entre los que buscar.</param>       
        public static double IDF(string term,string[] files, string route)
        {
             
            double NomberOfDocuments = files.Length;
            double idf = 0;
            //inicializar FoundT en 1 por si no se encuentra t en ningun documento no se haga división por 0
            double FoundT = 1;
            for (int i = 0; i < NomberOfDocuments; i++)
            {
                if (HelperMethods.Find(term, files[i])) FoundT++;
            }
            //calculando frecuencia inversa de documentos
            idf = Math.Log(NomberOfDocuments / FoundT);
            return idf;
        }
        ///<summary>
        ///calcula  la relevancia de una palabra especifica en cada documento de una colección
        /// </summary>
        public  static double[] TFIDF(string[] files,string route , string term, Symbol symbol)
        {
            score scr=new score();
            Dictionary<string, int> asterisks = symbol.asterisks;
            //para agregar importancia a las palabras que contienen asteriscos
            double increase = 1;
            if (asterisks.ContainsKey(term)) increase = Math.Pow(2, asterisks[term]);
             
            double[] answer = new double[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                answer[i] = (scr.TF(term, files[i]) * IDF(term,files, route)) * increase;
            }
            return answer;
        }
         //file es la path a las files
         //closeness lista con distanciasmejores por documento
         //queryguide es el query hevho array de string limpio
         //diccionario de las posiciones y luego el path
         //mejoras necesarias: trabajar con los tf guardados en diccionary y si estoy pa eso guardar los tfidf por 
         //palabra y en el de tf solo poner la palabra con mas frecuencia por documento 
        public static List<(double, string)> MV(List<(int dist, string document)> Closeness,string[] file,string[] queryGuide,string query,Symbol symbol, Dictionary<string, List<List<int>>> positions, string route, Dictionary<string, List<double>> DictionaryForTF, Dictionary<string, double> DictionaryForIDF)
        {  
            // System.Diagnostics.Stopwatch ccc = new System.Diagnostics.Stopwatch();
            // ccc.Start();

            //variables necesarias para obtener score...
            double[] tfidf;
            Vector[] vectors = new Vector[file.Length];
            //para valores de coseno entre documentos y query
            double[] cosin = new double[vectors.Length];
            //indices para determinar a cual archivo se refiere una vez los ordene segun el score
            double[] index = new double[vectors.Length];
            //variables para la devolucion.
            string[] filesName = new string[file.Length];
            List<(double, string)> answer = new List<(double, string)>();
            //reformar esto....
            string[] words = new string[positions.Count];
            double[] queryForVector = new double[words.Length];
            double[,] mdt = new double[words.Length, file.Length];
            double[] auxDouble = new double[words.Length];
            //pasando las palabras a words
            int ind=-1;
            foreach(string keys in positions.Keys)
            {
                ind++;
                words[ind]=keys;
            }

            for (int i = 0; i < words.Length; i++)
            {
                //obteniendo los tfidf de cada palabra del documento para cada documento

                List<double> lidf = new List<double>();

                for(int j = 0 ; j < file.Length ; j++)
                {
                    lidf.Add(DictionaryForTF[words[i]][j] * DictionaryForIDF[words[i]]);
                }

                tfidf = lidf.ToArray();

                for (int j = 0; j < mdt.GetLength(1); j++)
                {
                    mdt[i, j] = tfidf[j];
                }
                //obteniendo el vector correspondiente al query
                if (HelperMethods.FindInArray(queryGuide, words[i]))
                {
                    queryForVector[i] = 
                    HelperMethods.CountWordInArray(queryGuide, words[i]) * IDF(words[i],file, route);
                }
                else
                {
                    queryForVector[i] = 0;
                }
            }
            Vector queryVector = new Vector(queryForVector);
            //Obteniendo el vector correspondiente a cada documento
            for (int j = 0; j < mdt.GetLength(1); j++)
            {
                for (int i = 0; i < mdt.GetLength(0); i++)
                {
                    auxDouble[i] = mdt[i, j];
                }
                vectors[j] = new Vector(auxDouble);
                auxDouble = HelperMethods.fillWithZeros(auxDouble);
            }
            //obteniendo los coseno de los angulos entre los vectores correspondiente a cada 
            //documento y el vector correspondiente al query
            for (int i = 0; i < vectors.Length; i++)
            {
                cosin[i] = vectors[i].CosVector(queryVector);
            }
           
            //no se agregan a la solución los documentos vetados
            List<string> BanDocuments = operators.BanDocuments(symbol,route);
            if (BanDocuments.Count!=0)
            {
                for (int i = 0, j = 0; i < cosin.Length; i++)
                {
                    while (j < BanDocuments.Count)
                    {
                        if (BanDocuments[j] != file[i])
                        {
                            //agregamos a la solucion los documentos que no esten vetados con sus respectivos path.
                            answer.Add((cosin[i], file[i]));
                        }
                        j++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < cosin.Length; i++)
                {
                   answer.Add((cosin[i], file[i]));
                }
            }
            //Se aumenta el score de los doc que tienen a las palabras que se encuentran cerca afectados por el operador ~
            //primero se verifica si hay palabras afectadas por este operador se aumenta el score en un  20% del 25% de los  documentos 
            //con mejores resultados de cercanía dadas las condiciones del operador.
            double increment = 5 / 10;
            if (Closeness[0].document != "notElements")
            {
                int indexAux = 0;
                while (indexAux <= answer.Count/4)
                {
                    for (int i = 0; i < answer.Count; i++)
                    {
                        if (answer[i].Item2 == Closeness[indexAux].document)
                        {
                            answer[i]= (answer[i].Item1 * increment,answer[i].Item2);
                        }
                    }
                }
            }

             answer.Sort((x,y) => y.Item1.CompareTo(x.Item1));

            // Console.WriteLine("*****************************************************");
            //  foreach(var x in answer)
            //  {
            //      Console.WriteLine(x.Item2 + " " + x.Item1);
            //  }
            //   Console.WriteLine("*****************************************************");

            // Console.WriteLine("*****************************************************");

            // Console.WriteLine(ccc.ElapsedMilliseconds);

            // Console.WriteLine("*****************************************************");

            return answer;

        }

    }
}