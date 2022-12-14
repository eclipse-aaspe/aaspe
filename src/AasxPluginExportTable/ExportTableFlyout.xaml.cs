/*
Copyright (c) 2018-2019 Festo AG & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AasxIntegrationBase;
using AdminShellNS;
using Newtonsoft.Json;

namespace AasxPluginExportTable
{
    public partial class ExportTableFlyout : UserControl, IFlyoutControl
    {
        public event IFlyoutControlClosed ControlClosed;

        public List<ExportTableRecord> Presets = null;

        public ExportTableRecord Result = null;

        private int rows = 1, cols = 1;

        private List<TextBox> textBoxesHeader = new List<TextBox>();
        private List<TextBox> textBoxesElements = new List<TextBox>();

        //
        // Init
        //

        public ExportTableFlyout()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // init with defaults
            this.ThisInit(1, 4);

            // if preset, load first preset
            if (this.Presets != null && this.Presets.Count > 0)
                this.ThisFromPreset(this.Presets[0]);

            // placeholders
            TextBoxPlaceholders.Text =
                AdminShellUtil.CleanHereStringWithNewlines(
                @"All placeholders delimited by %{..}%, {} = set arithmetics, [] = optional
                {Referable}.{idShort, category, description[@en..], elementName, elementAbbreviation, kind, parent}, {Referable|Identifiable} = {SM, SME, CD}, depth, indent
                {Identifiable}.{identification[.{idType, id}], administration.{ version, revision}}, {Qualifiable}.qualifiers, {Qualifiable}.multiplicity
                {Reference}, {Reference}[0..n], {Reference}[0..n].{type, local, idType, value}, {Reference} = {semanticId, isCaseOf, unitId}
                SME.value, Property.{value, valueType, valueId}, MultiLanguageProperty.{value, vlaueId}, Range.{valueType, min, max}, Blob.{mimeType, value}, File.{mimeType, value}, ReferenceElement.value, RelationshipElement.{first, second}, SubmodelElementCollection.{value = #elements, ordered, allowDuplicates}, Entity.{entityType, asset}
                CD.{preferredName[@en..], shortName[@en..], unit, unitId, sourceOfDefinition, symbol, dataType, definition[@en..], valueFormat}

                Commands for header cells include: %fg={color}%, %bg={color}% with {color} = {#a030a0, Red, blue, ..}, %halign={left, center, right}%, %valign={top, center, bottom}%,
                %font={bold, italic, underline}, %frame={0,1,2,3}% (only whole table)");

            // combo Presets
            ComboBoxPreset.Items.Clear();
            if (this.Presets != null && this.Presets.Count > 0)
            {
                foreach (var p in this.Presets)
                    ComboBoxPreset.Items.Add("" + p.Name);
                ComboBoxPreset.SelectionChanged += ComboBoxPreset_SelectionChanged;
                ComboBoxPreset.SelectedIndex = 0;
            }

            // combo Formats
            ComboBoxFormat.Items.Clear();
            foreach (var f in ExportTableRecord.FormatNames)
                ComboBoxFormat.Items.Add("" + f);
            ComboBoxFormat.SelectedIndex = 0;
        }

        void DataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null)
                return;
            foreach (var oColumn in dg.Columns)
            {
                // This is how to set the width to *
                oColumn.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            }
        }

        //
        // Outer
        //

        public void ControlStart()
        {
        }

        public void ControlPreviewKeyDown(KeyEventArgs e)
        {
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Result = null;
            ControlClosed?.Invoke();
        }

        //
        // Mechanics
        //

        private void ThisInit(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;

            TextBoxNumRows.Text = "" + this.rows;
            TextBoxNumCols.Text = "" + this.cols;

            AdaptRowsCols(GridOuterHeader, textBoxesHeader, this.rows, this.cols);
            AdaptRowsCols(GridOuterElements, textBoxesElements, this.rows, this.cols);
        }

        private ExportTableRecord ThisToPreset()
        {
            var x = new ExportTableRecord(this.rows, this.cols, "",
                    this.textBoxesHeader.Select(tb => tb.Text),
                    this.textBoxesElements.Select(tb => tb.Text)
                    );

            x.Format = ComboBoxFormat.SelectedIndex;
            x.ReplaceFailedMatches = CheckBoxReplaceFailed.IsChecked == true;
            x.FailText = TextBoxFailText.Text;

            return x;
        }

        private void ThisFromPreset(ExportTableRecord preset)
        {
            // access
            if (preset == null || preset.Rows < 1 || preset.Cols < 1)
                return;

            // take over
            this.rows = preset.Rows;
            this.cols = preset.Cols;

            ComboBoxFormat.SelectedIndex = Math.Max(0, Math.Min(ComboBoxFormat.Items.Count - 1, preset.Format));

            TextBoxNumRows.Text = "" + this.rows;
            TextBoxNumCols.Text = "" + this.cols;

            CheckBoxReplaceFailed.IsChecked = preset.ReplaceFailedMatches;
            TextBoxFailText.Text = "" + preset.FailText;

            AdaptRowsCols(GridOuterHeader, textBoxesHeader, this.rows, this.cols);
            AdaptRowsCols(GridOuterElements, textBoxesElements, this.rows, this.cols);

            if (preset.Header != null)
                for (int i = 0; i < preset.Header.Count; i++)
                    if (i < this.textBoxesHeader.Count)
                        this.textBoxesHeader[i].Text = preset.Header[i];

            if (preset.Elements != null)
                for (int i = 0; i < preset.Elements.Count; i++)
                    if (i < this.textBoxesElements.Count)
                        this.textBoxesElements[i].Text = preset.Elements[i];
        }

        private void ComboBoxPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var i = ComboBoxPreset.SelectedIndex;
            if (this.Presets != null && i >= 0 && i < this.Presets.Count)
            {
                this.ThisFromPreset(this.Presets[i]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == ButtonExport)
            {
                this.Result = this.ThisToPreset();
                ControlClosed?.Invoke();
            }

            if (sender == ButtonResize)
            {
                if (!int.TryParse("" + TextBoxNumRows.Text, out int irows)
                    || !int.TryParse("" + TextBoxNumCols.Text, out int icols))
                    return;

                this.rows = Math.Max(1, irows);
                this.cols = Math.Max(1, icols);

                AdaptRowsCols(GridOuterHeader, textBoxesHeader, this.rows, this.cols);
                AdaptRowsCols(GridOuterElements, textBoxesElements, this.rows, this.cols);
            }

            if (sender == ButtonSavePreset)
            {
                // choose filename
                var dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "new.json";
                dlg.DefaultExt = "*.json";
                dlg.Filter = "Preset JSON file (*.json)|*.json|All files (*.*)|*.*";

                // save
                if (true == dlg.ShowDialog())
                {
                    try
                    {
                        var pr = this.ThisToPreset();
                        pr.SaveToFile(dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        AdminShellNS.LogInternally.That.SilentlyIgnoredError(ex);
                    }
                }
            }

            if (sender == ButtonLoadPreset)
            {
                // choose filename
                var dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "new.json";
                dlg.DefaultExt = "*.json";
                dlg.Filter = "Preset JSON file (*.json)|*.json|All files (*.*)|*.*";

                // save
                if (true == dlg.ShowDialog())
                {
                    try
                    {
                        var pr = ExportTableRecord.LoadFromFile(dlg.FileName);
                        this.ThisFromPreset(pr);
                    }
                    catch (Exception ex)
                    {
                        AdminShellNS.LogInternally.That.SilentlyIgnoredError(ex);
                    }
                }
            }
        }

        private void AdaptRowsCols(Grid grid, List<TextBox> tbs, int rows, int cols)
        {
            if (grid == null || tbs == null)
                return;

            // have header rows & columns
            rows += 1;
            cols += 1;

            // rework grid's cols
            while (grid.ColumnDefinitions.Count < cols)
            {
                var gl = new ColumnDefinition();
                gl.Width = new GridLength(1.0, GridUnitType.Star);
                grid.ColumnDefinitions.Add(gl);
            }
            while (grid.ColumnDefinitions.Count > cols)
            {
                grid.ColumnDefinitions.RemoveAt(grid.ColumnDefinitions.Count - 1);
            }

            // rework grid's rows
            while (grid.RowDefinitions.Count < rows)
            {
                var gl = new RowDefinition();
                gl.Height = new GridLength(1.0, GridUnitType.Star);
                grid.RowDefinitions.Add(gl);
            }
            while (grid.RowDefinitions.Count > rows)
            {
                grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
            }

            // more than required
            while (tbs.Count > rows * cols)
            {
                // delete last from grid
                var tb = tbs[tbs.Count - 1];
                grid.Children.Remove(tb);
                tbs.Remove(tb);
            }

            // re-align the existing ones
            for (int i = 0; i < tbs.Count; i++)
            {
                var tb = tbs[i];
                Grid.SetRow(tb, i / cols);
                Grid.SetColumn(tb, i % cols);
            }

            // new text boxes
            while (tbs.Count < rows * cols)
            {
                var tb = new TextBox();
                tb.Margin = new Thickness(0, 0, 4, 4);
                tb.TextWrapping = TextWrapping.Wrap;
                tb.AcceptsReturn = true;
                grid.Children.Add(tb);
                Grid.SetRow(tb, (tbs.Count) / cols);
                Grid.SetColumn(tb, (tbs.Count) % cols);
                tbs.Add(tb);
            }

            // in any case, re-color text boxes
            foreach (var child in grid.Children)
            {
                var tb = child as TextBox;
                if (tb != null)
                    if (Grid.GetRow(tb) < 1 || Grid.GetColumn(tb) < 1)
                    {
                        tb.Foreground = Brushes.DarkGray;
                        tb.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x15, 0x1f, 0x33));
                    }
                    else
                    {
                        tb.Foreground = TextBoxNumRows.Foreground;
                        tb.Background = TextBoxNumRows.Background;
                    }
            }
        }

        //
        // Business logic
        //
    }
}
