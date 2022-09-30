using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XGups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GiveawayPage : ContentPage
    {
        Human human;
        
        public GiveawayPage()
        {
            InitializeComponent();
           
            if (DateTime.Now > new DateTime(2022,09,1))
            {
                isWinner();
            }
            else
            {
                if (Preferences.Get("RegisterGUID", "") != "")
                {
                    isOnRegister();
                }
            }
        }
        public GiveawayPage(Human human)
        {
            InitializeComponent();

            BindingContext = this;

            this.human = human;

            HumanName.Text = $"{human.ShortInfo.SecondName} {human.ShortInfo.FirstName}";
            if (DateTime.Now > new DateTime(2022, 09, 1))
            {
                isWinner();
            }
            else
            {
                if (Preferences.Get("RegisterGUID", "") != "")
                {
                    isOnRegister();
                }
            }
        }
        private void Register_Clicked(object sender, EventArgs e)
        {
            Activity.IsRunning = true;
            Register.IsEnabled = false; 

            Task.Run(() =>
            {
                try
                {
                    var a = Request.GiveawayTESTAsync(human.ShortInfo.FirstName, human.ShortInfo.SecondName);
                    Preferences.Set("RegisterGUID", a);
                    isOnRegister();
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Ошибка", "Проблема с соединением, попробуй перезагрузить мобильный интернет и нажимай еще раз!", "Ок");
                        Activity.IsRunning = false;
                        Register.IsEnabled = true;
                    });
                }
            }); 

        }
        private void isOnRegister()
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                Description.Text = "Отлично! Осталось узнать результаты, они будут - 26 сентября.";
                HumanName.IsVisible = false;
                Register.IsVisible = false;
                Activity.IsRunning = false;
            });
        }
        private void isWinner()
        {
            Description.Text = "Отлично! Осталось узнать результаты, они будут - 26 .";
            HumanName.IsVisible = false;
            Register.IsVisible = false;
            Activity.IsRunning = false;

            #region Веб запрос
            Uri uri = new Uri($"http://www.xgups.ru/winner");
            WebRequest request = WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();

            string queryData;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();
            #endregion

            string[] words = queryData.Split(' ');

            human = new Human();
            human.ShortInfo.SecondName = "Тороповский";
            human.ShortInfo.FirstName = "Никита";

            if ( human.ShortInfo.SecondName == words[0] && human.ShortInfo.FirstName == words[1] && Preferences.Get("RegisterGUID", "") == words[2])
            {
                Description.Text = "Поздравляем, ты выиграл! Напиши мне в вк - vk.com/ntoropovsky";
            }
            else
            {
                Description.Text = $"Повезет в следующий раз!";
                HumanName.Text = $"Выиграл: {words[0]} {words[1]}";
                HumanName.IsVisible = true;
            }

        }
    }
}