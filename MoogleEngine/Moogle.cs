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

        //Obteniendo Dictionary con todas las palabras de la coleccion de documentos y sus 
        //respectivas posiciones por documento

        //Obteniendo Dictionary con todas las palabras de la coleccion y sus 
        // frecuencias por documento(convertir en tfidf)
        (DictionaryForTF, DictionaryForIDF, DictionaryForPositions) = wordInfo.FillDictionary();

         //guardar diccionary positions
         DictionaryWork.SaveDictionaryPosition(DictionaryForPositions);
        //recoger diccionario de posiciones
         DictionaryForPositions=DictionaryWork.TekeDictionaryPosition();

        //dictionary de sinonimos
        sinonymous = DictionaryWork.TekeDictionarySyn();
    }

    //metodo para devolver los item de busqueda
    public static SearchResult Query(string query)
    {
        //hallando una sugerencia para el query.
        Suggestion sgt = new Suggestion(query);
        string suggestion = sgt.suggestionForQuery(DictionaryForPositions);

        //...operadores...
        //obteniendo symbol para trabajar con los operadores.
        Symbol symbol = operators.GetSymbol(query, path).symbol;
        //obteniendo array de palabras del query sin operadores
       // string[] newQuery = operators.GetSymbol(query, path).pharse;
       string[] newQuery = suggestion.Split(' ');
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
        string[] snippet = WordInformation.snippet(newQuery, DictionaryForPositions, DocumentsInOrder, symbol);

        SearchItem[] items = new SearchItem[3];
        for (int i = 0; i < 3; i++)
        {
            items[i] = new SearchItem(scores[i].Item2, snippet[i], (float)scores[i].Item1);

        }
        return new SearchResult(items, suggestion);

            // SearchItem[] items1111 = new SearchItem[1] {
            //  new SearchItem("Not Found", "Not Found", 0.9f)};

            //  return new SearchResult(items1111, suggestion);
    }

}
