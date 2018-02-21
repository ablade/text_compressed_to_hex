using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Convert_button_Click(object sender, EventArgs e)
        {
            const string gzFileName = "alvintemp.gz";                             //Temp file to hold the compressed file 
            const string txtFileName = "alvintemp.txt";                           //Temp file to hold the text in the Textbox
            richTextBox1.SaveFile(txtFileName, RichTextBoxStreamType.PlainText);  //Save the data in the textbox to the temp file
            FileInfo fileToCompress = new FileInfo(txtFileName);
            using (FileStream originalFileStream = fileToCompress.OpenRead()) //Read the temp file
            using (FileStream compressedFileStream = File.Create(gzFileName)) //Create the compressed file
            using (GZipStream compressionStream = new GZipStream(compressedFileStream,CompressionMode.Compress)) //Use gzip compression
            {
                originalFileStream.CopyTo(compressionStream); //Write to the created compressed temporary file
            }

            if (File.Exists(gzFileName))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(gzFileName, FileMode.Open))) //Read compressed file in binary mode
                {
                    byte[] bytebuff = new byte[64]; //Ensure 8 bits for Hex characters
                    richTextBox1.Clear(); //Delete the text that was pasted on by the user
                    string hex; //String to hold the formated HEX string
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        bytebuff = reader.ReadBytes(64); //Read 64 bytes of the .gz file
                        hex = BitConverter.ToString(bytebuff).Replace("-", string.Empty);  //Convert the binary to hex string
                        richTextBox1.AppendText(hex + System.Environment.NewLine); //Write and append to the textbox
                    }
                }
                //Clean up delete temporary files
                File.Delete(txtFileName); 
                File.Delete(gzFileName);
            }    
        }

        private void Upload_button_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                const string gzFileName = "alvintempUpload.gz"; //Temporary file name to hold the compressed file
                string fname = ""; //The soon to be created Hex filename
                using (Stream originalFileStream = openFileDialog1.OpenFile()) //Open a dialog box for the user to choose a file
                using (Stream compressedFileStream = File.Create(gzFileName))  //Creae the temp file
                using (GZipStream compressionStream = new GZipStream(compressedFileStream,CompressionMode.Compress)) //Compress with gzip
                {
                    originalFileStream.CopyTo(compressionStream); //Compress the file to the temp file.
                    fname = openFileDialog1.SafeFileName.Split('.')[0] + ".hex"; //Create the new filename with the provided file with a new extention .hex and remove old extension
                }

                if (File.Exists(gzFileName))
                {

                    using (BinaryReader reader = new BinaryReader(File.Open(gzFileName, FileMode.Open))) //Read the compressed file in binary
                    using (StreamWriter writetext = new StreamWriter(fname)) //Create the file
                    {
                        byte[] bytebuff = new byte[64]; //Ensure 8 bits for Hex characters
                        string hex; //String to hold the formated HEX string
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            bytebuff = reader.ReadBytes(64); //Read 64 bytes of the .gz file
                            hex = BitConverter.ToString(bytebuff).Replace("-", string.Empty);  //Convert the binary to hex string
                            hex = BitConverter.ToString(bytebuff).Replace("-", string.Empty);
                            writetext.WriteLine(hex);//Write and append to created file
                        }
                    }

                    File.Delete(gzFileName);  //Clean temp file
                    MessageBox.Show("The file " + fname + " was created in " + Application.StartupPath); //Send a message to the user about the file created and its location
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
