

namespace BlackJack;
public class Hand {
    public  List<Card> cards = new();
    public static int strenght;

    public override string ToString()
    {

        return "Cards in hand"+cards.Count().ToString();
    }

    public int TranslateCard(Card card){
            int value = card.Weight switch
            {
                Weight.Ace   =>  1,
                Weight.Jack  =>  10,
                Weight.Queen =>  10,
                Weight.King  =>  10,
                _ => (int)card.Weight,
            };
            return (int) value;
    }
    public  int GetHandStrength(){
        int total_pts=0;
        var sortedCard = cards.OrderByDescending(x => (int) (x.Weight)).ToList(); // Ace first
        sortedCard.ForEach((card)=>{
            total_pts += TranslateCard(card);
            if(card.Weight.Equals(Weight.Ace)){
                int tmp = 10+total_pts;
                if (tmp <= 21){
                    total_pts+=10;
                }
            }
        });
        return total_pts;
    }

}