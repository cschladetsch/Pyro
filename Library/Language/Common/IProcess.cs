﻿namespace Diver.Language
{
    public interface IProcess
    {
        bool Failed { get; }
        string Error { get; }
    }
}