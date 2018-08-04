// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================
using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.Curvy.Controllers;

namespace FluffyUnderware.Curvy.PlayMaker
{
    [AddComponentMenu("Curvy/PlayMaker/Component Proxy")]
    [HelpUrl(CurvySpline.DOCLINK + "pmplaymakercurvycomponentproxy")]
    public class PlayMakerCurvyComponentProxy : MonoBehaviour
    {
        public enum PlayMakerProxyEventTarget { Owner, GameObject, BroadCastAll, FsmComponent };

        [System.Serializable]
        public struct FsmEventSetup
        {
            public PlayMakerProxyEventTarget target;
            public GameObject gameObject;
            public PlayMakerFSM fsmComponent;
            public string customEventName;
            public string builtInEventName;
            public bool debug;

            public FsmEventSetup(string eventName)
            {
                target=PlayMakerProxyEventTarget.Owner;
                gameObject=null;
                fsmComponent=null;
                customEventName=null;
                builtInEventName=eventName;
                debug = fsmComponent;
            }

            public string EventString
            {
                get
                {
                    return IsCustomEvent ? customEventName : builtInEventName;
                }
            }

            public bool IsCustomEvent
            {
                get
                {
                    return !string.IsNullOrEmpty(customEventName);
                }
            }

            public bool IsSetup
            {
                get
                {
                    switch (target)
                    {
                        case PlayMakerProxyEventTarget.GameObject:
                            return gameObject != null;
                        case PlayMakerProxyEventTarget.FsmComponent:
                            return fsmComponent != null;
                        default: return true;
                    }
                }
            }

        }

        public OwnerDefaultOption CurvyTargetOption;
        public GameObject CurvyTarget;

        // Spline Events
        public FsmEventSetup SplineOnRefresh=new FsmEventSetup("CURVY / ON SPLINE REFRESH");
        FsmEventTarget splineOnRefreshEventTarget;	
        // CG Events
        public FsmEventSetup CGOnRefresh = new FsmEventSetup("CURVY / ON GENERATOR REFRESH");
        FsmEventTarget cgOnRefreshEventTarget;
        // Spline Controller Events
        public FsmEventSetup SplineControllerOnCPReached = new FsmEventSetup("CURVY / ON CP REACHED");
        FsmEventTarget splineControllerOnCPreachedTarget;

        public static CurvySplineMoveEventArgs _OnCPReachedEventData;


        #region ### Unity Callbacks ###

        void Start()
        {
            var spl = CurvyTarget.GetComponent<CurvySpline>();
            if (spl)
                hookSplineEvents(spl);
            var cg = CurvyTarget.GetComponent<CurvyGenerator>();
            if (cg)
                hookCGEvents(cg);
            var ctrl = CurvyTarget.GetComponent<SplineController>();
            if (ctrl)
                hookControllerEvents(ctrl);
        }
        #endregion

        #region ### Helpers ###
        public bool DoesTargetImplementsEvent(FsmEventSetup fsmEventSetup)
        {

            string eventName = fsmEventSetup.EventString;

            if (fsmEventSetup.target == PlayMakerProxyEventTarget.BroadCastAll)
            {
                return FsmEvent.IsEventGlobal(eventName);
            }

            if (fsmEventSetup.target == PlayMakerProxyEventTarget.FsmComponent)
            {
                return PlayMakerUtils.DoesFsmImplementsEvent(fsmEventSetup.fsmComponent, eventName);
            }

            if (fsmEventSetup.target == PlayMakerProxyEventTarget.GameObject)
            {
                return PlayMakerUtils.DoesGameObjectImplementsEvent(fsmEventSetup.gameObject, eventName);
            }

            if (fsmEventSetup.target == PlayMakerProxyEventTarget.Owner)
            {
                return PlayMakerUtils.DoesGameObjectImplementsEvent(this.gameObject, eventName);
            }

            return false;
        }

