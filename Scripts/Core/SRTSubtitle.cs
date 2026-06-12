
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.SRT
{
    public class SRTSubtitle : UdonSharpBehaviour
    {
        public TextAsset srtFile;
        [TextArea(3, 10)]
        public string srtString;
        public VRCUrl videoUrl;
        [TextArea(3, 10)]
        public string[] subtitleText = new string[0];
        public float[] subtitleTimeStart = new float[0];
        public float[] subtitleTimeEnd = new float[0];
        public float[] lineTime = new float[0];
        public float offset = 0f;
        public int[] GetTextIndex(float time, float offset = 0f)
        {
            time += offset;
            var sRTSubtitle = this;
            if (subtitleText.Length == 0) SRTReader.ReadSRTFile(ref sRTSubtitle);
            var indexList = new DataList();
            for (int i = 0; i < subtitleText.Length; i++)
            {
                var _subtitleTimeStart = subtitleTimeStart[i];
                if (_subtitleTimeStart <= time && subtitleTimeEnd[i] >= time)
                {
                    indexList.Add(i);
                }
                if (_subtitleTimeStart > time) break;
            }
            var result = new int[indexList.Count];
            if (indexList.Count == 0) return result;
            for (int i = 0; i < indexList.Count; i++)
            {
                result[i] = indexList[i].Int;
            }
            return result;
        }
        public string[] GetText(float time, float offset = 0f)
        {
            var sRTSubtitle = this;
            if (subtitleText.Length == 0) SRTReader.ReadSRTFile(ref sRTSubtitle);
            var index = GetTextIndex(time, offset);
            var result = new string[index.Length];
            if (index.Length == 0) return result;
            for (int i = 0; i < index.Length; i++)
            {
                result[i] = subtitleText[index[i]];
            }
            return result;
        }
    }
}
