using System;

namespace CompanyName.ProjectName.WebApi.Models.Message
{
    public class ReadMessage
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public int ChannelId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
    }
}
