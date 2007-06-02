namespace EPocalipse.Json.JsonView
{
    partial class MainForm
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
            this.JsonViewer = new EPocalipse.Json.Viewer.JsonViewer();
            this.SuspendLayout();
            // 
            // JsonViewer
            // 
            this.JsonViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JsonViewer.Json = "";
            this.JsonViewer.Location = new System.Drawing.Point(0, 0);
            this.JsonViewer.Name = "JsonViewer";
            this.JsonViewer.Size = new System.Drawing.Size(933, 680);
            this.JsonViewer.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 680);
            this.Controls.Add(this.JsonViewer);
            this.Name = "MainForm";
            this.Text = "JSON Viewer";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private EPocalipse.Json.Viewer.JsonViewer JsonViewer;
    }
}

