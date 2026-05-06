using System;
using System.Collections.Generic;

namespace PropertyPal.Api.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int LeaseId { get; set; }

    public decimal Amount { get; set; }

    public int Status { get; set; }

    public DateTime DueDate { get; set; }

    public string TransactionReference { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidDate { get; set; }

    public virtual Lease Lease { get; set; } = null!;
}
