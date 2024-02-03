using System;

public class Player : IComparable
{
    public string nick;
    public int score;

    public Player(string nick, int score)
    {
        this.nick = nick;
        this.score = score;
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        if (obj is Player player)
            return this.score.CompareTo(player.score);
        else
            throw new ArgumentException("Object is not a Player");
    }
     
    public override string ToString()
    {
        return $"{this.nick}, {this.score}";
    }
}