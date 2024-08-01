﻿// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================
using UnityEngine;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;
using FluffyUnderware.Curvy;

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Delete all spline Control Points")]
    [HelpUrl(AssetInformation.DocsRedirectionBaseUrl + "pmcurvyclearspline")]
    public class CurvyClearSpline : FsmStateAction
    {
        [RequiredField, Tooltip("The Spline to address")]
        [CheckForComponent(typeof(CurvySpline))]
        public FsmOwnerDefault GameObject;


        // Code that runs on entering the state.
        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go)
            {
                CurvySpline spl = go.GetComponent<CurvySpline>();
                if (spl)
                {
                    spl.Clear();
                    spl.Refresh();
                }
            }

            Finish();
        }

        public override void Reset()
        {
            base.Reset();
            GameObject = null;
        }


    }
}