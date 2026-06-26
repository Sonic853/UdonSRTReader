
using JLChnToZ.VRC.VVMW;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Subtitle.VizVid
{
    public class VizVidSubtitleLink : UdonSharpBehaviour
    {
        public Core vizVidCore;
        public SubtitleUI subtitleUI;
        void Start()
        {
            vizVidCore._AddListener(this, "_onVideoStart");
            vizVidCore._AddListener(this, "_onVideoEnd");
        }
        void Update()
        {
            if (subtitleUI == null)
            {
                enabled = false;
                return;
            }
            subtitleUI.UpdateText(vizVidCore.Time);
        }
        public void _onVideoStart()
        {
            // Debug.Log("[VizVidSubtitleLink] _onVideoStart");
            if (subtitleUI == null)
            {
                enabled = false;
                return;
            }
            enabled = true;
            subtitleUI.Show(vizVidCore.Url, false);
        }
        public void _onVideoEnd()
        {
            // Debug.Log("[VizVidSubtitleLink] _onVideoEnd");
            if (subtitleUI != null) subtitleUI.Hide();
            enabled = false;
        }
    }
}
