using System.Collections.Generic;
using UnityEngine;

using Diver;
using Diver.Exec;
using UnityEngine.Assertions;

// A simple test that ensures we can call into and use all the
// C#7 features used by Diver.
public class TestExec : MonoBehaviour
{
    void Start()
    {
        IRegistry reg = new Diver.Impl.Registry();
        var code = reg.Add(new List<object>());
        var coro = reg.Add(new Continuation(code.Value));

        code.Value.Add(1);
        code.Value.Add(2);
        code.Value.Add(reg.Add(EOperation.Plus));

        var executor = reg.Add(new Executor());
        var exec = executor.Value;
        exec.Continue(coro);

        var data = exec.DataStack;
        Assert.AreEqual(1, data.Count);
        Assert.AreEqual(3, data.Pop());

        Debug.Log("1 + 2 = 3");
    }
}
