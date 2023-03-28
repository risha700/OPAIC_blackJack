using System.Globalization;

namespace BlackJack;

public class Card 
{

    public Colors Colors;
    public Weight Weight;
    public bool visible = false;

    public  Card(){
        BuildCard();
    }   
    public string[] BuildCard(){
        if (!visible){
            return new string[]
                {
                    $"┌───────┐",
                    $"│░░░░░░░│",
                    $"│░░░░░░░│",
                    $"│░░░░░░░│",
                    $"│░░░░░░░│",
                    $"│░░░░░░░│",
                    $"└───────┘",
                };
            }

        // char color = Colors.ToString()[0];
        char colorShape = (char) Colors;
        string value = Weight switch
                    {
                        Weight.Ace   =>  "A",
                        Weight.Ten   => "10",
                        Weight.Jack  =>  "J",
                        Weight.Queen =>  "Q",
                        Weight.King  =>  "K",
                        _ => ((int)Weight).ToString(CultureInfo.InvariantCulture), // print it as is in english
                    };
        string card = $"{value}{colorShape}";
        string a = card.Length < 3 ? $"{card} " : card;
        string b = card.Length < 3 ? $" {card}" : card;

        return new[]
            {
                $"┌───────┐",
                $"│{a}    │",
                $"│       │",
                $"│       │",
                $"│       │",
                $"│    {b}│",
                $"└───────┘",
            };
    }
}