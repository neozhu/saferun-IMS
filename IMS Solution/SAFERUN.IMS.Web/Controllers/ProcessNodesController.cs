﻿


using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using SAFERUN.IMS.Web.Models;
using SAFERUN.IMS.Web.Services;
using SAFERUN.IMS.Web.Repositories;
using SAFERUN.IMS.Web.Extensions;


namespace SAFERUN.IMS.Web.Controllers
{
    public class ProcessNodesController : Controller
    {
        
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<ProcessNode>, Repository<ProcessNode>>();
        //container.RegisterType<IProcessNodeService, ProcessNodeService>();
        
        //private ImsDbContext db = new ImsDbContext();
        private readonly IProcessNodeService  _processNodeService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ProcessNodesController (IProcessNodeService  processNodeService, IUnitOfWorkAsync unitOfWork)
        {
            _processNodeService  = processNodeService;
            _unitOfWork = unitOfWork;
        }

        // GET: ProcessNodes/Index
        public ActionResult Index()
        {
            
            //var processnodes  = _processNodeService.Queryable().AsQueryable();
            //return View(processnodes  );
			return View();
        }

        // Get :ProcessNodes/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
                        var processnodes  = _processNodeService.Query(new ProcessNodeQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
                        var datarows = processnodes .Select(  n => new {  Id = n.Id , Order = n.Order , Name = n.Name , MachineModel = n.MachineModel , Node = n.Node , Description = n.Description }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(ProcessNodeChangeViewModel processnodes)
        {
            if (processnodes.updated != null)
            {
                foreach (var updated in processnodes.updated)
                {
                    _processNodeService.Update(updated);
                }
            }
            if (processnodes.deleted != null)
            {
                foreach (var deleted in processnodes.deleted)
                {
                    _processNodeService.Delete(deleted);
                }
            }
            if (processnodes.inserted != null)
            {
                foreach (var inserted in processnodes.inserted)
                {
                    _processNodeService.Insert(inserted);
                }
            }
            _unitOfWork.SaveChanges();

            return Json(new {Success=true}, JsonRequestBehavior.AllowGet);
        }

		
		
       
        // GET: ProcessNodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessNode processNode = _processNodeService.Find(id);
            if (processNode == null)
            {
                return HttpNotFound();
            }
            return View(processNode);
        }
        

        // GET: ProcessNodes/Create
        public ActionResult Create()
        {
            ProcessNode processNode = new ProcessNode();
            //set default value
            return View(processNode);
        }

        // POST: ProcessNodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Order,Name,MachineModel,Node,Description,CreatedUserId,CreatedDateTime,LastEditUserId,LastEditDateTime")] ProcessNode processNode)
        {
            if (ModelState.IsValid)
            {
             				_processNodeService.Insert(processNode);
                           _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a ProcessNode record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(processNode);
        }

        // GET: ProcessNodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessNode processNode = _processNodeService.Find(id);
            if (processNode == null)
            {
                return HttpNotFound();
            }
            return View(processNode);
        }

        // POST: ProcessNodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Order,Name,MachineModel,Node,Description,CreatedUserId,CreatedDateTime,LastEditUserId,LastEditDateTime")] ProcessNode processNode)
        {
            if (ModelState.IsValid)
            {
                processNode.ObjectState = ObjectState.Modified;
                				_processNodeService.Update(processNode);
                                
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a ProcessNode record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(processNode);
        }

        // GET: ProcessNodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessNode processNode = _processNodeService.Find(id);
            if (processNode == null)
            {
                return HttpNotFound();
            }
            return View(processNode);
        }

        // POST: ProcessNodes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProcessNode processNode =  _processNodeService.Find(id);
             _processNodeService.Delete(processNode);
            _unitOfWork.SaveChanges();
           if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            DisplaySuccessMessage("Has delete a ProcessNode record");
            return RedirectToAction("Index");
        }


       

 

        private void DisplaySuccessMessage(string msgText)
        {
            TempData["SuccessMessage"] = msgText;
        }

        private void DisplayErrorMessage()
        {
            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
