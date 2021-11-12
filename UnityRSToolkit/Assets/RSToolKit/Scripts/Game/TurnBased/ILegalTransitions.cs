using System.Collections.Generic;

namespace RSToolkit.Game.TurnBased
{
    public interface ILegalTransitions<TTransition>
    {
        List<TTransition> GetLegalTransitions();
    }
}
