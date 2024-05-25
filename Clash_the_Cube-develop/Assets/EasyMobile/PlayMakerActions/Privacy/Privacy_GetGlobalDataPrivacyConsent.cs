﻿#if PLAYMAKER
using UnityEngine;
using System;
using System.Collections;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EasyMobile.PlayerMaker.Actions
{
    [ActionCategory("Easy Mobile - Privacy")]
    [Tooltip("Gets the global data privacy consent status.")]
    public class Privacy_GetGlobalDataPrivacyConsent : FsmStateAction
    {
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        [ActionSection("Result")]

        [Tooltip("The global data privacy consent.")]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(ConsentStatus))]
        public FsmEnum consentStatus;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Event sent if the consent is granted.")]
        public FsmEvent isConsentGrantedEvent;

        [Tooltip("Event sent if the consent is revoked.")]
        public FsmEvent isConsentRevokedEvent;

        [Tooltip("Event sent if the consent is unknown.")]
        public FsmEvent isConsentUnknownEvent;

        public override void Reset()
        {
            consentStatus = null;
            eventTarget = null;
            isConsentGrantedEvent = null;
            isConsentRevokedEvent = null;
            isConsentUnknownEvent = null; 
        }

        public override void OnEnter()
        {
            DoMyAction();

            if (!everyFrame)
                Finish();
        }

        public override void OnUpdate()
        {
            DoMyAction();   
        }

        void DoMyAction()
        {
            consentStatus.Value = Privacy.GlobalDataPrivacyConsent;

            switch (Privacy.GlobalDataPrivacyConsent)
            {
                case ConsentStatus.Granted:
                    Fsm.Event(eventTarget, isConsentGrantedEvent);
                    break;
                case ConsentStatus.Revoked:
                    Fsm.Event(eventTarget, isConsentRevokedEvent);
                    break;
                case ConsentStatus.Unknown:
                default:
                    Fsm.Event(eventTarget, isConsentUnknownEvent);
                    break;
            }
        }
    }
}
#endif

