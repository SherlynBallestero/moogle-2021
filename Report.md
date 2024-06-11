
# Moogle!
![](MoogleEdited.jpg)

## General Overview

**An information retrieval process** begins when a user submits a query to the system. A query, in turn, represents a formal statement of the need for information. In information retrieval, a query doesn't uniquely identify a single object within the collection. In fact, multiple objects can be potential responses to a query, each with varying degrees of relevance. Most information retrieval systems compute a ranking to determine how well each object responds to the query, ordering them based on their ranking value. Objects with higher values are presented to users, and the process can involve further iterations if the user wishes to refine their query.

## Information Representation and General Description

To represent the documents processed by Moogle!, three structures are created and populated with essential data. These structures are dictionaries, where:

1. One dictionary stores the positions of each word across all documents.
2. Another dictionary stores the term frequency (TF), which represents how many times a particular word appears in a document relative to the total number of words in that document.
3. The third dictionary contains inverse document frequency (IDF), which is the logarithm of the ratio between the total number of documents and the number of documents in which a word appears.

When a query is received, the following steps are taken for processing:

1. Extract lowercase letters and numbers from the query for further processing.
2. Obtain a Symbol object to handle operators.
3. Create an array from the query, ensuring it contains no null or empty elements. For each word, acquire at least two synonyms.
4. Two arrays, `newQuery` and `QuerySyn`, are used to determine document importance. The `MV` method in the `score` class calculates the importance. For synonyms, 20% of their value is added to the corresponding ranking obtained from `MV` in `newQuery`. The result represents the overall importance of the search in each file, sorted from highest to lowest.
5. Generate a snippet of text containing the most relevant words from the query. This helps users decide whether they want a specific document, presented in the order of the previously analyzed ranking.
6. Finally, create the `items` to return the search results.

```cs
for (int i = 0; i < scores.Count; i++)
{
    // Only add documents with a score greater than zero to the search results.
    if (scores[i].Item1 > 0)
    {
        items.Add(new SearchItem(scores[i].Item2.Substring(path.Length + 1), snippet[i], (float)scores[i].Item1));
    }
}
```

Certainly! Here's the English translation of the provided text:

---

## Ranking Modeling

To effectively retrieve relevant documents using information retrieval strategies, documents are transformed into a logical representation. Each retrieval strategy incorporates a specific model for document representation.

The vectorial modeling process employed in Moogle! for information retrieval was first introduced in 1968 by Gerard Salton and Lesk.

This model is based on multidimensional linear algebra. The terms indexed as documents are modeled as vectors, known as term vectors and document vectors, respectively. The set of term vectors represents the basis of the vector space. The weight of an indexed term in a document corresponds to the component of the document associated with the corresponding term vector from the base. To determine how relevant a document is to a query, the magnitude of the cosine of the angle between them is calculated—a measure of the similarity between two vectors in the space.

In Moogle, this process is carried out within the `score.cs` class:

```cs
public class score
{
    // Implementation details...
}
```

Additionally, the importance of documents based on terms is influenced by the use of operators.
Certainly! Here's the English translation of the provided text:

---

## Operators

The operators used in Moogle! are as follows:

1. A `!` symbol preceding a word (e.g., `"search algorithms !sorting"`) indicates that the specified word **must not appear** in any returned document.
2. A `^` symbol preceding a word (e.g., `"algorithms ^sorting"`) indicates that the specified word **must appear** in any returned document.
3. A `~` symbol placed between two or more terms indicates that those terms should **appear close together**. In other words, the closer the words are in the document, the higher the relevance. For example, in the search query `"algorithms ~ sorting"`, the score of a document increases if the words "algorithms" and "sorting" are closer together.

### How Are They Modeled?

When a user submits a request, we analyze the operators affecting it. The `Symbol.cs` class is used to define the desired object. Specifically, we apply the `"GetSymbol"` function from `"operators.cs"` to obtain a symbol and the set of words that make up the query, excluding operators.

A `Symbol` defined by the `Symbol.cs` class contains three components:

