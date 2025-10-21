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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        // добавим новое поле, кот будет харнить в себе экземпляр добавляемого сервиса
        private Service _currentServise = new Service();
        
        public AddEditPage(Service SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
                _currentServise = SelectedService;

            // при инициализации установим DataContext страницы - этот созданный объект
            DataContext = _currentServise;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentServise.Title))
                errors.AppendLine("Укажите название услуги");

            if (_currentServise.Cost == 0)
                errors.AppendLine("Укажите стоимость услуги");

            if (_currentServise.DiscountIt < 0 && _currentServise.DiscountIt >= 100)
                errors.AppendLine("Укажите скидку от 0 до 100");

            if (_currentServise.DurationInSeconds == 0)
                errors.AppendLine("Укажите длительность услуги");

            if (_currentServise.DurationInSeconds > 240 || _currentServise.DurationInSeconds < 0)
                errors.AppendLine("Длительность не может быть больше 240 минут и меньше 0 минут");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }


            var allServices = Vakhitova_AutoserviceEntities.GetContext().Service.ToList();
            allServices = allServices.Where(p => p.Title == _currentServise.Title).ToList();
            
            if (allServices.Count == 0)
            {
                // добавить в контекст текущие значения новой услуги
                if (_currentServise.ID == 0)
                    Vakhitova_AutoserviceEntities.GetContext().Service.Add(_currentServise);

                // сохранить изменения если при этом не получилось никаких ошибок
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

            else
            {
                MessageBox.Show("Уже существует такая услуга");
            }
        }
    }
}
