using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace DriveSize
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                MainTask();
            }
            catch 
            {
                
               
            }
          



        }
        protected string GetDriveeKind(DriveInfo Dir)
        {
            switch (Dir.DriveType)
            {
                case DriveType.CDRom:
                    return "CdRom";
                case DriveType.Fixed:
                    return "Local";
                case DriveType.Network:
                    return "Network";
                case DriveType.NoRootDirectory:
                    return "NoRootDirectory";
                case DriveType.Ram:
                    return "Ram";
                case DriveType.Removable:
                    return "Removable";
                case DriveType.Unknown:
                    return "Unkwon";
                default:
                    return "---";
            }
        }
        protected Bitmap GetDriveeIcon(DriveInfo Dir)
        {
            switch (Dir.DriveType)
            {
                case DriveType.CDRom:
                    return DriveSize.Properties.Resources.HDD;
                case DriveType.Fixed:
                    return DriveSize.Properties.Resources.HDD2;
                case DriveType.Network:
                    return DriveSize.Properties.Resources.NET2;
                case DriveType.NoRootDirectory:
                    return DriveSize.Properties.Resources.HDD;
                case DriveType.Ram:
                    return DriveSize.Properties.Resources.HDD;
                case DriveType.Removable:
                    return DriveSize.Properties.Resources.HDD;
                case DriveType.Unknown:
                    return DriveSize.Properties.Resources.HDD;
                default:
                    return DriveSize.Properties.Resources.HDD;
            }
        }
        protected string FormatBytes(long bytes)
        {
            string Value = "";
            try
            {
                string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
                int i = 0;
                double dblSByte = bytes;
                if (bytes > 1024)
                {
                    for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024)
                    {
                        dblSByte = bytes / 1024.0;
                    }
                }

                Value = String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
            }
            catch 
            {
                
               
            }
          

            return Value;
        }
        protected string GetDrivePercent(long Total, long Value)
        {
            string Valuess = "";
            try
            {
                 Valuess = String.Format("{0:0.0}", (double.Parse(Value.ToString()) * 100) / double.Parse(Total.ToString())).Replace(".0", "") + " %";
            }
            catch 
            {
                
            }

            return Valuess;
        }
        protected void MainTask()
        {
            timer1.Enabled = false;
            try
            {
                string ConfigPath2 = Path.GetDirectoryName(Application.ExecutablePath) + "\\Config.xml";
                if (File.Exists(ConfigPath2))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ConfigPath2);

                    //Display all the book titles.
                    XmlNodeList elemList = doc.GetElementsByTagName("RefreshTime");
                    foreach (XmlNode item in elemList)
                    {

                        timer1.Interval = int.Parse(item.Attributes["Value"].Value.Trim());
                    }
                }

                flowLayoutPanel1.Controls.Clear();

                DriveInfo[] allDrives = DriveInfo.GetDrives();

                foreach (DriveInfo dirInfo in allDrives)
                {
                    if (dirInfo.DriveType == DriveType.Network)
                    {
                        string ExistConfig = "";
                        Color PrColor=Color.Green;
                        double YellowPercent=0;
                        double RedPercent=0;
                        string ConfigPath=Path.GetDirectoryName(Application.ExecutablePath) + "\\Config.xml";
                        if (File.Exists(ConfigPath))
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(ConfigPath);

                            //Display all the book titles.
                            XmlNodeList elemList = doc.GetElementsByTagName("Drive");
                            foreach (XmlNode item in elemList)
                            {
                                if (item.Attributes["Name"].Value.ToLower() == dirInfo.Name.ToLower())
                                {
                                   // MessageBox.Show(item.Attributes["Name"].Value.ToLower());
                                    ExistConfig = " ( Cnfg )";
                                    YellowPercent=double.Parse(item.Attributes["Yellow"].Value.Trim());
                                    RedPercent=double.Parse(item.Attributes["Red"].Value.Trim());
                                }
                            }
                        }

                        string DriveKind = GetDriveeKind(dirInfo);

                        GroupBox GrBx = new GroupBox();
                        GrBx.Width = 700;
                        GrBx.Height = 90;
                        GrBx.Text = dirInfo.Name + ExistConfig ;
                        GrBx.Margin = new System.Windows.Forms.Padding(2);
                        GrBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));




                        PictureBox Pic = new PictureBox();
                        Pic.Image = GetDriveeIcon(dirInfo);
                        Pic.Location = new System.Drawing.Point(5, 15);
                        Pic.Size = new System.Drawing.Size(50, 65);
                        Pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        GrBx.Controls.Add(Pic);

                        double Free = (double.Parse(dirInfo.TotalFreeSpace.ToString()) * 100) / double.Parse(dirInfo.TotalSize.ToString());                      
                        Brush PrBrush = Brushes.Green;
                        if (YellowPercent > 0 && RedPercent > 0)
                        {
                            if (Free > YellowPercent)
                            {
                                PrBrush = Brushes.Green;
                            }
                            else
                            {
                                if (Free <= YellowPercent && Free > RedPercent)
                                {
                                    PrBrush = Brushes.Yellow;
                                }
                                else
                                {
                                    if (Free <= RedPercent)
                                    {
                                        PrBrush = Brushes.Red;
                                    }
                                }
                            }
                        }


                        NewProgressBar Pr = new NewProgressBar(PrBrush);
                        Pr.Maximum = 100;
                        Pr.Value = int.Parse(((dirInfo.TotalSize - dirInfo.TotalFreeSpace) * 100 / dirInfo.TotalSize).ToString());
                        Pr.Width = 450;
                        Pr.Height = 45;
                        Pr.Location = new Point(60, 15);
                        Pr.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
                        GrBx.Controls.Add(Pr);






                        Label LblDriveTitle = new Label();
                        LblDriveTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblDriveTitle.ForeColor = Color.Black;
                        LblDriveTitle.BackColor = System.Drawing.Color.Transparent;
                        LblDriveTitle.Text = Pathing.GetUNCPath(dirInfo.Name);
                        LblDriveTitle.Location = new System.Drawing.Point(55, 60);
                        LblDriveTitle.Width = 400;
                        LblDriveTitle.Height = 40;
                        GrBx.Controls.Add(LblDriveTitle);
                        LblDriveTitle.BringToFront();


                        Label LblTotalTitle = new Label();
                        LblTotalTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblTotalTitle.ForeColor = Color.Red;
                        LblTotalTitle.Text = "Total:";
                        LblTotalTitle.Location = new System.Drawing.Point(510, 15);
                        LblTotalTitle.Width = 40;
                        LblTotalTitle.Height = 15;
                        GrBx.Controls.Add(LblTotalTitle);

                        Label LblTotlaValue = new Label();
                        LblTotlaValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblTotlaValue.ForeColor = Color.Navy;
                        LblTotlaValue.Text = FormatBytes(dirInfo.TotalSize);
                        LblTotlaValue.Location = new System.Drawing.Point(550, 15);
                        LblTotlaValue.Height = 15;
                        LblTotlaValue.Width = 80;
                        GrBx.Controls.Add(LblTotlaValue);



                        Label LblTotalPercent = new Label();
                        LblTotalPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblTotalPercent.ForeColor = Color.Brown;
                        LblTotalPercent.Text = GetDrivePercent(dirInfo.TotalSize, dirInfo.TotalSize);
                        LblTotalPercent.Location = new System.Drawing.Point(630, 15);
                        LblTotalPercent.Height = 15;
                        LblTotalPercent.Width = 60;
                        GrBx.Controls.Add(LblTotalPercent);






                        Label LblFreeTitle = new Label();
                        LblFreeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblFreeTitle.ForeColor = Color.Red;
                        LblFreeTitle.Text = "Free:";
                        LblFreeTitle.Location = new System.Drawing.Point(510, 30);
                        LblFreeTitle.Width = 40;
                        LblFreeTitle.Height = 15;
                        GrBx.Controls.Add(LblFreeTitle);

                        Label LblFreeValue = new Label();
                        LblFreeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblFreeValue.ForeColor = Color.Navy;
                        LblFreeValue.Text = FormatBytes(dirInfo.TotalFreeSpace);
                        LblFreeValue.Location = new System.Drawing.Point(550, 30);
                        LblFreeValue.Height = 15;
                        LblFreeValue.Width = 80;
                        GrBx.Controls.Add(LblFreeValue);


                        Label LblFreePercent = new Label();
                        LblFreePercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblFreePercent.ForeColor = Color.Brown;
                        LblFreePercent.Text = GetDrivePercent(dirInfo.TotalSize, dirInfo.TotalFreeSpace);
                        LblFreePercent.Location = new System.Drawing.Point(630, 30);
                        LblFreePercent.Height = 15;
                        LblFreePercent.Width = 60;
                        GrBx.Controls.Add(LblFreePercent);








                        Label LblUseTitle = new Label();
                        LblUseTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblUseTitle.ForeColor = Color.Red;
                        LblUseTitle.Text = "Used:";
                        LblUseTitle.Location = new System.Drawing.Point(510, 45);
                        LblUseTitle.Width = 40;
                        LblUseTitle.Height = 15;
                        GrBx.Controls.Add(LblUseTitle);

                        Label LblUseValue = new Label();
                        LblUseValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblUseValue.ForeColor = Color.Navy;
                        LblUseValue.Text = FormatBytes(dirInfo.TotalSize - dirInfo.TotalFreeSpace);
                        LblUseValue.Location = new System.Drawing.Point(550, 45);
                        LblUseValue.Height = 15;
                        LblUseValue.Width = 80;
                        GrBx.Controls.Add(LblUseValue);

                        Label LblUsePercent = new Label();
                        LblUsePercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
                        LblUsePercent.ForeColor = Color.Brown;
                        LblUsePercent.Text = GetDrivePercent(dirInfo.TotalSize, dirInfo.TotalSize - dirInfo.TotalFreeSpace);
                        LblUsePercent.Location = new System.Drawing.Point(630, 45);
                        LblUsePercent.Height = 15;
                        LblUsePercent.Width = 60;
                        GrBx.Controls.Add(LblUsePercent);





                        flowLayoutPanel1.Controls.Add(GrBx);

                    }

                }
                timer1.Enabled = true;
            }
            catch 
            {
                timer1.Enabled = true;
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                MainTask();
            }
            catch 
            {
                
            }
          
        }
    }

    public static class Pathing
    {
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);
        /// <summary>
        /// Given a path, returns the UNC path or the original. (No exceptions
        /// are raised by this function directly). For example, "P:\2008-02-29"
        /// might return: "\\networkserver\Shares\Photos\2008-02-09"
        /// </summary>
        /// <param name="originalPath">The path to convert to a UNC Path</param>
        /// <returns>A UNC path. If a network drive letter is specified, the
        /// drive letter is converted to a UNC or network path. If the 
        /// originalPath cannot be converted, it is returned unchanged.</returns>
        public static string GetUNCPath(string originalPath)
        {
            StringBuilder sb = new StringBuilder(512);
            int size = sb.Capacity;

            // look for the {LETTER}: combination ...
            if (originalPath.Length > 2 && originalPath[1] == ':')
            {
                // don't use char.IsLetter here - as that can be misleading
                // the only valid drive letters are a-z && A-Z.
                char c = originalPath[0];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    int error = WNetGetConnection(originalPath.Substring(0, 2),
                        sb, ref size);
                    if (error == 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(originalPath);

                        string path = Path.GetFullPath(originalPath)
                            .Substring(Path.GetPathRoot(originalPath).Length);
                        return Path.Combine(sb.ToString().TrimEnd(), path);
                    }
                }
            }

            return originalPath;
        }
    }
    public class NewProgressBar : ProgressBar
    {
        Brush Br;
        public NewProgressBar(Brush InBr)
        {
            Br = InBr;
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;

            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
            e.Graphics.FillRectangle(Br, 2, 2, rec.Width, rec.Height);
        }
    }
}
