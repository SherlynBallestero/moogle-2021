# Moogle!

![](MoogleEdited.jpg)
> Proyecto de Programación I. Facultad de Matemática y Computación. Universidad de La Habana. Curso 2021.

>>Sherlyn Ballestero Cruz c113.
## General.

**Un proceso de recuperación de información** comienza cuando un usuario hace una consulta al sistema. Una consulta a su vez es una afirmación formal de la necesidad de una información. En la recuperación de información una consulta no identifica únicamente a un objeto dentro de la colección. De hecho varios objetos pueden ser respuesta a una consulta con diferentes grados de relevancia. La mayoría de los sistemas de recuperación de información computan un ranking para saber cuán bien cada objeto responde a la consulta, ordenando los objetos de acuerdo a su valor de ranking. Los objetos con mayor valor son mostrados a los usuarios y el proceso puede tener otras iteraciones si el usuario desea refinar su consulta.

## Representación de la información y descripción general.
Para representar los documentos a tratar en Moogle! se crean y rellenan tres estructuras que almacenan los datos necesarios para las funcionalidades esenciales.
Estos son tres objetos del tipo Dictionary, donde en uno se guarda por cada palabra que existe entre todsos los documentos las posiciones en que se encuentran por cada uno, otro almacena los tf(cantidad de veces en que aparece esta cadena entre la cantidad de palabras que tiene el texto) y el tercero contiene a los IDF(logaritmo de la cantidad de documentos entre la cantidad de documentos en que aparece la palabra).
Al recibir la consulta lo primero que se hace en el procesamiento es obtener de el solo las letras minúsculas y números para tratar la sugerencia, luego se obtiene un objeto Symbol para trabajar los operadores y posteriormente se toma a partir de la sugerencia un array garantizando que no lleve elementos nulos o vacios, del que se adquieren al menos dos sinonimos por cada palabra. 
Estos dos array `newQuery` y `QuerySyn` son empleados para conseguir la importancia de los documentos. Se logra a partir del método `MV` en la clase `score`, donde luego en la conformación del valor por documento general a los sinonimos se le asigna un 20% de su valor y se suma al correspondiente ranking dado por `MV` en `newQuery` para asi conformar el resultado final de la importancia de la busqueda en cada archivo. Por último se ordenan de mayor a menor.
Luego se conforma un pedazo de texto que contenga las palabras que más relevancia tenga en la consulta y asi el usuario interprete si quiere o no un documento especifico, se asegura que este sea dado en el orden del ranking anteriormente analizado.
Por último se forman los `items` para devolver.

```cs

 for (int i = 0; i < scores.Count; i++)
        {
            //solo se agregan los documentos con score mayor que cero a los resultados de la busqueda.
            if (scores[i].Item1 > 0){
                items.Add(new SearchItem(scores[i].Item2.Substring(path.Length+1), snippet[i], (float)scores[i].Item1));
            }
        }
```    


## Modelación del ranking.

Para recuperar efectivamente los documentos relevantes por estrategias de recuperación de información, los documentos son transformados en una representación lógica de los mismos. Cada estrategia de recuperación incorpora un modelo específico para sus propósitos de representación de los documentos.

La modelación del proceso de recuperación de información de manera vectorial que se emplea en Moogle! surge por primera vez en 1968, por Gerard Salton y Lesk.

Este modelo tiene su base en el álgebra lineal multidimensional. Los términos indexados como los documentos se modelan como vectores, los cuales son llamados vectores-términos y vectores-documentos, respectivamente. El conjunto de vectores términos representa la base de dicho espacio vectorial. El peso de un término indexado en un documento representa la componente del documento asociada al correspondiente vector término de la base. Para determinar cuán relevante es un documento a una consulta se determina la magnitud del coseno del ángulo entre ellos que es una medida de la similitud existente entre dos vectores en un espacio.
En Moogle este proceso se lleva a cabo dentro de la clase `score.cs`:

```cs
  public class score
    {
  
        public score()
        ...
    }
```
En la importancia que adquieren los documentos por términos actúa además la utilización de operadores.

## Operadores.
Los operadores empleados en Moogle! son:
- Un símbolo `!` delante de una palabra (e.j., `"algoritmos de búsqueda !ordenación"`) indica que esa palabra **no debe aparecer** en ningún documento que sea devuelto.
- Un símbolo `^` delante de una palabra (e.j., `"algoritmos de ^ordenación"`) indica que esa palabra **tiene que aparecer** en cualquier documento que sea devuelto.
- Un símbolo `~` entre dos o más términos indica que esos términos deben **aparecer cerca**, o sea, que mientras más cercanos estén en el documento mayor será la relevancia. Por ejemplo, para la búsqueda `"algoritmos ~ ordenación"`, mientras más cerca están las palabras `"algoritmo"` y `"ordenación"`, más alto debe ser el `score` de ese documento.

