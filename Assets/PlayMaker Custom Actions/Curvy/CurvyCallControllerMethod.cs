// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================
using UnityEngine;
using HutongGames.PlayMaker;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Controllers;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;

namespace FluffyUnderware.Curvy.PlayMaker.Actions
{
    [ActionCategory("Curvy")]
    [Tooltip("Send a command to a CurvyController. This is a convenience shortcut for the CallMethod action!")]
    [HelpUrl(AssetInformation.DocsRedirectionBaseUrl + "pmcurvycallcontrollermethod")]
    public class CurvyCallControllerMethod : FsmStateAction
    {
        public enum Methods
        {
            Stop,
            Play,
        }

        [ActionSection("Source")]
        [RequiredField, Tooltip("The Controller")]
        [CheckForComponent(typeof(CurvyController))]
        public FsmOwnerDefault GameObject;
        public Methods Command;

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(GameObject);
            if (go != null)
            {
                var mController = go.GetComponent<CurvyController>();
                if (mController)
                {
                    switch (Command)
                    {
                        case Methods.Stop: mController.Stop(); break;
                        case Methods.Play: mController.Play(); break;
                    }
                }
            }
        }

    }
}