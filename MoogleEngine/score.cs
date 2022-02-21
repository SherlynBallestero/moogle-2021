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
    public static double[] TFIDF(string route, string term)
    {

        string[] files = Directory.GetFiles(route, "*.txt");
        double[] answer = new double[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            answer[i] = TF(term, files[i]) * IDF(term, route);
        }
        return answer;
    }
         public static double[] MV( string query,string route)
    {
        string[] file = Directory.GetFiles(route, "*.txt");
        string[] words = { " " };
        string[] aux = { " " };
        string[] queryGuide = HelperMethods.SplitPhraseInWords(query);
        double[] tfidf;
        Vector[] vectors = new Vector[file.Length];
        //para valores de coseno entre documentos y query
        double[] cosin = new double[vectors.Length];
        //indices para determinar a cual archivo se refiere una vez los ordene segun el score
        double[] index = new double[vectors.Length];

        //obteniendo un array con cada palabra que se encuentra entre todos los documentos sin repetir y sin comas o espacios en blancos y normalizados.
        for (int i = 0; i < file.Length; i++)
        {
            StreamReader str = new StreamReader(file[i]);
            string line = str.ReadLine();
            while (line != null)
            {
                
                aux =HelperMethods.TokenWords(line);
                words = HelperMethods.Concat(words, aux);
                line = str.ReadLine();
            }
        }
        words = HelperMethods.NullDelet(words);
        double[] queryForVector = new double[words.Length];
        double[,] mdt = new double[words.Length, file.Length];
        double[] auxDouble = new double[words.Length];
        double[] score;
        double[] indexs;

        for (int i = 0; i < words.Length; i++)
        {
            //obteniendo los tfidf de cada palabra del documento para cada documento
            tfidf = TFIDF(route, words[i]);
            for (int j = 0; j < mdt.GetLength(1); j++)
            {
                    mdt[i, j] = tfidf[j];
            }
            //obteniendo el vector correspondiente al query
            if (HelperMethods.FindInArray(queryGuide, words[i]))
            {
                queryForVector[i] = CountWordInArray(queryGuide, words[i]) * IDF(words[i], route);
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
            auxDouble=fillWithZeros(auxDouble);
        }
        //obteniendo los coseno de los angulos entre los vectores correspondiente a cada 
        //documento y el vector correspondiente al query
        for (int i = 0; i < vectors.Length; i++)
        {
            cosin[i] = vectors[i].CosVector(queryVector);
        }
        score = HelperMethods.Ordenar(cosin, out indexs);
        string[] filesName = new string[file.Length];
        
        return score;
    }

    }
}