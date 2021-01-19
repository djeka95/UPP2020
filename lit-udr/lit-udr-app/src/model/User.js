export default class User {
    constructor(FirstName,LastName,Email,Password,Genre,Country,City,Writer,BetaReader,TaskId,ProcessDefinitionId,ProcessInstanceId) {
      this.firstName = FirstName;
      this.lastName = LastName;
      this.email = Email;
      this.password = Password;
      this.genre = Genre;
      this.country = Country;
      this.city = City;
      this.writer = Writer;
      this.betaReader = BetaReader;
      this.taskId = TaskId;
      this.processDefinitionId = ProcessDefinitionId;
      this.processInstanceId = ProcessInstanceId;
    }
  }