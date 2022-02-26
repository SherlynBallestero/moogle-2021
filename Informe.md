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


















