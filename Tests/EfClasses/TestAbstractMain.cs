// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Tests.EfClasses
{
    public class TestAbstractMain : TestAbstractBase
    {
        public TestAbstractMain(string abstractPropPrivate, string abstractPropPublic, string notAbstractPropPublic) 
            : base(abstractPropPrivate, abstractPropPublic)
        {
            NotAbstractPropPublic = notAbstractPropPublic;
        }

        public string NotAbstractPropPublic { get; set; }
        
    }
}