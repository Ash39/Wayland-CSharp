using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using WaylandProtocal;

namespace Wayland.Protocal.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            CodeGenerator.Main(new string[] {"../../../", "Generated"});

        }

        
    }
}