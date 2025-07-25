using System.Windows.Forms;

namespace msfs_simple_sail_core.UI
{
    partial class FormUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUI));
            timer1 = new Timer(components);
            buttonToggleSail = new Button();
            textBoxLog = new TextBox();
            panelRose = new DrawPanel();
            checkBoxAlwaysOnTop = new CheckBox();
            numericUpDownTrans = new NumericUpDown();
            labelTrans = new Label();
            numericUpDownPortJib = new NumericUpDown();
            labelportsheet = new Label();
            numericUpDownMainSheet = new NumericUpDown();
            numericUpDownStarJib = new NumericUpDown();
            labelmain = new Label();
            labelstarjib = new Label();
            buttonHdgHold = new Button();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTrans).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownPortJib).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMainSheet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownStarJib).BeginInit();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // buttonToggleSail
            // 
            buttonToggleSail.FlatStyle = FlatStyle.Flat;
            buttonToggleSail.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            buttonToggleSail.Location = new System.Drawing.Point(449, 12);
            buttonToggleSail.Name = "buttonToggleSail";
            buttonToggleSail.Size = new System.Drawing.Size(75, 23);
            buttonToggleSail.TabIndex = 0;
            buttonToggleSail.Text = "Toggle Sail";
            buttonToggleSail.UseVisualStyleBackColor = true;
            buttonToggleSail.Click += buttonToggleSail_Click;
            // 
            // textBoxLog
            // 
            textBoxLog.BorderStyle = BorderStyle.FixedSingle;
            textBoxLog.Location = new System.Drawing.Point(12, 603);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.Size = new System.Drawing.Size(512, 197);
            textBoxLog.TabIndex = 1;
            textBoxLog.Text = "this is \r\nmultiline\r\n";
            textBoxLog.TextChanged += textBoxLog_TextChanged;
            // 
            // panelRose
            // 
            panelRose.BackgroundImage = (System.Drawing.Image)resources.GetObject("panelRose.BackgroundImage");
            panelRose.Location = new System.Drawing.Point(12, 41);
            panelRose.Name = "panelRose";
            panelRose.Size = new System.Drawing.Size(512, 512);
            panelRose.TabIndex = 2;
            panelRose.Paint += panelRose_Paint;
            // 
            // checkBoxAlwaysOnTop
            // 
            checkBoxAlwaysOnTop.AutoSize = true;
            checkBoxAlwaysOnTop.Location = new System.Drawing.Point(12, 12);
            checkBoxAlwaysOnTop.Name = "checkBoxAlwaysOnTop";
            checkBoxAlwaysOnTop.Size = new System.Drawing.Size(99, 19);
            checkBoxAlwaysOnTop.TabIndex = 3;
            checkBoxAlwaysOnTop.Text = "always on top";
            checkBoxAlwaysOnTop.UseVisualStyleBackColor = true;
            checkBoxAlwaysOnTop.CheckedChanged += checkBoxAlwaysOnTop_CheckedChanged;
            // 
            // numericUpDownTrans
            // 
            numericUpDownTrans.BorderStyle = BorderStyle.None;
            numericUpDownTrans.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            numericUpDownTrans.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownTrans.Location = new System.Drawing.Point(117, 16);
            numericUpDownTrans.Maximum = new decimal(new int[] { 90, 0, 0, 0 });
            numericUpDownTrans.Name = "numericUpDownTrans";
            numericUpDownTrans.Size = new System.Drawing.Size(49, 15);
            numericUpDownTrans.TabIndex = 4;
            numericUpDownTrans.ValueChanged += numericUpDownTrans_ValueChanged;
            // 
            // labelTrans
            // 
            labelTrans.AutoSize = true;
            labelTrans.Location = new System.Drawing.Point(172, 13);
            labelTrans.Name = "labelTrans";
            labelTrans.Size = new System.Drawing.Size(75, 15);
            labelTrans.TabIndex = 5;
            labelTrans.Text = "transparency";
            labelTrans.Click += labelTrans_Click;
            // 
            // numericUpDownPortJib
            // 
            numericUpDownPortJib.BorderStyle = BorderStyle.None;
            numericUpDownPortJib.Location = new System.Drawing.Point(12, 574);
            numericUpDownPortJib.Name = "numericUpDownPortJib";
            numericUpDownPortJib.ReadOnly = true;
            numericUpDownPortJib.Size = new System.Drawing.Size(120, 19);
            numericUpDownPortJib.TabIndex = 6;
            numericUpDownPortJib.TextAlign = HorizontalAlignment.Right;
            numericUpDownPortJib.ValueChanged += numericUpDownPortJib_ValueChanged;
            // 
            // labelportsheet
            // 
            labelportsheet.AutoSize = true;
            labelportsheet.Location = new System.Drawing.Point(12, 556);
            labelportsheet.Name = "labelportsheet";
            labelportsheet.Size = new System.Drawing.Size(89, 15);
            labelportsheet.TabIndex = 7;
            labelportsheet.Text = "port jib sheet %";
            // 
            // numericUpDownMainSheet
            // 
            numericUpDownMainSheet.BorderStyle = BorderStyle.None;
            numericUpDownMainSheet.Location = new System.Drawing.Point(204, 574);
            numericUpDownMainSheet.Name = "numericUpDownMainSheet";
            numericUpDownMainSheet.ReadOnly = true;
            numericUpDownMainSheet.Size = new System.Drawing.Size(120, 19);
            numericUpDownMainSheet.TabIndex = 8;
            numericUpDownMainSheet.TextAlign = HorizontalAlignment.Right;
            numericUpDownMainSheet.ValueChanged += numericUpDownMainSheet_ValueChanged;
            // 
            // numericUpDownStarJib
            // 
            numericUpDownStarJib.BorderStyle = BorderStyle.None;
            numericUpDownStarJib.ForeColor = System.Drawing.SystemColors.ControlText;
            numericUpDownStarJib.Location = new System.Drawing.Point(404, 574);
            numericUpDownStarJib.Name = "numericUpDownStarJib";
            numericUpDownStarJib.ReadOnly = true;
            numericUpDownStarJib.Size = new System.Drawing.Size(120, 19);
            numericUpDownStarJib.TabIndex = 9;
            numericUpDownStarJib.TextAlign = HorizontalAlignment.Right;
            numericUpDownStarJib.ValueChanged += numericUpDownStarJib_ValueChanged;
            // 
            // labelmain
            // 
            labelmain.AutoSize = true;
            labelmain.Location = new System.Drawing.Point(204, 556);
            labelmain.Name = "labelmain";
            labelmain.Size = new System.Drawing.Size(78, 15);
            labelmain.TabIndex = 10;
            labelmain.Text = "main sheet %";
            // 
            // labelstarjib
            // 
            labelstarjib.AutoSize = true;
            labelstarjib.Location = new System.Drawing.Point(404, 556);
            labelstarjib.Name = "labelstarjib";
            labelstarjib.Size = new System.Drawing.Size(117, 15);
            labelstarjib.TabIndex = 11;
            labelstarjib.Text = "starboard jib sheet %";
            // 
            // buttonHdgHold
            // 
            buttonHdgHold.FlatStyle = FlatStyle.Flat;
            buttonHdgHold.Font = new System.Drawing.Font("Segoe UI", 7F);
            buttonHdgHold.Location = new System.Drawing.Point(362, 12);
            buttonHdgHold.Name = "buttonHdgHold";
            buttonHdgHold.Size = new System.Drawing.Size(81, 23);
            buttonHdgHold.TabIndex = 12;
            buttonHdgHold.Text = "Heading Hold";
            buttonHdgHold.UseVisualStyleBackColor = true;
            buttonHdgHold.Click += buttonHdgHold_Click;
            // 
            // FormUI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(536, 812);
            Controls.Add(buttonHdgHold);
            Controls.Add(labelstarjib);
            Controls.Add(labelmain);
            Controls.Add(numericUpDownStarJib);
            Controls.Add(numericUpDownMainSheet);
            Controls.Add(labelportsheet);
            Controls.Add(numericUpDownPortJib);
            Controls.Add(labelTrans);
            Controls.Add(numericUpDownTrans);
            Controls.Add(checkBoxAlwaysOnTop);
            Controls.Add(panelRose);
            Controls.Add(textBoxLog);
            Controls.Add(buttonToggleSail);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormUI";
            Text = "MSFS Simple Sail";
            ((System.ComponentModel.ISupportInitialize)numericUpDownTrans).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownPortJib).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMainSheet).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownStarJib).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonToggleSail;
        private System.Windows.Forms.TextBox textBoxLog;
        private DrawPanel panelRose;
        private CheckBox checkBoxAlwaysOnTop;
        private NumericUpDown numericUpDownTrans;
        private Label labelTrans;
        private NumericUpDown numericUpDownPortJib;
        private Label labelportsheet;
        private NumericUpDown numericUpDownMainSheet;
        private NumericUpDown numericUpDownStarJib;
        private Label labelmain;
        private Label labelstarjib;
        private Button buttonHdgHold;
    }

    class DrawPanel : Panel
    {
        public DrawPanel()
        {
            this.DoubleBuffered = true;
        }
    }
}