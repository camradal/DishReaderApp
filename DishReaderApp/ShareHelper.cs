using System;
using System.Windows;
using DishReaderApp.ViewModels;
using Microsoft.Phone.Tasks;

namespace DishReaderApp
{
    static internal class ShareHelper
    {
        internal static void ShareViaEmail(ShareViewModel model)
        {
            try
            {
                var task = new EmailComposeTask
                {
                    Subject = model.Title,
                    Body = string.Format("{0}\n\n{1}", model.Summary, model.Url)
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }

        internal static void ShareViaSocial(ShareViewModel model)
        {
            try
            {
                var task = new ShareLinkTask()
                {
                    Title = model.Title,
                    Message = model.Title,
                    LinkUri = new Uri(model.Url, UriKind.Absolute)
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }

        internal static void ShareViaSms(ShareViewModel model)
        {
            try
            {
                var task = new SmsComposeTask()
                {
                    Body = model.Title + "\n" + model.Url
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }

        internal static void ShareViaClipBoard(ShareViewModel model)
        {
            string text = model.Title + "\n" + model.Url;
            if (MessageBox.Show(text, "Copy to Clipboard?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Clipboard.SetText(text);
            }
        }
    }
}
