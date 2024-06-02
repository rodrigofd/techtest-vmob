
/// <summary>
/// Represents a matched word that can be formed by concatenating two words.
/// </summary>
public class MatchedWord
{
  // the word that can be formed by concatenating the first and second words
  public required string Word { get; set; }
  // the first word that is concatenated
  public required string First { get; set; }
  // the second word that is concatenated
  public required string Second { get; set; }
}

public class Program
{
  public const string WORDLIST_FILE_PATH = "docs/wordlist.txt";

  static void Main()
  {
    // read the file and store the words in a list
    var wordList = File.ReadAllLines(WORDLIST_FILE_PATH).ToList();

    // find the matching words according to the rules
    var matchedWords = WordMatcher.FindMatchingWords(wordList);

    // print the matching words
    foreach (var matchedWord in matchedWords)
    {
      Console.WriteLine($"{matchedWord.First} + {matchedWord.Second} => {matchedWord.Word}");
    }
  }
}

public class WordMatcher
{
  public static List<MatchedWord> FindMatchingWords(List<string> wordList)
  {
    List<MatchedWord> matchedWords = new();

    // iterate through the word
    foreach (var word in wordList)
    {
      // skip the word if it is not of length 6
      if (word.Length != 6) continue;

      // check if word can be formed by concatenating two words from the list
      // by breaking the word into two parts in all possible ways
      for (int indexInWord = 1; indexInWord < word.Length; indexInWord++)
      {
        var first = word.Substring(0, indexInWord);
        var second = word.Substring(indexInWord);
        if (wordList.Contains(first) && wordList.Contains(second))
        {
          // add the matched word to the list to be returned
          matchedWords.Add(new MatchedWord
          {
            Word = word,
            First = first,
            Second = second
          });
          break;
        }
      }
    }

    return matchedWords;
  }
}