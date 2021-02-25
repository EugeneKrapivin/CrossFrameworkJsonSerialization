﻿using System;
using System.Collections.Generic;

namespace Common
{
    public class Model
    {
        public string Str { get; set; } = Guid.NewGuid().ToString();
        public int Int { get; set; } = 9;

        public InnerModel Inner { get; set; } = new InnerModel();

        //public Type Type { get; set; } = typeof(string);

        public List<string> List { get; set; } = new List<string> {"test"};

        public class InnerModel
        {
            public string Str { get; set; } = Guid.NewGuid().ToString();
            public int Int { get; set; } = 10;
        }
    }
}
