using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestMod
{
    public class GUITest : MonoBehaviour
    {
        float t = 0f;
        public string testy = "This is a test run";

        void Update()
        {
            if (t > 500f)
                t = 20f;
            Loadingbarlook();
        }
        void Loadingbarlook()
        {
            t += Time.deltaTime * 100f;
        }

        public void OnGUI()
        {
            GUILayout.Box(testy, GUILayout.MaxWidth(t), GUILayout.Height(20));

        }
    }
}
