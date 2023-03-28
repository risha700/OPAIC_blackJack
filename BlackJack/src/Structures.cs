namespace BlackJack;

public enum Colors
{
    Clubs='♣',
    Diamonds='♦',
    Hearts='♥',
    Spades='♠',
}
public enum Weight
{
    Ace   = 01,
    Two   = 02,
    Three = 03,
    Four  = 04,
    Five  = 05,
    Six   = 06,
    Seven = 07,
    Eight = 08,
    Nine  = 09,
    Ten   = 10,
    Jack  = 11,
    Queen = 12,
    King  = 13,
}
public enum State
{
    Intro,
    Lobby,
    PlaceBet,
    ConfirmDealtBlackjack,
    ChooseMove,
    ConfirmBust,
    ConfirmSplit,
    ConfirmLoss,
    ConfirmDraw,
    ConfirmWin,
    Replay,
    OutOfMoney,
}