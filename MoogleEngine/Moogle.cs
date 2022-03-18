using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine;


public class Moogle
{
    //ruta de acceso a la coleccion de documentos
    static string path = Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Content");
    
    static string[] filesPath = new string[]{};
    //instancia de la clase wordInfp para obtener los diccionarios
    static WordInformation wordInfo = new WordInformation(path);
    //declarando todos los dictionarys que necesitaremos y rellenamos en Init()
    //Aqui guardamos por cada palabra contenida en la coleccion de documento sus respectivas posiciones por documento.
    static Dictionary<string, List<List<int>>> DictionaryForPositions = new Dictionary<string, List<List<int>>>();
    //en estos dos diccionarios guardamos frecuencia de las palabras y sus idf para utilizarlas de manera mas optima 
    //en la asignacion del score
    static Dictionary<string, List<double>> DictionaryForTF= new Dictionary<string, List<double>>();
    static Dictionary<string, double> DictionaryForIDF= new Dictionary<string,double>();

    static Dictionary<string, List<string>> synonymous = new Dictionary<string, List<string>>();
    //inicializando las estructuras que se utilizaran, las inicializamos al ejecutar el moogle. Se llaama a esta en moogle 
    //server en la linia 4
    public static void Init()
    {
        //obtener direcciones hacia los archivos contenidos en la coleccion
        filesPath = Directory.GetFiles(path, "*txt");
        //Obteniendo Dictionary con todas las palabras de la coleccion de documentos y sus 
        //respectivas posiciones por documento y dictionary con todas las palabras de la coleccion y sus 
        // frecuencias por documento(convertir en tfidf)
        (DictionaryForTF, DictionaryForIDF, DictionaryForPositions) = wordInfo.FillDictionary();
        //guardar diccionary positions
         DictionaryWork.SaveDictionaryPosition(DictionaryForPositions);

        //******************************
        //recoger diccionario de posiciones
        //esta funcion es una idea de implementacion que puede hacer mas rapida la busca,se propone trabajar en ella,
        //por ahora no se implemento porque habria que hacer muchos cambios de estructura.
       //////  DictionaryForPositions=DictionaryWork.TekeDictionaryPosition();
       //*******************************

        // recoger dictionary de sinonimos
        synonymous = DictionaryWork.TekeDictionarySyn();
    }

    //metodo para devolver los item de la busqueda
    public static SearchResult Query(string query)
    {
        //primero limpiamos el query o sea le dejamos unicamente letras y numeros y en minusculas
        List<string> lst= WordInformation.GetWordsFromString(query);
        string tempcad = HelperMethods.WordListToString(lst);

        //hallando una sugerencia para el query.
        Suggestion sgt = new Suggestion(tempcad);
        string suggestion = sgt.suggestionForQuery(DictionaryForPositions);

        //...operadores...

        //obteniendo symbol para trabajar con los operadores.
        Symbol symbol = operators.GetSymbol(query, path,DictionaryForPositions);
        //obteniendo array de palabras a partir del query sin operadores y que ademas se encuentre en nuestro conjunto total de palabras
        string[] newQuery = HelperMethods.NullDelet(suggestion.Split());
        //obtengo los sinonimos de las palabras del query
        System.Diagnostics.Stopwatch stp=new System.Diagnostics.Stopwatch();
    
        newQuery =wordInfo.AddSynonymous(newQuery,synonymous,DictionaryForPositions);
        
        //obteniendo lista con las distancias mas cercanas de las posiciones de las palabras afectadas por el operador"~"
        List<(int closeness, string document)> DistanceInWordsWhithOperator = operators.Closeness(symbol, path, DictionaryForPositions);
        //lista de score por nombre de documento ordenados de mayor a menor
            
        List<(double, string)> scores = score.MV(DistanceInWordsWhithOperator, filesPath, newQuery, suggestion, symbol, DictionaryForPositions, path, DictionaryForTF, DictionaryForIDF);
       
        //...snippet...
        //orden en que se debe obtener el snnipet segun el orden dado por el score.
        string[] DocumentsInOrder = new string[scores.Count];
        for (int i = 0; i < scores.Count; i++)
        {
            DocumentsInOrder[i] = scores[i].Item2;
        }
        //snippet por documento
        string[] snippet = WordInformation.snippet(newQuery, DictionaryForPositions, DocumentsInOrder, symbol, filesPath, DictionaryForTF, DictionaryForIDF);
        //..trabajo en el retorno de los resultados de la busqueda
        //Creando los items para conformar SearchResult que se devolvera
        List<SearchItem> items = new List<SearchItem>();
        //se piensa en retornar a lo sumo tres resultados
        //////////////////////////for (int i = 0; i < Math.Min(3, snippet.Length); i++)
        for (int i = 0; i < scores.Count; i++)
        {
            //solo se agregan los documentos con score mayor que cero a los resultados de la busqueda.
            if (scores[i].Item1 > 0){
                items.Add(new SearchItem(scores[i].Item2 + " Score: " + scores[i].Item1, snippet[i], (float)scores[i].Item1));
            }
        }
        //retornando resultado.
        SearchItem[] items1 = items.ToArray();
        return new SearchResult(items1, suggestion);
    }

}
