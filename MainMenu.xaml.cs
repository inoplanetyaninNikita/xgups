using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XGups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu : ContentPage
    {
        
        public static bool addUpdateIsAlreadyBeen = true;
        List<string> mounths = new List<string>()
        {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь" ,"Декабрь"
        };
        public static bool isEvenWeek(DateTime time)
        {
            int days = ((time - new DateTime(2022, 3, 14)).Days);
            float d = (days / 7) % 2;

            if (d % 2 == 1) return true;
            else return false;
        }

        public Human human;
        public DaysView LastSelectedView;

        private static DateTime thisTime;
        public ObservableCollection<LessonsView> NewLessons { get; set; } = new ObservableCollection<LessonsView>();
        public ObservableCollection<DaysView> DaysOnCalendar { get; set; }


        public Command Logout { get; set; }
        public Command ReverseSchedule { get; set; }

        public MainMenu()
        {
            OnStart();
        }
        public MainMenu(Human human)
        {
            

            LocalNotificationCenter.Current.CancelAll();
            this.human = human;
            OnStart();
            OpenDay(DateTime.Now);

            if(addUpdateIsAlreadyBeen)
            {
                GridBackgroundLock.IsVisible = false; 
                GridUpdate.IsVisible = false;
            }
            else
            {
                var byteArray = new WebClient().DownloadData("http://www.xgups.ru/UpdatePages/Update.jpg");
                updImage.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
            }
            

            //GroupLabel.Text = human.DetailInfo.gname;

        }
        public void OpenDay(DateTime datetime)
        {
            thisTime = datetime;
            DateLabel.Text = $"{mounths[datetime.Month - 1]}, {datetime.Year}";
            
            NewLessons.Clear();

            if (datetime.DayOfWeek != DayOfWeek.Sunday)
            {
                Format.Week week;
                int day = (int)datetime.DayOfWeek-1;

                if (isEvenWeek(datetime) == true) week = human.EvenWeek;
                else week = human.NotEvenWeek;
                
                for (int i = 0; i < week.Days[day].Lessons.Count; i++)
                {
                    if (week.Days[day].Lessons[i].Title != null && week.Days[day].Lessons[i].FullName.Contains("_") == false)
                    {
                        //week.Days[day].Lessons[i]
                        //Lessons.Add(week.Days[day].Lessons[i]);
                        var item = new LessonsView(week.Days[day].Lessons[i], datetime);
                        NewLessons.Add(item);

                        if(item.Type == LessonsType.Today) App.LessonsTime.Add(item);
                    }
                }
            }
            else
            {
                AnimParty();
            }
        }
        private bool haveLessons(DateTime datetime)
        {
            bool sucsess = false;
            if (datetime.DayOfWeek != DayOfWeek.Sunday)
            {
                Format.Week week;
                int day = (int)datetime.DayOfWeek - 1;

                if (isEvenWeek(datetime) == true) week = human.EvenWeek;
                else week = human.NotEvenWeek;

                int lessonCount = 0;
                for (int i = 0; i < week.Days[day].Lessons.Count; i++)
                {
                    if (week.Days[day].Lessons[i].Title != null && week.Days[day].Lessons[i].FullName.Contains("_") == false)
                    {
                        sucsess = true;
                        createNotify(datetime, week.Days[day].Lessons[i],lessonCount);
                        lessonCount++;
                    }
                }
            }
            return sucsess;
        }
        Random r = new Random();
        private void createNotify(DateTime datetime, Format.Lesson lesson, int lessonNum)
        {
            if (datetime.Day >= DateTime.Now.Day && datetime.Day <= DateTime.Now.Day + 7)
            {
                {
                    var sch = new NotificationRequestSchedule();
                    int hour = 0, minute = 0;
                    switch (lesson.Number)
                    {
                        case 0:
                            hour = 8; minute = 30;
                            break;
                        case 1:
                            hour = 10; minute = 15;
                            break;
                        case 2:
                            hour = 12; minute = 10;
                            break;
                        case 3:
                            hour = 13; minute = 55;
                            break;
                        case 4:
                            hour = 15; minute = 35;
                            break;
                        case 5:
                            hour = 17; minute = 15;
                            break;
                        case 6:
                            hour = 18; minute = 55;
                            break;
                        default:
                            return;
                            break;
                    }

                    sch.NotifyTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, hour, minute, 0).Subtract(new TimeSpan(0, 15, 0));
                    var notify = new NotificationRequest()
                    {
                        BadgeNumber = 0,
                        Description = $"{lesson.FullName} в {lesson.Auditorium}",
                        Title = "Пара",
                        Subtitle = "Через 15 минут",
                        ReturningData = "Data",
                        NotificationId = sch.NotifyTime.Value.Month * 100 + sch.NotifyTime.Value.Day * 10 + lesson.Number,
                        Schedule = sch,
                        CategoryType = NotificationCategoryType.Event
                    };
                    LocalNotificationCenter.Current.Show(notify);
                }
                if(lessonNum == 0)
                {
                    var sch = new NotificationRequestSchedule();
                    int hour = 0, minute = 0;
                    switch (lesson.Number)
                    {
                        case 0:
                            hour = 8; minute = 30;
                            break;
                        case 1:
                            hour = 10; minute = 15;
                            break;
                        case 2:
                            hour = 12; minute = 10;
                            break;
                        case 3:
                            hour = 13; minute = 55;
                            break;
                        case 4:
                            hour = 15; minute = 35;
                            break;
                        case 5:
                            hour = 17; minute = 15;
                            break;
                        case 6:
                            hour = 18; minute = 55;
                            break;
                        default:
                            return;
                            break;
                    }

                    sch.NotifyTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, hour, minute, 0).Subtract(new TimeSpan(1, 0, 0));
                    var notify = new NotificationRequest()
                    {
                        BadgeNumber = 0,
                        Description = $"{lesson.FullName} в {lesson.Auditorium}",
                        Title = "Пара",
                        Subtitle = "Через час",
                        ReturningData = "Data",
                        NotificationId = sch.NotifyTime.Value.Month * 1000 + sch.NotifyTime.Value.Day * 100,
                        Schedule = sch,
                        CategoryType = NotificationCategoryType.Event
                    };
                    LocalNotificationCenter.Current.Show(notify);
                }
            }
           
            return;
           
        }
        public async void AnimParty()
        {
            PartyIMG.Opacity = 0;
            PartyIMG.Scale = 0;

            PartyText.Opacity = 0;
            PartyText.Scale = 0;
            

            await Task.Run(() =>
            {
                PartyIMG.FadeTo(1, 500);
                PartyIMG.ScaleTo(1, 500);
            });
            await Task.Run(() =>
            {
                PartyText.FadeTo(1, 500);
                PartyText.ScaleTo(1, 500);
            });

               
        }
        public async void OnStart()
        {
            InitializeComponent();

            Logout = new Command(() =>
            {
                App.Current.MainPage = new MainPage();
            });
            ReverseSchedule = new Command(async () =>
            {
                if(human.ShortInfo.IsTeacher == true)
                {
                    return;
                }
                var action = await DisplayActionSheet("Настройки", "Отмена", "", "Посмотреть журнал","Посмотреть оценки") ;

                if (action != null && action == "Посмотреть журнал")
                {
                    Navigation.PushModalAsync(new JournalPage(human));
                }
                if (action != null && action == "Перевернуть расписание")
                {
                    if (human.EvenWeek != null && human.NotEvenWeek != null)
                    {
                        Format.Week swap = human.EvenWeek;
                        human.EvenWeek = human.NotEvenWeek;
                        human.NotEvenWeek = swap;
                        OpenDay(thisTime);
                    }
                    Request.reverse = !Request.reverse;
                    Preferences.Set("Reverse", Request.reverse);
                } 
                if(action != null && action == "Посмотреть оценки")
                {
                    Navigation.PushModalAsync(new Marks(human));
                }
            });
            NewLessons = new ObservableCollection<LessonsView>();

            DaysOnCalendar = new ObservableCollection<DaysView>();
            for (int i = 2; i > -30; i--)
            {
                bool lessons = haveLessons(DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)));
                var day = new DaysView(DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)),lessons)
                {
                    page = this,
                };
                DaysOnCalendar.Add(day);
            }

            BindingContext = this;
        }
        public void GiveawayAI()
        {
           // Navigation.PushModalAsync(new GiveawayPage(human));
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            await GridBackgroundLock.FadeTo(0, 200, Easing.Linear);
            await GridUpdate.FadeTo(0, 200, Easing.CubicOut);
            GridBackgroundLock.IsVisible = false;
            GridUpdate.IsVisible = false;
            addUpdateIsAlreadyBeen = true;
        }
    }

}