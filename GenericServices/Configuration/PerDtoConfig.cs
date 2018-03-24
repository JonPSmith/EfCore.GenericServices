// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace GenericServices.Configuration
{
    [Flags]
    public enum CrudTypes
    {
        None = 0,
        Create = 1,
        ReadOne = 2,
        ReadMany = 4,
        Update = 8,
        Delete = 16,

        AllCrud = Create | ReadOne | ReadMany | Update | Delete,
        AllCrudButList = Create | ReadOne | Update | Delete
    }

    /// <summary>
    /// This provides a per-DTO/ViewModel configuation source. This part is the part that doesn't need Generics
    /// </summary>
    public abstract class PerDtoConfig
    {
        //--------------------------------------------------
        //Control of (CRUD) Create, Read Update and Delete methods


        /// <summary>
        /// This defines what CRUD operations the DTO/ViewModel can do. By default it can be used in any GenericService command,
        /// but sometimes its useful to state that it can't do something - for instance many create/update methods aren't really useful in a ReadMany
        /// </summary>
        public virtual CrudTypes WhatCanThisDtoDo { get; } = CrudTypes.AllCrud;

        /// <summary>
        /// This allows you to specify the exact constructor, static method or AutoMapper to create/fill the entity:
        /// - use constructor: "ctor(n)", where n is the number of parameters the ctor has
        /// - static method: use MethodName, e.g. "CreateBookFactory" (can have num params too, e.g. CreateBookFactory(3))
        /// - AutoMapper: "AutoMapper"
        /// </summary>
        public virtual string CreateMethod { get; } = null;

        /// <summary>
        /// This allows you to specify the exact method or AutoMapper that can be used to update the entity
        /// The options are:
        /// - Method: use MethodName, e.g. "AddReview" (can have num params too, e.g. AddReview(3))
        /// - AutoMapper: "AutoMapper"
        /// </summary>
        public virtual string UpdateMethod { get; } = null;

        //------------------------------------------------------
        //Misc

        /// <summary>
        /// The default SaveChanges doesn't validate any entities written to the database (it assumes the front-end has validated it)
        /// By default the normal SaveChanges (non-validation) method  is used, but that can be overridden globally in the GenericServicesConfig class. 
        /// Setting this to true/false will override everything so that you can set validation on/off just the methods in this DTO.
        /// </summary>
        public virtual bool? UseSaveChangesWithValidation { get; } = null;


    }
}