// =====================================================================
// Copyright 2013-2016 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================
using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;
using FluffyUnderware.Curvy;

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Fire an event when the Curvy Spline is fully loaded")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvyinitialize")]
    public class CurvyInitialize : FsmStateAction
    {

        [RequiredField, Tooltip("The Spline to address")]
        [CheckForComponent(typeof(CurvySplineBase))]
        public FsmOwnerDefault GameObject;

        [Tooltip("Event to fire when the Curvy spline is fully loaded")]
        public FsmEvent successEvent;

        CurvySplineBase _spl;

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go)
                _spl = go.GetComponent<CurvySplineBase>();
        }

        public override void OnUpdate()
        {
            if (_spl && _spl.IsInitialized)
            {
                Fsm.Event(successEvent);
                Finish();
            }
        }

        public override void Reset()
        {
            base.Reset();
            GameObject = null;
            successEvent = FsmEvent.Finished;
        }
    }
}