namespace BlackJack;

public class Table 
{
    public static bool round_paid;
    List<Player> players = new();
    public static int tableWidth = Console.WindowWidth/2;
    public static int tableHeight = (int) Math.Min(((Console.WindowHeight/2) * 2), 20);
    // int padding = 
    public Table(List<Player> PlayerList){
        players = PlayerList;
        round_paid = false;
        Render();
        SetPlayers();

    }

    public void Render(){
        Console.Clear();
        int point = tableWidth;
        Utils.DrawRect(tableWidth, tableHeight+4, (x: tableWidth/2, y: 3));
    }
    public void SetPlayers()
    {
        int startPt = tableWidth-(tableWidth*15/100);
        int y = (tableHeight/4)- (tableHeight)*10/100; ;
        
        // dealer
        Utils.DrawRect(tableWidth/3, (tableHeight/2), (x: startPt, y: y));
        Utils.Render($"Dealer", x_axis:(tableWidth-(tableWidth*33/100))+(tableWidth/y), y_axis:y++);
        // player
        Utils.DrawRect(tableWidth/3, (tableHeight/2), (x: startPt, y: tableHeight-tableHeight*15/100));
        var player = players[^1];
        Utils.Render($"{Utils.Capitalize(player.name)}", x_axis:tableWidth, y_axis:tableHeight*85/100);
        // Utils.Render($"{Utils.Capitalize(player.name)}", x_axis:(tableWidth+tableWidth/3)+(tableWidth/y), y_axis:tableHeight+1);
        // Utils.Render($" Your Credit {player.balance:C}", x_axis:tableWidth, y_axis:0, bgColor:ConsoleColor.Green, renderSpace:true);
    }

}