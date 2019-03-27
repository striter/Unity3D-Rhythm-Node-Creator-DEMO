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
                return (bool)GetMethod("IsClipPlaying").Invoke(null, new object[] { curClip });
            return false;
        }
        public static int GetSampleDuration()
        {
            if(curClip!=null)
              return(int)GetMethod("GetSampleCount").Invoke(null, new object[] { curClip });
            return -1;
        }
        public static int GetCurSample()
        {

            if (curClip != null)
                return (int)GetMethod("GetClipSamplePosition").Invoke(null, new object[] { curClip });
            return -1;
        }
        public static float GetCurTime()
        {
            if (curClip != null)
                return (float)GetMethod("GetClipPosition").Invoke(null, new object[] { curClip});
            return -1;
        }
        public static void PlayClip()
        {
            if (curClip != null)
                GetMethod("PlayClip").Invoke(null, new object[] { curClip,0,false });
        }
        public static void PauseClip()
        {
            if (curClip != null)
                GetMethod("PauseClip").Invoke( null,  new object[] { curClip } );
        }
        public static void StopClip()
        {
            if(curClip!=null)
            GetMethod("StopClip").Invoke(null,  new object[] { curClip } );
        }
        public static void ResumeClip()
        {
            if (curClip != null)
                GetMethod("ResumeClip").Invoke(null, new object[] { curClip });
        }
        public static void SetSamplePosition(int startSample)
        {
            GetMethod("SetClipSamplePosition").Invoke(null, new object[] { curClip, startSample });
        }
        static MethodInfo GetMethod(string methodName)
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
           return  audioUtilClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
        }
    }
}
