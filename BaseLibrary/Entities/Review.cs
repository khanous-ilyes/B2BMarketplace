using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    /// <summary>
    /// Représente un avis/évaluation laissé après une transaction
    /// </summary>
    public class Review : BaseEntity
    {
        [Required]
        public int ContactRequestId { get; set; }
        
        [ForeignKey("ContactRequestId")]
        public ContactRequest? ContactRequest { get; set; }

        // Qui a laissé l'avis (Client ou Supplier via UserId)
        [Required]
        public int ReviewerUserId { get; set; }
        
        [ForeignKey("ReviewerUserId")]
        public User? ReviewerUser { get; set; }

        // Qui reçoit l'avis
        [Required]
        public int ReviewedUserId { get; set; }
        
        [ForeignKey("ReviewedUserId")]
        public User? ReviewedUser { get; set; }

        // Note sur 5 étoiles
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        // Commentaire texte (optionnel)
        public string? Comment { get; set; }

        // Tags prédéfinis (flags)
        public ReviewTags Tags { get; set; } = ReviewTags.None;

        // Est-ce que la transaction a été réussie selon ce reviewer ?
        public bool IsTransactionSuccessful { get; set; } = true;

        // Type de reviewer pour faciliter les requêtes
        public bool IsFromClient { get; set; }  // true = Client a noté, false = Supplier a noté
    }
}
