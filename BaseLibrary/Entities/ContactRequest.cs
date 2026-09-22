using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    /// <summary>
    /// Représente une demande de contact entre un Client et un Fournisseur
    /// pour un article spécifique
    /// </summary>
    public class ContactRequest : BaseEntity
    {
        [Required]
        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [Required]
        public int SupplierId { get; set; }
        
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        [Required]
        public int ArticleId { get; set; }
        
        [ForeignKey("ArticleId")]
        public Article? Article { get; set; }

        // Message initial du client
        [Required]
        public string Message { get; set; } = string.Empty;

        // Réponse du fournisseur (si accepté/refusé avec message)
        public string? ResponseMessage { get; set; }

        // Statut de la demande
        [Required]
        public ContactRequestStatus Status { get; set; } = ContactRequestStatus.Pending;

        // Quantité demandée (optionnel)
        public int? Quantity { get; set; }

        // Dates importantes
        public DateTime? RespondedAt { get; set; }      // Quand le fournisseur a répondu
        public DateTime? StartedAt { get; set; }        // Quand la transaction a commencé
        public DateTime? CompletedAt { get; set; }      // Quand la transaction est terminée

        // Confirmation de fin par les deux parties
        public bool ClientConfirmedCompletion { get; set; } = false;
        public bool SupplierConfirmedCompletion { get; set; } = false;

        // Résultat de la transaction
        public bool? IsSuccessful { get; set; }  // null = pas encore terminé

        // Informations de contact partagées (après acceptation)
        public bool ContactInfoShared { get; set; } = false;

        // Navigation
        public List<Review>? Reviews { get; set; }
    }
}
