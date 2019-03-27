using UnityEditor;
using UnityEngine;
using EToolsEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BeatNodes))]
public class BeatsEditor : Editor
{
    int I_RectHeight;
    int I_SliderWidth;
    int I_BeatsInView;
    int I_ViewBtnScale;
    int I_SliderBeatScale;
    int I_BeatCheckLine;
    enum_BeatType e_CurNodeType;

    float f_viewBeatHeight;
    float f_sliderBeatHeight;
    float f_sliderStartOffset;
    int i_totalBeats;
    Texture2D t2_BeatBtn;
    Texture2D t2_LineTex;

    BeatNodes beats;
    bool b_pause;
    int i_pauseSample;
    float f_curTime;
    float f_totalTime;
    float f_beatEach;
    int i_curSelectPos;

    float f_beatToScale;
    float f_beatCheck;

    Rect tempRect;
    Vector2 v2_mousePos = Vector2.zero;
    bool b_isLeftClick = false;
    bool Editable
    {
        get
        {
            return beats.AC_ClipToPlay != null;
        }
    }
    private void Awake()
    {
        I_RectHeight = 700;
        I_BeatCheckLine = 50;
        I_SliderWidth = 80;
        I_BeatsInView = 16;
        I_ViewBtnScale = 25;
        I_SliderBeatScale = 2;
        e_CurNodeType = enum_BeatType.Single;
        Event.current = new Event();
    }
    private void OnEnable()
    {
        beats = target as BeatNodes;
        if (!Editable )
        {
            return;
        }
        Init();
        t2_BeatBtn = Resources.Load<Texture2D>("Texture/Editor/BeatBtn");
        t2_LineTex = new Texture2D(1, 1);
        b_pause = false;
        i_pauseSample = 0;
        i_curSelectPos = -1;
        EditorApplication.update += Update;
    }
    private void OnDisable()
    {
        EditorApplication.update -= Update;
        i_pauseSample = 0;
        EAudio.StopClip();
        EditorUtility.SetDirty(target);
        beats.ForceSort();
    }
    void Update()
    {
        Repaint();
    }
    void Init()
    {
        EAudio.AttachClipTo(beats.AC_ClipToPlay);
        f_totalTime = beats.AC_ClipToPlay.length;

        f_beatEach = 60f / beats.I_BeatPerMinute;
        f_beatCheck = 0f;
        f_beatToScale = 0f;

        i_totalBeats = (int)(f_totalTime / f_beatEach);

        f_sliderStartOffset = beats.F_BeatStartOffset>0?I_RectHeight*(beats.F_BeatStartOffset/f_totalTime):0;
        f_sliderBeatHeight = (I_RectHeight - f_sliderStartOffset) / i_totalBeats;
        f_viewBeatHeight =((float)I_RectHeight -I_BeatCheckLine) / I_BeatsInView;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!Editable)
        {
            return;
        }

        DrawSongConroll();

        EventManage();

        DrawRect(GUILayoutUtility.GetRect(GUILayoutUtility.GetLastRect().width, I_RectHeight));

        v2_mousePos = Vector2.zero;

