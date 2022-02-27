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
        public static (Symbol symbol,string[] pharse) GetSymbol(string query,string route)
        {
           // string[] phrase = query.Split(' ');
            string[] words = query.Split(' ');
            //para saber a cual palabra ya sin los operadores corresponde la palabra del query que se analiza
            int count = -1;
            char[] Symbol = { '!', '^', '*', '~' };
            string yes = " ";
            string no = " ";
            List<(string, string)> Closeness = new List<(string, string)>();
            Dictionary<string, int> asterisks = new Dictionary<string, int>();
            foreach (string word in words)
            {
                count++;
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == Symbol[0])
                    {
                        no += word + " ";
                        break;
                    }
                    else if (word[i] == Symbol[1])
                    {
                        yes += word + " ";
                        break;
                    }
                    else if (word[i] == Symbol[2])
                    {
                        //implementar cuenta asteriscos
                        //count= CountAsterisks(word);
                       if (!asterisks.ContainsKey(words[count]))
                         asterisks.Add(words[count], CountAsterisks(word));

                    }
                }
                if (word == Symbol[3].ToString())
                {
                    Closeness.Add((words[count - 2], words[count + 1]));

                }
            }
            //para evitar que sean null si no hay palabras afectadas por ^ y !
            if(yes==" ")yes="notElements";
            if(no==" ")no="notElements";
            if(Closeness.Count==0)Closeness.Add(("notElements","notElements"));
            if(asterisks.Count==0)asterisks.Add("notElements",-1);

            (string[] t1, string[] t2) banDocs = (HelperMethods.TokenWords(yes), HelperMethods.TokenWords(yes));
            Symbol answer = new Symbol(banDocs, asterisks, Closeness);
            return (answer,words);
        }

        ///<summary>
        ///Función que determina las cercanias entre dos palabras dadas sus posiciones(para el operador "~")
        ///retorna las distancias más cortas por documento a partir de las palabras que se le da como imput.
        ///</summary>
        public static List<(int closeness, string document)> Closeness(Symbol symbol,string route,  Dictionary<string, (string[] index1, List<int[]> index2)> positions)
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
                    int[] aux1 = positions[words[i].t1].index2[i];
                    int[] aux2 = positions[words[i].t2].index2[i];
                    int min = int.MaxValue;
                    for (int j = 0; j < aux1.Length; j++)
                    {
                        for (int k = i + 1; k < aux2.Length; k++)
                        {
                            distance = Math.Abs(aux2[k] - aux1[j]);
                            min = Math.Min(distance, min);
                        }
                    }
                    distanceForDocument.Add((min, file));
                    
                    min = int.MaxValue;
                }
            }
            //margesort para listas y devolver distance ordenado de mayyor a menor
            HelperMethods.MargeSortToList(distanceForDocument);

            return distanceForDocument;
        }
        ///<summary>
        ///Metodo para obtener los documentos que se vetaran,asi poder satisfascer las condiciones de los operadores ! y ^
        ///</summary>
        public static List<string> BanDocuments(Symbol symbol,string route)
        {
            List<string> answer=new List<string>();
           //// HelperMetods hM = new HelperMetods(this.path);
            string[] files = Directory.GetFiles(route, "*.txt");
            bool[] marks = new bool[files.Length];
            //en yes encontramos las palabras que deben aparecer en el documento para no ser vetado y rn no las que no deben
            //aparecer para no ser vetados.
            (string[] yes, string[] no) b = symbol.banDocs;
            string[] yes = b.yes;
            string[] no = b.no;
            int y = 0;
            int n = 0;
            for(int i=0;i<marks.Length;i++)
                marks[i]=true;
            bool indexYes=true;
            bool indexNo=true;
            if (yes[0] != "notElements" && no[0] != "notElements")
            {
                while (y < yes.Length || n < no.Length)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (y < yes.Length)
                        {
                            if (!HelperMethods.Find(yes[y], files[i]))
                            {
                                indexYes = false;
                            }
                        }
                        if (n < no.Length)
                        {
                            if (HelperMethods.Find(no[n], files[i]))
                            {
                                indexNo = false;
                            }

                        }
                        if (!indexNo || !indexYes) marks[i] = false;
                    }
                    y++; n++;
                }
            }
            else if(yes[0] == "notElements" && no[0] != "notElements")
            {
                while ( n < no.Length)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (HelperMethods.Find(no[n], files[i]))
                            marks[i]=false;
                            
                    }
                    n++;
                }
            }
            else if(yes[0] != "notElements" && no[0] == "notElements")
            {
                while ( y < yes.Length)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (!HelperMethods.Find(yes[y], files[i]))
                            marks[i]=false;
                            
                    }
                    y++;
                }
            }
            //marks va a ser falso en i cuando vetamos al documento en la posicion i.
            for(int i=0;i<marks.Length;i++)
            {
                if(!marks[i])answer.Add(files[i]);
            }
            if(answer is null)answer.Add("notElements");
            return answer;
        }
        public static int CountAsterisks(string word)
        {
            int count=0;
            for(int i=0;i<word.Length;i++)
            {
                if(word[i]=='*')count++;
                else break;
            }
            return count;
        }

    }
}