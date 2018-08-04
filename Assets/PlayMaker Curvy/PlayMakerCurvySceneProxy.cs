// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================
using UnityEngine;
using System.Collections;

using HutongGames.PlayMaker;

namespace FluffyUnderware.Curvy.PlayMaker
{
    public class PlayMakerCurvySceneProxy : MonoBehaviour
    {
        public static PlayMakerFSM fsm;

        // Use this for initialization
        void Start()
        {
            PlayMakerCurvySceneProxy.fsm = GetComponent<PlayMakerFSM>();
        }

       
    }
}