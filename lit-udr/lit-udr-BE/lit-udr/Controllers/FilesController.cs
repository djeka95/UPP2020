using lit_udr.Camunda.Model;
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
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using User = lit_udr.EntityFramework.Model.User;

namespace lit_udr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        static readonly HttpClient client = new HttpClient();
        private IConfiguration _configuration;
        private readonly LitUdrContext _context;

        public FilesController(IConfiguration configuration, LitUdrContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> StartFileUpload(string hash)
        {
            if (hash == null)
                return BadRequest();

            NewUserData data = _context.NewUserData.FirstOrDefault(ud => ud.Hash.Equals(hash));


            HttpResponseMessage taskResponse2 = await client.GetAsync($"{_configuration["url"]}/task?processDefinitionId={data.processDefinitionId}&&processInstanceId={data.processInstanceId}");
            Task<string> jsonStringResult_22 = taskResponse2.Content.ReadAsStringAsync();
            List<CamundaTask> tasks = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResult_22.Result);

            if (tasks.Count == 0)
                return BadRequest();

            HttpResponseMessage taskFormResponse = await client.GetAsync($"{_configuration["url"]}/task/{tasks[0].id}/form-variables");
            Task<string> jsonStringResult = taskFormResponse.Content.ReadAsStringAsync();
            FormVariables vars2 = JsonConvert.DeserializeObject<FormVariables>(jsonStringResult.Result);

            return Ok(ParseAndGetConstraints(vars2, data.processDefinitionId, data.processInstanceId, tasks[0].id, null));
        }

        [HttpPost]
        [Route("complete")]
        public async Task<IActionResult> CompleteFileUploadTask([FromBody]NewUserDataDto dto)
        {
            if (dto == null)
                return BadRequest();

            NewUserData newUserData = _context.NewUserData.FirstOrDefault(nud => nud.Hash.Equals(dto.Hash));
            EntityFramework.Model.User user = _context.Users.FirstOrDefault(u => u.Email.Equals(newUserData.NewUserEmmail));
            if (!user.UserVerified)
                return BadRequest();

            if (dto.SimulateFail == true)
            {
                user.UserRetryCount += 1;
                _context.Users.Update(user);
                _context.SaveChanges();

                return Ok();
            }

            HttpResponseMessage taskResponse = await client.GetAsync($"{_configuration["url"]}/task?processDefinitionId={dto.ProcessDefinitionId}&&processInstanceId={dto.ProcessInstanceId}");
            Task<string> jsonStringResult = taskResponse.Content.ReadAsStringAsync();
            List<CamundaTask> tasks = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResult.Result);

            if (tasks.Count == 0)
                return BadRequest();

            //complete task
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{tasks[0].id}/complete", content);

            user.UserRetryCount += 1;
            _context.Update(user);
            _context.SaveChanges();

            try
            {
                WorkApplicationData data = new WorkApplicationData()
                {
                    BoardMembeNeedsMoreData = false,
                    BoardMembersApprove = 0,
                    BoardMembers = new List<UserReview>(),
                    processDefinitionId = dto.ProcessDefinitionId,
                    processInstanceId = dto.ProcessInstanceId,
                    WriterEmail = user.Email,
                    Comments = "Board member had this to say for your work: \n"
                };

                _context.workApplicationData.Add(data);
                _context.SaveChanges();


                WorkApplicationData currentData = _context.workApplicationData.FirstOrDefault(d => d.Id.Equals(data.Id));
                List<User> reviewers = _context.Users.Where(u => u.Email.Contains("clanodbora")).ToList();
                currentData.BoardMembersInitialCount = reviewers.Count();

                foreach(var reviewer in reviewers)
                {
                    currentData.BoardMembers.Add(new UserReview() { UserId = reviewer.Id, WorkApplicationDataId = currentData.Id });
                }

                _context.workApplicationData.Update(currentData);
                _context.SaveChanges();

                //logujem ga
                FetchAndLock fetchAndLock = new FetchAndLock() { workerId = user.Email, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "ProveraKojiPutPisacPodnosiRadove" } } };
                var fetchAndLockContent = new StringContent(JsonConvert.SerializeObject(fetchAndLock), Encoding.UTF8, "application/json");
                HttpResponseMessage lockExternalTask = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContent);

                //daj mi moj lokovan da izvucem id
                HttpResponseMessage externalTaskInfo = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={user.Email}");
                Task<string> jsonStringResult_1 = externalTaskInfo.Content.ReadAsStringAsync();
                List<CamundaExternalTask> externalTasks = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1.Result);
                CamundaExternalTask currenTask = externalTasks.First();

                //prosledimm rezultat
                ClanOdbora clan = new ClanOdbora() { value = user.Email };
                ClanoviOdbora clanovi = new ClanoviOdbora() { value = reviewers.ConvertAll<string>(u => u.FirstName) };
                NumberOfTrials brojPojusaja = new NumberOfTrials() { value = user.UserRetryCount };
                Camunda.Model.ProcessModel.VariablesSecondTask test = new VariablesSecondTask() { ClanOdbora = clan, ClanoviOdbora = clanovi, NumberOfTrials = brojPojusaja };
                CompleteExternalTask completeExternalTask = new CompleteExternalTask() { workerId = user.Email, variables = test };
                var completeExternalTaskContent = new StringContent(JsonConvert.SerializeObject(completeExternalTask), Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteExternalTask = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTask.id}/complete", completeExternalTaskContent);

            }
            catch(Exception e)
            {
                return BadRequest();
            }
       

            if(user.UserRetryCount > 3)
            {
                try
                {
                    EmailService.SendEmail(new UserDto() { Email = user.Email }, _context, "NumberOfUploads");

                    //logujem ga
                    FetchAndLock fetchAndLockFail = new FetchAndLock() { workerId = user.Email, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "NotificiranjeONeuspehu" } } };
                    var fetchAndLockContentFail = new StringContent(JsonConvert.SerializeObject(fetchAndLockFail), Encoding.UTF8, "application/json");
                    HttpResponseMessage lockExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContentFail);

                    //daj mi moj lokovan da izvucem id
                    HttpResponseMessage externalTaskInfoFail = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={user.Email}");
                    Task<string> jsonStringResult_1Fail = externalTaskInfoFail.Content.ReadAsStringAsync();
                    List<CamundaExternalTask> externalTasksFail = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1Fail.Result);
                    CamundaExternalTask currenTaskFail = externalTasksFail.First();

                    //prosledimm reziltat
                    CompleteExternalTask completeExternalTaskFail = new CompleteExternalTask() { workerId = user.Email, variables = null };
                    var completeExternalTaskContentFail = new StringContent(JsonConvert.SerializeObject(completeExternalTaskFail), Encoding.UTF8, "application/json");
                    HttpResponseMessage commpleteExternalTaskFail = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTaskFail.id}/complete", completeExternalTaskContentFail);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }
            }
            return Ok();

        }

        private string ParseAndGetConstraints(FormVariables vars, string processDefinitionId, string processInstanceId, string taskId, List<string> genreNames)
        {
            //Parsiranjee XML-a za validaciju
            Task<HttpResponseMessage> xml = client.GetAsync($"{_configuration["url"]}/process-definition/{processDefinitionId}/xml");
            Task<string> xmlResullt = xml.Result.Content.ReadAsStringAsync();
            CamundaDiagram result = JsonConvert.DeserializeObject<CamundaDiagram>(xmlResullt.Result);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result.bpmn20Xml);
            XmlNodeList nodes = doc.GetElementsByTagName("camunda:formField");

            foreach (var prop in vars.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.OuterXml.Contains(prop.PropertyType.Name))
                    {
                        XmlNodeList xmlNodes = node.ChildNodes;
                        foreach (XmlNode xmlNode in xmlNodes)
                        {
                            CamundaConstraint camundaConstraint = new CamundaConstraint();
                            if (xmlNode.InnerXml.Contains("required"))
                                camundaConstraint.Required = true;
                            if (xmlNode.InnerXml.Contains("minlength"))
                                camundaConstraint.MinLength = xmlNode.InnerXml.Substring(xmlNode.InnerXml.LastIndexOf("minlength\" config=\"") + "minlength\" config=\"".Length, 1);
                            if (xmlNode.InnerXml.Contains("maxlength"))
                            {
                                int startIndex = xmlNode.InnerXml.IndexOf("&quot;") + "&quot;".Length;
                                int endIndex = xmlNode.InnerXml.LastIndexOf("&quot;");
                                camundaConstraint.MaxLength = xmlNode.InnerXml.Substring(startIndex, endIndex - startIndex);

                            }
                            if (xmlNode.InnerXml.Contains("min"))
                                camundaConstraint.Min = xmlNode.InnerXml.Substring(xmlNode.InnerXml.LastIndexOf("min\" config=\"") + "min\" config=\"".Length, 1);

                            if (prop.PropertyType.Name.Equals("FirstName"))
                                if(vars.firstName != null)
                                    vars.firstName.value = camundaConstraint;

                            if (prop.PropertyType.Name.Equals("LastName"))
                                if (vars.lastName != null)
                                    vars.lastName.value = camundaConstraint;

                            if (prop.PropertyType.Name.Equals("Email"))
                                if (vars.email != null)
                                {
                                    camundaConstraint.value = camundaConstraint.MaxLength;
                                    vars.email.value = camundaConstraint;
                                }

                            if (prop.PropertyType.Name.Equals("Password"))
                                if (vars.password != null)
                                    vars.password.value = camundaConstraint;

                            if (prop.PropertyType.Name.Equals("Genre"))
                                if (vars.genre != null)
                                {
                                    camundaConstraint.value = genreNames;
                                    vars.genre.value = camundaConstraint;
                                }
                            if (prop.PropertyType.Name.Equals("Country"))
                                if (vars.country != null)
                                {
                                    camundaConstraint.value = CountryService.GetCountryList();
                                    vars.country.value = camundaConstraint;
                                }
                            if (prop.PropertyType.Name.Equals("City"))
                                if (vars.city != null)
                                    vars.city.value = camundaConstraint;

                            if (prop.PropertyType.Name.Equals("CamundaFile"))
                                if (vars.camundaFile != null)
                                    vars.camundaFile.value = camundaConstraint;


                            vars.TaskId = taskId;
                            vars.ProcessDefinitionId = processDefinitionId;
                            vars.ProcessInstanceId = processInstanceId;

                        }
                    }
                }
            }

            var props = new Dictionary<string, object>();
            foreach (var prop in vars.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                props.Add(prop.Name, prop.GetValue(vars, null));
            }

            return JsonConvert.SerializeObject(props.ToArray());
        }
    }
}