```cs
public class Symbol
{
    // To store the words affected by the ^ operator (yes, because these words must appear in the document)
    // In no, we store the words affected by the ! operator
    public (string[] yes, string[] no) banDocs;
    public Dictionary<string, int> asterisks;
    public List<(string, string)> Closeness;

    // Constructor
    public Symbol((string[] yes, string[] no) banDocs, Dictionary<string, int> asterisks, List<(string, string)> Closeness)
    {
        this.banDocs = banDocs;
        this.asterisks = asterisks;
        this.Closeness = Closeness;
    }
}
```

Certainly! Here's the English translation of the provided text:

---

## Components Explanation

1. **`banDocs`**:
   - This component is used to store documents affected by the `^` operator (in the `yes` array) and the `!` operator (in the `no` array).
   - In the `score` class, after calculating the cosine value between document vectors and term vectors, documents that are vetoed (i.e., not desired in the response) are excluded from the solution.

2. **`asterisks`**:
   - The words stored in this dictionary contain asterisks in the query (the keys), and their corresponding values represent the quantity of asterisks.
   - The impact on the score is considered during term weight calculation. Before calculating the weight, it's checked whether the query term contains asterisks. If it does, a `double increase` value is assigned, equivalent to 2 raised to the power of the number of asterisks. If it doesn't contain asterisks, `increase` remains 1.

3. **`Closeness`**:
   - `Closeness(1)` is a list containing pairs `(string t1, string t2)`, where `t1` and `t2` are words separated by `~`.
   - In the `MV` method within the `score` class, it analyzes which documents have a separation of less than 10 between these words and how many times they appear. The importance is modified by incrementing the value for each document, multiplied by a logarithmic scale factor based on how many documents contain each word that should be close (within a distance of less than 10).

## Suggestion

The `suggestion` parameter in the `SearchResult` class provides a suggestion to the user when their search yields very few results. This suggestion should be similar to the user's query but must exist. For example, if the user mistakenly types `"reculsibidá"` and no documents with that content are found (obviously), we can suggest the word `"recursividad"`.

This aspect is addressed in the `suggestion.cs` class. First, the `SimilarityInWords` function seeks similarity between two words. We aim to find something akin to the Edit Distance. The Edit Distance represents the minimum number of character changes (additions, deletions, or substitutions) required to transform one word into another. In this case, each character is assigned a weight logarithmically from right to left for each string. We attempt to match characters to maximize the weight. If characters match, we consider their relative positions. If they don't match, a small percentage of their value is applied. No weight is added for character deletions or additions. Prefixes are given more weight, relative character positions matter, minor character changes receive some weight, and deletions/additions receive no weight. The result is a percentage of similarity. The suggestion is then derived from the query and its similarities with words in the document collection.

## Synonyms

Synonyms are obtained from `Synonymous.json`, stored in the `DicctionarySyn` folder. Since JSON is a widely used format for data exchange, APIs have been developed for various programming languages to parse, generate, transform, and process JSON data. Using the `System.Text.Json.JsonSerializer` class, we transform the JSON file into a `Dictionary<string, List<string>>`, where each word is associated with its synonyms.
In the `WordInformation` class, the `AddSynonymous` method constructs `QuerySyn` with at most two synonyms for each word in the user's search query, ensuring that these synonyms exist in the document collection.

## Snippet

In the `WordInformation` class, you'll find the functions `snippetForASpecificDocument` and `snippet`:

```cs
public class WordInformation
{
    ...

    public static string snippetForASpecificDocument(string[] files, string[] queryWords, Dictionary<string, (string[] t1, List<int[]> t2)> dictionary, string pathToDocument, Symbol symbol)
    {
        // Implementation details...
    }

    public static string[] snippet(string[] query, Dictionary<string, (string[] t1, List<int[]> t2)> dictionary, string[] files, Symbol symbol)
    {
        // Implementation details...
    }
}
```

- **`snippetForASpecificDocument`**:
  - This function extracts a code snippet of an appropriate size for each document.
  - When the query contains only one term, it expands around that term to find a suitable segment based on its position in the text and returns it.
  - Otherwise, it identifies the most relevant word from the query and expands around it.

