namespace Skillion.IO
{
    public class DialogDirectiveResponse : StandardResponse<DialogResponse>
    {
        public DialogDirectiveResponse()
        {
            Response = new DialogResponse();    
        }
        
        public override DialogResponse Response { get; }

        public void UpdateIntent(Intent intent)
        {
            var directive = new DialogDirective {UpdatedIntent = intent};
            Response.Directives = new[] {directive};
        }
    }
    
    public class DialogResponse : ResponseBase<DialogDirective>
    {}

    public class DialogDirective
    {
        public string Type => "Dialog.Delegate";
        
        public Intent UpdatedIntent { get; set; }
    }
    
    
}