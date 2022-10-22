using System;


namespace BaltaDataAccess.Models{

    public class CarrerItem{

        public Guid Id { get; set; }

        public string? Title { get; set; }

        public Course? Course { get; set; }
    }
}