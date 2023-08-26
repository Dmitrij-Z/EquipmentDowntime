using System;
using System.IO;
using System.Windows;

namespace EquipmentDowntime
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("Непредвиденная ошибка:\r\n{0}", Msg(e));
            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //e.Handled = true;
        }
        private string Msg(System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.IO.StreamWriter writer;
            if (e.Exception.InnerException == null)
            {
                writer = new System.IO.StreamWriter(Path.Combine(Environment.CurrentDirectory, "EquipmentDowntime.txt"), true);
                writer.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + ": " + e.Exception.Message);
                writer.Close();
                return e.Exception.Message;
            }
            if (string.IsNullOrEmpty(e.Exception.InnerException.ToString()))
            {
                writer = new System.IO.StreamWriter(Path.Combine(Environment.CurrentDirectory, "EquipmentDowntime.txt"), true);
                writer.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + ": " + e.Exception.Message);
                writer.Close();
                return e.Exception.Message;
            }
            writer = new System.IO.StreamWriter(Path.Combine(Environment.CurrentDirectory, "EquipmentDowntime.txt"), true);
            writer.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + ": " + e.Exception.Message);
            writer.WriteLine("\r\nInnerException: " + e.Exception.InnerException);
            writer.Close();
            return e.Exception.InnerException.ToString();
        }
    }
}
