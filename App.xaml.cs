using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XGups
{
    public partial class App : Application
    {
        public static ImageSource imgUpd;
        public static string VER = "3.5";
        public static List<LessonsView> LessonsTime = new List<LessonsView>();

        public App()
        {
            InitializeComponent();
            MainPage = new loadpage();
            #region Test
            //MainPage = new UniversityLive();
            //return;

            //MainPage = new JournalPage();

            //var sch = new NotificationRequestSchedule();
            //sch.NotifyTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, 20, 00));
            //var notify = new NotificationRequest()
            //{
            //    BadgeNumber = 0,
            //    Description = $"{deserilize()}",
            //    Title = "Пара",
            //    Subtitle = "Через 15 минут",
            //    ReturningData = "Data",
            //    NotificationId = 1,
            //    Schedule = sch,
            //    CategoryType = NotificationCategoryType.Event
            //};
            //LocalNotificationCenter.Current.Show(notify);
            #endregion

            Request.reverse = Preferences.Get("Reverse", false);

            //Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);

                try
                {
                    if (Preferences.Get("Name", "") == "")
                    {
                        MainPage = new MainPage();
                    }
                    else
                    {
                        var upd = Task.Run(() =>
                        {
                            if (Request.GetUpdateVersion() != VER)
                            {
                                MainMenu.addUpdateIsAlreadyBeen = false;
                            }
                        });
                        string[] name = Preferences.Get("Name", "").Split(' ');
                        HumanSerilaze humanSerilaze = (HumanSerilaze)DerializeObject();
                        Human human = new Human(humanSerilaze);

                        if (human.DetailInfo.drcod == null) human = human;
                        else
                        {

                            Request.GetSchedule(human);
                            MainPage = new MainMenu(human);
                            return;
                        }

                        List<Human> humans = new List<Human>();

                        humans = Request.GetHuman(name[0], name[1]);
                        human = humans[0];

                        //Если 0 чуваков, выходим из поиска.
                        if (humans.Count == 0)
                        {

                            Dispatcher.BeginInvokeOnMainThread(() => { MainPage = new MainPage(); });
                            return;
                        }

                        Request.GetDetailInfo(human);
                        Request.GetSchedule(human);
                        upd.Wait();
                        Dispatcher.BeginInvokeOnMainThread(() => { MainPage = new MainMenu(human); });
                       
                    }

                }
                catch
                {
                    Dispatcher.BeginInvokeOnMainThread(() => { MainPage = new MainPage(); });
                }
        }
        public static object DerializeObject()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sample.txt");

            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            HumanSerilaze list = (bf.Deserialize(fileStream) as HumanSerilaze);
            fileStream.Close();
            return list;
        }
    }
}
