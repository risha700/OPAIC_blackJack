// using BlackJack;
namespace BlackJack.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Game game = new();
        Game.Setup();
        Table table = new Table(Game.activePlayers);
        
    }

    [Test]
    public void TestPlayerWin()
    {


        // Assert.Pass();
        var player = new Player("rs");
        Game.activePlayers.Add(player);

        // Table table = new Table(Game.activePlayers);
        Card JackCard = new(){ Colors= Colors.Diamonds, Weight= Weight.Jack };
        Card AceCard = new(){ Colors= Colors.Diamonds, Weight= Weight.Queen };

        player.hands[0].cards.Add(AceCard);
        player.hands[0].cards.Add(JackCard);

        StringAssert.AreEqualIgnoringCase(player.name, "rs");
        Assert.That(500, Is.EqualTo(player.balance));
        Assert.That(0, Is.EqualTo(player.bet));
        Assert.That(20, Is.EqualTo(player.hands[0].GetHandStrength()));
        Assert.That(State.Intro, Is.EqualTo(Game.state));
        Game.CheckWinner();
        Assert.That(State.ConfirmWin, Is.EqualTo(Game.state));
        

    }

    [Test]
    public void TestPlayerBust()
    {


        var player = new Player("rs");
        Game.activePlayers.Add(player);
        // Table table = new Table(Game.activePlayers);
        Card JackCard = new(){ Colors= Colors.Diamonds, Weight= Weight.Jack };
        Card AceCard = new(){ Colors= Colors.Diamonds, Weight= Weight.Ace };
        
  

        player.hands[0].cards.Add(new(){ Colors= Colors.Clubs, Weight= Weight.Two });
        player.hands[0].cards.Add(new(){ Colors= Colors.Spades, Weight= Weight.Three });
        
        var dealer = Game.activePlayers[0];
        dealer.hands[0].cards.Add(AceCard);
        dealer.hands[0].cards.Add(JackCard);


        StringAssert.AreEqualIgnoringCase(player.name, "rs");
        Assert.That(500, Is.EqualTo(player.balance));
        Assert.That(0, Is.EqualTo(player.bet));
        Assert.That(5, Is.EqualTo(player.hands[0].GetHandStrength()));
        Assert.That(21, Is.EqualTo(dealer.hands[0].GetHandStrength()));
        Assert.That(State.Intro, Is.EqualTo(Game.state));
        Game.CheckWinner();
        Assert.That(State.ConfirmLoss, Is.EqualTo(Game.state));
    }

    
    [Test]
    public void TestPlayerDraw()
    {
        var player = new Player("rs");
        Game.activePlayers.Add(player);
        var dealer = Game.activePlayers[0];
        // Table table = new Table(Game.activePlayers);
        player.hands[0].cards.Add(new(){ Colors= Colors.Clubs, Weight= Weight.Ten });
        player.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Three });
        player.hands[0].cards.Add(new(){ Colors= Colors.Hearts, Weight= Weight.Five });
        // 2aces and 6
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Six });
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ace });
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Spades, Weight= Weight.Ace });
        Assert.That(1, Is.EqualTo(dealer.hands.Count()));
        
        Assert.That(2, Is.EqualTo(Game.activePlayers.Count()));
        // Game.activePlayers.ForEach(p=>Console.WriteLine(p));
        Assert.That(3, Is.EqualTo(dealer.hands[0].cards.Count()));

        // dealer.hands.ForEach((hand)=>Console.WriteLine(hand.TranslateCard(hand.cards[dealer.hands.IndexOf(hand)])));        
        Assert.That(18, Is.EqualTo(player.hands[0].GetHandStrength()));
        Assert.That(18, Is.EqualTo(dealer.hands[0].GetHandStrength()));
        Game.CheckWinner();
        Assert.That(State.ConfirmDraw, Is.EqualTo(Game.state));


    }

    [Test]
    public void TestBlackJack3to1()
    {
        var player = new Player("rs");
        Game.activePlayers.Add(player);
        var dealer = Game.activePlayers[0];
        player.bet = 100;
        player.hands[0].bet = 100;
        player.balance -=100; // was 500
        Assert.That(400, Is.EqualTo(player.balance));
        // // Table table = new Table(Game.activePlayers);
        player.hands[0].cards.Add(new(){ Colors= Colors.Clubs, Weight= Weight.Jack });
        player.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ace });
        
        // // 2aces and 6
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ten });
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ten });
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Spades, Weight= Weight.Three });
        Game.CheckWinner();
        Assert.That(State.ConfirmDealtBlackjack, Is.EqualTo(Game.state));
        Assert.That(700, Is.EqualTo(player.balance));

    }

    [Test]
    public void TestPlayerSplit()
    {
        

        // Console.WriteLine($"Is paid Out???? {Table.round_paid}");
        var player = new Player("rs");
        Game.activePlayers.Add(player);
        var dealer = Game.activePlayers[0];
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Nine });
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ten });
        // Game.activePlayers.ForEach(p=>Console.Write(p));

        player.bet = 100;
        player.hands[0].bet = 100;
        player.balance -=100; // was 500
        Assert.That(400, Is.EqualTo(player.balance));
        // Table table = new Table(Game.activePlayers);
        player.hands[0].cards.Add(new(){ Colors= Colors.Clubs, Weight= Weight.Jack });
        player.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Jack });
        
        Game.deck[0] = new(){ Colors= Colors.Diamonds, Weight= Weight.Ten };
        Game.deck[1] = new(){ Colors= Colors.Diamonds, Weight= Weight.Five };
        
        // can split ??
        Assert.True(Game.CanSplit());
        Game.DoSplit(true);        
        
        // Assert.That(200, Is.EqualTo(player.bet));
        Assert.That(100, Is.EqualTo(player.hands[0].bet));
        Assert.That(100, Is.EqualTo(player.hands[1].bet));
        // Assert.That(200, Is.EqualTo(player.bet));
        Assert.That(2, Is.EqualTo(player.hands.Count()));
        Assert.That(2, Is.EqualTo(player.hands[0].cards.Count()));

        Assert.That(20, Is.EqualTo(player.hands[0].GetHandStrength()));
        Assert.That(15, Is.EqualTo(player.hands[1].GetHandStrength()));
        Assert.That(19, Is.EqualTo(dealer.hands[0].GetHandStrength()));

        Game.CheckWinner();
        // need to out the original bet only for one hand
        Assert.That(500, Is.EqualTo(player.balance));

        // busted
        // dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ten });
        // dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ten });
        // dealer.hands[0].cards.Add(new(){ Colors= Colors.Spades, Weight= Weight.Three });
        // Game.CheckWinner();
        // Assert.That(State.ConfirmDealtBlackjack, Is.EqualTo(Game.state));
        // Assert.That(700, Is.EqualTo(player.balance));
    }
    
    [Test]
    public void TestPlayerSplitComplex()
    {
        

        // Console.WriteLine($"Is paid Out???? {Table.round_paid}");
        var player = new Player("rs");
        Game.activePlayers.Add(player);
        var dealer = Game.activePlayers[0];
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Nine });
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ten });
        // Game.activePlayers.ForEach(p=>Console.Write(p));

        player.bet = 100;
        player.hands[0].bet = 100;
        player.balance -=100; // was 500
        Assert.That(400, Is.EqualTo(player.balance));
        // Table table = new Table(Game.activePlayers);
        player.hands[0].cards.Add(new(){ Colors= Colors.Clubs, Weight= Weight.Jack });
        player.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Jack });
        
        Game.deck[0] = new(){ Colors= Colors.Diamonds, Weight= Weight.Ten };
        Game.deck[1] = new(){ Colors= Colors.Diamonds, Weight= Weight.Nine };
        // can split ??
        Assert.True(Game.CanSplit());
        Game.DoSplit(true);        
        player.hands[1].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Two });
        
        Assert.That(100, Is.EqualTo(player.bet));
        Assert.That(100, Is.EqualTo(player.hands[0].bet));
        Assert.That(100, Is.EqualTo(player.hands[1].bet));
        // Assert.That(200, Is.EqualTo(player.bet));
        Assert.That(2, Is.EqualTo(player.hands.Count()));
        Assert.That(2, Is.EqualTo(player.hands[0].cards.Count()));
        Assert.That(3, Is.EqualTo(player.hands[1].cards.Count()));

        Assert.That(20, Is.EqualTo(player.hands[0].GetHandStrength()));
        Assert.That(21, Is.EqualTo(player.hands[1].GetHandStrength()));
        Assert.That(19, Is.EqualTo(dealer.hands[0].GetHandStrength()));

        Game.CheckWinner();
        // two hands won handbet * 2 total 400 back + 300 balance
        Assert.That(700, Is.EqualTo(player.balance));

    
    }
    [Test]
    public void DealerBust()
    {
        var player = new Player("rs");
        Game.activePlayers.Add(player);
        var dealer = Game.activePlayers[0];
        player.bet = 100;
        player.hands[0].bet = 100;
        player.balance -=100; // was 500
        Assert.That(400, Is.EqualTo(player.balance));
        // // Table table = new Table(Game.activePlayers);
        player.hands[0].cards.Add(new(){ Colors= Colors.Clubs, Weight= Weight.Jack });
        player.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Jack });
        
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ten });
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Diamonds, Weight= Weight.Ten });
        dealer.hands[0].cards.Add(new(){ Colors= Colors.Spades, Weight= Weight.Three });
        Assert.That(20, Is.EqualTo(player.hands[0].GetHandStrength()));
        Assert.That(23, Is.EqualTo(dealer.hands[0].GetHandStrength()));

        Game.CheckWinner();
        Assert.That(State.ConfirmWin, Is.EqualTo(Game.state));
        Assert.That(600, Is.EqualTo(player.balance));

    }

}