        if (GUI.changed)
        {
            Init();
        }
    }
    #region Common Function/Song Controll

    void SetSongPos(float posTime)
    {
        if (EAudio.IsAudioPlaying())
        {
            EAudio.SetSamplePosition((int)(EAudio.GetSampleDuration() * (posTime / f_totalTime)));
            i_pauseSample = EAudio.GetCurSample();
        }
        else
        {
            Play();
            EAudio.SetSamplePosition((int)(EAudio.GetSampleDuration() * (posTime / f_totalTime)));
            Pause();
        }
        f_beatCheck = 0;
    }
    void Pause()
    {
        i_pauseSample = EAudio.GetCurSample();
        EAudio.PauseClip();
        b_pause = true;
    }
    void Stop()
    {
        i_curSelectPos = -1;
        b_pause = false;
        i_pauseSample = 0;
        EAudio.StopClip();
        f_beatCheck = 0f;
    }
    void Play()
    {
        b_pause = false;
        i_pauseSample = 0;
        EAudio.PlayClip();
    }
    void Resume()
    {
        EAudio.ResumeClip();
        EAudio.SetSamplePosition(i_pauseSample);
        b_pause = false;
    }
    #endregion
    void EventManage()
    {
        if (Event.current.isMouse)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                v2_mousePos = Event.current.mousePosition;
            }
            b_isLeftClick = Event.current.button ==0;
        }
        else if (Event.current.isScrollWheel)
        {
            if (Event.current.delta.y > 0)
            {
                SetSongPos(f_curTime - .5f);
            }
            else if (Event.current.delta.y < 0)
            {

                SetSongPos(f_curTime + .5f);
            }
        }
        else if (Event.current.isKey && Event.current.type == EventType.KeyUp)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.F12:
                    {
                        beats.Clear();
                    }
                    break;
                case KeyCode.BackQuote:
                    {
                        if (!EAudio.IsAudioPlaying())
                        {
                            Play();
                        }
                        else if (b_pause)
                        {
                            Resume();
                        }
                        else
                        {
                            Pause();
                        }
                    }
                    break;
                case KeyCode.Escape:
                    {
                        if (EAudio.IsAudioPlaying())
                            Stop();
                    }
                    break;
                case KeyCode.W:
                    {
                        SetSongPos(f_curTime + 1f);
                    }
                    break;
                case KeyCode.S:
                    {
                        SetSongPos(f_curTime - 1f);
                    }
                    break;
            }
        }
    }
    void DrawSongConroll()
    {
        f_curTime = EAudio.GetCurTime();
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        if (EAudio.IsAudioPlaying())
        {
            if (!b_pause)
            {
                if (GUILayout.Button("Pause(~)"))
                {
                    Pause();
                }
            }
            else
            {
                if (GUILayout.Button("Resume(~)"))
                {
                    Resume();
                }
            }
        }
        else
        {
            if (GUILayout.Button("Play(~)"))
            {
                Play();
            }
        }
        GUILayout.Space(15);
        if (GUILayout.Button("Stop(Esc)"))
        {
            Stop();
        }
        GUILayout.Space(15);
        GUILayout.EndHorizontal();
    }
    #region Draw/Mouse Click
    void DrawRect(Rect mainRect)
    {
        Rect sliderRect = new Rect(mainRect.xMax-I_SliderWidth+5, mainRect.yMin, I_SliderWidth-5, mainRect.height);
        Rect viewRect = new Rect(mainRect.xMin, mainRect.yMin,mainRect.width-I_SliderWidth, mainRect.height);
        
        DrawView(viewRect);
        DrawSlider(sliderRect);
    }
    void DrawSlider(Rect totalRect)
    {
        GUI.color = Color.black;
        GUI.Box(totalRect, "");

        #region Progress Red Line
        GUI.color = Color.red;
        float curPos= f_curTime / f_totalTime;
        tempRect = new Rect(totalRect.xMin, totalRect.yMax - totalRect.height * curPos, totalRect.width, 1);
        GUI.DrawTexture(tempRect, t2_LineTex);
        #endregion

        for (int j = 0; j < 2; j++)
        {
            bool isLeft = j == 0;

            GUI.color = Color.gray;
            Rect midBorder = new Rect(totalRect.xMin + (totalRect.width / 3)*(j+1) - 2f, totalRect.yMin, 2f, totalRect.height);
            GUI.DrawTexture(midBorder, t2_LineTex);

            float f_startY = midBorder.yMax - I_SliderBeatScale - f_sliderStartOffset;
            for (int i = 0; i < i_totalBeats; i++)
            {
                GUI.color = GetNodeColor(true, beats.ContainsNode(i, isLeft) ? beats.GetNodeByPos(i).e_Type : enum_BeatType.Invalid);
                float f_rectY = f_startY - f_sliderBeatHeight * i;
                float f_rectX = midBorder.xMax - 2f - I_SliderBeatScale/2;
                tempRect = new Rect(f_rectX, f_rectY, I_SliderBeatScale * 2, (float)I_SliderBeatScale / 2);
                GUI.DrawTexture(tempRect, t2_LineTex);
            }
        }

        if (v2_mousePos != Vector2.zero && totalRect.Contains(v2_mousePos))
        {
            Vector2 offset = v2_mousePos - totalRect.position;
            float value = (totalRect.height - offset.y) / totalRect.height;
            SetSongPos(value * f_totalTime);
        }
    }
    void DrawView(Rect totalRect)
    {
        //Background
        GUI.color = Color.white;
        GUI.Box(totalRect, "");
        //Draw Main View
        float f_time = f_curTime - beats.F_BeatStartOffset;
        int i_startPos = Mathf.FloorToInt(f_time / f_beatEach);
        
        float f_timeParam = f_time >= 0 ? (f_time % f_beatEach)/f_beatEach: f_time/f_beatEach;
        for (int j = 0; j < 2; j++)
        {
            GUI.color = Color.gray;
            Rect lineRect = new Rect(totalRect.xMin + (totalRect.width / 4)*(j+2) - 2f, totalRect.yMin, 4f, totalRect.height);
            GUI.Box(lineRect, t2_LineTex);
            bool isLeft = j == 0;
            for (int i = 0; i < I_BeatsInView + 2; i++)
            {
                int i_curPos = i_startPos + i;
                bool clickable = true;
                if (i_curPos < 0 || i_curPos >= i_totalBeats)
                {
                    clickable = false;
                }
                if (i_curPos == i_curSelectPos&& beats.ContainsNode(i_curPos, isLeft))      //Draw High Light
                {
                    GUI.color = Color.grey;
                    tempRect = new Rect(lineRect.xMax - 2f - I_ViewBtnScale / 2-I_ViewBtnScale*.1f,
                        lineRect.yMax - I_BeatCheckLine - I_ViewBtnScale - I_ViewBtnScale * .1f
                        - (i - f_timeParam) * f_viewBeatHeight,
                        I_ViewBtnScale*1.2f, I_ViewBtnScale*1.2f);
                    GUI.DrawTexture(tempRect, t2_BeatBtn);
                }

                tempRect = new Rect(lineRect.xMax - 2f - I_ViewBtnScale / 2,
                    lineRect.yMax - I_BeatCheckLine - I_ViewBtnScale
                    - (i - f_timeParam) * f_viewBeatHeight,
                    I_ViewBtnScale, I_ViewBtnScale);

                GUI.color = GetNodeColor(clickable, beats.ContainsNode(i_curPos, isLeft) ? beats.GetNodeByPos(i_curPos).e_Type : enum_BeatType.Invalid);
                if (!totalRect.Contains(tempRect.position))
                {
                    continue;
                }
                GUI.DrawTexture(tempRect, t2_BeatBtn, ScaleMode.ScaleAndCrop);
                
                if (clickable && v2_mousePos != Vector2.zero && tempRect.Contains(v2_mousePos)) //Mouse Click
                {
                    if (!b_pause)
                    {
                        Pause();
                    }
                    if (b_isLeftClick)
                    {
                        if (beats.GetNodeByPos(i_curPos) == null||i_curSelectPos==i_curPos)
                        {
                            beats.SetNode(i_curPos, isLeft, e_CurNodeType);
                        }
                        i_curSelectPos = i_curPos;
                    }
                    else 
                    {
                        if (beats.ContainsNode(i_curPos, isLeft))
                        {
                            beats.RemoveNode(i_curPos);
                            i_curSelectPos = -1;
                        }
                    }
                }
            }
        }
        
        DrawViewExtra(totalRect, v2_mousePos);

        //Draw Beat Check Line(Red)
        GUI.color = new Color(1, 0, 0, .5f);
        tempRect = new Rect(totalRect.xMin, totalRect.yMax - I_BeatCheckLine, totalRect.width, 3f);
        GUI.DrawTexture(tempRect, t2_LineTex);
        //CheckLine Time/BeatPos
        tempRect = new Rect(totalRect.xMin, totalRect.yMax - I_BeatCheckLine - 20, 120, 20f);
        GUI.Label(tempRect, string.Format("Time:{0:000.0}/Pos:{1}", f_curTime, i_startPos>=0?i_startPos:0));
    }
    void DrawViewExtra(Rect totalRect,Vector2 v2_mousePos)
    {
        //Draw Node Counter
        GUI.color = Color.white;
        tempRect = new Rect(totalRect.xMin + 5, totalRect.yMin + 5, 120, 20);
        GUI.Box(tempRect, "Beats Set:"+ beats.GetNodes().Count.ToString() + "/" + i_totalBeats.ToString() );

        //Draw Total Time
        GUI.color = Color.white;
        tempRect = new Rect(totalRect.xMin + 5, totalRect.yMin + 30, 120, 20);
        GUI.Box(tempRect, "Clip Length:"+string.Format("{0:000.0}", beats.AC_ClipToPlay.length));

        //Draw Node Select
        if (i_curSelectPos != -1)
        {
            Node tempNode = beats.GetNodeByPos(i_curSelectPos);
            if (tempNode == null)
            {
                return;
            }
            List<float> beatsCenter = beats.BeatsCenterWithOffset(tempNode.i_BeatPos,tempNode.e_Type,f_beatEach);
            GUI.color = Color.white;
            tempRect = new Rect(totalRect.xMin + 5, totalRect.yMin + 55, 120, 40+beatsCenter.Count*20);
            GUI.Box(tempRect, "");

            tempRect = new Rect(totalRect.xMin + 10, totalRect.yMin + 60, 110, 20);
            GUI.Label(tempRect, "Ind:"+beats.GetNodeIndex(i_curSelectPos).ToString()+"/Pos:"+i_curSelectPos.ToString());
            tempRect = new Rect(totalRect.xMin + 10, totalRect.yMin + 80, 110, 20);
            GUI.Label(tempRect, "Type:" + beats.GetNodeByPos(i_curSelectPos).e_Type.ToString());

            for (int i = 0; i < beatsCenter.Count; i++)
            {
                tempRect = new Rect(totalRect.xMin + 10, totalRect.yMin + 80+(i+1)*20, 110, 20);
                GUI.Label(tempRect,"Beat " + (i+1).ToString() + ":" + beatsCenter[i].ToString());
            }

        }

        //Draw Node Type Select
        foreach (int i in System.Enum.GetValues(typeof(enum_BeatType)))
        {
            if (i == -1)
                continue;
            enum_BeatType type= (enum_BeatType)i;
            //HighLight
            if (type == e_CurNodeType)    
            {
                GUI.color = Color.grey;
                tempRect = new Rect(totalRect.xMin + 5-.1f*I_ViewBtnScale, totalRect.yMax-I_BeatCheckLine - 5 - (i * 1.1f + 1+.1f) * I_ViewBtnScale, I_ViewBtnScale*1.2f, I_ViewBtnScale*1.2f);
                GUI.DrawTexture(tempRect, t2_BeatBtn);
            }
            //Draw Tips
            GUI.color = Color.white;
            tempRect = new Rect(totalRect.xMin + 5 + I_ViewBtnScale + 10f, totalRect.yMax - I_BeatCheckLine - 5 - (i * 1.1f  + 1) * I_ViewBtnScale, 80, I_ViewBtnScale);
            GUI.Box(tempRect, type.ToString());

            //Draw Btn
            GUI.color = GetNodeColor(true,type);
            tempRect = new Rect(totalRect.xMin + 5, totalRect.yMax - I_BeatCheckLine - 5 - (i * 1.1f + 1) * I_ViewBtnScale, I_ViewBtnScale, I_ViewBtnScale);
            GUI.DrawTexture(tempRect,t2_BeatBtn);
            //Btn Click
            if (tempRect.Contains(v2_mousePos))
            {
                if (e_CurNodeType==type&&i_curSelectPos != -1)
                {
                    beats.AdjustNode(i_curSelectPos,e_CurNodeType);
                }
                e_CurNodeType = type;
            }

        }

        if (f_curTime - beats.F_BeatStartOffset > f_beatCheck)      //Check Beats
        {
            f_beatCheck += f_beatEach;
            f_beatToScale = 1f;
        }
        f_beatToScale = Mathf.Lerp(f_beatToScale, 0f, .02f);

        //Draw Beat Check Box
        GUI.color = Color.white;
        tempRect = new Rect(totalRect.xMax - 5 - 20, totalRect.yMax - 120 * f_beatToScale - 5, 20, 120 * f_beatToScale);
        GUI.Box(tempRect, "B");
    }
    #endregion
    #region ColorSetting
    Color GetNodeColor(bool editable, enum_BeatType type= enum_BeatType.Invalid)
    {
        if (!editable)
        {
            return Color.grey;
        }
        switch (type)
        {
            default:
                return Color.white;
            case enum_BeatType.Single:
                return Color.red;
            case enum_BeatType.Double:
                return new Color(1,.5f,0,1f);
            case enum_BeatType.Triple:
                return Color.magenta;
        }
    }
    #endregion
}
