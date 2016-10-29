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
        private const string UserMissAnswer = "You missed.";
        private const string UserTurnAnswer = "Now it\'s your turn.";
        private const string UserLoseAnswer = "Sorry, but you losed.";
        private const string MyHitAnswer = "Line {0}, colomn {1}.";

        private int Index => gameInfo.Line * 10 + gameInfo.Column;
        private int EnemyIndex => gameInfo.EnemyLine * 10 + gameInfo.EnemyColumn;

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

        public string Play(string recipientId,string method, string x = "", string y = "")
        {
            if (!methods.ContainsKey(method))
            {
                return BadRequestAnswer;
            }

            if (gameInfoRepository.GetGameInfo(recipientId) != null)
            {
                gameInfo = gameInfoRepository.GetGameInfo(recipientId);
            }
            else
            {
                gameInfo = new GameInfo() {GameStarted = false, RecipientId = recipientId};
                gameInfoRepository.SaveGameInfo(gameInfo);
            }
            myField = gameInfo.MyField.ToCharArray();
            enemyField = gameInfo.EnemyField.ToCharArray();
                
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
            enemyField[EnemyIndex] = '1';
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
            enemyField[EnemyIndex] = '1';
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
            if (gameInfo.MyField[Index] == '1')
            {
                myField[Index] = '0';
                gameInfo.MyAliveCells--;
                SaveGameInfo();
                if (gameInfo.MyAliveCells == 0)
                {
                    gameInfo.GameStarted = false;
                    SaveGameInfo();
                    return UserWonAnswer;    
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

        private string Start()
        {
            StartNewGame();
            return MakeGuess();
        }

        private string MakeGuess()
        {
            do
            {
                var index = random.Next(100);
                gameInfo.EnemyColumn = index%10;
                gameInfo.EnemyLine = index/10;
            } while (gameInfo.EnemyField[EnemyIndex] != '0');
            return string.Format(MyHitAnswer, gameInfo.EnemyLine, (char) (gameInfo.EnemyColumn + 66));
        }

        private bool GetLineFromRequest(string s)
        {
            var index = int.Parse(s);
            if (index > 0 && index < 10)
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
            var index = char.ToUpper(s[0]) - 66;
            if (index > 0 && index < 10)
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