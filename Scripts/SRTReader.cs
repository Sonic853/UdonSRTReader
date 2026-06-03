
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
        // static string Pattern() => @"(\d+)?\n(\d{1,}:)?(\d{1,}:)?(\d{1,}).(\d+)\s?-->\s?(\d{1,}:)?(\d{1,}:)?(\d{1,}).(\d+)(.*(?:\r?(?!\r?).*)*)\n(.*(?:\r?\n(?!\r?\n).*)*)";
        static string Pattern() => @"(\d+)?\n(?:(\d{1,})?:)?(?:(\d{1,})?:)?(\d{1,}).(\d+)\s?-->\s?(?:(\d{1,})?:)?(?:(\d{1,})?:)?(\d{1,}).(\d+)(.*(?:\r?(?!\r?).*)*)\n(.*(?:\r?\n(?!\r?\n).*)*)";
        static string SubPattern() => @"(?:(\d+)?\n)?(?:(\d{1,})?:)?(?:(\d{1,})?:)?(\d{1,}).(\d+)\s?,\s?(?:(\d{1,})?:)?(?:(\d{1,})?:)?(\d{1,}).(\d+)(.*(?:\r?(?!\r?).*)*)\n(.*(?:\r?\n(?!\r?\n).*)*)";
        static Regex RegexSubtitle() => new Regex(Pattern());
        static Regex RegexSub() => new Regex(SubPattern());
        [SerializeField] private SRTSubtitle[] sRTSubtitles;
        public SRTSubtitle[] SRTSubtitles
        {
            get
            {
                return sRTSubtitles;
            }
        }
        void Start()
        {
            for (int i = 0; i < sRTSubtitles.Length; i++)
            {
                var sRTSubtitle = sRTSubtitles[i];
                if (
                sRTSubtitle.subtitleTimeStart.Length == 0
                || sRTSubtitle.subtitleTimeEnd.Length == 0
                || sRTSubtitle.subtitleText.Length == 0
                )
                    ReadSRTFile(ref sRTSubtitle);
            }
        }
        public static void ReadSRTFile(ref SRTSubtitle sRTSubtitle)
        {
            var srtFile = sRTSubtitle.srtFile;
            var srtString = sRTSubtitle.srtString;
            if (srtFile == null && string.IsNullOrEmpty(srtString))
            {
                return;
            }
            if (!string.IsNullOrEmpty(srtString)) srtString = srtFile.text;
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
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var timeStart = ParseTime(match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value);
                var timeEnd = ParseTime(match.Groups[6].Value, match.Groups[7].Value, match.Groups[8].Value, match.Groups[9].Value);
                var text = match.Groups[11].Value;
                subtitleTimeStartList.Add(timeStart);
                subtitleTimeEndList.Add(timeEnd);
                subtitleTextList.Add(text);
            }
            var subtitleTimeStart = new float[subtitleTimeStartList.Count];
            var subtitleTimeEnd = new float[subtitleTimeEndList.Count];
            var subtitleText = new string[subtitleTextList.Count];
            var lineTime = new float[subtitleTimeStartList.Count];
            for (int i = 0; i < subtitleTimeStartList.Count; i++)
            {
                var timeStart = subtitleTimeStart[i] = subtitleTimeStartList[i].Float;
                var timeEnd = subtitleTimeEnd[i] = subtitleTimeEndList[i].Float;
                subtitleText[i] = subtitleTextList[i].String;
                lineTime[i] = timeEnd - timeStart;
            }
            sRTSubtitle.subtitleTimeStart = subtitleTimeStart;
            sRTSubtitle.subtitleTimeEnd = subtitleTimeEnd;
            sRTSubtitle.subtitleText = subtitleText;
            sRTSubtitle.lineTime = lineTime;
        }
        public static float ParseTime(string hour, string minute, string second, string millisecond)
        {
            return float.Parse(hour ?? "0") * 3600 + float.Parse(minute ?? "0") * 60 + float.Parse(second ?? "0") + float.Parse(millisecond ?? "0") / 1000;
        }
    }
}
