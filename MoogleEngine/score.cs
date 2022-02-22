using System.Collections.Generic;
using System.Collections;
using System;

namespace MoogleEngine
{

    public class score
    {
        public string path;
        public WordInformation(string path)
        {
            if (path == null)
                throw new ArgumentException("The imput path can't be null");
            this.path = path;
        }

        ///<summary>
        /////Con este metodo se buscara la frecuencia con que aparece un termino t en un documento d
        ///</summary>
        public double TF(string t, string pathToFolder)
        {
            t = Regex.Replace(t.Normalize(System.Text.NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");

            //vamos a llevar el termino a minuscula para encontrarlo siempre que aparezca ya sea en minuscula o en mayuscula
            t = t.ToLower();
            StreamReader str = new StreamReader(pathToFolder);
            double count = 0;
            string line = str.ReadLine();
            string[] words;
            while (line != null)
            {
                words = SplitPhraseInWords(line);
                string w = "";

                foreach (string word in words)
                {
                    w = Regex.Replace(word.Normalize(System.Text.NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");

                    if (w == t) count++;
                }
                line = str.ReadLine();
            }
            return count;
        }
        ///<summary>
        ///retorna Frecuencia Inversa de Documento, medida para conocer que tan raro es el término t en una colección de documentos dada.
        /// </summary> 
        /// <param name="route">Ruta hacia la carpeta en que se encuentran los archivos entre los que buscar.</param>       
        public static double IDF(string term, string route)
        {
            string[] files = Directory.GetFiles(route, "*.txt");
            double NomberOfDocuments = files.Length;
            double idf = 0;
            //inicializar FoundT en 1 por si no se encuentra t en ningun documento no se haga división por 0
            double FoundT = 1;
            for (int i = 0; i < NomberOfDocuments; i++)
            {
                if (Find(term, files[i])) FoundT++;
            }
            //calculando frecuencia inversa de documentos
            idf = Math.Log(NomberOfDocuments / FoundT);
            return idf;
        }
        ///<summary>
        ///calcula  la relevancia de una palabra especifica en cada documento de una colección
        /// </summary>
        public static double[] TFIDF(string route, string term, Symbol symbol)
        {
            Dictionary<string, int> asterisks = symbol.asterisks;
            //para agregar importancia a las palabras que contienen asteriscos
            int increase = 1;
            if (asterisks.ContainsKey(term)) increase = increase * Math.Pow(2, asterisks[term]);
            string[] files = Directory.GetFiles(route, "*.txt");
            double[] answer = new double[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                answer[i] = (TF(term, files[i]) * IDF(term, route)) * increase;
            }
            return answer;
        }
        public static List<(double, string)> MV(string query, Dictionary<string, (string[] index1, List<int[]> index2)> positions, string route)
        {
            //procesando operadores...
            operators operators = new operators(query, positions, route);
            Symbol symbol = operators.GetSymbol.symbol;
            string phrase = operators.GetSymbol.pharse;
            //variables necesarias para obtener score...
            HelperMethods hM = new HelperMethods(route);
            string[] file = Directory.GetFiles(route, "*.txt");
            string[] queryGuide = HelperMethods.SplitPhraseInWords(phrase);
            double[] tfidf;
            Vector[] vectors = new Vector[file.Length];
            //para valores de coseno entre documentos y query
            double[] cosin = new double[vectors.Length];
            //indices para determinar a cual archivo se refiere una vez los ordene segun el score
            double[] index = new double[vectors.Length];
            //variables para la devolucion.
            string[] filesName = new string[file.Length];
            List<(double, string)> answer = new List<(double, string)>();
            //lista que contiene las distancias entre las distancias entre las palabras para el operados ~
            List<(int dist, string document)> Closeness = operators.Closeness(symbol);
            words = hM.WordsInCollection();
            double[] queryForVector = new double[words.Length];
            double[,] mdt = new double[words.Length, file.Length];
            double[] auxDouble = new double[words.Length];
            double[] score;
            double[] indexs;

            for (int i = 0; i < words.Length; i++)
            {
                //obteniendo los tfidf de cada palabra del documento para cada documento
                tfidf = TFIDF(route, words[i], symbol);
                for (int j = 0; j < mdt.GetLength(1); j++)
                {
                    mdt[i, j] = tfidf[j];
                }
                //obteniendo el vector correspondiente al query
                if (hM.FindInArray(queryGuide, words[i]))
                {
                    queryForVector[i] = hM.CountWordInArray(queryGuide, words[i]) * IDF(words[i], route);
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
                auxDouble = hM.fillWithZeros(auxDouble);
            }
            //obteniendo los coseno de los angulos entre los vectores correspondiente a cada 
            //documento y el vector correspondiente al query
            for (int i = 0; i < vectors.Length; i++)
            {
                cosin[i] = vectors[i].CosVector(queryVector);
            }
            // score = hM.Ordenar(cosin, out indexs);
            //haremos cero el score de los documentos vetados
            list<string> BanDocuments = operators.BanDocuments(symbol);
            if (BanDocuments[0] != "notElements")
            {
                for (int i = 0, j = 0; i < cosin.Length; i++)
                {
                    while (j < BanDocuments.Count)
                    {
                        if (BanDocuments[i] != file[index[i]])
                        {
                            //agregamos a la solucion los documentos que no esten vetados con sus respectivos path.
                            answer.Add(cosin[i], file[index[i]]);
                        }
                    }
                }
            }
            //aumentar el score de los doc que tienen a las palabras que se encuentran cerca afectados por el operador ~
            //primero se verifica si hay palabras afectadas por este operador
            //se aumenta el score en un  20% de los 7 documentos con mejores resultados cercania dadas las condiciones del operador.
            double increment = 5 / 10;
            if (Closeness[0].document != "notElements")
            {
                int indexAux = 0;
                while (indexAux <= 7)
                {
                    for (int i = 0; i < answer.Count; i++)
                    {
                        if (answer[i].Item2 == Closeness[indexAux].document)
                        {
                            answer[i].Item1 = answer[i].Item1 * increment;
                        }
                    }
                }
            }


            return answer;

        }

    }
}