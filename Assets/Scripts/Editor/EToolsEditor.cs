using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace EToolsEditor
{
    
    public class EAudio
    {
        static AudioClip curClip;
        //Reflection Target  UnityEditor.AudioUtil;
        public static void AttachClipTo(AudioClip clip)
        {
            curClip = clip;
        }
        public static bool IsAudioPlaying()
        {
            if (curClip != null)
#if UNITY_2019_3_OR_NEWER
                return (bool)GetMethod("IsPreviewClipPlaying").Invoke(null, new object[] { });
#else
                return (bool)GetMethod("IsClipPlaying").Invoke(null, new object[] { curClip });
#endif
            return false;
        }
        public static int GetSampleDuration()
        {
            if(curClip!=null)
#if UNITY_2019_3_OR_NEWER
                return curClip.samples;
#else
                return(int)GetMethod("GetSampleCount").Invoke(null, new object[] { curClip });
#endif
            return -1;
        }
        public static int GetCurSample()
        {

            if (curClip != null)
#if UNITY_2019_3_OR_NEWER
                return (int)GetMethod("GetPreviewClipSamplePosition").Invoke(null, new object[] { });
#else
                return (int)GetMethod("GetClipSamplePosition").Invoke(null, new object[] { curClip });
#endif
            return -1;
        }
        public static float GetCurTime()
        {
            if (curClip != null)
#if UNITY_2019_3_OR_NEWER
                return (float)GetMethod("GetPreviewClipPosition").Invoke(null, new object[] { });
#else
                return (float)GetMethod("GetClipPosition").Invoke(null, new object[] { curClip});
#endif
            return -1;
        }
        public static void PlayClip()
        {
            if (curClip != null)
#if UNITY_2019_3_OR_NEWER
                GetMethod("PlayPreviewClip").Invoke(null, new object[] { curClip,0,false });
#else
                GetMethod("PlayClip").Invoke(null, new object[] { curClip,0,false });
#endif
        }
        public static void PauseClip()
        {
            if (curClip != null)
#if UNITY_2019_3_OR_NEWER
                GetMethod("PausePreviewClip").Invoke( null,  new object[] { } );
#else
                GetMethod("PauseClip").Invoke( null,  new object[] { curClip } );
#endif
        }
        public static void StopClip()
        {
            if(curClip!=null)
#if UNITY_2019_3_OR_NEWER
                GetMethod("StopAllPreviewClips").Invoke(null,  new object[] { } );
#else
                GetMethod("StopClip").Invoke(null,  new object[] { curClip } );
#endif
        }
        public static void ResumeClip()
        {
            if (curClip != null)
#if UNITY_2019_3_OR_NEWER
                GetMethod("ResumePreviewClip").Invoke(null, new object[] { });
#else
                GetMethod("ResumeClip").Invoke(null, new object[] { curClip });
#endif
        }
        public static void SetSamplePosition(int startSample)
        {
#if UNITY_2019_3_OR_NEWER
            GetMethod("SetPreviewClipSamplePosition").Invoke(null, new object[] { curClip, startSample });
#else
            GetMethod("SetClipSamplePosition").Invoke(null, new object[] { curClip, startSample });
#endif
        }
        static MethodInfo GetMethod(string methodName)
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
           return  audioUtilClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
        }
    }
}
