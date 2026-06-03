
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.SRT
{
    public class SRTSubtitle : UdonSharpBehaviour
    {
        [TextArea(3, 10)]
        [SerializeField] public TextAsset srtFile;
        [SerializeField] public string srtString;
        [TextArea(3, 10)]
        public string[] subtitleText = new string[0];
        public float[] subtitleTimeStart = new float[0];
        public float[] subtitleTimeEnd = new float[0];
        public float[] lineTime = new float[0];
        public float offset = 0f;
    }
}
