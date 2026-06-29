
using JLChnToZ.VRC.VVMW;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Subtitle.VizVid
{
    public class VizVidSubtitleLink : UdonSharpBehaviour
    {
        public Core videoCore;
        public SubtitleUI subtitleUI;
        void Start()
        {
            videoCore._AddListener(this, "_onVideoStart");
            videoCore._AddListener(this, "_onVideoEnd");
        }
        void Update()
        {
            if (subtitleUI == null)
            {
                enabled = false;
                return;
            }
            subtitleUI.UpdateText(videoCore.Time);
        }
        public void _onVideoStart()
        {
            // Debug.Log("[VizVidSubtitleLink] _onVideoStart");
            if (subtitleUI == null || float.IsInfinity(videoCore.Duration) || videoCore.Duration <= 0)
            {
                enabled = false;
                return;
            }
            enabled = true;
            subtitleUI.Show(videoCore.Url, false);
        }
        public void _onVideoEnd()
        {
            // Debug.Log("[VizVidSubtitleLink] _onVideoEnd");
            if (subtitleUI != null) subtitleUI.Hide();
            enabled = false;
        }
    }
}
