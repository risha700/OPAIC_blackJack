/* 
    Author: Ahmed A. <ahbox@outlook.com>
    Date: March 13, 2023
*/

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Threading;


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
                    Game.MainMenu();
                    Player player = Game.PreparePlayer();
                    Table table = new Table(Game.activePlayers);
                    Game.RenderGameInfo();
                    Game.state = State.PlaceBet;
                    break;
                case State.PlaceBet:
                    // render menu for betting
                    player = Game.activePlayers[^1];
                    if (player.hands[Game.activeHand]?.cards.Count() is 0){
                        player.GetBet(); // force bet
                        Game.DealRound(); // should be called once per round
                    }
                    Game.state = State.ChooseMove;
                    Game.RenderGameInfo();
                    break; 

                case State.ChooseMove:
                    Game.DisplayPlayerMenu();
                    break;
                
                case State.ConfirmBust:    
                    Game.ConfirmMessage("Busted", ConsoleColor.Red);
                    Game.PlayAgain();
                    break;

                case State.ConfirmLoss:
                    Game.ConfirmMessage("You Lost!", ConsoleColor.DarkRed);
                    Game.PlayAgain();
                    break;
                case State.ConfirmDraw:
                    Game.ConfirmMessage("Draw!");
                    Game.PlayAgain();
                    break;
                case State.ConfirmWin:
                // case State.ConfirmWin:
                    Game.ConfirmMessage("You Won!", ConsoleColor.Green);
                    Game.PlayAgain();
                    break;   
               case State.ConfirmDealtBlackjack:
                // case State.ConfirmWin:
                    Game.ConfirmMessage("Black Jack!", ConsoleColor.Magenta);
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
        Console.Clear();
        Utils.Render($"Game Over {Utils.Capitalize(Game.activePlayers[^1].name)}! {Game.exitMessage} :(\n",
        x_axis:(Game.width/3)+10, y_axis:3, bgColor: ConsoleColor.DarkBlue, renderSpace:true);
        Game.ProcessScores();
        Console.CursorVisible = true; // reset
            

    }
}


