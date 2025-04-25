using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
  public partial class Form1 : Form
  {
    private Dictionary<Button, string> buttonFilePaths = new Dictionary<Button, string>();
    private string _ticketDetailsText;
    private ITicketFactory _ticketFactory;
    private string _airport1Path;
    private string _airport2Path;

    public Form1()
    {
      InitializeComponent();
      _ticketFactory = new TextTicketFactory();

      string basePath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
      _airport1Path = Path.Combine(basePath, "Airport1");
      _airport2Path = Path.Combine(basePath, "Airport2");

      Directory.CreateDirectory(_airport1Path);
      Directory.CreateDirectory(_airport2Path);
    }

    private void AppendTextWithScroll(string text)
    {
      richTextBox1.AppendText(text);
      richTextBox1.SelectionStart = richTextBox1.TextLength;
      richTextBox1.ScrollToCaret();
    }

    private void button_Click(object sender, EventArgs e)
    {
      Button clickedButton = (Button)sender;
      Panel parentPanel = (Panel)clickedButton.Parent;

      _ticketDetailsText = "";

      foreach (Control control in parentPanel.Controls)
      {
        if (control is Label)
        {
          Label label = (Label)control;
          string labelText = label.Text;
          _ticketDetailsText += labelText + "\n";
        }
      }

      if (clickedButton.Text == "Забронировать")
      {
        clickedButton.Text = "Снять бронь";
        clickedButton.BackColor = Color.LightCoral;

        Random random = new Random();
        string fileName = $"ticket_{random.Next()}.txt";
        string filePath = Path.Combine(_airport1Path, fileName);

        try
        {
          ITicket ticket = _ticketFactory.CreateTicket(_ticketDetailsText);
          ticket.SaveTicket(filePath);

          AppendTextWithScroll($"\nФайл {fileName} создан по пути: {filePath}\n");
          buttonFilePaths[clickedButton] = filePath;
        }
        catch (Exception error)
        {
          AppendTextWithScroll($"Ошибка: {error}\n");
          return;
        }
      }
      else
      {
        clickedButton.Text = "Забронировать";
        clickedButton.BackColor = Color.White;

        if (buttonFilePaths.ContainsKey(clickedButton))
        {
          string filePathToDelete = buttonFilePaths[clickedButton];
          string fileName = Path.GetFileName(filePathToDelete);

          try
          {
            ITicket ticket = _ticketFactory.CreateTicket(_ticketDetailsText);
            ticket.DeleteTicket(filePathToDelete);

            AppendTextWithScroll($"\nФайл {fileName} удален\n");
            buttonFilePaths.Remove(clickedButton);
          }
          catch (FileNotFoundException error)
          {
            AppendTextWithScroll($"\nФайл не найден при удалении: {error.Message}\n");
          }
          catch (Exception error)
          {
            AppendTextWithScroll($"\nОшибка при удалении файла: {error.Message}\n");
          }
        }
        else
        {
          AppendTextWithScroll("\nФайл для этой кнопки не найден.\n");
        }
      }
    }

    private void SyncDirectories()
    {
      AppendTextWithScroll("\nНачало синхронизации директорий...\n");
      SyncDirectory(_airport1Path, _airport2Path, "Airport1 -> Airport2");
      SyncDirectory(_airport2Path, _airport1Path, "Airport2 -> Airport1");
      AppendTextWithScroll("Синхронизация завершена.\n");
    }

    private void SyncDirectory(string sourcePath, string targetPath, string syncDirection)
    {
      AppendTextWithScroll($"\nСинхронизация: {syncDirection}\n");

      var sourceFiles = Directory.GetFiles(sourcePath).Select(file => new FileInfo(file)).ToList();
      var targetFiles = Directory.GetFiles(targetPath).Select(file => new FileInfo(file)).ToList();

      var sourceDict = sourceFiles.ToDictionary(file => file.Name, file => file);
      var targetDict = targetFiles.ToDictionary(file => file.Name, file => file);

      foreach (var sourceFile in sourceFiles)
      {
        if (targetDict.TryGetValue(sourceFile.Name, out FileInfo targetFile))
        {
          if (sourceFile.LastWriteTime > targetFile.LastWriteTime)
          {
            File.Copy(sourceFile.FullName, targetFile.FullName, true);
            AppendTextWithScroll($"• Файл \"{sourceFile.Name}\" изменен\n");
          }
        }
        else
        {
          string destPath = Path.Combine(targetPath, sourceFile.Name);
          File.Copy(sourceFile.FullName, destPath);
          AppendTextWithScroll($"• Файл \"{sourceFile.Name}\" создан\n");
        }
      }

      foreach (var targetFile in targetFiles)
      {
        if (!sourceDict.ContainsKey(targetFile.Name))
        {
          File.Delete(targetFile.FullName);
          AppendTextWithScroll($"• Файл \"{targetFile.Name}\" удален\n");
        }
      }
    }

    private void btnSync_Click(object sender, EventArgs e)
    {
      SyncDirectories();
    }
  }
}