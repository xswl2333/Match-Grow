

using DG.Tweening;
using QFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;


public struct TimeTask
{
    public int id;
    public float taskTime;
    public Action onCompleted; // 完成回调事件
}


public class TimeTaskSystem : MonoSingleton<TimeTaskSystem>
{
    private Dictionary<int, TimeTask> timers = new Dictionary<int, TimeTask>();

    public void AddTimeTask(TimeTask task)
    {
        int id = task.id;
        timers[id] = task;
        ActionKit.Delay(task.taskTime, () => {
            task.onCompleted.Invoke();   
            this.StopTimeById(id);
        }    
        ).Start(this);
    }

    public void StopTimeById(int id)
    {
        if (timers.ContainsKey(id))
        {
            timers.Remove(id);
        }
    }



}