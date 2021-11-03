﻿namespace RSToolkit.Game.TurnBased
{
    public interface ICurrentPlayer<TPlayer>
    {
        /// <summary>
        /// The player whose turn it is in the current gamestate.
        /// </summary>
        TPlayer CurrentPlayer { get; }
    }
}
