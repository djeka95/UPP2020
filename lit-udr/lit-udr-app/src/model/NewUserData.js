export default class NewUserData {
    constructor(Hash,TaskId,ProcessDefinitionId,ProcessInstanceId,Id,NewUserEmail,SimulateFail){
        this.hash= Hash;
        this.taskId = TaskId;
        this.processDefinitionId = ProcessDefinitionId;
        this.processInstanceId = ProcessInstanceId
        this.id = Id;
        this.newUserEmail= NewUserEmail;
        this.simulateFail = SimulateFail;

    }
  }