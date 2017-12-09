using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MarsFrenzy
{
    public class DialogEvent
    {
        public delegate bool TriggerTest();
        public delegate void PostHook();
        public TriggerTest shouldTrigger;
        public PostHook postHook;
        public List<string> parts;
        private bool done = false;

        public DialogEvent(List<string> _parts, TriggerTest _shouldTrigger, PostHook _postHook = null)
        {
            parts = _parts;
            shouldTrigger = _shouldTrigger;
            postHook = _postHook;
        }

        public void StartThis()
        {
            done = true;
        }

        public bool isDone()
        {
            return done;
        }

        public void SetDone(bool _done)
        {
            done = _done;
        }
    }
}