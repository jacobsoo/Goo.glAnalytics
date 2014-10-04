using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace Goo.gl_Analytics
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class AnalyticsGrabber : Form
    {
        public static MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();
        public AnalyticsGrabber()
        {
            InitializeComponent();
        }

        private string FindScript()
        {
            string fpath = "";
            string base_path = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < 4; i++)
            {
                fpath = base_path + "\\beautify.js";
                if (!File.Exists(fpath)) base_path += "\\..";
                else break;
            }
            if (!File.Exists(fpath)) fpath = "";
            return fpath;
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            string szShortURL = txtURL.Text;
            string szBase = "https://www.googleapis.com/urlshortener/v1/url?shortUrl=";
            string szPath = "";
            string szResults = "";
            if (szShortURL.Contains("http://"))
            {
                szPath = szBase + szShortURL + "&projection=FULL";
            }
            else
            {
                szPath = szBase + "http://" + szShortURL + "&projection=FULL";
            }
            using (WebClient client = new WebClient())
            {
                szResults = client.DownloadString(szPath);
                txtResults.Text = szResults;

                sc.Language = "jScript";
                sc.AddObject("txtResults", txtResults, true);

                string fpath = FindScript();
                if (fpath.Length == 0)
                {
                    MessageBox.Show("Could not locate beautifer.js");
                    return;
                }

                String script = File.ReadAllText(fpath);
                sc.AddCode(script);
                sc.AddCode(@"txtResults.Text = js_beautify(txtResults.Text, {indent_size: 4, indent_char: ' '}).split('\n').join('\r\n');");
            }
        }
    }
}
