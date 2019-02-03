using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MailBee;
using MailBee.Mime;
using MailBee.Security;
using MailBee.SmtpMail;



namespace Fludd
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {

            InitializeComponent();
        }

        string[] froms;
        string[] to;
        string[] proxy;
        string msg = "";
        string proxy_path = "";
        string from_path = "";
        string to_path = "";
       

        Random rnd = new Random();
        [STAThread]
        public void Start_Click(object sender, EventArgs e)
        {

            progressBar1.Value = 0;
            int flag = 0;
            if(comboBox1.Text == "")
            {
                MessageBox.Show("Задайте тип прокси");
                flag = 1;
            }
            if (froms.Length == 0)
            {
                MessageBox.Show("Загрузите базу Input");
                flag = 1;
            }
            if(textBox1.Text == "")
            {
                foreach (char c in textBox1.Text)
                {
                    if (c < '0' || c > '9')
                    {
                    }
                    else
                    {
                        MessageBox.Show("В поле потоки должны быть только числа");
                        flag = 1;
                    }
                }
            }
            if (to.Length == 0)
            {
                MessageBox.Show("Загрузите базу To");
                flag = 1;

            }
            if (msg == "")
            {
                MessageBox.Show("Введите сообщение");
                flag = 1;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Введите заголовок");
                flag = 1;
            }
            if (proxy.Length == 0)
            {
                MessageBox.Show("Задайте прокси");
                flag = 1;
            }
            if (textBox3.Text == "")
            {
                MessageBox.Show("Введите имя");
                flag = 1;
            }
            if (flag == 0)
            {
                string prox_type = comboBox1.Text;
                int thread_len = Convert.ToInt32(textBox1.Text);
                int prox_per_thread = (proxy.Length - (proxy.Length % thread_len)) / thread_len;
                int from_per_thread = (froms.Length - (froms.Length % thread_len)) / thread_len;
                int to_per_thread = (to.Length - (to.Length % thread_len)) / thread_len;
                Thread_Set thread_obj = new Thread_Set(prox_per_thread, from_per_thread, to_per_thread, froms, to, proxy);
                for (int i = 1; i <= thread_len; i++)
                {
                    Thread myThread = new Thread(new ThreadStart(thread_obj.SendMail));
                    myThread.Name = i.ToString();
                    myThread.Start();
                }

                
                if (froms.Length == to.Length && flag == 0)
                {
                    for (int step = 0; step < to.Length; step++)
                    {
                        /*
                        string[] data = froms[step].Split(':');
                        MailAddress From = new MailAddress(data[0], textBox3.Text);
                        MailAddress To = new MailAddress(to[step]);
                        progressBar1.Value += delen-1;
                        */

                    }
                }
            }
            
        }



        private void message_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                msg = File.ReadAllText(openFileDialog1.FileName);
            }
        }
        private void proxyButton_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog4.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                proxy_path = openFileDialog4.FileName;
            }
            backgroundWorker1.RunWorkerAsync();
            this.proxyButton.Enabled = false;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.proxyButton.Enabled = true;
        }
        public void From_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                from_path = openFileDialog3.FileName;
            }
            this.From.Enabled = false;
            backgroundWorker2.RunWorkerAsync();
        }
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.From.Enabled = true;
        }

        public void To_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                to_path = openFileDialog2.FileName;
            }
            this.To.Enabled = false;
            backgroundWorker3.RunWorkerAsync();
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.To.Enabled = true;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            proxy = File.ReadAllLines(proxy_path);
            label12.Invoke(new Action(() => label12.Text = proxy.Length.ToString()));
            if(proxy.Length > 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    listBox3.Invoke(new Action(() => listBox3.Items.Add(proxy[i])));
                }
            }
            else
            {
                for (int i = 0; i < proxy.Length; i++)
                {
                    listBox3.Invoke(new Action(() => listBox3.Items.Add(proxy[i])));
                }
            }
            MessageBox.Show("Чтение прокси завершено.");
        }
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            froms = File.ReadAllLines(from_path);
            label4.Invoke(new Action(() => label4.Text = froms.Length.ToString()));
            if (froms.Length > 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    listBox1.Invoke(new Action(() => listBox1.Items.Add(froms[i])));
                }
            }
            else
            {
                for (int i = 0; i < froms.Length; i++)
                {
                    listBox1.Invoke(new Action(() => listBox1.Items.Add(froms[i])));
                }
            }
            MessageBox.Show("Чтение FROM завершено.");

        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            
            to = File.ReadAllLines(to_path);
            label5.Invoke(new Action(() => label5.Text = to.Length.ToString()));
            if (to.Length > 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    listBox2.Invoke(new Action(() => listBox2.Items.Add(to[i])));
                }
            }
            else
            {
                for (int i = 0; i < to.Length; i++)
                {
                    listBox2.Invoke(new Action(() => listBox2.Items.Add(to[i])));
                }
            }
            progressBar1.Invoke(new Action(() => progressBar1.Maximum = to.Length));
            MessageBox.Show("Чтение TO завершено.");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("По всем вопросам обращаться в Telegram @commodore");
        }
        private static string GetDomain(string email)
        {
            return email.Split('@').LastOrDefault();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MailBee.Global.LicenseKey = "MN110-F8303072303E306430950DA42BF8-00B5";
        }
    }
    public class Thread_Set
    {
        string[] froms;
        string[] to;
        string[] proxy;
        public int prox_per_thread;
        public int from_per_thread;
        public int to_per_thread;
        static string[] server_info(string domain)
        {
            string[] return_info = new string[3];
            string url = @"https://autoconfig.thunderbird.net/v1.1/" + domain;
            string html = string.Empty;
            string serverpath = "//outgoingServer[@type= " + '\u0022' + "smtp" + '\u0022' + "]/hostname";
            string portpath = "//outgoingServer[@type= " + '\u0022' + "smtp" + '\u0022' + "]/port";
            string socketrpath = "//outgoingServer[@type= " + '\u0022' + "smtp" + '\u0022' + "]/socketType";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(html);
                XmlNode node_server = xmlDoc.SelectSingleNode(serverpath);
                XmlNode node_port = xmlDoc.SelectSingleNode(portpath);
                XmlNode node_socket = xmlDoc.SelectSingleNode(socketrpath);
                return_info[0] = node_server.ChildNodes[0].InnerText;
                return_info[1] = node_port.ChildNodes[0].InnerText;
                return_info[2] = node_socket.ChildNodes[0].InnerText;
            }
            return return_info;
        }

        public Thread_Set(int _prox_per_thread, int _from_per_thread, int _to_per_thread, string[] _froms, string[] _to, string[] _proxy)
        {
            this.prox_per_thread = _prox_per_thread;
            this.from_per_thread = _from_per_thread;
            this.to_per_thread = _to_per_thread;
            this.froms = _froms;
            this.to = _to;
            this.proxy = _proxy;
        }

        public void SendMail()
        {
            if (to_per_thread > from_per_thread)
            {
                for (int step = 0; step < to.Length; step++)
                {
                     string[] data = froms[step%to_per_thread].Split(':');
                     MailAddress From = new MailAddress(data[0], textBox3.Text);
                     MailAddress To = new MailAddress(to[step]);
                     Form1.progressBar1.Invoke(new Action(() => Form1.progressBar1.Value++));
                }
            }
            else
            {

            }
            for (int i = 0; i <= (Convert.ToInt32(Thread.CurrentThread.Name) * prox_per_thread) - 1; i++)
            {
                
            }


        }
    }
}
