﻿using System;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using AutoUpdaterDotNET;

namespace SwitchHacksAllInOne
{
    /// <summary>
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update : MetroWindow
    {
        private readonly MainWindow _mainWindow;
        public string SearchPattern { get; set; }

        public Update(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            SearchPattern = "updater*.exe";

            InitializeComponent();
        }

        private void SearchForUpdates(object sender, RoutedEventArgs e)
        {
            SearchForUpdaters();


        }

        private void GoBackToMainFrm(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainWindow.Show();
        }

        /// <summary>
        /// Searches in the current directory for all files named after SearchPattern (updater*.exe),
        /// executes them and sets the Progressbar.
        /// </summary>
        public void SearchForUpdaters()
        {
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());
            DirectoryInfo[] directories = di.GetDirectories(SearchPattern, SearchOption.AllDirectories);
            FileInfo[] files = di.GetFiles(SearchPattern, SearchOption.AllDirectories);

            if (files.Length <= 0)
            {
                CouldntUpdate();
                return;
            }
            ProgressBar.Maximum = files.Length;
            ExecuteUpdaters(files);

            //if progressbar == maximum with tolerance of 1
            if (Math.Abs(ProgressBar.Value - ProgressBar.Maximum) < 1)
            {
                CouldUpdate();
            }
            else
            {
                CouldntUpdate();
            }

        }

        private void ExecuteUpdaters(FileInfo[] files)
        {
            foreach (FileInfo file in files)
            {
                var process = System.Diagnostics.Process.Start(file.FullName);
                process?.WaitForExit();
                ProgressBar.Value += 1;
                DoEvents();
            }
        }

        /// <summary>
        /// Shows Messagebox to notify you that the updating process wasn't successful
        /// </summary>
        public void CouldntUpdate()
        {
            MessageBox.Show("Something went wrong/some elements couldn't be updated");
        }

        /// <summary>
        /// Shows Messagebox to notify you that the updating process was successful
        /// </summary>
        public void CouldUpdate()
        {
            MessageBox.Show("Everything was updated!");
            this.Close();
        }

        /// <summary>
        /// Workaround method to update UI elements while in loop blocking the thread.
        /// </summary>
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));
        }

        private void UpdateSHAIO(object sender, RoutedEventArgs e)
        {
            AutoUpdater.Start();
        }
    }
}
