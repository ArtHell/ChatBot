using System;
using System.Collections;
using System.Collections.Generic;
using MyBot.Models;
using MyBot.Repositories;

namespace MyBot
{
    public class Game
    {
        private readonly GameInfoRepository gameInfoRepository = new GameInfoRepository();
        private readonly Random random;

        private delegate string OperationDelegate();
        private readonly Dictionary<string, OperationDelegate> methods;

        private GameInfo gameInfo;
        private char[] myField;
        private char[] enemyField;

        private const string RandomField = "1010000000" +
                                           "1010010000" +
                                           "1010000000" +
                                           "1000100000" +
                                           "0010000000" +
                                           "1010000000" +
                                           "1010000100" +
                                           "0000000000" +
                                           "1010010000" +
                                           "1010000000";

        private const string BadRequestAnswer = "Can you repeat, please?";
        private const string SayStartAnswer = "You sould start game before.";
        private const string UserWonAnswer = "You won, congratulations.";
        private const string UserHitAnswer = "You hitted.";
        private const string UserDeadHitAnswer = "You killed my ship.";
        private const string UserMissAnswer = "You missed.";
        private const string UserTurnAnswer = "Now it\'s your turn.";
        private const string UserLoseAnswer = "Sorry, but you losed.";
        private const string MyHitAnswer = "Line {0}, colomn {1}.";

        public Game()
        {
            random = new Random(DateTime.Now.Millisecond);

            methods =
                new Dictionary<string, OperationDelegate>
                {
                    { "Start", Start },
                    { "Hit", Hit },
                    { "Negative answer", NegativeAnswer },
                    { "Positive answer", PositiveAnswer },
                    { "Dead answer", DeadAnswer }
                };


        }

        public string Play(string recipientId, string method, string x = "", string y = "")
        {
            if (!methods.ContainsKey(method))
            {
                return BadRequestAnswer;
            }

            var info = gameInfoRepository.GetGameInfo(recipientId);
            if (info != null)
            {
                gameInfo = info;
                if (gameInfo.MyField != null && gameInfo.EnemyField != null)
                {
                    myField = gameInfo.MyField.ToCharArray();
                    enemyField = gameInfo.EnemyField.ToCharArray();
                }
            }
            else
            {
                gameInfo = new GameInfo() { GameStarted = false, RecipientId = recipientId };
                gameInfoRepository.AddGameInfo(gameInfo);
            }

            if (!gameInfo.GameStarted && method != "Start")
            {
                return SayStartAnswer;
            }

            if (method == "Hit")
            {
                if (!GetLineFromRequest(x) || !GetColumnFromRequest(y))
                {
                    return BadRequestAnswer;
                }
                SaveGameInfo();
            }
            return methods[method]();
        }

        private void StartNewGame()
        {
            myField = GenerateShips();
            enemyField = EmptyField();
            gameInfo.EnemyAliveCells = 20;
            gameInfo.MyAliveCells = 20;
            gameInfo.GameStarted = true;
        }

        private char[] GenerateShips()
        {
            var field = RandomField.ToCharArray();

            return field;
        }

        private char[] EmptyField()
        {
            var field = new string('0', 100).ToCharArray();
            return field;
        }

        private string DeadAnswer()
        {
            gameInfo.EnemyAliveCells--;
            if (gameInfo.EnemyAliveCells != 0) return MakeGuess();
            gameInfo.GameStarted = false;
            SaveGameInfo();
            return UserLoseAnswer;
        }

        private void SaveGameInfo()
        {
            gameInfo.MyField = new string(myField);
            gameInfo.EnemyField = new string(enemyField);
            gameInfoRepository.SaveGameInfo(gameInfo);
        }

        private string PositiveAnswer()
        {
            gameInfo.EnemyAliveCells--;
            if (gameInfo.EnemyAliveCells != 0) return MakeGuess();
            gameInfo.GameStarted = false;
            SaveGameInfo();
            return UserLoseAnswer;
        }

        private string NegativeAnswer()
        {
            return UserTurnAnswer;
        }

        private string Hit()
        {
            var index = gameInfo.Line*10 + gameInfo.Column;
            if (myField[index] == '1')
            {
                myField[index] = '0';
                gameInfo.MyAliveCells--;
                SaveGameInfo();
                if (gameInfo.MyAliveCells == 0)
                {
                    gameInfo.GameStarted = false;
                    SaveGameInfo();
                    return UserWonAnswer;
                }
                else if(IsEmptyCellsAround(index))
                {
                    return UserDeadHitAnswer;
                }
                else
                {
                    return UserHitAnswer;
                }
            }
            else
            {
                return UserMissAnswer + " " + MakeGuess();
            }
        }

        private bool IsEmptyCellsAround(int index)
        {
            if (index - 1 >= 0 && myField[index - 1] == '1')
            {
                return false;
            }

            if (index + 1 < 100 && myField[index + 1] == '1')
            {
                return false;
            }

            if (index - 10 >= 0 && myField[index - 10] == '1')
            {
                return false;
            }

            if (index + 10 >= 0 && myField[index + 10] == '1')
            {
                return false;
            }

            return true;
        }

        private string Start()
        {
            StartNewGame();
            return MakeGuess();
        }

        private string MakeGuess()
        {
            int index;
            do
            {
                index = random.Next(100);
            } while (enemyField[index] != '0');
            gameInfo.EnemyColumn = index % 10;
            gameInfo.EnemyLine = index / 10;
            enemyField[index] = '1';
            SaveGameInfo();
            return string.Format(MyHitAnswer, gameInfo.EnemyLine + 1, (char)(gameInfo.EnemyColumn + 65));
        }

        private bool GetLineFromRequest(string s)
        {
            var index = int.Parse(s) - 1;
            if (index >= 0 && index < 10)
            {
                gameInfo.Line = index;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool GetColumnFromRequest(string s)
        {
            var index = char.ToUpper(s[0]) - 65;
            if (index >= 0 && index < 10)
            {
                gameInfo.Column = index;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}