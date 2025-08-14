using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;
using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
        [Test]
        public void ProcessPayment_Should_ThrowException_When_PaymentIsNull()
        {
            var repo = new InvoiceRepository();
            var paymentProcessor = new InvoiceService(repo);

            Payment payment = null;

            var ex = Assert.Throws<ArgumentNullException>(() => paymentProcessor.ProcessPayment(payment));
            StringAssert.EndsWith("Payment cannot be null or empty", ex.Message);
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_PaymentAmountIsZero()
        {
            var repo = new InvoiceRepository();
            var paymentProcessor = new InvoiceService(repo);

            Payment payment = new Payment { Amount = -10 };

            var ex = Assert.Throws<ArgumentException>(() => paymentProcessor.ProcessPayment(payment));
            StringAssert.StartsWith("Payment amount cannot be negative", ex.Message);
        }

        [Test]
		public void ProcessPayment_Should_ThrowException_When_NoPaymentReference( )
		{
			var repo = new InvoiceRepository( );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( );
            var ex = Assert.Throws<ArgumentException>(() => paymentProcessor.ProcessPayment(payment));
            StringAssert.StartsWith("Found no invoice reference", ex.Message);
        }
        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoPaymentNeeded()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice(repo)
            {
                Amount = 0,
                AmountPaid = 0,
            };

            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

			var payment = new Payment() { Amount = 10, Reference ="abc"};

            var result = paymentProcessor.ProcessPayment(payment);
            Assert.AreEqual( "No payment needed", result );
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_InvoiceHasZeroAmountButContainspPayments()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice(repo)
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };

            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment() { Amount = 9, Reference = "abc" };

            var ex = Assert.Throws<InvalidOperationException>(() => paymentProcessor.ProcessPayment(payment));
            StringAssert.StartsWith("Invoice has zero amount but contains payments", ex.Message);
        }

        [Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var repo = new InvoiceRepository( );

			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 10,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 10
					}
				}
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoiceService( repo );

            var payment = new Payment() { Amount = 9, Reference = "abc" };

            var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual("Invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};
			repo.Add( invoice );

			var paymentProcessor = new InvoiceService( repo );

            var payment = new Payment() { Amount = 9, Reference = "abc" };

            var result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual("Payment exceeds the remaining invoice amount", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NonExistingHandlerByInvoiceType( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice( repo )
			{
				Amount = 5,
				AmountPaid = 0,
                Type = InvoiceType.Other, // Non-existing Handler
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
			repo.Add( invoice );

			var paymentProcessor = new InvoiceService( repo );

            var payment = new Payment() { Amount = 5, Reference = "abc" };

            var ex = Assert.Throws<NotSupportedException>(() => paymentProcessor.ProcessPayment(payment));
            StringAssert.StartsWith($"Unsupported invoice type: {invoice.Type}", ex.Message);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_InvoiceNowFullyPaid( )
		{
            var repo = new InvoiceRepository();
            var invoice = new Invoice(repo)
            {
                Amount = 5,
                AmountPaid = 0,
                Type = InvoiceType.Standard,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment() { Amount = 5, Reference = "abc" };

            var result = paymentProcessor.ProcessPayment(payment);
            Assert.AreEqual("Invoice is now fully paid", result);
        }

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentReceivedInvoiceNotFullyPaid( )
		{
            var repo = new InvoiceRepository();
            var invoice = new Invoice(repo)
            {
                Amount = 5,
                AmountPaid = 0,
                Type = InvoiceType.Standard,
                Payments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            repo.Add(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment() { Amount = 3, Reference = "abc" };

            var result = paymentProcessor.ProcessPayment(payment);
            Assert.AreEqual("Partial payment received, invoice not fully paid", result);
        }
	}
}