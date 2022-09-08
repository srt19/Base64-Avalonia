using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Text;
using System.IO;

namespace Base64_Avalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var Enc_rad_clik = this.FindControl<RadioButton>("Enc_rad");
            var Dec_rad_click = this.FindControl<RadioButton>("Dec_rad");
            var Convert_click = this.FindControl<Button>("Convert_button");
            var Browse_click = this.FindControl<Button>("Browse_sav");
            
            Enc_rad_clik.Click += CheckMode;
            Dec_rad_click.Click += CheckMode;
            Convert_click.Click += Run_convert;
            Browse_click.Click += Browse_txt;
        }
        protected internal bool B64mode = true;
        protected internal string SavPath = string.Empty;
        protected internal string LogDir = @"Log\";
        protected internal void CheckMode(object? sender, RoutedEventArgs e)
        {
            if (B64mode == true)
            {
                Convert_button.Content = "Decode";
                B64mode = false;
                Url_cek.IsEnabled = true;
                return;
            }
            else if (B64mode == false)
            {
                Convert_button.Content = "Encode";
                B64mode = true;
                Url_cek.IsEnabled = false;
                return;
            }
        }
        protected internal async void Browse_txt(object? sender, RoutedEventArgs e)
        {
            SaveFileDialog fdlg = new()
            {
                Title = "Save txt file",
            };
            fdlg.Filters!.Add(new FileDialogFilter() { Name = "txt files", Extensions = { "txt" } });
            fdlg.Filters.Add(new FileDialogFilter() { Name = "csv files", Extensions = { "csv" } });
            var result = await fdlg.ShowAsync(this);

            if (result != null)
            {
                SavPath = result.ToString();
                Sav_text.IsEnabled = true;
            }
            else
            {
                return;
            }
        }

        protected internal void Run_convert(object? sender, RoutedEventArgs e)
        {
            try
            {
                string input_text = Input_box.Text;
                string output_text = string.Empty;
                if (string.IsNullOrEmpty(input_text))
                {
                    Output_box.Text = "Input text is empty";
                    return;
                }

                if (B64mode == false && Url_cek.IsChecked == false)
                {
                    output_text = Decode64(input_text);
                    Output_box.Text = output_text;
                }

                else if (B64mode == false && Url_cek.IsChecked == true)
                {
                    string urltext = input_text.Replace("&", " ");
                    urltext = urltext.Replace("url=", "");
                    string[] list = urltext.Split();
                    urltext = list[1];
                    output_text = Decode64(urltext);
                    Output_box.Text = output_text;
                }

                else if (B64mode == true)
                {
                    byte[]? Inbytes = Encoding.UTF8.GetBytes(input_text);
                    output_text = Convert.ToBase64String(Inbytes);
                    Output_box.Text = output_text;
                }

                if (Sav_text.IsChecked != false)
                {
                    WriteTxt(input_text, output_text);
                }
            }
            catch (IndexOutOfRangeException err)
            {
                CheckLogDir();
                var ermes = $"[Error]: {err}" + Environment.NewLine;
                File.AppendAllText(@"Log\Error.log", ermes);
                Output_box.Text = "Please input full url link";
            }
            catch (FormatException err)
            {
                CheckLogDir();
                var ermes = $"[Error]: {err}" + Environment.NewLine;
                File.AppendAllText(@"Log\Error.log", ermes);
                Output_box.Text = "Please only input base64 string";
            }
            catch (Exception err)
            {
                CheckLogDir();
                var ermes = $"[Error]: {err}" + Environment.NewLine;
                File.AppendAllText(@"Log\Error.log", ermes);
                Output_box.Text = "Error occured, log created in Log\\Error.log";
            }
        }
        protected internal void CheckLogDir()
        {
            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }
        }

        protected internal static string Decode64(string x)
        {
            byte[]? Inbytes = Convert.FromBase64String(x);
            string Outtext = Encoding.UTF8.GetString(Inbytes);
            return Outtext;
        }

        protected internal void WriteTxt(string intext, string outtext)
        {
            if (SavPath.EndsWith(".csv"))
            {
                if (File.Exists(SavPath) != true)
                {
                    var headings = string.Format("{0};{1}", "Input", "Output" + Environment.NewLine);
                    var outs = string.Format("{0};{1}", intext, outtext + Environment.NewLine);
                    File.WriteAllText(SavPath, headings);
                    File.AppendAllText(SavPath, outs);
                }
                else
                {
                    var outs = string.Format("{0};{1}", intext, outtext + Environment.NewLine);
                    File.AppendAllText(SavPath, outs);
                }
            }

            else
            {
                File.AppendAllText(SavPath, outtext);
            }
        }
    }
}
