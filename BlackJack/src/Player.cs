using System.Diagnostics;


namespace BlackJack;

public class Player
{
public string name;
public Stopwatch timer;
public double balance; // starting bonus
static Random random = new();

public List<Hand> hands = new();
// public List<Card> cards = new();

public double bet;



// constructor
public Player(string playerName, double initial_bal=500.00){
    name =  String.IsNullOrEmpty(playerName.Trim().ToString().ToLower()) ?  $"player-{random.Next(1,1000)}": playerName;
    timer = new();
    balance = (double)initial_bal;
    hands.Clear();
    hands.Add(new Hand(){ cards = new (){} }); 
    bet = 0.00;
    // ranking
}
public override string ToString()
{
    return $"Player {name} has {balance} balance";
}

public int GetBet(){
        
        Utils.Render($"", x_axis:(Table.tableWidth)*55/100, y_axis:(Table.tableHeight)*85/100, renderSpace:true);                        
        int get_bet = Utils.Input($"Your bet: ", required:true);
        int safeBet = Math.Min(get_bet, (int) balance); // silent all in
        balance -= safeBet;
        bet += safeBet;
        return safeBet;

    }



}
