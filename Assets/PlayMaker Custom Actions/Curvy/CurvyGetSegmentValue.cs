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

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Retrieve or convert several segment values")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvygetsegmentvalue")]
    public class CurvyGetSegmentValue : FsmStateAction
    {
        [RequiredField, Tooltip("The Spline Segment to address")]
        [CheckForComponent(typeof(CurvySplineSegment))]
        public FsmOwnerDefault GameObject;
        [ActionSection("Input")]
        [RequiredField, Tooltip("Input value (F or Distance)")]
        public FsmFloat Input;
        [Tooltip("Whether Input value is in World Units (Distance) or F")]
        public FsmBool UseWorldUnits;
        [Tooltip("Use a linear approximation (slightly faster) for position?")]
        public FsmBool UseCache;
        [ActionSection("Based on Input")]
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated position")]
        public FsmVector3 StorePosition;
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated tangent")]
        public FsmVector3 StoreTangent;
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated Up-Vector")]
        public FsmVector3 StoreUpVector;
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated Rotation")]
        public FsmQuaternion StoreRotation;
        [Tooltip("Index of the UserValue you're interested in")]
        public FsmInt UserValueIndex;
        [UIHint(UIHint.Variable), Tooltip("Store the interpolated Scale")]
        public FsmVector3 StoreScale;
        [UIHint(UIHint.Variable), Tooltip("Store the TF")]
        public FsmFloat StoreTF;
        [UIHint(UIHint.Variable), Tooltip("Store the Distance")]
        public FsmFloat StoreDistance;
        [UIHint(UIHint.Variable), Tooltip("Store the local F")]
        public FsmFloat StoreSegmentF;
        [UIHint(UIHint.Variable), Tooltip("Store the local Distance")]
        public FsmFloat StoreSegmentDistance;
        [ActionSection("MetaData")]
        public FsmString MetaDataType;
        public FsmObject StoreMetadata;
        [Title("Store Interpolated")]
        public FsmVar StoreInterpolatedMetadata;
        [ActionSection("General")]
        [UIHint(UIHint.Variable), Tooltip("Store the Segment length")]
        public FsmFloat StoreLength;
        [UIHint(UIHint.Variable), Tooltip("Store the SegmentIndex")]
        public FsmInt StoreSegmentIndex;
        [UIHint(UIHint.Variable), Tooltip("Store the ControlPointIndex")]
        public FsmInt StoreControlPointIndex;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        [Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        CurvySplineSegment mSegment;

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
                mSegment = go.GetComponent<CurvySplineSegment>();
                if (mSegment && !everyFrame && !lateUpdate)
                {
                    DoInterpolate();
                }
            }
            if (!everyFrame)
                Finish();
        }

        // Code that runs every frame.
        public override void OnUpdate()
        {
            if (mSegment && !lateUpdate)
                DoInterpolate();
        }

        public override void OnLateUpdate()
        {
            if (mSegment && lateUpdate)
                DoInterpolate();
            if (!everyFrame)
                Finish();
        }

        void DoInterpolate()
        {
            if (!mSegment.Spline.IsInitialized) return;
            bool calc = !Input.IsNone;
            if (calc)
            {
                System.Type metaType = System.Type.GetType(MetaDataType.Value);
                float inputF = (UseWorldUnits.Value) ? mSegment.DistanceToLocalF(Input.Value) : Input.Value;

                if (StorePosition.UseVariable)
                    StorePosition.Value = (UseCache.Value) ? mSegment.InterpolateFast(inputF) : mSegment.Interpolate(inputF);

                if (StoreTangent.UseVariable)
                    StoreTangent.Value = mSegment.GetTangent(inputF);

                if (StoreUpVector.UseVariable)
                    StoreUpVector.Value = mSegment.GetOrientationUpFast(inputF);

                if (StoreRotation.UseVariable)
                    StoreRotation.Value = (StoreUpVector.IsNone) ? mSegment.GetOrientationFast(inputF) : Quaternion.LookRotation(mSegment.GetTangent(inputF), StoreUpVector.Value);

                if (StoreScale.UseVariable)
                    StoreScale.Value = mSegment.InterpolateScale(inputF);

                if (StoreTF.UseVariable)
                    StoreTF.Value = mSegment.LocalFToTF(inputF);

                if (StoreSegmentDistance.UseVariable)
                    StoreSegmentDistance.Value = mSegment.LocalFToDistance(inputF);

                if (StoreDistance.UseVariable)
                    StoreDistance.Value = (StoreSegmentDistance.UseVariable) ? StoreSegmentDistance.Value + mSegment.Distance : mSegment.LocalFToDistance(inputF) + mSegment.Distance;

                if (StoreSegmentF.UseVariable)
                    StoreSegmentF.Value = inputF;

                if (metaType != null)
                {
                    if (StoreMetadata.UseVariable)
                        StoreMetadata.Value = mSegment.GetMetaData(metaType);
                    if (StoreInterpolatedMetadata.useVariable)
                        StoreInterpolatedMetadata.SetValue(mSegment.InterpolateMetadata(metaType, inputF));
                }

            }
            // General
            if (StoreLength.UseVariable)
                StoreLength.Value = mSegment.Length;

            if (StoreSegmentIndex.UseVariable)
                StoreSegmentIndex.Value = mSegment.Spline.GetSegmentIndex(mSegment);
            if (StoreControlPointIndex.UseVariable)
                StoreControlPointIndex.Value = mSegment.Spline.GetControlPointIndex(mSegment);
        }

        public override void Reset()
        {
            base.Reset();
            GameObject = null;
            UseCache = false;
            UseWorldUnits = false;
            StorePosition = new FsmVector3() { UseVariable = true };
            StoreUpVector = new FsmVector3() { UseVariable = true };
            StoreRotation = new FsmQuaternion() { UseVariable = true };
            UserValueIndex = new FsmInt() { UseVariable = true };
            StoreTangent = new FsmVector3() { UseVariable = true };
            StoreDistance = new FsmFloat() { UseVariable = true };
            StoreTF = new FsmFloat() { UseVariable = true };
            StoreSegmentIndex = new FsmInt() { UseVariable = true };
            StoreControlPointIndex = new FsmInt() { UseVariable = true };
            StoreSegmentDistance = new FsmFloat() { UseVariable = true };
            StoreSegmentF = new FsmFloat() { UseVariable = true };
            StoreMetadata = new FsmObject() { UseVariable = true };
            StoreInterpolatedMetadata = new FsmVar() { useVariable = true };
            Input = 0;
            everyFrame = false;
            lateUpdate = false;
        }

    }
}