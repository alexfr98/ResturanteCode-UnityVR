using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;

public abstract class Observer
{
    public abstract void OnNotify(string name);
}

public class LogrosController : Observer
{
    public LogrosController()
    {


    }

    public override void OnNotify(string name)
    {

    }

   
}
