using System.Diagnostics;


namespace BlackJack;

public class Game
{
    public static int height = Console.WindowHeight;
    public static int width = Console.WindowWidth;
    public static List<(string playerName,  double balance)> scores = new();
    public static TimeSpan playerTimeOutSpan = TimeSpan.FromMilliseconds(15000); // 15 second autoplay timeout
    public static List<Player> activePlayers = new();
    public static Card JackCard = new(){ Colors= Colors.Diamonds, Weight= Weight.Jack };
    public static Stopwatch playerRoundStopWatch = new();
    public static Random random = new();
    public static List<Card> deck = new();
    public static Player? dealer;
    public static State state;
    public static int activeHand = 0; // defaults first
    // public static int playerXPosition = (width/3)+(width/8);
    public static int playerXPosition = (Table.tableWidth);
    public static int playerYPosition = (Table.tableHeight)*95/100;
    public static string AboutBlackJack = "\n Blackjack is a casino banking game. \nIt is the most widely played casino \nbanking game in the world. It uses \ndecks of 52 cards and descends from \na global family of casino banking \ngames known as Twenty-One. \nThis family of card games also \nincludes the European games Vingt-et-Un and \nPontoon, and the Russian game Ochko. \n";
    public static string BLACKJACK_BANNER = """
            _______  ___      _______  _______  ___   _   
            |  _    ||   |    |   _   ||       ||   | | | 
            | |_|   ||   |    |  |_|  ||       ||   |_| | 
            |       ||   |    |       ||       ||      _| 
            |  _   | |   |___ |       ||      _||     |_  
            | |_|   ||       ||   _   ||     |_ |    _  |
            |_______||_______||__| |__||_______||___| |_|
            """;
    public Game(){
        // contructor
        activePlayers.Clear();
        state = State.Intro;
        Setup();
        IntroAnimation();
        activeHand = 0;
        // assign dealer
        dealer = GetOrCreatePlayer("dealer", 999999);
        activePlayers.Add(dealer); // add it or not??
    }
    // events
    public event EventHandler? GameOver;
    
    public void setGameOver(){
        OnGameOver(EventArgs.Empty);
    }

    protected virtual void OnGameOver(EventArgs e){
            GameOver?.Invoke(this, e);
        }

    public static bool onConsoleResize = (height != Console.WindowHeight || width != Console.WindowWidth);
    public static void LoadingIndicator(int rounds=5){
        int x = width/3;
        int y = height/3;
        List<ConsoleColor> colors = new(){ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Green};
        for(int i=0; i<rounds;i++){
            Utils.Render(Utils.Stringify(JackCard.BuildCard()), textColor: colors[i], x_axis: x+24, y_axis:y+1, renderSpace:true);
            Thread.Sleep(10);
            Utils.Render(Utils.Stringify(JackCard.BuildCard()), x_axis: x+24, y_axis:y+1, erase:true);
        }
        
    }
    public static void IntroAnimation(){
        int x = width/3;
        int y = height/3;

        JackCard.visible = false;
        LoadingIndicator();
        Utils.Render(BLACKJACK_BANNER, x_axis: x, y_axis:y/4,textColor: ConsoleColor.Cyan, renderSpace:false);
        JackCard.visible = true;
        
        string[] array = JackCard.BuildCard().Where((e, i) => (i != 4)).ToArray(); // adjust height
        Utils.Render(Utils.Stringify(array), textColor: ConsoleColor.Black, bgColor:ConsoleColor.White, x_axis: x+47, y_axis:y/3, renderSpace:true);
    
    }
    public static void Setup(bool read_scores=true){
        deck = new();
        foreach (Colors color in Enum.GetValues<Colors>())
        {
            foreach (Weight value in Enum.GetValues<Weight>())
            {
                deck.Add(new()
                {
                    Colors = color,
                    Weight = value,
                    visible = true,
                });
            }
        }
        if(read_scores)
        ProcessScores(read:true); // read it
        

    }
    
