// using BlackJack;
namespace BlackJack.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Game game = new();
        Game.Setup();
    }

    [Test]
    public void TestPlayerWin()
    {


        // Assert.Pass();
        var player = new Player("rs");
        Game.activePlayers.Add(player);

        // Table table = new Table(Game.activePlayers);
        Card JackCard = new(){ Colors= Colors.Diamonds, Weight= Weight.Jack };
        Card AceCard = new(){ Colors= Colors.Diamonds, Weight= Weight.Ace };

        player.hands[0].cards.Add(AceCard);
        player.hands[0].cards.Add(JackCard);

        StringAssert.AreEqualIgnoringCase(player.name, "rs");
        Assert.That(500, Is.EqualTo(player.balance));
        Assert.That(0, Is.EqualTo(player.bet));
        Assert.That(21, Is.EqualTo(player.hands[0].GetHandStrength()));
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
    public void TestPlayerDoubleDown()
    {
    }

    [Test]
    public void TestPlayerSplit()
    {
    }
}