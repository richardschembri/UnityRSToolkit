using System.Collections;
using System.Collections.Generic;

namespace RSToolkit.Rythm
{
    public class RythmFactory<T> : where T : struct, IConvertible, IComparable
    {
        public RythmManager ManagerComponent{get; private set;}

        public T[][] Prompts {get; private set;}
        public Dictionary<T, string> PromptMaps{get; private set;}

        public RythmFactory(RythmManager manager, Dictionary<T, string> PromptMaps, T[][] prompts){
            ManagerComponent = manager;
            Prompts = prompts;
            var promptTypes = Enum.GetValues(typeof(T));
            if (values.Length < 1) { throw new ArgumentException("Enum provided to Initialize must have at least 1 visible definition"); }



            
        }

    }
}
