using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Runtime;
using System.Reflection;

namespace testrok
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this.comboBox1.SelectedItem.ToString());
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
            button1.Enabled = false;
            if (this.checkBox5.Checked == true)
            {
                Process.Start("KomodiaRelay.exe", this.textBox10.Text + " " + this.textBox4.Text + " " + this.textBox11.Text + " " + this.textBox12.Text + " u  6000");
            }
            backgroundWorker3.RunWorkerAsync();
        }
        
        private void post(string id)
        {
            try
            {
                    using (var wb = new WebClient())
                    {
                        wb.Headers.Add("User-Agent", "UnityPlayer/4.6.1f1 (http://unity3d.com)");
                        wb.Headers.Add("X-Unity-Version", "4.6.1f1");
                        wb.Headers.Add("Accept", "*/*");
                        wb.Headers.Add("Accept-Encoding", "identity");
                        wb.Headers.Remove("Expect");
                        wb.Headers.Remove("Connection");
                        var data1 = new NameValueCollection();
                        data1["ID"] = id.Substring(0,7);
                        data1["NM"] = this.textBox1.Text;
                        data1["VN"] = this.textBox2.Text;
                        data1["IP"] = this.textBox3.Text;
                        data1["PN"] = this.textBox4.Text;
                        data1["PW"] = "FALSE";
                        data1["TP"] = this.comboBox1.SelectedItem.ToString();
                        if (Convert.ToInt16(this.textBox5.Text) > 70)
                        {
                            this.textBox5.Text = "50";
                            data1["PL"] = this.textBox5.Text;
                        }
                        else
                        {
                            data1["PL"] = this.textBox5.Text;
                        }
                        Random rand = new Random();
                        if (this.checkBox4.Checked == false)
                        {
                            if (Convert.ToInt16(this.textBox6.Text) > 50)
                            {
                                this.textBox6.Text = "50";
                                data1["PC"] = this.textBox6.Text;
                            }
                            else
                            {
                                data1["PC"] = this.textBox6.Text;
                            }
                            this.textBox6.Enabled = true;
                        }
                        else
                        {
                            string rnd;
                            if (Convert.ToInt32(this.textBox8.Text) > 50)
                            {
                                rnd = Convert.ToString(rand.Next(Convert.ToInt32(this.textBox7.Text), 50));
                            }
                            else
                            {
                                rnd = Convert.ToString(rand.Next(Convert.ToInt32(this.textBox7.Text), Convert.ToInt32(this.textBox8.Text)));
                            }
                            
                            data1["PC"] = rnd;
                            this.textBox6.Text = rnd;
                            this.textBox6.Enabled = false;
                        }
                        if (this.checkBox1.Checked == true)
                        {
                            data1["DE"] = "TRUE";
                        }
                        else
                        {
                            data1["DE"] = "FALSE";
                        }
                        if (this.checkBox2.Checked == true)
                        {
                            data1["OF"] = "TRUE";
                        }
                        else
                        {
                            data1["OF"] = "FALSE";
                        }
                        if (this.checkBox3.Checked == true)
                        {
                            data1["PR"] = "TRUE";
                        }
                        else
                        {
                            data1["PR"] = "FALSE";
                        }
                        data1["ON"] = "TRUE";
                        var response = wb.UploadValues("http://store.codehatch.com/rok/server_update.php", "POST", data1);
                        wb.Dispose();
                        
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        public static bool SetAllowUnsafeHeaderParsing()
        {
            //Get the assembly that contains the internal class
            Assembly aNetAssembly = Assembly.GetAssembly(
              typeof(System.Net.Configuration.SettingsSection));
            if (aNetAssembly != null)
            {
                //Use the assembly in order to get the internal type for 
                // the internal class
                Type aSettingsType = aNetAssembly.GetType(
                  "System.Net.Configuration.SettingsSectionInternal");
                if (aSettingsType != null)
                {
                    //Use the internal static property to get an instance 
                    // of the internal settings class. If the static instance 
                    // isn't created allready the property will create it for us.
                    object anInstance = aSettingsType.InvokeMember("Section",
                      BindingFlags.Static | BindingFlags.GetProperty
                      | BindingFlags.NonPublic, null, null, new object[] { });
                    if (anInstance != null)
                    {
                        //Locate the private bool field that tells the 
                        // framework is unsafe header parsing should be 
                        // allowed or not
                        FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField(
                          "useUnsafeHeaderParsing",
                          BindingFlags.NonPublic | BindingFlags.Instance);
                        if (aUseUnsafeHeaderParsing != null)
                        {
                            aUseUnsafeHeaderParsing.SetValue(anInstance, true);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
          
        private string getid()
        {
            try
            {
                using (var wb = new WebClient())
                {
                    SetAllowUnsafeHeaderParsing();
                    var response = wb.DownloadString("http://store.codehatch.com/rok/lobby2.php");
                    var pos = response.IndexOf(this.textBox9.Text);
                    //MessageBox.Show(pos.ToString());
                    if (pos != -1)
                    {
                        string id = response.Substring(pos - 8, 7);
                        wb.Dispose();
                        return id;
                    }
                    else
                    {
                        wb.Dispose();
                        return null;
                    }
                    

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return null;
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            try
            {
                int s = 1;
                while (s==1)
                {
                    if (getid() != null)
                    {
                        string s1 = getid();
                        s1.Substring(0, 7);
                        post(s1);
                        DateTime time = DateTime.Now;
                        this.label10.Text = s1;
                        this.label12.Text = String.Format("{0:HH:mm:ss}", time);
                        Thread.Sleep(10000);
                    }
                    }
                
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.ToString());
            }
        }



        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                string wi1;
                string wi2 = @"settings.ini";
                wi1 = Path.Combine(Environment.CurrentDirectory, wi2);
                string correctwi1 = wi1.Replace(@"\", @"/");
                IniFile ini3 = new IniFile(correctwi1);
                ini3.IniWriteValue("System", "WorldName", this.textBox1.Text);
                ini3.IniWriteValue("System", "Version", this.textBox2.Text);
                ini3.IniWriteValue("System", "GamePort", this.textBox4.Text);
                ini3.IniWriteValue("System", "IP", this.textBox3.Text);
                ini3.IniWriteValue("System", "MaxPlayers", this.textBox5.Text);
                ini3.IniWriteValue("System", "mi", this.textBox7.Text);
                ini3.IniWriteValue("System", "ma", this.textBox8.Text);
                ini3.IniWriteValue("System", "SearchName", this.textBox9.Text);
                ini3.IniWriteValue("System", "LocalIP", this.textBox10.Text);
                ini3.IniWriteValue("System", "RedirectIP", this.textBox11.Text);
                ini3.IniWriteValue("System", "RedirectPORT", this.textBox12.Text);
                if (this.checkBox1.Checked)
                { ini3.IniWriteValue("System", "DE", "true"); }
                else
                { ini3.IniWriteValue("System", "DE", "false"); }
                if (this.checkBox2.Checked)
                { ini3.IniWriteValue("System", "Official", "true"); }
                else
                { ini3.IniWriteValue("System", "Official", "false"); }
                if (this.checkBox3.Checked)
                { ini3.IniWriteValue("System", "PR", "true"); }
                else
                { ini3.IniWriteValue("System", "PR", "false"); }
                if (this.checkBox4.Checked)
                { ini3.IniWriteValue("System", "RND", "true"); }
                else
                { ini3.IniWriteValue("System", "RND", "false"); }
                if (this.checkBox5.Checked)
                { ini3.IniWriteValue("System", "Redirect", "true"); }
                else
                { ini3.IniWriteValue("System", "Redirect", "false"); }
                Thread.Sleep(60000);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string de, official, pr,rnd,wi1,redir;

                string wi2 = @"settings.ini";
                wi1 = Path.Combine(Environment.CurrentDirectory, wi2);
                string correctwi1 = wi1.Replace(@"\", @"/");
                IniFile ini3 = new IniFile(correctwi1);

                this.textBox1.Text = ini3.IniReadValue("System", "WorldName");
                this.textBox2.Text = ini3.IniReadValue("System", "Version");
                this.textBox3.Text = ini3.IniReadValue("System", "IP");
                this.textBox4.Text = ini3.IniReadValue("System", "GamePort");
                this.textBox5.Text = ini3.IniReadValue("System", "MaxPlayers");
                this.textBox7.Text = ini3.IniReadValue("System", "mi");
                this.textBox8.Text = ini3.IniReadValue("System", "ma");
                this.textBox9.Text = ini3.IniReadValue("System", "SearchName");
                this.textBox10.Text = ini3.IniReadValue("System", "LocalIP");
                this.textBox11.Text = ini3.IniReadValue("System", "RedirectIP");
                this.textBox12.Text = ini3.IniReadValue("System", "RedirectPORT");
                de = ini3.IniReadValue("System", "DE");
                official= ini3.IniReadValue("System", "Official");
                pr = ini3.IniReadValue("System", "PR");
                rnd = ini3.IniReadValue("System", "RND");
                redir = ini3.IniReadValue("System", "Redirect");
                if (de == "true")
                { this.checkBox1.Checked = true; }
                if (official == "true")
                { this.checkBox2.Checked = true; }
                if (pr == "true")
                { this.checkBox3.Checked = true; }
                if (rnd == "true")
                { this.checkBox4.Checked = true; }
                if (redir == "true")
                { this.checkBox5.Checked = true; }
            }
            catch
            {

            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (backgroundWorker1.IsBusy == false)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                Thread.Sleep(60000);
            }
        }
    }
}
