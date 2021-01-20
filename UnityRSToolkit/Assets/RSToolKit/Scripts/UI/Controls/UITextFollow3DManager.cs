using RSToolkit.Controls;
using RSToolkit.Space3D.Cameras;
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
        bool KeepPrefabTransformValues = true;

        [SerializeField]
        private J[] _predefinedTargets;

        public Vector3 OffsetPosition = Vector3.zero;

        public void AddTarget(J target)
        {
            if(SpawnedGameObjects.Any(sgo => sgo.Target == target))
            {
                return;
            }

            T tf3d = SpawnedGameObjects.FirstOrDefault(sgo => sgo.Target == null);
            if(tf3d == null)
            {
                tf3d = SpawnAndGetGameObject(!KeepPrefabTransformValues);
                tf3d.OffsetPosition = OffsetPosition;
                var cfb = tf3d.GetComponent<CameraFacingBillboard>();
                if(cfb != null)
                {
                    cfb.OffsetRotation = GameObjectToSpawn.transform.localRotation.eulerAngles;
                }
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
