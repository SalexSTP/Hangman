using Hangman;

string[] words = ReadWordsFromFile();
Console.CursorVisible = false;
const char Underscore = '_';
const int MaxAllowedIncorrectCharacters = 6;

while (true)
{
    string word = GetRandomWord(words);
    string wordToGuess = new(Underscore, word.Length);

    int incorrectGuessCount = 0;
    List<char> playerUsedLetters = new List<char>();

    DrawCurrentGameState(false, incorrectGuessCount, wordToGuess, playerUsedLetters);
    PlayGame(word, wordToGuess, incorrectGuessCount, playerUsedLetters);

    Console.Write("If you want to play again, press [Enter]. Else, type 'quit': ");
    string playerInput = Console.ReadLine();

    if (playerInput == "quit")
    {
        Console.Clear();
        Console.WriteLine("Thank you for playing! Hangman was closed.");
        break;
    }

    Console.Clear();
}




static string[] ReadWordsFromFile()
{
    string currentDirectory = Directory.GetCurrentDirectory();
    string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName;

    const string WordsFileName = "words.txt";

    string path = $@"{projectDirectory}\{WordsFileName}";
    string[] words = File.ReadAllLines(path);

    return words;
}

void DrawCurrentGameState(bool inputIsValid, int incorrectGuess, string guessedWord, List<char> playerUsedLetters)
{
    Console.Clear();
    Console.WriteLine(Animations.wrongGuessesFrames[incorrectGuess]);
    Console.WriteLine($"Guess: {guessedWord}");
    Console.WriteLine($"You have to guess {guessedWord.Length} symbols.");
    Console.WriteLine($"The following letters are used: {String.Join(", ", playerUsedLetters)}");

    if (inputIsValid)
    {
        Console.WriteLine("You should type only a single character!");
    }

    Console.WriteLine("Your symbol: ");
}

void PlayGame(string word, string wordToGuess, int incorrectGuessCount, List<char> playerUsedLetters)
{
    while (true)
    {
        string playerInput = Console.ReadLine().ToLower();

        if (playerInput.Length != 1)
        {
            DrawCurrentGameState(true, incorrectGuessCount, wordToGuess, playerUsedLetters);
            continue;
        }

        char playerLetter = char.Parse(playerInput);
        playerUsedLetters.Add(playerLetter);

        bool playerLetterIsContained = CheckIfSymbolIsContained(word, playerLetter);

        if (playerLetterIsContained)
        {
            wordToGuess = AddLetterToGuessWord(word, playerLetter, wordToGuess);
        }
        else
        {
            incorrectGuessCount++;
        }

        DrawCurrentGameState(false, incorrectGuessCount, wordToGuess, playerUsedLetters);

        bool playerWins = CheckIfPlayerWins(wordToGuess);

        if (playerWins)
        {
            Console.Clear();
            Console.WriteLine(Animations.win);
            Console.WriteLine($"The word you guessed is [{word}].");

            break;
        }

        bool playerLoses = CheckIfPlayerLoses(incorrectGuessCount);

        if (playerLoses)
        {
            Console.SetCursorPosition(0, 0);
            DrawDeathAnimation(Animations.deathAnimationFrames);

            Console.Clear();
            Console.WriteLine(Animations.loss);
            Console.WriteLine($"The exact word is [{word}].");

            break;
        }
    }
}

string GetRandomWord(string[] words)
{
    Random random = new Random();
    string word = words[random.Next(words.Length)];
    return word.ToLower();
}

bool CheckIfSymbolIsContained(string word, char playerLetter)
{
    if (!word.Contains(playerLetter))
    {
        return false;
    }

    return true;
}

string AddLetterToGuessWord(string word, char playerLetter, string wordToGuess)
{
    char[] wordToGuessCharArray = wordToGuess.ToCharArray();

    for (int i = 0; i < wordToGuess.Length; i++)
    {
        if (playerLetter == word[i])
        {
            wordToGuessCharArray[i] = playerLetter;
        }
    }

    return new string(wordToGuessCharArray);
}

bool CheckIfPlayerWins(string wordToGuess)
{
    if (wordToGuess.Contains(Underscore))
    {
        return false;
    }
    return true;    
}

bool CheckIfPlayerLoses(int incorrectGuessCount)
{
    if (incorrectGuessCount == MaxAllowedIncorrectCharacters)
    {
        return true;
    }
    return false;
}

void DrawDeathAnimation(string[] deathAnimation)
{
    for (int i = 0; i < deathAnimation.Length; i++)
    {
        Console.WriteLine(deathAnimation[i]);
        Thread.Sleep(150);
        Console.SetCursorPosition(0, 0);
    }
}