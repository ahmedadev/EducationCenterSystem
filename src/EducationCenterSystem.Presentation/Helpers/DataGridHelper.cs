using EducationCenterSystem.Presentation.WinForms.Theme;

namespace EducationCenterSystem.Presentation.WinForms.Helpers;

public static class DataGridHelper
{
    public static void InitializeGrid(DataGridView grid)
    {
        // Standardize appearance
        grid.BackgroundColor = AppTheme.BackgroundDark;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.DefaultCellStyle.BackColor = AppTheme.SurfaceCard;
        grid.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
        grid.DefaultCellStyle.SelectionBackColor = AppTheme.AccentPrimary;
        grid.DefaultCellStyle.SelectionForeColor = Color.White;
        grid.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
        
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.BackgroundDark;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextSecondary;
        grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontSubtitle;
        grid.ColumnHeadersHeight = 50;
        grid.EnableHeadersVisualStyles = false;

        grid.RowHeadersVisible = false;
        grid.RowTemplate.Height = 45;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.ReadOnly = true;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        
        // Disable auto sort mode so we can handle it manually
        foreach (DataGridViewColumn col in grid.Columns)
        {
            col.SortMode = DataGridViewColumnSortMode.Programmatic;
        }
    }
}
