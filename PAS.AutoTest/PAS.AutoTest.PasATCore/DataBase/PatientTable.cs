using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore.DataBase
{
    public class PatientTable:DbBsae 
    {
        /// <summary>
        /// get all patients
        /// </summary>
        /// <returns></returns>
        public List<DbRecord> GetAllPatient()
        {
            return this.Select("Select * from patient");
        }

        /// <summary>
        /// check whether the specified patient exsits.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsPatientExist(string id)
        {
            return this.CheckExistence("Select * from patient where pid='"+id+"'");
        }

        /// <summary>
        /// retreive a single patient.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DbRecord GetPatient(string id)
        {
            List<DbRecord> result = this.Select("Select * from patient where pid="+id+"'");

            if (result.Count != 0)
                return result[0];
            else
                return null;
        }
    }
}
