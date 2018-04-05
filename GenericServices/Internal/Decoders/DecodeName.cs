// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericServices.Internal.Decoders
{
    internal enum DecodedNameTypes { NoNameGiven, Method, Ctor, AutoMapper}

    internal class DecodeName
    {
        public string Name { get; }
        public DecodedNameTypes NameType { get; }
        public int NumParams { get;  } = -1;  //This means number of parameters was not defined

        public DecodeName(string nameToDecode)
        {
            if (string.IsNullOrEmpty(nameToDecode))
            {
                NameType = DecodedNameTypes.NoNameGiven;
                return;
            }
            Name = nameToDecode.Trim();

            if (Name.Contains(","))
                throw new InvalidOperationException("You should only provide one name, not a comma-delimited list. "+
                                                    $"You string was {Name}.");

            var bracketIndex = Name.IndexOf('(');
            if (bracketIndex > 0)
            {
                var closeIndex = Name.IndexOf(')');
                if (closeIndex < 0)
                    throw new InvalidOperationException("The method/ctor string is of the wrong format. It should be of the form <Name>(<num params>), "+
                                                        $"e.g. MyMethod(4). Yours was {Name}, which isnt correct.");
                NumParams = int.Parse(Name.Substring(bracketIndex + 1, closeIndex - bracketIndex - 1));
                Name = Name.Substring(0, bracketIndex);
            }

            NameType = DecodedNameTypes.Method;
            if (Name.Equals(DecodedNameTypes.Ctor.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                NameType = DecodedNameTypes.Ctor;
                //I do this because the DecodedDto methods use exact match of name. MethodCtorMatch sets the name of a ctor in this way too
                Name = DecodedNameTypes.Ctor.ToString();      
            }
            else if (Name.Equals(CrudValues.UseAutoMapper, StringComparison.InvariantCultureIgnoreCase))
                NameType = DecodedNameTypes.AutoMapper;
        }

        public override string ToString()
        {
            return NameType == DecodedNameTypes.NoNameGiven
                ? null
                : NumParams < 0 ? Name : $"{Name}({NumParams})";
        }
    }
}