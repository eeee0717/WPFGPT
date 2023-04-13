using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using OpenAI.Chat;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace WPFGPT.Models;

public class Pdf
{
    public void GeneratePdf(string? keyWords, List<ChatPrompt> chatPrompts)
    {
        var chatStrings = this.HandlePrompts(chatPrompts);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


        
        int marginLeftRight = 30; //左右边距
        int marginTopBottom = 30; //上下边距
        int curX = marginLeftRight;
        int curY = marginTopBottom;
        PdfDocument document = new PdfDocument();

        PdfPage page = document.AddPage();
        page.Size = PageSize.A4;
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font;
        string fileName = "ChatPdf";
        
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            RestoreDirectory = true
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            font = new XFont("黑体", 40, XFontStyle.Bold);
            fileName =
                saveFileDialog.FileName.Substring(saveFileDialog.FileName.LastIndexOf("\\", StringComparison.Ordinal) +
                                                  1);
            if (fileName.Contains('.'))
            {
                fileName = fileName.Substring(0, fileName.IndexOf('.'));
            }
            gfx.DrawString(fileName, font, XBrushes.Black,
                new XRect(marginLeftRight, marginTopBottom, page.Width - 2 * curX, 80),
                XStringFormats.Center);
            
        }
        font = new XFont("华文宋体", 20, XFontStyle.Regular);
        curY += 80;
        string date = $"Date: {DateTime.Now:D}";
        gfx.DrawString(date, font, XBrushes.Black, new XRect(curX, curY, 100, 20), XStringFormats.CenterLeft);
        curY += 25;
        if (keyWords is null)
        {
            return;
        }

        gfx.DrawString("Key Words:", font, XBrushes.Black, new XRect(curX, curY, page.Width - 2 * curX, 20),
            XStringFormats.CenterLeft);
        string[] words = keyWords.Split(' ');
        curX += 40;
        words = words.Where(s => !string.IsNullOrEmpty(s)).ToArray();
        foreach (var word in words)
        {
            curY += font.Height;
            gfx.DrawString(word, font, XBrushes.Black, new XRect(curX, curY, 100, 20), XStringFormats.CenterLeft);
        }

        curY += 25;
        XPen pen = new XPen(XColor.FromKnownColor(XKnownColor.Black), 1);
        gfx.DrawLine(pen, curX, curY, page.Width - curX, curY + 2);
        font = new XFont("华文宋体", 14, XFontStyle.Regular);
        curY += 5;
        foreach (var chatString in chatStrings)
        {
            IEnumerable<string> lines = this.JudgeWordsLength(chatString);
            foreach (var line in lines)
            {
                gfx.DrawString(line, font, XBrushes.Black, new XRect(curX, curY, 100, 16), XStringFormats.CenterLeft);
                curY += font.Height;
                if (curY > page.Height - marginTopBottom)
                {
                    curY = marginTopBottom + 30;
                    var newPage = document.AddPage();
                    gfx = XGraphics.FromPdfPage(newPage);

                }
            }
        }

        
        document.Save($"{fileName}.pdf");
    }

    private string[] HandlePrompts(List<ChatPrompt> chatPrompts)
    {
        var chatString = "";
        foreach (var chatPrompt in chatPrompts)
        {
            var role = $"{chatPrompt.Role}:" + "\n";
            var content = "\t" + $"{chatPrompt.Content}" + "\n";
            chatString += $"{role}{content}";
        }

        string[] chatStrings = chatString.Split('\n');
        return chatStrings;
    }

    private IEnumerable<string> JudgeWordsLength(string chatString)
    {
        var words = chatString.Split(' ');
        var lines = new List<string>();
        var currentLine = new StringBuilder();
        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > 85)
            {
                lines.Add(currentLine.ToString().Trim());
                currentLine.Clear();
            }

            currentLine.Append(word + " ");
        }

        lines.Add(currentLine.ToString().Trim());
        return lines;
    }
}