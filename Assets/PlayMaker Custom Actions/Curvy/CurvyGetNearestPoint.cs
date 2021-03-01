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
    [Tooltip("Get data from a spline point nearest to a given point")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvygetnearestpoint")]
    public class CurvyGetNearestPoint : FsmStateAction
    {
        [RequiredField, Tooltip("The Spline or SplineGroup to address")]
        [CheckForComponent(typeof(CurvySpline))]
        public FsmOwnerDefault GameObject;

        [RequiredField, Tooltip("The known point in space")]
        public FsmVector3 SourcePoint;
        public Space SourceSpace = Space.World;
        [UIHint(UIHint.Variable), Tooltip("Store TF")]
        public FsmFloat StoreTF;
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated position")]
        public FsmVector3 StorePosition;
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated tangent")]
        public FsmVector3 StoreTangent;
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated Up-Vector")]
        public FsmVector3 StoreUpVector;
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated Rotation")]
        public FsmQuaternion StoreRotation;
        public Space Space;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        [Tooltip("Perform in LateUpdate.")]
        public bool lateUpdate;

        CurvySpline mSpline;

		public override void OnPreprocess()
		{
			#if PLAYMAKER_1_8_5_OR_NEWER
			if (lateUpdate)
			{
				Fsm.HandleLateUpdate = true;
			}
			#endif
		}


        // Code that runs on entering the state.
        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go)
            {
                mSpline = go.GetComponent<CurvySpline>();
                if (mSpline && !everyFrame && !lateUpdate)
                {
                    DoFindPoint();
                }
            }
            if (!everyFrame)
                Finish();
        }

        // Code that runs every frame.
        public override void OnUpdate()
        {
            if (mSpline && !lateUpdate)
                DoFindPoint();
        }

        public override void OnLateUpdate()
        {
            if (mSpline && lateUpdate)
                DoFindPoint();
            if (!everyFrame)
                Finish();
        }

        void DoFindPoint()
        {
            if (!mSpline.IsInitialized || SourcePoint.IsNone) return;

            if (StoreTF.IsNone && StorePosition.IsNone && StoreUpVector.IsNone && StoreRotation.IsNone && StoreTangent.IsNone) return;

            Vector3 pos = (Space == Space.Self) ? SourcePoint.Value : mSpline.transform.InverseTransformPoint(SourcePoint.Value);

            float _tf = mSpline.GetNearestPointTF(pos);

            if (StoreTF.UseVariable)
                StoreTF.Value = _tf;

            if (StorePosition.UseVariable)
            {
                StorePosition.Value = (Space == Space.Self) ? mSpline.Interpolate(_tf) : mSpline.transform.TransformPoint(mSpline.Interpolate(_tf));
            }

            if (StoreTangent.UseVariable)
                StoreTangent.Value = (Space==Space.Self) ? mSpline.GetTangent(_tf) : mSpline.transform.TransformDirection(mSpline.GetTangent(_tf));

            if (StoreUpVector.UseVariable)
            {
                StoreUpVector.Value = (Space==Space.Self) ? mSpline.GetOrientationUpFast(_tf) : mSpline.transform.TransformDirection(mSpline.GetOrientationUpFast(_tf));
            }
            if (StoreRotation.UseVariable)
            {
                if (Space == Space.Self)
                    StoreRotation.Value = (StoreUpVector.IsNone) ? mSpline.GetOrientationFast(_tf) : Quaternion.LookRotation(mSpline.GetTangent(_tf), StoreUpVector.Value);
                else
                {
                    StoreRotation.Value = Quaternion.LookRotation(mSpline.transform.TransformDirection(mSpline.GetTangent(_tf)), mSpline.transform.TransformDirection(mSpline.GetOrientationUpFast(_tf)));
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            GameObject = null;
            SourcePoint = new FsmVector3() { UseVariable = true };
            StoreTF = new FsmFloat() { UseVariable = true };
            StorePosition = new FsmVector3() { UseVariable = true };
            StoreTangent = new FsmVector3() { UseVariable = true };
            StoreUpVector = new FsmVector3() { UseVariable = true };
            StoreRotation = new FsmQuaternion() { UseVariable = true };
            everyFrame = false;
            lateUpdate = false;
        }

    }
}