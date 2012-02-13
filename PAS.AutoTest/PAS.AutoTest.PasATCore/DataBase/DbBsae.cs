using System;
using System.Collections.Generic;
using System.Text;

using System.Data;

namespace PAS.AutoTest.PasATCore.DataBase
{
    public class DbBsae
    {
        /// <summary>
        /// Connecting string
        /// </summary>
        public static string ConnectingString = string.Empty;

        protected List<DbRecord> Select(string selStr)
        {
            List<DbRecord> records = new List<DbRecord>();


            return records;
        }

        /// <summary>
        /// check whether a record existed
        /// </summary>
        /// <param name="selStr"></param>
        /// <returns></returns>
        protected bool CheckExistence(string selStr)
        {
            return false;
        }
    }

    public class DbRecord
    {
        private DataRow mDR=null;

        public DbRecord(DataRow dr)
        {
            this.mDR =dr;
        }
       
        /// <summary>
        /// Determine whether this record is empty or not.
        /// </summary>
        public bool HasValue
        {
            get{ return (this.mDR ==null? false:true);}
        }

        /// <summary>
        /// Coloumn count.
        /// </summary>
        public int FiledCount
        {
            get { return this.mDR.ItemArray.Length; }
        }

        /// <summary>
        /// get cell value.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object GetCellValue(string columnName)
        {
            return this.mDR[columnName];
        }

        /// <summary>
        /// get cell value.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetCellValue(int index)
        {
            return this.mDR[index];
        }

        /// <summary>
        /// compare this to a xmlparamenter
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(XMLParameter))
            {
                //compare a dbrecord and a xmlparameter
            }
            return true ;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
