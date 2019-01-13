namespace ICE_ICE_BABY
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
            this.idText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.outputText = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.startServer = new System.Windows.Forms.Button();
            this.connectToServer = new System.Windows.Forms.Button();
            this.idInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.chatInput = new System.Windows.Forms.TextBox();
            this.chatOutput = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // idText
            // 
            this.idText.Location = new System.Drawing.Point(499, 54);
            this.idText.Name = "idText";
            this.idText.ReadOnly = true;
            this.idText.Size = new System.Drawing.Size(200, 20);
            this.idText.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(564, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Connection ID";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 100);
            this.button1.TabIndex = 2;
            this.button1.Text = "Do your magic!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // outputText
            // 
            this.outputText.Location = new System.Drawing.Point(130, 12);
            this.outputText.Multiline = true;
            this.outputText.Name = "outputText";
            this.outputText.ReadOnly = true;
            this.outputText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputText.Size = new System.Drawing.Size(345, 100);
            this.outputText.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(705, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 20);
            this.button2.TabIndex = 4;
            this.button2.Text = "📝";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // startServer
            // 
            this.startServer.Location = new System.Drawing.Point(69, 61);
            this.startServer.Name = "startServer";
            this.startServer.Size = new System.Drawing.Size(112, 46);
            this.startServer.TabIndex = 5;
            this.startServer.Text = "Start";
            this.startServer.UseVisualStyleBackColor = true;
            this.startServer.Visible = false;
            this.startServer.Click += new System.EventHandler(this.startServer_Click);
            // 
            // connectToServer
            // 
            this.connectToServer.Location = new System.Drawing.Point(37, 48);
            this.connectToServer.Name = "connectToServer";
            this.connectToServer.Size = new System.Drawing.Size(113, 46);
            this.connectToServer.TabIndex = 6;
            this.connectToServer.Text = "Connect to Server";
            this.connectToServer.UseVisualStyleBackColor = true;
            this.connectToServer.Visible = false;
            // 
            // idInput
            // 
            this.idInput.Location = new System.Drawing.Point(12, 35);
            this.idInput.Name = "idInput";
            this.idInput.Size = new System.Drawing.Size(200, 20);
            this.idInput.TabIndex = 7;
            this.idInput.TextChanged += new System.EventHandler(this.idInput_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Target Connection ID";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.idInput);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.startServer);
            this.groupBox1.Location = new System.Drawing.Point(12, 118);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(254, 123);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(218, 35);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(25, 20);
            this.button4.TabIndex = 10;
            this.button4.Text = "📝";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // chatInput
            // 
            this.chatInput.Location = new System.Drawing.Point(275, 382);
            this.chatInput.Name = "chatInput";
            this.chatInput.Size = new System.Drawing.Size(200, 20);
            this.chatInput.TabIndex = 10;
            this.chatInput.Visible = false;
            this.chatInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chatInput_KeyPress);
            // 
            // chatOutput
            // 
            this.chatOutput.Location = new System.Drawing.Point(275, 118);
            this.chatOutput.Name = "chatOutput";
            this.chatOutput.ReadOnly = true;
            this.chatOutput.Size = new System.Drawing.Size(200, 258);
            this.chatOutput.TabIndex = 11;
            this.chatOutput.Text = "";
            this.chatOutput.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.connectToServer);
            this.groupBox2.Location = new System.Drawing.Point(24, 262);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            this.groupBox2.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.chatOutput);
            this.Controls.Add(this.chatInput);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.outputText);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.idText);
            this.Name = "Form1";
            this.Text = "ICE ICE BABY";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox idText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox outputText;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button startServer;
        private System.Windows.Forms.Button connectToServer;
        private System.Windows.Forms.TextBox idInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox chatInput;
        private System.Windows.Forms.RichTextBox chatOutput;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

