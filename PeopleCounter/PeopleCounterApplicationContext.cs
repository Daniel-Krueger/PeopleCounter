using System;
using System.Configuration;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PeopleCounter
{
    public class PeopleCounterApplicationContext : System.Windows.Forms.ApplicationContext
    {
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderMenuItem;
        private Dictionary<NotifyIcon, CounterType> iconMapping = new Dictionary<NotifyIcon, CounterType>();

        private System.ComponentModel.IContainer components = null;
        private readonly string csvHeader;
        private readonly string csvDelimiter;
        private readonly string loggedDateTimeFormat;

        private string csvFolderPath;
        private StreamWriter yearFile;
        private StreamWriter monthFile;
        private bool writeMonthlyFile = false;
        public PeopleCounterApplicationContext()
        {
            InitializeComponent();

            csvHeader = ConfigurationManager.AppSettings["csvHeader"];
            csvDelimiter = ConfigurationManager.AppSettings["csvDelimiter"];
            loggedDateTimeFormat = ConfigurationManager.AppSettings["loggedDateTimeFormat"];
            csvFolderPath = ConfigurationManager.AppSettings["folder"];
            writeMonthlyFile = ConfigurationManager.AppSettings["monthFile"] == "true";
            var counterTypeSection = (CounterTypeSection)ConfigurationManager.GetSection("counterTypeSection");
            for (var i = 0; i < counterTypeSection.CounterTypes.Count; i++)
            {
                CounterTypeElement counterTypeElement = counterTypeSection.CounterTypes[i];
                var icon = new System.Windows.Forms.NotifyIcon(this.components);
                CounterType counterType = new CounterType()
                {
                    BackgroundColor = Color.FromName(counterTypeElement.BackGroundColor),
                    ForegroundColor = new SolidBrush(Color.FromName(counterTypeElement.ForeGroundColor)),
                    IconText = counterTypeElement.IconText,
                    CSVIdentifier = counterTypeElement.CsvIdentifier,
                    Icon = icon


                };
                iconMapping.Add(icon, counterType);

            }


            foreach (var icon in iconMapping.Keys)
            {
                var counterType = iconMapping[icon];

                icon.ContextMenuStrip = this.contextMenu;
                icon.Text = counterType.IconText;
                icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.countClick);
                icon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.countClick);

                UpdateIcon(counterType);
                icon.Visible = true;
            }
            EnsureFiles();
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.openFolderMenuItem,
                this.exitMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(121, 26);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(120, 22);
            this.exitMenuItem.Text = ConfigurationManager.AppSettings.Get("exitMenu");
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // openFolderMenuItem
            // 
            this.openFolderMenuItem.Name = "openFolderMenuItem";
            this.openFolderMenuItem.Size = new System.Drawing.Size(120, 22);
            this.openFolderMenuItem.Text = ConfigurationManager.AppSettings.Get("openFolderMenu");
            this.openFolderMenuItem.Click += new System.EventHandler(this.openFolderMenuItem_Click);
        }

        private void countClick(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                var counterType = iconMapping[(NotifyIcon)sender];
                IncreaseCounter(counterType);
            }
        }

        private void IncreaseCounter(CounterType type)
        {
            type.Counter++;
            UpdateIcon(type);

            string textToWrite = string.Concat(type.CSVIdentifier, csvDelimiter, DateTime.Now.ToString(loggedDateTimeFormat));
            WriteToFile(yearFile, textToWrite);
            if (writeMonthlyFile)
            {
                WriteToFile(monthFile, textToWrite);
            }
        }

        private void UpdateIcon(CounterType type)
        {
            type.Icon.Text = $"{type.IconText}{type.Counter}";
            // Load the original image
            Bitmap bmp = new Bitmap(32, 32);

            RectangleF rectf = new RectangleF(0, 0, bmp.Width, bmp.Height);
            // Create graphic object that will draw onto the bitmap
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(type.BackgroundColor);
            // ------------------------------------------
            // Ensure the best possible quality rendering
            // ------------------------------------------
            // The smoothing mode specifies whether lines, curves, and the edges of filled areas use smoothing (also called antialiasing). 
            // One exception is that path gradient brushes do not obey the smoothing mode. 
            // Areas filled using a PathGradientBrush are rendered the same way (aliased) regardless of the SmoothingMode property.
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // The interpolation mode determines how intermediate values between two endpoints are calculated.
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Use this property to specify either higher quality, slower rendering, or lower quality, faster rendering of the contents of this Graphics object.
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // This one is important
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;


            // Create string formatting options (used for alignment)
            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // Draw the text onto the image
            g.DrawString(type.Counter.ToString(), new Font("Tahoma", 16), type.ForegroundColor, rectf, format);


            // Flush all graphics changes to the bitmap
            g.Flush();

            // Now save or use the bitmap
            type.Icon.Icon = Icon.FromHandle(bmp.GetHicon());
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose(true);
            Program.Quit();
        }

        private void EnsureFiles()
        {
            DirectoryInfo csvFolder;
            if (!System.IO.Directory.Exists(csvFolderPath))
            {
                csvFolder = System.IO.Directory.CreateDirectory(csvFolderPath);
            }
            else
            {
                csvFolder = new DirectoryInfo(csvFolderPath);
            }
            string yearFileName = System.IO.Path.Combine(csvFolder.FullName, $"{DateTime.Now.Year}.csv");
            bool yearFileExists = File.Exists(yearFileName);
            yearFile = File.AppendText(yearFileName);
            if (!yearFileExists)
            {
                WriteToFile(yearFile, csvHeader);
            }
            if (writeMonthlyFile)
            {
                string monthFileName = System.IO.Path.Combine(csvFolder.FullName, $"{DateTime.Now.Year}-{DateTime.Now.Month.ToString("00")}.csv");
                bool monthFileNameExists = File.Exists(yearFileName);
                if (!monthFileNameExists)
                {
                    monthFile = File.AppendText(monthFileName);

                    if (writeMonthlyFile)
                    {
                        WriteToFile(monthFile, csvHeader);
                    }
                }
            }
        }

        private void WriteToFile(StreamWriter file, string text)
        {
            //byte[] info = new UTF8Encoding(true).GetBytes(text + "\r\n");
            file.Write(text + "\r\n");
            file.Flush();
        }

        private void openFolderMenuItem_Click(object sender, EventArgs e)
        {

            if (Directory.Exists(this.csvFolderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = this.csvFolderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", this.csvFolderPath));
            }
        }

        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (yearFile != null)
            {
                yearFile.Dispose();
            }

            if (monthFile != null)
            {
                monthFile.Dispose();
            }
            base.Dispose(disposing);

        }

    }
}
