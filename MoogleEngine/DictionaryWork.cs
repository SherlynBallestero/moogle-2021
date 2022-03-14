using System.Text.Json;
using System.IO;
namespace MoogleEngine
{
    public class DictionaryWork
    {
        
        public DictionaryWork()
        {

        }
        //funcion para recoger en un dictionary el diccionario de sinonimos
        public static Dictionary<string, List<string>> TekeDictionarySyn()
        {
            string parent = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string path = Path.Join(parent, "DicctionarySyn" , "Synonymous.json");
            Dictionary<string,List<string>> sinonimos = new Dictionary<string, List<string>>();
            string fileContent = File.ReadAllText(path);                    
            return sinonimos = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(fileContent) ?? throw new Exception();
        }

        //funcion para recoger en un dictionary el diccionario de posiciones 
        public static Dictionary<string, List<List<int>>> TekeDictionaryPosition()
        {
            string path = Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "DictionaryPositions", "DicctionaryPositions.json");
            //Console.WriteLine("Direccion de recogida" + path);
            Dictionary<string, List<List<int>>> DictionaryForPositions = new Dictionary<string, List<List<int>>>();
            return DictionaryForPositions = JsonSerializer.Deserialize<Dictionary<string, List<List<int>>>>(json: File.ReadAllText(path)) ?? throw new Exception();
        }

        //funcion para guardar el dicionario de posiciones de las palabras en una carpeta
        public static void SaveDictionaryPosition(Dictionary<string, List<List<int>>> DictionaryForPositions)
        {
            string path = Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "DictionaryPositions", "DicctionaryPositions.json");
            
            File.WriteAllText(path, JsonSerializer.Serialize(DictionaryForPositions));
        }

        public static void SaveFileNames(List<string> names){
            string path = Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "FileNames", "FileNames.json");
            File.WriteAllText(path, JsonSerializer.Serialize(names));
        }

        public static List<string> TakeFileNames(){
            string path = Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "FileNames", "FileNames.json");
            List<string> names = new List<string>();
            return names = JsonSerializer.Deserialize<List<string>>(json: File.ReadAllText(path)) ?? throw new Exception();

        }
    }
}