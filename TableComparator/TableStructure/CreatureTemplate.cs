﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TableComparator.TableStructure
{
    public class Creature
    {
        public uint entry;
        public float speed_run;
        public float speed_walk;
        public uint faction;
        public uint dynamicFlags;
        public uint attackPower;
        public uint rangeAttackPower;
        public uint dmgMultiplier;
        public uint baseAttackTime;
        public uint rangeAttackTime;
        public uint unit_flag;
        public uint unit_class;
        public float minDamage;
        public float maxDamage;
        public float scale;
        public uint vehicleId;
    };
}