﻿using System;

namespace CompanyName.ProjectName.Repository.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
    }
}
