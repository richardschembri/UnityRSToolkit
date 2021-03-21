using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RSToolkit.Voxel{
    [CustomEditor(typeof(VoxelAnimator), true)]
    public class VoxelAnimatorEditor : Editor
    {
        VoxelAnimator _targetVoxelAnimator;
        void OnEnable()
        {
            _targetVoxelAnimator = (VoxelAnimator)target;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if(!Application.isPlaying || !_targetVoxelAnimator.DebugMode){
                return;
            }

            if (_targetVoxelAnimator.CurrentClip != null)
            {
                EditorGUILayout.LabelField($"Current Clip: { _targetVoxelAnimator.CurrentClip.Value.name }", EditorStyles.boldLabel);
            }
        }
    }
}
