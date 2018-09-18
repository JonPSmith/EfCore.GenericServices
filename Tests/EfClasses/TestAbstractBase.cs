// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Tests.EfClasses
{
    public abstract class TestAbstractBase
    {
        protected TestAbstractBase(string abstractPropPrivate, string abstractPropPublic)
        {
            AbstractPropPrivate = abstractPropPrivate;
            AbstractPropPublic = abstractPropPublic;
        }

        public int Id { get; private set; }

        public string AbstractPropPrivate { get; private set; }

        public string AbstractPropPublic { get; set; }

        public void SetAbstractPropPrivate(string abstractPropPrivate)
        {
            AbstractPropPrivate = abstractPropPrivate;
        }
    }
}