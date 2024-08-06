// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
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
    [HelpUrl(AssetInformation.DocsRedirectionBaseUrl + "pmcurvyinitialize")]
    public class CurvyInitialize : FsmStateAction
    {

        [RequiredField, Tooltip("The Spline to address")]
        [CheckForComponent(typeof(CurvySpline))]
        public FsmOwnerDefault GameObject;

        [Tooltip("Event to fire when the Curvy spline is fully loaded")]
        public FsmEvent successEvent;

        CurvySpline _spl;

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go)
                _spl = go.GetComponent<CurvySpline>();
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