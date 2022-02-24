namespace MoogleEngine;


public static class Moogle
{  
    //metodo para devolver los item de busqueda
    public static SearchResult Query(string query) {
        //ruta de acceso a la coleccion de documentos
         string path=Directory.GetCurrentDirectory()+"\\..\\Content\\";
        //nombre de los archivos en el path
        string[] filesNames=Directory.GetFileSystemEntries(path);
        //obtener direcciones hacia los archivos contenidos en la coleccion
        string[] filesPath=Directory.GetFiles(path);
        //implementar lo de los sinonimos,que dios me ayude y no se me olvide que ouse esto aqui
        
        //instancia de la clase wordInfp para obtener los diccionarios
        WordInformation wordInfo=new WordInformation(path);
        //Obteniendo Dictionary con todas las palabras de la coleccion de documentos y sus 
        //respectivas posiciones por documento
        Dictionary<string, (string[], List<int[]>)> DictionaryForPositions=wordInfo.FillDictionary().t2;
        //Obteniendo Dictionary con todas las palabras de la coleccion de documentos y sus 
        //respectivas frecuencias por documento
        Dictionary<string, (string[], List<double>)> DictionaryForTF=wordInfo.FillDictionary().t1;
        //...operadores...
        //obteniendo symbol para trabajar con los operadores.
        Symbol symbol=operators.GetSymbol(query,path).symbol;
        //obteniendo array de palabras del query sin operadores
        string[] newQuery=operators.GetSymbol(query,path).pharse;
        //obteniendo lista con las distancias mas cercanas de las posiciones de las palabras afectadas por el operador"~"
        List<(int closeness, string document)> DistanceInWordsWhithOperator=operators.Closeness(symbol,path,DictionaryForPositions);
        //lista de score por nombre de documento ordenados de mayor a menor
        List<(double, string)> scores=score.MV(DistanceInWordsWhithOperator,filesPath,newQuery,query,symbol,DictionaryForPositions,path); 
        //cacho de codigo por documento
        string[] snippet=WordInformation.snippet(newQuery,DictionaryForPositions,filesPath,symbol);
        // Modifique este método para responder a la búsqueda

        // SearchItem[] items = new SearchItem[3] {
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        // };

        
         SearchItem[] items = new SearchItem[10]; 
        for(int i=0;i<10;i++)
        {
           SearchItem aux=new SearchItem(scores[i].Item2,"algo",Convert.ToSingle( scores[i].Item1));
           items[i]=aux;
        }
        return new SearchResult(items, query);
    }

}
