using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Domain.Constants;

namespace MyCRM.Domain.Entities
{
    public class WorkTask
    {
        public int Id { get; set; }
        public int DealId { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public WorkTaskStatus Status { get; set; }
        public Deal? Deal { get; set; }
        public WorkTask(int dealId, string title, DateTime dueDate)
        {
            DealId = dealId;
            Title = title;
            DueDate = dueDate;
            Status = WorkTaskStatus.InProgress;
        }
    }
}