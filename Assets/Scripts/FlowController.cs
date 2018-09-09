using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class FlowController : MonoBehaviour
    {

        public TextMesh amount;

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
                case Constants.POTATO:
                    potatoesLogo.SetActive(true);
                    break;
                case Constants.WATER:
                    waterLogo.SetActive(true);
                    break;
                case Constants.ELECTRICITY:
                    electricityLogo.SetActive(true);
                    break;
                case Constants.DUCTTAPE:
                    ductTapeLogo.SetActive(true);
                    break;
                case Constants.SCRAP:
                    scrapLogo.SetActive(true);
                    break;
            }

            transform.localPosition -= new Vector3(0, 0.7f * _offset, 0);
        }
    }

}