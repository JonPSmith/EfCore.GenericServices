using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenericServices.Setup
{
    /// <summary>
    /// ICreateNewDBContext
    /// </summary>
    public interface ICreateNewDBContext
    {
        /// <summary>
        /// CreateNew
        /// </summary>
        /// <returns></returns>
        DbContext CreateNew();

    }
}
