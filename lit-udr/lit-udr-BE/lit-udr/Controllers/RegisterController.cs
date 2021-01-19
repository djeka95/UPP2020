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
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using User = lit_udr.EntityFramework.Model.User;
using Writer = lit_udr.Camunda.Model.ProcessModel.Writer;

namespace lit_udr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        static readonly HttpClient client = new HttpClient();
        private IConfiguration _configuration;
        private readonly LitUdrContext _context;

        public RegisterController(IConfiguration configuration,LitUdrContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> StartRegistration(string processDefinitionId, string processInstanceId)
        {
            try
            {
                List<ProcessDefinitionData> data;
                List<string> genreNames = new List<string>();
                _context.Genres.ToList().ForEach(x => genreNames.Add(x.Name));

                if (processDefinitionId == null && processInstanceId == null)
                {


                    HttpResponseMessage response = await client.GetAsync($"{_configuration["url"]}/process-definition");
                    Task<string> jsonStringResult_1 = response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<List<ProcessDefinitionData>>(jsonStringResult_1.Result);
                    processDefinitionId = data[0].Id;

                    var jsonData = JsonConvert.SerializeObject(new StartProcess() { variables = new lit_udr.Camunda.Model.Variables() { Korisnik = new lit_udr.Camunda.Model.User() { value = "Nemanja" } } });
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage startResppponse = await client.PostAsync($"{_configuration["url"]}/process-definition/{data[0].Id}/start", content);

                    HttpResponseMessage taskResponse = await client.GetAsync($"{_configuration["url"]}/task?processDefinitionId={data[0].Id}&&name=Registracija");
                    Task<string> jsonStringResult_2 = taskResponse.Content.ReadAsStringAsync();
                    List<CamundaTask> tasks = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResult_2.Result);

                    HttpResponseMessage taskFormResponse = await client.GetAsync($"{_configuration["url"]}/task/{tasks[0].id}/form-variables");
                    Task<string> jsonStringResult_3 = taskFormResponse.Content.ReadAsStringAsync();
                    FormVariables vars = JsonConvert.DeserializeObject<FormVariables>(jsonStringResult_3.Result);

                    return Ok(ParseAndGetConstraints(vars, processDefinitionId, tasks[0].processInstanceId, tasks[0].id, genreNames));
                }



                HttpResponseMessage taskResponse2 = await client.GetAsync($"{_configuration["url"]}/task?processDefinitionId={processDefinitionId}&&processInstanceId={processInstanceId}");
                Task<string> jsonStringResult_22 = taskResponse2.Content.ReadAsStringAsync();
                List<CamundaTask> tasks2 = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResult_22.Result);

                if (tasks2.Count == 0)
                    return BadRequest();

                HttpResponseMessage taskFormResponse2 = await client.GetAsync($"{_configuration["url"]}/task/{tasks2[0].id}/form-variables");
                Task<string> jsonStringResult_32 = taskFormResponse2.Content.ReadAsStringAsync();
                FormVariables vars2 = JsonConvert.DeserializeObject<FormVariables>(jsonStringResult_32.Result);

                return Ok(ParseAndGetConstraints(vars2, processDefinitionId, tasks2[0].processInstanceId, tasks2[0].id, genreNames)); 
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewUser([FromBody]UserDto dto)
        {
            int userId = -1;
            try
            {
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{dto.TaskId}/complete", content);

                lit_udr.EntityFramework.Model.User newUser = new lit_udr.EntityFramework.Model.User()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Password = CreatePasswordHash(dto.Password),
                    Email = dto.Email.ToLower(),
                    Country = dto.Country,
                    City = dto.City,
                    Writer = dto.Writer,
                    BetaReader = dto.BetaReader,
                    UserVerified = false,
                    UserRetryCount = 0,
                    UserGenres = new List<UserGenre>()
                };

                dto.Genre = dto.Genre.Distinct().ToList();
                foreach(string genre in dto.Genre)
                {
                    lit_udr.EntityFramework.Model.Genre genreInDatabase = _context.Genres.FirstOrDefault(x => x.Name.Equals(genre));
                    newUser.UserGenres.Add(new UserGenre() { UserId = newUser.Id, GenreId = genreInDatabase.Id });
                }
                _context.Users.Add(newUser);
                _context.SaveChanges();

                userId = newUser.Id;
                ControlFlow.ResumeOnError(() => { EmailService.SendEmail(dto, _context, "Registration"); }); 

                //logujem ga
                FetchAndLock fetchAndLock = new FetchAndLock() { workerId = newUser.Email, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "ProveraRegistracije" } } };
                var fetchAndLockContent = new StringContent(JsonConvert.SerializeObject(fetchAndLock), Encoding.UTF8, "application/json");
                HttpResponseMessage lockExternalTask = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContent);

                //daj mi moj lokovan da izvucem id
                HttpResponseMessage externalTaskInfo = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={newUser.Email}");
                Task<string> jsonStringResult_1 = externalTaskInfo.Content.ReadAsStringAsync();
                List<CamundaExternalTask> externalTasks = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1.Result);
                CamundaExternalTask currenTask = externalTasks.First();

                //prosledimm reziltat
                CompleteExternalTask completeExternalTask = new CompleteExternalTask() { workerId = newUser.Email, variables = new Camunda.Model.ProcessModel.Variables() { DataCorrect = new DataCorrect() { value = true } } };
                var completeExternalTaskContent = new StringContent(JsonConvert.SerializeObject(completeExternalTask), Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteExternalTask = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTask.id}/complete", completeExternalTaskContent);



                return Ok();

            }
            catch(Exception e)
            {
                User currentUser = _context.Users.FirstOrDefault(u => u.Id.Equals(userId));
                if(currentUser != null)
                {
                    _context.Users.Remove(currentUser);
                    _context.SaveChanges();
                }

                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{dto.TaskId}/complete", content);

                //logujem ga
                FetchAndLock fetchAndLock = new FetchAndLock() { workerId = "BadRequest", maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "ProveraRegistracije" } } };
                var fetchAndLockContent = new StringContent(JsonConvert.SerializeObject(fetchAndLock), Encoding.UTF8, "application/json");
                HttpResponseMessage lockExternalTask = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContent);

                //daj mi moj lokovan da izvucem id
                HttpResponseMessage externalTaskInfo = await client.GetAsync($"{_configuration["url"]}/external-task?workerId=BadRequest");
                Task<string> jsonStringResult_1 = externalTaskInfo.Content.ReadAsStringAsync();
                List<CamundaExternalTask> externalTasks = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1.Result);
                CamundaExternalTask currenTask = externalTasks.First();

                //prosledimm reziltat
                CompleteExternalTask completeExternalTask = new CompleteExternalTask() { workerId = "BadRequest", variables = new Camunda.Model.ProcessModel.Variables() { DataCorrect = new DataCorrect() { value = false } } };
                var completeExternalTaskContent = new StringContent(JsonConvert.SerializeObject(completeExternalTask), Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteExternalTask = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTask.id}/complete", completeExternalTaskContent);


                return BadRequest();
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
                User currentUser = _context.Users.FirstOrDefault(u => u.Email.Equals(data.NewUserEmmail));
                currentUser.UserVerified = true;


                _context.Update(currentUser);
                _context.SaveChanges();

                if(data.Writer)
                    ControlFlow.ResumeOnError(() => { EmailService.SendEmail(new UserDto() { FirstName = data.Hash, Writer = data.Writer, Email = data.NewUserEmmail }, null, "WorkUpload"); });

                HttpResponseMessage taskResponse = await client.GetAsync($"{_configuration["url"]}/task?processDefinitionId={data.processDefinitionId}&&processInstanceId={data.processInstanceId}");
                Task<string> jsonStringResult = taskResponse.Content.ReadAsStringAsync();
                List<CamundaTask> tasks = JsonConvert.DeserializeObject<List<CamundaTask>>(jsonStringResult.Result);

                if (tasks.Count == 0)
                    return BadRequest();

                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteTask = await client.PostAsync($"{_configuration["url"]}/task/{tasks[0].id}/complete", content);

                //logujem ga
                FetchAndLock fetchAndLock = new FetchAndLock() { workerId = currentUser.Email, maxTasks = 10, topics = new List<Topic>() { new Topic() { lockDuration = 10000, topicName = "ProveraKorisnikovePotvrde" } } };
                var fetchAndLockContent = new StringContent(JsonConvert.SerializeObject(fetchAndLock), Encoding.UTF8, "application/json");
                HttpResponseMessage lockExternalTask = await client.PostAsync($"{_configuration["url"]}/external-task/fetchAndLock", fetchAndLockContent);

                //daj mi moj lokovan da izvucem id
                HttpResponseMessage externalTaskInfo = await client.GetAsync($"{_configuration["url"]}/external-task?workerId={currentUser.Email}");
                Task<string> jsonStringResult_1 = externalTaskInfo.Content.ReadAsStringAsync();
                List<CamundaExternalTask> externalTasks = JsonConvert.DeserializeObject<List<CamundaExternalTask>>(jsonStringResult_1.Result);
                CamundaExternalTask currenTask = externalTasks.First();

                //prosledimm reziltat
                CompleteExternalTask completeExternalTask = new CompleteExternalTask() { workerId = currentUser.Email, variables = new VariablesFirstTask() { Writer = new Writer() { value = data.Writer } } };
                var completeExternalTaskContent = new StringContent(JsonConvert.SerializeObject(completeExternalTask), Encoding.UTF8, "application/json");
                HttpResponseMessage commpleteExternalTask = await client.PostAsync($"{_configuration["url"]}/external-task/{currenTask.id}/complete", completeExternalTaskContent);


                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogIn([FromBody]LogInDto dto)
        {
            if (dto == null)
                return BadRequest();

            User currentUser = _context.Users.Where(u => u.Email.Equals(dto.Email.ToLower()) && u.Password.Equals(dto.Password)).FirstOrDefault();
            if (currentUser == null)
                return BadRequest();
            
            return Ok(JsonConvert.SerializeObject(currentUser.Email));
        }

        private string CreatePasswordHash(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        private string ParseAndGetConstraints(FormVariables vars,string processDefinitionId,string processInstanceId, string taskId, List<string> genreNames)
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
                                if (vars.firstName != null)
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
