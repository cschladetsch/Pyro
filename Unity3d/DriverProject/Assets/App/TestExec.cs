using System.Collections;
using System.Collections.Generic;
using Diver;
using Diver.Exec;
using Diver.Impl;
using UnityEngine;
using UnityEngine.Assertions;

public class TestExec : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IRegistry _reg = new Registry();
        var code = _reg.Add(new List<object>());
        var coro = _reg.Add(new Continuation(code.Value));

        code.Value.Add(1);
        code.Value.Add(2);
        code.Value.Add(_reg.Add(EOperation.Plus));

        var executor = _reg.Add(new Executor());
        var exec = executor.Value;
        exec.Continue(coro);

        var data = exec.DataStack;
        Assert.AreEqual(1, data.Count);
        Assert.AreEqual(3, data.Pop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
