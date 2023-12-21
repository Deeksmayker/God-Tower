using UnityEngine;
using NTC.Global.Cache;
using System;
using System.Collections.Generic;

public class Helper : MonoCache{
    public static Helper Instance;

    private List<MyTask> _tasks = new();
    
    private void Awake(){
        if (Instance && Instance != this){
            Destroy(this);
            return;
        }

        Instance = this;
    }

//@NOWORK C# SHIT
    protected override void Run(){
        for (var i = 0; i < _tasks.Count; i++){
            if (Time.time - _tasks[i].startTime > _tasks[i].waitTime){
                //_tasks[i].action.Invoke();
                //_tasks[i].action = _tasks[i].action2;

                _tasks.RemoveAt(i);
            }
        }
    }

    public  void ChangeBoolAfterTime(ref bool refBool, float time, bool newValue){
        //_tasks.Add(new MyTask(){startTime = Time.time, waitTime = time, ref refBool, action2 = newValue});
    }
}

public struct MyTask{
    public float startTime;
    public float waitTime;
    public bool action;
    public bool action2;
}
