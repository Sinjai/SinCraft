using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Updater
{
    class Program
    {
        static int tries = 0;
        static bool usingConsole = false;
        static string parent = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        static string parentfullpathdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(globalException);
            try
            {
                string[] foundView = File.ReadAllLines("Viewmode.cfg");
                if (foundView[4].Split(' ')[2].ToLower() == "true")
                {
                    usingConsole = true;
                }
            }
            catch { }
            if (args.Length < 1)
            {
                Console.WriteLine("Updater was started incorrectly.");
                MessageBox.Show("Updater was started incorrectly.", "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            else
            {
                try
                {
                    if (args[0].Contains("securitycheck10934579068013978427893755755270374"))
                    {
                        args[0] = args[0].Replace("securitycheck10934579068013978427893755755270374", "");
                        if (args[0] == ".exe")
                            args[0] = "securitycheck10934579068013978427893755755270374.exe";
                        Console.WriteLine("Waiting for " + args[0] + " to exit...");
                        while (Process.GetProcessesByName(args[0]).Length > 0)
                        {
                            //Sit here and do nothing
                        }
                    }
                    else
                    {
                        Console.WriteLine("Updater was started incorrectly.");
                        MessageBox.Show("Updater was started incorrectly.", "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
                catch (Exception e) { UpdateFailure(e); }
                Update(args);
            }
        }
        static void Update(string[] args)
        {
            Console.WriteLine("Updating SinCraft...");
            try
            {
                tries++;
                if (File.Exists("SinCraft.update") || File.Exists("SinCraft_.update"))
                {
                    try
                    {
                        if (File.Exists("SinCraft.update"))
                        {
                            if (File.Exists(args[0]))
                            {
                                if (File.Exists("SinCraft.backup"))
                                    File.Delete("SinCraft.backup");
                                File.Move(args[0], "SinCraft.backup");
                            }
                            File.Move("SinCraft.update", args[0]);
                        }
                    }
                    catch (Exception e)
                    {
                        if (tries > 4)
                        {
                            UpdateFailure(e);
                        }
                        else
                        {
                            Console.WriteLine("\n\nAn error occured while updating.  Retrying...\n\n");
                            Thread.Sleep(100);
                            Update(args);
                        }
                    }
                    try
                    {
                        if (File.Exists("SinCraft_.update"))
                        {
                            if (File.Exists("SinCraft_.dll"))
                            {
                                if (File.Exists("SinCraft_.backup"))
                                    File.Delete("SinCraft_.backup");
                                File.Move("SinCraft_.dll", "SinCraft_.backup");
                            }
                            File.Move("SinCraft_.update", "SinCraft_.dll");
                        }
                    }
                    catch (Exception e)
                    {
                        if (tries > 4)
                        {
                            UpdateFailure(e);
                        }
                        else
                        {
                            Console.WriteLine("\n\nAn error occured while updating.  Retrying...\n\n");
                            Thread.Sleep(100);
                            Update(args);
                        }
                    }
                }
                else
                {
                    NoUpdateFiles();
                }
                Console.WriteLine("SinCraft successfully updated.  Starting SinCraft...");
                try
                {
                    if (!usingConsole)
                    {
                        Process.Start(args[0]);
                    }
                    else
                    {
                        Process.Start("mono", parentfullpathdir + "/" + args[0]);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to start SinCraft.  You will need to start it manually.");
                    MessageBox.Show("Updater has updated SinCraft, but was unable to start it.  You will need to start it manually.", "Updater", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception e)
            {
                if (tries > 4)
                {
                    UpdateFailure(e);
                }
                else
                {
                    Console.WriteLine("\n\nAn error occured while updating.  Retrying...\n\n");
                    Thread.Sleep(100);
                    Update(args);
                }
            }
        }
        static void UpdateFailure(Exception e)
        {
            Console.WriteLine("Updater is unable to update SinCraft.\n\n" + e.ToString() + "\n\nPress any key to exit.");
            MessageBox.Show("Updater is unable to update SinCraft.\n\n" + e.ToString(), "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
        static void NoUpdateFiles()
        {
            Console.WriteLine("Updater has no files to update.  Press any key to exit.");
            MessageBox.Show("Updater has no files to update.", "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
        static void globalException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("UnhandledException:\n\n" + e);
            MessageBox.Show("UnhandledException:\n\n" + e, "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
    }
}
