namespace FourierHabr
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            box = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)box).BeginInit();
            SuspendLayout();
            // 
            // box
            // 
            box.BackColor = Color.White;
            box.BorderStyle = BorderStyle.FixedSingle;
            box.Location = new Point(12, 12);
            box.Name = "box";
            box.Size = new Size(960, 544);
            box.TabIndex = 0;
            box.TabStop = false;
            box.Paint += box_Paint;
            // 
            // timer1
            // 
            timer1.Interval = 33;
            // 
            // button1
            // 
            button1.Location = new Point(993, 12);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1103, 664);
            Controls.Add(button1);
            Controls.Add(box);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)box).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox box;
        private System.Windows.Forms.Timer timer1;
        private Button button1;
    }
}
