namespace MoogleEngine
{
    public class Suggestion
    {
        public string query;
        public Suggestion(string query)
        {
            this.query = query;
        }
        ///<summary>
        ///Metodo que retorna la similitud entre dos palabras. A cada caracter le haremos
        ///corresponder un peso en escala logaritmica de derecha a izquierda para cada string 
        ///retorna el por ciento de similitud.
        ///</summary>
        double SimilarityInWords(string s1, string s2)
        {
            //Tamaño de la primera palabra.
            int n = s1.Length;
            //Tamaño de la segunda palabra.
            int m = s2.Length;

            //Si algun string tiene tamaño 0 entonces no hay similitud.
            if (n == 0 || m == 0) return 0;

            //Pesos en escala logaritmica para el primer string.
            double[] v1 = new double[n];
            //Pesos en escala logaritmica para el segundo string.
            double[] v2 = new double[m];
            //dp[i,j] indica el maximo match de pesos entre el prefijo de tamaño i del primer string
            //y el prefijo de tamaño j del segundo string.
            double[,] dp = new double[n + 1, m + 1];

            //Aqui almacenaremos la sumatoria de todos los pesos de ambos string
            //para luego dividir el maximo match de pesos entre la suma de todo lo cual devolvera
            //un numero en el intervalo [0,1]
            double tot = 0;

            //Asignando pesos en escala logaritmica para el primer string
            for (int i = 0; i < n; i++)
            {
                //Aqui usaremos la funcion de logaritmo natural sobre la posicion relativa
                //Usaremos como limite inferior 0.5 y los demas pesos variaran entre 0.5 y 1
                v1[i] = Math.Log((Math.E - 1.0) * (1.0 - (double)i / n) + 1.0) / 2.0 + 0.5;
                //Sumando los pesos
                tot += v1[i];
            }

            //Asignando pesos en escala logaritmica para el segundo string
            for (int i = 0; i < m; i++)
            {
                //Aqui se usara la funcion de logaritmo natural sobre la posicion relativa
                //Usaremos como limite inferior 0.5 y los demas pesos variaran entre 0.5 y 1
                v2[i] = Math.Log((Math.E - 1.0) * (1.0 - (double)i / m) + 1.0) / 2.0 + 0.5;
                //Sumando los pesos
                tot += v2[i];
            }

            //Calculando la informacion
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    //Calculando eliminando caracter i del primer string
                    dp[i, j] = Math.Max(dp[i, j], dp[i - 1, j]);
                    //Calculando eliminando caracter j del segundo string
                    dp[i, j] = Math.Max(dp[i, j], dp[i, j - 1]);

                    //Calculando matcheando caracter i del primer string
                    //con caracter j del segundo string si son iguales
                    //en caso contrario cambiar uno por el otro
                    if (s1[i - 1] == s2[j - 1])
                    {
                        //Hallando el porciento logaritmico de la diferencia absoluta de
                        //las posiciones relativas usando logaritmo natural
                        double posdif = 1 / Math.Log(Math.E + Math.Abs((double)i / n - (double)j / m));
                        //Sumando pesos de caracteres y aplicando el porciento posicional
                        dp[i, j] = Math.Max(dp[i, j], dp[i - 1, j - 1] + (v1[i - 1] + v2[j - 1]) * posdif);
                    }
                    else
                    {
                        //Hallando el porciento logaritmico de la diferencia absoluta de
                        //las posiciones relativas usando logaritmo natural
                        double posdif = 1 / Math.Log(Math.E + Math.Abs((double)i / n - (double)j / m));
                        //Sumando pesos de caracteres, aplicando el porciento posicional y
                        //aplicando el 10% por ser un cambio de caracter
                        dp[i, j] = Math.Max(dp[i, j], dp[i - 1, j - 1] + (v1[i - 1] + v2[j - 1]) * posdif * 0.1);
                    }
                }
            }

            //Hallando el porciento de similitud
            //Esto significa el maximo match de pesos dividido entre
            //la suma de los pesos, tambien nos aseguramos de devolver un
            //numero en el intervalo [0,1] aunque la funcion de por si esta
            //hecha para devolver un numero en ese intervalo, solo lo hacemos
            //por precaucion
            return Math.Min(Math.Max(dp[n, m] / tot, 0), 1);
        }
        ///<summary>
        ///funcion para encontrat la sugerencia a partur de las similitudes de las palabras que tenemos en 
        ///los documentos con en query ingresado.
        ///</summary>
        public string suggestionForQuery( Dictionary<string,  List<List<int>>> dictionary)
        {
            Suggestion sgt=new Suggestion(this.query);
            string[] words=query.Split(' ');
            string answer="";
            double max=-1;
            string aux="";
            foreach (string word in words)
            {
                //Se buscan palabras similares solo cuando no se encuentra alguna de las palabras del query en 
                //los documentos
                if (!dictionary.ContainsKey(word))
                {
                    foreach (string key in dictionary.Keys)
                    {
                        //nos quedamos con la primera palabra similar que encontramos.
                        if ((SimilarityInWords(word, key) >max))
                        {
                            aux= key;
                            max=SimilarityInWords(word, key);
                        }
                    }
                    answer+=aux+" ";
                    max=-1;
                }
                else
                {
                    answer+=word+" ";
                }

            }
            return answer;
        }
    }
}