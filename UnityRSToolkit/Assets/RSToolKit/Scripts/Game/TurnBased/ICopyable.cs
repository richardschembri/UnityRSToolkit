namespace RSToolkit.Game.TurnBased
{
    public interface ICopyable<T>
    {
        /// <summary>
        /// Return a copy of the object that has no shared mutable state with the original.
        /// </summary>
        T DeepCopy();
    }
}
