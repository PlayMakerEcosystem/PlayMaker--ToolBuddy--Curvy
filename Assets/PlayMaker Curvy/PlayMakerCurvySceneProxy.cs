// =====================================================================
// Copyright 2013-2016 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
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