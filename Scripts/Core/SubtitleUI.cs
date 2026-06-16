
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System;
using TMPro;

namespace Sonic853.SRT
{
    public class SubtitleUI : UdonSharpBehaviour
    {
        public SRTReader sRTReader;
        public SRTSubtitle targetSubtitle;
        public GameObject subtitleDisplay;
        public Text subtitle;
        public TMP_Text subtitleTMP;
        float lastTime;
        public float time;
        public bool autoSelect = true;
        void Start()
        {
            if (sRTReader == null) sRTReader = SRTReader.Instance();
        }

        void Update()
        {
            if (subtitle == null && subtitleTMP == null)
            {
                Debug.LogWarning("[Sonic853.SRT.SubtitleUI.Update] No subtitle component found");
                enabled = false;
                return;
            }
            if (targetSubtitle == null) { return; }
            if (lastTime == time) return;
            lastTime = time;
            var subtitleText = string.Join("\n", targetSubtitle.GetText(time));
            if (subtitleTMP != null)
            {
                subtitleTMP.text = subtitleText;
                if (subtitleDisplay == null) subtitleDisplay = subtitleTMP.gameObject;
            }
            if (subtitle != null)
            {
                subtitle.text = subtitleText;
                if (subtitleDisplay == null) subtitleDisplay = subtitle.gameObject;
            }
            subtitleDisplay.SetActive(!string.IsNullOrEmpty(subtitleText));
        }
        public void Show(VRCUrl url)
        {
            if (sRTReader != null && autoSelect)
            {
                var subtitle = sRTReader.GetSRTSubtitle(url);
                if (subtitle != null)
                {
                    targetSubtitle = subtitle;
                }
            }
            ClearText();
            enabled = true;
        }
        public void Hide()
        {
            ClearText();
            enabled = false;
        }
        public void ClearText()
        {
            if (subtitleTMP != null)
            {
                subtitleTMP.text = "";
                if (subtitleDisplay == null) subtitleDisplay = subtitleTMP.gameObject;
            }
            if (subtitle != null)
            {
                subtitle.text = "";
                if (subtitleDisplay == null) subtitleDisplay = subtitle.gameObject;
            }
            subtitleDisplay.SetActive(false);
        }
    }
}
