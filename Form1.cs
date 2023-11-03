using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace ImageRemoval
{
    public partial class Form1 : Form
    {
        string selectedPath = ""; // Variable to store the selected path
        List<string> filesToDelete = new List<string>();

        public Form1()
        {
            InitializeComponent();
            PathTextBox.TextChanged += PathTextBox_TextChanged;
        }

        private void PathTextBox_TextChanged(object sender, EventArgs e)
        {
            selectedPath = PathTextBox.Text; // Update selectedPath when text changes in the textbox
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    selectedPath = folderBrowserDialog.SelectedPath;
                    PathTextBox.Text = selectedPath;
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedPath) && Directory.Exists(selectedPath))
            {
                int width = (int)WidthNumericUpDown.Value;
                int height = (int)HeightNumericUpDown.Value;

                string[] imageFiles = Directory.GetFiles(selectedPath, "*.png");

                foreach (string file in imageFiles)
                {
                    using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (Image image = Image.FromStream(stream, false, false))
                        {
                            if (image.Width != width || image.Height != height)
                            {
                                filesToDelete.Add(file);
                            }
                        }
                    }
                }

                if (filesToDelete.Count > 0)
                {
                    DialogResult result = MessageBox.Show($"{filesToDelete.Count} files will be deleted. Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        foreach (string file in filesToDelete)
                        {
                            File.Delete(file);
                        }
                        MessageBox.Show("Files deleted.");
                        filesToDelete.Clear(); // Clear the list after deletion
                        RefreshImageCount(); // Update the image count label
                    }
                    else
                    {
                        filesToDelete.Clear();
                        MessageBox.Show("Deletion canceled.");
                    }
                }
                else
                {
                    MessageBox.Show("No files to delete with the specified resolution.");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid directory path.");
            }
        }

        private void RefreshImageCount()
        {
            if (!string.IsNullOrEmpty(selectedPath) && Directory.Exists(selectedPath))
            {
                int width = (int)WidthNumericUpDown.Value;
                int height = (int)HeightNumericUpDown.Value;

                string[] imageFiles = Directory.GetFiles(selectedPath, "*.png");

                int filesCount = 0;
                foreach (string file in imageFiles)
                {
                    using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (Image image = Image.FromStream(stream, false, false))
                        {
                            if (image.Width != width || image.Height != height)
                            {
                                filesCount++;
                            }
                        }
                    }
                }

                ImageCountLabel.Text = $"{imageFiles.Length - filesCount} images found with specified resolution";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}