
using JLChnToZ.VRC.VVMW;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.SRT.VizVid
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
            subtitleUI.time = vizVidCore.Time;
        }
        public void _onVideoStart()
        {
            // Debug.Log("[VizVidSubtitleLink] _onVideoStart");
            if (subtitleUI.sRTReader != null)
            {
                var subtitle = subtitleUI.sRTReader.GetSRTSubtitle(vizVidCore.Url);
                if (subtitle != null)
                {
                    subtitleUI.targetSubtitle = subtitle;
                }
            }
            enabled = true;
            subtitleUI.ClearText();
            subtitleUI.enabled = true;
        }
        public void _onVideoEnd()
        {
            // Debug.Log("[VizVidSubtitleLink] _onVideoEnd");
            subtitleUI.enabled = false;
            subtitleUI.ClearText();
            enabled = false;
        }
    }
}
