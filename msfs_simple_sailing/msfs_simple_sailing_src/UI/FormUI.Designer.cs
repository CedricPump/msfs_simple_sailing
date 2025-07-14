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
            textBoxLog.Location = new System.Drawing.Point(12, 559);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.Size = new System.Drawing.Size(512, 139);
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
            // FormUI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(536, 708);
            Controls.Add(panelRose);
            Controls.Add(textBoxLog);
            Controls.Add(buttonToggleSail);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormUI";
            Text = "MSFS Simple Sail";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonToggleSail;
        private System.Windows.Forms.TextBox textBoxLog;
        private DrawPanel panelRose;
    }

    class DrawPanel : Panel
    {
        public DrawPanel()
        {
            this.DoubleBuffered = true;
        }
    }
}