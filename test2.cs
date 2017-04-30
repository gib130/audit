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
            //Запрещаем повторное нажатие на кнопку запуска программы
            button1.Enabled = false;
            //Запускаем основной поток программы
            backgroundWorker1.RunWorkerAsync();
            //Запускаем сохранение поток, сохраняющий данные программы
            backgroundWorker2.RunWorkerAsync();
            //Если нужно, запускаем побочное приложение и передаём параметры
            if (this.checkBox5.Checked == true)
            {
                Process.Start("KomodiaRelay.exe", this.textBox10.Text + " " + this.textBox4.Text + " " + this.textBox11.Text + " " + this.textBox12.Text + " u  6000");
            }
            backgroundWorker3.RunWorkerAsync();
        }
        
        private void Post(string id)
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
                        var response = wb.UploadValues("http://store.codehatch.com/rok/server_update.php", "POST", GetData(id));
                        wb.Dispose();
                        
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка " + ex.ToString());
            }
        }
        //Получаем данные для нашего запроса
        private NameValueCollection GetData(string id)
        {
            try
            {
                var data1 = new NameValueCollection();
                data1["ID"] = id.Substring(0, 7);
                data1["NM"] = this.textBox1.Text;
                data1["VN"] = this.textBox2.Text;
                data1["IP"] = this.textBox3.Text;
                data1["PN"] = this.textBox4.Text;
                data1["PW"] = "FALSE";
                data1["TP"] = this.comboBox1.SelectedItem.ToString();
                data1["PL"] = GetMaxSlots();
                data1["PC"] = GetOnline();
                data1["DE"] = "FALSE";
                data1["OF"] = "FALSE";
                data1["PR"] = "FALSE";
                if (this.checkBox1.Checked)
                    data1["DE"] = "TRUE";
                if (this.checkBox2.Checked == true)
                    data1["OF"] = "TRUE";
                if (this.checkBox3.Checked == true)
                    data1["PR"] = "TRUE";
                data1["ON"] = "TRUE";
                return data1;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка при сборке данных для запроса");
                return null;
            }
        }
        string GetMaxSlots()
        {
            //Ограничиваем максимальное кол-во слотов 70 
            try
            {
                if (Convert.ToInt16(this.textBox5.Text) > 70)
                {
                    this.textBox5.Text = "70";
                    return "70";
                }
                else
                {
                    return this.textBox5.Text;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Не верно введено кол-во слотов");
                return null;
            }
        }
        string GetOnline()
        {
            try
            {
                //Берём введённое число игроков пользователем или генерируем его
                Random rand = new Random();
                //Проверяем, нужно ли делать рандомное число игроков
                if (!this.checkBox4.Checked)
                {
                    this.textBox6.Enabled = true;
                    //Проверяем, чтобы кол-во игроков было не больше 50
                    if (Convert.ToInt16(this.textBox6.Text) > 50)
                    {
                        this.textBox6.Text = "50";
                    }

                    //Проверяем, чтобы кол-во игроков не превышало кол-во слотов для них
                    if (Convert.ToInt16(this.textBox6.Text) < Convert.ToInt16(this.textBox5.Text))
                    {
                        return this.textBox6.Text;
                    }
                    else
                    {
                        this.textBox6.Text = Convert.ToString(Convert.ToInt16(this.textBox5.Text) - 5);
                        return this.textBox6.Text;
                    }
                }
                else
                {
                    //Генерируем число игроков из наших промежутков минимум-максимум
                    this.textBox6.Enabled = false;
                    if (Convert.ToInt32(this.textBox8.Text) > 50)
                    {
                        this.textBox6.Text = Convert.ToString(rand.Next(Convert.ToInt32(this.textBox7.Text), 50));
                        return this.textBox6.Text;
                    }
                    else
                    {
                        this.textBox6.Text = Convert.ToString(rand.Next(Convert.ToInt32(this.textBox7.Text), Convert.ToInt32(this.textBox8.Text)));
                        return this.textBox6.Text;
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Возникла ошибка с выбором отображаемого кол-ва игроков");
                return null;
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
                MessageBox.Show(ex.ToString() + " Возникла ошибка с получением id сервера из списка серверов");
                return null;
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            try
            {
                while (true)
                {
                    if (getid() != null)
                    {
                        string s1 = getid();
                        s1.Substring(0, 7);
                        Post(s1);
                        DateTime time = DateTime.Now;
                        this.label10.Text = s1;
                        this.label12.Text = String.Format("{0:HH:mm:ss}", time);
                        Thread.Sleep(10000);
                    }
                    }
                
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.ToString() + " Возникла ошибка с отправкой информации о сервере");
            }
        }



        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                try
                {
                    IniFile ini3 = new IniFile(Path.Combine(Environment.CurrentDirectory, @"settings.ini").Replace(@"\", @"/"));
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString() + "Возникла ошибка с сохранением данных");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                IniFile ini3 = new IniFile(Path.Combine(Environment.CurrentDirectory, @"settings.ini").Replace(@"\", @"/"));
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
                if (ini3.IniReadValue("System", "DE") == "true")
                { this.checkBox1.Checked = true; }
                if (ini3.IniReadValue("System", "Official") == "true")
                { this.checkBox2.Checked = true; }
                if (ini3.IniReadValue("System", "PR") == "true")
                { this.checkBox3.Checked = true; }
                if (ini3.IniReadValue("System", "RND") == "true")
                { this.checkBox4.Checked = true; }
                if (ini3.IniReadValue("System", "Redirect") == "true")
                { this.checkBox5.Checked = true; }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString() + "Возникла ошибка с загрузкой сохранённых даннных");
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
