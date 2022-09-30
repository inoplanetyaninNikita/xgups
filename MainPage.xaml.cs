using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XGups
{
    public partial class MainPage : ContentPage
    {
        DateTime date;
        public ObservableCollection<HumanView> humanViewList { get; set; } = new ObservableCollection<HumanView>();
        public List<Human> humans_ = new List<Human>();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            Name.Text = Preferences.Get("Name", "");

            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerEvent);
        }
        private bool textIsChange = false;
        private bool isRunning = false;
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            int Count = 0;
            string[] Name = ((Entry)sender).Text.Split(' ');
            for (int i = 0; i < Name.Length; i++) if (String.IsNullOrWhiteSpace(Name[i]) == false) Count++;

            if (Count == 2 || Count == 3) LoginIN.IsEnabled = true;
            else LoginIN.IsEnabled = false;
            textIsChange = true;
            date = DateTime.Now;
        }
        private bool OnTimerEvent()
        {
            var task = Task.Run(() =>
            {
                if (textIsChange == true && date.Add(new TimeSpan(0, 0, 0, 0, 50)) < DateTime.Now)
                {
                    humans_.Clear();
                    textIsChange = false;
                    var sec = Name.Text.Split(' ');
                    if (sec.Length == 1)
                    {
                        var humans = Request.GetHuman(Name.Text);
                       
                        for (int i = 0; i < humans.Count; i++)
                        {
                            humans_.Add(humans[i]);
                        }
                    }
                    if (sec.Length == 2)
                    {
                        var humans = Request.GetHuman(sec[0], sec[1]);
                        for (int i = 0; i < humans.Count; i++)
                        {
                            humans_.Add(humans[i]);
                            

                        }
                    }
                    isRunning = true;

                }
            });
            Task.Run(() =>
            {
                task.Wait();
                if(isRunning == true)
                {
                    
                    Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        humanViewList.Clear();
                        for (int i = 0; i < humans_.Count; i++)
                        {
                            Human item = humans_[i];
                            humanViewList.Add(new HumanView()
                            {
                                FirstName = humans_[i].ShortInfo.FirstName,
                                SecondName = humans_[i].ShortInfo.SecondName,
                                Human = humans_[i],
                                GroupOrTeacher = humans_[i].ShortInfo.IsTeacher == true ? "Преподаватель" : humans_[i].ShortInfo.Faculty,
                                Click = new Command(() =>
                                {
                                    Activity.IsRunning = true;
                                    LoginIN.IsEnabled = false;
                                    Name.IsEnabled = false;

                                    var task = Task.Run(() =>
                                     {

                                         //ShortInfo.SecondName = list[0];
                                         //ShortInfo.FirstName = list[1];
                                         //ShortInfo.ThirdName = list[2];
                                         //ShortInfo.Departmant = list[3];
                                         //ShortInfo.Faculty = list[4];
                                         //ShortInfo.Position = list[5];
                                         //ShortInfo.ID = Convert.ToInt32(list[6]);
                                         //ShortInfo.IsTeacher = list[7] == "0" ? false : true;
                                         Human human = new Human(new List<string>()
                                         {
                                           item.ShortInfo.SecondName,
                                           item.ShortInfo.FirstName,
                                           item.ShortInfo.ThirdName,
                                           item.ShortInfo.Departmant,
                                           item.ShortInfo.Faculty,
                                           item.ShortInfo.Position,
                                           item.ShortInfo.ID.ToString(),
                                           item.ShortInfo.IsTeacher == true ? "1" : "0"
                                         });
                                        Request.GetDetailInfo(human);
                                        try
                                        {
                                            Request.GetSchedule(human);
                                        }
                                        catch (Exception ex)
                                        {
                                            Device.BeginInvokeOnMainThread(() => DisplayAlert("Ошибка", $"Проблема с автоматическим поиском группы {human.DetailInfo.gname}. Заработает чутка позже, извини :(", "Ок"));
                                            Activity.IsRunning = false;
                                            LoginIN.IsEnabled = true;
                                            Name.IsEnabled = true;
                                            return;
                                        }


                                        Preferences.Set("Name", Name.Text);
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            App.Current.MainPage = new MainMenu(human);
                                        });
                                        HumanSerilaze h = new HumanSerilaze(human);
                                        SerializeObject(h);
                                        // 
                                    });
                                })
                            }); 
                        }

                    });
                    isRunning = false;
                }

            });
            return true;
        }

        private async void LoginIN_Clicked(object sender, EventArgs e)
        {

            string[] name = Name.Text.Split(' ');

            Activity.IsRunning = true;
            LoginIN.IsEnabled = false;
            Name.IsEnabled = false;
            //SaveData(human);
            var task = Task.Run(() =>
            {
                Human human;
                List<Human> humans = new List<Human>();

                humans = Request.GetHuman(name[0], name[1]);

                //Если 0 чуваков, выходим из поиска.
                if (humans.Count == 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Ошибка", "Такого человека нет", "Ок");
                        Activity.IsRunning = false;
                        LoginIN.IsEnabled = true;
                        Name.IsEnabled = true;
                    }
                    );
                    return;
                }
                human = humans[0];

                //if (human.ShortInfo.IsTeacher == true && human.ShortInfo == null)
                //{
                //    Device.BeginInvokeOnMainThread(() => DisplayAlert("Ошибка", "Для преподователей расписание пока не работает :(", "Ок"));
                //    Activity.IsRunning = false;
                //    LoginIN.IsEnabled = true;
                //    Name.IsEnabled = true;
                //    return;
                //}

                Request.GetDetailInfo(human);
                try
                {
                    Request.GetSchedule(human);
                }
                catch(Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() => DisplayAlert("Ошибка", $"Проблема с автоматическим поиском группы {human.DetailInfo.gname}. Заработает чутка позже, извини :(", "Ок"));
                    Activity.IsRunning = false;
                    LoginIN.IsEnabled = true;
                    Name.IsEnabled = true;
                    return;
                }
               

                Preferences.Set("Name", Name.Text);
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.Current.MainPage = new MainMenu(human);
                });
                HumanSerilaze h = new HumanSerilaze(human);
                SerializeObject(h);
                // 
            });
            
        }
        public static void SerializeObject(object sender)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sample.txt");
            var list = sender;
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fileStream, list);
            fileStream.Close();
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
    [Serializable]
    public class HumanSerilaze
    {
        public HumanSerilaze(Human human)
        {
            sh_SecondName = human.ShortInfo.SecondName;
            sh_FirstName = human.ShortInfo.FirstName;
            sh_ThirdName = human.ShortInfo.ThirdName;
            sh_Departmant = human.ShortInfo.Departmant;
            sh_Faculty = human.ShortInfo.Faculty;
            sh_Position = human.ShortInfo.Position;
            sh_ID = human.ShortInfo.ID;
            sh_IsTeacher = human.ShortInfo.IsTeacher;

            dt_fio = human.DetailInfo.fio;
            dt_gname = human.DetailInfo.gname;
            dt_kvrname = human.DetailInfo.kvrname;
            dt_drrname = human.DetailInfo.drrname;
            dt_drcod = human.DetailInfo.drcod; 
            dt_flrname = human.DetailInfo.flrname;
            dt_id1c = human.DetailInfo.id1c;
            dt_id = human.DetailInfo.id;
            dt_avfoto = human.DetailInfo.avfoto;
            dt_mail = human.DetailInfo.mail;
            dt_depname = human.DetailInfo.depname;
            dt_parentdepname = human.DetailInfo.parentdepname;
            dt_fc = human.DetailInfo.fc;
            dt_namerp = human.DetailInfo.namerp;
            dt_codkaf = human.DetailInfo.codkaf;
            dt_codgr = human.DetailInfo.codgr;
            dt_kaflname = human.DetailInfo.kaflname;
            dt_pthdata = human.DetailInfo.pthdata;
            dt_URIForSchedule = human.DetailInfo.URIScheduleGroup.ToString();

        }

        public string sh_SecondName;
        public string sh_FirstName;
        public string sh_ThirdName;
        public string sh_Departmant;
        public string sh_Faculty;
        public string sh_Position;
        public int sh_ID;
        public bool sh_IsTeacher;

        public string dt_fio;
        public string dt_gname;
        public string dt_kvrname;
        public string dt_drrname;
        public string dt_drcod;
        public string dt_flrname;
        public string dt_id1c;
        public string dt_id;
        public string dt_avfoto;
        public string dt_mail;
        public string dt_depname;
        public string dt_parentdepname;
        public string dt_fc;
        public string dt_namerp;
        public string dt_codkaf;
        public string dt_codgr;
        public string dt_kaflname;
        public string dt_pthdata;

        public string dt_URIForSchedule;
    }
    [Serializable]
    public class LessonsSerilaze
    {
        public string Name, Teacher,Auditorium,PerfectTime;
        public LessonsType type;
        public DateTime startLesson, endLesson;
        public LessonsSerilaze(Format.Lesson lesson, DateTime dateTime)
        {
            Name = lesson.Title;
            Teacher = lesson.TeacherFIO;
            Auditorium = lesson.Auditorium;
            PerfectTime = lesson.PerfectTime;

            DateTime now = DateTime.Now;
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            switch (lesson.Number)
            {
                case 0:
                    startLesson = date.Add(new TimeSpan(8, 30, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 1:
                    startLesson = date.Add(new TimeSpan(10, 15, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 2:
                    startLesson = date.Add(new TimeSpan(12, 10, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 3:
                    startLesson = date.Add(new TimeSpan(13, 55, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 4:
                    startLesson = date.Add(new TimeSpan(15, 35, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 5:
                    startLesson = date.Add(new TimeSpan(18, 55, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 6:
                    startLesson = date;
                    endLesson = startLesson.AddMinutes(90);
                    break;
                default:
                    break;
            }
        }
    }
    public class HumanView
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string GroupOrTeacher { get; set; }
        public Human Human { get; set; }
        public Command Click { get; set; }
    }
}
