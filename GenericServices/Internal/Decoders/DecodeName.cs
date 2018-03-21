// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericServices.Internal.Decoders
{
    internal enum NameTypes { Method, Ctor, AutoMapper}

    internal class DecodeName
    {
        public string Name { get; private set; }
        public NameTypes NameType { get; private set; }
        public int NumParams { get; private set; } = -1;  //This means number of parameters was not defined

        public DecodeName(string nameToDecode)
        {
            if (nameToDecode.Contains(","))
                throw new InvalidOperationException("In this case we only expect one name, not a comma-delimited list. "+
                                                    $"You string was {nameToDecode}.");

            var bracketIndex = nameToDecode.IndexOf('(');
            if (bracketIndex > 0)
            {
                var closeIndex = nameToDecode.IndexOf(')');
                if (closeIndex < 0)
                    throw new InvalidOperationException("The method/ctor string is of the wrong format. It should be of the form <Name>(<num params>), "+
                                                        $"e.g. MyMethod(4). Yours was {nameToDecode}, which isnt correct.");
                NumParams = int.Parse(nameToDecode.Substring(bracketIndex + 1, closeIndex - bracketIndex - 1));
                nameToDecode = nameToDecode.Substring(0, bracketIndex);
            }

            Name = nameToDecode.Trim();
            NameType = NameTypes.Method;
            if (nameToDecode.Equals(NameTypes.Ctor.ToString(), StringComparison.InvariantCultureIgnoreCase))
                NameType = NameTypes.Ctor;
            else if (nameToDecode.Equals(NameTypes.AutoMapper.ToString(), StringComparison.InvariantCultureIgnoreCase))
                NameType = NameTypes.AutoMapper;
        }

        public override string ToString()
        {
            return NumParams < 0 ? Name : $"{Name}({NumParams})";
        }

        /// <summary>
        /// This handles a comma delimited list of names
        /// </summary>
        /// <param name="namesToDecode"></param>
        /// <returns></returns>
        public static IEnumerable<DecodeName> ProcessPossibleListOfNames(string namesToDecode)
        {
            return namesToDecode.Split(',').Select(x => new DecodeName(x.Trim()));
        }
    }
}