using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Windows.Threading;

/* Bernd Verhofstadt
 * Artesis Plantijn Hogeschool Antwerpen
 * PBA-EA
 */


namespace HtmlAgility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    /*TODO
     * Delete id="menu"
     * Delete class = "tnetUrl"
     * Delete id="footer"
     */

    public partial class MainWindow : Window
    {
        private int l = 0;
        private string[] linkPage = new string[100]; 
        private string[] titlePage = new string[100];
        DispatcherTimer ProgressWebsite;
        
        
        public MainWindow()
        {
            InitializeComponent();
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            wc.DownloadStringAsync(new Uri("http://tweakers.mobi/"));
            DateTime time = DateTime.Now;
            string format = "D";
            DatumToday.Content = time.ToString(format);
            ProgressWebsite = new DispatcherTimer(DispatcherPriority.Render);
            InitializeTimer();
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                
                var scrape = new HtmlAgilityPack.HtmlDocument();
                scrape.LoadHtml(e.Result);
                try
                {
                    
                    HtmlNode docnode = scrape.DocumentNode;
                    HtmlNode[] divnodes = docnode.Descendants("div").ToArray();
                    for (int i = 0; i < divnodes.Length; i++)
                    {
                        if (divnodes[i].Attributes["class"] != null)
                        {
                            if (divnodes[i].Attributes["class"].Value == "content")
                            {
                                HtmlNode[] anodes = divnodes[i].Descendants("a").ToArray();
                                foreach (var htmlNode in anodes)
                                {
                                    linkPage[l] = Convert.ToString(htmlNode.Attributes["href"].Value);
                                    titlePage[l] = htmlNode.InnerText;

                                    if (titlePage[l].Contains("Nieuwsberichten van") || titlePage[l].Contains("nieuwsberichten van"))
                                    {
                                        listviewresult.Items.Add(new Run(titlePage[l]));
                                        //MessageBox.Show(titlePage[l]);
                                    }
                                    else
                                    {
                                        listviewresult.Items.Add(titlePage[l]);
                                    }
                                    
                                    l++;
                                }
                            }
                        }
                    }

                    try
                    {
                        ProgressBar.Value = ProgressBar.Minimum;
                        ProgressWebsite.Start();
                        BrowserPage.Navigate(linkPage[0]);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Can't load page...");
                        throw;
                    }                    
                }
                catch (Exception)
                {
                    MessageBox.Show("Controleer je netwerkverbindeng","Netwerkprobleem",MessageBoxButton.OK,MessageBoxImage.Warning);
                }
            }
        }

        private void InitializeTimer()
        {
            ProgressWebsite.Tick +=ProgressWebsite_Tick;
            ProgressWebsite.Interval = new TimeSpan(0, 0, 0, 0, 100);
            ProgressWebsite.IsEnabled = true;
        }

        private void ProgressWebsite_Tick(object sender, EventArgs e)
        {
            ProgressBar.Value += 1;
            if (ProgressBar.Value == ProgressBar.Maximum)
                ProgressWebsite.Stop();
        }

        private void listviewresult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ProgressBar.Value = ProgressBar.Minimum;
                ProgressWebsite.Start();
                int selectedIndex = listviewresult.SelectedIndex;
                //BrowserPage.Navigate("http://verhofstadt.eu");
                BrowserPage.Navigate(linkPage[selectedIndex]);
            }
            catch (Exception)
            {
                MessageBox.Show("Controleer je netwerkverbindeng", "Netwerkprobleem", MessageBoxButton.OK, MessageBoxImage.Warning);
                throw;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://ap.be");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBar.Value = ProgressBar.Minimum;
                ProgressWebsite.Start();
                BrowserPage.GoBack();
            }
            catch (Exception) 
            {
                ProgressWebsite.Stop();
                ProgressBar.Value = ProgressBar.Maximum;
            }
        }

        private void BrowserPage_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ProgressWebsite.Stop();
            ProgressBar.Value = ProgressBar.Maximum;
        }      
    }
}
