namespace OpTime
{
    partial class KomplektsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KomplektsForm));
            this.AddBINK_TB = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.AddBINK_Btn = new System.Windows.Forms.Button();
            this.BINK_LV = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // AddBINK_TB
            // 
            this.AddBINK_TB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddBINK_TB.Location = new System.Drawing.Point(198, 192);
            this.AddBINK_TB.Name = "AddBINK_TB";
            this.AddBINK_TB.Size = new System.Drawing.Size(244, 20);
            this.AddBINK_TB.TabIndex = 50;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 195);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(147, 13);
            this.label9.TabIndex = 49;
            this.label9.Text = "Добавить новый комплект:";
            // 
            // AddBINK_Btn
            // 
            this.AddBINK_Btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddBINK_Btn.Location = new System.Drawing.Point(448, 190);
            this.AddBINK_Btn.Name = "AddBINK_Btn";
            this.AddBINK_Btn.Size = new System.Drawing.Size(87, 23);
            this.AddBINK_Btn.TabIndex = 48;
            this.AddBINK_Btn.Text = "Добавить";
            this.AddBINK_Btn.UseVisualStyleBackColor = true;
            this.AddBINK_Btn.Click += new System.EventHandler(this.AddBINK_Btn_Click);
            // 
            // BINK_LV
            // 
            this.BINK_LV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BINK_LV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.BINK_LV.HideSelection = false;
            this.BINK_LV.Location = new System.Drawing.Point(15, 12);
            this.BINK_LV.MultiSelect = false;
            this.BINK_LV.Name = "BINK_LV";
            this.BINK_LV.Size = new System.Drawing.Size(520, 172);
            this.BINK_LV.TabIndex = 46;
            this.BINK_LV.UseCompatibleStateImageBehavior = false;
            this.BINK_LV.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Комплект";
            this.columnHeader1.Width = 145;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Суммарное время наработки (в часах)";
            this.columnHeader2.Width = 209;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Суммарное время наработки";
            this.columnHeader3.Width = 217;
            // 
            // KomplektsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 228);
            this.Controls.Add(this.AddBINK_TB);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.AddBINK_Btn);
            this.Controls.Add(this.BINK_LV);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KomplektsForm";
            this.Text = "Список комплектов приборов";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KomplektsForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox AddBINK_TB;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button AddBINK_Btn;
        private System.Windows.Forms.ListView BINK_LV;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}