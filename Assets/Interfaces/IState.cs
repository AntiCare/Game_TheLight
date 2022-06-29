using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Interfaces
{
    public interface IState
    {
        public States Action(States state);
    }
}

