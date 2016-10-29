using System;
using System.Collections;
using System.Collections.Generic;

namespace MyBot
{
    public class Game
    {
        private readonly Random random;
        private int line;
        private int column;
        private int myAliveCells;
        private int enemyAliveCells;
        private int enemyLine;
        private int enemyColumn;
        private delegate string OperationDelegate();
        private readonly Dictionary<string, OperationDelegate> methods;
        
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
        private const string UserWonAnswer = "You won, congratulations.";
        private const string UserHitAnswer = "You hitted.";
        private const string UserMissAnswer = "You missed.";
        private const string UserTurnAnswer = "Now it\'s your turn.";
        private const string UserLoseAnswer = "Sorry, but you losed.";
        private const string MyHitAnswer = "Line {0}, colomn {1}.";

        private int Index => line * 10 + column;
        private int EnemyIndex => enemyLine * 10 + enemyColumn;

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
            StartNewGame();
        }

        public string Play(string method, string x = "", string y = "")
        {
            if (!methods.ContainsKey(method))
                return BadRequestAnswer;
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
            enemyAliveCells = 20;
            myAliveCells = 20;
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
            enemyAliveCells--;
            if (enemyAliveCells != 0) return MakeGuess();
            StartNewGame();
            return UserLoseAnswer;
        }

        private string PositiveAnswer()
        {
            enemyField[EnemyIndex] = '1';
            enemyAliveCells--;
            if (enemyAliveCells != 0) return MakeGuess();
            StartNewGame();
            return UserLoseAnswer;
        }

        private string NegativeAnswer()
        {
            return UserTurnAnswer;
        }

        private string Hit()
        {
            if (myField[Index] == '1')
            {
                myField[Index] = '0';
                myAliveCells--;
                if (myAliveCells == 0)
                {
                    StartNewGame();
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
            return MakeGuess();
        }

        private string MakeGuess()
        {
            do
            {
                var index = random.Next(100);
                enemyColumn = index%10;
                enemyLine = index/10;
            } while (enemyField[EnemyIndex] != '0');
            return string.Format(MyHitAnswer, enemyLine, (char) (enemyColumn + 65));
        }

        private bool GetLineFromRequest(string s)
        {
            var index = int.Parse(s);
            if (index > 0 && index < 10)
            {
                line = index;
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
            if (index > 0 && index < 10)
            {
                column = index;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}