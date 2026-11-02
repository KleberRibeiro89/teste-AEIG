namespace AEIG.Domain;

public class AvisoDuplicadoException : Exception
{
    public AvisoDuplicadoException()
        : base("Aviso de pagamento já processado.")
    {
    }
}
