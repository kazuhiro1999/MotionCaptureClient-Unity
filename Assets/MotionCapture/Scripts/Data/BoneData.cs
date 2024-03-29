using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MotionCapture.Data
{
    [System.Serializable]
    public class BoneData
    {
        public string Name;
        public Vector3 Position;

        public BoneData(string name, Vector3 position) {
            this.Name = name;
            this.Position = position;
        }
    }
}
