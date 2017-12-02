using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class FlowController : MonoBehaviour
    {

        public TextMesh amount;
        public Transform wrapper;

        public GameObject waterLogo;
        public GameObject potatoesLogo;
        public GameObject electricityLogo;
        public GameObject ductTapeLogo;
        public GameObject scrapLogo;

        public void Init(string _resourceName, float _amount, int _offset)
        {
            string amountStr = "";
            if (_amount >= 0)
            {
                amountStr = "+";
            }

            amount.text = amountStr + _amount.ToString("0.0");

            switch(_resourceName)
            {
                case "potatoes":
                    potatoesLogo.SetActive(true);
                    break;
                case "water":
                    waterLogo.SetActive(true);
                    break;
                case "electricity":
                    electricityLogo.SetActive(true);
                    break;
                case "ductTape":
                    ductTapeLogo.SetActive(true);
                    break;
                case "scrap":
                    scrapLogo.SetActive(true);
                    break;
            }

            wrapper.localPosition += new Vector3(0, 0.5f * _offset, 0);
        }
    }

}