using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace tkc
{
    class Program
    {
        private static string ts;

        static void Main(string[] args)
        {
            Console.Title = "Iniciando...";

            while (true)
            {
                ini();
            }
        }

        static void ini() //Inicia um OpenFIleDIalog com filtro para so pegar .txt, aonde fica os tokens seprardos por linhas
        {
            try
            {
                OpenFileDialog of = new OpenFileDialog
                {
                    InitialDirectory = @"C:\",
                    Title = "Selecione os tokens",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "txt",
                    Filter = "txt files (*.txt)|*.txt",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (of.ShowDialog() == DialogResult.OK)
                {
                    check(of.FileName);

                }
            }
            catch (Exception ex)
            {
                Console.Title = "Erro Detectado!!";
                ColorConsole.WriteEmbeddedColorLine($"[red]Erro[/red] detectado, salvado infos do erro no [red]Errolog.txt[/red]\n");
                File.WriteAllText("Errorlog.txt", ex.ToString());
                Console.ReadLine();
            }
        }

        static void check(string token)
        {
            try
            {
                Random r = new Random();
                int sucess = 0;
                int fail = 0;

                string[] filelines = File.ReadAllLines(token.Replace("\"", ""));

                foreach (string acesso in filelines)
                {
                    Console.Title = $"Token Cheker | Token: {filelines.Length} | Sucess: {sucess} | Failed: {fail}";
                    try
                    {
                        var client = new RestClient("https://canary.discord.com");       ///Faz a request get com o token para ver se ele existe
                        var request = new RestRequest("api/v8/users/@me", Method.GET);
                        request.AddHeader("Authorization", acesso);
                        client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/0.0.305 Chrome/69.0.3497.128 Electron/4.0.8 Safari/537.36";

                        var content = client.Execute(request);
                        

                        if (content.Content.Contains("401: Unauthorized"))
                        {
                            ColorConsole.WriteEmbeddedColorLine("[darkred]INVALIDO [/darkred]- Token invalido [red]" + acesso + "[/red] \n");   //Escreve na tela quando o token esta invalido
                            fail++;
                        }
                        else if (content.Content.Contains("\"username\": "))
                        {
                            ColorConsole.WriteEmbeddedColorLine($"[darkgreen]VALIDO [/darkgreen]- Token:[green] {acesso} [/green] \n"); //Escreve quando a tela esta valida
                            ts += $"{acesso} \n";
                            File.WriteAllText("tokens.txt", ts);
                            sucess++;
                        }
                        else if (content.Content.Contains("has banned you temporarily from accessing this website."))
                        {
                            ColorConsole.WriteEmbeddedColorLine("[darkred]ERRO [/darkred]-[red] Seu ip levou temp ban, mude o endpoint [/red] \n"); //Mostra se o ip levou ban
                            Console.ReadLine();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Title = "Erro Detectado!!";
                        ColorConsole.WriteEmbeddedColorLine($"[red]Erro[/red] detectado, salvado infos do erro no [red]Errolog.txt[/red]\n");
                        File.WriteAllText("Errorlog.txt", ex.Message);
                        Console.ReadLine();
                    }
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Title = "Erro Detectado!!";
                ColorConsole.WriteEmbeddedColorLine($"[red]Erro[/red] detectado, salvado infos do erro no [red]Errolog.txt[/red]\n");
                File.WriteAllText("Errorlog.txt", ex.Message);
                Console.ReadLine();
            }
        }
    }
}