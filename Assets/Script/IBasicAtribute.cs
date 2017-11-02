using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    public interface IBasicAtribute
    {
        string Name { get; set; }
        int Level { get; set; }
    }
}
