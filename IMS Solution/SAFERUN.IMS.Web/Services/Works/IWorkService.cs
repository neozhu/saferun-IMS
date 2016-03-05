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

namespace SAFERUN.IMS.Web.Services
{
    public interface IWorkService:IService<Work>
    {

                  IEnumerable<Work> GetByWorkTypeId(int  worktypeid);
                 IEnumerable<Work> GetByOrderId(int  orderid);
        
                 IEnumerable<WorkDetail>   GetWorkDetailsByWorkId (int workid);
         
         
 
		void ImportDataTable(DataTable datatable);
	}
}