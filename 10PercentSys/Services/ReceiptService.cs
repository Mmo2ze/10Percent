using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using _10PercentSys.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Maui.Layouts;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Image = System.Drawing.Image;
using Size = _10PercentSys.Models.Size;

public class ReceiptService
{
    private const string PrinterName = "XP-80C"; // Change to your printer's name

    public void PrintReceipt(Order order)
    {
        try
        {
            using var printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = PrinterName;
            printDoc.PrintPage += (sender, e) => PrintReceiptPage(e, order);
            printDoc.Print();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Printer error: {ex.Message}");
        }
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    private void PrintReceiptPage(PrintPageEventArgs e, Order order)
    {
        float yPos = 10;

        int leftMargin = 10;
        Font titleFont = new Font("Courier New", 14, FontStyle.Bold);
        Font font = new Font("Courier New", 10, FontStyle.Bold);
        Font boldFont = new Font("Courier New", 12, FontStyle.Bold);
        Font cFont = new Font("Courier New", 6, FontStyle.Bold);
        Font tFont = new Font("Courier New", 9, FontStyle.Bold);

        Brush brush = Brushes.Black;
        Graphics g = e.Graphics;
        Font smallFont = new Font("Courier New", 9, FontStyle.Bold);
        // Branding
        // g.DrawString("10 Percent Coffee", titleFont, brush, leftMargin + 50, yPos);
        yPos += 20;
        string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "imgs", "10 LOGO 1.png");

        // Load and draw the logo
        try
        {
            if (File.Exists(logoPath))
            {
                using var logo = Image.FromFile(logoPath);
                g.DrawImage(logo, leftMargin + 20, yPos, 210, 110); // Adjust position and size
                yPos += 120; // Move down after logo
            }
            else
            {
                Console.WriteLine("Logo file not found: " + logoPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading logo: {ex.Message}");
        }

        g.DrawString("==== RECEIPT ====", titleFont, brush, leftMargin + 25, yPos);
        yPos += 30;
        g.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), font, brush, leftMargin, yPos);
        g.DrawString(DateTime.Now.ToString("ORDER#" + order.Id), boldFont, brush, leftMargin + 160, yPos);
        g.DrawRectangle(new Pen(Color.Black, 2), leftMargin + 160, yPos, (order.Id.ToString().Length + 5) * 12 + 5, 20);
        yPos += 30;
        g.DrawString("----------------------------------", font, brush, leftMargin, yPos);
        var tableYStart = yPos;
        yPos += 20;

        // Table Header
        g.DrawString("Item", boldFont, brush, leftMargin, yPos);
        g.DrawString("Qty", boldFont, brush, leftMargin + 130, yPos);
        g.DrawString("Price", boldFont, brush, leftMargin + 180, yPos);
        yPos += 20;
        g.DrawString("----------------------------------", font, brush, leftMargin, yPos);
        yPos += 20;

        foreach (var item in order.OrderProducts)
        {
            if (item.Name.Length > 15)
            {
                float oldYpos = yPos;
                var NameArray = item.Name.Split(" ");
                List<string> NewNameArray = [];
                if (NameArray.Length < 3)
                {
                    g.DrawString(item.Name + item.StringSize, tFont, brush, leftMargin, yPos);
                    g.DrawString(item.Quantity.ToString(), font, brush, leftMargin + 130, yPos);
                    g.DrawString(item.Total.ToString("F2"), font, brush, leftMargin + 180, yPos);
                    g.DrawString(" EGP", cFont, brush, leftMargin + item.Total.ToString("F2").Length * 7 + 185,
                        yPos + 5);
                    yPos += 20;
                }
                else
                {
                    for (int i = 0; i < NameArray.Length; i++)
                    {
                        if (NameArray.Length % 2 == 0)
                        {
                            NewNameArray.Add(NameArray[i] + " " + NameArray[i + 1]);
                            i++;
                        }
                        else
                        {
                            if (i == NameArray.Length - 1)
                                NewNameArray.Add(NameArray[i] + item.StringSize);
                            else if (i % 2 == 0)
                                NewNameArray.Add(NameArray[i] + " " + NameArray[i + 1]);
                        }
                    }

                    foreach (var name in NewNameArray)
                    {
                        g.DrawString(name, smallFont, brush, leftMargin, yPos);
                        yPos += 10;
                    }


                    g.DrawString(item.Quantity.ToString(), font, brush, leftMargin + 130, (yPos - 10 + oldYpos) / 2);
                    g.DrawString(item.Total.ToString("F2"), font, brush, leftMargin + 180,
                        (yPos - 10 + oldYpos) / 2);
                    g.DrawString(" EGP", cFont, brush, leftMargin + item.Total.ToString("F2").Length * 7 + 185,
                        (yPos - 10 + oldYpos) / 2 + 5);
                    yPos += 10;
                }
            }
            else
            {
                var myFont = item.Name.Length > 10 ? smallFont : font;

                g.DrawString(item.Name + item.StringSize, myFont, brush, leftMargin, yPos);
                g.DrawString(item.Quantity.ToString(), font, brush, leftMargin + 130, yPos);
                g.DrawString(item.Total.ToString("F2"), font, brush, leftMargin + 180, yPos);
                g.DrawString(" EGP", cFont, brush, leftMargin + item.Total.ToString("F2").Length * 7 + 185,
                    yPos + 5);
                yPos += 20;
            }
        }


        g.DrawLine(new Pen(Color.Black), leftMargin + 125, tableYStart + 8, leftMargin + 125, yPos + 8);
        g.DrawLine(new Pen(Color.Black), leftMargin + 175, tableYStart + 8, leftMargin + 175, yPos + 8);
        g.DrawString("----------------------------------", font, brush, leftMargin, yPos);
        yPos += 20;
        g.DrawString($"Subtotal: {CalculateSubtotal(order.OrderProducts):F2} EGP", font, brush, leftMargin, yPos);
        yPos += 20;
        g.DrawString(
            $"Discount: {order.Discount}% (-{CalculateDiscountAmount(order.OrderProducts, order.Discount):F2} EGP)",
            font, brush, leftMargin, yPos);
        yPos += 20;
        g.DrawString($"Tax: {CalculateTaxAmount(order.OrderProducts, order):F2} EGP", font, brush, leftMargin, yPos);
        yPos += 20;
        g.DrawString($"TOTAL: {CalculateTotal(order.OrderProducts, order):F2} EGP",
            new Font("Courier New", 14, FontStyle.Bold),
            brush, leftMargin + 40, yPos);
        yPos += 30;

        g.DrawString("Thank you for visiting 10 Percent Coffee!", smallFont, brush, leftMargin, yPos);
    }


