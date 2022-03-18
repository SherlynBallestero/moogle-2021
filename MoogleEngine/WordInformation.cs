using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Collections.Generic;
using System;

namespace MoogleEngine
{
    //con esta clase se pretende crear un diccionario a partir de una direccion,en donde guarde la información que se requiere de cada palabra que se encuentra en los documento de una carpeta dada por dicha dirección.
    public class WordInformation
    {

        public string path;
      
        public WordInformation(string path)
        {
            if (path == null)
                throw new ArgumentException("The input path can't be null");
            this.path = path;
        }
        

        ///<summary>
        ///Devuelve un array a partir de otro array de string pero arrays sin elementos null y sin espacios en blanco
        ///</summary>
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
        ///En este proceso se obtiene en un array cada palabra que existe en total de todos los documentos sin 
        ///repeticion y asegurando que no hayan espacios en blanco o elementos null.
        ///</summary>   
        public string[] WordsInCollection()
        {
            string route = this.path;
            string[] file = Directory.GetFiles(route, "*.txt");

            HashSet<string> hs = new HashSet<string>();

            for (int i = 0; i < file.Length; i++)
            {
                string line = System.IO.File.ReadAllText(file[i]);
                List<string> aux = GetWordsFromString(line);

                foreach (var x in aux)
                {
                    hs.Add(x);
                }
            }

            List<string> words = new List<string>();

            foreach (var x in hs)
            {
                words.Add(x);
            }

            return words.ToArray();
        }
        ///<summary>
        ///Devuelve un segmento de codigo de determinado documento
        ///</summary>
        public static string snippetForASpecificDocument(string[] documents, string[] queryWords, Dictionary<string, List<List<int>>> dictionary, string pathToDocument, Symbol symbol, string[] filesname, Dictionary<string, List<double>> DictionaryForTF,Dictionary<string, double> DictionaryForIDF)
        {
            int maxlarge = 40;
            int minlarge = 15;
            int[] aux = new int[2];
            string[] text =  GetWordsFromString(File.ReadAllText(pathToDocument)).ToArray();
            int index = 0;
            List<string> MostValueWords = new List<string>();
            List<double> ValueForWords = new List<double>();

            //buscando cual es el indice correspondiente al documento para ubicarse en el diccionary a cual 
            //conjunto de posiciones acceder.
            for (int i = 0; i < filesname.Length; i++)
            {
                if (filesname[i] == pathToDocument)
                {
                    index = i;
                    break;
                }
            }

            int pos = 0, end = 0;
            
            //para cuando el query tiene solo una palabra
            if (queryWords.Length == 1)
            {

                //no se va a verificar si se encuentra porque si es solo un termino y el dictionario de posiciones no la
                // contiene nunca entra a crear el snippet y devuelve una la sugerencia. 
                
                if(dictionary.ContainsKey(queryWords[0]))pos = dictionary[queryWords[0]][index][0];
                else pos = 0;

                if(pos - 5 >= 0)pos = pos - 5;
                else pos = 0;

                if(pos + 30 < text.Length)end = pos + 30;
                else end = text.Length;

                return string.Join(" ", text[pos..end]);
            }
            //cuando tiene mas de una palabra nos ubicamos a partir d ela palabra mas relevante
            double tfidf = double.MinValue;
            //auxiliar
            string cad = "";

            //Iterando por las palabras del query y nos quedamos con la de mayor relevancia
            foreach(var x in queryWords)
            {   
                if(dictionary.ContainsKey(x) && dictionary[x][index][0] != -1)
                {
                    double temp = DictionaryForTF[x][index] * DictionaryForIDF[x];
                    if(tfidf < temp)
                    {
                        tfidf = temp;
                        cad = x;
                    }
                }
            }
            // si[] tomame 30 palabras a partor de la posicion 0

            if(tfidf == double.MinValue)
            {
                return string.Join(" ", text[0..(Math.Min(30, text.Length))]);
            }
            //aca se ajusta la posicion desde la que se comienzan a tomar las palabras que saldran en el snippet,se trata de 
            //tomar 5 palabras anteriores a la palabra mas relevante si no se puede nos quedamos con cero como posicion inicial
            //si no se puede tomar 30 mas se toma hasta el final del texto. 
                
            if(dictionary.ContainsKey(cad))pos = dictionary[cad][index][0];
            else pos = 0;

            if(pos - 5 >= 0)pos = pos - 5;
            else pos = 0;

            if(pos + 30 < text.Length)end = pos + 30;
            else end = text.Length;

            return string.Join(" ", text[pos..end]);
            
        }
        ///<summary>
        ///Devuelve un segmento de codigo para cada documento
        ///</summary>
        public static string[] snippet(string[] query, Dictionary<string, List<List<int>>> dictionary, string[] files, Symbol symbol, string[] filesname, Dictionary<string, List<double>> DictionaryForTF,Dictionary<string, double> DictionaryForIDF)
        {
            string[] answer = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                answer[i] = snippetForASpecificDocument(files, query, dictionary, files[i], symbol, filesname,DictionaryForTF,DictionaryForIDF);
            }
            return answer;
        }
         ///<summary>
        ///Devuelve una lista a partir de un string pero con las palabras que contiene donde nos aseguramos que cada
        ///char es una letra o un numero y ademas en minusculas.
        ///</summary>
        public static List<string> GetWordsFromString(string cad)
        {

            List<string> answer = new List<string>();
            StringBuilder word = new StringBuilder();
            //se recorre la cadena que se da como entrada
            for (int i = 0; i < cad.Length; i++)
            {
                //si el char que se analiza es una letra o un numero se agrega a la palabra que se incorporara a la lista, o sea se 
                //va conformando cada palabra que se pondra por separado en la lista que se retorna solo con letras o digitos.
                if (Char.IsLetterOrDigit(cad[i]))
                {
                    word.Append(cad[i]);
                }
                else
                {
                    //si no lo es vemos si se ha agregado a word algun char
                    if (word.Length > 0)
                    {
                        //agregamos a la lista de retorno lo que se ha conformado de la palabra limpia en minusculas y se limpia el string builder
                        //para seguir a la siguiente pelabra a formar.
                        answer.Add(word.ToString().ToLower());
                        word.Clear();
                    }
                }
            }
            // aca se agrega la ultima palabra que se formo a la lista.

            if (word.Length > 0)
            {
                answer.Add(word.ToString().ToLower());
                word.Clear();
            }

            return answer;
        }

