namespace BattleShipGame
{
    partial class Wait
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
            this.components = new System.ComponentModel.Container();
            this.WaitMessage = new System.Windows.Forms.Label();
            this.PlayerTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // WaitMessage
            // 
            this.WaitMessage.AutoSize = true;
            this.WaitMessage.Location = new System.Drawing.Point(12, 42);
            this.WaitMessage.Name = "WaitMessage";
            this.WaitMessage.Size = new System.Drawing.Size(175, 13);
            this.WaitMessage.TabIndex = 0;
            this.WaitMessage.Text = "Waiting for the 2nd Player to Join In";
            // 
            // PlayerTimer
            // 
            this.PlayerTimer.Interval = 2000;
            this.PlayerTimer.Tick += new System.EventHandler(this.PlayerTimer_Tick);
            // 
            // Wait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(209, 105);
            this.ControlBox = false;
            this.Controls.Add(this.WaitMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Wait";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wait";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label WaitMessage;
        public System.Windows.Forms.Timer PlayerTimer;

    }
}