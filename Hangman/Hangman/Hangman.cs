using Hangman;
using System.Text;

string[] words = ReadWordsFromFile();
Console.CursorVisible = false;
Console.OutputEncoding = Encoding.UTF8;
const char Underscore = '_';
int maxAllowedIncorrectCharacters = 6; // Set this as a variable
int sumOfCorrectInARow = 0;
int consecutiveIncorrectGuesses = 0;
string fire = "🔥";

while (true)
{
string word = GetRandomWord(words);
string wordToGuess = new(Underscore, word.Length);

int incorrectGuessCount = 0;
List<char> playerUsedLetters = new List<char>();

DrawCurrentGameState(false, false, incorrectGuessCount, wordToGuess, playerUsedLetters, maxAllowedIncorrectCharacters, clearScreen: true);
PlayGame(word, wordToGuess, incorrectGuessCount, playerUsedLetters, ref maxAllowedIncorrectCharacters);

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

void DrawCurrentGameState(bool inputIsValid, bool inputIsDifferent, int incorrectGuess, string guessedWord, List<char> playerUsedLetters, int maxAllowedIncorrectCharacters, bool clearScreen = true)
{
if (clearScreen) Console.Clear();

int animationFrameIndex = Math.Min(incorrectGuess, Animations.wrongGuessesFrames.Length - 1);
Console.WriteLine(Animations.wrongGuessesFrames[animationFrameIndex]);

if (sumOfCorrectInARow > 1)
{
Console.WriteLine($"You are on a {sumOfCorrectInARow} {fire} streak");
}

int unguessedLetters = guessedWord.Count(c => c == Underscore);

Console.WriteLine($"Guess: {guessedWord}");
Console.WriteLine($"You have to guess {unguessedLetters} symbols.");
Console.WriteLine($"The following letters are used: {String.Join(", ", playerUsedLetters)}");
Console.WriteLine($"Tries left: {maxAllowedIncorrectCharacters - incorrectGuess}");
Console.Write("Your symbol: ");

if (inputIsValid)
{
Console.WriteLine("You should only type a single letter that is not a number or a symbol!");
}

if (inputIsDifferent)
{
Console.WriteLine("You can't use the same letter again!");
}
}

void PlayGame(string word, string wordToGuess, int incorrectGuessCount, List<char> playerUsedLetters, ref int maxAllowedIncorrectCharacters)
{
while (true)
{
string playerInput = Console.ReadLine().ToLower();

if (playerInput == "dev")
{
EnterDevMode(ref word, ref wordToGuess, ref incorrectGuessCount, ref maxAllowedIncorrectCharacters);
DrawCurrentGameState(false, false, incorrectGuessCount, wordToGuess, playerUsedLetters, maxAllowedIncorrectCharacters, clearScreen: false);
continue;
}

if (playerInput.Length != 1 || playerInput.All(char.IsDigit) || playerInput.All(char.IsPunctuation))
{
DrawCurrentGameState(true, false, incorrectGuessCount, wordToGuess, playerUsedLetters, maxAllowedIncorrectCharacters, clearScreen: false);
continue;
}

char playerLetter = char.Parse(playerInput);

if (playerUsedLetters.Contains(playerLetter))
{
DrawCurrentGameState(false, true, incorrectGuessCount, wordToGuess, playerUsedLetters, maxAllowedIncorrectCharacters, clearScreen: false);
continue;
}

bool playerLetterIsContained = CheckIfSymbolIsContained(word, playerLetter);

if (playerLetterIsContained)
{
sumOfCorrectInARow++;
consecutiveIncorrectGuesses = 0;
wordToGuess = AddLetterToGuessWord(word, playerLetter, wordToGuess);
}
else
{
sumOfCorrectInARow = 0;
incorrectGuessCount++;
consecutiveIncorrectGuesses++;
}

playerUsedLetters.Add(playerLetter);

if (consecutiveIncorrectGuesses >= 3)
{
OfferGift(ref incorrectGuessCount, ref wordToGuess, word);
consecutiveIncorrectGuesses = 0;
}

DrawCurrentGameState(false, false, incorrectGuessCount, wordToGuess, playerUsedLetters, maxAllowedIncorrectCharacters, clearScreen: true);

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

void EnterDevMode(ref string word, ref string wordToGuess, ref int incorrectGuessCount, ref int maxAllowedIncorrectCharacters)
{
Console.WriteLine("Dev mode activated. Type 'exit' to leave dev mode.");
while (true)
{
Console.Write("Command> ");
string devInput = Console.ReadLine();

if (devInput == "exit") break;

if (devInput.StartsWith("Word |"))
{
word = devInput.Split('|')[1].Trim().ToLower();
wordToGuess = new string(Underscore, word.Length);
Console.WriteLine($"Word set to: {word}");
}
else if (devInput.StartsWith("Tries |"))
{
if (int.TryParse(devInput.Split('|')[1].Trim(), out int newTries) && newTries >= 0)
{
maxAllowedIncorrectCharacters = newTries;
incorrectGuessCount = 0;
Console.WriteLine($"Tries left set to: {newTries}");
}
else
{
Console.WriteLine("Invalid number of tries. Please enter a positive integer.");
}
}
else if (devInput.StartsWith("Streak |"))
{
if (int.TryParse(devInput.Split('|')[1].Trim(), out int newStreak))
{
sumOfCorrectInARow = newStreak;
Console.WriteLine($"Streak set to: {newStreak}");
}
else
{
Console.WriteLine("Invalid streak number.");
}
}
else if (devInput.StartsWith("Receive | Gift"))
{           
OfferGift(ref incorrectGuessCount, ref wordToGuess, word);
}
else
{
Console.WriteLine("Unknown command. Available commands:");
Console.WriteLine("  Word | (New Word)");
Console.WriteLine("  Tries | (New Amount of Tries)");
Console.WriteLine("  Streak | (New Streak Number)");
Console.WriteLine("  Receive | Gift");
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
Console.WriteLine("You chose to continue guessing!");
}

Thread.Sleep(2000);
Console.Clear();
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
    return incorrectGuessCount >= maxAllowedIncorrectCharacters;
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