    private decimal CalculateSubtotal(List<OrderProduct> orderItems) => orderItems.Sum(i => i.Price * i.Quantity);

    private decimal CalculateDiscountAmount(List<OrderProduct> orderItems, int discount) =>
        CalculateSubtotal(orderItems) * (discount / 100m);

    private decimal CalculateTaxAmount(List<OrderProduct> orderItems, Order order)
    {
        decimal subtotal = CalculateSubtotal(orderItems) - CalculateDiscountAmount(orderItems, order.Discount);
        decimal taxRate = (order.Vat10 ? 10m : 0) + (order.Vat14 ? 14m : 0);
        return subtotal * (taxRate / 100m);
    }

    private decimal CalculateTotal(List<OrderProduct> orderItems, Order order)
    {
        return CalculateSubtotal(orderItems) - CalculateDiscountAmount(orderItems, order.Discount) +
               CalculateTaxAmount(orderItems, order);
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public void PrintSummary(List<Order> orders, DateTime startDate, DateTime endDate, decimal grandTotal)
    {
        Dictionary<ProductSummary, int> productQuantities = new Dictionary<ProductSummary, int>();
        foreach (var order in orders)
        {
            foreach (var item in order.OrderProducts)
            {
                if (productQuantities.ContainsKey(item.Summary))
                {
                    productQuantities[item.Summary] += item.Quantity;
                }
                else
                {
                    productQuantities[item.Summary] = item.Quantity;
                }
            }
        }

        productQuantities = productQuantities.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    
        try
        {
            float yPos = 60;
            var printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = PrinterName;
            printDoc.PrintPage += (sender, e) =>
                PrintSummaryIntroPage(e, startDate, endDate);
            printDoc.PrintPage += (sender, e) =>
                PrintSummaryPage(e, productQuantities.Take(40).ToDictionary(), yPos);
            printDoc.Print();
            for (int i = 1; i <= productQuantities.Count / 40; i++)
            {
                yPos = 10;
                printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = PrinterName;
                var i1 = i;
                printDoc.PrintPage += (sender, e) =>
                    PrintSummaryPage(e, productQuantities.Skip(i1*40).Take(i1 + 1 * 40).ToDictionary(), yPos);
                printDoc.Print();

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Printer error: {ex.Message}");
        }
    }
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]

    private float PrintSummaryIntroPage(PrintPageEventArgs e,DateTime startDate, DateTime endDate)
    {
        float yPos = 10;
        Graphics g = e.Graphics;
        Font font = new Font("Courier New", 10, FontStyle.Bold);
        Font boldFont = new Font("Courier New", 12, FontStyle.Bold);
        int leftMargin = 10;
        Brush brush = Brushes.Black;
        Font smallFont = new Font("Courier New", 9, FontStyle.Bold);
        Font titleFont = new Font("Courier New", 14, FontStyle.Bold);
        g.DrawString("==== Summary ====", titleFont, brush, leftMargin + 25, yPos);
        yPos += 30;
        g.DrawString("From: " + startDate.ToString("yyyy-MM-dd HH:mm"), font, brush, leftMargin, yPos);
        yPos += 20;
        g.DrawString("To: " + endDate.ToString("yyyy-MM-dd HH:mm"), font, brush, leftMargin, yPos);
        return yPos;
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    private float PrintSummaryPage(PrintPageEventArgs e, Dictionary<ProductSummary, int> productQuantities, float yPos)
    {
        int leftMargin = 10;
        Font titleFont = new Font("Courier New", 14, FontStyle.Bold);
        Font font = new Font("Courier New", 10, FontStyle.Bold);
        Font boldFont = new Font("Courier New", 12, FontStyle.Bold);
        Font cFont = new Font("Courier New", 6, FontStyle.Bold);
        Font tFont = new Font("Courier New", 9, FontStyle.Bold);

        Brush brush = Brushes.Black;
        Graphics g = e.Graphics;
        Font smallFont = new Font("Courier New", 9, FontStyle.Bold);
        // Branding
        // g.DrawString("10 Percent Coffee", titleFont, brush, leftMargin + 50, yPos);
        yPos += 20;


        yPos += 20;
        foreach (var product in productQuantities.OrderByDescending(x => x.Value))
        {
            if (product.Key.Name.Length < 25)
            {
                g.DrawString($"{product.Key.Name}", font, brush, leftMargin, yPos);
            }
            else
                g.DrawString($"{product.Key.Name}", smallFont, brush, leftMargin, yPos);

            g.DrawString($"X{product.Value}", boldFont, brush, leftMargin + 220, yPos);
            yPos += 20;
        }

        return yPos;
    }
}