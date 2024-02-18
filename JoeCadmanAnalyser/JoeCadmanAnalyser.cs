using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JoeCadmanAnalyser
{
    public partial class JoeCadmanAnalyser : Form
    {
        public JoeCadmanAnalyser()
        {
            InitializeComponent();
        }

        private void JoeCadmanAnalyser_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void JoeCadmanAnalyser_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                PrepareHeaderForMatch = header => Regex.Replace(header.Header, @"\s", string.Empty),
                ShouldSkipRecord = (row) =>
                {
                    //your logic for skipping records goes here

                    const int numberOfColumns = 16;

                    return row.Row.ColumnCount < numberOfColumns;

                }
            };


            foreach (string file in files)
            {
                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<JoeCadmanRecord>().ToList();

                    var groupings = records
                        .GroupBy(x => x.attempt);

                    var maxTimes = groupings
                        .Select(x => (attemptNo: x.Key, maxTime: x.Select(x => x.time).Max()))
                        .ToArray();

                    resultsTextBox.Text = string.Join("\r\n", maxTimes.Select(x => $"attempt no {x.attemptNo} has max time {x.maxTime}"));
                }
            }


        }
    }
}
