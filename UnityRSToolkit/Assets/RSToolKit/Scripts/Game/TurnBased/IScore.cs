namespace RSToolkit.Game.TurnBased
{
    public interface IScore<TPlayer>
    {
        int Score(TPlayer player);
    }
}
