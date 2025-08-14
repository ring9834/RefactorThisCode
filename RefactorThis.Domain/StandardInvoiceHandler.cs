using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    public class StandardInvoiceHandler : IInvoiceTypeHandler
    {
        public void ApplyPayment(Invoice invoice, Payment payment)
        {
            invoice.AmountPaid += payment.Amount;
            invoice.Payments.Add(payment);
        }

        public decimal CalculateTax(decimal paymentAmount) => 0m; // No tax for Standard
    }
}
