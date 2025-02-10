﻿namespace Ekip2.Domain.Core.Base;
public class BaseEntity : IUpdatableEntity
{
    public Guid Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Status Status { get; set; }
}
