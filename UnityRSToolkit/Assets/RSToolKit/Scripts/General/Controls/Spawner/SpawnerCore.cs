namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;
    using UnityEngine.Events;
    using System.Collections.ObjectModel;

    public abstract class SpawnerCore<T> : RSMonoBehaviour where T : MonoBehaviour
    {
        public int SpawnLimit = -1;
        // public bool isParent = false;
        public Transform SpawnParent = null;
        private List<T> _spawnedGameObjects;

        public bool CollectChildrenAlreadyInScene = true;

        public ReadOnlyCollection<T> SpawnedGameObjects
        {
            get
            {
                if(_spawnedGameObjects == null){
                    _spawnedGameObjects = new List<T>();
                }
                return _spawnedGameObjects.AsReadOnly();
            }
        }

        public class SpawnerEvent : UnityEvent<T> { }

        public SpawnerEvent OnSpawnEvent = new SpawnerEvent();

        public void DestroyLastSpawnedGameObject()
        {
            if (SpawnedGameObjects.Count > 0)
            {
                var spawnedGameObject = SpawnedGameObjects[SpawnedGameObjects.Count - 1];
                DestroySpawnedGameObject(spawnedGameObject);
            }
        }

        public void DestroySpawnedGameObject(T spawnedGameObject)
        {
            if (spawnedGameObject != null && SpawnedGameObjects.Contains(spawnedGameObject)){
                DestroyImmediate(spawnedGameObject);
                _spawnedGameObjects.Remove(spawnedGameObject);
            }
        }
        public void DestroyAllSpawns(){
            if (SpawnedGameObjects.Count > 0){
                DestroySpawnedGameObject(SpawnedGameObjects[SpawnedGameObjects.Count -1]);
                DestroyAllSpawns();
            }
        }
        public T SpawnAndGetGameObject(T gameObjectToSpawn ,bool useSpawnerTransformValues = true)
        {
            if (SpawnLimit > 0 && SpawnedGameObjects.Count >= SpawnLimit){
                return null;
            }

            var spawnedGameObject = Instantiate(gameObjectToSpawn);

            ValidateSpawnParent();
            spawnedGameObject.transform.SetParent(SpawnParent);

            if (useSpawnerTransformValues)
            {
                gameObject.transform.CopyValuesTo(spawnedGameObject.transform, false);
            }
            else
            {
                spawnedGameObject.transform.ResetScaleAndRotation();
                spawnedGameObject.transform.localPosition = transform.localPosition;
            }

            _spawnedGameObjects.Add(spawnedGameObject);
            OnSpawnEvent.Invoke(spawnedGameObject);
            return spawnedGameObject;
        }

        private void ValidateSpawnParent()
        {

            if (SpawnParent == null)
            {
                SpawnParent = this.transform;
            } 
        }

        public void CollectChildren()
        {
            var spawnchildren = SpawnParent.GetTopLevelChildren<T>();
            for(int i = 0; i <  spawnchildren.Length; i++)
            {
                if (!_spawnedGameObjects.Contains(spawnchildren[i]))
                {
                    _spawnedGameObjects.Add(spawnchildren[i]);
                }
            }
        }

        #region RSMonoBehaviour Functions
        protected override void Init()
        {
            ValidateSpawnParent();
            CollectChildren();
        }
        #endregion RSMonoBehaviour Functions

    }
}