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
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvyinsertcontrolpoints")]
    public class CurvyInsertControlPoints : FsmStateAction
    {
        public enum InsertMode { Before, After };

        [RequiredField, Tooltip("The Spline to address")]
        [CheckForComponent(typeof(CurvySpline))]
        public FsmOwnerDefault GameObject;
        [RequiredField, Tooltip("The Control Point Index to add before/after")]
        public FsmInt Index;
        public InsertMode Mode = InsertMode.After;
        [RequiredField, Tooltip("Points to insert")]
        public FsmVector3[] Points;
        public Space Space;


        // Code that runs on entering the state.
        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go)
            {
                CurvySpline spl = go.GetComponent<CurvySpline>();
                if (spl)
                {
                    if (!Index.IsNone)
                    {
                        CurvySplineSegment seg=(Index.Value>=0 && Index.Value<spl.ControlPointCount) ? spl.ControlPointsList[Index.Value] : null;
                        for (int i = 0; i < Points.Length; i++)
                        {
                            if (!Points[i].IsNone)
                            {
                                CurvySplineSegment newCP;
                                if (Mode == InsertMode.After)
                                    newCP = spl.InsertAfter(seg);
                                else
                                    newCP = spl.InsertBefore(seg);
                                if (Space == Space.Self)
                                    newCP.SetLocalPosition(Points[i].Value);
                                else
                                    newCP.SetPosition(Points[i].Value);
                            }
                        }
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
            Index = -1;
            Points = new FsmVector3[1];
            Space = Space.Self;
            Mode = InsertMode.After;
        }


    }
}