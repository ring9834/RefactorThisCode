using System;
using System.Collections.Generic;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoiceService
	{
		private readonly InvoiceRepository _invoiceRepository;
        private readonly Dictionary<InvoiceType, IInvoiceTypeHandler> _handlers;

        public InvoiceService( InvoiceRepository invoiceRepository )
		{
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
            _handlers = new Dictionary<InvoiceType, IInvoiceTypeHandler>
            {
                { InvoiceType.Standard, new StandardInvoiceHandler() },
                { InvoiceType.Commercial, new CommercialInvoiceHandler() }
            };
        }

		public string ProcessPayment( Payment payment )
		{
            if (payment == null) throw new ArgumentNullException("Payment cannot be null or empty", nameof(payment));
            if (payment.Amount < 0) throw new ArgumentException("Payment amount cannot be negative", nameof(payment));
            if (string.IsNullOrEmpty(payment.Reference)) throw new ArgumentException("Found no invoice reference", nameof(payment));

            var invoice = _invoiceRepository.GetInvoice(payment.Reference)
                ?? throw new InvalidOperationException($"No invoice found for reference {payment.Reference}");

            if (invoice.Amount == 0)
            {
                return invoice.Payments.Count == 0
                    ? "No payment needed"
                    : throw new InvalidOperationException("Invoice has zero amount but contains payments");
            }

            decimal remainingAmount = invoice.Amount - invoice.AmountPaid;
            if (remainingAmount == 0)
                return "Invoice was already fully paid";
            if (payment.Amount > remainingAmount)
                return "Payment exceeds the remaining invoice amount";

            if (!_handlers.TryGetValue(invoice.Type, out var handler))
                throw new NotSupportedException($"Unsupported invoice type: {invoice.Type}");

            handler.ApplyPayment(invoice, payment);
            invoice.Save();

            return payment.Amount == remainingAmount
                ? "Invoice is now fully paid"
                : "Partial payment received, invoice not fully paid";
        }
    }
}