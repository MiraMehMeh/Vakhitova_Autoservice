using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();
            //добавляем строки
                //загрузить в список из бд
            var currentServices = Vakhitova_AutoserviceEntities.GetContext().Service.ToList();
                // связать с нашим листвью
            ServiceListView.ItemsSource = currentServices;
            //добавили строки

            DataContext = Vakhitova_AutoserviceEntities.GetContext();

            ComboType.SelectedIndex = 0;

            //вызываем UpdateServices()
            UpdateServices();
        }

        int CountRecords; // кол-во записей в таблице
        int CountPage; // общее кол-во страниц
        int CurrentPage = 0; // текущая страница

        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;

        private void ChangePage(int direction, int? selectedPage) // функция отвечающая за разделение list`а
        {
            // direction - направление.   0 - начало, 1 - пред. стр.,  2 - след.стр.
            // selectedPage - при нажатии на стрелочки передается null,
            // при выборе определенной страницы в этой переменной находится номер страницы
          
            CurrentPageList.Clear();  // начальная очистка листа
            CountRecords = TableList.Count;  // определение кол-ва записей во всем списке
            // определение кол-ва страниц
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }

            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;
            // проверка на правильность - если
            // currentpage (номер текущей страницы) "правильный"

            int min;

            if (selectedPage.HasValue) // проверка на значение не null (тк мб null)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }

            }

            else   // если нажата стрелка
            {
                switch (direction)
                {
                    case 1:  //нажата кнопка "Предыдущая стр"
                        if (CurrentPage > 0) // т.е. кнопка нажата правильно и "назад" можно идти
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;

                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }

                        else   
                        {
                            Ifupdate = false;
                            // в случае если currentpage пытается выйти из диапазона внесение данных не произойдет
                        }

                        break;

                    case 2: // нажата кнопка "следующая стр" 
                        if (CurrentPage < CountPage - 1)  // если вперед идти можно
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }

                        else
                        {
                            Ifupdate = false;
                        }

                        break;

                }
            }

            if (Ifupdate) // если current page не вышел из диапазона то
            {
                PageListBox.Items.Clear();
                // удаление старых знач из listbox`a номеров страниц, нужно чтобы при изменении 
                // кол-ва записей кол-во страниц динамически изменялось

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                //  вывод колва записей на странице и общего колва
                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();

                ServiceListView.ItemsSource = CurrentPageList;
                //обновить отображение списка услуг
                ServiceListView.Items.Refresh();
            }
        }


        private void UpdateServices()
        {
            //берем из бд данные таблицы Сервис
            var currentServices = Vakhitova_AutoserviceEntities.GetContext().Service.ToList();

            //прописываем фильтрацию по условию задачи
            if (ComboType.SelectedIndex == 0)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.DiscountIt) >= 0 && Convert.ToInt32(p.DiscountIt) <= 100)).ToList();
            }

            if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.DiscountIt) >= 0 && Convert.ToInt32(p.DiscountIt) < 5)).ToList();
            }

            if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.DiscountIt) >= 5 && Convert.ToInt32(p.DiscountIt) < 15)).ToList();
            }

            if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.DiscountIt) >= 15 && Convert.ToInt32(p.DiscountIt) < 30)).ToList();
            }

            if (ComboType.SelectedIndex == 4)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.DiscountIt) >= 30 && Convert.ToInt32(p.DiscountIt) < 70)).ToList();
            }

            if (ComboType.SelectedIndex == 5)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.DiscountIt) >= 70 && Convert.ToInt32(p.DiscountIt) < 100)).ToList();
            }

            //реализуем поиск данных в листвью при вводе текста в окно поиска
            currentServices = currentServices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            //для отображения итогов фильтра и поиска в листвью
            ServiceListView.ItemsSource = currentServices.ToList();

            if (RButtonDown.IsChecked.Value)
            {
                //для отображения итогов фильтра и поиска в листвью по убыванию
                currentServices = currentServices.OrderByDescending(p => p.Cost).ToList();
            }

            if (RButtonUp.IsChecked.Value)
            {
                //для отображения итогов фильтра и поиска в листвью по возрастанию
                currentServices = currentServices.OrderBy(p => p.Cost).ToList();
            }

            // для отображения итогов фильтра и поиска в листвью
            ServiceListView.ItemsSource = currentServices;
            //заполнение таблицы для постраничного вывода
            TableList = currentServices;
            // вызов функции отображения кол-ва страниц с параметрами:
            // направление 0 - начальная загрузка
            // 0 - выбранная страница
            ChangePage(0, 0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // забираем сервис для кот. нажата кнопка удалить
            var currentService = (sender as Button).DataContext as Service;

            // проверка на возможность удаления
            var currentClientServices = Vakhitova_AutoserviceEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentService.ID).ToList();

            if (currentClientServices.Count != 0)  // если есть записи на этот сервис
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу");

            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Vakhitova_AutoserviceEntities.GetContext().Service.Remove(currentService);
                        Vakhitova_AutoserviceEntities.GetContext().SaveChanges();

                        // выводим в листвью измененную таблицу сервис
                        ServiceListView.ItemsSource = Vakhitova_AutoserviceEntities.GetContext().Service.ToList();
                        // чтобы применились фильтры и поиск, если они были на форме изначально
                        UpdateServices();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно редактирования/добавления услуг
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // открыть окно редактирования/добавления услуг
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Vakhitova_AutoserviceEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Vakhitova_AutoserviceEntities.GetContext().Service.ToList();
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно записи клиента на выбранную услугу
            Manager.MainFrame.Navigate(new SignUpPage((sender as Button).DataContext as Service));
        }
    }
}
