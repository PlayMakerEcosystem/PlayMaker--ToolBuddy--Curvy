// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using FluffyUnderware.Curvy.Controllers;
using UnityEngine;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Get Control Point Event Data")]
    [HelpUrl(AssetInformation.DocsRedirectionBaseUrl + "pmcurvygetsplineeventdata")]
    public class CurvyGetSplineEventData : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Store the Spline")]
        public FsmObject StoreSpline;
        [UIHint(UIHint.Variable), Tooltip("Store the Spline's GameObject")]
        public FsmGameObject StoreGameObject;

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            switch (State.Fsm.LastTransition.EventName)
            {
                case "CURVY / ON CP REACHED":
                    getData(PlayMakerCurvyComponentProxy._OnCPReachedEventData);
                    break;
                case "CURVY / ON END REACHED":
                    getData(PlayMakerCurvyComponentProxy._OnEndReachedEventData);
                    break;
            }

            Finish();
        }

        protected virtual void getData(CurvySplineMoveEventArgs e)
        {
            if (e == null)
                return;

            if (!StoreSpline.IsNone)
                StoreSpline.Value = e.Spline;
            if (!StoreGameObject.IsNone)
                StoreGameObject.Value = e.Spline.gameObject;
        }



    }
}