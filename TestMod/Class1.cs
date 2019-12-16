using SDG.Framework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestMod
{
    public class Class1 : IModuleNexus
    {
        public GameObject obj;
        public Manager man;


        public void initialize()
        {
            obj = new GameObject("ManagerForThisModWithARandomName");
            obj.AddComponent<Manager>();
            //Debug.Log("On");
        }


        public void shutdown()
        {
            obj = GameObject.Find("ManagerForThisModWithARandomName");
            man = obj.GetComponent<Manager>();

            man.Selfdestroy();
            //Debug.Log((object)"Off");
        }
    }
}
