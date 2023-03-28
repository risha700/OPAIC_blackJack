/* 
    Author: Ahmed A. <ahbox@outlook.com>
    Date: March 13, 2023
*/

// using System;
// using System.IO;
// using System.Linq;
// using System.Collections;
// using System.Drawing;
// using System.Diagnostics;
// using System.Threading;


// public delegate void Notify();  // delegate

namespace BlackJack;

   
class Play{
    public static void AttachToGameOver(object sender, EventArgs e)
    {
        Console.WriteLine("Process Completed!");
    }
    static void Main(string[] args)
    {   
        
        Console.Clear();
        Game game = new();
        // game.GameOver += AttachToGameOver;

        do{
            switch (Game.state)
            {
                case State.Intro:
                    Player player = Game.PreparePlayer();
                    Table table = new Table(Game.activePlayers);
                    Game.RenderGameInfo();
                    Game.state = State.PlaceBet;
                    break;
                case State.PlaceBet:
                    // render menu for betting
                    player = Game.activePlayers[^1];
                    if (player.hands[Game.activeHand]?.cards.Count() is 0){
                        Game.RenderGameInfo();
                        player.GetBet(); // force bet
                        Game.DealRound(); // should be called once per round
                    }
                    Game.RenderGameInfo();
                    Game.state = State.ChooseMove;
                    break; 

                case State.ChooseMove:
                    Game.DisplayPlayerMenu();
                    break;
                
                case State.ConfirmBust:    
                    Game.PlayAgain();
                    break;

                case State.ConfirmLoss:
                    Game.ConfirmMessage("You Lost!");
                    Game.PlayAgain();
                    break;
                case State.ConfirmDraw:
                    Game.ConfirmMessage("Draw!");
                    Game.PlayAgain();
                    break;
                case State.ConfirmWin or State.ConfirmDealtBlackjack:
                // case State.ConfirmWin:
                    Game.ConfirmMessage("You Won!");
                    Game.PlayAgain();
                    break;    
                case State.Lobby:
                    Console.Clear();
                    Game.ProcessScores();
                    game = new();
                    break;
                default:
                    break;
            }


        }while(Game.state is not State.OutOfMoney);
        // Console.Clear();
        Utils.Write($"\n Game Over {Utils.Capitalize(Game.activePlayers[^1].name)}!, you are out of money :(", x_axis:(Game.width/3) + 24, y_axis:(Game.height/2), color: ConsoleColor.DarkYellow);
        Game.ProcessScores();
        Console.CursorVisible = true; // reset
            

    }
}


