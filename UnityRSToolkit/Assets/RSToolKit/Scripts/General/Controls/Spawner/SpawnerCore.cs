namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;
    using UnityEngine.Events;
    using System.Collections.ObjectModel;
    using RSToolkit.Collections;

    public abstract class SpawnerCore<T> : RSMonoBehaviour where T : MonoBehaviour
    {
        public int SpawnLimit = -1;
        // public bool isParent = false;
        public Transform SpawnParent = null;
        private List<T> _spawnedGameObjects = new List<T>();
        private SizedStack<T> _pooledGameObjects;
        public int PoolSize = -1;
        public bool CollectChildrenAlreadyInScene = true;

        public ReadOnlyCollection<T> SpawnedGameObjects
        {
            get
            {
                return _spawnedGameObjects.AsReadOnly();
            }
        }

        public class SpawnerEvent : UnityEvent<T> { }

        public SpawnerEvent OnSpawnEvent = new SpawnerEvent();

        public void DestroyLastSpawnedGameObject(float? time = null)
        {
            if (SpawnedGameObjects.Count > 0)
            {
                var spawnedGameObject = SpawnedGameObjects[SpawnedGameObjects.Count - 1];
                DestroySpawnedGameObject(spawnedGameObject, time);
            }
        }

        void NotDelayedDestroySpawnedGameObject(T spawnedGameObject){
            if (PoolSize > 0 && !_pooledGameObjects.IsFull())
            {
                spawnedGameObject.gameObject.SetActive(false);
                _pooledGameObjects.Push(spawnedGameObject);
            }
            else
            {
                Destroy(spawnedGameObject.gameObject);
            }
        }

        IEnumerator DelayedDestroySpawnedGameObject(T spawnedGameObject, float time){
            yield return new WaitForSeconds(time);
            NotDelayedDestroySpawnedGameObject(spawnedGameObject);
        }

        public void DestroySpawnedGameObject(T spawnedGameObject, float? time = null)
        {
            if (spawnedGameObject != null && SpawnedGameObjects.Contains(spawnedGameObject)){
                if(time != null)
                {
                    StartCoroutine(DelayedDestroySpawnedGameObject(spawnedGameObject, time.Value));
                }
                else
                {
                    NotDelayedDestroySpawnedGameObject(spawnedGameObject);
                }
                _spawnedGameObjects.Remove(spawnedGameObject);
            }
        }

        IEnumerator DelayedDestroyAllSpawns(float time){
            yield return new WaitForSeconds(time);
            DestroyAllSpawns();
        }

        public void DestroyAllSpawns(float time){
            StartCoroutine(DelayedDestroyAllSpawns(time));
        }
        public void DestroyAllSpawns(){
            if (SpawnedGameObjects.Count > 0){
                DestroySpawnedGameObject(SpawnedGameObjects[SpawnedGameObjects.Count -1]);
                DestroyAllSpawns();
            }
        }

        public T SpawnAndGetGameObject(T gameObjectToSpawn, bool useSpawnerTransformValues = true)
        {
            if (SpawnLimit > 0 && SpawnedGameObjects.Count >= SpawnLimit){
                return null;
            }

            T spawnedGameObject;
            if(PoolSize > 0 && _pooledGameObjects.Any()){
               spawnedGameObject = _pooledGameObjects.Pop(); 
               spawnedGameObject.gameObject.SetActive(true);
            }else{
                spawnedGameObject = Instantiate(gameObjectToSpawn);
            }

            ValidateSpawnParent();
            spawnedGameObject.transform.SetParent(SpawnParent);

            if (useSpawnerTransformValues)
            {
                gameObject.transform.CopyValuesTo(spawnedGameObject.transform, false);
            }
            else
            {
                gameObjectToSpawn.transform.CopyValuesTo(spawnedGameObject.transform, false);
                // spawnedGameObject.transform.ResetScaleAndRotation();
                spawnedGameObject.transform.localPosition = Vector3.zero; //transform.localPosition;
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
        public override bool Init(bool force = false)
        {
            if(base.Init(force)){
                ValidateSpawnParent();
                CollectChildren();
                if(PoolSize > 0){
                    _pooledGameObjects = new SizedStack<T>(PoolSize);
                }
                return true;
            }

            return false;
        }
        #endregion RSMonoBehaviour Functions

    }
}