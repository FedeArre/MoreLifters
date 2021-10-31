using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifterMod
{
    class InvalidVersionPopup : MonoBehaviour
    {
        Rect popupLocation;
        bool showMessage;
        string boxMessage = "WARNING! You are using an invalid version for the MoreLifters mod. The lastest testing version is recommendated. Do not report any issues from this version!";

        void Start()
        {
            int width = (Screen.width - 300) / 2;
            int height = (Screen.height - 100) / 2;
            popupLocation = new Rect(width, height, 300, 100);
            showMessage = true;
        }

        void OnGUI()
        {
            if (showMessage)
            {
                GUI.Box(popupLocation, "");
                GUI.Label(popupLocation, boxMessage);
            }
        }
    }
}
