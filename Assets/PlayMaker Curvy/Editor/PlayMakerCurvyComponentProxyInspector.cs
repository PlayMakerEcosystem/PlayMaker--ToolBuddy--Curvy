// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using HutongGames.PlayMaker;

using System.Collections;
using FluffyUnderware.Curvy.PlayMaker;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.Curvy.Controllers;

namespace FluffyUnderware.CurvyEditor.PlayMaker
{
    [CustomEditor(typeof(PlayMakerCurvyComponentProxy))]
    public class PlayMakerCurvyComponentProxyInspector : Editor
    {


        private PlayMakerCurvyComponentProxy _target;

        

        public override void OnInspectorGUI()
        {

            _target = (PlayMakerCurvyComponentProxy)target;

            // let the user select the target for the Curvy component
            _target.CurvyTargetOption = (OwnerDefaultOption)EditorGUILayout.EnumPopup("Target", _target.CurvyTargetOption);

            if (_target.CurvyTargetOption == OwnerDefaultOption.SpecifyGameObject)
            {
                EditorGUI.indentLevel++;
                _target.CurvyTarget = (GameObject)EditorGUILayout.ObjectField("Curvy Target", _target.CurvyTarget, typeof(GameObject), true);
                EditorGUI.indentLevel--;
            }
            else
            {
                _target.CurvyTarget = _target.gameObject;
            }

            ContextGUI();

        }

        void ContextGUI()
        {

            if (_target.CurvyTarget == null)
                return;

            if (_target.CurvyTarget.GetComponent<CurvySpline>() != null)
                splineEventsGUI();
            
            if (_target.CurvyTarget.GetComponent<CurvyGenerator>() != null)
                cgEventsGUI();
            if (_target.CurvyTarget.GetComponent<SplineController>() != null)
                controllerEventsGUI();
        }

        void splineEventsGUI()
        {
            EventTargetGUI(ref _target.SplineOnRefresh,"On Refresh");
            
        }

        void cgEventsGUI()
        {
            EventTargetGUI(ref _target.CGOnRefresh, "On Refresh");
        }

        void controllerEventsGUI()
        {
            EventTargetGUI(ref _target.SplineControllerOnCPReached, "On CP Reached");
        }

        void EventTargetGUI(ref PlayMakerCurvyComponentProxy.FsmEventSetup fsmEventSetup, string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            bool isTargetSetup = EventTargetSetupGUI(ref fsmEventSetup);
            if (!isTargetSetup)
            {
                ErrorFeedbackGui("No target defined");
                return;
            }
            bool isimplemented = _target.DoesTargetImplementsEvent(fsmEventSetup);
            
            GUILayout.BeginHorizontal();
            if (!fsmEventSetup.IsCustomEvent)
            {
                EditorGUILayout.LabelField("Event (built in)", fsmEventSetup.builtInEventName, "WordWrapLabel");

                if (GUILayout.Button("Edit", GUILayout.Width(40), GUILayout.Height(15)))
                {
                    fsmEventSetup.customEventName = fsmEventSetup.builtInEventName;
                }
            }
            else
            {
                fsmEventSetup.customEventName = EditorGUILayout.TextField("Event (Custom)", fsmEventSetup.customEventName, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("X", GUILayout.Width(21), GUILayout.Height(15)))
                {
                    fsmEventSetup.customEventName = "";
                }
            }
            GUILayout.EndHorizontal();
            if (!isimplemented)
            {
                if (fsmEventSetup.target == PlayMakerCurvyComponentProxy.PlayMakerProxyEventTarget.BroadCastAll)
                {
                    ErrorFeedbackGui("There is no such global Event");
                }
                else
                {
                    ErrorFeedbackGui("This Event is not implemented on target");
                }

            }
            fsmEventSetup.debug = EditorGUILayout.Toggle("Debug", fsmEventSetup.debug);
            EditorGUI.indentLevel--;

        }

        bool EventTargetSetupGUI(ref PlayMakerCurvyComponentProxy.FsmEventSetup fsmEventSetup)
        {
            fsmEventSetup.target = (PlayMakerCurvyComponentProxy.PlayMakerProxyEventTarget)EditorGUILayout.EnumPopup("Target", fsmEventSetup.target);

            if (fsmEventSetup.target == PlayMakerCurvyComponentProxy.PlayMakerProxyEventTarget.FsmComponent)
            {
                fsmEventSetup.fsmComponent = (PlayMakerFSM)EditorGUILayout.ObjectField(fsmEventSetup.fsmComponent, typeof(PlayMakerFSM), true);

                return fsmEventSetup.fsmComponent != null;
            }
            else if (fsmEventSetup.target == PlayMakerCurvyComponentProxy.PlayMakerProxyEventTarget.GameObject)
            {
                fsmEventSetup.gameObject = (GameObject)EditorGUILayout.ObjectField(fsmEventSetup.gameObject, typeof(GameObject), true);

                return fsmEventSetup.gameObject != null;
            }
            else if (fsmEventSetup.target == PlayMakerCurvyComponentProxy.PlayMakerProxyEventTarget.Owner)
            {
                return true;
            }
            else if (fsmEventSetup.target == PlayMakerCurvyComponentProxy.PlayMakerProxyEventTarget.BroadCastAll)
            {
                return true;
            }

            return false;
        }
        
        

        #region Sub ui elements

        void ErrorFeedbackGui(string error)
        {
            GUILayout.Space(5);
            GUILayout.Label(error, "flow node 5", GUILayout.ExpandWidth(true), GUILayout.Height(24));
            GUILayout.Space(5);
        }

        

        #endregion


    }
}