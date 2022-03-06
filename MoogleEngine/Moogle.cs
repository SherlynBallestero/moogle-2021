namespace MoogleEngine;


public static class Moogle
{

    //metodo para devolver los item de busqueda
    public static SearchResult Query(string query)
    {
        //ruta de acceso a la coleccion de documentos
        string path = Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Content");

        //nombre de los archivos en el path
        string[] filesNames = Directory.GetFileSystemEntries(path);
        //obtener direcciones hacia los archivos contenidos en la coleccion
        string[] filesPath = Directory.GetFiles(path, "*txt");


        //instancia de la clase wordInfp para obtener los diccionarios
        WordInformation wordInfo = new WordInformation(path);
        //Obteniendo Dictionary con todas las palabras de la coleccion de documentos y sus 
        //respectivas posiciones por documento
        Dictionary<string, List<List<int>>> DictionaryForPositions = wordInfo.FillDictionary().t2;
        //guardar diccionary positions
         DictionaryWork.SaveDictionaryPosition(DictionaryForPositions);
        //recoger diccionario de posiciones
         DictionaryForPositions=DictionaryWork.TekeDictionaryPosition();


        //Obteniendo Dictionary con todas las palabras de la coleccion y sus 
        // frecuencias por documento(convertir en tfidf)
        //////// Dictionary<string, (string[], List<double>)> DictionaryForTFidf=wordInfo.FillDictionary().t1;
        //dictionary de sinonimos
        Dictionary<string, List<string>> sinonymous = DictionaryWork.TekeDictionarySyn();

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
        List<(double, string)> scores = score.MV(DistanceInWordsWhithOperator, filesPath, newQuery, suggestion, symbol, DictionaryForPositions, path);
   
   

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
