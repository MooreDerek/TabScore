﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Resources;
// Sets the UI culture to German for debugging
// using System.Globalization;
// using System.Threading;

namespace TabScore2Starter
{
    public partial class TabScoreForm : Form
    {
        private string connectionString = "";
        private readonly ResourceManager resourceManager;

        public TabScoreForm()
        {
            // Sets the UI culture to German for debugging
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");

            resourceManager = new ResourceManager("TabScore2Starter.Strings", typeof(TabScoreForm).Assembly);
            InitializeComponent();
        }

        private void TabScoreForm_Load(object sender, EventArgs e)
        {
            Text = $"TabScore2Starter - {resourceManager.GetString("Version")} {Assembly.GetExecutingAssembly().GetName().Version}";
            
            string argsString = "", pathToDB = "";
            string[] arguments = Environment.GetCommandLineArgs();

            // Parse command line args correctly to get DB path
            foreach (string s in arguments)
            {
                argsString = argsString + s + " ";
            }
            arguments = argsString.Split(new Char[] { '/' });
            foreach (string s in arguments)
            {
                if (s.StartsWith("f:["))
                {
                    pathToDB = s.Split(new char[] { '[', ']' })[1];
                    break;
                }
            }

            if (pathToDB == "")
            {
                // No database in arguments
                AddDatabaseFileButton.Visible = true;
            }
            else if (!File.Exists(pathToDB))
            {
                MessageBox.Show(resourceManager.GetString("DatabaseNotExist"), "TabScore2Starter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddDatabaseFileButton.Visible = true;
            }
            else
            {
                Database.SetAccessControl(pathToDB);
                connectionString = Database.ConnectionString(pathToDB);
                if (Database.Initialize(connectionString))
                {
                    PathToDBLabel.Text = pathToDB;
                    File.WriteAllText(Environment.ExpandEnvironmentVariables(@"%Public%\TabScore\TabScoreDB.txt"), connectionString);
                    SessionStatusLabel.Text = resourceManager.GetString("SessionRunning");
                    SessionStatusLabel.ForeColor = Color.Green;
                    OptionsButton.Visible = true;
                    ResultsViewerButton.Visible = true;
                    AddHandRecordFileButton.Visible = true;
                    HandsList handsList = new HandsList(connectionString);
                    if (handsList.Count > 0)
                    {
                        AddHandRecordFileButton.Enabled = false;
                        PathToHandRecordFileLabel.Text = resourceManager.GetString("IncludedInDatabase");
                        AnalysingLabel.Text = resourceManager.GetString("Analysing");
                        AnalysingLabel.Visible = true;
                        AnalysingProgressBar.Visible = true;
                        AnalysisCalculationBackgroundWorker.RunWorkerAsync();
                    }
                }
                else
                {
                    // Database is not valid for some reason and Initialize failed
                    AddDatabaseFileButton.Visible = true;
                }
            }
        }

        private void AddDatabaseFileButton_Click(object sender, EventArgs e)
        {
            if (DatabaseFileDialog.ShowDialog() == DialogResult.OK)
            {
                string pathToDB = DatabaseFileDialog.FileName;
                Database.SetAccessControl(pathToDB);
                connectionString = Database.ConnectionString(pathToDB);
                if (Database.Initialize(connectionString))
                {
                    AddDatabaseFileButton.Enabled = false;
                    PathToDBLabel.Text = pathToDB;
                    File.WriteAllText(Environment.ExpandEnvironmentVariables(@"%Public%\TabScore\TabScoreDB.txt"), connectionString);
                    SessionStatusLabel.Text = resourceManager.GetString("SessionRunning"); ;
                    SessionStatusLabel.ForeColor = Color.Green;
                    OptionsButton.Visible = true;
                    ResultsViewerButton.Visible = true;
                    AddHandRecordFileButton.Visible = true;
                    HandsList handsList = new HandsList(connectionString);
                    if (handsList.Count > 0)
                    {
                        AddHandRecordFileButton.Enabled = false;
                        PathToHandRecordFileLabel.Text = resourceManager.GetString("IncludedInDatabase");
                        AnalysingLabel.Text = resourceManager.GetString("Analysing");
                        AnalysingLabel.Visible = true;
                        AnalysingProgressBar.Visible = true;
                        AnalysisCalculationBackgroundWorker.RunWorkerAsync();
                    }
                }
            }
        }

        private void AddHandRecordFileButton_Click(object sender, EventArgs e)
        {
            if (HandRecordFileDialog.ShowDialog() == DialogResult.OK)
            {
                PathToHandRecordFileLabel.Text = HandRecordFileDialog.FileName;
                StreamReader file = new StreamReader(HandRecordFileDialog.FileName);
                HandsList handsList = new HandsList(file);
                if (handsList.Count == 0)
                {
                    MessageBox.Show(resourceManager.GetString("FileNoHandRecords"), "TabScore2Starter", MessageBoxButtons.OK);
                }
                else
                {
                    handsList.WriteToDB(connectionString);
                    AddHandRecordFileButton.Enabled = false;
                    AnalysingLabel.Text = resourceManager.GetString("Analysing");
                    AnalysingLabel.Visible = true;
                    AnalysingProgressBar.Visible = true;
                    AnalysisCalculationBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void TabScoreForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            File.WriteAllText(Environment.ExpandEnvironmentVariables(@"%Public%\TabScore\TabScoreDB.txt"), "");
        }

        private void AnalysisCalculation_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            HandsList handsList = new HandsList(connectionString);
            HandEvaluationsList handEvaluationsList = new HandEvaluationsList();
            int counter = 0;
            foreach (Hand hand in handsList)
            {
                HandEvaluation handEvaluation = new HandEvaluation(hand);
                handEvaluationsList.Add(handEvaluation);
                counter++;
                worker.ReportProgress((int)((float)counter / (float)handsList.Count * 100.0));
            }
            handEvaluationsList.WriteToDB(connectionString);
        }

        private void AnalysisCalculation_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AnalysingProgressBar.Value = e.ProgressPercentage;
        }

        private void AnalysisCalculation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AnalysingProgressBar.Value = 100;
            AnalysingLabel.Text = resourceManager.GetString("AnalysisComplete");
            AddHandRecordFileButton.Text = resourceManager.GetString("ChangeHandRecordFile");
            AddHandRecordFileButton.Enabled = true;
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            OptionsForm frmOptions = new OptionsForm(connectionString, new Point(Location.X + 30, Location.Y + 30));
            frmOptions.ShowDialog();
        }

        private void ResultsViewerButton_Click(object sender, EventArgs e)
        {
            ViewResultsForm frmResultsViewer = new ViewResultsForm(connectionString, new Point(Location.X + 30, Location.Y + 30));
            frmResultsViewer.ShowDialog();
        }
    }
}
