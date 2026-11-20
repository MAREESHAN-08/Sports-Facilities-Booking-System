using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SportsBookingApp.ViewModels;

namespace SportsBookingApp.Services;

public static class ReceiptPdfGenerator
{
    public static byte[] Generate(ReceiptViewModel r)
    {
        byte[] qrBytes = BuildQr(
            "Ref: " + r.Reference +
            " | Facility: " + r.FacilityName +
            " | Date: " + r.BookingDate.ToString("dd MMM yyyy") +
            " | Time: " + Time(r.StartTime) + "-" + Time(r.EndTime) +
            " | Total: " + r.TotalPrice.ToString("0.00"));

        return QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(1.1f, Unit.Centimetre);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(10f));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("ARENAPULSE SPORTS COUNCIL").Bold().FontSize(13).FontColor("#0d6e63");
                        c.Item().Text("Community Sports & Arena Booking Gateway").FontSize(9).FontColor("#555555");
                        c.Item().PaddingTop(6).Text("OFFICIAL BOOKING PASS & RECEIPT").Bold().FontSize(13).FontColor("#F5A623");
                    });
                    row.ConstantItem(70).Image(qrBytes);
                });

                page.Content().PaddingTop(12).Column(content =>
                {
                    content.Spacing(5);

                    content.Item().Text("Reference: " + r.Reference).Bold().FontSize(12).FontColor("#0d6e63");
                    content.Item().Text("Status: " + r.Status).Bold();
                    content.Item().Text("Issued: " + r.CreatedOn.ToString("dd MMM yyyy HH:mm"));

                    content.Item().Text("Booking Details").Bold().FontSize(11).FontColor("#0d6e63");
                    content.Item().Text("Player Name: " + r.MemberName);
                    content.Item().Text("Email: " + r.MemberEmail);
                    content.Item().Text("Facility: " + r.FacilityName);
                    content.Item().Text("Category: " + r.TypeName);
                    content.Item().Text("Location: " + r.Location);
                    content.Item().Text("Venue Contact: " + (string.IsNullOrEmpty(r.ContactPhone) ? "-" : r.ContactPhone));
                    content.Item().Text("Date: " + r.BookingDate.ToString("dddd, dd MMMM yyyy"));
                    content.Item().Text("Time Slot: " + Time(r.StartTime) + " - " + Time(r.EndTime));
                    content.Item().Text("Duration: " + r.Hours.ToString("0.#") + " hour(s)");
                    content.Item().Text("Rate: £" + r.HourlyRate.ToString("0.00") + " per hour");

                    content.Item().PaddingTop(8).Background("#0d6e63").Padding(10).Row(row =>
                    {
                        row.RelativeItem().Text("TOTAL AMOUNT PAID / DUE").Bold().FontColor("#FFFFFF");
                        row.AutoItem().Text("£" + r.TotalPrice.ToString("0.00")).Bold().FontSize(14).FontColor("#FFFFFF");
                    });

                    content.Item().PaddingTop(6).Text("Please present this digital pass (printed or on your mobile device) at the arena reception before your slot begins. Enjoy your match!").FontSize(8.5f).FontColor("#666666");
                });

                page.Footer().AlignCenter().Text("ArenaPulse — Empowering Active Communities & Champions");
            });
        }).GeneratePdf();
    }

    private static string Time(TimeSpan t)
    {
        return t.ToString(@"hh\:mm");
    }

    private static byte[] BuildQr(string text)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qr = new PngByteQRCode(data);
        return qr.GetGraphic(8);
    }
}