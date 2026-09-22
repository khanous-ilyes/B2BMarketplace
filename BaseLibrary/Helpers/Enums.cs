namespace BaseLibrary.Helpers
{
    public enum UserType
    {
        Supplier,
        Client,
        Admin
    }

    public enum UserStatus
    {
        
        Pending,
        Active,
        Suspended
    }

    public enum ArticleType
    {
        Product,
        Machine,
        Service
    }


    public enum SubscriptionStatus
    {
        Active,
        Expired,
        Cancelled,
        Pending,
        Rejected
    }

    public enum ArticleStatus
    {
        Draft,
        Published,
        Archived
    }

    public enum PriceType
    {
        Fixed,
        Hourly,
        Daily,
        Quote
    }


    public enum MediaType
    {
        Image,
        Video,
        Document
    }


    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

    public enum ConversationStatus
    {
        Open,
        Closed,
        Archived
    }

    public enum QuoteRequestStatus
    {
        Pending,
        Quoted,
        Accepted,
        Rejected
    }

    // Contact Request Status Flow:
    // Pending -> Accepted/Rejected
    // Accepted -> InProgress -> Completed/Cancelled
    public enum ContactRequestStatus
    {
        Pending,      // Client a envoyé une demande
        Accepted,     // Fournisseur a accepté, infos partagées
        Rejected,     // Fournisseur a refusé
        InProgress,   // Transaction en cours
        Completed,    // Transaction terminée
        Cancelled     // Annulée par l'une des parties
    }

    // Statut de vérification fournisseur (Badge de confiance)
    public enum VerificationStatus
    {
        None,           // Pas encore demandé
        Pending,        // Demande en attente
        UnderReview,    // En cours de vérification par l'admin
        Approved,       // Vérifié ✓
        Rejected        // Refusé
    }

    // Tags prédéfinis pour les avis
    [Flags]
    public enum ReviewTags
    {
        None = 0,
        Professional = 1,       // Professionnel
        Responsive = 2,         // Réactif
        GoodQuality = 4,        // Bonne qualité
        GoodPrice = 8,          // Bon prix
        Reliable = 16,          // Fiable
        Recommended = 32,       // Je recommande
        Slow = 64,              // Lent
        Aggressive = 128,       // Agressif
        BadCommunication = 256, // Mauvaise communication
        NotRecommended = 512    // Je ne recommande pas
    }
}

