﻿using System;
using System.Windows.Forms;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;
using SonicRetro.SAModel.SAEditorCommon;

namespace SAToolsHub
{
	public partial class GamePaths : Form
	{
		public GamePaths()
		{
			InitializeComponent();

			SetValues();
		}

		void SetValues()
		{
			string sadxPCPath = Program.Settings.SADXPCPath;
			string sa2PCPath = Program.Settings.SA2PCPath;

			SADXPath.Text = sadxPCPath;
			SA2Path.Text = sa2PCPath;
		}

		void SaveSettings()
		{
			Program.Settings.SADXPCPath = SADXPath.Text;
			Program.Settings.SA2PCPath = SA2Path.Text;

			Program.Settings.Save();
		}

		private void btnSavePaths_Click(object sender, EventArgs e)
		{
			SaveSettings();

			// check validity
			string sadxFailReason = "";
			bool sadxpcValid = GamePathChecker.CheckSADXPCValid(
				Program.Settings.SADXPCPath, out sadxFailReason);

			if (!sadxpcValid)
			{
				MessageBox.Show(sadxFailReason, "SADX PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			string sa2PCFailReason = "";
			bool sa2PCValid = GamePathChecker.CheckSA2PCValid(
				Program.Settings.SA2PCPath, out sa2PCFailReason);

			if (!sa2PCValid)
			{
				MessageBox.Show(sa2PCFailReason, "SA2 PC Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			if (!Program.AnyGamesConfigured())
			{
				DialogResult dialogResult = MessageBox.Show("No games configured", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

				if (dialogResult == DialogResult.Cancel)
				{
					this.DialogResult = DialogResult.Abort;
					Application.Exit();
					return;
				}
				else
				{
					return;
				}
			}

			// hide and save
			this.Hide();
		}

		private void btnSADXPath_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				SADXPath.Text = folderDialog.SelectedPath;
			}
		}

		private void btnSA2Path_Click(object sender, EventArgs e)
		{
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				SA2Path.Text = folderDialog.SelectedPath;
			}
		}
	}
}
