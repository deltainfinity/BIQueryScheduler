using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIQueryScheduler.API.Models
{ 
    public class BIDataBase
    {
        /// <summary>
        /// BI database connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Stored procedures
        /// </summary>
        public StoredProcedures StoredProcedures { get; set; }
    }

    public class StoredProcedures
    {
        /// <summary>
        /// Stored procedure one
        /// </summary>
        public string SP1 { get; set; }

        /// <summary>
        /// Stored procedure two
        /// </summary>
        public string SP2 { get; set; }

        /// <summary>
        /// Stored procedure three
        /// </summary>
        public string SP3 { get; set; }

        /// <summary>
        /// Stored procedure four
        /// </summary>
        public string SP4 { get; set; }

        /// <summary>
        /// Stored procedure five
        /// </summary>
        public string SP5 { get; set; }

        /// <summary>
        /// Stored procedure six
        /// </summary>
        public string SP6 { get; set; }

        /// <summary>
        /// Stored procedure seven
        /// </summary>
        public string SP7 { get; set; }

        /// <summary>
        /// Stored procedure eight
        /// </summary>
        public string SP8 { get; set; }

        /// <summary>
        /// Stored procedure nine
        /// </summary>
        public string SP9 { get; set; }
    }
}
