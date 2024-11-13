namespace Hangman
{
    public class Methods
    {
        public static string[] ReadWordsFromFile()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName;

            const string WordsFileName = "words.txt";

            string path = $@"{projectDirectory}\{WordsFileName}";
            string[] words = File.ReadAllLines(path);

            return words;
        }
    }
}
