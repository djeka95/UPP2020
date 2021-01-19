export default class ReviewDto {
    constructor(Token,Id,Result,ProcessDefinitionId,ProcessInstanceId,Comment) {
      this.token = Token;
      this.id = Id;
      this.Result = Result;
      this.processDefinitionId = ProcessDefinitionId;
      this.processInstanceId = ProcessInstanceId
      this.comment = Comment;
    }
  }