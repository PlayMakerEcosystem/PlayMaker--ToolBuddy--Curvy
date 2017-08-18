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
    [Tooltip("Get Spline Event data")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvygetcontrolpointeventdata")]
    public class CurvyGetControlPointEventData : CurvyGetSplineEventData
    {
        [UIHint(UIHint.Variable), Tooltip("Store the Control Point")]
        public FsmObject StoreControlPoint;

       
        protected override void getData(System.ComponentModel.CancelEventArgs e)
        {
            base.getData(e);
            if (e == null)
                return;

            var se = e as CurvyControlPointEventArgs;
            if (se != null)
            {
                if (!StoreControlPoint.IsNone)
                    StoreControlPoint.Value = se.ControlPoint;
            }
        }



    }
}