    public static void ProcessScores(string fileName="scores.txt", bool read=false) {
        string dir = System.IO.Directory.GetCurrentDirectory(); 
        string file_path = Path.Join(dir, fileName);
        if (read){

            if(!File.Exists(file_path)){
                var f = File.Create(file_path);
                f.Close();
                }

            string[] content = File.ReadAllLines(file_path);
            for(int i=0; i<content.Length;i++){
                string[] temp = content[i].Split(" ");
                if (temp[0]!= "dealer")
                scores.Add((playerName: temp[0].Trim(), balance: Convert.ToDouble(temp[1])));
            }
            scores = scores.DistinctBy((s)=>s.playerName).ToList();
            scores.Sort((a, b) => b.balance.CompareTo(a.balance));
            
        }
        else
        {
            var player =  Game.activePlayers[^1];            
            _sanitize_scores(player);
            int player_idx = scores.FindIndex((p)=>p.playerName.ToLower().Equals(player.name.ToLower()));
            if(player_idx > -1){ // update latest info
                scores.RemoveAt(player_idx);
                scores.Add((playerName: player.name.ToLower().Trim(), balance: player.balance));
            }
        // filter unique and get old balance
            scores = scores.DistinctBy((s)=>s.playerName).ToList();
            using StreamWriter sw = new(file_path);
            scores.ForEach(async (score)=>{
                if(!score.playerName.ToLower().Equals("dealer") && !String.IsNullOrEmpty(score.playerName))
                    await sw.WriteLineAsync($"{score.playerName.ToLower().Replace(" ", String.Empty).Trim()} {score.balance}"); 
            });
            sw.Close(); //save
        }


    }
    public static Player GetOrCreatePlayer(string pName, double startingBalance=500.00){
        scores.Add((playerName: pName.ToLower(), balance: startingBalance));
        // filter unique and get old balance
        scores = scores.DistinctBy((s)=>s.playerName).ToList();
        scores.Sort((a, b) => b.balance.CompareTo(a.balance));
        (string playerName, double balance) p = scores.Find(p=>p.playerName == pName);
        Player player = new Player(p.playerName, p.balance);
        _sanitize_scores(player);
        return player;
    }

    private static void _sanitize_scores(Player player){
        int idx = scores.FindIndex((p)=>p.balance.Equals(player.name));
        if(idx>=0){scores.RemoveAt(idx);}
        scores.Add((playerName: player.name.ToLower(), balance: player.balance));
        scores = scores.DistinctBy((s)=>s.playerName).ToList();
        scores.Sort((a, b) => b.balance.CompareTo(a.balance));
    }
    public static void ShowAchievers(){
        // Utils.Render($"Best Records \n\n", x_axis: (width/3)+(width/10), y_axis:(height/3), 
        Utils.Render($"Best Records \n\n", x_axis: 0, y_axis:0, renderSpace:true, textColor:ConsoleColor.Blue, bgColor:ConsoleColor.DarkYellow);
        Utils.Render($"Player \t Score \n");
        int safeRange = scores.Count() >=5 ? 5 : scores.Count() ;
        scores.GetRange(0, safeRange).ForEach((score)=>{
            if(!score.playerName.ToLower().Equals("dealer")){
                if (scores.First().Equals(score))
                    Utils.Render($"{scores.IndexOf(score)+1} {Utils.Capitalize(score.playerName).PadRight(10)} {score.balance} \n", bgColor:ConsoleColor.Green);
                else
                    Utils.Render($"{scores.IndexOf(score)+1} {Utils.Capitalize(score.playerName).PadRight(10)} {score.balance} \n");
            }
        });
    }

    public static void Shuffle(List<Card> cards)
    {

        for (int i = 0; i < cards.Count; i++)
        {
            int swap = random.Next(cards.Count);
            (cards[i], cards[swap]) = (cards[swap], cards[i]);
        }
    }
    
    public static void PayOut(Player player, double rate=2){
        if(!Table.round_paid){
            player.balance += (double) (player.bet)*rate;
            Table.round_paid = true;
        }
        // render paid out
        RenderGameInfo();
    }

    // deal first round
    public static void DealRound(){
        Setup(); // reset deck
        Shuffle(deck);
        int index = 0;
        activePlayers.ForEach((player)=>{
            player.hands.Clear();// reset hand to move out incase of split
            player.hands?.Add(new Hand(){
                cards = new (){deck[index], deck[index+1]}
            }); 
            index+=2;
        });
        deck.RemoveRange(0,4);
        // should render
        RenderDealerCards(); // flipped
        RenderPlayerCards(activePlayers[^1]);
        RenderGameInfo();
    }

    public static void DoDoubleDown (Player player){
        DealCard(player);
        player.balance -= player.bet;
        player.bet += player.bet;
        Utils.Render($"Your bet: {player.bet}  ", x_axis:(Table.tableWidth)*55/100, y_axis:(Table.tableHeight)*85/100, renderSpace:true); 
        RenderGameInfo();
        if(player.hands[activeHand]?.GetHandStrength() > 21) 
        {
            ConfirmBusted();
        } else {
            // if has split hand then go next
            TurnToDealer();
        }
    }

