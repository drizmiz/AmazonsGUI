namespace AmazonChessGUI
{
    partial class SinglePlayerForm
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
            this.saveButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.regretButton = new System.Windows.Forms.Button();
            this.hintButton = new System.Windows.Forms.Button();
            this.helpButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.chessTable1 = new AmazonChessGUI.ChessTable();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(422, 84);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "保存";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(422, 42);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 2;
            this.loadButton.Text = "读取";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(422, 124);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(75, 23);
            this.newButton.TabIndex = 3;
            this.newButton.Text = "新游戏";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.NewButton_Click);
            // 
            // regretButton
            // 
            this.regretButton.Location = new System.Drawing.Point(422, 317);
            this.regretButton.Name = "regretButton";
            this.regretButton.Size = new System.Drawing.Size(75, 23);
            this.regretButton.TabIndex = 4;
            this.regretButton.Text = "悔棋";
            this.regretButton.UseVisualStyleBackColor = true;
            this.regretButton.Click += new System.EventHandler(this.RegretButton_Click);
            // 
            // hintButton
            // 
            this.hintButton.Location = new System.Drawing.Point(422, 355);
            this.hintButton.Name = "hintButton";
            this.hintButton.Size = new System.Drawing.Size(75, 23);
            this.hintButton.TabIndex = 5;
            this.hintButton.Text = "提示";
            this.hintButton.UseVisualStyleBackColor = true;
            this.hintButton.Click += new System.EventHandler(this.HintButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(53, 404);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 23);
            this.helpButton.TabIndex = 6;
            this.helpButton.Text = "帮助";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(324, 404);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 7;
            this.exitButton.Text = "退出";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // chessTable1
            // 
            this.chessTable1.Horizontal = 9;
            this.chessTable1.Location = new System.Drawing.Point(34, 23);
            this.chessTable1.Name = "chessTable1";
            this.chessTable1.Size = new System.Drawing.Size(382, 375);
            this.chessTable1.TabIndex = 0;
            this.chessTable1.Vertical = 9;
            // 
            // SinglePlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(530, 450);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.hintButton);
            this.Controls.Add(this.regretButton);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.chessTable1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SinglePlayerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "单人模式";
            this.ResumeLayout(false);

        }

        #endregion

        private ChessTable chessTable1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button regretButton;
        private System.Windows.Forms.Button hintButton;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Button exitButton;
    }
}