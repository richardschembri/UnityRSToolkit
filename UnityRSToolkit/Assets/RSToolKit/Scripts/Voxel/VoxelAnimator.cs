// Based on https://github.com/matpow2/voxie/tree/master/unity3d
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Voxel{
    public class VoxelAnimator : RSMonoBehaviour
    {
        [System.Serializable]
        public struct VoxelAnimationClip{
            public enum ELoopType
            {
                FORWARD = 0,
                PINGPONG = 1
            }

            public string name;
            public Mesh[] meshes;
            public float interval;
            public int loops; // = -1;
            public ELoopType type; // = LoopType.Forward;

        }

        public VoxelAnimationClip[] animationClips;

        public int MeshIndex {get; private set;} = 0; 
        int _nextMeshIndex;

        private int _clipIndex = 0;
        public VoxelAnimationClip? CurrentClip {get{return animationClips.Length > 0 ? (VoxelAnimationClip?)animationClips[_clipIndex] : null;}} // = null;

        float _elapsedTime = 0.0f;
        public int RemainingLoops {get; private set;} = -1;
        bool reversed = false;

        private MeshFilter _meshFilterComponent;

        #region RSMonoBehaviour Functions

        public override bool Init(bool force = false){
            if(!base.Init(force)){
                return false;
            }

            _meshFilterComponent = GetComponent<MeshFilter>();
            return true;
        }
        #endregion RSMonoBehaviour Functions

        void AdjustMeshIndexForLoop(int nextMeshIndex){

            if (RemainingLoops != -1) {
                RemainingLoops--;
                if (RemainingLoops == 0)
                    return;
            }

            switch (CurrentClip.Value.type) {
                case VoxelAnimationClip.ELoopType.FORWARD:
                    nextMeshIndex = 0;
                    break;
                case VoxelAnimationClip.ELoopType.PINGPONG:
                    reversed = !reversed;
                    if (reversed)
                        nextMeshIndex = CurrentClip.Value.meshes.Length - 2;
                    else
                        nextMeshIndex = 1;
                    break;
            }

            MeshIndex = nextMeshIndex;
        }

        #region MonoBehaviour Functions

        void Update()
        {
            if(!Initialized){
                return;
            }
            if (CurrentClip == null || CurrentClip.Value.interval == 0.0f || CurrentClip.Value.loops == 0)
                return;
            _elapsedTime += Time.deltaTime;
            while (_elapsedTime > CurrentClip.Value.interval) {
                _elapsedTime -= CurrentClip.Value.interval;
                _nextMeshIndex = MeshIndex;
                if (reversed)
                    _nextMeshIndex--;
                else
                    _nextMeshIndex++;
                if (_nextMeshIndex >= CurrentClip.Value.meshes.Length || _nextMeshIndex < 0)
                    AdjustMeshIndexForLoop(_nextMeshIndex);
                else
                    MeshIndex = _nextMeshIndex;
                _meshFilterComponent.mesh = CurrentClip.Value.meshes[MeshIndex];
            }
        }
        #endregion MonoBehaviour Functions
    }
}
