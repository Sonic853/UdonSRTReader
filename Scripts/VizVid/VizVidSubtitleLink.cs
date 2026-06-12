
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
            enabled = true;
            subtitleUI.enabled = true;
        }
        public void _onVideoEnd()
        {
            subtitleUI.enabled = false;
            subtitleUI.subtitleDisplay.SetActive(false);
            enabled = false;
        }
    }
}
