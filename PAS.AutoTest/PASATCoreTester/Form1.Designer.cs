namespace PASATCoreTester
{
    partial class Form1
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
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tbService = new System.Windows.Forms.TextBox();
            this.tbMethod = new System.Windows.Forms.TextBox();
            this.tbParas = new System.Windows.Forms.TextBox();
            this.tbTemplate = new System.Windows.Forms.RichTextBox();
            this.tbM = new System.Windows.Forms.RichTextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.tbPatients = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.tbThread = new System.Windows.Forms.TextBox();
            this.button8 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbIndex = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(540, 132);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "load";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(450, 170);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Insert";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tbService
            // 
            this.tbService.Location = new System.Drawing.Point(118, 12);
            this.tbService.Name = "tbService";
            this.tbService.Size = new System.Drawing.Size(185, 20);
            this.tbService.TabIndex = 3;
            // 
            // tbMethod
            // 
            this.tbMethod.Location = new System.Drawing.Point(118, 57);
            this.tbMethod.Name = "tbMethod";
            this.tbMethod.Size = new System.Drawing.Size(185, 20);
            this.tbMethod.TabIndex = 4;
            // 
            // tbParas
            // 
            this.tbParas.Location = new System.Drawing.Point(118, 99);
            this.tbParas.Name = "tbParas";
            this.tbParas.Size = new System.Drawing.Size(185, 20);
            this.tbParas.TabIndex = 5;
            // 
            // tbTemplate
            // 
            this.tbTemplate.Location = new System.Drawing.Point(349, 12);
            this.tbTemplate.Name = "tbTemplate";
            this.tbTemplate.Size = new System.Drawing.Size(383, 107);
            this.tbTemplate.TabIndex = 7;
            this.tbTemplate.Text = "";
            // 
            // tbM
            // 
            this.tbM.Location = new System.Drawing.Point(118, 141);
            this.tbM.Name = "tbM";
            this.tbM.Size = new System.Drawing.Size(185, 52);
            this.tbM.TabIndex = 6;
            this.tbM.Text = "";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(247, 327);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(116, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "Create Patients";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tbPatients
            // 
            this.tbPatients.Location = new System.Drawing.Point(21, 327);
            this.tbPatients.Name = "tbPatients";
            this.tbPatients.Size = new System.Drawing.Size(170, 20);
            this.tbPatients.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(382, 332);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 10;
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(21, 360);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(170, 23);
            this.pb.TabIndex = 11;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(247, 360);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(116, 23);
            this.button5.TabIndex = 12;
            this.button5.Text = "Load Patients";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(247, 289);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(116, 23);
            this.button6.TabIndex = 13;
            this.button6.Text = "Create Patients";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(385, 360);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(174, 23);
            this.button7.TabIndex = 14;
            this.button7.Text = "Load Patients Concurrently";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // tbThread
            // 
            this.tbThread.Location = new System.Drawing.Point(385, 330);
            this.tbThread.Name = "tbThread";
            this.tbThread.Size = new System.Drawing.Size(170, 20);
            this.tbThread.TabIndex = 15;
            this.tbThread.Text = "10";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(384, 289);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 16;
            this.button8.Text = "button8";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Service Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Function Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Parameter Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Reqired:";
            // 
            // tbIndex
            // 
            this.tbIndex.Location = new System.Drawing.Point(349, 134);
            this.tbIndex.Name = "tbIndex";
            this.tbIndex.Size = new System.Drawing.Size(185, 20);
            this.tbIndex.TabIndex = 21;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(540, 170);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 22;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 322);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbIndex);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.tbThread);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.pb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPatients);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.tbTemplate);
            this.Controls.Add(this.tbService);
            this.Controls.Add(this.tbMethod);
            this.Controls.Add(this.tbParas);
            this.Controls.Add(this.tbM);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.TextBox tbService;
        private System.Windows.Forms.TextBox tbMethod;
        private System.Windows.Forms.TextBox tbParas;
        private System.Windows.Forms.RichTextBox tbM;
        private System.Windows.Forms.RichTextBox tbTemplate;

        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox tbPatients;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox tbThread;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbIndex;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

