using System.Collections.Generic;

namespace RSToolkit.Game.TurnBased
{
    public interface ILegalMoves<TMove>
    {
        /// <summary>
        /// Return a list of legal moves for the current gamestate.
        /// </summary>
        List<TMove> GetLegalMoves();
    }
}
