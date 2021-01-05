// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using ServiceLayer.HomeController.Dtos;
using StatusGeneric;

namespace ServiceLayer.HomeController
{
    public interface IChangePubDateService : IStatusGeneric
    {
        ChangePubDateDto GetOriginal(int id);
        Book UpdateBook(int id, ChangePubDateDto dto);
    }
}