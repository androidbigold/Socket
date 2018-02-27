using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace client
{
    public partial class Form2 : Form
    {
        //客户端与服务器之间的连接状态
        public bool bConnected = false;
        //监听线程
        public Thread tAcceptMsg = null;
        //用于socket通信的ip地址和通信端口
        public IPEndPoint IPP = null;
        //socket通信
        public Socket socket = null;
        //网络访问的基础数据流
        public NetworkStream nStream = null;
        //创建读取器
        public TextReader tReader = null;
        //创建编写器
        public TextWriter wReader = null;

        public Form2()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel1.ClientRectangle,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid);

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel2.ClientRectangle,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid);

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel3.ClientRectangle,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid);

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel4.ClientRectangle,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid,
                Color.White, 1, ButtonBorderStyle.Solid);

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("确定要退出程序吗？", "提示", MessageBoxButtons.YesNo))
            {
                if (socket != null)
                {
                    if (socket.Connected)
                    {
                        MessageBox.Show("请先关闭连接");
                        e.Cancel = true;
                    }
                    else
                    {
                        e.Cancel = false;
                        Application.Exit();
                    }
                }
                else
                {
                    e.Cancel = false;
                    System.Environment.Exit(0);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void AcceptMessage()
        {
            string sTemp;
            while (bConnected)
            {
                try
                {
                    sTemp = tReader.ReadLine();
                    if (sTemp.Length != 0)
                    {
                        lock (this)
                        {
                            textBox3.Text = "服务器:" + sTemp + "\r\n" + textBox3.Text;
                        }
                    }
                }
                catch
                {

                }
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                IPP = new IPEndPoint(IPAddress.Parse(textBox1.Text), 65535);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(IPP);
                if (socket.Connected)
                {
                    nStream = new NetworkStream(socket);
                    tReader = new StreamReader(nStream);
                    wReader = new StreamWriter(nStream);
                    tAcceptMsg = new Thread(new ThreadStart(this.AcceptMessage));
                    tAcceptMsg.Start();
                    bConnected = true;
                    button1.Enabled = false;
                    MessageBox.Show("与服务器成功连接，可以通信了!");
                    textBox2.Text = DateTime.Now.ToString() + " : 与服务器" +
                        socket.RemoteEndPoint.ToString() + "连接成功.\r\n" + textBox2.Text;
                }
            }
            catch
            {
                MessageBox.Show("无法与服务器通信!");

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                try
                {
                    lock (this)
                    {
                        textBox3.Text = "客户端:" + textBox4.Text + "\r\n" + textBox3.Text;
                        wReader.WriteLine(textBox4.Text);
                        wReader.Flush();
                        textBox4.Text = "";
                        textBox4.Focus();
                    }
                }
                catch
                {
                    MessageBox.Show("与服务器连接断开了");
                }
            }
            else
            {
                MessageBox.Show("未与服务器建立连接，不能通信.");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            bConnected = false;
            button1.Enabled = true;
            tAcceptMsg.Abort();
            if (socket.RemoteEndPoint != null)
                textBox2.Text = DateTime.Now.ToString() + " : 与服务器" +
            socket.RemoteEndPoint.ToString() + "断开连接.\r\n" + textBox2.Text;
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
    }
}
