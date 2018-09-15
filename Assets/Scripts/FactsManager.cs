using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    [System.Serializable]
    public class FactsManager
    {
        private static FactsManager m_instance;
        public static FactsManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        private Dictionary<string, bool> m_switches = new Dictionary<string, bool>();
        private Dictionary<string, float> m_values = new Dictionary<string, float>();

        public void Init()
        {
            m_instance = this;
        }

        public bool ReadSwitch(string _name)
        {
            bool ret = false;
            m_switches.TryGetValue(_name, out ret);
            return ret;
        }

        public void SetSwitch(string _name, bool _value)
        {
            m_switches.Add(_name, _value);
        }



        public float? ReadValue(string _name)
        {
            float ret;
            m_values.TryGetValue(_name, out ret);
            return ret;
        }

        public void SetValue(string _name, float _value)
        {
            m_values.Add(_name, _value);
        }
    }
}