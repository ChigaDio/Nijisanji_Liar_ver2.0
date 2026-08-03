

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace GameCore.Scenario
{
    public abstract class BaseScenarioRoleData
    {
        public ScenarioRoleID RoleID { get; protected set; }

        public abstract void ReadBinary(BinaryReader reader);
    }
}

