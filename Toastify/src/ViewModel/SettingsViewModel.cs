using System;
using System.Windows.Forms;
using Toastify.Common;
using Toastify.Helpers;
using Toastify.Model;

namespace Toastify.ViewModel
{
    public class SettingsViewModel : ObservableObject
    {
        public Settings Settings { get; }

        #region Commands

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand DefaultCommand { get; }

        public DelegateCommand SelectFileForSavingTrackCommand { get; }

        public DelegateCommand<string> ChangeToastColorTopCommand { get; }

        public DelegateCommand<string> ChangeToastColorBottomCommand { get; }

        public DelegateCommand<string> ChangeToastBorderColorCommand { get; }

        #endregion Commands

        /// <summary>
        /// Occurs when after a Save command.
        /// </summary>
        public event EventHandler SettingsSaved;

        public SettingsViewModel()
        {
            this.Settings = Settings.Instance.Clone();

            this.SaveCommand = new DelegateCommand(this.Save);
            this.DefaultCommand = new DelegateCommand(this.Default);
            this.SelectFileForSavingTrackCommand = new DelegateCommand(this.SelectFileForSavingTrack);
            this.ChangeToastColorTopCommand = new DelegateCommand<string>(hex => this.ChangeToastColor(ColorChangeTarget.ToastTop, hex));
            this.ChangeToastColorBottomCommand = new DelegateCommand<string>(hex => this.ChangeToastColor(ColorChangeTarget.ToastBottom, hex));
            this.ChangeToastBorderColorCommand = new DelegateCommand<string>(hex => this.ChangeToastColor(ColorChangeTarget.ToastBorder, hex));
        }

        private void Save()
        {
            this.Settings.Save(true);
            this.SettingsSaved?.Invoke(this, EventArgs.Empty);
        }

        private void Default()
        {
            this.Settings.Default();
            this.Save();
        }

        private void SelectFileForSavingTrack()
        {
            var dialog = new OpenFileDialog()
            {
                FileName = Settings.Instance.SaveTrackToFilePath ?? string.Empty,
                CheckPathExists = true,
                CheckFileExists = false,
                ShowReadOnly = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                this.Settings.SaveTrackToFilePath = dialog.FileName;
        }

        private void ChangeToastColor(ColorChangeTarget target, string hexColor)
        {
            string alpha = hexColor.Substring(1, 2);
            ColorDialog dialog = new ColorDialog
            {
                Color = ColorHelper.HexToColor(hexColor)
            };
            dialog.ShowDialog();

            var color = dialog.Color;
            string newHexColor = $"#{alpha}{color.R:X2}{color.G:X2}{color.B:X2}";

            switch (target)
            {
                case ColorChangeTarget.ToastTop:
                    this.Settings.ToastColorTop = newHexColor;
                    break;

                case ColorChangeTarget.ToastBottom:
                    this.Settings.ToastColorBottom = newHexColor;
                    break;

                case ColorChangeTarget.ToastBorder:
                    this.Settings.ToastBorderColor = newHexColor;
                    break;
            }
        }

        private enum ColorChangeTarget
        {
            ToastTop,
            ToastBottom,
            ToastBorder
        }
    }
}