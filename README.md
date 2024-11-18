# Hangman
Hangman C# Project

# Task of the Game:
- the goal is to guess the missing word.
- you will have underscores where the missing letters should be placed.
- if the letter that you have chosen is in the answer then all the underscores where the letter should go will be filled out.
- every time you choose a wrong letter you lose a try and the hangman starts building up.
- you should solve the word before the hangman dies.
- if you enter a number or an input with more than 1 characters the game will tell you that you should type only one letter.
- if you type the same letter twice or more the game will tell you that you have already used the word and try to type another word without removing a life.
- there is a guess streak which starts showing up when you guess 2 times in a row and adds up with each right guess. It resets when your guess is wrong.
- there is a random gift which uppon acception can give you 1 extra try or reveal a letter. You can also decline the gift if you don't want it.

# Structure
- .vs
- Hangman
    - bin
    - obj
    - Animations.cs
    - Hangman.cs
    - Hangman.csproj
    - words.txt
- Hangman.sln

# Branches
- main: where the game is placed.
- dev-branch: a version of the game with dev mode (cheats) for testing.