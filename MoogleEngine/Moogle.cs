using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine;


public class Moogle
{
    //ruta de acceso a la coleccion de documentos
    static string path = Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Content");
    //instancia de la clase wordInfp para obtener los diccionarios

    static string[] filesNames = new string[]{};
    static string[] filesPath = new string[]{};
    static WordInformation wordInfo = new WordInformation(path);
    static Dictionary<string, List<List<int>>> DictionaryForPositions = new Dictionary<string, List<List<int>>>();
    static Dictionary<string, List<double>> DictionaryForTF= new Dictionary<string, List<double>>();
    static Dictionary<string, double> DictionaryForIDF= new Dictionary<string,double>();

    static Dictionary<string, List<string>> sinonymous = new Dictionary<string, List<string>>();
    public static void Init()
    {
        //nombre de los archivos en el path
        filesNames = Directory.GetFileSystemEntries(path);
        //obtener direcciones hacia los archivos contenidos en la coleccion
        filesPath = Directory.GetFiles(path, "*txt");
        // Comprobando cambios en la carpeta contents
      //  List<string> names = DictionaryWork.TakeFileNames();

        //Obteniendo Dictionary con todas las palabras de la coleccion de documentos y sus 
        //respectivas posiciones por documento y dictionary con todas las palabras de la coleccion y sus 
        // frecuencias por documento(convertir en tfidf)
        (DictionaryForTF, DictionaryForIDF, DictionaryForPositions) = wordInfo.FillDictionary();

         //guardar diccionary positions
         DictionaryWork.SaveDictionaryPosition(DictionaryForPositions);
        //recoger diccionario de posiciones
       ///  DictionaryForPositions=DictionaryWork.TekeDictionaryPosition();

        //dictionary de sinonimos
        sinonymous = DictionaryWork.TekeDictionarySyn();
    }

    public static List<Tuple<string,int>> GetWordsAndPositionsFromString(string cad)
    {
        List<Tuple<string,int>> vect = new List<Tuple<string,int>>();

        int last = -1;
        StringBuilder word = new StringBuilder();

        for(int i = 0 ; i < cad.Length ; i++)
        {
            if(Char.IsLetterOrDigit(cad[i]))
            {
                if(word.Length == 0)last = i;
                word.Append(cad[i]);
            }
            else
            {
                if(word.Length > 0)
                {
                    vect.Add(new Tuple<string,int>(word.ToString().ToLower(), last));
                    last = -1;
                    word.Clear();
                }
            }
        }

        if(word.Length > 0)
        {
            vect.Add(new Tuple<string,int>(word.ToString().ToLower(), last));
            last = -1;
            word.Clear();
        }

        return vect;
    }

    public static string FixQueryWithNewWords(string query, List<string> vect)
    {
        List<Tuple<string,int>> arr = GetWordsAndPositionsFromString(query);

        for(int i = arr.Count-1 ; i >= 0 ; i--)
        {
            query = query.Remove(arr[i].Item2, arr[i].Item1.Length);
            query = query.Insert(arr[i].Item2, vect[i]);
        }

        return query;
    }

     public static string WordListToString(List<string> vect)
    {
        StringBuilder cad = new StringBuilder();

        for(int i = 0 ; i < vect.Count ; i++)
        {
            if(i > 0)cad.Append(" ");
            cad.Append(vect[i]);
        }

        return cad.ToString();
    }

    //metodo para devolver los item de busqueda
    public static SearchResult Query(string query)
    {
        List<string> lst= WordInformation.GetWordsFromString(query);

        string tempcad = WordListToString(lst);

        //hallando una sugerencia para el query.
        Suggestion sgt = new Suggestion(tempcad);
        string suggestion = sgt.suggestionForQuery(DictionaryForPositions);

        //...operadores...
        //obteniendo symbol para trabajar con los operadores.
        Symbol symbol = operators.GetSymbol(query, path);
        //obteniendo array de palabras del query sin operadores
       // string[] newQuery = operators.GetSymbol(query, path).pharse;
       
        string[] newQuery = HelperMethods.NullDelet(suggestion.Split());
        //obteniendo lista con las distancias mas cercanas de las posiciones de las palabras afectadas por el operador"~"
        List<(int closeness, string document)> DistanceInWordsWhithOperator = operators.Closeness(symbol, path, DictionaryForPositions);
        //lista de score por nombre de documento ordenados de mayor a menor
        List<(double, string)> scores = score.MV(DistanceInWordsWhithOperator, filesPath, newQuery, suggestion, symbol, DictionaryForPositions, path, DictionaryForTF, DictionaryForIDF);
   
   

    //    //organizando el snnipet segun el orden dado por el score.
        string[] DocumentsInOrder = new string[scores.Count];
        for (int i = 0; i < scores.Count; i++)
        {
            DocumentsInOrder[i] = scores[i].Item2;
        }
        //cacho de codigo por documento
        string[] snippet = WordInformation.snippet(newQuery, DictionaryForPositions, DocumentsInOrder, symbol, filesPath, DictionaryForTF, DictionaryForIDF);

        //SearchItem[] items = new SearchItem[Math.Min(3, snippet.Length)];
        List<SearchItem> items = new List<SearchItem>();
        for (int i = 0; i < Math.Min(3, snippet.Length); i++)
        {
            if (scores[i].Item1 > 0){
                items.Add(new SearchItem(scores[i].Item2 + " Score: " + scores[i].Item1, snippet[i], (float)scores[i].Item1));
            }
            

        }
        SearchItem[] items1 = items.ToArray();
        return new SearchResult(items1, suggestion);

            // SearchItem[] items1111 = new SearchItem[1] {
            //  new SearchItem("Not Found", "Not Found", 0.9f)};

            //  return new SearchResult(items1111, suggestion);
    }

}
