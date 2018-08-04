// =====================================================================
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
    [Tooltip("Delete spline Control Points")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvydeletecontrolpoints")]
    public class CurvyDeleteControlPoints : FsmStateAction
    {
        [RequiredField, Tooltip("The Spline to address")]
        [CheckForComponent(typeof(CurvySpline))]
        public FsmOwnerDefault GameObject;
        [RequiredField, Tooltip("The start Control Point Index to delete")]
        public FsmInt StartIndex;
        [RequiredField, Tooltip("The number of Control Points to delete")]
        public FsmInt Count;


        // Code that runs on entering the state.
        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go)
            {
                CurvySpline spl = go.GetComponent<CurvySpline>();
                if (spl)
                {
                    if (!StartIndex.IsNone && !Count.IsNone && Count.Value > 0)
                    {
                        for (int i = 0; i < Count.Value; i++)
                            spl.Delete(spl.ControlPointsList[StartIndex.Value]);

                        spl.Refresh();
                    }

                }
            }

            Finish();
        }

        public override void Reset()
        {
            base.Reset();
            GameObject = null;
            StartIndex = 0;
            Count = 1;
        }


    }
}