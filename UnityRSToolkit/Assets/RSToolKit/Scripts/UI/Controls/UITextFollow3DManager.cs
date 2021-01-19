using RSToolkit.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace RSToolkit.UI.Controls
{
    public class UITextFollow3DManager<T,J> : Spawner<T>
        where T : UITextFollow3D<J>
        where J : MonoBehaviour
    {

        [SerializeField]
        private J[] _predefinedTargets;

        public void AddTarget(J target)
        {
            if(SpawnedGameObjects.Any(sgo => sgo.Target == target))
            {
                return;
            }

            T tf3d = SpawnedGameObjects.FirstOrDefault(sgo => sgo.Target == null);
            if(tf3d == null)
            {
                tf3d = SpawnAndGetGameObject();
            }
            tf3d.Target = target;
            LogInDebugMode($"Added Target {target.name}");
        }

        protected override void Init()
        {
            base.Init();
            for(int i = 0; i < _predefinedTargets.Length; i++)
            {
                AddTarget(_predefinedTargets[i]);
            }
        }

    }
}
