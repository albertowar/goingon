namespace GoingOn.Persistence.MemoryDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class PersistenceException : Exception
    {
        public PersistenceException(string message)
            : base(message)
        {
        }
    }
}