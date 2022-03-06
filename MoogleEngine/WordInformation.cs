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

            foreach (string word in words)
            {
                w = word;
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

            HashSet<string> hs = new HashSet<string>();

            for (int i = 0; i < file.Length; i++)
            {
                string line = System.IO.File.ReadAllText(file[i]);
               // aux = TokenWords(line);

                List<string> aux = GetWordsFromString(line);

                foreach(var x in aux)
                {
                    hs.Add(x);
                }
            }

            List<string> words = new List<string>();
            
            foreach(var x in hs)
            {
                words.Add(x);
            }

            return words.ToArray();
        }
        ///<summary>
        ///En este proceso se obtiene en un array cada palabra que existe en un documento, ademas de las comas 
        ///y espacios en blancos para saber las posiciones.
        ///</summary>   
        public static string[] WordsInDocuments(string document)
        {
            string[] reader = System.IO.File.ReadAllText(document).Split(' ');
            return reader;

        }
        ///<summary>
        ///Devuelve las posiciones de un termino en un documento
        ///</summary>
        public static List<int> PositionInDocument(string word, string document, string route)
        {
            score scr = new score();
            string[] allWordsInDocument = WordsInDocuments(document);
            allWordsInDocument=WordInformation.NullDelet(allWordsInDocument);
            List<int> answer = new List<int>(); 
            if (scr.TF(word, document) == 0)
            {
                answer.Add(-1);
                return answer;
            } 
            
            for (int i = 0; i < allWordsInDocument.Length; i++)
            {
                if (allWordsInDocument[i] == word)
                {
                   answer.Add(i);
                }
            }
            return answer;
        }
        ///<summary>
        ///Devuelve un segmento de codigo de determinado documento
        ///</summary>
        public static string snippetForASpecificDocument(string[] documents,string[] queryWords, Dictionary<string, List<List<int>>> dictionary, string pathToDocument, Symbol symbol)
        {//Nota:**asegurarme de pasar el query limpio**
            int maxlarge = 40;
            int minlarge = 15;
           int[] aux = new int[2];
            string[] text = File.ReadAllText(pathToDocument).Split();
            //se utiliza el dictionary para obtener los documentos
            int index = 0;
            List<string> MostValueWords = new List<string>();
            List<double> ValueForWords = new List<double>();
            //para cuando el query tiene solo una palabra
           if(queryWords.Length==1)
           {
               //no se va a verificar si se encuentra porque si es solo un termino y el dictionario de posiciones no la
               // contiene nunca entra a crear el snippet y devuelve una la sugerencia. 
               int pos=dictionary.ContainsKey(queryWords[0])?dictionary[queryWords[0]][index][0]:0;
                pos=pos-5>=0?pos-5:0;
                int end=pos+30<text.Length?end=pos+30:text.Length;
                return string.Join(" ",text[pos..end]);
           }
           
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
                ValueForWords.Add(score.TFIDF(documents,pathToDocument, word, symbol)[index]);
            }
            //dados los valores por cada palabra del query se toman las mejores valoradas.
            double[] values = new double[ValueForWords.Count];
            ValueForWords.CopyTo(0, values, 0, ValueForWords.Count - 1);
            Array.Sort(values);
            //que palabra tiene mayor valor?
            for (int i = 0; i < 2; i++)
            {   if(values.Length<2 && i==1)break;
                for (int j = 0; j < ValueForWords.Count; j++)
                {
                    //en values tenemos los valores ordenados de mayor a 
                    //menor,se espera obtener ahora a que palabras les corresponde los mayores valores
                    //las localizamos dadas la lista no ordenada ya que en el indice j se encuentra el indice 
                    //correspondiente a la palabra a la que se le asigna dicho valor.
                    if (values[i] - ValueForWords[j]<0.000001) MostValueWords.Add(queryWords[j]);
                }
            }
            string bestQuery1 = MostValueWords.Count>0? MostValueWords[0]:"-";
            string bestQuery2 = MostValueWords.Count>0? MostValueWords[1]:"-";
            string[] bestQuery={bestQuery1,bestQuery2};

            //aqui se asumio q con las dos mejores palabras bastaban pero esta sujeto a cambio
            //obteniendo un array con todas las posiciones de todas las palabras del query en el documento dado.
            for (int i = 0; i < queryWords.Length; i++)
            {
                if(dictionary.ContainsKey(queryWords[i]))
                    aux = ConcatInt(dictionary[queryWords[i]][index].ToArray(), aux);
            }

            Array.Sort(aux);//ordena de menor a mayor
            //buscando el subconjunto de menor distancia al que pertenecen todas las palabras del query.
            int min = int.MaxValue;
            int lowerEnd = 0;
            int topEnd = 0;
           
            //este boolean dira si entramos en el caso en el que las dos palabras con mas 
            //valor estan extremadamente separadas
            ///////revisar que esto no me de error
            bool extremeSeparation = false;
            

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
            //tomando en el array documento un segmento adecuado se construye a partir del array
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
            string answer = "";
            //para caso extremo de separacion
            string answer2 = "";
            if (extremeSeparation)
            {
                //que necesitamos
                 //posiciones de las dos palabras mas importantes en el documento(dos randoms)
                 int mostImpWordPosition1 = dictionary[bestQuery1][index][0]>=0?dictionary[bestQuery1][index][0]:0;
                 int mostImpWordPosition2=0;
                
                 if(dictionary.ContainsKey(bestQuery2))
                {
                     mostImpWordPosition2 = dictionary[bestQuery2][index][0];
                }
          
               //para caso extremo decidimos estos bordes
                int lowerbound1 = mostImpWordPosition1;
                int upperbound1 = mostImpWordPosition1 + minlarge;
                int lowerbound2 = mostImpWordPosition2 - minlarge>=0?mostImpWordPosition2 - minlarge:0;
                int upperbound2 = mostImpWordPosition2;
                answer += string.Join(" ",text[lowerbound1..upperbound1]);
                if (lowerbound2 >=0 && upperbound2 >= 0) answer+="(...)" + string.Join(" ",text[lowerbound2..upperbound2]);
            }
            else
            {

                answer=string.Join(" ",text[Math.Max(lowerEnd,0)..Math.Min(topEnd, text.Length)]);
            }
            return answer;
        }
        ///<summary>
        ///Devuelve un segmento de codigo para cada documento
        ///</summary>
        public static string[] snippet(string[] query, Dictionary<string, List<List<int>>> dictionary, string[] files, Symbol symbol)
        {
            string[] answer=new string[files.Length];
            for(int i=0;i<files.Length;i++)
            {
                answer[i]=snippetForASpecificDocument(files,query,dictionary,files[i],symbol);
            }
            return answer;
        }

        public static List<string> GetWordsFromString(string cad)
        {
            List<string> vect = new List<string>();

            StringBuilder word = new StringBuilder();

            for(int i = 0 ; i < cad.Length ; i++)
            {
                if(Char.IsLetterOrDigit(cad[i]))
                {
                    word.Append(cad[i]);
                }
                else
                {
                    if(word.Length > 0)
                    {
                        vect.Add(word.ToString().ToLower());
                        word.Clear();
                    }
                }
            }

            if(word.Length > 0)
            {
                vect.Add(word.ToString().ToLower());
                word.Clear();
            }

            return vect;
        }
        
        ///<summary>
        ///Rellenar dos diccionarios y devuelve un diccionario que lleva por cada palabra q existe referente a cada
        /// documento la frecuencia de dada palabra para cada documento y otro que por cada documento almacena las
        /// documento la frecuencia de dada palabra para cada documento.    
        ///</summary>        
        public (Dictionary<string, List<double>> t1, Dictionary<string, List<List<int>>> t2) FillDictionary()
        {
            score score = new score();
            string[] file = Directory.GetFiles(this.path, "*.txt");
            string[] words = { " " };
            words = WordsInCollection();

            //Diccionary para guardar por cada palabra(clave) ,en relacion con cada documento el tf de esa palabra en
            // ese documento.
            Dictionary<string,List<double>> wordsTf = new Dictionary<string,List<double>> ();
            //Diccionary para guardar por cada palabra(clave) ,en relacion con cada documento las posiciones en que
            // encontramos dicha palabra.
            Dictionary<string, List<List<int>>> wordInf = new  Dictionary<string, List<List<int>>>();
            //rellenando...
            List<double> tfAux = new List<double>();

            Dictionary<string,Dictionary<string,List<int>>> WordsAll = new Dictionary<string, Dictionary<string, List<int>>>();

            Dictionary<string,int> TextSize = new Dictionary<string, int>();

            foreach(string cad in words)
            {
                WordsAll.Add(cad, new Dictionary<string, List<int>>());
            }

            foreach(string f in file)
            {
                string[] text = GetWordsFromString(System.IO.File.ReadAllText(f)).ToArray();

                TextSize.Add(f, text.Length);

                for(int i = 0 ; i < text.Length ; i++)
                {
                    if(WordsAll[text[i]].ContainsKey(f) == false)
                    {
                        WordsAll[text[i]].Add(f, new List<int>());
                    }

                    WordsAll[text[i]][f].Add(i);
                }
            }

            for (int i = 0; i < words.Length; i++)
            {
                List<List<int>> Positions = new List<List<int>>();

                List<double> TFaux = new List<double>();

                foreach (string f in file)
                {
                    if(WordsAll[words[i]].ContainsKey(f) == false)
                    {
                        Positions.Add(new List<int>(){-1});
                        TFaux.Add(0);
                    }   
                    else
                    {
                        Positions.Add(WordsAll[words[i]][f]);
                        if(TextSize[f] == 0)
                        {
                            TFaux.Add(0);
                        }
                        else
                        {
                            TFaux.Add((double)WordsAll[words[i]][f].Count / (double)TextSize[f]);
                        }
                    }
                }
                
                wordInf.Add(words[i], Positions);

                wordsTf.Add(words[i], TFaux);
            }

            return (wordsTf, wordInf);
        }
      
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
        public static bool SubsetContent(int lowerEnd, int topEnd, string[] queryWords, Dictionary<string, List<List<int>>> dictionary, int documentIndex)
        {
            foreach (string word in queryWords)
            {
                if(dictionary.ContainsKey(word))
                    if (!IsInSet(dictionary[word][documentIndex].ToArray(), lowerEnd, topEnd)) return false;
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