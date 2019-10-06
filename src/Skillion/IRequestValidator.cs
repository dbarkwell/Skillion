namespace Skillion
{
    public interface IRequestValidator
    {
        bool IsApplicationVerified();

        bool IsCertificateVerified();
    }
}