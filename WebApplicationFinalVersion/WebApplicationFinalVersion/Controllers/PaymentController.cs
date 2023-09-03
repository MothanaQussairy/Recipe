using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.Net.Mail;
using System.Net;
using WebApplicationFinalVersion.Models;

namespace WebApplicationFinalVersion.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ModelContext _context;
        private static decimal?  _amount;
        public PaymentController(ModelContext context)
        {
            _context = context;
        }
        
        public IActionResult SetAmount(decimal id)
        {
            _amount = _context.Reciperequests.Where(r => r.Requestid == id).FirstOrDefault().Cost;
            return RedirectToAction("Index");
        }
        public IActionResult Index(decimal id)
        {
              
            return View();
        }

        public IActionResult PayNow(string cardNumber, string cvv, string nameOnCard)
        {
            if (ExistCard(cardNumber, cvv, nameOnCard))
            {
                var card = _context.Payments.FirstOrDefault(c => c.CardNumber.Equals(cardNumber));
                bool isEnough = card.Amount >= _amount;

                if (isEnough)
                {
                    card.Amount -= _amount;
                    SendInvoiceEmail(cardNumber);
                    Edit(card);
                    TempData["PaymentMessage"] = "Payment successful.";
                }
                else
                {
                    TempData["PaymentMessage"] = "Insufficient funds.";
                }
            }
            else
            {
                TempData["PaymentMessage"] = "Invalid card details.";
            }

            return RedirectToAction("Index");
        }
        private bool ExistCard(string card_number, string cvv, string name_on_card)
        {
            return _context.Payments.Any(card =>
                card.CardNumber == card_number &&
                card.CardCvc == cvv &&
                card.Column1 == name_on_card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Payment payment)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardExists(payment.PaymentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            return View("Index");
        }
        private bool CardExists(decimal id)
        {
            return (_context.Payments?.Any(e => e.PaymentId == id)).GetValueOrDefault();
        }
        public void SendInvoiceEmail(string cardNumber)
        {
            var userId = HttpContext.Session.GetInt32("id");
            var user = _context.Users.FirstOrDefault(u => u.Userid == userId);

            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                // Handle the case when user information or email is not available
                return;
            }

            string customerEmail = user.Email;

            using (SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential("hamza1082001@outlook.com", "1082001h");
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage("hamza1082001@outlook.com", customerEmail);
                mailMessage.Subject = "Invoice";

                // Create a more professional-looking invoice content
                string invoiceHtml = $@"
            <html>
                <body>
                    <h2>Invoice</h2>
                    <p>Dear {user.Username}</p>
                    <p>Thank you for your recent purchase. Please find attached the invoice for the amount of ${_amount:F2}.</p>
                    <p>Date: {DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt")}</p>
                    <p>Card Number: {cardNumber}</p>
                    <p>If you have any questions or concerns, please don't hesitate to contact our customer support.</p>
                    <p>Best Regards,</p>
                    <p>Tasty Recipe</p>
                </body>
            </html>";

                // Convert invoice HTML to PDF
                HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
                PdfDocument pdf = htmlToPdfConverter.Convert(invoiceHtml, "");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    pdf.Save(memoryStream);
                    pdf.Close(true);

                    // Attach the PDF to the email
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    mailMessage.Attachments.Add(new Attachment(memoryStream, "invoice.pdf", "application/pdf"));

                    // Send the email
                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception appropriately (log, notify, etc.)
                        Console.WriteLine("An error occurred while sending the email: " + ex.Message);
                    }
                }
            }
        }

    }
}