    public static bool CanDoubleDown(){
        // either after split

        // or after first deal
        Player player = activePlayers[^1];
        bool first_round = (player.hands.Count() == 1 && player.hands[0].cards.Count()==2);
        bool got_enough = player.balance >= player.bet;
        return first_round && got_enough;
    }
    public static void DrawMenu(bool erase=false){
        Player player = activePlayers[^1];
        int xPt = (Table.tableWidth)*90/100;
        int yPt = (Table.tableHeight)*65/100;
        Utils.DrawRect(8, 4, (xPt+1, yPt));
        Utils.DrawRect(8, 4, (xPt+15, yPt));
        Utils.Render("1. Stay\n", x_axis:xPt+2, y_axis:yPt+1);
        Utils.Render("2. Hit", x_axis:xPt+16, y_axis:yPt+1);
        // double down
        if(CanDoubleDown()){
            Utils.DrawRect(13, 4, (xPt+30, yPt));
            Utils.Render("3. Double Down", x_axis:xPt+31, y_axis:yPt+1);
        }
        if(CanSplit()){
            Utils.DrawRect(8, 4, (xPt+30, yPt));
            Utils.Render("4. Split", x_axis:xPt+43, y_axis:yPt+1);
        }
        if (erase){
            for (int i = 0; i <= 3; i++)
            {
                Utils.Render("                                                              ", x_axis:xPt, y_axis:yPt+i, erase:true);
            }
        }
        
    }

