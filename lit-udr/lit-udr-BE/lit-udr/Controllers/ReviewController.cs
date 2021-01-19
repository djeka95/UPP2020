using lit_udr.Camunda.Model.ProcessModel;
using lit_udr.EntityFramework;
using lit_udr.EntityFramework.Model;
using lit_udr.Model;
using lit_udr.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lit_udr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        static readonly HttpClient client = new HttpClient();
        public IConfiguration _configuration;
        public readonly LitUdrContext _context;

        public ReviewController(IConfiguration configuration, LitUdrContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviewTasks(string Token)
        {
            User currentUser = _context.Users.FirstOrDefault(u => u.Email.Equals(Token));
            List<UserReview> reviewData = _context.UserReview.Where(u => u.UserId.Equals(currentUser.Id)).ToList();
            List<WorkApplicationDataDto> data = new List<WorkApplicationDataDto>();

            foreach(var review in reviewData)
            {
                WorkApplicationData result = _context.workApplicationData.FirstOrDefault(data => data.Id.Equals(review.WorkApplicationDataId));
                if (result != null)
                    data.Add(new WorkApplicationDataDto() {processDefinitionId = result.processDefinitionId,processInstanceId = result.processInstanceId,Id = result.Id });
            }

            return Ok(JsonConvert.SerializeObject(data));
        }

        [HttpPost]
        public async Task<IActionResult> PostReviewData([FromBody]ReviewDto dto)
        {
            if (dto == null)
                return BadRequest();

            User boardMember = _context.Users.FirstOrDefault(u => u.Email.Equals(dto.Token));
            WorkApplicationData data = _context.workApplicationData.FirstOrDefault(wad => wad.Id.Equals(dto.Id));
            List<UserReview> allReviews = _context.UserReview.Where(ur => ur.WorkApplicationDataId.Equals(dto.Id)).ToList();
            UserReview currentReview = allReviews.Find(u => u.WorkApplicationDataId.Equals(data.Id) && u.UserId.Equals(boardMember.Id));
            data.Comments += $"\n {boardMember.Email}: {dto.Comment} \n";

            switch (dto.Result)
            {
                case "approve":
                    _context.UserReview.Remove(currentReview);
                    data.BoardMembersApprove += 1;

                    _context.workApplicationData.Update(data);
                    _context.SaveChanges();
                    break;
                case "decline":
                    _context.UserReview.Remove(currentReview);

                    _context.SaveChanges();
                    break;
                case "needMoreData":
                    _context.UserReview.Remove(currentReview);
                    data.BoardMembeNeedsMoreData = true;

                    _context.workApplicationData.Update(data);
                    _context.SaveChanges();
                    break;
            }

            //TODO u workaplication data sacuvati komentare u novom polu i posle ih proslediti kroz mail
            List<UserReview> currentAllReviews = _context.UserReview.Where(ur => ur.WorkApplicationDataId.Equals(dto.Id)).ToList();
            if (currentAllReviews.Count > 0)
            {
                HttpResponseMessage taskResponse = await client.GetAsync($"{_configuration["url"]}/task?processDefinitionId={data.processDefinitionId}&&processInstanceId={data.processInstanceId}");
                Task<string> jsonStringResult = taskResponse.Content.ReadAsStringAsync();
                List<CamundaTask> tasks = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResult.Result);

                if (tasks.Count == 0)
                    return BadRequest();

                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{tasks[0].id}/complete", content);
            }
            else
            {
                HttpResponseMessage taskResponse = await client.GetAsync($"{_configuration["url"]}/task?processDefinitionId={data.processDefinitionId}&&processInstanceId={data.processInstanceId}");
                Task<string> jsonStringResult = taskResponse.Content.ReadAsStringAsync();
                List<CamundaTask> tasks = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResult.Result);

                if (tasks.Count == 0)
                    return BadRequest();

                StringContent content;
                if (data.BoardMembeNeedsMoreData)
                {
                    content = new StringContent(JsonConvert.SerializeObject(new CompleteUserTaskWithVariable() { variables = new WriterFailedAbstract() { WriterFailed = new object() } }), Encoding.UTF8, "application/json");
                    HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{tasks[0].id}/complete", content);

                    try
                    {
                        string guid = Guid.NewGuid().ToString();
                        ControlFlow.ResumeOnError(() => { EmailService.SendEmail(new UserDto() { Email = data.WriterEmail, Password = guid, FirstName = data.Comments }, _context, "NeedMoreWork");});

                        NewUserData newUserData = new NewUserData() { Hash = guid, NewUserEmmail = data.WriterEmail, processDefinitionId = data.processDefinitionId, processInstanceId = data.processInstanceId };
                        _context.NewUserData.Add(newUserData);
                        _context.SaveChanges();

                        //TODO sacuvati u bazu i obrnuti krug
                        //logujem ga
                        FetchAndLock fetchAndLockFail = new FetchAndLock() { workerId = data.WriterEmail, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "NotificiranjeIDavanjeRoka" } } };
                        var fetchAndLockContentFail = new StringContent(JsonConvert.SerializeObject(fetchAndLockFail), Encoding.UTF8, "application/json");
                        HttpResponseMessage lockExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContentFail);

                        //daj mi moj lokovan da izvucem id
                        HttpResponseMessage externalTaskInfoFail = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={data.WriterEmail}");
                        Task<string> jsonStringResult_1Fail = externalTaskInfoFail.Content.ReadAsStringAsync();
                        List<CamundaExternalTask> externalTasksFail = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1Fail.Result);
                        CamundaExternalTask currenTaskFail = externalTasksFail.First();

                        //prosledimm reziltat
                        CompleteExternalTask completeExternalTaskFail = new CompleteExternalTask() { workerId = data.WriterEmail, variables = null };
                        var completeExternalTaskContentFail = new StringContent(JsonConvert.SerializeObject(completeExternalTaskFail), Encoding.UTF8, "application/json");
                        HttpResponseMessage commpleteExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTaskFail.id}/complete", completeExternalTaskContentFail);

                        const int scheduledPeriodMilliseconds = 70000;
                        var allTasksWaitHandle = new AutoResetEvent(true);

                        ThreadPool.RegisterWaitForSingleObject(
                        allTasksWaitHandle,
                        (s, b) => { UploadTimeOutCheck(data, _context, _configuration); },
                        null,
                        scheduledPeriodMilliseconds, false);
                    }
                    catch (Exception e)
                    {
                        return BadRequest();
                    }

                }
                else if(data.BoardMembersApprove == data.BoardMembersInitialCount)
                {
                    content = new StringContent(JsonConvert.SerializeObject(new CompleteUserTaskWithVariable() { variables = new WriterFailedAbstract() { WriterFailed = new WriterFailed() { value = false } } }), Encoding.UTF8, "application/json");
                    HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{tasks[0].id}/complete", content);

                    try
                    {
                        string guid = Guid.NewGuid().ToString();
                        ControlFlow.ResumeOnError(() => { EmailService.SendEmail(new UserDto() { Email = data.WriterEmail, Password = guid, FirstName = data.Comments }, _context, "Approved");});

                        NewUserData newUserData = new NewUserData() { Hash = guid, processDefinitionId = data.processDefinitionId, processInstanceId = data.processInstanceId };
                        _context.NewUserData.Add(newUserData);
                        _context.SaveChanges();

                        //logujem ga
                        FetchAndLock fetchAndLockFail = new FetchAndLock() { workerId = data.WriterEmail, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "NotificiranjeOUspehu" } } };
                        var fetchAndLockContentFail = new StringContent(JsonConvert.SerializeObject(fetchAndLockFail), Encoding.UTF8, "application/json");
                        HttpResponseMessage lockExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContentFail);

                        //daj mi moj lokovan da izvucem id
                        HttpResponseMessage externalTaskInfoFail = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={data.WriterEmail}");
                        Task<string> jsonStringResult_1Fail = externalTaskInfoFail.Content.ReadAsStringAsync();
                        List<CamundaExternalTask> externalTasksFail = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1Fail.Result);
                        CamundaExternalTask currenTaskFail = externalTasksFail.First();

                        //prosledimm reziltat
                        CompleteExternalTask completeExternalTaskFail = new CompleteExternalTask() { workerId = data.WriterEmail, variables = null };
                        var completeExternalTaskContentFail = new StringContent(JsonConvert.SerializeObject(completeExternalTaskFail), Encoding.UTF8, "application/json");
                        HttpResponseMessage commpleteExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTaskFail.id}/complete", completeExternalTaskContentFail);

                        const int scheduledPeriodMilliseconds = 70000;
                        var allTasksWaitHandle = new AutoResetEvent(true);

                        ThreadPool.RegisterWaitForSingleObject(
                        allTasksWaitHandle,
                        (s, b) => { TimeOutCheck(data,_context,_configuration); },
                        null,
                        scheduledPeriodMilliseconds,false);
                    }
                    catch (Exception e)
                    {
                        return BadRequest();
                    }
                }
                else if(data.BoardMembersApprove < (data.BoardMembersInitialCount - data.BoardMembersApprove))
                {
                    content = new StringContent(JsonConvert.SerializeObject(new CompleteUserTaskWithVariable() { variables = new WriterFailedAbstract() { WriterFailed = new WriterFailed() { value = true } } }), Encoding.UTF8, "application/json");
                    HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{tasks[0].id}/complete", content);

                    try
                    {
                        ControlFlow.ResumeOnError(() => { EmailService.SendEmail(new UserDto() { Email = data.WriterEmail, FirstName = data.Comments }, _context, "Declined");});
                        //logujem ga
                        FetchAndLock fetchAndLockFail = new FetchAndLock() { workerId = data.WriterEmail, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "NotificiranjeOOdbijanju" } } };
                        var fetchAndLockContentFail = new StringContent(JsonConvert.SerializeObject(fetchAndLockFail), Encoding.UTF8, "application/json");
                        HttpResponseMessage lockExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContentFail);

                        //daj mi moj lokovan da izvucem id
                        HttpResponseMessage externalTaskInfoFail = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={data.WriterEmail}");
                        Task<string> jsonStringResult_1Fail = externalTaskInfoFail.Content.ReadAsStringAsync();
                        List<CamundaExternalTask> externalTasksFail = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1Fail.Result);
                        CamundaExternalTask currenTaskFail = externalTasksFail.First();

                        //prosledimm reziltat
                        CompleteExternalTask completeExternalTaskFail = new CompleteExternalTask() { workerId = data.WriterEmail, variables = null };
                        var completeExternalTaskContentFail = new StringContent(JsonConvert.SerializeObject(completeExternalTaskFail), Encoding.UTF8, "application/json");
                        HttpResponseMessage commpleteExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTaskFail.id}/complete", completeExternalTaskContentFail);

                        const int scheduledPeriodMilliseconds = 70000;
                        var allTasksWaitHandle = new AutoResetEvent(true);

                        ThreadPool.RegisterWaitForSingleObject(
                        allTasksWaitHandle,
                        (s, b) => { UploadTimeOutCheck(data, _context, _configuration); },
                        null,
                        scheduledPeriodMilliseconds, false);
                    }
                    catch (Exception e)
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    content = new StringContent(JsonConvert.SerializeObject(new CompleteUserTaskWithVariable() { variables = new WriterFailedAbstract() { WriterFailed = new object() } }), Encoding.UTF8, "application/json");
                    HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{tasks[0].id}/complete", content);

                    try
                    {
                        string guid = Guid.NewGuid().ToString();
                        ControlFlow.ResumeOnError(() => { EmailService.SendEmail(new UserDto() { Email = data.WriterEmail, Password = guid, FirstName = data.Comments }, _context, "NeedMoreWork");});

                        NewUserData newUserData = new NewUserData() { Hash = guid, NewUserEmmail = data.WriterEmail, processDefinitionId = data.processDefinitionId, processInstanceId = data.processInstanceId };
                        _context.NewUserData.Add(newUserData);
                        _context.SaveChanges();

                        //logujem ga
                        FetchAndLock fetchAndLockFail = new FetchAndLock() { workerId = data.WriterEmail, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "NotificiranjeIDavanjeRoka" } } };
                        var fetchAndLockContentFail = new StringContent(JsonConvert.SerializeObject(fetchAndLockFail), Encoding.UTF8, "application/json");
                        HttpResponseMessage lockExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContentFail);

                        //daj mi moj lokovan da izvucem id
                        HttpResponseMessage externalTaskInfoFail = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={data.WriterEmail}");
                        Task<string> jsonStringResult_1Fail = externalTaskInfoFail.Content.ReadAsStringAsync();
                        List<CamundaExternalTask> externalTasksFail = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1Fail.Result);
                        CamundaExternalTask currenTaskFail = externalTasksFail.First();

                        //prosledimm reziltat
                        CompleteExternalTask completeExternalTaskFail = new CompleteExternalTask() { workerId = data.WriterEmail, variables = null };
                        var completeExternalTaskContentFail = new StringContent(JsonConvert.SerializeObject(completeExternalTaskFail), Encoding.UTF8, "application/json");
                        HttpResponseMessage commpleteExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTaskFail.id}/complete", completeExternalTaskContentFail);
                    }
                    catch (Exception e)
                    {
                        return BadRequest();
                    }
                }

            }

            return Ok();
        }

        private async static void TimeOutCheck(WorkApplicationData data, LitUdrContext context,IConfiguration _configuration)
        {
            //provera da li je pisac uplatio clanarinu
            HttpResponseMessage taskResponseCheck = await client.GetAsync($"{_configuration["url"]}/external-task?processDefinitionId={data.processDefinitionId}&&processInstanceId={data.processInstanceId}");
            Task<string> jsonStringResultCheck = taskResponseCheck.Content.ReadAsStringAsync();
            List<CamundaTask> tasksCheck = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResultCheck.Result);

            if (tasksCheck.Count > 0)
            {
                ControlFlow.ResumeOnError(() => { EmailService.SendEmail(new UserDto() { Email = data.WriterEmail }, context, "LatePay");});
                //logujem ga
                FetchAndLock fetchAndLockFail = new FetchAndLock() { workerId = data.WriterEmail, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "NotificiranjeOIstekuRokaZaUplatu" } } };
                var fetchAndLockContentFail = new StringContent(JsonConvert.SerializeObject(fetchAndLockFail), Encoding.UTF8, "application/json");
                HttpResponseMessage lockExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContentFail);

                //daj mi moj lokovan da izvucem id
                HttpResponseMessage externalTaskInfoFail = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={data.WriterEmail}");
                Task<string> jsonStringResult_1Fail = externalTaskInfoFail.Content.ReadAsStringAsync();
                List<CamundaExternalTask> externalTasksFail = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1Fail.Result);
                CamundaExternalTask currenTaskFail = externalTasksFail.First();

                //prosledimm reziltat
                CompleteExternalTask completeExternalTaskFail = new CompleteExternalTask() { workerId = data.WriterEmail, variables = null };
                var completeExternalTaskContentFail = new StringContent(JsonConvert.SerializeObject(completeExternalTaskFail), Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTaskFail.id}/complete", completeExternalTaskContentFail);

            }
        }

        private async static void UploadTimeOutCheck(WorkApplicationData data, LitUdrContext context, IConfiguration _configuration)
        {
            //provera da li je pisac uplatio clanarinu
            HttpResponseMessage taskResponseCheck = await client.GetAsync($"{_configuration["url"]}/external-task?processDefinitionId={data.processDefinitionId}&&processInstanceId={data.processInstanceId}");
            Task<string> jsonStringResultCheck = taskResponseCheck.Content.ReadAsStringAsync();
            List<CamundaTask> tasksCheck = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResultCheck.Result);

            if (tasksCheck.Count > 0)
            {
                ControlFlow.ResumeOnError(() => { EmailService.SendEmail(new UserDto() { Email = data.WriterEmail }, context, "LateUpload");});
                //logujem ga
                FetchAndLock fetchAndLockFail = new FetchAndLock() { workerId = data.WriterEmail, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "NotificiranjeONePostovanjuRoka" } } };
                var fetchAndLockContentFail = new StringContent(JsonConvert.SerializeObject(fetchAndLockFail), Encoding.UTF8, "application/json");
                HttpResponseMessage lockExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContentFail);

                //daj mi moj lokovan da izvucem id
                HttpResponseMessage externalTaskInfoFail = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={data.WriterEmail}");
                Task<string> jsonStringResult_1Fail = externalTaskInfoFail.Content.ReadAsStringAsync();
                List<CamundaExternalTask> externalTasksFail = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1Fail.Result);
                CamundaExternalTask currenTaskFail = externalTasksFail.First();

                //prosledimm reziltat
                CompleteExternalTask completeExternalTaskFail = new CompleteExternalTask() { workerId = data.WriterEmail, variables = null };
                var completeExternalTaskContentFail = new StringContent(JsonConvert.SerializeObject(completeExternalTaskFail), Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTaskFail.id}/complete", completeExternalTaskContentFail);

            }
        }

        [HttpGet]
        [Route("activate")]
        public async Task<IActionResult> Activate(string hash)
        {
            if (hash == null)
                return BadRequest();

            try
            {
                NewUserData data = _context.NewUserData.FirstOrDefault(ud => ud.Hash.Equals(hash));

                HttpResponseMessage taskResponse = await client.GetAsync($"{_configuration["url"]}/task?processDefinitionId={data.processDefinitionId}&&processInstanceId={data.processInstanceId}");
                Task<string> jsonStringResult = taskResponse.Content.ReadAsStringAsync();
                List<CamundaTask> tasks = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResult.Result);

                if (tasks.Count == 0)
                    return BadRequest();

                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{tasks[0].id}/complete", content);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }

}
