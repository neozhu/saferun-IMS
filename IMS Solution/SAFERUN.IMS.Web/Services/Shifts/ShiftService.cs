﻿
             
           
 

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;

using SAFERUN.IMS.Web.Models;
using SAFERUN.IMS.Web.Repositories;

using System.Data;
using System.Reflection;
namespace SAFERUN.IMS.Web.Services
{
    public class ShiftService : Service< Shift >, IShiftService
    {

        private readonly IRepositoryAsync<Shift> _repository;
		 private readonly IDataTableImportMappingService _mappingservice;
        public  ShiftService(IRepositoryAsync< Shift> repository,IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository=repository;
			_mappingservice = mappingservice;
        }
        
                  
        

		public void ImportDataTable(System.Data.DataTable datatable)
        {
            foreach (DataRow row in datatable.Rows)
            {
                 
                Shift item = new Shift();
                foreach (DataColumn col in datatable.Columns)
                {
                    var sourcefieldname = col.ColumnName;
                    var mapping = _mappingservice.FindMapping("Shift", sourcefieldname);
                    if (mapping != null && row[sourcefieldname] != DBNull.Value)
                    {
                        
                        Type shifttype = item.GetType();
						PropertyInfo propertyInfo = shifttype.GetProperty(mapping.FieldName);
						propertyInfo.SetValue(item, Convert.ChangeType(row[sourcefieldname], propertyInfo.PropertyType), null);
                        //shifttype.GetProperty(mapping.FieldName).SetValue(item, row[sourcefieldname]);
                    }

                }
                
                this.Insert(item);
               

            }
        }
    }
}



