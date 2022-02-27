using System.Collections.Generic;
namespace MoogleEngine
{
    public class Symbol
    {
        //para almacenar en yes las palabras afectadas por el operador ^(yes porque esas palabras deben aparecer en el documento)
        //en no guardamos las palabras afectadas por el operador !
        public (string[] yes,string[] no) banDocs;
        public Dictionary<string, int> asterisks;
       public List<(string,string)> Closeness;
        //constructor
        public Symbol((string[] yes,string[] no) banDocs, Dictionary<string, int> asterisks,List<(string,string)> Closeness)
        {
            this.banDocs=banDocs;
            this.asterisks=asterisks;
            this.Closeness=Closeness;
        }
    }
}