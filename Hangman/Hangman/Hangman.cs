using Hangman;
using System.Text;

string[] words = ReadWordsFromFile();
Console.CursorVisible = false;
Console.OutputEncoding = Encoding.UTF8;
const char Underscore = '_';
const int MaxAllowedIncorrectCharacters = 6;
int sumOfCorrectInARow = 0;
int consecutiveIncorrectGuesses = 0;  
string fire = "🔥";


while (true)
{
    string word = GetRandomWord(words);
    string wordToGuess = new(Underscore, word.Length);

    int incorrectGuessCount = 0;
    List<char> playerUsedLetters = new List<char>();

    DrawCurrentGameState(false, false, incorrectGuessCount, wordToGuess, playerUsedLetters);
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

void DrawCurrentGameState(bool inputIsValid, bool inputIsDifferent, int incorrectGuess, string guessedWord, List<char> playerUsedLetters)
{
    Console.Clear();
    Console.WriteLine(Animations.wrongGuessesFrames[incorrectGuess]);
    if (inputIsValid) Console.WriteLine("You should type only a single letter!");
    if (inputIsDifferent) Console.WriteLine("You can't use the same letter again!");

    int unguessedLetters = guessedWord.Count(c => c == Underscore);
    Console.WriteLine($"Guess: {guessedWord}");
    Console.WriteLine($"You have to guess {unguessedLetters} symbols.");
    Console.WriteLine($"The following letters are used: {String.Join(", ", playerUsedLetters)}");
    Console.WriteLine($"Tries: {MaxAllowedIncorrectCharacters - incorrectGuess}");
    Console.Write("Your symbol: ");
    if (sumOfCorrectInARow > 1)
    {
        Console.WriteLine($"You are on a {sumOfCorrectInARow} {fire} streak");
    }
}

void PlayGame(string word, string wordToGuess, int incorrectGuessCount, List<char> playerUsedLetters)
{
    while (true)
    {
        string playerInput = Console.ReadLine().ToLower();
        if (playerInput.Length != 1 || playerInput.All(char.IsDigit))
        {
            DrawCurrentGameState(true, false, incorrectGuessCount, wordToGuess, playerUsedLetters);
            continue;
        }

        char playerLetter = char.Parse(playerInput);
        if (playerUsedLetters.Contains(playerLetter))
        {
            DrawCurrentGameState(false, true, incorrectGuessCount, wordToGuess, playerUsedLetters);
            continue;
        }
        if (!word.Contains(playerLetter))
        {
            sumOfCorrectInARow++;
            playerUsedLetters.Add(playerLetter);
        }

        bool playerLetterIsContained = CheckIfSymbolIsContained(word, playerLetter);

        if (playerLetterIsContained)
        {
            wordToGuess = AddLetterToGuessWord(word, playerLetter, wordToGuess);
            consecutiveIncorrectGuesses = 0;  
        }
        else
        {
            incorrectGuessCount++;
            sumOfCorrectInARow = 0;
            consecutiveIncorrectGuesses++;  
        }

        
        if (consecutiveIncorrectGuesses >= 3)
        {
            OfferGift(ref incorrectGuessCount, ref wordToGuess, word);
            consecutiveIncorrectGuesses = 0;  
        }

        DrawCurrentGameState(false, false, incorrectGuessCount, wordToGuess, playerUsedLetters);
        bool playerWins = CheckIfPlayerWins(wordToGuess);

        if (playerWins)
        {
            Console.Clear();
            Console.WriteLine(Animations.win);
            if (sumOfCorrectInARow > 1)
            {
                Console.WriteLine($"CONGRATULATIONS! YOU WON WITH A {sumOfCorrectInARow} {fire} STREAK!");
            }
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

void OfferGift(ref int incorrectGuessCount, ref string wordToGuess, string word)
{
    Console.WriteLine("A wild gift appears! Would you like to open it? (yes/no)");
    string response = Console.ReadLine().ToLower();

    if (response == "yes")
    {
        Random random = new Random();
        int giftType = random.Next(2);

        if (giftType == 0)
        {
            
            incorrectGuessCount = Math.Max(incorrectGuessCount - 1, 0);
            Console.WriteLine("The gift gives you an extra try!");
        }
        else
        {
            
            char hintLetter = RevealRandomLetter(word, wordToGuess);
            wordToGuess = AddLetterToGuessWord(word, hintLetter, wordToGuess);
            Console.WriteLine($"The gift reveals a letter: {hintLetter}");
        }
    }
    else
    {
        incorrectGuessCount--;
        Console.WriteLine("You chose to continue guessing!");
    }
}

char RevealRandomLetter(string word, string wordToGuess)
{
    List<char> missingLetters = new List<char>();
    for (int i = 0; i < word.Length; i++)
    {
        if (wordToGuess[i] == Underscore)
        {
            missingLetters.Add(word[i]);
        }
    }
    Random random = new Random();
    return missingLetters[random.Next(missingLetters.Count)];
}

string GetRandomWord(string[] words)
{
    Random random = new Random();
    string word = words[random.Next(words.Length)];
    return word.ToLower();
}

bool CheckIfSymbolIsContained(string word, char playerLetter)
{
    return word.Contains(playerLetter);
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
    return !wordToGuess.Contains(Underscore);
}

bool CheckIfPlayerLoses(int incorrectGuessCount)
{
    return incorrectGuessCount == MaxAllowedIncorrectCharacters;
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
