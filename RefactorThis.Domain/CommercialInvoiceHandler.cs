using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    public class CommercialInvoiceHandler : IInvoiceTypeHandler
    {
        private const decimal TaxRate = 0.14m; // Moved magic number to constant

        public void ApplyPayment(Invoice invoice, Payment payment)
        {
            invoice.AmountPaid += payment.Amount;
            invoice.Payments.Add(payment);
            invoice.TaxAmount += CalculateTax(payment.Amount);
        }

        public decimal CalculateTax(decimal paymentAmount) => paymentAmount * TaxRate;
    }
}
