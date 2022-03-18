using System.Collections.Generic;
using System;
namespace MoogleEngine
{
    public class operators
    {

       //constructor...
        public operators()
        {
            
        }
        //methodos...
        ///<summary>
        ///en modificacion pero la talla es que me devuelva (1,2,3) donde: 
        /// (1)~ >Las palabras afectadas por ! y ^(o documentos vetados segun la aparicion de estos operadores,aun estamos pensando...)
        /// (2)~ >Las palabras afectadas por (*,|*|).
        /// (3)~ >Palabras a las que le corresponde calcular la cercania.
        ///Ademas nos aporta con una variable por referencia la devolucion de un query limpio(sin operadores)
        ///</summary>
        public static Symbol GetSymbol(string query,string route, Dictionary<string, List<List<int>>> positions)
        {
            Operator Op = new Operator(query);

            string yes = " ";
            string no = " ";
            string aux="";

            List<(string, string)> Closeness = new List<(string, string)>();
            Dictionary<string, int> asterisks = new Dictionary<string, int>();

            foreach(var x in Op.MustExistWords)
            {
                Suggestion sg=new Suggestion(x);

                aux=sg.suggestionForQuery(positions);
                
                yes += WordInformation.GetWordsFromString(aux)[0] + " ";
            }

            foreach(var x in Op.MustNotExistWords)
            {
                Suggestion sg=new Suggestion(x);

                aux=sg.suggestionForQuery(positions);
                
               
                no += WordInformation.GetWordsFromString(aux)[0] + " ";
            }

            foreach(var x in Op.PairWords)
            {
                Suggestion sg=new Suggestion(x.Item1);
                Suggestion sg2=new Suggestion(x.Item2);

                aux=sg.suggestionForQuery(positions);
                string  aux2=sg2.suggestionForQuery(positions);
                Closeness.Add((WordInformation.GetWordsFromString(aux)[0], WordInformation.GetWordsFromString(aux2)[0]));
            }
            
            foreach(var x in Op.ImportanceWords)
            {
                if(x.Value != 0)
                {                      
                    Suggestion sg=new Suggestion(x.Key);
                    aux=sg.suggestionForQuery(positions);
                    asterisks.Add(WordInformation.GetWordsFromString(aux)[0], x.Value);
                }
            }

            if(yes==" ")yes="notElements";
            if(no==" ")no="notElements";
            if(Closeness.Count==0)Closeness.Add(("notElements","notElements"));
            if(asterisks.Count==0)asterisks.Add("notElements",-1);

            string[] ttt1 = new string[]{};
            string[] ttt2 = new string[]{};

            if(yes != "notElements")
            {
                ttt1 = WordInformation.GetWordsFromString(yes).ToArray();
            }
            else
            {
                ttt1 = new string[]{"notElements"};
            }

            if(no != "notElements")
            {
                ttt2 = WordInformation.GetWordsFromString(no).ToArray();
            }
            else
            {
                ttt2 = new string[]{"notElements"};
            }

            (string[] t1, string[] t2) banDocs = (ttt1, ttt2);
            Symbol answer = new Symbol(banDocs, asterisks, Closeness);
            return answer;
        }

