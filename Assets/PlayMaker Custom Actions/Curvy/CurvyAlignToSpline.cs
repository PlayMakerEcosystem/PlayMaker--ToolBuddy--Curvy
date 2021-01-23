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
using FluffyUnderware.Curvy.Utils;

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Align a GameObject to a Curvy Spline ")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvyaligntospline")]
    public class CurvyAlignToSpline : FsmStateAction
    {
        [RequiredField, Tooltip("The GameObject to align")]
        public FsmOwnerDefault GameObject;

        [RequiredField, Tooltip("The Spline or SplineGroup to address")]
        [CheckForComponent(typeof(CurvySpline))]
        public FsmGameObject Spline;

        [RequiredField, Tooltip("Position on spline (TF or distance)")]
        public FsmFloat Position;
        public CurvyPositionMode PositionMode;
        public CurvyClamping Clamping;
        public FsmBool UseCache;

        [Tooltip("Set Orientation?")]
        public bool SetOrientation;

        public Space Space;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public PlayMakerActionsUtils.EveryFrameUpdateSelector updateType;

        CurvySpline mSpline;


		public override void OnPreprocess()
        {
            if (updateType == PlayMakerActionsUtils.EveryFrameUpdateSelector.OnFixedUpdate)
            {
                Fsm.HandleFixedUpdate = true;
            }
			#if PLAYMAKER_1_8_5_OR_NEWER
			if (updateType == PlayMakerActionsUtils.EveryFrameUpdateSelector.OnLateUpdate)
			{
				Fsm.HandleLateUpdate = true;
			}
			#endif
        }

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            if (!Spline.IsNone)
                mSpline = Spline.Value.GetComponent<CurvySpline>();

            if (mSpline && !everyFrame)
            {
                DoInterpolate();
            }
            if (!everyFrame)
                Finish();
        }

        // Code that runs every frame.
        public override void OnUpdate()
        {
            if (mSpline && updateType == PlayMakerActionsUtils.EveryFrameUpdateSelector.OnUpdate)
                DoInterpolate();
        }

        public override void OnLateUpdate()
        {
            if (mSpline && updateType == PlayMakerActionsUtils.EveryFrameUpdateSelector.OnLateUpdate)
                DoInterpolate();
        }

        public override void OnFixedUpdate()
        {
            if (mSpline && updateType == PlayMakerActionsUtils.EveryFrameUpdateSelector.OnFixedUpdate)
                DoInterpolate();
        }

        void DoInterpolate()
        {
            if (!mSpline.IsInitialized) return;
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go)
            {
			
                float tf = (PositionMode == CurvyPositionMode.Relative) ? CurvyUtility.ClampTF(Position.Value, Clamping) : mSpline.DistanceToTF(Position.Value, Clamping);
                Vector3 p = (UseCache.Value) ? mSpline.InterpolateFast(tf) : mSpline.Interpolate(tf);

                if (Space == Space.Self)
                {
                    go.transform.localPosition = p;

                    if (SetOrientation)
                        go.transform.localRotation = mSpline.GetOrientationFast(tf);
                }
                else
                {
                    go.transform.position = mSpline.transform.TransformPoint(p);
                    
                    if (SetOrientation)
                        go.transform.rotation = mSpline.transform.rotation*mSpline.GetOrientationFast(tf);
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            GameObject = null;
            Spline = null;
            SetOrientation = true;
            UseCache = false;
            Position = 0;
            Space = Space.World;
            PositionMode = CurvyPositionMode.Relative;
            Clamping = CurvyClamping.Clamp;
            everyFrame = false;
            updateType = PlayMakerActionsUtils.EveryFrameUpdateSelector.OnUpdate;
        }

    }
}