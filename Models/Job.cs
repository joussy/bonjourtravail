namespace bonjourtravail_api.Models;

public class Job : Offre
{
    /// <summary>
    /// Vrai si l'offre a été crée par Bonjour-Travail, faux si elle vient de Pole Emploi 
    /// </summary>
    public bool Internal { get; set; } = false;
}