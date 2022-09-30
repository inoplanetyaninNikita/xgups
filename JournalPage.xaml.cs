using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XGups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JournalPage : ContentPage
    {
        public string Lek { get; set; } = "30";
        public string Lab { get; set; } = "30";
        public string All { get; set; } = "30";
        public ObservableCollection<Human.Detail.JLesson> Lessons { get; set; } = new ObservableCollection<Human.Detail.JLesson>();
        public JournalPage()
        {
            var h = Request.GetHuman("Глазырина", "Ксения")[0];
            Request.GetDetailInfo(h);
            Request.GetJournal(h);

            {
                Lek = h.DetailInfo.Student.emlek;
                Lab = h.DetailInfo.Student.emlab;
                All = h.DetailInfo.Student.emall;
            }
            
            InitializeComponent();
            for (int i = 0; i < h.DetailInfo.Student.JLessons.Count; i++)
            {
                Lessons.Add(h.DetailInfo.Student.JLessons[i]);
            }
            BindingContext = this;
            
        }
        public JournalPage(Human human)
        {
            //if(human.DetailInfo.Student == null) Request.GetJournal(human);
            Request.GetJournal(human);
            InitializeComponent();
            {
                Lek = human.DetailInfo.Student.emlek;
                Lab = human.DetailInfo.Student.emlab;
                All = human.DetailInfo.Student.emall;
            }
            for (int i = 0; i < human.DetailInfo.Student.JLessons.Count; i++)
            {
                Lessons.Add(human.DetailInfo.Student.JLessons[i]);
            }
            BindingContext = this;
        }
    }
}