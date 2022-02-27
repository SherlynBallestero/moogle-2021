# Moogle!

![](moogle.png)
> Proyecto de Programación I. Facultad de Matemática y Computación. Universidad de La Habana. Curso 2021.

>Sherlyn Ballestero Cruz c113.
## General.

**Un proceso de recuperación de información** comienza cuando un usuario hace una consulta al sistema. Una consulta a su vez es una afirmación formal de la necesidad de una información. En la recuperación de información una consulta no identifica únicamente a un objeto dentro de la colección. De hecho varios objetos pueden ser respuesta a una consulta con diferentes grados de relevancia. La mayoría de los sistemas de recuperación de información computan un ranking para saber cuán bien cada objeto responde a la consulta, ordenando los objetos de acuerdo a su valor de ranking. Los objetos con mayor ranking son mostrados a los usuarios y el proceso puede tener otras iteraciones si el usuario desea refinar su consulta.

## Modelacion del ranking.

Para recuperar efectivamente los documentos relevantes por estrategias de recuperación de información, los documentos son transformados en una representación lógica de los mismos. Cada estrategia de recuperación incorpora un modelo específico para sus propósitos de representación de los documentos.

La modelación del proceso de recuperación de información de manera vectorial que se emplea en Moogle! surge por primera vez en 1968, por Gerard Salton y Lesk.

Este modelo tiene su base en el álgebra lineal multidimensional. Los términos indexados como los documentos se modelan como vectores, los cuales son llamados vectores-términos y vectores-documentos, respectivamente. El conjunto de vectores términos representa la base de dicho espacio vectorial. El peso de un término indexado en un documento representa la componente del documento asociada al correspondiente vector término de la base. Para determinar cuan relevante es un documento a una consulta se determina la magnitud del coseno del ángulo entre ellos que es una medida de la similitud existente entre dos vectores en un espacio.
En Moogle este proceso se lleva a cabo dentro de la clase score.cs:

```cs
  public class score
    {
  
        public score()
        ...
    }
```
En el score que adquieren los documentos por términos actúa además la utilización de operadores.

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

Es empleado para almacenar  los documentos que son afectados por los operadosres ^ (en el array yes) y !(en el array no). En la clase score luego de asignar el valor correspondiente al coseno entre los vectores documentos y vector término no se agregan a la solución  los documentos que se vetan,se dice que se vetan dado que son los documentos que no se quieren en la respuesta.

```cs
    //haremos cero el score de los documentos vetados
            List<string> BanDocuments = operators.BanDocuments(symbol,route);
            if (BanDocuments.Count!=0)
            {
                for (int i = 0, j = 0; i < cosin.Length; i++)
                {
                    while (j < BanDocuments.Count)
                    {
                        if (BanDocuments[j] != file[i])
                        {
                            //agregamos a la solucion los documentos que no esten vetados con sus respectivos path.
                            answer.Add((cosin[i], file[i]));
                        }
                        j++;
                    }
                }
            }
```   

`asterisks`

`Closeness`


















