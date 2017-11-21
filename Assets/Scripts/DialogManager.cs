using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class DialogManager : MonoBehaviour
    {
        protected static DialogManager instance;
        public GameObject window;
        public Text text;
        private int idx = 0;
        private List<string> parts;
        private bool active = false;
        private List<DialogEvent> events;
        private DialogEvent currentDialog;

        // Use this for initialization
        void Start()
        {
            instance = this;
            window.SetActive(false);
            idx = 0;
            active = false;
            events = Dialogs.InitDialogs();
        }

        // Update is called once per frame
        void Update()
        {
            if (active && Input.GetMouseButtonDown(0))
            {
                Next();
            }
            if (!active)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    DialogEvent ev = events[i];
                    if (!ev.isDone() && ev.shouldTrigger())
                    {
                        StartDialog(ev);
                        break;
                    }
                }
            }
        }

        public void StartDialog(DialogEvent _de)
        {
            Debug.Log("Start Dialog");
            _de.StartThis();
            currentDialog = _de;
            parts = _de.parts;
            events.Remove(_de);
            GameManager.Instance.Pause();
            window.SetActive(true);
            idx = -1;
            active = true;
            Next();
        }

        public void Next()
        {
            idx++;
            if (idx >= parts.Count)
            {
                EndDialog();
                return;
            }
            text.text = parts[idx];
        }

        public void EndDialog()
        {
            window.SetActive(false);
            idx = 0;
            parts = null;
            active = false;
            if (currentDialog.postHook != null)
            {
                currentDialog.postHook();
            }
            GameManager.Instance.EndDialog();
        }

        public bool IsActive()
        {
            return active;
        }

        public static DialogManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
}