`"Cómo los modelamos????"`
Dado una petición del usuario procedemos a anlizar los operadores que la afectan y se utiliza la clase `"Symbol.cs"` para definir el objeto que se quiere de vuelta. Es decir se aplica la función `"GetSymbol"` contenida en `"operators.cs"` para obtener un symbol y el conjunto de palabras que integran al query libre de operadores.
Un symbol definido por la clase Symbol.cs:

```cs
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
```

contiene tres componentes donde:

`banDocs` 
Es empleado para almacenar  los documentos que son afectados por los operadosres ^ (en el array yes) y !(en el array no). En la clase score luego de asignar el valor correspondiente al coseno entre los vectores documentos y vector término no se agregan a la solución  los documentos que se vetan(se dice que se veta un archivo cuando no se quiere en la respuesta).


`asterisks`
Las palabras que almacenamos en este dictionary serán las que contienen asteriscos en la consulta (la clave) y tendrán correspondida la cantidad. Su insidencia sobre el score se trabaja a la hora de calcuar el peso de los términos. Donde antes de calcular el peso se verifica si el término en el query contiene asteriscos y se le da un valor a `double increase` equivalente a 2^(cantidad de asteriscos que contiene), si no contiene `increase` vale 1.

`Closeness`
 Closeness(1) es una lista que contiene `(string t1, strieg t2)` donde t1 y t2 son las palabras que estan separadas por `~`. Luego en `MV` en la clase score se analiza en que documentos se encuentran con una separación inferior a 10 y cuantas veces, para modificar la importancia,dándole un incremento al valor de cada archivo multiplicando el resultado un número en escala logarítmica que trata cuantos documentos se tienen cada palabra que deben estar cercanas( a una distancia inferior a 10).  

 ## Sugerencia.

 El parámetro `suggestion` de la clase `SearchResult` es para darle una sugerencia al usuario cuando su búsqueda da muy pocos resultados. Esta sugerencia debe ser algo similar a la consulta del usuario pero que sí exista, de forma que si el usuario se equivoca, por ejemplo, escribiendo `"reculsibidá"`, y no aparece (evidentemente) ningún documento con ese contenido, le podamos sugerir la palabra `"recursividad"`.
 
 Se soluciona este aspecto en la clase `suggestion.cs` .Primero con la función `SimilarityInWords` se busca la similitud entre dos palabras. Nos enfrentamos a hallar algo similar al Edit Distance. El Edit Distance es la minima cantidad de caracteres que debemos cambiar agregar o eliminar de una palabra para obtener otra. En este caso a cada caracter se le hace corresponder un peso en escala logarítmica de derecha a izquierda para cada string. Luego trata de hacer  matches de caracteres tal que se maximice el peso. En caso de que matcheen se tendra en cuenta las posiciones relativas de los caracteres, en caso de que no solo se le aplica un porciento pequeño de su valor, y en el caso de eliminar o agregar caracteres no se suma ningun peso. Se le da más valor a los prefijos, se le da valor a las posiciones relativas de los caracteres que matcheen, se le da un valor pequeño a los cambios de caracter y no se le da valor a las eliminaciones o agregos de caracteres. La respuesta es el porciento de similitud. Luego se encuentra la sugerencia a partir del query y las similitudes con las palabras que se tienen en la colección de documentos.

 ## Sinónimos.
 Los sinonimos se obtienen de `Synonymous.json` guardado en la carpeta `DicctionarySyn`. 
 Al ser JSON un formato muy extendido para el intercambio de datos, se han desarrollado API para distintos lenguajes  que permiten analizar sintácticamente, generar, transformar y procesar este tipo de dato, es decir,podemos convertir objetos JSON en objetos del lenguaje. De este modo con la clase `class System.Text.Json.JsonSerializer` se trasforma el .json del directorio en un `Dictionary<string,List<string>>`, donde a cada palabra se le asigna sus sinónimos.
    Con este en la clase `WordInformation` en el metodo `AddSynonymous` se conforma `QuerySyn` con a lo sumo dos sinonimos de cada palabra de la busqueda del usuario y se asegura que estos sinonimos esten en la colección de documentos.
 

 ## Snippet.
 En la clase ` WordInformation` se encuentran las funciones `snippetForASpecificDocument`y `snippet`:

```cs
  public class WordInformation
    {
        ...

         public static string snippetForASpecificDocument(string[] files,string[] queryWords, Dictionary<string, (string[] t1, List<int[]> t2)> dictionary, string pathToDocument, Symbol symbol){...}
        ...
        public static string[] snippet(string[] query, Dictionary<string, (string[] t1, List<int[]> t2)> dictionary, string[] files, Symbol symbol){...}...
        
    }    

```

Con `snippetForASpecificDocument` se encuentra un segmento de codigo de un tamaño que se considere adecuado para cada documento.
Cuando el query solo contiene un término se procede extendiendonos a sus alrrededores tratando de tomar un segmento adecuado a partir de la posición de este en el texto y se retorna. De lo contrario se toma la palabra con más relevancia de la consulta y nos expandimos a su alrededor.
