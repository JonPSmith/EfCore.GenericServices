// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace ServiceLayer.HomeController
{
    public class DropdownTuple
    {
        public string Value { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return $"{nameof(Value)}: {Value}, {nameof(Text)}: {Text}";
        }
    }
}