        ///<summary>
        ///Función que determina las cercanias entre dos palabras dadas sus posiciones(para el operador "~")
        ///retorna las distancias más cortas por documento a partir de las palabras que se le da como imput.
        ///</summary>
        public static List<(int closeness, string document)> Closeness(Symbol symbol,string route,  Dictionary<string, List<List<int>>> positions)
        {
           
            List<(string t1, string t2)> words = symbol.Closeness;
           List<(int, string)> distanceForDocument = new List<(int,string)>();
            if(words[0].t1=="notElements")
            {
                distanceForDocument.Add((0,"notElements"));
                return distanceForDocument;
            }
            string[] files = Directory.GetFiles(route, "*.txt");
            //se  recorre cada carpeta haciendo el proceso de obtener las distancias minimas entre las posiciones 
            //en que se encuentran las palabras que nos interesa.
            int distance = 0;
            
            for (int i = 0; i < words.Count; i++)
            {
                foreach (string file in files)
                {
                    //procedimiento para asegurarnos de buscar las posiciones de las palabras afectadas por el operador
                    //de cercania en casi de que no la encontremos entre nuestros documentos a partir del metodo hecho para sugerencias.
                    List<int> aux1=new List<int>();
                    if(positions.ContainsKey(words[i].t1))
                    {
                        aux1 = positions[words[i].t1][i];
                       
                    }
                    else
                    {
                        Suggestion sg=new Suggestion(words[i].t1);
                        string suggestionInOperators1=sg.suggestionForQuery(positions);
                        suggestionInOperators1=WordInformation.GetWordsFromString(suggestionInOperators1)[0];
                         aux1 = positions[suggestionInOperators1][i];
                    }

                    
                   

                     List<int> aux2=new List<int>();
                    if(positions.ContainsKey(words[i].t2))
                    {
                         aux2 = positions[words[i].t2][i];
                    }
                    else
                    {
                        Suggestion sg=new Suggestion(words[i].t2);
                        string suggestionInOperators2=sg.suggestionForQuery(positions);
                        suggestionInOperators2=WordInformation.GetWordsFromString(suggestionInOperators2)[0];
                         aux1 = positions[suggestionInOperators2][i];
                    }

                    int min = int.MaxValue;
                    for (int j = 0; j < aux1.Count; j++)
                    {
                        for (int k = i + 1; k < aux2.Count; k++)
                        {
                            distance = Math.Abs(aux2[k] - aux1[j]);
                            min = Math.Min(distance, min);
                        }
                    }
                    distanceForDocument.Add((min, file));
                    
                    min = int.MaxValue;
                }
            }
            //margesort para listas y devolver distance ordenado de mayor a menor
            HelperMethods.MargeSortToList(distanceForDocument);

            return distanceForDocument;
        }
        ///<summary>
        ///Metodo para obtener los documentos que se vetaran,asi poder satisfascer las condiciones de los operadores ! y ^
        ///</summary>
        public static List<string> BanDocuments(Symbol symbol,string route,Dictionary<string,List<List<int>>> positions)
        {
            List<string> answer = new List<string>();
           //// HelperMetods hM = new HelperMetods(this.path);
            string[] files = Directory.GetFiles(route, "*.txt");
            bool[] marks = new bool[files.Length];
            //en yes encontramos las palabras que deben aparecer en el documento para no ser vetado y rn no las que no deben
            //aparecer para no ser vetados.
            (string[] yes, string[] no) b = symbol.banDocs;
            string[] yes = b.yes;
            string[] no = b.no;
            //revisamos enntre las posiciones d elas palabras de yes y no que tenemos por documento y asi en dependencia de si la queremos o no la
            //la marcamos en true cuando no la queremos entre los documentos de retorno en la busqueda. 
            if(yes[0] != "notElements")
            {
                for(int i = 0 ; i < files.Length ; i++)
                {
                    foreach(var x in yes)
                    {
                        if(positions.ContainsKey(x)){
                            if(positions[x][i][0] == -1)
                            {
                                marks[i] = true;
                            }
                        }
                    }
                }
            }

            if(no[0] != "notElements")
            {
                for(int i = 0 ; i < files.Length ; i++)
                {
                    foreach(var x in no)
                    {
                        if(positions.ContainsKey(x)){
                            if(positions[x][i][0] != -1)
                            {
                                marks[i] = true;
                            }
                        }
                    }
                }
            }

            //marks va a ser true en i cuando vetamos al documento en la posicion i.
            for(int i=0;i<marks.Length;i++)
            {
                if(marks[i])answer.Add(files[i]);
            }
            if(answer is null)answer.Add("notElements");
            return answer;
        }
   

    }
}