﻿namespace KitapTakipMaui.Models
{
    public class BookDetailsDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PageCount { get; set; }
    }
}