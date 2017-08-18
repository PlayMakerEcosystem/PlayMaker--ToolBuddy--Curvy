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
    [Tooltip("Retrieve or convert several spline values")]
    [HelpUrl(CurvySpline.DOCLINK + "pmcurvygetvalue")]
    public class CurvyGetValue : FsmStateAction
    {
        [ActionSection("Input")]
        [RequiredField, Tooltip("The Spline or SplineGroup to address")]
        [CheckForComponent(typeof(CurvySplineBase))]
        public FsmOwnerDefault GameObject;
        [RequiredField, Tooltip("Input value (TF or Distance)")]
        public FsmFloat Input;
        [Tooltip("Whether Input value is in World Units (Distance) or TF")]
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
        [UIHint(UIHint.Variable), Tooltip("Store the Segment")]
        public FsmGameObject StoreSegment;
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
        [UIHint(UIHint.Variable), Tooltip("Store the total spline length")]
        public FsmFloat StoreLength;
        [UIHint(UIHint.Variable), Tooltip("Store the number of segments or splines (depending on input GameObject)")]
        public FsmInt StoreCount;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        [Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        CurvySplineBase mSpline;

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
                mSpline = go.GetComponent<CurvySplineBase>();
                if (mSpline && !everyFrame && !lateUpdate)
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
            if (mSpline && !lateUpdate)
                DoInterpolate();
        }

        public override void OnLateUpdate()
        {
            if (mSpline && lateUpdate)
                DoInterpolate();
            if (!everyFrame)
                Finish();
        }

        void DoInterpolate()
        {
            if (!mSpline.IsInitialized) return;
            System.Type metaType = System.Type.GetType(MetaDataType.Value);
            bool calc = !Input.IsNone;
            if (calc)
            {
                float f = (UseWorldUnits.Value) ? mSpline.DistanceToTF(Input.Value) : Input.Value;

                if (StorePosition.UseVariable)
                    StorePosition.Value = (UseCache.Value) ? mSpline.InterpolateFast(f) : mSpline.Interpolate(f);

                if (StoreTangent.UseVariable)
                    StoreTangent.Value = mSpline.GetTangent(f);

                if (StoreUpVector.UseVariable)
                    StoreUpVector.Value = mSpline.GetOrientationUpFast(f);

                if (StoreRotation.UseVariable)
                    StoreRotation.Value = (StoreUpVector.IsNone) ? mSpline.GetOrientationFast(f) : Quaternion.LookRotation(mSpline.GetTangent(f), StoreUpVector.Value);

                if (StoreScale.UseVariable)
                    StoreScale.Value = mSpline.InterpolateScale(f);

                if (StoreTF.UseVariable)
                    StoreTF.Value = f;

                if (StoreDistance.UseVariable)
                    StoreDistance.Value = (UseWorldUnits.Value) ? Input.Value : mSpline.TFToDistance(f);
                if (metaType != null)
                {
                    if (StoreMetadata.UseVariable)
                        StoreMetadata.Value = mSpline.GetMetadata(metaType, f);
                    if (StoreInterpolatedMetadata.useVariable)
                        StoreInterpolatedMetadata.SetValue(mSpline.InterpolateMetadata(metaType, f));
                }


                CurvySplineSegment seg = null;
                float segF = 0;
                if (StoreSegment.UseVariable)
                {
                    seg = getSegment(f, out segF);
                    StoreSegment.Value = seg.gameObject;
                }

                if (StoreSegmentF.UseVariable)
                {
                    if (!seg)
                        seg = getSegment(f, out segF);
                    StoreSegmentF.Value = segF;
                }

                if (StoreSegmentDistance.UseVariable)
                {
                    if (!seg)
                        seg = getSegment(f, out segF);
                    StoreSegmentDistance.Value = seg.LocalFToDistance(segF);
                }
            }
            // General
            if (StoreLength.UseVariable)
                StoreLength.Value = mSpline.Length;

            if (StoreCount.UseVariable)
            {
                StoreCount.Value = (mSpline is CurvySplineGroup) ? ((CurvySplineGroup)mSpline).Count : ((CurvySpline)mSpline).Count;
            }
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
            StoreSegment = new FsmGameObject() { UseVariable = true };
            StoreSegmentDistance = new FsmFloat() { UseVariable = true };
            StoreSegmentF = new FsmFloat() { UseVariable = true };
            StoreLength = new FsmFloat() { UseVariable = true };
            StoreCount = new FsmInt() { UseVariable = true };
            MetaDataType = new FsmString();
            StoreMetadata = new FsmObject() { UseVariable = true };
            StoreInterpolatedMetadata = new FsmVar() { useVariable = true };
            Input = 0;
            everyFrame = false;
            lateUpdate = false;
        }

        CurvySplineSegment getSegment(float tf, out float localF)
        {
            if (mSpline is CurvySplineGroup)
            {
                float ltf;
                CurvySpline spl = ((CurvySplineGroup)mSpline).TFToSpline(tf, out ltf);
                return spl.TFToSegment(ltf, out localF);
            }
            else
                return ((CurvySpline)mSpline).TFToSegment(tf, out localF);
        }
    }
}