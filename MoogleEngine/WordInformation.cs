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
                throw new ArgumentException("The imput path can't be null");
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
        ///Devuelve la concatenacion de dos arrays  sin espacios en blanco
        ///</summary>
        public static string[] ConcatWhithRepetition(string[] a, string[] b)
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
                    if (!(String.IsNullOrEmpty(a[i])))
                        concated[i] = a[i];
                }
                else
                {
                    if (!(String.IsNullOrEmpty(b[j])))
                        concated[i] = b[j];
                    j++;
                }
            }
            concated = NullDelet(concated);
            return concated;
        }

        ///<summary>
        ///Devuelve la concatenacion de dos arrays sin repeticion y sin espacios en blanco
        ///</summary>
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
        ///Devuelve true si el parametro b se encuentra en el string a
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
        ///A partir de una frase devuelve un array de string donde a cada palabra se le hacen las transformaciones necesarias para que solo contenga letras y numeros en utfo
        ///</summary>
        public static string[] TokenWords(string phrase)
        {
            string[] words = phrase.Split(' ');
            string[] newWords = new string[words.Length];
            int index = 0;
            string w = "";
            bool change = false;
            foreach (string word in words)
            {
                // for (int i = 0; i < word.Length; i++)
                // {
                //     if (char.IsLetter(word[i]) || char.IsNumber(word[i]))
                //     {
                //         w += word[i];
                //     }
                //     else
                //     {
                //         //change se hace false cuando hubo un caso en q se encontro con un char que no es letra
                //         change = true;
                //     }
                // }
                // w = Regex.Replace(w.Normalize(System.Text.NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
                // newWords[index] = w;
                // w = "";
                // index++;
                // change = false;
                w=word;
                  w = Regex.Replace(w.Normalize(System.Text.NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
                  newWords[index] = w;
            }
            return newWords;
        }
        ///<summary>
        ///devuelve un array de string a partir de una frase
        ///</summary>       
        public static string[] SplitPhraseInWords(string phrase)
        {
            string[] words = phrase.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToLower();
            }
            return words;
        }

        ///<summary>
        ///En este proceso se obtiene en un array cada palabra que existe en total de todos los documentos sin 
        ///repeticion y asegurando que no hayan espacios en blanco o elementos null.
        ///</summary>   
        public string[] WordsInCollection()
        {
            string route = this.path;
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
        ///En este proceso se obtiene en un array cada palabra que existe en un documento, ademas de las comas 
        ///y espacios en blancos para saber las posiciones.
        ///</summary>   
        public static string[] WordsInDocuments(string document)
        {
            string[] reader = System.IO.File.ReadAllText(document).Split();
            return reader;

        }
        ///<summary>
        ///Devuelve las posiciones de un termino en un documento
        ///</summary>
        public static int[] PositionInDocument(string word, string document, string route)
        {
            score scr = new score();
            string[] allWordsInDocument = WordsInDocuments(document);
            int[] ret = { -1 };
            //if (TF(word, document) == 0) return ret;
            if (scr.TF(word, document) == 0) return ret;
            int[] answer = new int[Convert.ToInt32(scr.TF(word, document))];
            for (int i = 0, index = 0; i < allWordsInDocument.Length; i++)
            {
                if (allWordsInDocument[i] == word)
                {
                    answer[index] = i;
                    index++;
                }
            }
            return answer;
        }
        ///<summary>
        ///Devuelve un cacho de codigo de determinado documento
        ///</summary>
        public static string snippetForASpecificDocument(string[] queryWords, Dictionary<string, (string[] t1, List<int[]> t2)> dictionary, string pathToDocument, Symbol symbol)
        {//**asegurarme de pasar el query limpio**
            int maxlarge = 40;
            int minlarge = 15;
            //string[] queryWords = query.Split();
            int[] aux = new int[2];
            string[] text = File.ReadAllText(pathToDocument).Split(' ');

            //List<int> aux=new List<int>();
            string[] documents = dictionary[queryWords[0]].t1;
            int index = 0;
            List<string> MostValueWords = new List<string>();
            List<double> ValueForWords = new List<double>();


            //buscando cual es el indice correspondiente al documento para ubicarse en el diccionary a cual 
            //conjunto de posiciones acceder.
            for (int i = 0; i < documents.Length; i++)
            {
                if (documents[i] == pathToDocument)
                {
                    index = i;
                    break;
                }
            }
            //para tener en cuenta las palabras mas relevantes del cuery en este documento
            foreach (string word in queryWords)
            {
                //obteniendo el valor de la palabra para el documento
                ValueForWords.Add(score.TFIDF(pathToDocument, word, symbol)[index]);
            }
            //dados los valores por cada palabra del query se toman las mejores valoradas.
            double[] values = new double[ValueForWords.Count];
            ValueForWords.CopyTo(0, values, 0, ValueForWords.Count - 1);
            Array.Sort(values);
            //que palabra tiene mayor valor?
            for (int i = 0; i < values.Length / 2; i++)
            {
                for (int j = 0; j < ValueForWords.Count; j++)
                {
                    //en values tenemos los valores ordenados de mayor a 
                    //menor,se espera obtener ahora a que palabras les corresponde los mayores valores
                    //las localizamos dadas la lista no ordenada ya que en el indice j se encuentra el indice 
                    //correspondiente a la palabra a la se le asigna dicho valor.
                    if (values[i] == ValueForWords[j]) MostValueWords.Add(queryWords[j]);
                }
            }
            string[] bestQuery = new string[MostValueWords.Count];
            //aqui se asumio q con las dos mejores palabras bastaban pero esta sujeto a cambio
            MostValueWords.CopyTo(0, bestQuery, 0, 2);

            //obteniendo un array con todas las posiciones de todas las palabras del query en el documento dado.
            for (int i = 0; i < queryWords.Length; i++)
            {
                //implementar concatInt
                aux = ConcatInt(dictionary[queryWords[i]].t2[index], aux);
            }

            Array.Sort(aux);
            //buscando el subconjunto de menor distancia al que pertenecen todas las palabras del query.
            int min = int.MaxValue;
            int lowerEnd = 0;
            int topEnd = 0;
            //posiciones de las dos palabras mas importantes en el documento
            int mostImpWordPosition1 = dictionary[bestQuery[0]].t2[index][0];
            int mostImpWordPosition2 = dictionary[bestQuery[1]].t2[index][0];
            //este boolean nos dira si entramos en el caso en el que las dos palabras con mas 
            //valor estan extremadamente separadas
            ///////revisar que esto no me de error
            bool extremeSeparation = false;
            //para caso extremo decidimos estos bordes
            int lowerbound1 = mostImpWordPosition1;
            int upperbound1 = mostImpWordPosition1 + minlarge;
            int lowerbound2 = mostImpWordPosition2 - minlarge;
            int upperbound2 = mostImpWordPosition2;

            for (int i = 0; i < aux.Length; i++)
            {
                for (int j = i + queryWords.Length; j < aux.Length; j++)
                {

                    if (SubsetContent(aux[i], aux[j], queryWords, dictionary, index))
                    {
                        min = Math.Min(aux[j] - aux[i], min);
                        lowerEnd = aux[i];
                        topEnd = aux[j];
                    }
                }
            }
            //tomando en el array documento un cacho desente se construye a partir del array
            //priemro se ajusta el espacio que se tomara
            if ((topEnd - lowerEnd) > maxlarge)
            {
                if (SubsetContent(lowerEnd, topEnd / 2, bestQuery, dictionary, index) && (topEnd / 2 - lowerEnd) <= maxlarge + 15)
                {
                    topEnd = topEnd / 2;
                }
                else if (SubsetContent(lowerEnd + lowerEnd / 2, topEnd, bestQuery, dictionary, index) && (topEnd / 2 - (lowerEnd + lowerEnd / 2)) <= maxlarge + 15)
                {
                    lowerEnd = lowerEnd + lowerEnd / 2;
                }
                else
                {
                    // en este caso tomamos dos conjuntos separados con las dos palabras mas importantes del query
                    extremeSeparation = true;
                    //entonces tomamos dos subconjuntos de tamaño aceptable que tome a las dos palabras mas imp del query 
                }
            }
            else if ((topEnd - lowerEnd) < minlarge)
            {
                if (topEnd + minlarge / 2 < text.Length)
                {
                    topEnd = topEnd + minlarge / 2;
                }
                if (lowerEnd - minlarge / 2 > 0)
                {
                    lowerEnd = lowerEnd - minlarge / 2;
                }
            }
            //ahora se tomara el texto a devolver
            string answer = " ";
            //para caso extremo de separacion
            string answer2 = "";
            if (extremeSeparation)
            {

                for (int i = lowerbound1, j = lowerbound2; i < upperbound1; i++)
                {
                    answer += text[i] + " ";
                    if (j < text.Length) answer2 += text[j] + " ";
                }
                answer = answer + "..." + answer2;
            }
            else
            {
                for (int i = lowerEnd; i < topEnd; i++)
                {
                    answer += text[i] + " ";
                }
            }
            return answer;
        }
        ///<summary>
        ///Devuelve un cacho de codigo para cada documento
        ///</summary>
        public static string[] snippet(string[] query, Dictionary<string, (string[] t1, List<int[]> t2)> dictionary, string[] files, Symbol symbol)
        {
            string[] answer=new string[files.Length];
            for(int i=0;i<files.Length;i++)
            {
                answer[i]=snippetForASpecificDocument(query,dictionary,files[i],symbol);
            }
            return answer;
        }
        ///<summary>
        ///Rellenar dos diccionarios y devuelve un diccionario que lleva por cada palabra q existe referente a cada
        /// documento la frecuencia de dada palabra para cada documento y otro que por cada documento almacena las
        /// documento la frecuencia de dada palabra para cada documento.    
        ///</summary>        
        public (Dictionary<string, (string[], List<double>)> t1, Dictionary<string, (string[], List<int[]>)> t2) FillDictionary()
        {
            score score = new score();
            string[] file = Directory.GetFiles(this.path, "*.txt");
            string[] words = { " " };
            words = WordsInCollection();
            //Diccionary para guardar por cada palabra(clave) ,en relacion con cada documento el tf de esa palabra en
            // ese documento.
            Dictionary<string, (string[], List<double>)> wordsTf = new Dictionary<string, (string[], List<double>)>();
            //Diccionary para guardar por cada palabra(clave) ,en relacion con cada documento las posiciones en que
            // encontramos dicha palabra.
            Dictionary<string, (string[], List<int[]>)> wordInf = new Dictionary<string, (string[], List<int[]>)>();
            //rellenando...
            List<double> tfAux = new List<double>();
            List<int[]> Positions = new List<int[]>();

            for (int i = 0; i < words.Length; i++)
            {
                foreach (string f in file)
                {
                    tfAux.Add(score.TF(words[i], f));
                    Positions.Add(PositionInDocument(words[i], f, this.path));
                }
                wordsTf.Add(words[i], (file, tfAux));
                wordInf.Add(words[i], (file, Positions));
                tfAux = new List<double>();
                Positions = new List<int[]>();
            }
            return (wordsTf, wordInf);
        }
        // // //  public void RealeaseDictionary()
        // // //  {
        // // //      File.WriteAllText(@"D:\cibernética\cibernética\Primer año\pro\proyectos\proyectoFormal\First project moogle-2021\Dicctionary",contents: JsonSerializer.Serialize(vocabulary));
        // // //  }
        // // //  public void TekeDictionary()
        // // //  {
        // // //      _vocabulary=JsonSerializer.Deserialize<Dictionary<string, (string[], List<double>, List<int[]>)>>(json:File.ReadAllText(@"D:\cibernética\cibernética\Primer año\pro\proyectos\proyectoFormal\First project moogle-2021\Dicctionary"))??throw new Exception();
        // // //  }
        ///<summary>
        ///Metodo para concatenar dos arrays de enteros
        ///</summary>
        public static int[] ConcatInt(int[] a, int[] b)
        {
            int[] concated = new int[a.Length + b.Length];
            int[] c = { -1 };
            //creo que no debe haber ningun problema con eso pq no tiene pq llegar el caso en que aux sea {-1} dado que se le pasan 
            //arrays de int con las posiciones en que se encuentra una palabra dada para cierto doc,hay que ver cuando esa palabra no se encuentra en el documento que es lo q me da en las posiciones,en el caso de que obtenga -1 habria que ver no vaya a ser que mande a indexar en el documento en menos 1

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
        ///Metodo para determinar si dado un intervalo [a,b] referentes a posicion a y
        // posicion b en el documento saber si las palabras del query aparecen en dicho intervalo.
        ///</summary>
        public static bool SubsetContent(int lowerEnd, int topEnd, string[] queryWords, Dictionary<string, (string[] t1, List<int[]> t2)> dictionary, int documentIndex)
        {
            foreach (string word in queryWords)
            {
                if (!IsInSet(dictionary[word].t2[documentIndex], lowerEnd, topEnd)) return false;
            }
            return true;
        }
        ///<summary>
        ///Metodo para determinar si dado un intervalo [a,b] referentes a posicion a y
        // posicion b en el documento saber si las palabras con mas peso del query aparecen en dicho intervalo.
        ///</summary>
        public static bool IsInSet(int[] a, int lowerEnd, int topEnd)
        {
            foreach (int position in a)
            {
                if (position <= topEnd && position >= lowerEnd) return true;
            }
            return false;
        }
    }
}