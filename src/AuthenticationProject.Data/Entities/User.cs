namespace AuthenticationProject.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Shared.Projections;

    internal class User : IUser
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [DataType(DataType.EmailAddress), Required]
        public string EmailAddress { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        public string TwitterUserName { get; set; }
    }
}
