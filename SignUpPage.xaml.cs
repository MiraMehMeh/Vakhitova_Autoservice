using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Vakhitova_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        // добавим новое поле кот будет хранить в себе экземпляр добавляемого сервиса
        private Service _currentService = new Service();

        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
                this._currentService = SelectedService;

            // при инициализации установим Datacontext страницы - этот созданный объект
            // чтобы на форму подггрузить выбранные наименования услуги и длительность
            DataContext = _currentService;

            //вытащим из бд табл клиент
            var _currentClient = Vakhitova_AutoserviceEntities.GetContext().Client.ToList();
            // свяжем с комбобоксом
            ComboClient.ItemsSource = _currentClient;
        }

        private ClientService _currentClientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Добавить текущие значения новой записи
            _currentClientService.ClientID = ComboClient.SelectedIndex + 1; // тк нумерация с 0
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                Vakhitova_AutoserviceEntities.GetContext().ClientService.Add(_currentClientService);

            //сохранить изм-я если никаких ошибок не получилось при этом
            try
            {
                Vakhitova_AutoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            string[] start = s.Split(':');

            if (start.Length != 2)
            {
                TBEnd.Text = "Неверный формат времени";
                return;
            }

            if (s.Length != 5 || s[2] != ':')
            {
                TBEnd.Text = "Неверный формат времени (должно быть HH:mm)";
                return;
            }

            if (!int.TryParse(start[0], out int startHour) || !int.TryParse(start[1], out int startMinute))
            {
                TBEnd.Text = "Неверный формат времени";
                return;
            }

            // Добавляем проверки на часы и минуты
            if (startHour < 0 || startHour > 23 || startMinute < 0 || startMinute > 59)
            {
                TBEnd.Text = "Неверный формат времени (часы: 0-23, минуты: 0-59)";
                return;
            }

            int totalMinutes = startHour * 60 + startMinute + _currentService.DurationInSeconds;
            int EndHour = totalMinutes / 60;
            int EndMin = totalMinutes % 60;

            EndHour = EndHour % 24; // Это остается для обработки переполнения

            s = EndHour.ToString("D2") + ":" + EndMin.ToString("D2");
            TBEnd.Text = s;

        }
    }
}
