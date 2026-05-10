using System.Collections.Generic;

public enum CustomerReactionType
{
    Bad,
    Normal,
    Perfect
}

public class OrderEvaluation
{
    public CustomerReactionType reactionType;

    public int basePrice;
    public int addonsPrice;
    public int penalties;
    public int finalMoney;

    public bool wrongDrinkType;
    public bool wrongPour;
    public bool wrongBeans;
    public int wrongAddonsCount;

    public List<string> mistakes = new List<string>();
}