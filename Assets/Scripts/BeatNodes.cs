using System.Collections.Generic;
using UnityEngine;
public enum enum_BeatType
{
    Invalid = -1,
    Single = 1,
    Double = 2,
    Triple = 3,
}
[CreateAssetMenu(fileName = "Nodes_", menuName = "BeatsNodes")]
public class BeatNodes : ScriptableObject
{
    [SerializeField,HideInInspector]    
    List<Node> l_Nodes = new List<Node>();
    public int  I_BeatPerMinute;
    public float F_BeatStartOffset;
    public AudioClip AC_ClipToPlay;
    #region Interact APIs
    Node tempNode;
    public void Clear()
    {
        l_Nodes.Clear();
    }
    public int GetPerfectScore()
    {
        int perfect=0;
        for (int i = 0; i < l_Nodes.Count; i++)
        {
            switch (l_Nodes[i].e_Type)
            {
                case enum_BeatType.Single:
                    perfect+=1;
                    break;
                case enum_BeatType.Double:
                    perfect+=2;
                    break;
                case enum_BeatType.Triple:
                    perfect += 3;
                    break;
            }
        }
        return perfect;
    }
    public List<Node> GetNodes()
    {
        return l_Nodes;
    }
    public void ForceSort()
    {
        l_Nodes.Sort((left,right)=> { return left.i_BeatPos >= right.i_BeatPos ? 1 : -1; });
    }
    public bool ContainsNode(int beatPos,bool isLeft)
    {
        tempNode = l_Nodes.Find(p => p.i_BeatPos == beatPos);
        return tempNode != null&& tempNode.b_IsLeft==isLeft;
    }
    public void SetNode(int beatPos,bool isLeft,enum_BeatType type)
    {
        tempNode = GetNodeByPos(beatPos);
        if (tempNode != null)
        {
            int index = l_Nodes.IndexOf(tempNode);
            Node n = new Node(beatPos, isLeft,type);
            l_Nodes[index] = n;
        }
        else
        {
            for (int i = 0; i < l_Nodes.Count; i++)
            {
                if (l_Nodes[i].i_BeatPos>beatPos)
                {
                    l_Nodes.Insert(i,new Node(beatPos,isLeft,type));
                    return;
                }
            }
            l_Nodes.Add(new Node(beatPos, isLeft,type));
        }
    }
    public void AdjustNode(int beatPos, enum_BeatType type)
    {

        tempNode = GetNodeByPos(beatPos);
        if (tempNode != null)
        {
            int index = l_Nodes.IndexOf(tempNode);
            Node n = new Node(beatPos, tempNode.b_IsLeft, type);
            l_Nodes[index] = n;
        }
        else
        {
            Debug.LogError("Can't Adjust A Unexisted Node!Pos:"+beatPos) ;
        }
    }
    public void RemoveNode(int beatPos)
    {
        tempNode = GetNodeByPos(beatPos);
        if (tempNode == null)
        {
            Debug.LogWarning(beatPos.ToString() + " Node Not Found Howdf U Remove A UnAddNode In Editor?");
            return;
        }
        l_Nodes.Remove(tempNode);
    }
    public Node GetNodeByPos(int beatPos)
    {
        return l_Nodes.Find(p => p.i_BeatPos == beatPos);
    }
    public Node GetNodeByIndex(int index)
    {
        return l_Nodes[index];
    }
    public int GetNodeIndex(int beatPos)
    {
        return l_Nodes.FindIndex(p=>p.i_BeatPos==beatPos);
    }
    public Dictionary<float,int> GetTotalBeatsCenterWithOffset(float f_beatEach)
    {
        Dictionary<float, int> dic = new Dictionary<float, int>();
        for (int i = 0; i < l_Nodes.Count; i++)
        {
            switch (l_Nodes[i].e_Type)
            {
                case enum_BeatType.Single:
                    {
                        dic.Add(F_BeatStartOffset + l_Nodes[i].i_BeatPos * f_beatEach,i);
                    }
                    break;
                case enum_BeatType.Double:
                    {

                        dic.Add(F_BeatStartOffset + l_Nodes[i].i_BeatPos * f_beatEach, i);
                        dic.Add(F_BeatStartOffset + l_Nodes[i].i_BeatPos * f_beatEach + f_beatEach * .5f, i);
                    }
                    break;
                case enum_BeatType.Triple:
                    {
                        dic.Add(F_BeatStartOffset + l_Nodes[i].i_BeatPos * f_beatEach, i);
                        dic.Add(F_BeatStartOffset + l_Nodes[i].i_BeatPos * f_beatEach + f_beatEach / 3,i);
                        dic.Add(F_BeatStartOffset + l_Nodes[i].i_BeatPos * f_beatEach + f_beatEach * 2 / 3,i);
                    }
                    break;
            }
        }
        return dic;
    }
    public List<float> BeatsCenterWithOffset(int beatPos, enum_BeatType type,float f_beatEach)
    {
        List<float> beatMids = new List<float>();
        switch (type)
        {
            case enum_BeatType.Single:
                {
                    beatMids.Add(F_BeatStartOffset + beatPos * f_beatEach);
                }
                break;
            case enum_BeatType.Double:
                {

                    beatMids.Add(F_BeatStartOffset + beatPos * f_beatEach);
                    beatMids.Add(F_BeatStartOffset + beatPos * f_beatEach + f_beatEach * .5f);
                }
                break;
            case enum_BeatType.Triple:
                {
                    beatMids.Add(F_BeatStartOffset + beatPos * f_beatEach);
                    beatMids.Add(F_BeatStartOffset + beatPos * f_beatEach + f_beatEach / 3);
                    beatMids.Add(F_BeatStartOffset + beatPos * f_beatEach+ f_beatEach * 2 / 3);
                }
                break;
        }
        return beatMids;
    }
    #endregion
}


[System.Serializable]
public class Node
{
    public Node(int beatPos, bool isLeft,enum_BeatType type)
    {
        i_BeatPos = beatPos;
        b_IsLeft = isLeft;
        e_Type = type;
    }
    public int i_BeatPos;
    public bool b_IsLeft;
    public enum_BeatType e_Type;
}