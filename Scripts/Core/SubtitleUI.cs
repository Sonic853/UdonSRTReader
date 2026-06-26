
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System;
using TMPro;

namespace Sonic853.Subtitle
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
            UpdateText(time);
        }
        public void UpdateText(float _currentTime)
        {
            if (subtitle == null && subtitleTMP == null)
            {
                Debug.LogWarning("[Sonic853.Subtitle.SubtitleUI.Update] No subtitle component found");
                enabled = false;
                return;
            }
            time = _currentTime;
            if (targetSubtitle == null || lastTime == time) { return; }
            lastTime = time;
            var subtitleText = string.Join("\n", targetSubtitle.GetText(time));
            if (subtitleTMP != null)
            {
                if (subtitleTMP.text == subtitleText) { return; }
                subtitleTMP.text = subtitleText;
                if (subtitleDisplay == null) subtitleDisplay = subtitleTMP.gameObject;
            }
            if (subtitle != null)
            {
                if (subtitle.text == subtitleText) { return; }
                subtitle.text = subtitleText;
                if (subtitleDisplay == null) subtitleDisplay = subtitle.gameObject;
            }
            subtitleDisplay.SetActive(!string.IsNullOrEmpty(subtitleText));
        }
        public void Show(VRCUrl url, bool enableUpdate = true)
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
            enabled = enableUpdate;
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
