// =====================================================================
// Copyright 2013-2016 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================
using UnityEngine;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;
using FluffyUnderware.Curvy;

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Gets a Control Point or Segment GameObject")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvygetcontrolpoint")]
    public class CurvyGetControlPoint : FsmStateAction
    {
        [RequiredField, Tooltip("The Spline to address")]
        [CheckForComponent(typeof(CurvySpline))]
        public FsmOwnerDefault GameObject;

        [RequiredField, Tooltip("Index of Control Point or Segment")]
        public FsmInt Index;

        [Tooltip("Whether to retrieve Segments or Control Points")]
        public FsmBool GetSegment;

        [UIHint(UIHint.Variable), Tooltip("Store the Control Point")]
        public FsmGameObject StoreObject;

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go)
            {
                CurvySpline spl = go.GetComponent<CurvySpline>();
                if (spl)
                {
                    if (GetSegment.Value)
                    {
                        if (spl.Count > 0)
                        {
                            StoreObject.Value = spl[Mathf.Clamp(Index.Value, 0, spl.Count - 1)].gameObject;
                        }
                    }
                    else if (spl.ControlPointCount > 0)
                        StoreObject.Value = spl.ControlPointsList[Mathf.Clamp(Index.Value, 0, spl.ControlPointCount - 1)].gameObject;
                }
            }

            Finish();
        }

        public override void Reset()
        {
            base.Reset();
            GameObject = null;
            StoreObject = new FsmGameObject() { UseVariable = true };
            GetSegment = false;
            Index = 0;
        }

    }
}