using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestMod
{
    public class Manager : MonoBehaviour
    {

        public GameObject objectspawn1;
        public GameObject manger;



        void Start()
        {
            manger = GameObject.Find("ManagerForThisModWithARandomName");
            DontDestroyOnLoad(this.manger);

            objectspawn1 = new GameObject("Testobject");
            DontDestroyOnLoad(objectspawn1);

            objectspawn1.AddComponent<GUITest>();
            objectspawn1.AddComponent<WorkScript>();

        }


        public void Selfdestroy()
        {
            Destroy(objectspawn1);
            Destroy(manger);
        }
    }
}
