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
}