namespace eVision.eCrypt.KeyGenerator.Helpers
{
    using System;
    using System.Windows;

    using Encryption.Helpers;

    using Ookii.Dialogs.Wpf;

    internal static class CustomMessageBox
    {
        private const string Space = " ";

        public static MessageBoxResult Show(string description, string title = null, string yesText = null, string noText = null)
        {
            Contract.Requires<ArgumentException>(yesText != noText, "No text can't be the same as Yea button text", nameof(noText));
            Contract.Requires<ArgumentException>(noText != ButtonType.Yes.ToString(), "No text can't be Yes", nameof(noText));

            string yes = yesText ?? ButtonType.Yes.ToString();
            string no = noText ?? ButtonType.No.ToString();
            var dialog = new TaskDialog
            {
                WindowTitle = string.IsNullOrWhiteSpace(title) ? Space : title,
                MainInstruction = description,
                Buttons = { new TaskDialogButton(yes), new TaskDialogButton(no) }
            };

            TaskDialogButton selectedButton = dialog.ShowDialog();
            return selectedButton.Text == yes ? MessageBoxResult.Yes : MessageBoxResult.No;
        }
    }
}
