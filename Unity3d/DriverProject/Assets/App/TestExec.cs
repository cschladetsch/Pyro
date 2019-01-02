using System.Collections.Generic;
using UnityEngine;

using Diver;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;
using UnityEngine.Assertions;

// A simple test that ensures we can call into and use all the
// C#7 features used by Diver.
public class TestExec : MonoBehaviour
{
    private IRegistry _reg;
    private RhoTranslator _rho;
    private PiTranslator _pi;
    private Executor _exec;

    void Awake()
    {
        _reg = new Registry();
        _exec = new Executor();
        _rho = new RhoTranslator(_reg);
        _pi = new PiTranslator(_reg);
    }

    void Start()
    {
        _pi.Translate("1 2 +");
        _exec.Continue(_pi.Continuation);
        Debug.Log(_exec.Pop<int>());

        _rho.Translate("1 + 2");
        _exec.Continue(_rho.Result());
        Debug.Log(_exec.Pop<int>());
    }
}
