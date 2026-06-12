
using System;
using System.Text.RegularExpressions;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.SRT
{
    public class SRTReader : UdonSharpBehaviour
    {
        public static SRTReader Instance()
        {
            return (SRTReader)GameObject.Find("Sonic853.SRTReader").GetComponent(typeof(UdonBehaviour));
        }
        bool isLoaded = false;
        // static string Pattern() => @"(\d+)?\n(\d{1,}:)?(\d{1,}:)?(\d{1,}).(\d+)\s?-->\s?(\d{1,}:)?(\d{1,}:)?(\d{1,}).(\d+)(.*(?:\r?(?!\r?).*)*)\n(.*(?:\r?\n(?!\r?\n).*)*)";
        static string Pattern() => @"(\d+)?\n(?:(\d{1,})?:)?(?:(\d{1,})?:)?(\d{1,}).(\d+)\s?-->\s?(?:(\d{1,})?:)?(?:(\d{1,})?:)?(\d{1,}).(\d+)(.*(?:\r?(?!\r?).*)*)\n(.*(?:\r?\n(?!\r?\n).*)*)";
        static string SubPattern() => @"(?:(\d+)?\n)?(?:(\d{1,})?:)?(?:(\d{1,})?:)?(\d{1,}).(\d+)\s?,\s?(?:(\d{1,})?:)?(?:(\d{1,})?:)?(\d{1,}).(\d+)(.*(?:\r?(?!\r?).*)*)\n(.*(?:\r?\n(?!\r?\n).*)*)";
        static Regex RegexSubtitle() => new Regex(Pattern());
        static Regex RegexSub() => new Regex(SubPattern());
        [SerializeField] private SRTSubtitle[] sRTSubtitles;
        public SRTSubtitle[] SRTSubtitles => sRTSubtitles;
        [SerializeField] SRTSubtitle sRTSubtitlePrefab;
        void Start()
        {
            LoadSRTFiles();
        }
        public void LoadSRTFiles(bool force = false)
        {
            for (int i = 0; i < sRTSubtitles.Length; i++)
            {
                var sRTSubtitle = sRTSubtitles[i];
                if (
                force
                || sRTSubtitle.subtitleTimeStart.Length == 0
                || sRTSubtitle.subtitleTimeEnd.Length == 0
                || sRTSubtitle.subtitleText.Length == 0
                )
                {
                    ReadSRTFile(ref sRTSubtitle);
                }
            }
            isLoaded = true;
        }
        public static void ReadSRTFile(ref SRTSubtitle sRTSubtitle)
        {
            var srtFile = sRTSubtitle.srtFile;
            var srtString = sRTSubtitle.srtString;
            if (srtFile == null && string.IsNullOrEmpty(srtString))
            {
                return;
            }
            if (string.IsNullOrEmpty(srtString)) sRTSubtitle.srtString = srtString = srtFile.text;
            var regexSubtitle = RegexSubtitle();
            var matches = regexSubtitle.Matches(srtString);
            if (matches.Count == 0)
            {
                var regexSub = RegexSub();
                matches = regexSub.Matches(srtString);
            }
            var subtitleTimeStartList = new DataList();
            var subtitleTimeEndList = new DataList();
            var subtitleTextList = new DataList();
            var lineTimeList = new DataList();
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var timeStart = ParseTime(match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value);
                var timeEnd = ParseTime(match.Groups[6].Value, match.Groups[7].Value, match.Groups[8].Value, match.Groups[9].Value);
                var text = match.Groups[11].Value;
                var index = FloatArrayFindMax(subtitleTimeStartList, timeStart);
                subtitleTimeStartList.Insert(index, timeStart);
                subtitleTimeEndList.Insert(index, timeEnd);
                subtitleTextList.Insert(index, text);
                lineTimeList.Insert(index, timeEnd - timeStart);
            }
            var subtitleTimeStart = new float[subtitleTimeStartList.Count];
            var subtitleTimeEnd = new float[subtitleTimeEndList.Count];
            var subtitleText = new string[subtitleTextList.Count];
            var lineTime = new float[lineTimeList.Count];
            for (int i = 0; i < subtitleTimeStartList.Count; i++)
            {
                subtitleTimeStart[i] = subtitleTimeStartList[i].Float;
                subtitleTimeEnd[i] = subtitleTimeEndList[i].Float;
                subtitleText[i] = subtitleTextList[i].String;
                lineTime[i] = lineTimeList[i].Float;
            }
            sRTSubtitle.subtitleTimeStart = subtitleTimeStart;
            sRTSubtitle.subtitleTimeEnd = subtitleTimeEnd;
            sRTSubtitle.subtitleText = subtitleText;
            sRTSubtitle.lineTime = lineTime;
        }
        public SRTSubtitle GetSRTSubtitle(VRCUrl videoUrl)
        {
            if (videoUrl == null) return null;
            var videoUrlString = videoUrl.ToString();
            if (string.IsNullOrEmpty(videoUrlString)) return null;
            foreach (var sRTSubtitle in sRTSubtitles)
            {
                if (sRTSubtitle.videoUrl == null) continue;
                if (sRTSubtitle.videoUrl.ToString() == videoUrlString)
                {
                    return sRTSubtitle;
                }
            }
            return null;
        }
        public SRTSubtitle AddSRTSubtitle(string srtString, VRCUrl videoUrl = null)
        {
            if (string.IsNullOrEmpty(srtString)) return null;
            foreach (var sTRS in sRTSubtitles)
            {
                if (sTRS.srtString == srtString)
                {
                    return sTRS;
                }
            }
            var sRTSubtitleObject = Instantiate(sRTSubtitlePrefab.gameObject);
            sRTSubtitleObject.SetActive(true);
            sRTSubtitleObject.transform.parent = sRTSubtitlePrefab.transform.parent;
            var sRTSubtitle = (SRTSubtitle)sRTSubtitleObject.GetComponent(typeof(UdonBehaviour));
            sRTSubtitle.srtString = srtString;
            sRTSubtitle.videoUrl = videoUrl;
            ReadSRTFile(ref sRTSubtitle);
            var sRTSubtitlesLength = sRTSubtitles.Length;
            var _sRTSubtitles = new SRTSubtitle[sRTSubtitlesLength + 1];
            Array.Copy(sRTSubtitles, _sRTSubtitles, sRTSubtitlesLength);
            _sRTSubtitles[sRTSubtitlesLength] = sRTSubtitle;
            sRTSubtitles = _sRTSubtitles;
            return sRTSubtitle;
        }
        static float ParseTime(string hour, string minute, string second, string millisecond)
        {
            return float.Parse(hour ?? "0") * 3600 + float.Parse(minute ?? "0") * 60 + float.Parse(second ?? "0") + float.Parse(millisecond ?? "0") / 1000;
        }
        /// <summary>
        /// 在int数组中从0开始查找比number大的最小值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        static int FloatArrayFindMax(DataList array, float number)
        {
            if (array.Count == 0) return 0;
            for (int i = 0; i < array.Count; i++)
            {
                var data = array[i].Float;
                if (data > number)
                {
                    return i;
                }
            }
            return array.Count - 1;
        }
        public string[] GetTextIndex(int _index, float time, float offset = 0f)
        {
            if (sRTSubtitles.Length <= _index) return new string[0];
            return sRTSubtitles[_index].GetText(time, offset);
        }
    }
}
