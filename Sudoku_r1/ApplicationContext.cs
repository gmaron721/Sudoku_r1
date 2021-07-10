using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Sudoku_r1
{
    class ApplicationContext : DbContext
    {
        public DbSet<Sudoku_Page> Sudoku_Pages { get; set; }
        public DbSet<Sudoku_URL> Sudoku_Urls { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                options.UseSqlite(@"Data Source=" + AppDomain.CurrentDomain.BaseDirectory + @"Resources\base.db");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                options.UseSqlite(@"Data Source=" + AppDomain.CurrentDomain.BaseDirectory + @"\tmp\base.db");
            }
        }
    }
    public class Sudoku_Page
    {
        public int Id { get; set; }
        public string Page { get; set; }
    }
    public class Sudoku_URL
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Time { get; set; }
    }
}
