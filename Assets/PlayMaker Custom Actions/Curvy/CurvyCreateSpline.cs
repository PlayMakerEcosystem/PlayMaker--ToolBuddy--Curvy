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
    [Tooltip("Creates a Curvy spline")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvycreatespline")]
    public class CurvyCreateSpline : FsmStateAction
    {
        public CurvyInterpolation Interpolation;

        [Tooltip("Close the spline?")]
        public FsmBool CloseSpline;

        [Tooltip("Automatic end tangents?")]
        public FsmBool AutoEndTangents;

        [Tooltip("Granularity of internal approximation")]
        public FsmInt CacheDensity;

        [Tooltip("How the Up-Vector should be calculated")]
        public CurvyOrientation Orientation;

        [Tooltip("Automatically refresh when a Control Point position changed")]
        public FsmBool AutoRefresh;

        [UIHint(UIHint.Variable), Tooltip("Optionally store the created spline object")]
        public FsmGameObject storeObject;

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            CurvySpline spl = CurvySpline.Create();
            spl.Closed = CloseSpline.Value;
            spl.AutoEndTangents = AutoEndTangents.Value;
            spl.CacheDensity = CacheDensity.Value;
            spl.Orientation = Orientation;
            storeObject.Value = spl.gameObject;
            Finish();
        }

        public override void Reset()
        {
            base.Reset();
            CloseSpline = true;
            AutoEndTangents = true;
            CacheDensity = 25;
            Orientation = CurvyOrientation.Dynamic;
            AutoRefresh = true;
            storeObject = null;
        }

    }
}