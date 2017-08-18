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
using FluffyUnderware.Curvy.Utils;


namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Set the various move properties value of curvy Spline Controller ")]
   // [HelpUrl(CurvySpline.DOCLINK + "pmcurvyaligntospline")]
	public class CurvySplineControllerSetMoveProperties : FsmStateAction
    {
		[RequiredField, Tooltip("The GameObject with curvy Spline Controller")]
		[CheckForComponent(typeof(CurvyController))]
        public FsmOwnerDefault GameObject;

		[Tooltip("Move mode")]
		[ObjectType(typeof(CurvyController.MoveModeEnum))]
		public FsmEnum MoveMode;

        [Tooltip("Move Speed")]
        public FsmFloat Speed;

		[Tooltip("Clamping mode")]
		[ObjectType(typeof(CurvyClamping))]
		public FsmEnum Clamping;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

		CurvyController mCurvyController;
		GameObject go;

        // Code that runs on entering the state.
        public override void OnEnter()
        {
			go = Fsm.GetOwnerDefaultTarget(GameObject);
			if (go)
			{
				mCurvyController = go.GetComponent<CurvyController> ();
			}
				
            if (!everyFrame)
            {
                DoAction();
            }

            if (!everyFrame)
                Finish();
        }

        // Code that runs every frame.
        public override void OnUpdate()
        {
                DoAction();
        }


        void DoAction()
        {
			if (!mCurvyController.IsInitialized)
			{
				return;
			}

			if (!MoveMode.IsNone)
			{
				mCurvyController.MoveMode = (CurvyController.MoveModeEnum)MoveMode.Value;
			}

			if (!Speed.IsNone)
			{
				mCurvyController.Speed = Speed.Value;
			}

			if (!Clamping.IsNone)
			{
				mCurvyController.Clamping = (CurvyClamping)Clamping.Value;
			}
            
        }

        public override void Reset()
        {
            base.Reset();
            GameObject = null;
			Speed = null;
            everyFrame = false;
        }

    }
}