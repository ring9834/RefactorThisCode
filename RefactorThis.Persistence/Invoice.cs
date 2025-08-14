using System;
using System.Collections.Generic;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;
		public Invoice( InvoiceRepository repository )
		{
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Payments = new List<Payment>(); // Initialize to avoid null
        }

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public decimal TaxAmount { get; set; }
		public List<Payment> Payments { get; set; }
		
		public InvoiceType Type { get; set; }
	}

	public enum InvoiceType
	{
		Standard,
		Commercial,
		Other
	}
}