
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Yamadev.YamaStream;

namespace Sonic853.Subtitle.Yama
{
    public class YamaSubtitleLink : YamaPlayerListener
    {
        public Controller videoCore;
        public SubtitleUI subtitleUI;
        void Start()
        {
            videoCore.AddListener(this);
        }
        void Update()
        {
            if (subtitleUI == null)
            {
                enabled = false;
                return;
            }
            subtitleUI.UpdateText(videoCore.VideoTime);
        }
        public override void OnVideoStart()
        {
            if (subtitleUI == null || videoCore.IsLive)
            {
                enabled = false;
                return;
            }
            enabled = true;
            var currentUrl = TrackUtils.GetUrl(videoCore.Track);
            subtitleUI.Show(currentUrl, false);
        }
        public override void OnVideoEnd()
        {
            if (subtitleUI != null) subtitleUI.Hide();
            enabled = false;
        }
    }
}
