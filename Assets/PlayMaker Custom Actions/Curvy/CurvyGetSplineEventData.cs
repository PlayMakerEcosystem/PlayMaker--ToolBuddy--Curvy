// =====================================================================
// Copyright 2013-2016 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================
using UnityEngine;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Get Control Point Event Data")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvygetsplineeventdata")]
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
            }
            
            Finish();
        }

        protected virtual void getData(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;

            var se = e as CurvySplineEventArgs;
            if (se!=null)
            {
                if (!StoreSpline.IsNone)
                    StoreSpline.Value = se.Spline;
                if (!StoreGameObject.IsNone)
                    StoreGameObject.Value = se.Spline.gameObject;
            }
        }



    }
}