        ///<summary>
        ///Se rellenan tres diccionarios, uno que lleva por cada palabra q existe referente a cada
        /// documento la frecuencia de la palabra, otro que por cada documento almacena las
        ///frecuencia de la palabra y un tercero que para cada documento nos dice rl idf(rareza) de la palabra.
        ///Con palabra se refiere al total de todas las palabras del conjunto de documentos.    
        ///</summary>        
        public (Dictionary<string, List<double>> TF, Dictionary<string, double> IDF, Dictionary<string, List<List<int>>> Pos) FillDictionary()
        {
            //Se hace instancia d ela clase score.
            score score = new score();
            string[] file = Directory.GetFiles(this.path, "*.txt");
            string[] words = { " " };
            //obteniendo cada palabra de la coleccion
            words = WordsInCollection();

            //Diccionary para guardar por cada palabra(clave) ,en relacion con cada documento el tf de esa palabra en
            // ese documento.
            Dictionary<string, List<double>> wordsTf = new Dictionary<string, List<double>>();
            //Diccionary para guardar por cada palabra(clave) ,en relacion con cada documento las posiciones en que
            //se  encuentra dicha palabra.
            Dictionary<string, List<List<int>>> wordInf = new Dictionary<string, List<List<int>>>();
             //Diccionary para guardar por cada palabra(clave) ,en relacion con cada documento los idf de la palabra.
             Dictionary<string, double> wordsIDF = new Dictionary<string, double>();
            
            //rellenando...
            List<double> tfAux = new List<double>();

            Dictionary<string, Dictionary<string, List<int>>> WordsAll = new Dictionary<string, Dictionary<string, List<int>>>();

            Dictionary<string, int> TextSize = new Dictionary<string, int>();

            //Creando keys  a partir del array de palabras
            foreach (string cad in words)
            {
                WordsAll.Add(cad, new Dictionary<string, List<int>>());
            }

            //Armando posiciones para cada palabra en cada documento
            foreach (string f in file)
            {
                //se hace array todo el documento
                string[] text = GetWordsFromString(System.IO.File.ReadAllText(f)).ToArray();
                //se almacena por cada documento su tamaño
                TextSize.Add(f, text.Length);
                //vamos por cada palabra del documento
                for (int i = 0; i < text.Length; i++)
                {
                    //Si no se ha agregado el documento lo agregamos he inicializamos la lista de posiciones para la palabra text[i]
                    if (WordsAll[text[i]].ContainsKey(f) == false)
                    {
                        WordsAll[text[i]].Add(f, new List<int>());
                    }
                    //se agrega a wordsAll en la palabra text[i] la posicion en que se encuentra dicha palabra

                    WordsAll[text[i]][f].Add(i);
                }
            }

            //Calculando IDF
            foreach (var word in WordsAll)
            {
                //cantidad de docs en que esta
                int cont = word.Value.Count;
                //idf es el logaritmo de la cantidad de documentos entre la cantidad de documentos en que se encuentra la palabra
                wordsIDF.Add(word.Key, Math.Log(file.Length / cont));
            }

            //Llenando TF y Positions
            //vamos por cada palabra 
            for (int i = 0; i < words.Length; i++)
            {
                List<List<int>> Positions = new List<List<int>>();
                List<double> TFaux = new List<double>();
                //vamos por cada documento
                foreach (string f in file)
                {
                    //si en wordsAll no se encuentra la palabra entonces no esta en ese documento por tanto agregamos 
                    //-1 para decir que no esta a la lista de posiciones correspondiente a dicha palabra, y a la frecuencia 0
                    if (WordsAll[words[i]].ContainsKey(f) == false)
                    {
                        Positions.Add(new List<int>() { -1 });
                        TFaux.Add(0);
                    }
                    else
                    {
                        //agregame a la lista de posiciones de la palabra i en el documento f las posiciones calculadas y almacenadas en 
                        // el dictionary WordsAll y al tfaux 0 si no hay palabras en el documento, si hay pues se agrega el tf correspondiente
                        // o sea la cant de veces que se encuentra word[i] entre el tamaño del documento
                        Positions.Add(WordsAll[words[i]][f]);
                        if (TextSize[f] == 0)
                        {
                            TFaux.Add(0);
                        }
                        else
                        {
                            TFaux.Add((double)WordsAll[words[i]][f].Count / (double)TextSize[f]);
                        }
                    }
                }
                //agregamos las posiciones y tf calculadas para la palabra word[i] a los diccionarios que se retornaran

                wordInf.Add(words[i], Positions);

                wordsTf.Add(words[i], TFaux);
            }

            return (wordsTf, wordsIDF, wordInf);
        }

        ///<summary>
        ///agregando sinonimos al query y formando un nuevo query con 5 veces el query que nos da el usuario mas 3 sinonimos  
        ///por cada palabra que contiene el query para darles un valor a los sinonimos pero que no sea elevado con respectos a las
        /// palabras exactas de la busqueda 
        ///</summary>
        public  string[] AddSynonymous(string[] word,Dictionary<string, List<string>> synonymous,Dictionary<string, List<List<int>>> DictionaryForPositions)
        {
             List<string> synonymousQuery=new List<string>(); 
            foreach(string y in word)
            {
                if(synonymous.ContainsKey(y))
            {
                int count=0;
                foreach(string x in synonymous[y])
                {
                    if(DictionaryForPositions.ContainsKey(x))
                    {
                        synonymousQuery.Add(x);
                        count++;
                    }
                    if(count==2)break;
                        
                }
            }
            }
            foreach(string z in word)
            {
                int count=0;
                while(count<5)
                {
                    synonymousQuery.Add(z);
                    count++;
                }
            }
            return synonymousQuery.ToArray();

        }
       
    }
}