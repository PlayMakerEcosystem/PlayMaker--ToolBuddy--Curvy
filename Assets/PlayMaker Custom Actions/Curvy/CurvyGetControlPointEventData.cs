// =====================================================================
// Copyright 2013-2016 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using FluffyUnderware.Curvy.Controllers;
using UnityEngine;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Get Spline Event data")]
    [HelpUrl(AssetInformation.DocsRedirectionBaseUrl + "pmcurvygetcontrolpointeventdata")]
    public class CurvyGetControlPointEventData : CurvyGetSplineEventData
    {
        [UIHint(UIHint.Variable), Tooltip("Store the Control Point")]
        public FsmObject StoreControlPoint;


        protected override void getData(CurvySplineMoveEventArgs e)
        {
            base.getData(e);
            if (e == null)
                return;

            if (!StoreControlPoint.IsNone)
                StoreControlPoint.Value = e.ControlPoint;
        }



    }
}