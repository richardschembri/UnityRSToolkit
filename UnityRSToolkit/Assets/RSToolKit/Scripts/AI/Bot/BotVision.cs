using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using RSToolkit.Helpers;
using System;
using UnityEngine.Events;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(Bot))]
    public class BotVision : MonoBehaviour
    {
        public float ViewMagnitude = 10;
        public float SqrViewMagnitude
        {
            get
            {
                return ViewMagnitude * ViewMagnitude;
            }
        }

        private Bot m_botComponent;
        public Bot BotComponent
        {
            get
            {
                if (m_botComponent == null)
                {
                    m_botComponent = GetComponent<Bot>();
                }
                return m_botComponent;
            }

        }

        public float FieldOfViewAngle = 45;

        public string[] LookOutForTags;
        public Transform[] LookOutForTransforms;

        public class OnTransformSeenEvent : UnityEvent<Transform> { }
        public OnTransformSeenEvent OnTransformSeen = new OnTransformSeenEvent();

        private List<Transform> m_tagLookOutForTransforms = new List<Transform>();
        public List<Transform> GetTagLookOutForTransforms(bool refresh = false)
        {
            if (!refresh && m_tagLookOutForTransforms.Any())
            {
                m_tagLookOutForTransforms = m_tagLookOutForTransforms.Where(t => t != null).ToList();
                return m_tagLookOutForTransforms;
            }

            m_tagLookOutForTransforms = new List<Transform>();
            for (int i = 0; i < LookOutForTags.Length; i++)
            {
                try{
                    m_tagLookOutForTransforms.AddRange(GameObject.FindGameObjectsWithTag(LookOutForTags[i])
                                .Select(go => go.transform));
                }catch(Exception ex){
                    // Tag does not exist
                }
            }
            m_tagLookOutForTransforms.Remove(transform); // Remove self
            return m_tagLookOutForTransforms;
        }

        public IEnumerable<Transform> GetAllLookOutForTransforms(bool refresh = false)
        {
            return GetTagLookOutForTransforms(refresh).Union(LookOutForTransforms);
        }

        private bool IsWithinSight(Transform target, string tag = ""){
            if(!string.IsNullOrEmpty(tag) && target.tag != tag){
                return false;
            }

            return ProximityHelpers.IsWithinSight(transform, target, FieldOfViewAngle, SqrViewMagnitude);
        }

        public virtual bool IsWithinSight(string tag = "")
        {
            return GetAllLookOutForTransforms().Any(t => IsWithinSight(t, tag));
        }

        public virtual bool IsWithinSight<T>(string tag = "") where T: MonoBehaviour
        {
            return GetAllLookOutForTransforms().Any(t => IsWithinSight(t, tag) && t.GetComponent<T>() != null);
        }

        public IEnumerable<Transform> GetTransformsWithinSight(bool refreshList = false, string tag = "")
        {
            return GetAllLookOutForTransforms(refreshList).Where(t => IsWithinSight(t, tag));
        }

        public IEnumerable<T> GetWithinSight<T>(bool refreshList = false, string tag = "") where T : MonoBehaviour
        {
            return GetAllLookOutForTransforms(refreshList).Where(t => IsWithinSight(t, tag) && t.GetComponent<T>() != null).Select(t => t.GetComponent<T>()); ;
        }

        public Transform[] DoLookoutFor(bool newTransformsOnly = true, bool refreshList = false, string tag = "")
        {
            IEnumerable<Transform> targets;
            targets = GetTransformsWithinSight(refreshList, tag);

            Transform[] result = new Transform[0];

            if (newTransformsOnly)
            {
                result = targets.Where(t => !BotComponent.NoticedTransforms.Contains(t)).ToArray();
            }

            for (int i = 0; i < result.Length; i++)
            {
                OnTransformSeen.Invoke(result[i]);
            }

            return result;
        }

        public T[] DoLookoutFor<T>(bool newTransformsOnly = true, bool refreshList = false, string tag = "") where T : MonoBehaviour
        {
            IEnumerable<T> targets;
            targets = GetWithinSight<T>(refreshList, tag);

            T[] result = new T[0];

            if (newTransformsOnly)
            {
                result = targets.Where(t => !BotComponent.NoticedTransforms.Contains(t.transform)).Cast<T>().ToArray();
            }

            for (int i = 0; i < result.Length; i++)
            {
                OnTransformSeen.Invoke(result[i].transform);
            }

            return result;
        }

        // Draw the line of sight representation within the scene window
        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            ProximityHelpers.DrawGizmoLineOfSight(transform, FieldOfViewAngle, ViewMagnitude, IsWithinSight());
#endif
        }

    }
}