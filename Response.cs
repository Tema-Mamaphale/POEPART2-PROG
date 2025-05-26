namespace Chatbot_Luna
{
    public class Response
    {
        public string Definition { get; set; }
        public string Tip { get; set; }       // Added Tip property
        public string Example { get; set; }

        public Response(string definition, string tip, string example)
        {
            Definition = definition;
            Tip = tip;
            Example = example;
        }
    }
}
