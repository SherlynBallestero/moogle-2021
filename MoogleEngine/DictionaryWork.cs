using System.Text.Json;
namespace MoogleEngine
{
    public class DictionaryWork
    {
        public DictionaryWork()
        {

        }
        //funcion para recoger en un dictionary el diccionario de sinonimos
        public static Dictionary<string, List<string[]>> TekeDictionarySyn()
        {
            string path = Directory.GetCurrentDirectory() + "\\..\\DicctionarySyn\\";
            Dictionary<string, List<string[]>> sinonimos = new Dictionary<string, List<string[]>>();
            return sinonimos = JsonSerializer.Deserialize<Dictionary<string, List<string[]>>>(json: File.ReadAllText(path)) ?? throw new Exception();
        }
        //funcion para recoger en un dictionary el diccionario de posiciones 
        public static  Dictionary<string, (string[], List<int[]>)> TekeDictionaryPosition()
        {
            string path = Directory.GetCurrentDirectory() + "\\..\\DicctionaryPositions\\";
            Dictionary<string, (string[], List<int[]>)> DictionaryForPositions=new Dictionary<string, (string[], List<int[]>)>();
            return  DictionaryForPositions= JsonSerializer.Deserialize<Dictionary<string, (string[], List<int[]>)>>(json: File.ReadAllText(path)) ?? throw new Exception();
        }
        //funcion para guardar el dicionario de posiciones de las palabras en una carpeta
        public static void SaveDictionaryPosition(Dictionary<string, (string[], List<int[]>)> DictionaryForPositions)
        {
            string path = Directory.GetCurrentDirectory() + "\\..\\DicctionaryPositions\\";
            File.WriteAllText(path, JsonSerializer.Serialize(DictionaryForPositions));
        }
    }
}