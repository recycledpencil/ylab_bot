using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using use_CoreTweet.lib;
using CoreTweet;
using CoreTweet.Streaming;

namespace Twitter_Client_Use_Core_Tweet
{
    public partial class Form1 : Form
    {
        UCT uct;
        Tokens t;

        System.Threading.Thread th,tweet_th;
        delegate void dleg1(string str);


        public Form1()
        {
            InitializeComponent();
            uct = new UCT();
            t = uct.connect_gui();

            th = new System.Threading.Thread(new System.Threading.ThreadStart(back_loop));
            th.IsBackground = true;
            //スレッドを開始する
            th.Start();

            /*tweet_th = new System.Threading.Thread(new System.Threading.ThreadStart(bot));
            tweet_th.IsBackground = true;
            //スレッドを開始する
            tweet_th.Start();*/

            tweet_th = new System.Threading.Thread(new System.Threading.ThreadStart(mae_bot));
            tweet_th.IsBackground = true;
            //スレッドを開始する
            tweet_th.Start();

        }

        private void addtxt(string s)
        {
            richTextBox1.Focus();
            richTextBox1.AppendText(System.Environment.NewLine + s);
        }
        private void mae_bot()
        {
            DateTime 提出日 = new DateTime(2016, 2, 1, 12, 0, 0);

            DateTime 提出日2 = new DateTime(2016, 1, 31, 12, 0, 0);
            bool day1 = false;
            bool day2 = false;
            bool hour12 = false;
            bool hour4 = false;
            bool end = false;
            
            while (true)
            {
                // 現在の日付と時刻を取得する
                DateTime dtNow = DateTime.Now;

              /*  if (dtNow.Minute==0)
                {
                    TimeSpan ts = (提出日 - dtNow);
                    string nokori = ts.Days + "日" + ts.Hours + "時間" + ts.Minutes + "分" + ts.Seconds + "秒!!";
                    nokori = "卒論提出まで後" + nokori;

                    t.Statuses.Update(new Dictionary<string, object>() { { "status", nokori } });


                    ts = (提出日2 - dtNow);
                    nokori = ts.Days + "日" + ts.Hours + "時間" + ts.Minutes + "分" + ts.Seconds + "秒!!";
                    var text = "@YDKKK 先進レポート提出まで " + nokori;

                    t.Statuses.Update(new Dictionary<string, object>() { { "status", text } });
                    System.Threading.Thread.Sleep(1000 * 60);

                    t.Statuses.Update(new Dictionary<string, object>() { { "status", "山井研はいいぞ！" } });


                }
                */
                if (dtNow.Day == 31 && dtNow.Hour == 12 && day2 == false)
                {
                    string nokori = "@Masami_CS 卒論提出まで後2日！";
                    t.Statuses.Update(new Dictionary<string, object>() { { "status", nokori } });
                    day2 = true;
                }
                if (dtNow.Day == 31 && dtNow.Hour == 12 && day1 == false)
                {
                    string nokori = "@Masami_CS 卒論提出まで後1日！";
                    t.Statuses.Update(new Dictionary<string, object>() { { "status", nokori } });
                    day1 = true;
                }
                if (dtNow.Day == 1 && dtNow.Hour == 0 && hour12 == false)
                {
                    string nokori = "@Masami_CS 卒論提出まで後12時間！";
                    t.Statuses.Update(new Dictionary<string, object>() { { "status", nokori } });
                    hour12 = true;
                }
                if (dtNow.Day == 1 && dtNow.Hour == 0 && hour4 == false)
                {
                    string nokori = "@Masami_CS 卒論提出まで後4時間！";
                    t.Statuses.Update(new Dictionary<string, object>() { { "status", nokori } });
                    hour4 = true;
                }
                if (dtNow.Day == 1 && dtNow.Hour == 12 && end == false)
                {
                    string nokori = "@Masami_CS 卒論提出締め切り！！";
                    t.Statuses.Update(new Dictionary<string, object>() { { "status", nokori } });
                    end = true;
                }
                System.Threading.Thread.Sleep(1000 * 60 );
            }

        }

        private void bot()
        {
            DateTime 提出日 = new DateTime(2016, 2, 1, 12, 0, 0);
            while (true)
            {
                // 現在の日付と時刻を取得する
                DateTime dtNow = DateTime.Now;
                //MessageBox.Show((提出日 - dtNow).ToString());
                TimeSpan ts = (提出日 - dtNow);
                string nokori = ts.Days + "日" + ts.Hours + "時間" + ts.Minutes + "分" + ts.Seconds + "秒";
                nokori = "卒論提出まで後" + nokori;

                t.Statuses.Update(new Dictionary<string, object>() { { "status", nokori } });
                System.Threading.Thread.Sleep(1000*10);
                var text = "今日も山井研はいいぞ！ " + dtNow.ToString();
                //var text = "@Masami_CS 山井研はいいぞ！ "+ dtNow.ToString();
                //var text = "@YDKKK 山井研はいいぞ！ " + dtNow.ToString();
                t.Statuses.Update(new Dictionary<string, object>() { { "status", text } });
                System.Threading.Thread.Sleep(1000*60*10);
            }
        }

        public void back_loop()
        {
            Invoke(new dleg1(addtxt), "開始します");

            var home = t.Statuses.HomeTimeline();//タイムライン20個取得
            for (int i = 0; i < home.Count; i++)
            {
                Invoke(new dleg1(addtxt), home[i].User.ScreenName + "::" + home[i].User.Name + "::" + home[i].Text);
            }

            var stream = t.Streaming.StartStream(CoreTweet.Streaming.StreamingType.User,new StreamingParameters(replies => "all"));

            foreach (var message in stream)
            {
                if (message is StatusMessage)
                {
                    var status = (message as StatusMessage).Status;
                    //Console.WriteLine(string.Format("{0}:{1}", status.User.ScreenName, status.Text));
                    if (status.RetweetedStatus != null)
                    {
                        Invoke(new dleg1(addtxt), string.Format("{2}::{0}[{3}][{4}]:{1}", status.User.ScreenName, status.Text, status.User.Name, status.RetweetedStatus.FavoriteCount, status.RetweetedStatus.RetweetCount));
                    }
                    else
                    {
                        Invoke(new dleg1(addtxt), string.Format("{2}::{0}:{1}", status.User.ScreenName, status.Text, status.User.Name));
                    }
                }
                else if (message is EventMessage)
                {
                    var ev = message as EventMessage;
                    //Console.WriteLine(string.Format("{0}:{1}->{2}",
                    // ev.Event, ev.Source.ScreenName, ev.Target.ScreenName));
                    // Invoke(new dleg1(addtxt), string.Format("{0}:[{3}][{4}]{1}->{2}", ev.Event, ev.Source.ScreenName, ev.Target.ScreenName,ev.Source.Status.RetweetedStatus.RetweetCount, ev.Source.Status.RetweetedStatus.FavoriteCount));
                    Invoke(new dleg1(addtxt), string.Format("{0}:{1}->{2}",
                     ev.Event, ev.Source.ScreenName, ev.Target.ScreenName));


                }
                Invoke(new dleg1(addtxt), "--------------------------------");
            }


        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);//ブラウザで認証を開く

        }
    }
}
