// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericServices.Configuration
{
    /// <summary>
    /// This provides a per-DTO/ViewModel configuation source. This part is the part that doesn't need Generics
    /// </summary>
    public abstract class PerDtoConfig
    {
        //--------------------------------------------------
        //Control of (CRUD) Create, Read Update and Delete methods

        /// <summary>
        /// This allows you to specify the exact constructor, static method or AutoMapper that can be used to update the entity
        /// The options are:
        /// - use constructor: "ctor(n)", where n is the number of parameters the ctor has
        /// - static method: use MethodName, e.g. "CreateBookFactory" 
        /// - AutoMapper: "AutoMapper"
        /// If there are multiple options then provide as comma delimited list, e.g. "ctor(4), CreateBookFactory"
        /// </summary>
        public string CreateMethodsCtors { get; } = null;

        /// <summary>
        /// This allows you to specify the exact method or AutoMapper that can be used to update the entity
        /// The options are:
        /// - Method: use MethodName, e.g. "AddReview" 
        /// - AutoMapper: "AutoMapper"
        /// If there are multiple options then provide as comma delimited list, e.g. "AddPromotion, RemovePromotion"
        /// </summary>
        public string UpdateMethods { get; } = null;

        //------------------------------------------------------
        //Misc

        ///// <summary>
        ///// The default SaveChanges doesn't validate any entities written to the database (it assumes the front-end has validated it)
        ///// By default the normal SaveChanges (non-validation) method  is used, but that can be overridden globally in the GenericServicesConfig class. 
        ///// Setting this to true/false will override everything so that you can set validation on/off just the methods in this DTO.
        ///// </summary>
        //public virtual bool? UseSaveChangesWithValidation { get; } = null;


    }
}