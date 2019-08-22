namespace RSToolkit.Helpers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class CollectionHelpers 
    {
        public static int GetCircularIndex(int index, int size){
            return ((index % size) + size) % size;
        }
    }
}