using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    /// <summary>
    /// Interface for handling different types of invoices.
    /// Open/Closed Principle: New invoice types can be added without modifying existing code.
    /// </summary>
    public interface IInvoiceTypeHandler
    {
        void ApplyPayment(Invoice invoice, Payment payment);
        decimal CalculateTax(decimal paymentAmount);
    }
}
