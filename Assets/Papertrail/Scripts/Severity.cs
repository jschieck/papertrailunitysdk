using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Papertrail
{
    public enum Severity
    {
        Emergency = 0,
        Alert = 1,
        Critical = 2,
        Error = 3,
        Warning = 4,
        Notice = 5,
        Informational = 6,
        Debug = 7,
        Off = 99,
    }
}