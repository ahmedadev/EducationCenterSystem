namespace EducationCenterSystem.Presentation.WinForms.Components;

public static class DataGridStyler
{
    public static void ApplyModernTheme(this DataGridView grid)
    {
        // General Settings
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.RowHeadersVisible = false;
        
        // Behavior
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // Header Styling
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(243, 244, 246); // Light Gray
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(55, 65, 81);    // Dark Gray
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
        grid.ColumnHeadersHeight = 45;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        // Rows Styling
        grid.DefaultCellStyle.BackColor = Color.White;
        grid.DefaultCellStyle.ForeColor = Color.FromArgb(31, 41, 55);                 // Text Gray
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);       // Light Indigo (Hover effect)
        grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(31, 41, 55);          // Text Gray
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        grid.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);

        grid.RowTemplate.Height = 45;
        grid.GridColor = Color.FromArgb(229, 231, 235);                               // Border Color
        
        // Alternating Rows (Optional)
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
    }
}
