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
    public class MessagesController : Controller
    {
        
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Message>, Repository<Message>>();
        //container.RegisterType<IMessageService, MessageService>();
        
        //private ImsDbContext db = new ImsDbContext();
        private readonly IMessageService  _messageService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public MessagesController (IMessageService  messageService, IUnitOfWorkAsync unitOfWork)
        {
            _messageService  = messageService;
            _unitOfWork = unitOfWork;
        }

        // GET: Messages/Index
        public ActionResult Index()
        {
            
            //var messages  = _messageService.Queryable().Include(m => m.RequestMessage).AsQueryable();
            
             //return View(messages);
			 return View();
        }

        // Get :Messages/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            			 
            var messages  = _messageService.Query(new MessageQuery().Withfilter(filters)).Include(m => m.RequestMessage).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
            
                        var datarows = messages .Select(  n => new { RequestMessageFrom = (n.RequestMessage==null?"": n.RequestMessage.From) , Id = n.Id , From = n.From , To = n.To , Title = n.Title , Content = n.Content , Status = n.Status , CreateTime = n.CreateTime , MessageId = n.MessageId }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(MessageChangeViewModel messages)
        {
            if (messages.updated != null)
            {
                foreach (var updated in messages.updated)
                {
                    _messageService.Update(updated);
                }
            }
            if (messages.deleted != null)
            {
                foreach (var deleted in messages.deleted)
                {
                    _messageService.Delete(deleted);
                }
            }
            if (messages.inserted != null)
            {
                foreach (var inserted in messages.inserted)
                {
                    _messageService.Insert(inserted);
                }
            }
            _unitOfWork.SaveChanges();

            return Json(new {Success=true}, JsonRequestBehavior.AllowGet);
        }

				public ActionResult GetMessages()
        {
            var messageRepository = _unitOfWork.Repository<Message>();
            var data = messageRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, From = n.From });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
		
		
       
        // GET: Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = _messageService.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        [ChildActionOnly]
        public ActionResult GetMyMessage(string user)
        {
            var messages = _messageService.Queryable().Where(x => x.To == user && x.Status==0).ToList();

            return PartialView("_ToMyMesssage", messages);
        }
        

        // GET: Messages/Create
        public ActionResult Create()
        {
            Message message = new Message();
            //set default value
            var messageRepository = _unitOfWork.Repository<Message>();
            ViewBag.MessageId = new SelectList(messageRepository.Queryable(), "Id", "From");
            return View(message);
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RequestMessage,Id,From,To,Title,Content,Status,CreateTime,MessageId")] Message message)
        {
            if (ModelState.IsValid)
            {
             				_messageService.Insert(message);
                           _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Message record");
                return RedirectToAction("Index");
            }

            var messageRepository = _unitOfWork.Repository<Message>();
            ViewBag.MessageId = new SelectList(messageRepository.Queryable(), "Id", "From", message.MessageId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(message);
        }

        // GET: Messages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = _messageService.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            var messageRepository = _unitOfWork.Repository<Message>();
            ViewBag.MessageId = new SelectList(messageRepository.Queryable(), "Id", "From", message.MessageId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RequestMessage,Id,From,To,Title,Content,Status,CreateTime,MessageId")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.ObjectState = ObjectState.Modified;
                				_messageService.Update(message);
                                
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Message record");
                return RedirectToAction("Index");
            }
            var messageRepository = _unitOfWork.Repository<Message>();
            ViewBag.MessageId = new SelectList(messageRepository.Queryable(), "Id", "From", message.MessageId);
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(message);
        }

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = _messageService.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message =  _messageService.Find(id);
             _messageService.Delete(message);
            _unitOfWork.SaveChanges();
           if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            DisplaySuccessMessage("Has delete a Message record");
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
