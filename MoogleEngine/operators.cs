using System.Collections.Generic;
using System;
namespace MoogleEngine
{
    public class operators
    {
        public string query;
        //direccion hasta la carpeta que contiene la coleccion de documentos
        public string path;
        //this.positions dictionary que tiene por palabras string[](que almacena los documentos en el orden en que te 
        //da en los mismos indices de una lista las posiciones en que se encuentra dicha palabra en el correspondiente documento)
        public Dictionary<string, (string[] index1, List<int[]> index2)> positions;
        //constructor...
        public operators(string query, Dictionary<string, (string[], List<int[]>)> positions, string path)
        {
            this.query = query;
            this.positions = positions;
            this.path = path;
        }
        //methodos...
        ///<summary>
        ///en modificacion pero la talla es que me devuelva (1,2,3) donde: 
        /// (1)~ >Las palabras afectadas por ! y ^(o documentos vetados segun la aparicion de estos operadores,aun estamos pensando...)
        /// (2)~ >Las palabras afectadas por (*,|*|).
        /// (3)~ >Palabras a las que le corresponde calcular la cercania.
        ///Ademas nos aporta con una variable por referencia la devolucion de un query limpio(sin operadores)
        ///</summary>
        public static (Symbol, string pharse) GetSymbol(string query)
        {
            HelperMethods hM = new HelperMethods(this.path);
            string phrase = hM.TokenWords(query);
            string[] words = query.Split(' ');
            int count = 0;
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
                        asterisks.Add(word, CountAsterisks(word));

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

            (string[] t1, string[] t2) banDocs = (hM.TokenWords(yes), hM.TokenWords(yes));
            Symbol answer = new Symbol(banDocs, asterisks, Closeness);
            return (answer, phrase);
        }

        ///<summary>
        ///metodo para determinar las cercanias entre dos palabras dada sus posiciones(para el operador "~")
        ///retorna las distancias mas cortas por documento a partir de las palabras que se le da como imput.
        ///</summary>
        public static List<(int closeness, string document)> Closeness(Symbol symbol)
        {
            HelperMetods hM = new HelperMetods(this.path);
            List<(string t1, string t2)> words = symbol.Closeness();
            string[] files = Directory.GetFiles(this.path, "*.txt");
            //pasamos por cada carpeta haciendo el proceso de obtener las distancias minimas entre las posiciones 
            //en que se encuentran las palabras que nos interesa.
            int distance = 0;
            List<(int, string)> distanceForDocument = new List<int>();
            for (int i = 0; i < words.Count; i++)
            {
                foreach (string file in files)
                {
                    int[] aux1 = this.position[words[i].t1].index2;
                    int[] aux2 = this.position[words[i].t2].index2;
                    int min = int.MaxValue;
                    for (int i = 0; i < aux1.Length; i++)
                    {
                        for (int j = i + 1; j < aux2.Length; j++)
                        {
                            distance = Math.Abs(aux2[j] - aux1[i]);
                            min = Math.Min(distancia, min);
                        }
                    }
                    distanceForDocument.Add(min, file[i]);
                    min = int.MaxValue;
                }
            }
            return distanceForDocument;
        }
        ///<summary>
        ///Metodo para obtener los documentos que se vetaran,asi poder satisfascer las condiciones de los operadores ! y ^
        ///</summary>
        public static list<string> BanDocuments(Symbol symbol)
        {
            List<string> answer=new List<string>();
            HelperMetods hM = new HelperMetods(this.path);
            string[] files = Directory.GetFiles(this.path, "*.txt");
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
                            if (!hM.Find(yes[y], file))
                            {
                                indexYes = false;
                            }
                        }
                        if (n < no.Length)
                        {
                            if (hM.Find(no[n], file))
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
                        if (hM.Find(no[n], file))
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
                        if (!hM.Find(yes[y], file))
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

    }
}