    public static void ConfirmBusted(){
        Player player = activePlayers[^1];
        Game.state = State.ConfirmBust;
        player.hands[0] = new();
    }
    public static void ConfirmMessage(string message, ConsoleColor level=ConsoleColor.Yellow){            
        DrawMenu(erase:true);
        Utils.Render($"{message.ToString()}", x_axis: playerXPosition, y_axis:playerYPosition+2, renderSpace:true, bgColor:level);
    }
    public static void TurnToDealer(){
        // revert turn to dealer
        DrawMenu(erase:true);
        Player dealer = activePlayers[0];
        RenderDealerCards(true);
        // int? hand_strenght = dealer.hands[activeHand]?.GetHandStrength();
        RenderGameInfo(renderDealer:true);
        // start robot logic
        DealerBrain();
        CheckWinner();
    }
    public static void DisplayPlayerMenu(){
        DrawMenu();
        var s = Enum.GetNames(typeof(State));
        var predicate = s.Where((v)=> (v == "Lobby" || v == "PlaceBet"|| v == "ChooseMove"));

        while (predicate.Contains(Game.state.ToString()))
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1 or ConsoleKey.NumPad1: // stay
                    TurnToDealer();
                    
                    break;
                case ConsoleKey.D2 or ConsoleKey.NumPad2: // hit
                    Player player = activePlayers[^1];
                    if (player.hands[activeHand]?.GetHandStrength() <= 21){
                        Game.DealCard(player);
                        if(player.hands[activeHand]?.GetHandStrength() > 21)
                            ConfirmBusted();
                    }
                    break;
                case ConsoleKey.D3 or ConsoleKey.NumPad3 : // double down double the bet for one card only
                    player = activePlayers[^1];
                    if(CanDoubleDown())
                        DoDoubleDown(player);
                    break;
                case ConsoleKey.D4 or ConsoleKey.NumPad4: // split when cards are the same and pay the same bet
                    if(CanSplit()){
                        //DoSplit();
                    }
                    break;
                default:
                    // CheckWinner();
                    break;
            }
            DrawMenu(erase:true);
            if(Game.state == State.ChooseMove)
            DrawMenu();
            // CheckWinner();
        }      
        

        // TODO:
        // doubledown
        // split
    }

    public static void DealCard(Player player, int hand=0, bool forDealer=false){
        // TODO: handle split
        Card card = deck[0];
        // player.cards.Add(card);
        card.visible = true;
        player.hands[hand].cards.Add(card);
        deck.RemoveAt(0);
        if(forDealer == true){
            RenderDealerCards(visible:true);   
        }else{
            RenderPlayerCards(player);
        }
        // forDealer?RenderDealerCards(visible:true):RenderPlayerCards(player);
       
        RenderGameInfo(renderDealer:forDealer);

    }
    
    public static void RenderGameInfo(bool erase = false, bool renderDealer=false){
        int xPt = (Table.tableWidth)*55/100;
        int yPt = (Table.tableHeight);
        string statement = "Hand Score";

            activePlayers.ForEach((player)=>{
            if (player.name.ToLower().Equals("dealer")&& renderDealer){
                yPt = (Table.tableHeight)*30/100;
                Utils.Render($"Dealer {statement} {player.hands[Game.activeHand]?.GetHandStrength().ToString()} ", x_axis: xPt, y_axis:yPt+1, renderSpace:true);   
            }else if(!player.name.ToLower().Equals("dealer")&&!renderDealer){
                yPt = (Table.tableHeight)*85/100;
                Utils.Render($"Balance {player.balance.ToString("c0").Replace(',', '.')}",  x_axis: xPt, y_axis:yPt+2, renderSpace:true, bgColor:ConsoleColor.Green);
                if( Game.state is State.ChooseMove ||Game.state is State.ConfirmWin){
                    Utils.Render($"{statement} {player.hands[Game.activeHand]?.GetHandStrength().ToString()} ", x_axis: xPt, y_axis:yPt+4, renderSpace:true); 
                }
                
            }

        });
        
    }
    public static bool CanSplit(){
        Player player = activePlayers[^1];
        return (bool)(player.hands[activeHand].cards.Count()==2&&
            player.hands[activeHand].cards[0].Weight == player.hands[activeHand].cards[1].Weight);
    }
    public static bool QualifesForBlackJack(){
        Player player = activePlayers[^1];
        return (bool)(player.hands[activeHand].cards.Count()==2 && player.hands[activeHand].GetHandStrength()==21);
    }
    public static void CheckWinner(){
        
        int dealerScore=0;
        int playerScore=0;
        Player player = activePlayers[^1];
        Player dealer = activePlayers[0];

        activePlayers.ForEach((player)=>{
            player.hands.ForEach((hand)=>{
                if(player.name.Equals("dealer")){
                    dealerScore = (int) hand.GetHandStrength();
                }else{
                    playerScore = (int) hand.GetHandStrength();
                }
                
            });
        });
        
        bool LessOrEqual21 = (dealerScore <= 21 && playerScore<=21) ;// && (dealerScore>17|| (dealerScore<17&&dealerScore>=playerScore)));
      
                
        if(dealerScore>21){
             if(QualifesForBlackJack()){
                    Game.state = State.ConfirmDealtBlackjack; // 21
                    Game.PayOut(player, 3);
             }else{
                    Game.state = State.ConfirmWin; // dealer bust
                    Game.PayOut(player);
             }
        }
        if(LessOrEqual21){

                if(dealerScore == playerScore){
                    Game.state = State.ConfirmDraw;
                    // return money
                    Game.PayOut(player, rate:1.00);

                }

                else if(dealerScore>playerScore){ // dealer higher
                    Game.state = State.ConfirmLoss;
                }else if(playerScore>dealerScore){
                    if(QualifesForBlackJack()){
                        Game.state = State.ConfirmDealtBlackjack; // 21
                        Game.PayOut(player, rate:3);
                        return;
                    }
                    Game.state = State.ConfirmWin; // won
                    Game.PayOut(player, rate:2);
                    // double the bet
                }
                
            }         


    }
    public static void DealerBrain(){
        // TODO:recurse on split 
        // get player's the closest hand to win
        

        while(activePlayers[0].hands[0].GetHandStrength()<21 && 
        activePlayers[0].hands[0].GetHandStrength() < activePlayers[^1].hands[0].GetHandStrength()){
            DealCard(activePlayers[0], forDealer:true);
            RenderGameInfo(renderDealer:true);
            CheckWinner();
            Thread.Sleep(100); // just suspense

        }
        RenderGameInfo(renderDealer:true); // force refresh
    
        
    }
    public static void GameOverHandler(object sender, EventArgs e){
        Console.WriteLine("Game Over");
    }

    public static Player PreparePlayer(){
        Utils.Render($"\n    Join the game   \n\n Enter Player Name  ", x_axis:(width/3)+(width/10), y_axis:(Game.height/3)+(height/10), bgColor:ConsoleColor.DarkBlue, renderSpace:true);
        
        // Utils.Write("Enter Player Name:");
        string playerName = Utils.Input(" ", required:false, stringType:true);
        Player player = GetOrCreatePlayer(playerName.ToLower().Trim());
        activePlayers.Add(player);
        return player;
    }
    public static void RenderPlayerCards(Player player){            
        for (int h=0; h<player.hands.Count();h++){
            for (int i=0; i<player.hands[h].cards.Count();i++)
                Utils.Render($"{Utils.Stringify(player.hands[h].cards[i].BuildCard())}", x_axis: playerXPosition+(i*3)+h, y_axis:playerYPosition,renderSpace:true);
            if(Game.state.Equals(State.ConfirmSplit))
                playerXPosition+=(h+1)*h;
        }

    }
    public static void RenderDealerCards(bool visible= false){
        bool shouldFlip = visible;
        Player player = activePlayers[0];
        int dealerXPosition = (Table.tableWidth);
        int dealerYPosition = (Table.tableHeight)*23/100;
        for (int h=0; h<player.hands.Count();h++){
            for (int i=0; i<player.hands[h].cards.Count();i++){
                if(i==0 && !visible ) {
                    shouldFlip = true;
                }else if(i!=0 && !visible) {
                    shouldFlip = false;
                }
                player.hands[h].cards[i].visible = shouldFlip;
                Utils.Render($"{Utils.Stringify(player.hands[h].cards[i].BuildCard())}", x_axis: dealerXPosition+(i*3)+h, y_axis:dealerYPosition,renderSpace:true);
                if(Game.state.Equals(State.ConfirmSplit))
                    playerXPosition+=(h+1)*h;
            }

        }

    }
    public static void MainMenu(){
        int xPt = (Table.tableWidth)*55/100;
        int yPt = (Table.tableHeight)*99/100;
        Utils.DrawRect(20, 4, (xPt, yPt));
        Utils.Render("I. How to play\n", x_axis:xPt+2, y_axis:yPt+1);
        Utils.DrawRect(20, 4, (xPt+30, yPt));
        Utils.Render("L. Show achievers\n", x_axis:xPt+32, y_axis:yPt+1);
        Utils.DrawRect(20, 4, (xPt+60, yPt));
        Utils.Render("P. Play Now\n", x_axis:xPt+62, y_axis:yPt+1);

        switch(Console.ReadKey(true).Key){
            
            case ConsoleKey.B: // back
                clear(0, 0, (width/3)-2, height);
                Game.state = State.Intro;
                MainMenu();
                break;
            case ConsoleKey.L: // show achievers
                ShowAchievers();
                Utils.Write("press B: to go back to lobby", x_axis:0, y_axis:width/2);
                MainMenu();
                break;
            case ConsoleKey.I: // how to play
                // show info inside switch
                clear();
                Console.SetCursorPosition(0,0);
                Console.WriteLine(Game.AboutBlackJack);
                Utils.Write("press B: to go back to lobby", x_axis:0, y_axis:width/2);
                MainMenu();
                break;
            default: // play now
                clear();
                break; // continue

        }
    
    // clear screen at x,y
     void clear(int x=-1, int y=-1, dynamic? areax=null, dynamic? areay=null){
         (int h, int v) area = (h:100, v:20);
         
        if (areax is not null && areay is not null){
            area = (h: areax, v: areay);
        }
       
        if(x == -1){x=xPt;}
        if(y == -1){y=yPt;}
        for (int i = 0; i <= area.h; i++)
        {
            for(int w=0; w<=area.v;w++)
            Utils.Render(" ", x_axis:x+i, y_axis:y+w, erase:true);
        }
    }
            

    }
    public static void PlayAgain(){
        Game.ProcessScores(); // save 
        if(Game.activePlayers[^1].balance <=0){
            Game.state = State.OutOfMoney;
            return;
        }
        // reset hands and cards

        activePlayers.ForEach((player)=>{
            player.bet = 0.00;
            player.hands.ForEach((h)=>h.cards.Clear());
            player.hands.Clear();

            player.hands.Add(new Hand(){ cards = new (){} }); 
        });
        

        Utils.Render($"Play again? y|yes n|no  ", x_axis:(Table.tableWidth)*90/100, y_axis:(Table.tableHeight)*70/100,
                    textColor:ConsoleColor.DarkBlue, bgColor:ConsoleColor.Cyan , renderSpace:true);
        switch(Console.ReadKey(true).Key){
            case (ConsoleKey.Y):
                Game.Setup(read_scores:true);
                new Table(Game.activePlayers);
                Game.RenderGameInfo();
                Game.state = State.PlaceBet;
                break;
            default:
                Game.state = State.Lobby;
                break;

        }
    
        
    }



} 