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
    
        public static List<(double, string)> MV(string[] file,string[] queryGuide,string query,Symbol symbol, Dictionary<string, List<List<int>>> positions, string route, Dictionary<string, List<double>> DictionaryForTF, Dictionary<string, double> DictionaryForIDF)
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
                    if(symbol.asterisks.ContainsKey(words[i]))
                    {
                         mdt[i, j] = tfidf[j] * Math.Pow(2, symbol.asterisks[words[i]]);
                    }
                    else
                    {
                         mdt[i, j] = tfidf[j];
                    }
                   
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
            List<string> BanDocuments = operators.BanDocuments(symbol,route,positions);
            if (BanDocuments.Count!=0)
            {
                for (int i = 0; i < cosin.Length; i++)
                {
                    int j = 0;

                    bool flag = true;

                    while (j < BanDocuments.Count)
                    {
                        if (BanDocuments[j] == file[i])
                        {
                            flag = false;
                        }
                        j++;
                    }

                    if(flag)
                    {
                        //agregamos a la solucion los documentos que no esten vetados con sus respectivos path.
                        answer.Add((cosin[i], file[i]));
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
            //estructura auxiliar para darle valor as los documentos con las palabras afectadas por el operador ~
            //se le da mas importancia a los archivos donde estas estan mas cercanas.

            Dictionary<string, int> mp = new Dictionary<string, int>();

            if(symbol.Closeness[0] != ("notElements", "notElements"))
            {
                for(int i = 0 ; i < file.Length ; i++)
                {
                    mp.Add(file[i], 0);

                    foreach(var x in symbol.Closeness)
                    {
                        List<(int, int)> lst = new List<(int, int)>();
                        //se agrega a la lista auxiliar las posiciones de la palabras 1 y 2 de `symbol.Closeness` se les 
                        //clasifica en p1 y p2 para saber si corresponde a la primera o la segunda palabra
                        foreach(var p1 in positions[x.Item1][i])
                        {
                            if(p1 != -1)lst.Add((p1, 1));
                        }

                        foreach(var p2 in positions[x.Item2][i])
                        {
                            if(p2 != -1)lst.Add((p2, 2));
                        }
                        //se ordena la lista auxiliar 
                        lst.Sort((x,y) => y.Item1.CompareTo(x.Item1));
                        //se itera por posiciones sucesivas verificando que sean de tipos diferentes y quedandonos 
                        //con distancia inferior a 10, o sea todas las sucesivas con distancia inferior a 10 aumentan un
                        // contador en el diccionario auxiliar
                        for(int k = 1 ; k < lst.Count ; k++)
                        {
                            if(lst[k].Item2 != lst[k-1].Item2 && Math.Abs(lst[k].Item1 - lst[k-1].Item1) < 10)
                            {
                                mp[file[i]]++;
                            }
                        }
                    }
                }
            }
            //ahora al ranking le asignamos cierto incremento a partir de las palabras cercanas.
            for(int i = 0 ; i < answer.Count ; i++)
            {
                if(mp.ContainsKey(answer[i].Item2))
                {
                    answer[i] = (answer[i].Item1 * (Math.Log(mp[answer[i].Item2]+1)+1), answer[i].Item2);
                }
            }

            

             answer.Sort((x,y) => y.Item1.CompareTo(x.Item1));

         

            return answer;

        }

    }
}