        void setupEventTarget(FsmEventSetup fsmEventSetup, ref FsmEventTarget fsmEventTarget)
        {
            if (fsmEventTarget == null)
            {
                fsmEventTarget = new FsmEventTarget();
            }

            // BROADCAST
            if (fsmEventSetup.target == PlayMakerProxyEventTarget.BroadCastAll)
            {
                fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
                fsmEventTarget.excludeSelf = false;
            }

            // FSM COMPONENT
            else if (fsmEventSetup.target == PlayMakerProxyEventTarget.FsmComponent)
            {
                fsmEventTarget.target = FsmEventTarget.EventTarget.FSMComponent;
                fsmEventTarget.fsmComponent = fsmEventSetup.fsmComponent;
            }

            // GAMEOBJECT
            else if (fsmEventSetup.target == PlayMakerProxyEventTarget.GameObject)
            {
                fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
                fsmEventTarget.gameObject = new FsmOwnerDefault();
                fsmEventTarget.gameObject.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
                fsmEventTarget.gameObject.GameObject.Value = fsmEventSetup.gameObject;
            }

            // OWNER
            else if (fsmEventSetup.target == PlayMakerProxyEventTarget.Owner)
            {
                fsmEventTarget.ResetParameters();
                fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
                fsmEventTarget.gameObject = new FsmOwnerDefault();
                fsmEventTarget.gameObject.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
                fsmEventTarget.gameObject.GameObject.Value = this.gameObject;

            }

        }

        void FirePlayMakerEvent(FsmEventSetup fsmEventSetup, FsmEventTarget fsmEventTarget, FsmEventData eventData)
        {

            if (eventData != null)
            {
                HutongGames.PlayMaker.Fsm.EventData = eventData;
            }
            fsmEventTarget.excludeSelf = false;
            
            if (PlayMakerCurvySceneProxy.fsm == null)
            {
                Debug.LogError("Missing 'PlayMaker Curvy' prefab in scene");
                return;
            }
            Fsm _fsm = PlayMakerCurvySceneProxy.fsm.Fsm;

            if (fsmEventSetup.debug) Debug.Log("Fire event: " + fsmEventSetup.EventString);
            _fsm.Event(fsmEventTarget, fsmEventSetup.EventString);
        }
        #endregion

        #region ### Spline Events ###

        void hookSplineEvents(CurvySpline spline)
        {
            if (SplineOnRefresh.IsSetup)
            {
                spline.OnRefresh.AddListener(onSplineRefresh);
                setupEventTarget(SplineOnRefresh, ref splineOnRefreshEventTarget);
            }
        }

        void onSplineRefresh(CurvySplineEventArgs e)
        {
            FsmEventData _eventData = new FsmEventData();
            this.FirePlayMakerEvent(SplineOnRefresh, splineOnRefreshEventTarget, _eventData);
        }

        #endregion

        #region ### CG Events ###

        void hookCGEvents(CurvyGenerator cg)
        {
            if (CGOnRefresh.IsSetup)
            {
                cg.OnRefresh.AddListener(onCGRefresh);
                setupEventTarget(CGOnRefresh, ref cgOnRefreshEventTarget);
            }
        }

        void onCGRefresh(CurvyCGEventArgs e)
        {
            FsmEventData _eventData=new FsmEventData();
            this.FirePlayMakerEvent(CGOnRefresh,cgOnRefreshEventTarget,_eventData);
        }

        

        #endregion

        #region ### Controller Events ###

        void hookControllerEvents(SplineController c)
        {
            if (SplineControllerOnCPReached.IsSetup)
            {
                c.OnControlPointReached.AddListener(onControllerCPReached);
                setupEventTarget(SplineControllerOnCPReached, ref splineControllerOnCPreachedTarget);
            }
        }

        void onControllerCPReached(CurvySplineMoveEventArgs e)
        {
            FsmEventData _eventData = new FsmEventData();
            _OnCPReachedEventData = e;
            this.FirePlayMakerEvent(SplineControllerOnCPReached, splineControllerOnCPreachedTarget, _eventData);
            e = _OnCPReachedEventData;
        }

        #endregion

    }
}